using GeoViewSE_Linnaeus.Analysis;
using Microsoft.VisualBasic;
using Plotly.NET;
using Plotly.NET.LayoutObjects;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Statistics;
using SkiaSharp;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using static GeoViewSE_Linnaeus.analysis.AnalysisiEngine.AnalysisEngine;
using Brushes = System.Drawing.Brushes;


namespace GeoViewSE_Linnaeus.analysis
    {
        public class AnalysisiEngine
        {
            public class AnalysisEngine
            {
            // DECLARATIONS
            bool IsTimeDependent(string name) =>
name == "Temperature" || name == "Wind" || name == "Humidity";

            public enum VariableKind
                {
                    Constant,      // t.ex. geokemi, litologi-booleaner
                    Continuous,    // t.ex. temp, nederbörd, vind, densitet
                    Boolean,       // t.ex. IsGrundvattenMagasin, IsMoran
                    Distance       // t.ex. DistCoast, DistRiver, DistSmhiWeather
                }
            private readonly List<(string X, string Y)> _interactionPairs = new()
{
    ("Temperature", "Humidity"),
    ("Temperature", "DistCoast"),
    ("Humidity", "DistCoast"),
    ("Wind", "DirCoast"),
    ("Wind", "Humidity"),
    ("SoilDepthMeters", "DistRidge"),
    //("Humidity", "DistRiver"),
    ("DirCoast", "DistCoast"),
    ("HarvestHostvete", "SoilDepthMeters"),
    ("HarvestHavre", "Humidity"),
    ("HarvestSlattervallTotal", "Temperature"),
    ("Temperature", "Lat"),
    ("Humidity", "Lat"),
    ("Wind", "Lat"),
    ("Fe2O3", "NiPpm"),
    ("NiPpm", "CrPpm"),
    ("IsMoran", "IsLera"),
    ("IsLera", "Al2O3"),
    ("SiO2", "Fe2O3")
};
            private readonly Dictionary<string, double> _densityCache = new();


            public class VariableMeta
                {
                    public string Name { get; set; } = "";
                    public VariableKind Kind { get; set; }
                    public string? DistancePartner { get; set; }  // t.ex. "DistSmhiWeather"
                    public bool IsTarget { get; set; } = false;
                }

                // RETURN
                public class VariableResult
                {
                    public string Name { get; set; } = "";
                    public double? ExplanationDegree { get; set; }        // t.ex. R² eller korrelation
                    public double? DistanceLossCorrelation { get; set; }
                    public Func<double, double>? DistanceModel { get; set; } // f(d) → förklaringsgrad
                    public double? Mean { get; set; }
                    public double? StdDev { get; set; }
                    public double? DistanceModelA { get; set; }
                    public double? DistanceModelK { get; set; }
                    public double? AsymptoticCorrelation { get; set; } //Den asymptotiska, brusfria, riktade korrelationen mellan variabeln och densiteten.
                    public double? AsymptoticCI_Low { get; set; }
                    public double? AsymptoticCI_High { get; set; }
                    public double? SampleCorrelation { get; set; }        // NY: empirisk r
                    public double? BootstrapR_StdDev { get; set; }        // NY: ISD-mått

            }

            // CALCULUS
            public class VariableMatrix
                {
                    public List<double> Values { get; } = new();
                    public List<double>? Distances { get; set; }  // null om ingen distanspartner
                    public List<double> GlobalDensity { get; } = new();
                    public List<string> PointIds { get; } = new();   // ⭐ NYTT
            }


            // READ
            private readonly List<Dictionary<string, object?>> _rows;

                public AnalysisEngine(List<Dictionary<string, object?>> rows)
                {
                    _rows = rows;
                }

                // ANALYSE
                public void Run()
                {
                    foreach (var v in GeoViewSE_Linnaeus.Analysis.VariableCatalog.Variables)
                    {
                        var result = AnalyzeVariable(v);
                        // spara i lista, skriv logg, etc.
                    }
                }
public List<VariableResult> RunAndReturnResults(Action<int, int>? progress = null)
{
    var list = new List<VariableResult>();
    var vars = GeoViewSE_Linnaeus.Analysis.VariableCatalog.Variables;
    int total = vars.Count;
    int index = 0;

    foreach (var v in vars)
    {
        var result = AnalyzeVariable(v);
        list.Add(result);

        index++;
        progress?.Invoke(index, total);
    }

    return list;
}
            public static void ExportResultsToCsv(List<VariableResult> results, string path)
            {
                var sb = new StringBuilder();

                // Header
                sb.AppendLine("Name;ExplanationDegree (R2); SampleCorrelation (r);DistanceLossCorrelation;Mean;StdDev;ModelA;ModelK;Fisher-Z-corr (r);Fzc-CI-low;Fzc-CI-high;BootstrapR_StdDev");

                foreach (var r in results)
                {
                    // Hoppa över distanspartners (DistCoast, DistRiver, DistSmhiWeather etc.)
                    if (r.Name.StartsWith("Dist"))
                        continue;

                    string line = string.Join(";",
                        r.Name,
                        r.ExplanationDegree?.ToString() ?? "",
                        r.SampleCorrelation?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.DistanceLossCorrelation?.ToString() ?? "",
                        r.Mean?.ToString() ?? "",
                        r.StdDev?.ToString() ?? "",
                        r.DistanceModelA?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.DistanceModelK?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.AsymptoticCorrelation?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.AsymptoticCI_Low?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.AsymptoticCI_High?.ToString(CultureInfo.InvariantCulture) ?? "",
                        r.BootstrapR_StdDev?.ToString(CultureInfo.InvariantCulture) ?? ""
                    );

                    sb.AppendLine(line);
                }

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }



            private VariableResult AnalyzeVariable(VariableMeta meta)
                {
                var matrix = BuildVariableMatrix(meta);


                if (matrix.Values.Count == 0)
                    return new VariableResult { Name = meta.Name };
                double explanation = 0;
                double? mean = null;
                double? sd = null;
                double? distanceLossCorrelation = null;
                Func<double, double>? distanceModel = null;
                double? distanceModelA = null;
                double? distanceModelK = null;
                double? asymptoticR = null;
                double? ciLow = null;
                double? ciHigh = null;
                double? sampleR = null;
                double? bootstrapRStd = null;

                switch (meta.Kind)
                {
                    case VariableKind.Boolean:
                        // Boolean: hur stor andel är true?
                        // Values innehåller 0 eller 1
                        double trueCount = 0;
                        foreach (var v in matrix.Values)
                            if (v == 1) trueCount++;

                        explanation = trueCount / matrix.Values.Count;
                        break;

                    case VariableKind.Constant:
                    case VariableKind.Continuous:
                        // Medelvärde och standardavvikelse
                        var m = Mean(matrix.Values);
                        var s = StdDev(matrix.Values, m);


                        mean = m;
                        sd = s;
                        // Förklaringsgrad:
                        double r = IsTimeDependent(meta.Name)
    ? CorrelationTimeAware(matrix.Values, matrix.GlobalDensity, matrix.PointIds)
    : Correlation(matrix.Values, matrix.GlobalDensity);

                        explanation = r * r;
                        sampleR = r;
                        // Bootstrap: inherent sampling dependency
                        var (bootMean, bootStd) = BootstrapCorrelation(matrix.Values, matrix.GlobalDensity, 200);
                        bootstrapRStd = bootStd;
                        // --- Fisher r-baserad korrelation + konfidensintervall ---
                        int n;

                        // 1) Välj r och n beroende på time-aware eller ej
                        double r2;
                        if (IsTimeDependent(meta.Name))
                        {
                            // r och n från första tidssteget
                            (r2, n) = CorrelationTimeAwareFirstStep(matrix.Values, matrix.GlobalDensity, matrix.PointIds);
                        }
                        else
                        {
                            r2 = r;
                            n = matrix.Values.Count;
                        }

                        // 2) Om för få punkter → inga CI
                        if (n < 4)
                        {
                            asymptoticR = null;
                            ciLow = null;
                            ciHigh = null;
                        }
                        else
                        {
                            // Fisher-z på r
                            double zr = 0.5 * Math.Log((1 + r) / (1 - r));

                            double se = 1.0 / Math.Sqrt(n - 3);
                            double zLow = zr - 1.96 * se;
                            double zHigh = zr + 1.96 * se;

                            // transformera tillbaka
                            ciLow = (Math.Exp(2 * zLow) - 1) / (Math.Exp(2 * zLow) + 1);
                            ciHigh = (Math.Exp(2 * zHigh) - 1) / (Math.Exp(2 * zHigh) + 1);

                            // tills vidare: ingen separat "asymptoticR"
                            asymptoticR = null;
                        }

                        if (meta.DistancePartner != null && matrix.Distances != null && matrix.Distances.Count == matrix.Values.Count)
                        {
                            // 1. Beräkna medelvärde
                            double mea = Mean(matrix.Values);

                            // 2. Bygg dev-lista
                            var dev = new List<double>(matrix.Values.Count);
                            for (int i = 0; i < matrix.Values.Count; i++)
                                dev.Add(Math.Abs(matrix.Values[i] - mea));

                            // 3. Korrelation mellan dev och distans
                            double rDist = Correlation(dev, matrix.Distances);

                            // 4. Spara som “distance loss” (tolkning: hur starkt avvikelsen växer med distans)
                            distanceLossCorrelation = rDist; // eller Math.Abs(rDist)

                            // 5. Exponential fit
                            var (A, k) = FitExponential(dev, matrix.Distances);

                            // 6. Spara modellen
                            distanceModel = d => A * Math.Exp(k * d);

                            distanceModelA = A;
                            distanceModelK = k;
                        }
                        SaveHexbinPlot(meta.Name, matrix.Values, matrix.GlobalDensity, "HexbinPlots");

                        break;

                    case VariableKind.Distance:
                        // Distansvariabler analyseras senare (distansmodell)
                        explanation = 0;
                        break;
                }

                return new VariableResult
                {
                    Name = meta.Name,
                    ExplanationDegree = explanation,
                    DistanceLossCorrelation = distanceLossCorrelation,
                    DistanceModel = distanceModel,
                    DistanceModelA = distanceModelA,
                    DistanceModelK = distanceModelK,
                    Mean = mean,
                    StdDev = sd,
                    AsymptoticCorrelation = asymptoticR,
                    AsymptoticCI_Low = ciLow,
                    AsymptoticCI_High = ciHigh,
                    SampleCorrelation = sampleR,
                    BootstrapR_StdDev = bootstrapRStd
                };

            }

            private VariableMatrix BuildVariableMatrix(VariableMeta meta)
            {
                var m = new VariableMatrix();
                var seenPointIds = new HashSet<string>();

                foreach (var row in _rows)
                {
                    // Hämta PointId
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;

                    string pid = pidObj?.ToString() ?? "";

                    // CONSTANT → hoppa över om vi redan sett denna punkt
                    if (meta.Kind == VariableKind.Constant && seenPointIds.Contains(pid))
                        continue;

                    if (meta.Kind == VariableKind.Constant)
                        seenPointIds.Add(pid);

                    // Hämta variabelvärdet
                    if (!row.TryGetValue(meta.Name, out var raw))
                        continue;

                    if (raw == null || raw is DBNull)
                        continue;

                    if (!TryToDouble(raw, out double value))
                        continue;

                    m.Values.Add(value);

                    // Hämta Lon/Lat (behövs för densitet)
                    if (!row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj))
                        continue;

                    if (!TryToDouble(lonObj, out double lon) ||
                        !TryToDouble(latObj, out double lat))
                        continue;

                    // GLOBAL DENSITY
                    //double density = ComputeGlobalDensity(lon, lat); original
                    double density = GetOrComputeGlobalDensityForPoint(pid, lon, lat);
                    m.GlobalDensity.Add(density);

                    //pId
                    m.PointIds.Add(pid);

                    // Distanspartner
                    if (meta.DistancePartner != null)
                    {
                        if (row.TryGetValue(meta.DistancePartner, out var rawDist) &&
                            rawDist != null && !(rawDist is DBNull) &&
                            TryToDouble(rawDist, out double distVal))
                        {
                            m.Distances ??= new List<double>();
                            m.Distances.Add(distVal);
                        }
                    }
                }

                return m;
            }


            private bool TryToDouble(object raw, out double value)
                {
                    switch (raw)
                    {
                        case double d:
                            value = d; return true;
                        case float f:
                            value = f; return true;
                        case int i:
                            value = i; return true;
                        case long l:
                            value = l; return true;
                        case string s when double.TryParse(
                            s,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var parsed):
                            value = parsed; return true;
                        default:
                            value = 0; return false;
                    }
                }
            private double Mean(List<double> values)
            {
                if (values.Count == 0) return double.NaN;
                double sum = 0;
                foreach (var v in values) sum += v;
                return sum / values.Count;
            }

            private double StdDev(List<double> values, double mean)
            {
                if (values.Count < 2) return 0;
                double sumSq = 0;
                foreach (var v in values)
                    sumSq += (v - mean) * (v - mean);
                return Math.Sqrt(sumSq / (values.Count - 1));
            }
            private double ComputeGlobalDensity(double lon, double lat)
            {
                var dists = new List<double>();
                var seen = new HashSet<string>(); // unika PointId för jämförelse
                                                  // Hämta PointId

                foreach (var row in _rows)
                {
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;

                    string pid = pidObj?.ToString() ?? "";

                    // ⭐ NYTT: hoppa över om vi redan räknat denna punkt
                    if (seen.Contains(pid))
                        continue;

                    seen.Add(pid);

                    if (!row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj))
                        continue;

                    if (!TryToDouble(lonObj, out double lon2) ||
                        !TryToDouble(latObj, out double lat2))
                        continue;

                    double dx = lon - lon2;
                    double dy = lat - lat2;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist > 0)
                        dists.Add(dist);
                }

                if (dists.Count == 0)
                    return 0;

                // GLOBAL densitet = medelavstånd till ALLA punkter
                double sum = 0;
                foreach (var d in dists)
                    sum += d;

                double meanDist = sum / dists.Count;
                double eps = 1e-6;

                return 1.0 / (eps + meanDist);
            }
            private double Correlation(List<double> x, List<double> y)
            {
                int n = Math.Min(x.Count, y.Count);
                if (n < 2) return 0;

                double meanX = Mean(x);
                double meanY = Mean(y);

                double sumXY = 0;
                double sumXX = 0;
                double sumYY = 0;

                for (int i = 0; i < n; i++)
                {
                    double dx = x[i] - meanX;
                    double dy = y[i] - meanY;

                    sumXY += dx * dy;
                    sumXX += dx * dx;
                    sumYY += dy * dy;
                }

                if (sumXX == 0 || sumYY == 0)
                    return 0;

                return sumXY / Math.Sqrt(sumXX * sumYY);
            }
            private (double A, double k) FitExponential(List<double> dev, List<double> dist)
            {
                var y = new List<double>();
                var x = dist;

                for (int i = 0; i < dev.Count; i++)
                {
                    if (dev[i] > 0)
                        y.Add(Math.Log(dev[i]));
                    else
                        y.Add(Math.Log(1e-6)); // skydd
                }

                double meanX = Mean(x);
                double meanY = Mean(y);

                double sumXY = 0;
                double sumXX = 0;

                for (int i = 0; i < x.Count; i++)
                {
                    double dx = x[i] - meanX;
                    double dy = y[i] - meanY;

                    sumXY += dx * dy;
                    sumXX += dx * dx;
                }
                if (sumXX == 0)
                {
                    // Alla distanser är identiska → ingen lutning kan beräknas
                    return (A: 0, k: 0);
                }
                double k = sumXY / sumXX;
                double alpha = meanY - k * meanX;
                double A = Math.Exp(alpha);

                return (A, k);
            }
            private (double meanR, double sdR) BootstrapCorrelation(List<double> x, List<double> y, int B = 200)
            {
                int n = Math.Min(x.Count, y.Count);
                if (n < 2 || B <= 1)
                    return (double.NaN, double.NaN);

                var rnd = new Random(12345); // ev. gör seed konfigurerbart
                var rs = new List<double>(B);

                for (int b = 0; b < B; b++)
                {
                    var bx = new List<double>(n);
                    var by = new List<double>(n);

                    for (int i = 0; i < n; i++)
                    {
                        int idx = rnd.Next(n); // [0, n)
                        bx.Add(x[idx]);
                        by.Add(y[idx]);
                    }

                    double rb = Correlation(bx, by);
                    rs.Add(rb);
                }

                double meanR = Mean(rs);
                double sdR = StdDev(rs, meanR);
                return (meanR, sdR);
            }

            private void SaveHexbinPlot(string varName, List<double> values, List<double> density, string folder)
            {
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, $"hexbin_{varName}.html");

                var chart = Chart2D.Chart.Histogram2D<double, double, IEnumerable<double>, double>(
                    X: values,
                    Y: density,
                    NBinsX: 100,
                    NBinsY: 100,
                    ColorScale: StyleParam.Colorscale.Viridis
                )
                .WithTitle($"Densitetsplot: {varName}")
                .WithYAxisStyle(title: Title.init("Global densitet"))
                .WithXAxisStyle(title: Title.init($"{varName} (värde)"));

            chart.SaveHtml(path);
            }



            public (double alpha, double beta, double gamma) InteractionTriple(
                List<double> x, List<double> y, List<double> z)
            {
                int n = x.Count;
                if (n == 0) return (0, 0, 0);

                double meanX = x.Average();
                double meanY = y.Average();
                double meanZ = z.Average();

                double sdX = StdDev(x, meanX);
                double sdY = StdDev(y, meanY);
                double sdZ = StdDev(z, meanZ);

                double A = 0, B = 0, C = 0;

                for (int i = 0; i < n; i++)
                {
                    double xp = (x[i] - meanX) / sdX;
                    double yp = (y[i] - meanY) / sdY;
                    double zp = (z[i] - meanZ) / sdZ;

                    double signZ = Math.Sign(zp);
                    double signX = Math.Sign(xp);

                    double a = Math.Abs(zp - xp) * signZ;
                    double b = Math.Abs(zp - yp) * signZ;
                    double c = Math.Abs(xp - yp) * signX;

                    A += a;
                    B += b;
                    C += c;
                }

                return (A / n, B / n, C / n);
            }

            public double CorrXY(List<double> x, List<double> y)
            {
                return Correlation(x, y);
            }
            private List<double> ExtractVariableValues(string varName)
            {
                var list = new List<double>();

                foreach (var row in _rows)
                {
                    if (row.TryGetValue(varName, out var raw) &&
                        raw != null && !(raw is DBNull) &&
                        TryToDouble(raw, out double v))
                    {
                        list.Add(v);
                    }
                }

                return list;
            }
            private List<double> ExtractDensityValues()
            {
                var list = new List<double>();

                foreach (var row in _rows)
                {
                    // Hämta PointId
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;

                    string pid = pidObj?.ToString() ?? "";

                    // Hämta Lon/Lat
                    if (!row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj))
                        continue;

                    if (!TryToDouble(lonObj, out double lon) ||
                        !TryToDouble(latObj, out double lat))
                        continue;

                    // Använd cache
                    double d = GetOrComputeGlobalDensityForPoint(pid, lon, lat);
                    list.Add(d);
                }

                return list;
            }

            public List<(string X, string Y, double CorrXY, double Alpha, double Beta, double Gamma, double Beff)>
        RunInteractionAnalysis()
            {
                var results = new List<(string, string, double, double, double, double, double)>();

                var (x1, y1, z1, pids2) = ExtractAlignedXYZ("Temperature", "Temperature");
                double bEff = ComputeDeltaZ(x1, z1, pids2);
                double Nsd = ComputeNSD();
                double Roundness = MeasureDistributionShape();
                ExportNsdAndShape(Nsd, Roundness, @"C:\Networks\NSD_SHAPE.txt");
                ExportNsdAndRoundnessPlots_SystemDraw();


                foreach (var (xName, yName) in _interactionPairs)
                {
                    var (x, y, z, pids) = ExtractAlignedXYZ(xName, yName);

                    // säkerhet: hoppa över om för få datapunkter
                    if (x.Count < 2 || y.Count < 2 || z.Count < 2)
                        continue;

                    double corrXY = CorrXY(x, y);
                    //var (alpha, beta, gamma) = InteractionTriple(x, y, z);
                    var (alpha, beta, gamma) =
    (IsTimeDependent(xName) || IsTimeDependent(yName))
    ? InteractionTripleTimeAware(x, y, z, pids)
    : InteractionTriple(x, y, z);

                    results.Add((xName, yName, corrXY, alpha, beta, gamma, bEff));
                }
                ExportAssociativeNetwork(
                    results,
                    @"C:\Networks\species_network.png");
                var bn = RunBayesNetHillClimbing();
                ExportBayesNetDag(bn, @"C:\Networks\bayes_network.png");


                return results;
            }

            public void ExportInteractionResults(
                List<(string X, string Y, double CorrXY, double Alpha, double Beta, double Gamma, double Beff)> results,
                string path)
            {
                var sb = new StringBuilder();
                sb.AppendLine("VarX;VarY;CorrXY;Alpha;Beta;Gamma;Beff");

                foreach (var r in results)
                {
                    sb.AppendLine($"{r.X};{r.Y};{r.CorrXY};{r.Alpha};{r.Beta};{r.Gamma};{r.Beff}");
                }
                sb.AppendLine();

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            private (List<double> X, List<double> Y, List<double> Z, List<string> Pids)
            ExtractAlignedXYZ(string xName, string yName)
            {
                var X = new List<double>();
                var Y = new List<double>();
                var Z = new List<double>();
                var P = new List<string>();

                bool xIsConst = VariableCatalog.GetMeta(xName).Kind == VariableKind.Constant;
                bool yIsConst = VariableCatalog.GetMeta(yName).Kind == VariableKind.Constant;

                var seen = new HashSet<string>();

                foreach (var row in _rows)
                {
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;

                    string pid = pidObj?.ToString() ?? "";

                    // ⭐ NYTT: constant–constant → använd bara första förekomsten per punkt
                    if (xIsConst && yIsConst)
                    {
                        if (seen.Contains(pid))
                            continue;

                        seen.Add(pid);
                    }

                    if (!row.TryGetValue(xName, out var rawX) ||
                        !row.TryGetValue(yName, out var rawY) ||
                        !row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj))
                        continue;

                    if (!TryToDouble(rawX, out double xv)) continue;
                    if (!TryToDouble(rawY, out double yv)) continue;
                    if (!TryToDouble(lonObj, out double lon)) continue;
                    if (!TryToDouble(latObj, out double lat)) continue;

                    //double density = ComputeGlobalDensity(lon, lat); original
                    double density = GetOrComputeGlobalDensityForPoint(pid, lon, lat);

                    X.Add(xv);
                    Y.Add(yv);
                    Z.Add(density);
                    P.Add(pid);
                }

                return (X, Y, Z, P);
            }
            private double GetOrComputeGlobalDensityForPoint(string pointId, double lon, double lat)
            {
                if (_densityCache.TryGetValue(pointId, out var cached))
                    return cached;

                double d = ComputeGlobalDensity(lon, lat);
                _densityCache[pointId] = d;
                return d;
            }
            public double ComputeDeltaZ(List<double> temps, List<double> dens, List<string> pids)
            {
                var groups = new Dictionary<string, (double t, double d)>();

                for (int i = 0; i < temps.Count; i++)
                {
                    string pid = pids[i];

                    // ta bara första förekomsten per PID
                    if (!groups.ContainsKey(pid))
                        groups[pid] = (temps[i], dens[i]);
                }

                var tList = new List<double>();
                var dList = new List<double>();

                foreach (var kv in groups.Values)
                {
                    tList.Add(kv.t);
                    dList.Add(kv.d);
                }

                if (tList.Count < 2)
                    return double.NaN;

                double r = Correlation(tList, dList);

                double meanT = Mean(tList);
                double meanD = Mean(dList);

                double sdT = StdDev(tList, meanT);
                double sdD = StdDev(dList, meanD);

                if (sdT == 0)
                    return double.NaN;

                return r * (sdD / sdT);
            }


            public class AssocNode
            {
                public string Name { get; set; } = "";

                public double X { get; set; }
                public double Y { get; set; }

                public List<AssocEdge> Edges { get; set; } = new();

                public System.Drawing.Color Color { get; set; }
            }
            public class AssocEdge
            {
                public AssocNode A { get; set; }
                public AssocNode B { get; set; }

                public double Corr { get; set; }

                public double Alpha { get; set; }
                public double Beta { get; set; }
                public double Gamma { get; set; }
            }
            private System.Drawing.Color RandomColor(Random rnd)
            {
                return System.Drawing.Color.FromArgb(
                    rnd.Next(80, 255),
                    rnd.Next(80, 255),
                    rnd.Next(80, 255));
            }

            public void ExportAssociativeNetwork(
                List<(string X, string Y, double CorrXY, double Alpha, double Beta, double Gamma, double bEff)> results,
                string outputPath)
            {
                int width = 1600;
                int height = 1200;
                Random rnd = new Random();

                // 1. DATA-SETUP (Samma som förut)
                var nodes = new Dictionary<string, AssocNode>();
                foreach (var r in results)
                {
                    if (!nodes.ContainsKey(r.X)) nodes[r.X] = new AssocNode { Name = r.X, Color = RandomColor(rnd) };
                    if (!nodes.ContainsKey(r.Y)) nodes[r.Y] = new AssocNode { Name = r.Y, Color = RandomColor(rnd) };
                }

                var edges = new List<AssocEdge>();
                foreach (var r in results)
                {
                    var edge = new AssocEdge { A = nodes[r.X], B = nodes[r.Y], Corr = r.CorrXY, Alpha = r.Alpha, Beta = r.Beta, Gamma = r.Gamma };
                    edges.Add(edge);
                    nodes[r.X].Edges.Add(edge);
                    nodes[r.Y].Edges.Add(edge);
                }

                // 2. POSITIONERING (Circle Layout)
                int nodeCount = nodes.Count;
                double centerX = width / 2.0;
                double centerY = height / 2.0;
                double radius = Math.Min(width, height) * 0.35;
                int idx = 0;
                foreach (var n in nodes.Values)
                {
                    double angle = 2.0 * Math.PI * idx / nodeCount;
                    n.X = centerX + radius * Math.Cos(angle);
                    n.Y = centerY + radius * Math.Sin(angle);
                    idx++;
                }

                // 3. RENDERING (Lager-baserad)
                using var bmp = new Bitmap(width, height);
                using var g = Graphics.FromImage(bmp);
                g.Clear(System.Drawing.Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var edgeFont = new System.Drawing.Font("Segoe UI", 9);
                var nodeFont = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // LAGER 1: RITA BARA LINJERNA
                using (var edgePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Gray), 1))
                {
                    foreach (var e in edges)
                    {
                        g.DrawLine(edgePen, (float)e.A.X, (float)e.A.Y, (float)e.B.X, (float)e.B.Y);
                    }
                }

                // LAGER 2: RITA BARA NODERNA (Cirklarna)
                float rSize = 30;
                foreach (var n in nodes.Values)
                {
                    using var brush = new SolidBrush(n.Color);
                    g.FillEllipse(brush, (float)n.X - rSize, (float)n.Y - rSize, rSize * 2, rSize * 2);
                    g.DrawEllipse(Pens.Black, (float)n.X - rSize, (float)n.Y - rSize, rSize * 2, rSize * 2);
                }

                // LAGER 3: RITA ALL TEXT ÖVERST
                // Rita först kant-texterna
                foreach (var e in edges)
                {
                    float mx = (float)(e.A.X + e.B.X) / 2f;
                    float my = (float)(e.A.Y + e.B.Y) / 2f;
                    string txt = $"r={e.Corr:F2}\n({e.Alpha:F2},{e.Beta:F2},{e.Gamma:F2})";

                    var textSize = g.MeasureString(txt, edgeFont);
                    // Vit bakgrundsplatta för att texten ska "poppa"
                    g.FillRectangle(Brushes.White, mx - textSize.Width / 2, my - textSize.Height / 2, textSize.Width, textSize.Height);
                    g.DrawString(txt, edgeFont, Brushes.Black, mx, my, sf);
                }

                // Rita sedan nodernas namn
                foreach (var n in nodes.Values)
                {
                    double angle = Math.Atan2(n.Y - centerY, n.X - centerX);
                    float tx = (float)(n.X + (rSize + 12) * Math.Cos(angle));
                    float ty = (float)(n.Y + (rSize + 12) * Math.Sin(angle));

                    var nodeSf = new StringFormat();
                    // Dynamisk justering: text till höger om cirkeln är Near, till vänster Far
                    nodeSf.Alignment = Math.Cos(angle) > 0 ? StringAlignment.Near : StringAlignment.Far;
                    nodeSf.LineAlignment = StringAlignment.Center;

                    g.DrawString(n.Name, nodeFont, Brushes.Black, tx, ty, nodeSf);
                }

                // 4. SPARA
                string? dir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            public class BayesNetResult
            {
                public List<string> Variables { get; set; } = new();
                public List<Dictionary<string, string>> Rows { get; set; } = new();
                public List<(string From, string To)> Edges { get; set; } = new();
                // NYTT:
                public Dictionary<string, CptNode> Cpts { get; set; } = new();
                public List<EdgeInfo> EdgeInfos { get; set; } = new();
            }
            private (double p33, double p66) ComputeTertiles(List<double> values)
            {
                var sorted = values
                    .Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
                    .OrderBy(v => v)
                    .ToList();

                if (sorted.Count == 0)
                    return (double.NaN, double.NaN);

                double P(double p)
                {
                    double idx = (sorted.Count - 1) * p;
                    int i0 = (int)Math.Floor(idx);
                    int i1 = (int)Math.Ceiling(idx);
                    if (i0 == i1) return sorted[i0];
                    double t = idx - i0;
                    return sorted[i0] * (1 - t) + sorted[i1] * t;
                }

                double p33 = P(0.33);
                double p66 = P(0.66);
                return (p33, p66);
            }

            private string DiscretizeToTertile(double value, double p33, double p66)
            {
                if (double.IsNaN(value) || double.IsNaN(p33) || double.IsNaN(p66))
                    return "Missing";

                if (value <= p33) return "Low";
                if (value <= p66) return "Mid";
                return "High";
            }
            public BayesNetResult BuildBayesNetData()
            {
                var result = new BayesNetResult();

                // 1. Samla variabelnamn från interactionPairs
                var varNames = new HashSet<string>();
                foreach (var (X, Y) in _interactionPairs)
                {
                    varNames.Add(X);
                    varNames.Add(Y);
                }

                // Lägg till densitet som målvariabel
                varNames.Add("GlobalDensity");

                result.Variables = varNames.ToList();

                // 2. Extrahera kontinuerliga värden per variabel
                var valueMap = new Dictionary<string, List<double>>();
                foreach (var name in result.Variables)
                    valueMap[name] = new List<double>();

                // Vi bygger rader genom att gå över _rows
                foreach (var row in _rows)
                {
                    // Hämta PointId, Lon, Lat för densitet
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;
                    string pid = pidObj?.ToString() ?? "";

                    if (!row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj) ||
                        !TryToDouble(lonObj, out double lon) ||
                        !TryToDouble(latObj, out double lat))
                        continue;

                    double density = GetOrComputeGlobalDensityForPoint(pid, lon, lat);

                    // Lägg in GlobalDensity
                    valueMap["GlobalDensity"].Add(density);

                    // Övriga variabler
                    foreach (var name in result.Variables)
                    {
                        if (name == "GlobalDensity")
                            continue;

                        if (row.TryGetValue(name, out var raw) &&
                            raw != null && !(raw is DBNull) &&
                            TryToDouble(raw, out double v))
                        {
                            valueMap[name].Add(v);
                        }
                        else
                        {
                            valueMap[name].Add(double.NaN);
                        }
                    }
                }

                // 3. Beräkna tertiler per variabel
                var tertiles = new Dictionary<string, (double p33, double p66)>();
                foreach (var name in result.Variables)
                {
                    var (p33, p66) = ComputeTertiles(valueMap[name]);
                    tertiles[name] = (p33, p66);
                }

                // 4. Bygg diskret rad-tabell
                int nRows = valueMap["GlobalDensity"].Count;
                for (int i = 0; i < nRows; i++)
                {
                    var rowDict = new Dictionary<string, string>();

                    foreach (var name in result.Variables)
                    {
                        double v = valueMap[name][i];
                        var (p33, p66) = tertiles[name];

                        string cat = DiscretizeToTertile(v, p33, p66);
                        rowDict[name] = cat;
                    }

                    result.Rows.Add(rowDict);
                }

                return result;
            }
            private class BayesNetInternal
            {
                public List<string> Variables { get; }
                public int[,] Data;          // [nRows, nVars], värden 0,1,2 (Low/Mid/High)
                public int VarCount => Variables.Count;
                public bool[,] Adj;          // Adj[i,j] = kant i -> j

                public BayesNetInternal(List<string> vars, int[,] data)
                {
                    Variables = vars;
                    Data = data;
                    Adj = new bool[vars.Count, vars.Count];
                }
            }
            private int StateIndex(string state)
            {
                return state switch
                {
                    "Low" => 0,
                    "Mid" => 1,
                    "High" => 2,
                    _ => 0
                };
            }
            private BayesNetInternal ToInternal(BayesNetResult bn)
            {
                int nVars = bn.Variables.Count;
                int nRows = bn.Rows.Count;

                var data = new int[nRows, nVars];

                for (int i = 0; i < nRows; i++)
                {
                    var row = bn.Rows[i];
                    for (int j = 0; j < nVars; j++)
                    {
                        string varName = bn.Variables[j];
                        string state = row[varName];
                        data[i, j] = StateIndex(state);
                    }
                }

                return new BayesNetInternal(bn.Variables, data);
            }
            private bool HasCycle(BayesNetInternal net)
            {
                int n = net.VarCount;
                var visited = new bool[n];
                var stack = new bool[n];

                bool Dfs(int v)
                {
                    visited[v] = true;
                    stack[v] = true;

                    for (int w = 0; w < n; w++)
                    {
                        if (!net.Adj[v, w]) continue;

                        if (!visited[w] && Dfs(w))
                            return true;
                        if (stack[w])
                            return true;
                    }

                    stack[v] = false;
                    return false;
                }

                for (int i = 0; i < n; i++)
                {
                    if (!visited[i] && Dfs(i))
                        return true;
                }

                return false;
            }
            private double ScoreNetwork(BayesNetInternal net)
            {
                int nVars = net.VarCount;
                int nRows = net.Data.GetLength(0);
                int r = 3; // antal tillstånd per variabel

                double logLik = 0.0;
                int kParams = 0;

                for (int j = 0; j < nVars; j++)
                {
                    // hitta föräldrar till j
                    var parents = new List<int>();
                    for (int i = 0; i < nVars; i++)
                        if (net.Adj[i, j])
                            parents.Add(i);

                    int q = (int)Math.Pow(r, parents.Count); // antal parent-kombinationer

                    // counts[ parentConfig, state ]
                    int[,] counts = new int[q, r];

                    for (int n = 0; n < nRows; n++)
                    {
                        int state = net.Data[n, j];

                        int parentIndex = 0;
                        for (int p = 0; p < parents.Count; p++)
                        {
                            int parentVar = parents[p];
                            int parentState = net.Data[n, parentVar];
                            parentIndex = parentIndex * r + parentState;
                        }

                        counts[parentIndex, state]++;
                    }

                    // log-likelihood med Laplace-smoothing
                    for (int parentConfig = 0; parentConfig < q; parentConfig++)
                    {
                        int N_ij = 0;
                        for (int s = 0; s < r; s++)
                            N_ij += counts[parentConfig, s];

                        if (N_ij == 0)
                            continue;

                        for (int s = 0; s < r; s++)
                        {
                            double N_ijk = counts[parentConfig, s] + 1.0; // Laplace
                            double denom = N_ij + r;
                            double p = N_ijk / denom;
                            logLik += (N_ijk - 1.0) * Math.Log(p);
                        }
                    }

                    // antal parametrar för denna nod
                    kParams += (r - 1) * q;
                }

                double bic = logLik - 0.5 * kParams * Math.Log(nRows);
                return bic;
            }
            public BayesNetResult RunBayesNetHillClimbing()
            {
                var bnData = BuildBayesNetData();
                var net = ToInternal(bnData);

                int n = net.VarCount;

                double bestScore = ScoreNetwork(net);
                bool improved = true;

                while (improved)
                {
                    improved = false;
                    double currentBest = bestScore;
                    int bestFrom = -1, bestTo = -1;
                    bool bestAdd = false, bestRemove = false, bestReverse = false;

                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i == j) continue;

                            // 1. Testa ADD i->j om ingen kant finns
                            if (!net.Adj[i, j])
                            {
                                net.Adj[i, j] = true;
                                if (!HasCycle(net))
                                {
                                    double s = ScoreNetwork(net);
                                    if (s > currentBest)
                                    {
                                        currentBest = s;
                                        bestFrom = i;
                                        bestTo = j;
                                        bestAdd = true;
                                        bestRemove = bestReverse = false;
                                    }
                                }
                                net.Adj[i, j] = false;
                            }

                            // 2. Testa REMOVE i->j om kant finns
                            if (net.Adj[i, j])
                            {
                                net.Adj[i, j] = false;
                                double s = ScoreNetwork(net);
                                if (s > currentBest)
                                {
                                    currentBest = s;
                                    bestFrom = i;
                                    bestTo = j;
                                    bestRemove = true;
                                    bestAdd = bestReverse = false;
                                }
                                net.Adj[i, j] = true;
                            }

                            // 3. Testa REVERSE i->j till j->i
                            if (net.Adj[i, j] && !net.Adj[j, i])
                            {
                                net.Adj[i, j] = false;
                                net.Adj[j, i] = true;

                                if (!HasCycle(net))
                                {
                                    double s = ScoreNetwork(net);
                                    if (s > currentBest)
                                    {
                                        currentBest = s;
                                        bestFrom = i;
                                        bestTo = j;
                                        bestReverse = true;
                                        bestAdd = bestRemove = false;
                                    }
                                }

                                net.Adj[j, i] = false;
                                net.Adj[i, j] = true;
                            }
                        }
                    }

                    if (currentBest > bestScore && (bestAdd || bestRemove || bestReverse))
                    {
                        bestScore = currentBest;
                        improved = true;

                        if (bestAdd)
                            net.Adj[bestFrom, bestTo] = true;
                        else if (bestRemove)
                            net.Adj[bestFrom, bestTo] = false;
                        else if (bestReverse)
                        {
                            net.Adj[bestFrom, bestTo] = false;
                            net.Adj[bestTo, bestFrom] = true;
                        }
                    }
                }

                // Fyll Edges i bnData
                bnData.Edges.Clear();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (net.Adj[i, j])
                        {
                            string from = net.Variables[i];
                            string to = net.Variables[j];
                            bnData.Edges.Add((from, to));
                        }
                    }
                }
                // NYTT: beräkna CPT + MI
                ComputeCpts(bnData);
                ComputeEdgeMutualInformation(bnData);
                ExportCptsToCsv(bnData, @"C:\Networks\cpt_tables.csv"); 

                return bnData;
            }
            public Dictionary<string, double> QueryDensityDistribution(
                BayesNetResult bn,
                Dictionary<string, string> evidence)
            {
                string target = "GlobalDensity";

                int matchCount = 0;
                var counts = new Dictionary<string, int>
                {
                    ["Low"] = 0,
                    ["Mid"] = 0,
                    ["High"] = 0
                };

                foreach (var row in bn.Rows)
                {
                    bool ok = true;
                    foreach (var kv in evidence)
                    {
                        if (!row.TryGetValue(kv.Key, out var val))
                        {
                            ok = false;
                            break;
                        }
                        if (val != kv.Value)
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (!ok) continue;

                    if (!row.TryGetValue(target, out var densState))
                        continue;

                    if (!counts.ContainsKey(densState))
                        counts[densState] = 0;

                    counts[densState]++;
                    matchCount++;
                }

                var result = new Dictionary<string, double>
                {
                    ["Low"] = 0,
                    ["Mid"] = 0,
                    ["High"] = 0
                };

                if (matchCount == 0)
                    return result;

                foreach (var k in counts.Keys.ToList())
                {
                    result[k] = counts[k] / (double)matchCount;
                }

                return result;
            }
            public void ExportBayesNetDag(BayesNetResult bn, string pathPng, int width = 1200, int height = 800)
            {
                var vars = bn.Variables;
                var edges = bn.Edges;

                using var bmp = new Bitmap(width, height);
                using var g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(System.Drawing.Color.White);

                int n = vars.Count;
                float cx = width / 2f;
                float cy = height / 2f;
                float radius = Math.Min(width, height) * 0.35f;

                var positions = new Dictionary<string, PointF>();

                var miLookup = bn.EdgeInfos
    .ToDictionary(e => (e.From, e.To), e => e.MutualInformation);


                for (int i = 0; i < n; i++)
                {
                    double angle = 2 * Math.PI * i / n - Math.PI / 2;
                    float x = cx + (float)(radius * Math.Cos(angle));
                    float y = cy + (float)(radius * Math.Sin(angle));
                    positions[vars[i]] = new PointF(x, y);
                }

                using var font = new System.Drawing.Font("Segoe UI", 10);
                using var nodeBrush = Brushes.LightBlue;
                using var nodePen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 2);
                using var textBrush = Brushes.Black;

                float nodeR = 25f;

                var rnd = new Random(1234);
                var edgeColors = edges.ToDictionary(
                    e => (e.From, e.To),
                    e => System.Drawing.Color.FromArgb(
                        200,
                        rnd.Next(50, 200),
                        rnd.Next(50, 200),
                        rnd.Next(50, 200)
                    )
                );


                // ---------------------------------------------------------
                // 1. RITA PILAR (med korrigerad start/slutpunkt)
                // ---------------------------------------------------------
                foreach (var (from, to) in edges)
                {
                    var p1 = positions[from];
                    var p2 = positions[to];

                    float dx = p2.X - p1.X;
                    float dy = p2.Y - p1.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 0.001f) continue;

                    float ux = dx / dist;
                    float uy = dy / dist;

                    var start = new PointF(p1.X + ux * nodeR, p1.Y + uy * nodeR);
                    var end = new PointF(p2.X - ux * nodeR, p2.Y - uy * nodeR);

                    var color = edgeColors[(from, to)];
                    using var arrowPen = new System.Drawing.Pen(color, 2);
                    arrowPen.CustomEndCap = new AdjustableArrowCap(6, 8);

                    g.DrawLine(arrowPen, start, end);
                }


                // ---------------------------------------------------------
                // 2. RITA NODER + NAMN + FÖRÄLDRAR
                // ---------------------------------------------------------
                foreach (var v in vars)
                {
                    var p = positions[v];

                    var rect = new RectangleF(p.X - nodeR, p.Y - nodeR, nodeR * 2, nodeR * 2);
                    g.FillEllipse(nodeBrush, rect);
                    g.DrawEllipse(nodePen, rect);

                    var size = g.MeasureString(v, font);
                    g.DrawString(v, font, textBrush, p.X - size.Width / 2, p.Y - size.Height / 2);

                    var parents = edges.Where(e => e.To == v).Select(e => e.From).ToList();
                    if (parents.Count > 0)
                    {
                        string parentText = "← " + string.Join(", ", parents);
                       // g.DrawString(parentText, font, Brushes.DarkSlateGray, p.X - 20, p.Y + nodeR + 5);
                       //hur läger jag istället in MI här: inget from / to i scope
                    }


                }
                // efter nod-loopen
                foreach (var (from, to) in edges)
                {
                    var p1 = positions[from];
                    var p2 = positions[to];

                    float dx = p2.X - p1.X;
                    float dy = p2.Y - p1.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 0.001f) continue;

                    float ux = dx / dist;
                    float uy = dy / dist;

                    var start = new PointF(p1.X + ux * nodeR, p1.Y + uy * nodeR);
                    var end = new PointF(p2.X - ux * nodeR, p2.Y - uy * nodeR);

                    if (miLookup.TryGetValue((from, to), out var mi))
                    {
                        float midX = (start.X + end.X) / 2;
                        float midY = (start.Y + end.Y) / 2;

                        var text = $"{mi:0.00}";
                        var size = g.MeasureString(text, font);

                        float pad = 2;

                        var color = edgeColors[(from, to)];
                        var bg = System.Drawing.Color.FromArgb(40, color.R, color.G, color.B); // svag bakgrund

                        var bgRect = new RectangleF(
                            midX - size.Width / 2 - pad,
                            midY - size.Height / 2 - pad,
                            size.Width + pad * 2,
                            size.Height + pad * 2
                        );

                        using var bgBrush = new SolidBrush(bg);
                        using var fgBrush = new SolidBrush(color);

                        g.FillEllipse(bgBrush, bgRect);
                        g.DrawEllipse(new System.Drawing.Pen(color, 1), bgRect);

                        g.DrawString(text, font, fgBrush, midX - size.Width / 2, midY - size.Height / 2);
                    }
                }

                bmp.Save(pathPng, ImageFormat.Png);
            }
            public void ExportCptsToCsv(BayesNetResult bn, string path)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Variable,Parents,ParentConfig,Low,Mid,High");

                foreach (var kv in bn.Cpts)
                {
                    var node = kv.Value;
                    string parentList = string.Join(",", node.Parents);

                    foreach (var row in node.Table)
                    {
                        string parentConfig = row.Key;
                        var probs = row.Value;

                        sb.AppendLine($"{node.Variable},{parentList},{parentConfig},{probs["Low"]},{probs["Mid"]},{probs["High"]}");
                    }
                }

                File.WriteAllText(path, sb.ToString());
            }
            private static readonly string[] States = new[] { "Low", "Mid", "High" };

            private string MakeParentKey(List<string> parentNames, Dictionary<string, string> row)
            {
                if (parentNames.Count == 0)
                    return "(no-parents)";

                var parts = new List<string>();
                foreach (var p in parentNames)
                {
                    row.TryGetValue(p, out var val);
                    parts.Add($"{p}={val}");
                }
                return string.Join("|", parts);
            }
            private void ComputeCpts(BayesNetResult bn)
            {
                bn.Cpts.Clear();

                var vars = bn.Variables;
                var edges = bn.Edges;

                // För varje variabel: hitta dess föräldrar
                foreach (var v in vars)
                {
                    var parents = edges
                        .Where(e => e.To == v)
                        .Select(e => e.From)
                        .Distinct()
                        .ToList();

                    var cpt = new CptNode
                    {
                        Variable = v,
                        Parents = parents
                    };

                    // counts[parentKey][state] = antal
                    var counts = new Dictionary<string, Dictionary<string, int>>();

                    foreach (var row in bn.Rows)
                    {
                        string parentKey = MakeParentKey(parents, row);

                        if (!row.TryGetValue(v, out var state))
                            state = "Missing";

                        if (!counts.TryGetValue(parentKey, out var stateDict))
                        {
                            stateDict = new Dictionary<string, int>();
                            foreach (var s in States)
                                stateDict[s] = 0;
                            counts[parentKey] = stateDict;
                        }

                        if (!stateDict.ContainsKey(state))
                            stateDict[state] = 0;

                        stateDict[state]++;
                    }

                    // Normalisera till sannolikheter (med ev. Laplace-smoothing om du vill)
                    var table = new Dictionary<string, Dictionary<string, double>>();

                    foreach (var kv in counts)
                    {
                        string parentKey = kv.Key;
                        var stateCounts = kv.Value;

                        int total = stateCounts.Values.Sum();
                        if (total == 0)
                        {
                            // fallback: uniform
                            var uniform = States.ToDictionary(s => s, s => 1.0 / States.Length);
                            table[parentKey] = uniform;
                            continue;
                        }

                        var probs = new Dictionary<string, double>();
                        foreach (var s in States)
                        {
                            stateCounts.TryGetValue(s, out int c);
                            probs[s] = c / (double)total;
                        }

                        table[parentKey] = probs;
                    }

                    cpt.Table = table;
                    bn.Cpts[v] = cpt;
                }
            }
            private double ComputeMutualInformationForPair(
                BayesNetResult bn,
                string varX,
                string varY)
            {
                int n = bn.Rows.Count;
                if (n == 0) return 0.0;

                // joint[xState][yState]
                var joint = new Dictionary<string, Dictionary<string, int>>();
                var countX = new Dictionary<string, int>();
                var countY = new Dictionary<string, int>();

                foreach (var s in States)
                {
                    countX[s] = 0;
                    countY[s] = 0;
                    joint[s] = new Dictionary<string, int>();
                    foreach (var t in States)
                        joint[s][t] = 0;
                }

                int total = 0;

                foreach (var row in bn.Rows)
                {
                    if (!row.TryGetValue(varX, out var sx)) continue;
                    if (!row.TryGetValue(varY, out var sy)) continue;

                    if (!States.Contains(sx) || !States.Contains(sy))
                        continue;

                    joint[sx][sy]++;
                    countX[sx]++;
                    countY[sy]++;
                    total++;
                }

                if (total == 0) return 0.0;

                double mi = 0.0;

                foreach (var sx in States)
                {
                    foreach (var sy in States)
                    {
                        double pxy = joint[sx][sy] / (double)total;
                        if (pxy <= 0) continue;

                        double px = countX[sx] / (double)total;
                        double py = countY[sy] / (double)total;

                        if (px <= 0 || py <= 0) continue;

                        mi += pxy * Math.Log(pxy / (px * py)); // ln-bas
                    }
                }

                return mi;
            }

            private void ComputeEdgeMutualInformation(BayesNetResult bn)
            {
                bn.EdgeInfos.Clear();

                foreach (var (from, to) in bn.Edges)
                {
                    double mi = ComputeMutualInformationForPair(bn, from, to);
                    bn.EdgeInfos.Add(new EdgeInfo
                    {
                        From = from,
                        To = to,
                        MutualInformation = mi
                    });
                }
            }
            public class CptNode
            {
                public string Variable { get; set; } = "";
                public List<string> Parents { get; set; } = new();
                // Nyckel: parent-konfiguration (t.ex. "Temperature=Low|Humidity=High")
                // Värde: dictionary över barnets tillstånd → sannolikhet
                public Dictionary<string, Dictionary<string, double>> Table { get; set; } = new();
            }

            public class EdgeInfo
            {
                public string From { get; set; } = "";
                public string To { get; set; } = "";
                public double MutualInformation { get; set; }
            }
            private double CorrelationTimeAware(List<double> values, List<double> density, List<string> pids)
            {
                // ⭐ Rätt tidsgruppering
                var groups = SplitIntoTimeSteps(values, density, density, pids);

                var rs = new List<double>();

                foreach (var g in groups)
                {
                    var xs = g.X;
                    var zs = g.Y; // density

                    if (xs.Count > 1)
                        rs.Add(Correlation(xs, zs));
                }

                if (rs.Count == 0)
                    return 0;

                return rs.Average();
            }

            private (double a, double b, double c) InteractionTripleTimeAware(
                List<double> x, List<double> y, List<double> z, List<string> pids)
            {
                // Dela upp i tidssteg
                var groups = SplitIntoTimeSteps(x, y, z, pids);

                // Om inga grupper → inget resultat
                if (groups.Count == 0)
                    return (0, 0, 0);

                // Vi använder endast första tidssteget
                var g = groups[0];

                var xs = g.X;
                var ys = g.Y;
                var zs = g.Z;

                if (xs.Count < 2)
                    return (0, 0, 0);

                // Kör exakt samma mått som den vanliga varianten
                return InteractionTriple(xs, ys, zs);
            }


            private List<(List<double> X, List<double> Y, List<double> Z)>
                SplitIntoTimeSteps(List<double> x, List<double> y, List<double> z, List<string> pids)
            {
                var groups = new List<(List<double>, List<double>, List<double>)>();

                var seen = new HashSet<string>();
                var currentX = new List<double>();
                var currentY = new List<double>();
                var currentZ = new List<double>();

                for (int i = 0; i < x.Count; i++)
                {
                    string pid = pids[i];

                    // ⭐ Om PID redan setts → ny körning börjar
                    if (seen.Contains(pid))
                    {
                        // spara föregående körning
                        groups.Add((currentX, currentY, currentZ));

                        // starta ny körning
                        currentX = new List<double>();
                        currentY = new List<double>();
                        currentZ = new List<double>();
                        seen.Clear();
                    }

                    seen.Add(pid);

                    currentX.Add(x[i]);
                    currentY.Add(y[i]);
                    currentZ.Add(z[i]);
                }

                // lägg till sista körningen
                if (currentX.Count > 0)
                    groups.Add((currentX, currentY, currentZ));

                return groups;
            }
            private (double r, int n) CorrelationTimeAwareFirstStep(
    List<double> values,
    List<double> density,
    List<string> pids)
            {
                // Dela upp i tidssteg (du har redan SplitIntoTimeSteps)
                var groups = SplitIntoTimeSteps(values, density, density, pids);

                if (groups.Count == 0)
                    return (0, 0);

                var g = groups[0];
                var xs = g.X;
                var ys = g.Y; // density

                int n = Math.Min(xs.Count, ys.Count);
                if (n < 2)
                    return (0, n);

                double r = Correlation(xs, ys);
                return (r, n);
            }
            public double ComputeNSD()
            {
                var seen = new HashSet<string>();
                var densities = new List<double>();

                foreach (var row in _rows)
                {
                    if (!row.TryGetValue("PointId", out var pidObj))
                        continue;

                    string pid = pidObj?.ToString() ?? "";

                    if (seen.Contains(pid))
                        continue;

                    seen.Add(pid);

                    if (!row.TryGetValue("Lon", out var lonObj) ||
                        !row.TryGetValue("Lat", out var latObj))
                        continue;

                    if (!TryToDouble(lonObj, out double lon) ||
                        !TryToDouble(latObj, out double lat))
                        continue;

                    double d = GetOrComputeGlobalDensityForPoint(pid, lon, lat);
                    densities.Add(d);
                }

                return densities.Count == 0 ? 0 : densities.Average();
            }
 

            public double MeasureDistributionShape()
            {
                var seenPoints = new HashSet<string>();
                var lons = new List<double>();
                var lats = new List<double>();
                // 1. Extrahera unika geografiska mätpunkter där arten finns
                foreach (var row in _rows)
                {
                    if (!row.TryGetValue("PointId", out var pidObj)) continue;
                    string pid = pidObj?.ToString() ?? "";

                    if (seenPoints.Contains(pid)) continue;
                    seenPoints.Add(pid);

                    if (row.TryGetValue("Lon", out var lonObj) && row.TryGetValue("Lat", out var latObj))
                    {
                        if (TryToDouble(lonObj, out double lon) && TryToDouble(latObj, out double lat))
                        {
                            lons.Add(lon);
                            lats.Add(lat);
                        }
                    }
                }

                if (lons.Count < 3)
                {
                    return 0;
                }

                // 2. Beräkna tyngdpunkten (Centroiden)
                double meanLon = lons.Average();
                double meanLat = lats.Average();

                // 3. Beräkna varians och kovarians för koordinaterna
                double varLon = 0;
                double varLat = 0;
                double covLonLat = 0;
                int n = lons.Count;

                for (int i = 0; i < n; i++)
                {
                    double dLon = lons[i] - meanLon;
                    double dLat = lats[i] - meanLat;

                    varLon += dLon * dLon;
                    varLat += dLat * dLat;
                    covLonLat += dLon * dLat;
                }

                varLon /= (n - 1);
                varLat /= (n - 1);
                covLonLat /= (n - 1);

                // 4. Lös egenvärdena för kovariansmatrisen (Analytisk PCA)
                // Matrisen är: [ varLon    covLonLat ]
                //              [ covLonLat  varLat   ]
                double trace = varLon + varLat;
                double determinant = (varLon * varLat) - (covLonLat * covLonLat);

                // Mittermsteg för pq-formeln/egenvärdesekvationen
                double discriminant = Math.Sqrt((trace * trace / 4.0) - determinant);
                double lambda1 = (trace / 2.0) + discriminant; // Storaxel (Varians i primär spridningsriktning)
                double lambda2 = (trace / 2.0) - discriminant; // Lillaxel (Varians i vinkelrät riktning)

                // 5. Beräkna rundhetskvoten (Roundness Ratio)
                double roundness = lambda1 > 0 ? lambda2 / lambda1 : 0;


                return roundness;
            }
            private void ExportNsdAndShape(double nsd, double roundness, string path)
            {
                var sb = new StringBuilder();
                sb.AppendLine("NSD;Roundness");
                sb.AppendLine($"{nsd};{roundness}");
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            public void ExportNsdAndRoundnessPlots_SystemDraw()
            {
                string[] species = { "jättebalsamin", "jätteloka", "jätteslide", "parkslide", "hägg" };

                double[] nsd = {
        Math.Round(0.25440790018705783, 3),
        Math.Round(0.3043114600219482, 3),
        Math.Round(0.2926464306483415, 3),
        Math.Round(0.3138266291926267, 3),
        Math.Round(0.26582787965658156, 3)
    };

                double[] roundness = {
        Math.Round(0.06626993448179098, 3),
        Math.Round(0.20924675546138347, 3),
        Math.Round(0.14277604469059277, 3),
        Math.Round(0.20638059499381783, 3),
        Math.Round(0.2061240187544668, 3)
    };

                string[] colorsHex = {
        "#f25454",
        "#d2f254",
        "#54f293",
        "#5493f2",
        "#d254f2"
    };

                Directory.CreateDirectory(@"C:\Networks\");

                DrawBarChart(
                    values: nsd,
                    labels: species,
                    colorsHex: colorsHex,
                    title: "NSD per art",
                    yMax: 0.35,
                    outputPath: @"C:\Networks\NSD.png"
                );

                DrawBarChart(
                    values: roundness,
                    labels: species,
                    colorsHex: colorsHex,
                    title: "Roundness per art",
                    yMax: 0.25,
                    outputPath: @"C:\Networks\Roundness.png"
                );
            }

            private void DrawBarChart(double[] values, string[] labels, string[] colorsHex, string title, double yMax, string outputPath)
            {
                int width = 900;
                int height = 600;
                int marginLeft = 80;
                int marginBottom = 80;
                int marginTop = 60;

                using (var bmp = new Bitmap(width, height))
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Title
                    using (var titleFont = new System.Drawing.Font("Segoe UI", 20, FontStyle.Bold))
                    using (var black = new SolidBrush(System.Drawing.Color.Black))
                    {
                        g.DrawString(title, titleFont, black, new PointF(20, 10));
                    }

                    int barCount = values.Length;
                    int plotWidth = width - marginLeft - 40;
                    int plotHeight = height - marginBottom - marginTop;
                    int barWidth = plotWidth / (barCount * 2);

                    // Axes
                    using (var axisPen = new System.Drawing.Pen(System.Drawing.Color.Black, 2))
                    {
                        g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, marginTop + plotHeight);
                        g.DrawLine(axisPen, marginLeft, marginTop + plotHeight, marginLeft + plotWidth, marginTop + plotHeight);
                    }

                    // Bars
                    for (int i = 0; i < barCount; i++)
                    {
                        double v = values[i];
                        int barHeight = (int)(v / yMax * plotHeight);

                        int x = marginLeft + i * (2 * barWidth) + barWidth / 2;
                        int y = marginTop + plotHeight - barHeight;

                        System.Drawing.Color c = ColorTranslator.FromHtml(colorsHex[i]);
                        using (var brush = new SolidBrush(c))
                        {
                            g.FillRectangle(brush, x, y, barWidth, barHeight);
                        }

                        // Border
                        using (var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1))
                        {
                            g.DrawRectangle(pen, x, y, barWidth, barHeight);
                        }

                        // Label under bar
                        using (var font = new System.Drawing.Font("Segoe UI", 10))
                        using (var black = new SolidBrush(System.Drawing.Color.Black))
                        {
                            g.DrawString(labels[i], font, black, new PointF(x, marginTop + plotHeight + 5));
                        }

                        // Value above bar
                        using (var font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold))
                        using (var black = new SolidBrush(System.Drawing.Color.Black))
                        {
                            g.DrawString(values[i].ToString("0.000"), font, black, new PointF(x, y - 20));
                        }
                    }

                    bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }




        }
    }
    }
