using GeoViewSE_Linnaeus.Analysis;
using Microsoft.VisualBasic;
using Plotly.NET;
using Plotly.NET.LayoutObjects;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Statistics;
using SQLitePCL;
//accsesables:
//list of what variable type each variables is, maybe so accsessible as v.type, eg. boolean, constant, continous, distance 
//*distance is the distance between a meassurment poin (eg. SMHI-station) and the calculas point (the points in geojson - in this case points of Prunus padus)
//list of what variables is listed to what distance variable - not alla have distance variables linked and they dont need to calculate distance based explanationprobability

//retrive data
//use timeseriers reader or read self

//annalysis
//create point density matrix or logical relation equivalent
//regression annalysis
//explain distribution of pointlayer, calculate explanationdegree for each variable
//boolean, how whell does a point correlate to true (the point=prunus padus exist = true) eg. does watermagasin there tend to be true or false and how often = explanationdegree
//costant, avg-value of all points, value range (SD), does the point density correspond to high/low-values of the variable - strength of correlation
//continous, avg-value of all points and times, value range (SD), does the point density correspond to high/low-values of the variable - strength of correlation
//if properties exist in geojson correlation for each vriable against each property in the geojson
//explain distance based exlanationprobability foreach(p in v.points){p.expl_degree = v.expl_degree * (1-p.value_dev%_from_v_mean_value); v.distBasedProbSet.add(p.dist, p.expl_degree) } explProb_degree = MathFormula.FindExpModel(v.distBasedProbSet) //endresult loss in %/meter as decribed as f(x)
//create permutation variables for those showing interaction effects (secondary permutations not allowed var_a--b ok, var_a--b--c not ok)



//return to caller
//sort away bad hits
// foreach#ofinterest(v in variables)(return v.explanationdegree, v.distbased_probabilitydegree)




using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media;
using static GeoViewSE_Linnaeus.analysis.AnalysisiEngine.AnalysisEngine;


namespace GeoViewSE_Linnaeus.analysis
    {
        public class AnalysisiEngine
        {
            public class AnalysisEngine
            {
                // DECLARATIONS
                public enum VariableKind
                {
                    Constant,      // t.ex. geokemi, litologi-booleaner
                    Continuous,    // t.ex. temp, nederbörd, vind, densitet
                    Boolean,       // t.ex. IsGrundvattenMagasin, IsMoran
                    Distance       // t.ex. DistCoast, DistRiver, DistSmhiWeather
                }
            private const string V = "title";


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
                sb.AppendLine("Name,ExplanationDegree (R2), SampleCorrelation (r),DistanceLossCorrelation,Mean,StdDev,ModelA,ModelK,Fisher-Z-corr (r),Fzc-CI-low,Fzc-CI-high,BootstrapR_StdDev");

                foreach (var r in results)
                {
                    // Hoppa över distanspartners (DistCoast, DistRiver, DistSmhiWeather etc.)
                    if (r.Name.StartsWith("Dist"))
                        continue;

                    string line = string.Join(",",
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
                        double r = Correlation(matrix.Values, matrix.GlobalDensity);
                        explanation = r * r;
                        sampleR = r;
                        // Bootstrap: inherent sampling dependency
                        var (bootMean, bootStd) = BootstrapCorrelation(matrix.Values, matrix.GlobalDensity, 200);
                        bootstrapRStd = bootStd;
                        // --- Fisher asymptotic correlation + confidence interval ---
                        double meanVal = mean.Value;
                        double sdVal = sd.Value;

                        double globalMean = Mean(matrix.GlobalDensity);
                        double globalSd = StdDev(matrix.GlobalDensity, globalMean);

                        double zSum = 0;
                        int n = matrix.Values.Count;

                        for (int i = 0; i < n; i++)
                        {
                            double xi = matrix.Values[i];
                            double yi = matrix.GlobalDensity[i];

                            double dx = xi - meanVal;
                            double dy = yi - globalMean;

                            double denom = sdVal * globalSd;
                            if (denom == 0)
                                continue;

                            double ri = dx * dy / denom;

                            // clamp
                            ri = Math.Max(-0.999999, Math.Min(0.999999, ri));

                            // Fisher transform
                            double zi = 0.5 * Math.Log((1 + ri) / (1 - ri));
                            zSum += zi;
                        }

                        double zMean = zSum / n;

                        // asymptotic correlation
                        asymptoticR = (Math.Exp(2 * zMean) - 1) / (Math.Exp(2 * zMean) + 1);

                        // 95% confidence interval for Fisher z
                        double se = 1.0 / Math.Sqrt(n - 3);
                        double zLow = zMean - 1.96 * se;
                        double zHigh = zMean + 1.96 * se;

                        // transform back
                        ciLow = (Math.Exp(2 * zLow) - 1) / (Math.Exp(2 * zLow) + 1);
                        ciHigh = (Math.Exp(2 * zHigh) - 1) / (Math.Exp(2 * zHigh) + 1);


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
                    double density = ComputeGlobalDensity(lon, lat);
                    m.GlobalDensity.Add(density);

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



            private (double[,], double[], double[]) Compute2DHistogramAdaptive(
                double[] x, double[] y, int targetBins = 50)
            {
                double xMin = x.Min();
                double xMax = x.Max();
                double yMin = y.Min();
                double yMax = y.Max();

                // bin width baserat på spann / targetBins
                double xStep = (xMax - xMin) / targetBins;
                double yStep = (yMax - yMin) / targetBins;

                int xBins = targetBins;
                int yBins = targetBins;

                double[,] grid = new double[xBins, yBins];

                for (int i = 0; i < x.Length; i++)
                {
                    int xi = (int)((x[i] - xMin) / xStep);
                    int yi = (int)((y[i] - yMin) / yStep);

                    // Se till att indexen inte hamnar utanför array-gränser
                    xi = Math.Min(xi, xBins - 1);
                    yi = Math.Min(yi, yBins - 1);

                    if (xi >= 0 && xi < xBins && yi >= 0 && yi < yBins)
                        grid[xi, yi]++;
                }

                // Skapa bin-centrala koordinater för x och y
                double[] xs = new double[xBins];
                double[] ys = new double[yBins];

                for (int i = 0; i < xBins; i++)
                    xs[i] = xMin + i * xStep + xStep / 2;  // centrum av bin
                for (int i = 0; i < yBins; i++)
                    ys[i] = yMin + i * yStep + yStep / 2;  // centrum av bin

                return (grid, xs, ys);
            }

            private double?[,] ToNullable(double[,] src)
            {
                int w = src.GetLength(0);
                int h = src.GetLength(1);

                double?[,] dst = new double?[w, h];

                for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                        dst[i, j] = src[i, j];

                return dst;
            }


        }
    }
    }
