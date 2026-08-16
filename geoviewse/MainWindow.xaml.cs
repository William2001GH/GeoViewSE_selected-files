using ABI.System;
using BruTile;
using BruTile.FileSystem; //lösa kast -  verifiera inte doublepoint viktning, density ,rossbyvågor, minedat mineral->mineralLayer, gärdsgårdslager
using BruTile.Predefined;
using BruTile.Web;
using BruTile.Wms;
using CefSharp;
using CefSharp.DevTools.DOM;
using CefSharp.Wpf;
using DotSpatial.Projections;
using DotSpatial.Projections.GeographicCategories;
using DotSpatial.Projections.ProjectedCategories;
using DotSpatial.Projections.Transforms;
using GeoViewSE.PluginsCore;
using GeoViewSE.timeslider;
using GeoViewSE_Linnaeus.client;
using GeoViewSE_Linnaeus.Helpers;
using GeoViewSE_Linnaeus.media;
using GeoViewSE_Linnaeus.modeller.fysik;
using GeoViewSE_Linnaeus.server;
using GeoViewSE_Linnaeus.skript;
using GeoViewSE_Linnaeus.ThreeD;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Extensions.Cache;
using Mapsui.Extensions.Provider;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Nts.Providers;
using Mapsui.Nts.Providers.Shapefile;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Providers.Wfs;
using Mapsui.Providers.Wms;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Provider;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.Core;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Distance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Windowing.Common.Input;
using OSGeo.GDAL;
using OSGeo.OSR;
using Plotly.NET;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Statistics;
using SkiaSharp;
using SQLitePCL;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Media.Devices;
using Windows.Web.Http;
using static GeoViewSE_Linnaeus.analysis.AnalysisiEngine;
using static GeoViewSE_Linnaeus.CountyCollectiveHandler;
using static OpenTK.Graphics.OpenGL.GL;
using static Plotly.NET.StyleParam;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = Mapsui.Styles.Color;
using IFeature = NetTopologySuite.Features.IFeature;
using Image = System.Windows.Controls.Image;
using Path = System.IO.Path;
using Point = NetTopologySuite.Geometries.Point;
using RoutedEventArgs = System.Windows.RoutedEventArgs;
using GeoViewSE.timeslider;
using GeoViewSE_Linnaeus.data.county;




namespace GeoViewSE_Linnaeus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public HistoricalPositionLayer _historicalLayer;
        private Server? _server;
        private WmsProvider? heatProvider;
        private WmsProvider? fireClassProvider;
        private readonly string _diseaseDbPath =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "GeoViewSE", "disease.sqlite");
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private MediaPlayer musicPlayer = new MediaPlayer();
        private Mapsui.Layers.Layer castleLayer;
        private Mapsui.Layers.Layer botanicalGardensLayer;
        private Mapsui.Layers.Layer churchesLayer;
        private Mapsui.Layers.Layer runeLayer;
        private Mapsui.Layers.Layer walkingRoutesLayer;
        private Mapsui.Layers.Layer beachesLayer;
        private Mapsui.Layers.Layer berggrundLayer;
        private Mapsui.Layers.Layer biogeoregionLayer;
        private Mapsui.Layers.Layer fossilLayer;
        private Mapsui.Layers.Layer statueLayer;
        private Mapsui.Layers.Layer windLayer;
        private Mapsui.Layers.Layer waterLayer;
        private Mapsui.Layers.Layer nuclearLayer;
        private Mapsui.Layers.Layer airportLayer;
        private double ResolutionLast = -1.0;
        public Mapsui.Layers.Layer pdbdLayer;
        internal Mapsui.Layers.Layer clientPhotoLayer;
        public Mapsui.Layers.Layer damLayer;
        public Mapsui.Layers.Layer battleLayer;
        public Mapsui.Layers.Layer caveLayer;
        public Mapsui.Layers.Layer osparLayer;
        public Mapsui.Layers.Layer churchFullLayer;
        public Mapsui.Layers.Layer harbourLayer;
        public Mapsui.Layers.Layer gradnatLayer;
        public Mapsui.Layers.ImageLayer heatLayer;
        public Mapsui.Layers.ImageLayer fireClassLayer;
        public Mapsui.Layers.Layer ruinLayer;
        public Mapsui.Layers.Layer stationLayer;
        public Mapsui.Layers.Layer railwayLayer;
        public Mapsui.Layers.Layer ebhLayer0;
        public Mapsui.Layers.Layer ebhLayer1;
        public Mapsui.Layers.Layer ebhLayer2;
        public Mapsui.Layers.Layer ebhLayer3;
        public Mapsui.Layers.Layer ebhLayer4;
        public Mapsui.Layers.Layer? trawlingLayer;
        public Mapsui.Layers.Layer worldMarineHeritageSitesLayer;
        public Mapsui.Layers.Layer pokestopLayer;
        public Mapsui.Layers.Layer townsLayer;
        public Mapsui.Layers.Layer uasLayer;
        public Mapsui.Layers.Layer fyrLayer;
        public Mapsui.Layers.Layer golfLayer;
        public Mapsui.Layers.Layer bombLayer;
        public Mapsui.Layers.Layer etymologiLayer;
        public Mapsui.Layers.Layer tvLayer;
        public Mapsui.Layers.Layer filmLayer;
        public Mapsui.Layers.Layer footballLayer;

        public Mapsui.Layers.Layer IceALayer { get; private set; }
        public Mapsui.Layers.Layer IceBLayer { get; private set; }
        public Mapsui.Layers.Layer IceCLayer { get; private set; }
        public Mapsui.Layers.Layer IceDLayer { get; private set; }
        private FeatureCollection arsenicCollection;
        private FeatureCollection coastlineFeatures;
        private FeatureCollection reserveFeatures;
        private FeatureCollection? tatortFeatures;
        private FeatureCollection? smaortFeatures;
        private FeatureCollection berggrundFeatures;
        private FeatureCollection bioRegFeatures;
        private FeatureCollection jordFeatures;
        private FeatureCollection gvFeatures;
        private FeatureCollection lanFeatures;
        private FeatureCollection regionFeatures;
        private FeatureCollection kommunFeatures;
        private FeatureCollection civoFeatures;
        private FeatureCollection geokemiFeatures;
        private FeatureCollection powTowerFeatures;
        private FeatureCollection cableFeatures;
        private FeatureCollection fireFeatures;
        private FeatureCollection lcFeatures;
        private FeatureCollection vattendistriktFeatures;
        private FeatureCollection soilFeatures;
        private FeatureCollection ridgeFeatures;
        private FeatureCollection wellFeatures;
        private FeatureCollection laFeatures;
        private FeatureCollection riverFeatures;
        private FeatureCollection? subbasinFeatures;
        private FeatureCollection postcodeFeatures;
        private FeatureCollection swedenFeatures;
        private FeatureCollection churchFullFeatures;
        private FeatureCollection sockenFeatures;
        private FeatureCollection educationFeatures;
        private FeatureCollection healthFeatures;
        private FeatureCollection harbourFeatures;
        private FeatureCollection nutsFeatures;
        private FeatureCollection desoFeatures;
        private STRtree<IFeature> jordIndex;
        private STRtree<IFeature> gvIndex;
        private STRtree<IFeature> kommunIndex;
        private STRtree<IFeature> civoIndex;
        private STRtree<IFeature> geokemiIndex;
        private STRtree<IFeature> powTowers;
        private STRtree<IFeature> cableIndex;
        private STRtree<IFeature> fireIndex;
        private STRtree<IFeature> soilIndex;
        private STRtree<IFeature> ridgeIndex;
        private STRtree<IFeature>? tatortsIndex;
        private STRtree<IFeature> smaortIndex;
        private STRtree<IFeature> wellIndex;
        private STRtree<IFeature> laIndex;
        private STRtree<IFeature> desoIndex;
        private STRtree<IFeature> riverIndex;
        private STRtree<IFeature> subbasinIndex;
        private STRtree<IFeature> postcodeIndex;
        private STRtree<IFeature> swedenIndex;
        private STRtree<IFeature> churchFullIndex;
        private STRtree<IFeature> sockenIndex;
        private STRtree<IFeature> educationIndex;
        private STRtree<IFeature> healthIndex;
        private STRtree<IFeature> harbourIndex;
        private STRtree<IFeature> nutsIndex;
        private STRtree<IFeature> railwayIndex;
        private STRtree<IFeature> wallaceIndex = new STRtree<IFeature>();
        private STRtree<IFeature> weberIndex = new STRtree<IFeature>();
        private List<MindatItem> _mindatItems = new List<MindatItem>();
        private bool _mindatLoaded = false;
        private bool renderingPaused = false;
        private MRect bounds;
        private bool measuring = false;
        private bool onlineSearchAvailable = false;
        private bool isPinned = false;
        private bool isPlayingSound = false;
        private bool debugShowRawWeather = false;
        public bool vattendistrikpluginbool = false;
        private bool boolFlagRefDefined = false;
        public bool timeSeriesRequested = false;
        public bool manualActivation = false;
        private bool _cinematicMode;
        private bool editMode = false;
        private bool _is3D = false;
        private bool _cesiumLoaded = false;
        private MPoint? measureStart = null;
        private MPoint? userLocation = null; //new
                                             // private Image userLocationIcon; //new
        private System.Windows.Shapes.Line? measureLine = null;
        private List<(MPoint world, string iconPath)> churchIcons = new();
        private readonly List<(MPoint world, string iconPath)> runeIcons = new();
        private readonly List<(MPoint world, string iconPath)> beachIcons = new();
        private readonly List<(MPoint world, string iconPath)> fossilIcons = new();
        private readonly List<(MPoint world, string iconPath)> statueIcons = new();
        private List<(MPoint world, IFeature feature)> windPoints = new();
        private List<(MPoint world, IFeature feature)> bedrockPoints = new();
        private List<(MPoint world, IFeature feature)> waterPoints = new();
        private List<(MPoint world, IFeature feature)> nuclearPoints = new();
        private List<(MPoint world, IFeature feature)> airportPoints = new();
        private List<(MPoint world, IFeature feature)> fyrPoints = new();
        private List<Dictionary<string, object?>> rows;
        private readonly List<InfoWindow> openInfoWindows = new();
        private readonly List<Image> churchImageControls = new();
        private readonly List<Image> runeImageControls = new();
        private readonly List<Image> beachImageControls = new();
        private readonly List<Image> fossilImageControls = new();
        private readonly List<Image> statueImageControls = new();
        private List<SoundPoint> soundPoints = new();
        private static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        private PerformanceCounter cpuCounter;
        private List<PerformanceCounter> gpuCounters = new();
        private bool gpuInitialized = false;
        private System.Windows.Threading.DispatcherTimer cpuTimer;
        private int frameCount = 0;
        private Stopwatch fpsWatch = new Stopwatch();
        private System.Windows.Threading.DispatcherTimer perfTimer;
        private DateTime lastInfoTime = DateTime.MinValue;
        private readonly System.TimeSpan infoCooldown = System.TimeSpan.FromMilliseconds(10000);
        private bool measuringArea = false;
        private List<MPoint> areaPoints = new();
        private Mapsui.Layers.Layer passagesLayer;
        private const System.String TrafikverketApiKey = "9fe5b4f3e8ca468dba11cff1f294f286";
        private const System.String n2yoSatleiteApiKey = "JP3URQ-N9D9F7-5MWFZ5-5OQI";
        private System.String musicPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "music/output.mp3");
        public static string ClientLayerPath =>
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "GeoViewSE", "clientLayer.geojson");
        private MemoryProvider clientProvider;
        private MemoryLayer clientLayer;
        public List<Mapsui.IFeature> clientFeatures = new();
        private string lastInfoWindowText = "";
        private string? lastSoundPath = null;
        private const int MaxInfoWindows = 4;
        private readonly double rasterOriginX = 250005.0;
        private readonly double rasterOriginY = 7699995.0;
        private readonly double rasterPixelSize = 10.0;
        private static readonly HashSet<int> OcobsBanlist = new HashSet<int>

{
    33024, 2109, 35057, 35068, 38004, 35056, 33012, 33031,
    35154, 33001, 33011, 2076, 35067, 38042, 38045, 35054,
    33004, 38003, 35070, 33027, 35071, 33023, 35063
};

        private readonly Dictionary<string, string> _definitions =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { "skugglängdsfaktor",
      "Skugglängdsfaktor k betyder att skugglängd = k * objektets höjd." },

    { "ppm",
      "ppm parts per milion - miljontedelar." },

    { "länstillhörighet",
      "Länstillhörighet är det län vägen administrativt tillhör." },

    { "nvdb",
      "NVDB vägnummer är den officiella numreringen av vägar i Sverige." },

    { "developer",
      "Utvecklare: William Knutsson." },
    {
      "utvecklare",
      "Utvecklare: William Knutsson."
    },
    {
      "skapades programmet",
      "Programet skapades år 2026"
    },
    {
      "kompabilitet",
      "Programmet är kompatibel för Windwos 10 och frammåt"
    }
};
        private static readonly Dictionary<string, string> CountyCodes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
    { "Stockholm län", "01" },
    { "Uppsala län", "03" },
    { "Södermanland län", "04" },
    { "Östergötland län", "05" },
    { "Jönköping län", "06" },
    { "Kronoberg län", "07" },
    { "Kalmar län", "08" },
    { "Gotland län", "09" },
    { "Blekinge län", "10" },
    { "Skåne län", "12" },
    { "Halland län", "13" },
    { "Västra Götaland län", "14" },
    { "Värmlands län", "17" },
    { "Örebro län", "18" },
    { "Västmanland län", "19" },
    { "Dalarna län", "20" },
    { "Gävleborg län", "21" },
    { "Västernorrland län", "22" },
    { "Jämtland län", "23" },
    { "Västerbotten län", "24" },
    { "Norrbotten län", "25" }
    };

        // Rasterdata GDAL
        private float[,] vegetationRaster;
        private int rasterWidth;
        private int rasterHeight;
        private bool vegetationRasterLoaded = false;
        public bool measuringHabitat = false;
        private List<MPoint> habitatPoints = new();
        private static readonly ProjectionInfo WebMercatorMine = ProjectionInfo.FromEpsgCode(3857);
        private static readonly ProjectionInfo Sweref99TmMine = ProjectionInfo.FromEpsgCode(3006);
        private static readonly OSGeo.OSR.SpatialReference SrWebMercator;
        private static readonly OSGeo.OSR.SpatialReference SrSweref99Tm;
        private static readonly OSGeo.OSR.CoordinateTransformation MercatorToSweref;
        private static readonly List<Mapsui.IFeature> _minimapViewportFeatures = new();

        private readonly MemoryLayer _minimapViewportLayer = new MemoryLayer
        {
            Name = "ViewportBox",
            Style = null,
            Features = _minimapViewportFeatures
        };

        private MemoryLayer debugRasterLayer;
        private OSGeo.OSR.CoordinateTransformation mercatorToSweref99Tm;
        private OSGeo.OSR.CoordinateTransformation _toWebMercator;
        public StudyHandler studyHandler; //was private
        public string studyHandlerTagFilter = null;
        private List<DmItem> _dmItems = new();   // alla objekt med koordinater
        private bool _dmLoaded = false;          // så vi inte laddar två gånger
        public bool UiFrozen { get; set; } = true;
        private const string SosBaseUrl = "https://api.artdatabanken.se/sos";
        private const string ApiKey = "4dfeaf12e8d04645956ed5d155e78487";

        private List<SpeciesObservation> _currentSpeciesObservations = new List<SpeciesObservation>();

        public class SpeciesObservation
        {
            public long Id { get; set; }
            public string ScientificName { get; set; }
            public string CommonName { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string TaxonId { get; set; }
            // Lägg till fler fält vid behov
        }
        private void RenderLoop(object? sender, EventArgs e)
        {
            frameCount++;
        }
        private string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
        public MainWindow()
        {
            //TP Start

            InitializeComponent();


            UpdateUiForMode();
            _ = AddWmsLayerAsync();

            Init();

            var list = mapControl.Map.Navigator.Resolutions;

            var sb = new StringBuilder();
            sb.AppendLine("=== Mapsui Resolutions ===");

            for (int i = 0; i < list.Count; i++)
            {
                sb.AppendLine($"Zoom {i}: {list[i]} m/px");
            }

            Log.Info("tmpcaller", sb.ToString());



            studyHandler = new StudyHandler("data/geojson/import/studies_debug.geojson");
            dbCreater.Initialize();
            InitGdal();
            LoadBathymetry();
            musicPlayer.Open(new System.Uri(musicPath, UriKind.Absolute));
            musicPlayer.MediaEnded += (s, e) => { musicPlayer.Position = System.TimeSpan.Zero; musicPlayer.Play(); }; // Loop

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // första värdet är alltid 0, så vi triggar den
            cpuTimer = new System.Windows.Threading.DispatcherTimer();
            cpuTimer.Interval = System.TimeSpan.FromMilliseconds(800);
            cpuTimer.Tick += CpuTimer_Tick;
            cpuTimer.Start();
            fpsWatch.Start();
            perfTimer = new System.Windows.Threading.DispatcherTimer();
            perfTimer.Interval = System.TimeSpan.FromMilliseconds(500);
            perfTimer.Tick += PerfTimer_Tick;
            perfTimer.Start();

            var utcTimer = new System.Windows.Threading.DispatcherTimer();
            utcTimer.Interval = System.TimeSpan.FromSeconds(1);
            utcTimer.Tick += (_, __) =>
            {
                TxtUtcClock.Text = "UTC: " + DateTime.UtcNow.ToString("yyyy:MM:dd:HH:mm:ss");
            };
            utcTimer.Start();

            var iconTimer = new System.Windows.Threading.DispatcherTimer();
            iconTimer.Interval = System.TimeSpan.FromSeconds(10);

            iconTimer.Tick += async (_, _) =>
            {
                await GetUserLocationAsync();
            };

            iconTimer.Start();

            Gdal.AllRegister();  //GDAL


            //_ = TestMsbApi();          // bara för utveckling
            _ = UpdateMsbOverlayAsync(); // overlay i UI

            // FPS-hook
            //CompositionTarget.Rendering += (_, __) => frameCount++;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            CompositionTarget.Rendering += RenderLoop;
            mapControl.MouseMove += MapControl_MouseMove;

            InitOnlineSearchUI();
            this.KeyDown += MainWindow_KeyDown;
            try
            {
                // 1. Skapa karta och bakgrundslager (OSM)
                //Esri-map
                //var esri = new TileLayer(KnownTileSources.Create(KnownTileSource.));



                LoggingWidget.ShowLoggingInMap = ActiveMode.No; //Here

                //minimap
                var miniLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
                minimap.Map = new Map();
                minimap.Map.Layers.Add(miniLayer);
                minimap.Map.Widgets.Clear();
                minimap.Map.Layers.Add(_minimapViewportLayer);

                //ClientLayer
                if (!File.Exists(ClientLayerPath))
                {
                    MessageBox.Show($"GeoViewSE whants to create a editable layer for you press ok to continue");
                    clientFeatures = new List<Mapsui.IFeature>();
                }
                else
                {
                    var geoJsonSerializer = GeoJsonSerializer.Create();
                    FeatureCollection fcClient;

                    using (var fs = new FileStream(ClientLayerPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    using (var jr = new Newtonsoft.Json.JsonTextReader(sr))
                    {
                        fcClient = geoJsonSerializer.Deserialize<FeatureCollection>(jr);
                    }

                    clientFeatures = new List<Mapsui.IFeature>();

                    if (fcClient != null)
                    {
                        foreach (var f in fcClient)
                        {
                            var mapsuiFeature = new Mapsui.Nts.GeometryFeature(f.Geometry);

                            if (f.Attributes != null)
                            {
                                foreach (var name in f.Attributes.GetNames())
                                    mapsuiFeature[name] = f.Attributes[name];
                            }

                            clientFeatures.Add(mapsuiFeature);
                        }
                    }
                }
                clientLayer = new MemoryLayer("ClientLayer")
                {
                    Features = clientFeatures,
                    Style = new Mapsui.Styles.SymbolStyle
                    {
                        Fill = new Mapsui.Styles.Brush(Color.Red),
                        SymbolScale = 0.8
                    }
                };

                mapControl.Map.Layers.Add(clientLayer);


                // 2. Läs in GeoJSON för kategori via NTS-provider
                var json = File.ReadAllText("data/geojson/castles.geojson", Encoding.UTF8);
                var provider = new GeoJsonProvider(json);
                var pointStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Black, 2),
                    SymbolScale = 0.5,
                    MinVisible = 2000,
                    MaxVisible = 500000
                };
                Mapsui.Styles.SymbolStyle.DefaultWidth = 16;
                Mapsui.Styles.SymbolStyle.DefaultHeight = 16;
                castleLayer = new Mapsui.Layers.Layer("Castles")
                {
                    DataSource = provider,
                    Style = pointStyle
                };
                mapControl.Map.Layers.Add(castleLayer);
                provider = null;
                pointStyle = null;

                //trawling border
                var jsonTrawling = File.ReadAllText("data/geojson/import/trawling_boundary.geojson", Encoding.UTF8);
                var providerTrawling = new GeoJsonProvider(jsonTrawling);
                var trawlingStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 1, 1, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 1, 0, 0), 1),
                };
                Mapsui.Styles.SymbolStyle.DefaultWidth = 16;
                Mapsui.Styles.SymbolStyle.DefaultHeight = 16;
                trawlingLayer = new Mapsui.Layers.Layer("trawlingLayer")
                {
                    DataSource = providerTrawling,
                    Style = trawlingStyle
                };
                mapControl.Map.Layers.Add(trawlingLayer);
                trawlingLayer.Enabled = false;
                providerTrawling = null;
                trawlingStyle = null;

                //Botaniska trädgårdar
                var jsonBotanical = File.ReadAllText("data/geojson/botanical_gardens.geojson", Encoding.UTF8);
                var providerBotanical = new GeoJsonProvider(jsonBotanical);
                var botanicalStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Triangle,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Green),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.DarkGreen, 2),
                    SymbolScale = 0.6
                };
                botanicalGardensLayer = new Mapsui.Layers.Layer("botanicalGardensLayer")
                {
                    DataSource = providerBotanical,
                    Style = botanicalStyle
                };
                mapControl.Map.Layers.Add(botanicalGardensLayer);
                botanicalStyle = null;
                providerBotanical = null;

                //World Marine Heritage Sites
                var jsonMarine = File.ReadAllText("data/geojson/import/world_marine_heritage_sites.geojson", Encoding.UTF8);
                var providerMarine = new GeoJsonProvider(jsonMarine);
                var marineStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(100, 0, 100, 255)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(255, 0, 100, 255), 2),
                };
                Mapsui.Styles.SymbolStyle.DefaultWidth = 16;
                Mapsui.Styles.SymbolStyle.DefaultHeight = 16;
                worldMarineHeritageSitesLayer = new Mapsui.Layers.Layer("worldMarineHeritageSitesLayer")
                {
                    DataSource = providerMarine,
                    Style = marineStyle
                };
                mapControl.Map.Layers.Add(worldMarineHeritageSitesLayer);
                worldMarineHeritageSitesLayer.Enabled = true;
                providerMarine = null;
                marineStyle = null;

                //galcier
                AddIceLayer("icea.geojson", "IceA_Layer", Color.FromArgb(255, 100, 180, 255), layer => IceALayer = layer);  // 10-11thousand ya
                AddIceLayer("iceb.geojson", "IceB_Layer", Color.FromArgb(255, 70, 130, 255), layer => IceBLayer = layer);   // 11-12thousand ya
                AddIceLayer("icec.geojson", "IceC_Layer", Color.FromArgb(255, 40, 80, 220), layer => IceCLayer = layer);    // 12-13thousand ya
                AddIceLayer("iced.geojson", "IceD_Layer", Color.FromArgb(255, 0, 50, 180), layer => IceDLayer = layer);     // 13-13.5thousand ya
                //kyrkor
                var jsonChurches = File.ReadAllText("data/geojson/churches.geojson", Encoding.UTF8);
                var providerChurches = new GeoJsonProvider(jsonChurches);
                var churchStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                    SymbolScale = 1.0
                };
                churchesLayer = new Mapsui.Layers.Layer("Churches")
                {
                    DataSource = providerChurches,
                    Style = churchStyle
                };
                mapControl.Map.Layers.Add(churchesLayer);
                churchStyle = null;
                var churchReader = new GeoJsonReader();
                var churchFeatures = churchReader.Read<FeatureCollection>(jsonChurches);
                foreach (var f in churchFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        churchIcons.Add((new MPoint(p.X, p.Y), "data/ikoner/church.png"));
                    }
                }
                // Fulltäckande kyrklager (byggnadsregistret)
                var jsonChurchFull = File.ReadAllText("data/geojson/kyrkor.geojson", Encoding.UTF8);

                var readerChurchFull = new GeoJsonReader();
                churchFullFeatures = readerChurchFull.Read<FeatureCollection>(jsonChurchFull);

                // STRtree
                churchFullIndex = new STRtree<IFeature>();
                foreach (var f in churchFullFeatures)
                {
                    try
                    {
                        churchFullIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }

                // Lagerstil
                var churchFullStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 255, 255, 255)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.5
                };

                // Lager
                churchFullLayer = new Mapsui.Layers.Layer("Kyrkor (fulltäckande)")
                {
                    DataSource = new GeoJsonProvider(jsonChurchFull),
                    Style = churchFullStyle,
                    Enabled = false // default avstängt
                };

                mapControl.Map.Layers.Add(churchFullLayer);


                //Runor
                var jsonRunes = File.ReadAllText("data/geojson/runestones.geojson", Encoding.UTF8);
                var providerRunes = new GeoJsonProvider(jsonRunes);
                var runeStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                    SymbolScale = 1.0
                };
                runeLayer = new Mapsui.Layers.Layer("Runestones")
                {
                    DataSource = providerRunes,
                    Style = runeStyle
                };
                mapControl.Map.Layers.Add(runeLayer);
                var runeReader = new GeoJsonReader();
                var runeFeatures = runeReader.Read<FeatureCollection>(jsonRunes);
                foreach (var f in runeFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        runeIcons.Add((
                            new MPoint(p.X, p.Y),
                            "data/ikoner/runa.png"
                        ));
                    }
                }
                //Click border
                var jsonClick = File.ReadAllText("data/geojson/clickborder.geojson");
                var readerClick = new GeoJsonReader();

                swedenFeatures = readerClick.Read<FeatureCollection>(jsonClick);
                swedenIndex = new STRtree<IFeature>();

                foreach (var f in swedenFeatures)
                {
                    try
                    {
                        swedenIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }

                //Ospar gräns
                var jsonOspar = File.ReadAllText("data/geojson/ospar_boundary.geojson", Encoding.UTF8);
                var providerOspar = new GeoJsonProvider(jsonOspar);
                var osparStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                };
                osparLayer = new Mapsui.Layers.Layer("osparLayer")
                {
                    DataSource = providerOspar,
                    Style = osparStyle
                };
                mapControl.Map.Layers.Add(osparLayer);

                //EBH 
                string ebhPath = "data/geojson/import/ebh.geojson";
                var geoJsonSerializer1 = NetTopologySuite.IO.GeoJsonSerializer.Create();
                FeatureCollection fc = null;
                using (var fileStream = new FileStream(ebhPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                    fc = geoJsonSerializer1.Deserialize<FeatureCollection>(jsonReader);
                }

                if (fc != null)
                {
                    var fc0 = new List<Mapsui.IFeature>();
                    var fc1 = new List<Mapsui.IFeature>();
                    var fc2 = new List<Mapsui.IFeature>();
                    var fc3 = new List<Mapsui.IFeature>();
                    var fc4 = new List<Mapsui.IFeature>();

                    foreach (var f in fc)
                    {
                        // Skapa en Mapsui-feature av NTS-featuren
                        var mapsuiFeature = new Mapsui.Nts.GeometryFeature(f.Geometry);

                        if (!f.Attributes.Exists("RISKKLASS"))
                        {
                            fc0.Add(mapsuiFeature);
                            continue;
                        }

                        var val = f.Attributes["RISKKLASS"];

                        if (val == null || val.ToString() == "" || val.ToString() == "NaN")
                            fc0.Add(mapsuiFeature);
                        else if (val.ToString() == "1")
                            fc1.Add(mapsuiFeature);
                        else if (val.ToString() == "2")
                            fc2.Add(mapsuiFeature);
                        else if (val.ToString() == "3")
                            fc3.Add(mapsuiFeature);
                        else if (val.ToString() == "4")
                            fc4.Add(mapsuiFeature);
                        else
                            fc0.Add(mapsuiFeature);
                    }

                    // Skapa lager
                    ebhLayer0 = CreateEbhLayer(fc0, "ebhLayer0", Color.White);
                    ebhLayer1 = CreateEbhLayer(fc1, "ebhLayer1", Color.FromArgb(180, 120, 120, 120));
                    ebhLayer2 = CreateEbhLayer(fc2, "ebhLayer2", Color.FromArgb(180, 255, 255, 0));
                    ebhLayer3 = CreateEbhLayer(fc3, "ebhLayer3", Color.FromArgb(180, 255, 165, 0));
                    ebhLayer4 = CreateEbhLayer(fc4, "ebhLayer4", Color.FromArgb(180, 255, 0, 0));

                    mapControl.Map.Layers.Add(ebhLayer0);
                    mapControl.Map.Layers.Add(ebhLayer1);
                    mapControl.Map.Layers.Add(ebhLayer2);
                    mapControl.Map.Layers.Add(ebhLayer3);
                    mapControl.Map.Layers.Add(ebhLayer4);

                    // Default avstängt
                    ebhLayer0.Enabled = false;
                    ebhLayer1.Enabled = false;
                    ebhLayer2.Enabled = false;
                    ebhLayer3.Enabled = false;
                    ebhLayer4.Enabled = false;
                }

                //gradnät lat/lon
                var jsonGradnat = File.ReadAllText("data/geojson/import/gradnat.geojson", Encoding.UTF8);
                var providerGradnat = new GeoJsonProvider(jsonGradnat);
                var gradnatStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(0, 0, 0, 0)), // Genomskinlig fyllning
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(180, 50, 50, 50), 1.5), // Tydliga mörkgrå linjer för gradnätet
                };
                gradnatLayer = new Mapsui.Layers.Layer("gradnatLayer")
                {
                    DataSource = providerGradnat,
                    Style = gradnatStyle
                };
                mapControl.Map.Layers.Add(gradnatLayer);
                gradnatLayer.Enabled = false;
                providerGradnat = null;
                gradnatStyle = null;
                jsonGradnat = null;

                //Ruiner
                var jsonRuins = File.ReadAllText("data/geojson/import/ruiner.geojson", Encoding.UTF8);
                var providerRuins = new GeoJsonProvider(jsonRuins);
                var ruinStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(0, 120, 180, 100)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.4
                };
                ruinLayer = new Mapsui.Layers.Layer("ruinLayer")
                {
                    DataSource = providerRuins,
                    Style = ruinStyle
                };
                mapControl.Map.Layers.Add(ruinLayer);
                ruinLayer.Enabled = false;
                jsonRuins = null;
                providerRuins = null;
                ruinStyle = null;

                //Pokestops
                var jsonPoke = File.ReadAllText("data/geojson/poke_stops.geojson", Encoding.UTF8);
                var providerPoke = new GeoJsonProvider(jsonPoke);
                var uri = new System.Uri(
    Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "data",
        "ikoner",
        "poke_stop.png"));
                var pokeStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uri.AbsoluteUri
                    },
                    SymbolScale = 0.1
                };
                pokestopLayer = new Mapsui.Layers.Layer("pokestopLayer")
                {
                    DataSource = providerPoke,
                    Style = pokeStyle
                };
                mapControl.Map.Layers.Add(pokestopLayer);
                pokestopLayer.Enabled = false;
                jsonPoke = null;
                providerPoke = null;
                pokeStyle = null;

                //Town density centras
                var jsonTownC = File.ReadAllText("data/geojson/bebygelsemittpunkter.geojson", Encoding.UTF8);
                var providerTownC = new GeoJsonProvider(jsonTownC);
                var uriTC = new System.Uri(
    Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "data",
        "ikoner",
        "town.png"));
                var townCStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriTC.AbsoluteUri
                    },
                    SymbolScale = 0.4
                };
                townsLayer = new Mapsui.Layers.Layer("townsLayer")
                {
                    DataSource = providerTownC,
                    Style = townCStyle
                };
                mapControl.Map.Layers.Add(townsLayer);
                townsLayer.Enabled = false;
                jsonTownC = null;
                providerTownC = null;
                uriTC = null;
                townCStyle = null;

                //Järnvägstationer
                var jsonStation = File.ReadAllText("data/geojson/import/railway_stations.geojson", Encoding.UTF8);
                var providerStation = new GeoJsonProvider(jsonStation);
                var stationStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(120, 120, 0, 100)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.4
                };
                stationLayer = new Mapsui.Layers.Layer("stationLayer")
                {
                    DataSource = providerStation,
                    Style = stationStyle
                };
                mapControl.Map.Layers.Add(stationLayer);
                stationLayer.Enabled = false;
                jsonStation = null;
                providerStation = null;
                stationStyle = null;

                // --- JÄRNVÄGSBUFFERT-INLÄSNING (MINNESOPTIMERAD) ---
                string railwayPath = "data/geojson/import/railway_buffer.geojson";
                railwayIndex = new STRtree<IFeature>();

                if (File.Exists(railwayPath))
                {
                    var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();

                    using (var fileStream = new FileStream(railwayPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        // Läser direkt från strömmen utan att dumpa hela filen som en sträng i RAM
                        var railwayFeatures = geoJsonSerializer.Deserialize<FeatureCollection>(jsonReader);

                        if (railwayFeatures != null)
                        {
                            foreach (var f in railwayFeatures)
                            {
                                try
                                {
                                    if (f.Geometry != null)
                                    {
                                        railwayIndex.Insert(f.Geometry.EnvelopeInternal, f);
                                    }
                                }
                                catch { /* Ignorera trasiga geometrier */ }
                            }
                        }
                    } // <- Strömmar och temporära samlingar frigörs här!
                }
                //Raster
                var schema = new GlobalSphericalMercator();
                var directory = "data/raster/tiles_oland";   // ändra till din sökväg
                var format = "png";
                var tileSource = new FileTileSource(schema, directory, format);
                var rasterLayer = new TileLayer(tileSource)
                {
                    Name = "Generalstabskartan Öland",
                    MinVisible = 1,
                    MaxVisible = 200000
                };
                mapControl.Map.Layers.Add(rasterLayer);
                //Floder

                riverIndex = new STRtree<IFeature>();
                var jsonRiversystem = File.ReadAllText("data/geojson/import/riversystem.geojson", Encoding.UTF8);
                var riverReader = new GeoJsonReader();
                riverFeatures = riverReader.Read<FeatureCollection>(jsonRiversystem);

                foreach (var f in riverFeatures)
                {
                    riverIndex.Insert(f.Geometry.EnvelopeInternal, f);
                }
                //järnvägar

                //Wallace & Weber lines
                var reader3 = new GeoJsonReader();
                var reader4 = new GeoJsonReader();
                // --- WALLACE-INLÄSNING ---
                if (File.Exists("data/geojson/import/wallace_line.geojson"))
                {
                    // using stänger och frigör strömmen och filen så fort måsvingen slutar
                    using (var fileStream = new FileStream("data/geojson/import/wallace_line.geojson", FileMode.Open, FileAccess.Read))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        // Vi läser direkt från strömmen, ingen stor mellanliggande sträng behövs
                        var wallaceFeatures = reader3.Read<FeatureCollection>(streamReader.ReadToEnd());

                        foreach (var f in wallaceFeatures)
                        {
                            wallaceIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                    } // <- Här rensas streamReader, fileStream och wallaceFeatures bort ur minnet!
                }
                reader3 = null;
                // --- WEBER-INLÄSNING ---
                if (File.Exists("data/geojson/import/weber_line.geojson"))
                {
                    using (var fileStream = new FileStream("data/geojson/import/weber_line.geojson", FileMode.Open, FileAccess.Read))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        var weberFeatures = reader4.Read<FeatureCollection>(streamReader.ReadToEnd());

                        foreach (var f in weberFeatures)
                        {
                            weberIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                    } // <- Här rensas allt bort för Weber ur minnet!
                }

                //Sjöar
                subbasinIndex = new STRtree<IFeature>();
                var jsonLakes = File.ReadAllText("data/geojson/import/lakes.geojson", Encoding.UTF8);
                var lakesReader = new GeoJsonReader();
                subbasinFeatures = lakesReader.Read<FeatureCollection>(jsonLakes);
                foreach (var f in subbasinFeatures)
                {
                    subbasinIndex.Insert(f.Geometry.EnvelopeInternal, f);
                }
                jsonLakes = null;
                lakesReader = null;
                subbasinFeatures.Clear();
                subbasinFeatures = null;
                //PDBD fossiler
                var jsonPdbd = File.ReadAllText("data/geojson/import/pbdb.geojson", Encoding.UTF8);
                var providerPdbd = new GeoJsonProvider(jsonPdbd);
                var uriPdbd = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"trilobite.png"));
                var pdbdStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriPdbd.AbsoluteUri
                    },
                    SymbolScale = 0.6
                };


                pdbdLayer = new Mapsui.Layers.Layer("pdbdLayer")
                {
                    DataSource = providerPdbd,
                    Style = pdbdStyle
                };

                mapControl.Map.Layers.Add(pdbdLayer);
                pdbdLayer.Enabled = false;
                jsonPdbd = null;
                providerPdbd = null;
                uriPdbd = null;
                pdbdStyle = null;

                // klientens fotoalbum
                var jsonPhoto = File.ReadAllText("plugins/ImageCollections/mapping.geojson", Encoding.UTF8);
                var providerPhoto = new GeoJsonProvider(jsonPhoto);
                var uriPhoto = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"applephoto.png"));
                var photoStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriPhoto.AbsoluteUri
                    },
                    SymbolScale = 0.05
                };


                clientPhotoLayer = new Mapsui.Layers.Layer("clientPhotoLayer")
                {
                    DataSource = providerPhoto,
                    Style = photoStyle
                };

                mapControl.Map.Layers.Add(clientPhotoLayer);
                clientPhotoLayer.Enabled = false;
                jsonPhoto = null;
                providerPhoto = null;
                uriPhoto = null;
                photoStyle = null;

                // Etymologi
                string filePath = "data/geojson/etymologi.geojson";
                if (File.Exists(filePath))
                {
                    using (var streamE = File.OpenRead(filePath))
                    using (var readerE = new StreamReader(streamE, Encoding.UTF8))
                    {
                        var jsonEtym = readerE.ReadToEnd();

                        var providerEtym = new GeoJsonProvider(jsonEtym);

                        var etymStyle = new Mapsui.Styles.SymbolStyle
                        {
                            SymbolType = SymbolType.Rectangle,
                            Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 200, 100)),
                            Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                            SymbolScale = 0.6
                        };

                        etymologiLayer = new Mapsui.Layers.Layer("etymologiLayer")
                        {
                            DataSource = providerEtym,
                            Style = etymStyle,
                            Enabled = false
                        };

                        mapControl.Map.Layers.Add(etymologiLayer);

                        providerEtym = null;
                        jsonEtym = null;
                        etymStyle = null;
                    }
                }
                //Fotbollsplaner
                string footballPath = "data/geojson/sport/fotbollsplaner.geojson";

                if (File.Exists(footballPath))
                {
                    Log.Info("football layer", "file exists");

                    using (var streamFo = File.OpenRead(footballPath))
                    using (var readerFo = new StreamReader(streamFo, Encoding.UTF8))
                    {
                        var jsonFo = readerFo.ReadToEnd();
                        var providerFo = new GeoJsonProvider(jsonFo);

                        var footballStyle = new Mapsui.Styles.SymbolStyle
                        {
                            SymbolType = SymbolType.Ellipse,
                            Fill = new Mapsui.Styles.Brush(
                                Color.FromArgb(200, 50, 180, 70)
                            ),
                            Outline = new Mapsui.Styles.Pen(
                                Color.Black,
                                1
                            ),
                            SymbolScale = 0.6
                        };

                        footballLayer = new Mapsui.Layers.Layer("footballLayer")
                        {
                            DataSource = providerFo,
                            Style = footballStyle,
                            Enabled = false
                        };

                        mapControl.Map.Layers.Add(footballLayer);

                        providerFo = null;
                        jsonFo = null;
                        footballStyle = null;
                    }
                }
                else
                {
                    Log.Info(
                        "Football layer",
                        "Football layer missing or not found"
                    );
                }
                //Filmplatser
                string tvPath = "data/geojson/kultur/tv.geojson";
                if (File.Exists(tvPath))
                {
                    using (var streamF = File.OpenRead(tvPath))
                    using (var readerF = new StreamReader(streamF, Encoding.UTF8))
                    {
                        var jsonF = readerF.ReadToEnd();
                        var providerF = new GeoJsonProvider(jsonF);

                        var tvStyle = new Mapsui.Styles.SymbolStyle
                        {
                            SymbolType = SymbolType.Ellipse,
                            Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 50, 150, 255)), // blå ton
                            Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                            SymbolScale = 0.6
                        };

                        tvLayer = new Mapsui.Layers.Layer("tvLayer")
                        {
                            DataSource = providerF,
                            Style = tvStyle,
                            Enabled = false
                        };

                        mapControl.Map.Layers.Add(tvLayer);

                        providerF = null;
                        jsonF = null;
                        tvStyle = null;
                    }
                }
                //Tv-serieplatser
                string filmPath = "data/geojson/kultur/film.geojson";
                if (File.Exists(filmPath))
                {
                    Log.Info("film layer", "file exists");
                    using (var streamF = File.OpenRead(filmPath))
                    using (var readerF = new StreamReader(streamF, Encoding.UTF8))
                    {
                        var jsonF = readerF.ReadToEnd();
                        var providerF = new GeoJsonProvider(jsonF);

                        var filmStyle = new Mapsui.Styles.SymbolStyle
                        {
                            SymbolType = SymbolType.Ellipse,
                            Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 255, 120, 0)), // orange ton
                            Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                            SymbolScale = 0.6
                        };

                        filmLayer = new Mapsui.Layers.Layer("filmLayer")
                        {
                            DataSource = providerF,
                            Style = filmStyle,
                            Enabled = true
                        };

                        mapControl.Map.Layers.Add(filmLayer);

                        providerF = null;
                        jsonF = null;
                        filmStyle = null;
                    }
                }
                else { Log.Info("film layer", "film layer missing or not found"); }

                // Golfbanor
                var jsonGolf = File.ReadAllText("data/geojson/import/golfbanor.geojson", Encoding.UTF8);
                var providerGolf = new GeoJsonProvider(jsonGolf);
                var uriGolf = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"golf.png"));
                var golfStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriGolf.AbsoluteUri
                    },
                    SymbolScale = 0.1
                };


                golfLayer = new Mapsui.Layers.Layer("golfLayer")
                {
                    DataSource = providerGolf,
                    Style = golfStyle
                };

                mapControl.Map.Layers.Add(golfLayer);
                golfLayer.Enabled = false;
                jsonGolf = null;
                providerGolf = null;
                uriGolf = null;
                golfStyle = null;

                // Bomber
                var jsonBomb = File.ReadAllText("data/geojson/bomber.geojson", Encoding.UTF8);
                var providerBomb = new GeoJsonProvider(jsonBomb);
                var uriBomb = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"blast.png"));
                var bombStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriBomb.AbsoluteUri
                    },
                    SymbolScale = 0.6
                };


                bombLayer = new Mapsui.Layers.Layer("bombLayer")
                {
                    DataSource = providerBomb,
                    Style = bombStyle
                };

                mapControl.Map.Layers.Add(bombLayer);
                bombLayer.Enabled = false;
                jsonBomb = null;
                providerBomb = null;
                uriBomb = null;
                bombStyle = null;

                // Fyrar
                var jsonFyr = File.ReadAllText("data/geojson/import/fyrar.geojson", Encoding.UTF8);
                var providerFyr = new GeoJsonProvider(jsonFyr);
                var uriFyr = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"lighthouse.png"));
                var fyrStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriFyr.AbsoluteUri
                    },
                    SymbolScale = 0.03
                };


                fyrLayer = new Mapsui.Layers.Layer("fyrLayer")
                {
                    DataSource = providerFyr,
                    Style = fyrStyle
                };

                mapControl.Map.Layers.Add(fyrLayer);
                fyrLayer.Enabled = false;
                jsonFyr = null;
                providerFyr = null;
                uriFyr = null;
                fyrStyle = null;

                // Flygplatser
                var jsonAirports = File.ReadAllText("data/geojson/airports.geojson", Encoding.UTF8);
                var providerAirports = new GeoJsonProvider(jsonAirports);
                var uriAir = new System.Uri(
Path.Combine(
AppDomain.CurrentDomain.BaseDirectory,
"data",
"ikoner",
"airplane.png"));
                var airportStyle = new ImageStyle
                {
                    Image = new Mapsui.Styles.Image
                    {
                        Source = uriAir.AbsoluteUri
                    },
                    SymbolScale = 0.4
                };


                airportLayer = new Mapsui.Layers.Layer("Flygplatser")
                {
                    DataSource = providerAirports,
                    Style = airportStyle
                };

                mapControl.Map.Layers.Add(airportLayer);
                airportLayer.Enabled = true;

                // Läs in features
                var airportReader = new GeoJsonReader();
                var airportFeatures = airportReader.Read<FeatureCollection>(jsonAirports);

                // Lista för FindNearest
                foreach (var f in airportFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        airportPoints.Add((new MPoint(p.X, p.Y), f));
                    }
                }
                jsonAirports = null;


                // Kärnkraftverk
                using (var streamK = File.OpenRead("data/geojson/nuclear.geojson"))
                using (var readerK = new StreamReader(streamK))
                {
                    var jsonNuclear = readerK.ReadToEnd();

                    var providerNuclear = new GeoJsonProvider(jsonNuclear);

                    var nuclearStyle = new Mapsui.Styles.SymbolStyle
                    {
                        SymbolType = SymbolType.Ellipse,
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(220, 255, 180, 120)),
                        Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                        SymbolScale = 0.7
                    };

                    nuclearLayer = new Mapsui.Layers.Layer("Kärnkraftverk")
                    {
                        DataSource = providerNuclear,
                        Style = nuclearStyle
                    };

                    mapControl.Map.Layers.Add(nuclearLayer);

                    // Läs FeatureCollection temporärt
                    var nuclearReader = new GeoJsonReader();
                    var nuclearFeatures = nuclearReader.Read<FeatureCollection>(jsonNuclear);

                    foreach (var f in nuclearFeatures)
                    {
                        if (f.Geometry is NetTopologySuite.Geometries.Point p)
                        {
                            nuclearPoints.Add((new MPoint(p.X, p.Y), f));
                        }
                    }

                    nuclearFeatures.Clear();
                    nuclearFeatures = null;
                    nuclearReader = null;
                    jsonNuclear = null;
                    providerNuclear = null;
                    nuclearStyle = null;
                }
                //Vattenreningsverk
                using (var streamV = File.OpenRead("data/geojson/import/vattenreningsverk.geojson"))
                using (var readerV = new StreamReader(streamV))
                {
                    var jsonWater = readerV.ReadToEnd();

                    var providerWater = new GeoJsonProvider(jsonWater);

                    var waterStyle = new Mapsui.Styles.SymbolStyle
                    {
                        SymbolType = SymbolType.Ellipse,
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 200, 100)),
                        Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                        SymbolScale = 0.6
                    };

                    waterLayer = new Mapsui.Layers.Layer("Reningsverk")
                    {
                        DataSource = providerWater,
                        Style = waterStyle
                    };

                    mapControl.Map.Layers.Add(waterLayer);
                    waterLayer.Enabled = false;

                    var waterReader = new GeoJsonReader();
                    var waterFeatures = waterReader.Read<FeatureCollection>(jsonWater);

                    foreach (var f in waterFeatures)
                    {
                        if (f.Geometry is NetTopologySuite.Geometries.Point p)
                        {
                            waterPoints.Add((new MPoint(p.X, p.Y), f));
                        }
                    }

                    waterFeatures.Clear();
                    waterFeatures = null;
                    waterReader = null;
                    jsonWater = null;
                    providerWater = null;
                    waterStyle = null;
                }

                //Nuts Id
                try
                {
                    string pathNuts = "data/geojson/import/nutsId.geojson";
                    string jsonNuts = File.ReadAllText(pathNuts);

                    var readerNuts = new GeoJsonReader();
                    nutsFeatures = readerNuts.Read<FeatureCollection>(jsonNuts);

                    nutsIndex = new STRtree<IFeature>();

                    foreach (var f in nutsFeatures)
                    {
                        try
                        {
                            nutsIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                }
                catch (System.Exception ex)
                {
                    // logg
                }

                // Fossiler

                var jsonFossils = File.ReadAllText("data/geojson/fossils.geojson", Encoding.UTF8);
                var providerFossils = new GeoJsonProvider(jsonFossils);
                var fossilStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                    SymbolScale = 1.0
                };
                fossilLayer = new Mapsui.Layers.Layer("Fossils")
                {
                    DataSource = providerFossils,
                    Style = fossilStyle
                };
                mapControl.Map.Layers.Add(fossilLayer);
                var fossilReader = new GeoJsonReader();
                var fossilFeatures = fossilReader.Read<FeatureCollection>(jsonFossils);
                foreach (var f in fossilFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        string iconType = f.Attributes["ikontype"]?.ToString()?.ToLower() ?? "default";
                        string iconPath = iconType switch
                        {
                            "ammonite" => "data/ikoner/ammonite.png",
                            "trilobite" => "data/ikoner/trilobite.png",
                            "crinoid" => "data/ikoner/crinoid.png",
                            _ => "data/ikoner/fossil_default.png"
                        };
                        fossilIcons.Add((new MPoint(p.X, p.Y), iconPath));
                    }
                }
                // Marktäckedata 2023
                var jsonLandcover = File.ReadAllText("data/geojson/import/gr_cover_type_jan_2023.geojson");
                var lcReader = new GeoJsonReader();
                lcFeatures = lcReader.Read<FeatureCollection>(jsonLandcover);

                var lcLayer = new Mapsui.Layers.Layer("Marktäcke")
                {
                    DataSource = new GeoJsonProvider(jsonLandcover),
                    Style = new VectorStyle
                    {
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(60, 100, 200, 100)),
                        Line = new Mapsui.Styles.Pen(Color.FromArgb(120, 50, 100, 50), 1)
                    }
                };

                mapControl.Map.Layers.Add(lcLayer);
                lcLayer.Enabled = false;
                // Vattendammar 
                var jsonWaterDam = File.ReadAllText("data/geojson/import/dammregistret.geojson", Encoding.UTF8);
                var providerWaterDam = new GeoJsonProvider(jsonWaterDam);

                var damStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(20, 90, 50, 105)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.5
                };
                damLayer = new Mapsui.Layers.Layer("Vattendammar")
                {
                    DataSource = providerWaterDam,
                    Style = damStyle
                };
                mapControl.Map.Layers.Add(damLayer);
                damLayer.Enabled = false;
                // Vindkraftverk
                var jsonWind = File.ReadAllText("data/geojson/import/vkv.geojson", Encoding.UTF8);
                var providerWind = new GeoJsonProvider(jsonWind);

                var windStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 150, 255)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.5
                };

                windLayer = new Mapsui.Layers.Layer("Vindkraftverk")
                {
                    DataSource = providerWind,
                    Style = windStyle
                };

                mapControl.Map.Layers.Add(windLayer);
                windLayer.Enabled = false;
                var windReader = new GeoJsonReader();
                var windFeatures = windReader.Read<FeatureCollection>(jsonWind);

                foreach (var f in windFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        windPoints.Add((new MPoint(p.X, p.Y), f));
                    }
                }
                // Bergledningsvärmetal värmeledningsförmåga i kordinaten x,y i den ytliga berggrunden (vilken i regel är identisk ner till ett visst djup).
                var jsonHeat = File.ReadAllText("data/geojson/import/bedrock_heat_transfer_value.geojson", Encoding.UTF8);

                var heatReader = new GeoJsonReader();
                var heatFeatures = heatReader.Read<FeatureCollection>(jsonHeat);

                foreach (var f in heatFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        bedrockPoints.Add((new MPoint(p.X, p.Y), f));
                    }
                }
                //Slagfält
                var jsonBattles = File.ReadAllText("data/geojson/battles.geojson", Encoding.UTF8);
                var providerBattles = new GeoJsonProvider(jsonBattles);

                var battleStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Triangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(220, 200, 50, 50)), // rödaktig
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.6
                };

                battleLayer = new Mapsui.Layers.Layer("Slagfält")
                {
                    DataSource = providerBattles,
                    Style = battleStyle,
                };

                mapControl.Map.Layers.Add(battleLayer);
                battleLayer.Enabled = true;
                // Grottor
                var jsonCaves = File.ReadAllText("data/geojson/caves.geojson", Encoding.UTF8);
                var providerCaves = new GeoJsonProvider(jsonCaves);

                var caveStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(220, 255, 215, 0)), // guld/gul
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.5
                };

                caveLayer = new Mapsui.Layers.Layer("Grottor")
                {
                    DataSource = providerCaves,
                    Style = caveStyle,
                };

                mapControl.Map.Layers.Add(caveLayer);
                caveLayer.Enabled = true;



                // Soundscape
                var soundJson = File.ReadAllText("data/audiogram/soundscape/sonograms.json");
                var arr = JArray.Parse(soundJson);

                foreach (var item in arr)
                {
                    var coord = item["koordinat"]!.ToObject<double[]>();
                    soundPoints.Add(new SoundPoint
                    {
                        Lon = coord![0],
                        Lat = coord[1],
                        Path = item["path"]!.ToString(),
                        Time = DateTime.Parse(item["time_of_recording"]!.ToString())
                    });
                }
                // Region
                var path = "data/geojson/regions.geojson";

                using (var streamReader = new StreamReader(path))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var geoJsonReader = new GeoJsonReader();
                    regionFeatures = geoJsonReader.Read<FeatureCollection>(jsonReader);
                }
                // Län
                var jsonLan = File.ReadAllText("data/geojson/import/lan.geojson");
                var lanReader = new GeoJsonReader();
                lanFeatures = lanReader.Read<FeatureCollection>(jsonLan);
                // Komun
                var jsonKommun = File.ReadAllText("data/geojson/import/komuner.geojson");
                var kommunReader = new GeoJsonReader();
                kommunFeatures = kommunReader.Read<FeatureCollection>(jsonKommun);

                kommunIndex = new STRtree<IFeature>();

                foreach (var f in kommunFeatures)
                {
                    try
                    {
                        kommunIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                // civilområde
                var jsonCivo = File.ReadAllText("data/geojson/import/civo.geojson");
                var civoReader = new GeoJsonReader();
                civoFeatures = civoReader.Read<FeatureCollection>(jsonCivo);

                civoIndex = new STRtree<IFeature>();

                foreach (var f in civoFeatures)
                {
                    try
                    {
                        civoIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                // Demografiskt statistikområde (DeSo)
                var jsonDeso = File.ReadAllText("data/geojson/import/deso.geojson");
                var desoReader = new GeoJsonReader();
                desoFeatures = desoReader.Read<FeatureCollection>(jsonDeso);

                desoIndex = new STRtree<IFeature>();

                foreach (var f in desoFeatures)
                {
                    try
                    {
                        desoIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch (System.Exception ex) { Log.Error("Demografiskt statistikområde", ex.Message); }
                }
                //LA-område
                try
                {
                    string pathLa = "data/geojson/import/la_areas.geojson";
                    string jsonLa = File.ReadAllText(pathLa);

                    var readerLa = new GeoJsonReader();
                    laFeatures = readerLa.Read<FeatureCollection>(jsonLa);

                    laIndex = new STRtree<IFeature>();

                    foreach (var f in laFeatures)
                    {
                        try
                        {
                            laIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                }
                catch (System.Exception ex)
                {
                    //logg
                }
                //socken
                try
                {
                    string pathSocken = "data/geojson/import/socken.geojson";
                    string jsonSocken = File.ReadAllText(pathSocken);

                    var readerSocken = new GeoJsonReader();
                    sockenFeatures = readerSocken.Read<FeatureCollection>(jsonSocken);

                    sockenIndex = new STRtree<IFeature>();

                    foreach (var f in sockenFeatures)
                    {
                        try
                        {
                            sockenIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                }
                catch (System.Exception ex)
                {
                    // logg
                }

                // Geokemi
                var jsonGeokemi = File.ReadAllText("data/geojson/import/geokemi.geojson");
                var geokemiReader = new GeoJsonReader();
                geokemiFeatures = geokemiReader.Read<FeatureCollection>(jsonGeokemi);

                geokemiIndex = new STRtree<IFeature>();

                foreach (var f in geokemiFeatures)
                {
                    try
                    {
                        geokemiIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }


                // --- Beaches ---
                var jsonBeaches = File.ReadAllText("data/geojson/beaches.geojson", Encoding.UTF8);
                var providerBeaches = new GeoJsonProvider(jsonBeaches);
                // Osynlig symbolstil (som runor/kyrkor)
                var beachStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                    SymbolScale = 1.0
                };
                beachesLayer = new Mapsui.Layers.Layer("Beaches")
                {
                    DataSource = providerBeaches,
                    Style = beachStyle
                };
                mapControl.Map.Layers.Add(beachesLayer);
                // Läs in punkterna för ikon‑overlay
                var beachReader = new GeoJsonReader();
                var beachFeatures = beachReader.Read<FeatureCollection>(jsonBeaches);
                foreach (var f in beachFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        beachIcons.Add((new MPoint(p.X, p.Y), "data/ikoner/beach.png"));
                    }
                }
                var geoJsonTextCoastline = File.ReadAllText("data/geojson/coastline/sweden_mercator_diffuse.geojson");
                var providerCoastline = new GeoJsonProvider(geoJsonTextCoastline);
                var coastlineLayer = new Mapsui.Layers.Layer("Sweden Coastline")
                {
                    DataSource = providerCoastline,
                    Style = new VectorStyle
                    {
                        Line = new Mapsui.Styles.Pen(Color.Black, 1)
                    }
                };
                mapControl.Map.Layers.Add(coastlineLayer);
                coastlineLayer.Enabled = false;
                var readerCoastline = new GeoJsonReader();
                coastlineFeatures = readerCoastline.Read<FeatureCollection>(geoJsonTextCoastline);
                var jsonBorder = File.ReadAllText("data/geojson/import/outline_lines.geojson", Encoding.UTF8);
                var providerBorder = new GeoJsonProvider(jsonBorder);
                var borderStyle = new VectorStyle
                {
                    Fill = null,
                    Line = new Mapsui.Styles.Pen(Color.HotPink, 0.5)
                };
                var borderLayer = new Mapsui.Layers.Layer("Sveriges Gräns")
                {
                    DataSource = providerBorder,
                    Style = borderStyle
                };
                mapControl.Map.Layers.Add(borderLayer);
                //Plugins
                LoadPluginLayers();
                //Statyer
                var jsonStatues = File.ReadAllText("data/geojson/statues.geojson", Encoding.UTF8);
                var providerStatues = new GeoJsonProvider(jsonStatues);
                var statueStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Rectangle,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(1, 0, 0, 0)),
                    Outline = new Mapsui.Styles.Pen(Color.FromArgb(0, 0, 0, 0), 1),
                    SymbolScale = 1.0
                };
                statueLayer = new Mapsui.Layers.Layer("Statues")
                {
                    DataSource = providerStatues,
                    Style = statueStyle
                };
                mapControl.Map.Layers.Add(statueLayer);
                var statueReader = new GeoJsonReader();
                var statueFeatures = statueReader.Read<FeatureCollection>(jsonStatues);
                foreach (var f in statueFeatures)
                {
                    if (f.Geometry is NetTopologySuite.Geometries.Point p)
                    {
                        statueIcons.Add((
                            new MPoint(p.X, p.Y),
                            "data/ikoner/statue.png"
                        ));
                    }
                }
                //Naturvårdsoråden
                var jsonReserves = File.ReadAllText("data/geojson/import/nature_reserve_diffus_10m.geojson");
                var providerReserves = new GeoJsonProvider(jsonReserves);
                var reserveStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(80, 0, 128, 0)), // semi-transparent grön
                    Line = new Mapsui.Styles.Pen(Color.DarkGreen, 1)
                };
                var reservesLayer = new Mapsui.Layers.Layer("Naturreservat")
                {
                    DataSource = providerReserves,
                    Style = reserveStyle
                };
                mapControl.Map.Layers.Add(reservesLayer);
                var reserveReader = new GeoJsonReader();
                reserveFeatures = reserveReader.Read<FeatureCollection>(jsonReserves);

                //järnvägar
                var jsonRailway = File.ReadAllText("data/geojson/import/railways.geojson");
                var providerRailway = new GeoJsonProvider(jsonRailway);
                var railwayStyle = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(80, 20, 128, 30)),
                    Line = new Mapsui.Styles.Pen(Color.DarkGreen, 1)
                };
                railwayLayer = new Mapsui.Layers.Layer("railwayLayer")
                {
                    DataSource = providerRailway,
                    Style = railwayStyle
                };
                mapControl.Map.Layers.Add(railwayLayer);
                railwayLayer.Enabled = false;
                //järnvägsbuffer


                //viltpassager
                var jsonPassages = File.ReadAllText("data/geojson/import/documentedwildlifedispersalroadpassages.geojson");
                var providerPassages = new GeoJsonProvider(jsonPassages);
                var passageStyle = new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = SymbolType.Ellipse,
                    Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 100, 100)),
                    Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                    SymbolScale = 0.5
                };
                passagesLayer = new Mapsui.Layers.Layer("Vägpassager")
                {
                    DataSource = providerPassages,
                    Style = passageStyle
                };
                mapControl.Map.Layers.Add(passagesLayer);
                passagesLayer.Enabled = false;
                // 3. Walking routes (leder)
                var jsonRoutes = File.ReadAllText("data/geojson/import/walking_routes.geojson", Encoding.UTF8);
                var providerRoutes = new GeoJsonProvider(jsonRoutes);
                var routeStyle = new VectorStyle
                {
                    Line = new Mapsui.Styles.Pen(Color.Orange, 1),   // tydlig färg
                    Fill = null
                };
                walkingRoutesLayer = new Mapsui.Layers.Layer("Walking Routes")
                {
                    DataSource = providerRoutes,
                    Style = routeStyle
                };
                walkingRoutesLayer.Enabled = false;
                mapControl.Map.Layers.Add(walkingRoutesLayer);
                //Elnät
                var jsonPowTower = File.ReadAllText("data/geojson/import/torn.geojson");

                var readerPowTowers = new GeoJsonReader();
                powTowerFeatures = readerPowTowers.Read<FeatureCollection>(jsonPowTower);

                powTowers = new STRtree<IFeature>();

                foreach (var f in powTowerFeatures)
                {
                    try
                    {
                        powTowers.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                var jsonKablar = File.ReadAllText("data/geojson/import/kablar.geojson");

                var readerCables = new GeoJsonReader();
                cableFeatures = readerCables.Read<FeatureCollection>(jsonKablar);

                cableIndex = new STRtree<IFeature>();

                foreach (var f in cableFeatures)
                {
                    try
                    {
                        cableIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Brandstationer
                var jsonFireStations = File.ReadAllText("data/geojson/import/fire_stations.geojson");

                var readerFireStations = new GeoJsonReader();
                fireFeatures = readerFireStations.Read<FeatureCollection>(jsonFireStations);

                fireIndex = new STRtree<IFeature>();

                foreach (var f in fireFeatures)
                {
                    try
                    {
                        fireIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Hamnar
                try
                {
                    string pathHarbour = "data/geojson/import/harbours.geojson";
                    string jsonHarbour = File.ReadAllText(pathHarbour);

                    var readerHarbour = new GeoJsonReader();
                    harbourFeatures = readerHarbour.Read<FeatureCollection>(jsonHarbour);

                    harbourIndex = new STRtree<IFeature>();

                    foreach (var f in harbourFeatures)
                    {
                        try
                        {
                            harbourIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                    // Lagerstil
                    var harbourStyle = new Mapsui.Styles.SymbolStyle
                    {
                        SymbolType = SymbolType.Triangle,
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(200, 0, 128, 255)), // blå
                        Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                        SymbolScale = 0.6
                    };

                    // Mapsui-lager
                    harbourLayer = new Mapsui.Layers.Layer("Hamnar")
                    {
                        DataSource = new GeoJsonProvider(jsonHarbour),
                        Style = harbourStyle,
                        Enabled = false // default avstängt
                    };
                    mapControl.Map.Layers.Add(harbourLayer);
                }

                catch (System.Exception ex)
                {
                    // logg
                }
                // --- EDUCATIONAL CENTERS --- Utbildningscentra eg. skolor, universitet, högskolor, gymnasier
                try
                {
                    string pathEdu = "data/geojson/import/educationalserviceseurostat.geojson";
                    string jsonEdu = File.ReadAllText(pathEdu);

                    var readerEdu = new GeoJsonReader();
                    educationFeatures = readerEdu.Read<FeatureCollection>(jsonEdu);

                    educationIndex = new STRtree<IFeature>();

                    foreach (var f in educationFeatures)
                    {
                        try
                        {
                            educationIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                }
                catch (System.Exception ex)
                {
                    // logg
                }

                // --- HEALTH CENTERS ---   Hälsocenter eg. sjukhus, vårdscentraler, kliniker
                try
                {
                    string pathHealth = "data/geojson/import/helathserviceseurostat2023.geojson";
                    string jsonHealth = File.ReadAllText(pathHealth);

                    var readerHealth = new GeoJsonReader();
                    healthFeatures = readerHealth.Read<FeatureCollection>(jsonHealth);

                    healthIndex = new STRtree<IFeature>();

                    foreach (var f in healthFeatures)
                    {
                        try
                        {
                            healthIndex.Insert(f.Geometry.EnvelopeInternal, f);
                        }
                        catch { }
                    }
                }
                catch (System.Exception ex)
                {
                    // logg
                }
                //Tätorter
                using (var streamT = File.OpenRead("data/geojson/import/tatort.geojson"))
                using (var readerT = new StreamReader(streamT))
                {
                    var jsonTatort = readerT.ReadToEnd();
                    var tatortReader = new GeoJsonReader();
                    tatortFeatures = tatortReader.Read<FeatureCollection>(jsonTatort);
                    tatortsIndex = new STRtree<IFeature>();

                    foreach (var f in tatortFeatures)
                    {
                        try
                        {
                            tatortsIndex.Insert(f.Geometry.EnvelopeInternal, f); //index may be null here ):
                        }
                        catch { }
                    }
                    jsonTatort = null;
                    tatortFeatures.Clear();
                    tatortFeatures = null;

                }
                //Småorter
                using (var streamS = File.OpenRead("data/geojson/import/smaorter.geojson"))
                using (var readerS = new StreamReader(streamS))
                {
                    var jsonSmaort = readerS.ReadToEnd();
                    var smaortReader = new GeoJsonReader();
                    smaortFeatures = smaortReader.Read<FeatureCollection>(jsonSmaort);
                    smaortIndex = new STRtree<IFeature>();

                    foreach (var f in smaortFeatures)
                    {
                        try
                        {
                            smaortIndex.Insert(f.Geometry.EnvelopeInternal, f); //index may be null here ):
                        }
                        catch { }
                    }
                    jsonSmaort = null;
                    smaortFeatures.Clear();
                    smaortFeatures = null;
                }
                //isräfflor
                var jsonIceRidge = File.ReadAllText("data/geojson/import/ice_jacks.geojson");

                var readerIceJacks = new GeoJsonReader();
                ridgeFeatures = readerIceJacks.Read<FeatureCollection>(jsonIceRidge);

                ridgeIndex = new STRtree<IFeature>();

                foreach (var f in ridgeFeatures)
                {
                    try
                    {
                        ridgeIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Brunnar
                string jsonWhells = File.ReadAllText("data/geojson/import/whells.geojson");

                var readerWhells = new GeoJsonReader();
                wellFeatures = readerWhells.Read<FeatureCollection>(jsonWhells);

                wellIndex = new STRtree<IFeature>();

                foreach (var f in wellFeatures)
                {
                    try
                    {
                        wellIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Jorddjup
                var jsonTerraDepth = File.ReadAllText("data/geojson/import/soil_depth.geojson");

                var readerTerraDepth = new GeoJsonReader();
                soilFeatures = readerTerraDepth.Read<FeatureCollection>(jsonTerraDepth);

                soilIndex = new STRtree<IFeature>();

                foreach (var f in soilFeatures)
                {
                    try
                    {
                        soilIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Jordmån
                var jsonJord = File.ReadAllText("data/geojson/import/jordart_diffuse_50m.geojson");
                var providerJord = new GeoJsonProvider(jsonJord);
                var jordLayer = new Mapsui.Layers.Layer("Jordarter")
                {
                    DataSource = providerJord,
                    Style = new VectorStyle
                    {
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(40, 200, 150, 0)),
                        Line = new Mapsui.Styles.Pen(Color.FromArgb(120, 80, 80, 80), 1)
                    }
                };
                mapControl.Map.Layers.Add(jordLayer);
                jordLayer.Enabled = false;
                var jordReader = new GeoJsonReader();
                jordFeatures = jordReader.Read<FeatureCollection>(jsonJord);
                jordIndex = new STRtree<IFeature>();
                foreach (var f in jordFeatures)
                {
                    try
                    {
                        jordIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch
                    {
                        // Ignorera ogiltiga polygoner
                    }
                }
                // Berggrund
                using (var streamB = File.OpenRead("data/geojson/import/sgu_berggrundsytor_diffuse_50m.geojson"))
                using (var readerB = new StreamReader(streamB))
                {
                    var jsonBerggrund = readerB.ReadToEnd();

                    var providerBerggrund = new GeoJsonProvider(jsonBerggrund);

                    berggrundLayer = new Mapsui.Layers.Layer("Berggrund")
                    {
                        DataSource = providerBerggrund,
                        Style = new VectorStyle
                        {
                            Fill = new Mapsui.Styles.Brush(Color.FromArgb(40, 150, 100, 200)),
                            Line = new Mapsui.Styles.Pen(Color.FromArgb(120, 80, 80, 120), 1)
                        }
                    };

                    mapControl.Map.Layers.Add(berggrundLayer);
                    berggrundLayer.Enabled = false;

                    var berggrundReader = new GeoJsonReader();
                    berggrundFeatures = berggrundReader.Read<FeatureCollection>(jsonBerggrund);

                    berggrundReader = null;
                    jsonBerggrund = null;
                    providerBerggrund = null;
                }

                // UAS zoner
                using (var streamU = File.OpenRead("data/geojson/import/uas.geojson"))
                using (var readerU = new StreamReader(streamU))
                {
                    var jsonUas = readerU.ReadToEnd();

                    var providerUas = new GeoJsonProvider(jsonUas);

                    var uasStyle = new VectorStyle
                    {
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(40, 150, 100, 200)),
                        Line = new Mapsui.Styles.Pen(Color.FromArgb(120, 80, 80, 120), 1)
                    };

                    uasLayer = new Mapsui.Layers.Layer("uasLayer")
                    {
                        DataSource = providerUas,
                        Style = uasStyle
                    };

                    mapControl.Map.Layers.Add(uasLayer);
                    uasLayer.Enabled = false;

                    jsonUas = null;
                    providerUas = null;
                    uasStyle = null;
                }

                //Biogeoregion
                using (var streamBi = File.OpenRead("data/geojson/import/biogeografiska_regioner.geojson"))
                using (var readerBi = new StreamReader(streamBi))
                {
                    var jsonBioregion = readerBi.ReadToEnd();
                    var BioRegReader = new GeoJsonReader();
                    bioRegFeatures = BioRegReader.Read<FeatureCollection>(jsonBioregion);

                    BioRegReader = null;
                    jsonBioregion = null;
                }


                //Grundvatten
                var jsonGV = File.ReadAllText("data/geojson/import/grv_diffuse_25m_cleaned.geojson");
                var gvReader = new GeoJsonReader();
                gvFeatures = gvReader.Read<FeatureCollection>(jsonGV);
                gvIndex = new STRtree<IFeature>();
                foreach (var f in gvFeatures)
                {
                    try
                    {
                        gvIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //postkoder
                var jsonPostCode = File.ReadAllText("data/geojson/import/postcodes.geojson");
                var readerPostCode = new GeoJsonReader();

                postcodeFeatures = readerPostCode.Read<FeatureCollection>(jsonPostCode);
                postcodeIndex = new STRtree<IFeature>();

                foreach (var f in postcodeFeatures)
                {
                    try
                    {
                        postcodeIndex.Insert(f.Geometry.EnvelopeInternal, f);
                    }
                    catch { }
                }
                //Arsenik
                var jsonArsenic = File.ReadAllText("data/geojson/import/arsenic_moraine.geojson");
                var reader = new GeoJsonReader();
                arsenicCollection = reader.Read<FeatureCollection>(jsonArsenic);

                var min = SphericalMercator.FromLonLat(10.5, 55.0); // sydväst
                var max = SphericalMercator.FromLonLat(24.5, 69.5); // nordost

                var swedenMercator = new MRect(
                    min.x, min.y,
                    max.x, max.y
                );
                var minZomOut = SphericalMercator.FromLonLat(8.5, 60.0); // sydväst
                var maxZomOut = SphericalMercator.FromLonLat(11.5, 80.5); // nordost

                var swedenMercatorZomOut = new MRect(
                    minZomOut.x, minZomOut.y,
                    maxZomOut.x, maxZomOut.y
                );

                mapControl.Map.Navigator.ZoomToBox(swedenMercator);
                minimap.Map.Navigator.ZoomToBox(swedenMercatorZomOut);

                Dispatcher.BeginInvoke(new Action(CreateChurchIcons));
                Dispatcher.BeginInvoke(new Action(CreateRuneIcons));
                Dispatcher.BeginInvoke(new Action(CreateFossilIcons));
                Dispatcher.BeginInvoke(new Action(CreateStatueIcons));
                var margin = 25000; // 50 km buffert
                bounds = new MRect(
                    min.x - margin,
                    min.y - margin,
                    max.x + margin,
                    max.y + margin
                );
                mapControl.Map.Navigator.ViewportChanged += EnforceBounds;
                mapControl.Info += async (s, e) =>  //added "async"
                {

                    var info = e.GetMapInfo(mapControl.Map.Layers);
                    if (editMode)
                    {
                        if (info.WorldPosition != null)
                        {
                            var pos = info.WorldPosition;

                            var ntsPoint = new NetTopologySuite.Geometries.Point(pos.X, pos.Y);
                            var feature = new GeometryFeature(ntsPoint);
                            feature["Created"] = DateTime.UtcNow.ToString("o");
                            feature["Type"] = "ClientPoint";
                            var note = await AskUserForNoteAsync();
                            if (!string.IsNullOrWhiteSpace(note))
                                feature["Note"] = note;
                            //Ask for client note/anteckning
                            clientFeatures.Add(feature);
                            Log.Info("ClientLayer", $"Added feature at {pos.X:F2}, {pos.Y:F2}");
                            clientLayer.Features = clientFeatures;
                            clientLayer.FeaturesWereModified();
                            clientLayer.DataHasChanged();

                            SaveClientLayer();   // skriver GeoJSON

                            return; // STOPPA den tunga infopipen
                        }
                    }
                    if (info?.Feature == null) return;
                    if (measuringHabitat)
                    {
                        var worldA = info.WorldPosition;
                        if (worldA == null) return;

                        // Avsluta om vi klickar nära första punkten
                        if (habitatPoints.Count > 2)
                        {
                            var first = habitatPoints[0];
                            var vp = mapControl.Map.Navigator.Viewport;
                            var firstScreen = vp.WorldToScreen(first.X, first.Y);
                            var clickScreen = vp.WorldToScreen(worldA.X, worldA.Y);

                            double dx = clickScreen.X - firstScreen.X;
                            double dy = clickScreen.Y - firstScreen.Y;
                            double dist = Math.Sqrt(dx * dx + dy * dy);

                            if (dist < 15) //15m?
                            {
                                FinishHabitatMeasurement();
                                return;
                            }
                        }

                        habitatPoints.Add(worldA);
                        DrawHabitatPreview();
                        return;
                    }

                    if (measuringArea)
                    {
                        var worldLocal = e.GetMapInfo(mapControl.Map.Layers)?.WorldPosition;
                        if (worldLocal == null) return;
                        // Om vi har minst 3 punkter → kolla om vi klickar nära första punkten
                        if (areaPoints.Count > 2)
                        {
                            var first = areaPoints[0];
                            var vp = mapControl.Map.Navigator.Viewport;
                            var firstScreen = vp.WorldToScreen(first.X, first.Y);
                            var clickScreen = vp.WorldToScreen(worldLocal.X, worldLocal.Y);
                            double dx = clickScreen.X - firstScreen.X;
                            double dy = clickScreen.Y - firstScreen.Y;
                            double distPixels = Math.Sqrt(dx * dx + dy * dy);
                            if (distPixels < 15) // 15 pixlar tolerans
                            {
                                FinishAreaMeasurement();
                                return;
                            }
                        }
                        // Annars: lägg till punkt
                        areaPoints.Add(worldLocal);
                        DrawAreaPreview();
                        return;
                    }
                    if (measuring)
                    {
                        var meassure_world = info.WorldPosition;

                        if (measureStart == null)
                        {
                            // Första punkten
                            measureStart = meassure_world;
                            Log.Info("Measure", "User started measuring distance");
                        }
                        else
                        {
                            var startLonLat = SphericalMercator.ToLonLat(measureStart.X, measureStart.Y);
                            var endLonLat = SphericalMercator.ToLonLat(meassure_world.X, meassure_world.Y);
                            // Andra punkten → avsluta mätning
                            double dist = Haversine(startLonLat.lat, startLonLat.lon,
                                                    endLonLat.lat, endLonLat.lon);
                            string distText = dist < 1000
                                ? $"{dist:F0} m"
                                : $"{dist / 1000:F2} km";
                            LiveDistanceText.Visibility = System.Windows.Visibility.Collapsed;
                            overlayCanvas.Children.Clear();
                            measuring = false;
                            measureStart = null;
                            Log.Info("Measure", $"Measured distance = {distText} m");
                            MessageBox.Show($"Avstånd: {distText}", "Mätverktyg");

                        }

                        return; // hoppa över vanlig InfoWindow
                    }
                    if (DateTime.Now - lastInfoTime < infoCooldown)
                        return;
                    lastInfoTime = DateTime.Now;
                    //Only if we come to here the progressbar is relevant to show

                    if (info.Layer?.Name == "Walking Routes") { if (!walkingRoutesLayer.Enabled) return; var props = info.Feature; string id = props["OBJECTID"]?.ToString() ?? "Okänt ID"; string ledKod = props["Statlig_led"]?.ToString() ?? "Okänd kod"; string ledNamn = props["Statlig_led_namn"]?.ToString() ?? "Okänt namn"; string längdRaw = props["Längd_på_delsträcka_m"]?.ToString() ?? "0"; double längdMeter = double.TryParse(längdRaw, out var lm) ? lm : 0; string längdVisning; if (längdMeter < 1000) { längdVisning = $"{Math.Round(längdMeter)} m"; } else { längdVisning = $"{(längdMeter / 1000):F1} km"; } var world2 = info.WorldPosition; var lonLat2 = SphericalMercator.ToLonLat(world2.X, world2.Y); MessageBox.Show($"Led: {ledNamn}\n" + $"Kod: {ledKod}\n" + $"Längd: {längdVisning} \n" + $"ID: {id}\n\n" + $"Lon: {lonLat2.lon:F5}, Lat: {lonLat2.lat:F5}", "Vandringsled"); return; }

                    var world = info.WorldPosition; // MPoint
                    if (world != null)
                    {
                        Log.Info("MapClick", $"User clicked at {world.X:F2}, {world.Y:F2}");
                    }

                    string? environmentalDataHeat = null;
                    var screenPosition = e.ScreenPosition;

                    var WmsResult = await heatProvider.GetFeatureInfoAsync( //deference of a posibly null referens
    mapControl.Map.Navigator.Viewport,
    screenPosition
);
                    try
                    {
                        foreach (var layer in WmsResult)
                        {
                            foreach (var feature in layer.Value)
                            {
                                //Log.Info("TYPE", feature.GetType().FullName ?? "");
                                foreach (var field in feature.Fields)
                                {
                                    var value = feature[field];
                                    if (field == "Classify.PixelValue")
                                    {
                                        Log.Info("Maxmarkyttemperatur Juli-Augusti 2023-2025", $"{value} C");

                                        environmentalDataHeat = $"Maxmarkyttemperatur Juli-Augusti 2023-2025 {value} C";
                                    }
                                    else
                                    {
                                        // Log.Info("WMS", $"field {field} value {value}");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex3)
                    {
                        Log.Error("WMS", $"{ex3}");
                    }
                    string? environmentalDataFireClass = null;

                    var fireClassInfo = await fireClassProvider.GetFeatureInfoAsync(
    mapControl.Map.Navigator.Viewport,
    screenPosition
);
                    try
                    {
                        foreach (var layer in fireClassInfo)
                        {
                            foreach (var feature in layer.Value)
                            {
                                foreach (var field in feature.Fields)
                                {
                                    var value = feature[field];
                                    //Log.Info("WMS",$"{value}");
                                    if (field == "Raster.Klassnamn")
                                    {
                                        environmentalDataFireClass = $"Brandbränsleklass (MSB): {value}";
                                    }
                                    else
                                    {
                                        //Log.Info("WMS", $"field {field} value {value}");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex_)
                    {
                        Log.Error("WMS", $"{ex_}");

                    }

                    if (!IsInsideSweden(world.X, world.Y))
                    {
                        // Klick utanför Sverige → avbryt
                        return;
                    }
                    ShowProgress();
                    SetProgress(0);
                    // Hitta närmaste arsenikpunkt
                    var nearest = FindNearestArsenicPoint(world.X, world.Y);
                    string environmentalData = "";
                    string environmentalDataArsenik = "";
                    string environmentalDataCoord = "";
                    string environmentalDataBiogeographicalBorders = "";
                    string environmentalDataArtDataBanken = "";
                    string environmentalDataCountryName = "";
                    string environmentalDataRegion = ""; // Som i Kalmar Region etc.
                    string environmentalDataSpecificInfo = "";
                    string environmentalDataCountyName = ""; // Som i Kalmar Län etc.
                    string environmentalDataCountyHousholdCount = "";
                    string environmentalDataCountyElectricity = "";
                    string environmentalDataCountyCostalArea = "";
                    string environmentalDataRegionSicknesStats = "";
                    string environmentalDataCountyCode = "";
                    string environmentalDataCountyArea = "";
                    string environmentalDataCountyHarvest = "";
                    string environmentalDataCountyPopulation = "";
                    string environmentalDataCountyForeignBorn = "";
                    string environmentalDataCountyHorses = "";
                    string environmentalDataCountyCats = "";
                    string environmentalDataCountyDogs = "";
                    string environmentalDataCountyCattle = "";
                    string environmentalDataCountyTbe = "";
                    string environmentalDataCountyHarpest = "";
                    string environmentalDataCountyCarDensity = "";
                    string environmentalDataCountyTraficDeaths = "";
                    string environmentalDataCountyNumbOfMunicipals = "";
                    string environmentalDataCountyMcUsage = "";
                    string environmentalDataCountyTruckUsage = "";
                    string environmentalDataCountyBusUsage = "";
                    string environmentalDataStress = "";
                    string environmentalDataCountyCarFuel = "";
                    string environemntalDataCountyCarUsage = "";
                    string environmentalDataPostCodes = "";
                    string environmentalDataKommunNamn = "";
                    string environmetalDataKidNames = null;
                    string environmentalDataDeSo = "";
                    string environmentalDataKommunCrimStats = "";
                    string environmentalDataKommunMaleMeanLife = "";
                    string environmentalDataSickLeave = "";
                    string environmentalDataKommunKod = "";
                    string environmentalDataLaArea = "";
                    string environmentalDataSockenName = "";
                    string environmentalDataLandsdel = "";
                    string environmentalDataNutsId = "";
                    string environmentalDataCountyAgeDistrib = "";
                    string environmentalDataNearbyScientificStudies = "";
                    string environmentalDataSateliteData = "";
                    string environmentalDataSunRiseTime = "";
                    string environmentalDataSunSetTime = "";
                    string environmentalDataTraficData = "";
                    string environmentalDataTraficDataRoadNumb = "";
                    string environmentalDataTraficDataSpeedLimit = "";
                    string environmentalDataSoundSpeed = "";
                    string environmentalDataWeatherData = "";
                    string environmentalDataCentrifugalforce = "";
                    string environmentalDataWindKineticEnergy = "";
                    string environmentalDataCorriolisFrequency = "";
                    string environmentalDataCorriolisInterstitialPeriod = "";
                    string environmentalDataWeatherDataCloudCoverage = "";
                    string environmentalDataSolarElevaion = "";
                    string environmentalDataCoastDistance = "";
                    string environmentalDataCoastBearing = "";
                    string environmentalDataIsNatureReserveBoolean = "";
                    string environmentalDataWaterDepth = "";
                    string environmentalDataOpenMeteoHeader = "";
                    string environmentalDataMetersAboveSeaLvl = "";
                    string environmentalDataGravitationPotential = "";
                    string environmentalDataAirCompositionData = "";
                    string environmentalDataPollenData = "";
                    string environmentalDataUv = "";
                    string environmentalDataPollenKollenHeader = "";
                    string environmentalDataPollenStationName = "";
                    string environmentalDataPollenValues = "";
                    string environmetalDataLayerSpecfics = "";
                    string environmentalDataWildLifeRoadPassageClciked = "";
                    string environmentalDataCaveClicked = "";
                    string environmentalDataBattleFieldClicked = "";
                    string environmentalDataVattenDistrikt = "";
                    string environmentalDataWindEnergyInVicinity = "";
                    string environmentalDataReningsverkInVicinity = "";
                    string environmentalDataNuclearInVicinity = "";
                    string environmentalDataClosestWaterTemp = "";
                    string environmentalDataClosestOceanSalinity = "";
                    string environmentalDataWellsInVicinity = "";
                    string environmentalDataChurchesInVicinity = "";
                    string environmentalDataAirportInVicinity = "";
                    string environmentalDataCurrentMoonPhase = "";
                    string environmentalDataNerbyLandCoverage = "";
                    string environmentalDataLocalBedgroundDescription = "";
                    string environmetnalDataTatort = "";
                    string environmentaldataIgHashtags = null;
                    string environmetnalDataSmaort = "";
                    string environmentalDataBiogeographicalRegion = "";
                    string environmentalDataSoilType = "";
                    string environmentalDataLocalGeochemistry = "";
                    string environmentalDataPowerTowerInVicinity = "";
                    string environmentalDataPowerCableInVicinity = "";
                    string environmentalDataFireStationInVicinity = "";
                    string environmentalDataHarbourInVicinity = "";
                    string environmentalDataHealthFacilityInVicinity = "";
                    string environmentalDataEducationFacilityInVicinity = "";
                    string environmentalDataHydraulicKInbedground = "";
                    string environmentalDataIceRidgeInVcinity = "";
                    string environmentalDataSoilDepthInVicinity = "";
                    string environmentalDataGroundWater = "";
                    string environmentalDataRecordedAudioSound = "";
                    string environmentalDataGravitation = "";
                    string environmentalDataTerrain = "";
                    string environmentalDataLandWaterRatio = "";
                    string environmentalDataClosestRiver = "";
                    string environmentalDataLocalLandscapeDiversity = "";
                    string environmentalDataLocalNdvi = "";
                    string environmentalDataCivo = "";
                    string envionmentalDataBedrockHeatTransfer = "";


                    if (nearest != null)
                    {
                        var props = nearest.Attributes;
                        string arsenikRaw = props["as_ppm"]?.ToString() ?? "okänt";
                        double arsenikVal = double.TryParse(arsenikRaw, out var val) ? val : 0;
                        string arsenikVisning = arsenikVal > 0 ? $"{arsenikVal:F2} ppm" : "okänt";
                        Point pt = null;
                        if (nearest.Geometry is MultiPoint mp && mp.NumGeometries > 0)
                            pt = mp.Geometries[0] as Point;
                        if (pt != null)
                        {
                            double dx = pt.X - world.X;
                            double dy = pt.Y - world.Y;
                            double distMeters = Math.Sqrt(dx * dx + dy * dy);
                            string distVisning = distMeters < 1000
                                ? $"{distMeters:F0} m"
                                : $"{distMeters / 1000:F1} km";
                            environmentalDataArsenik =
                                $"Miljödata: Arsenikhalt i närmaste moränprov ({distVisning}): {arsenikVisning}";
                        }
                    }
                    SetProgress(20);
                    var lonLat = SphericalMercator.ToLonLat(world.X, world.Y);
                    double lon = lonLat.lon;
                    double lat = lonLat.lat;
                    double[] xy = new double[] { lon, lat };
                    double[] z = new double[] { 0 };


                    var wgs84 = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    var sweref99tm = DotSpatial.Projections.KnownCoordinateSystems.Projected.NationalGrids.SwedishNationalGrid;
                    var rt90 = DotSpatial.Projections.KnownCoordinateSystems.Projected.NationalGrids.RT9025gonWest;
                    var utm33 = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone33N;

                    // Transformera
                    DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, wgs84, sweref99tm, 0, 1);


                    // xy[0] = Easting (X)
                    // xy[1] = Northing (Y)
                    double swerefX = xy[0];
                    double swerefY = xy[1];

                    double[] xyRT90 = new double[] { lon, lat };
                    double[] zRT90 = new double[] { 0 };

                    DotSpatial.Projections.Reproject.ReprojectPoints(xyRT90, zRT90, wgs84, rt90, 0, 1);

                    double rt90X = xyRT90[0];
                    double rt90Y = xyRT90[1];

                    double[] xyUTM = new double[] { lon, lat };
                    double[] zUTM = new double[] { 0 };

                    DotSpatial.Projections.Reproject.ReprojectPoints(xyUTM, zUTM, wgs84, utm33, 0, 1);

                    double utmX = xyUTM[0];
                    double utmY = xyUTM[1];

                    var (swX, swY) = ConvertWebMercatorToSweref(world.X, world.Y);
                    var (rtX, rtY) = ConvertWebMercatorToRt90(world.X, world.Y);
                    var (utmX2, utmY2) = ConvertWebMercatorToUtm33(world.X, world.Y);
                    var sweref1200 = ConvertWebMercatorToEpsg(world.X, world.Y, 3007);
                    var sweref1330 = ConvertWebMercatorToEpsg(world.X, world.Y, 3008);
                    var sweref1500 = ConvertWebMercatorToEpsg(world.X, world.Y, 3009);
                    var sweref1630 = ConvertWebMercatorToEpsg(world.X, world.Y, 3010);
                    var sweref1800 = ConvertWebMercatorToEpsg(world.X, world.Y, 3011);
                    var sweref1415 = ConvertWebMercatorToEpsg(world.X, world.Y, 3012);
                    var sweref1545 = ConvertWebMercatorToEpsg(world.X, world.Y, 3013);
                    var sweref1715 = ConvertWebMercatorToEpsg(world.X, world.Y, 3014);
                    var sweref1845 = ConvertWebMercatorToEpsg(world.X, world.Y, 3015);
                    var sweref2015 = ConvertWebMercatorToEpsg(world.X, world.Y, 3016);
                    var sweref2145 = ConvertWebMercatorToEpsg(world.X, world.Y, 3017);
                    var sweref2315 = ConvertWebMercatorToEpsg(world.X, world.Y, 3018);
                    var rt38 = ConvertWebMercatorToEpsg(world.X, world.Y, 3022);
                    var wgs84_ = ConvertWebMercatorToEpsg(world.X, world.Y, 4326);


                    environmentalDataCoord +=
                    //$"\n\n--- Koordinater ---" +
                    $"\nWebMercator X: {world.X:F2}" +
                    $"\nWebMercator Y: {world.Y:F2}" +
                    $"\nSWEREF99TM X (öst): {swX:F2}" +
                    $"\nSWEREF99TM Y (nord): {swY:F2}" +
                    $"\nRT90 X: {rtX:F2}" +
                    $"\nRT90 Y: {rtY:F2}" +
                    $"\nUTM33N X: {utmX2:F2}" +
                    $"\nUTM33N Y: {utmY2:F2}";
                    environmentalDataCoord += $"\nSWEREF99 12 00: {sweref1200.X:F2}, {sweref1200.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 13 30: {sweref1330.X:F2}, {sweref1330.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 15 00: {sweref1500.X:F2}, {sweref1500.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 16 30: {sweref1630.X:F2}, {sweref1630.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 18 00: {sweref1800.X:F2}, {sweref1800.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 14 15: {sweref1415.X:F2}, {sweref1415.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 15 45: {sweref1545.X:F2}, {sweref1545.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 17 15: {sweref1715.X:F2}, {sweref1715.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 18 45: {sweref1845.X:F2}, {sweref1845.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 20 15: {sweref2015.X:F2}, {sweref2015.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 21 45: {sweref2145.X:F2}, {sweref2145.Y:F2}";
                    environmentalDataCoord += $"\nSWEREF99 23 15: {sweref2315.X:F2}, {sweref2315.Y:F2}";
                    environmentalDataCoord += $"\nRT38 X: {rt38.X:F2}";
                    environmentalDataCoord += $"\nRT38 Y: {rt38.Y:F2}";
                    environmentalDataCoord += $"\nWGS84 Lon: {wgs84_.X:F6}";
                    environmentalDataCoord += $"\nWGS84 Lat: {wgs84_.Y:F6}";


                    SetProgress(25);

                    // EPSG:3857 (WebMercator)
                    try
                    {
                        await ShowNearbyMindatItemsAsync(lon, lat, 10, "place");
                    }
                    catch (System.Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    SetProgress(27);
                    try
                    {
                        string species = await LoadSpeciesInRadiusAsync(lat, lon, 100);
                        environmentalDataArtDataBanken = species;

                    }
                    catch (System.Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        Log.Error("Artpdatabanken API", exception.Message);
                    }



                    var (wallaceDist, wallacePoint) = FindNearestLineWGS84(lon, lat, wallaceIndex);
                    environmentalDataBiogeographicalBorders += "Biogeografiska gränser";
                    environmentalDataBiogeographicalBorders += FormatLineOutput("Wallace-linjen", wallaceDist, wallacePoint);

                    var (weberDist, weberPoint) = FindNearestLineWGS84(lon, lat, weberIndex);
                    environmentalDataBiogeographicalBorders += FormatLineOutput("Weber-linjen", weberDist, weberPoint);


                    bool sweden = IsInsideSweden(world.X, world.Y);
                    if (sweden)
                    {
                        environmentalDataCountryName = "[LAND]\nSverige";
                    }
                    else
                    {
                        environmentalDataCountryName = "?";
                    }
                    var region = FindRegionPolygon(lon, lat);
                    if (region != null)
                    {
                        string regionName = region.Attributes?["name"]?.ToString() ?? "Out of bounds";
                        environmentalDataRegion += $"{regionName}";
                    }

                    var county = FindCountyPolygon(lon, lat);

                    if (county != null)
                    {
                        string countyName = county.Attributes["NAME_1"]?.ToString() ?? "Okänt";


                        try
                        {
                            var instagram = new InstagramService();
                            var (posts, usedHashtag) = await instagram.GetHashtagPostsAsync(countyName);

                            // Visa resultatet
                            var mediawindow = new InstagramResultsWindow(usedHashtag, posts);
                            mediawindow.Show();
                        }
                        catch (System.Exception _ex)
                        {
                            Log.Error("Instagram API", _ex.Message);
                        }
                        try
                        {
                            var namnData = GetHumanNameForLan(countyName);

                            string flicka = namnData.Flicknamn;
                            string pojke = namnData.Pojknamn;

                            environmetalDataKidNames = $"Flicknamn: {flicka}, Pojknamn: {pojke}";
                        } catch { throw; }
                        environmentalDataCountyName +=
                            $"{countyName}";

                        switch (countyName)
                        {
                            case "Kalmar":
                                var kalmarInfoString = Kalmar.GetKalmarInfo();
                                environmentalDataSpecificInfo += kalmarInfoString;
                                break;
                            default:
                                break;
                        }

                        var households = CountyCollectiveHandler.GetHouseholdsForCounty(countyName);

                        if (households != null)
                        {
                            environmentalDataCountyHousholdCount += $"Antal hushåll i länet (2025): {households}";
                        }

                        var electricity = CountyCollectiveHandler.GetElectricityForCounty(countyName);


                        if (electricity == null || electricity == 0)
                        {
                            environmentalDataCountyElectricity += "Total elproduktion (2024) i länet: saknas data";
                        }
                        else
                        {
                            environmentalDataCountyElectricity += $"Total elproduktion i länet (2024): >= {electricity:N0} MWh";
                        }

                        var coastalArea = CountyCollectiveHandler.GetCoastalAreaForCounty(countyName);
                        environmentalDataCountyCostalArea += $" (Havs-)kustareal: {coastalArea} km2";

                        string RegionName = GetRegionCodeByCounty(countyName);

                        var categories = dbCreater.GetAllCategories();

                        environmentalDataRegionSicknesStats += $"Sjukdomsfall tillgängliga i registret";

                        foreach (var icd in categories)
                        {
                            if (icd != "J45-J46" && icd != "J60-J70")
                            {
                                int women = dbCreater.GetNumbOfCases(2022, RegionName, icd, "WOMEN");
                                int men = dbCreater.GetNumbOfCases(2022, RegionName, icd, "MEN");
                                environmentalDataRegionSicknesStats += $"\n icd: {icd} men: {men}, women: {women}";
                            }
                            else
                            {
                                int total = dbCreater.GetNumbOfCases(2022, RegionName, icd, "UNKNOWN");
                                environmentalDataRegionSicknesStats += $"\n icd: {icd} total: {total}";

                            }
                        }

                        string? searchStr = countyName + " " + "län";

                        string? countyCode = GetCountyCode(searchStr);

                        if (countyCode != null)
                        {
                            environmentalDataCountyCode += $"Länskod: {countyCode}";
                        }
                        var areaCounty = CountyAreaHandler.GetAreaForCounty(countyName);

                        if (areaCounty != null)
                        {
                            environmentalDataCountyArea += $"Areal: {areaCounty:F0} km²";
                        }

                        var harvest = HarvestHandler.GetHarvestForCounty(countyName);

                        if (harvest != null)
                        {
                            environmentalDataCountyHarvest += "Jordbruksverket (Skörd 2024)";

                            foreach (var row in harvest)
                                environmentalDataCountyHarvest += $"\n{row}";
                        }
                        else
                        {
                            Log.Error("Harvest data", $"Harvest returns null for {countyName}");
                        }
                        var density = CountyPopulationHandler.GetPopulationDensityForCounty(countyName);
                        if (density != null)
                        {
                            environmentalDataCountyPopulation += "Befolkningstäthet (SCB)";
                            environmentalDataCountyPopulation += $"Invånare per kvadratkilometer i länet: {density}";
                        }
                        var foreignBorn = CountyForeignBornHandler.GetForeignBornForCounty(countyName);

                        if (foreignBorn != null)
                        {
                            environmentalDataCountyForeignBorn += $"Totalt antal utrikes födda 2024: {foreignBorn}";
                        }

                        var horseCount = CountyHorseHandler.GetHorsesForCounty(countyName);
                        if (horseCount != null)
                        {
                            environmentalDataCountyHorses += $"Jorbruksverket 2016 antal hästar: {horseCount}\n realtiv andel av Rikets: {horseCount / 355000} %";
                        }
                        var catCount = CountyCatHandler.GetCatsForCounty(countyName);
                        if (catCount != null)
                        {
                            environmentalDataCountyCats += $"Antal katter 2025: {catCount}";
                        }
                        var dogCount = CountyDogHandler.GetDogsForCounty(countyName);
                        if (dogCount != null)
                        {
                            environmentalDataCountyDogs += $"Antal hundar 2025: {dogCount}";
                        }
                        var cowCount = CountyCattleHandler.GetCattleForCounty(countyName);
                        if (cowCount != null)
                        {
                            environmentalDataCountyCattle += $"Jorbruksverket antal kor 2020: {cowCount}";
                        }
                        var tbe = CountyTbeHandler.GetTbeCasesForCounty(countyName);

                        if (tbe != null)
                        {
                            environmentalDataCountyTbe += "Rapporterade TBE-fall (2015–2024) Fohm";
                            environmentalDataCountyTbe += $"\nTotalt antal fall: {tbe}";
                        }
                        var harpest = CountyHarpestHandler.GetHarpestForCounty(countyName);

                        if (harpest != null)
                        {
                            //  environmentalDataCountyHarpest += "\n--- Harpestfall (2024) Fohm ---";
                            environmentalDataCountyHarpest += $"Antal rapporterade fall harpest (2024, Fohm): {harpest}";
                        }
                        var carDensity = CountyCarHandler.GetCarDensity(countyName);
                        if (carDensity != null)
                        {
                            environmentalDataCountyCarDensity += $"Bilar i länet per kvm2: {carDensity}";
                        }
                        var pc = GetPostcode(world.X, world.Y);
                        int? municipalityCount =
    CountyCollectiveHandler.GetMunicipalityCount(countyName);

                        if (municipalityCount != null)
                        {
                            environmentalDataCountyNumbOfMunicipals =
                                $"Kommuner i länet: {municipalityCount}";
                        }
                        var trafficDeaths =
    CountyCollectiveHandler.GetTrafficDeathsPer100k(countyName);

                        if (trafficDeaths != null)
                        {
                            environmentalDataCountyTraficDeaths =
                                $"Trafikdödade: {trafficDeaths:F1} per 100 000 invånare";
                        }
                        int? averageMileage =
    CountyCollectiveHandler.GetAverageCarMileage(countyName);

                        if (averageMileage != null)
                        {
                            environemntalDataCountyCarUsage =
                                $"Personbilars medelkörsträcka: {averageMileage:N0} mil";
                        }
                        int? averageMotorcycleMileage =
    CountyCollectiveHandler.GetAverageMotorcycleMileage(countyName);

                        if (averageMotorcycleMileage != null)
                        {
                            environmentalDataCountyMcUsage =
                                $"Motorcyklars medelkörsträcka: {averageMotorcycleMileage:N0} mil";
                        }
                        int? averageTruckMileage =
    CountyCollectiveHandler.GetAverageTruckMileage(countyName);

                        if (averageTruckMileage != null)
                        {
                            environmentalDataCountyTruckUsage =
                                $"Lastbilars medelkörsträcka: {averageTruckMileage:N0} mil";
                        }
                        int? averageBusMileage =
    CountyCollectiveHandler.GetAverageBusMileage(countyName);

                        if (averageBusMileage != null)
                        {
                            environmentalDataCountyBusUsage =
                                $"Bussars medelkörsträcka: {averageBusMileage:N0} mil";
                        }
                        CountyVehicleRegistrations? registrations =
    CountyCollectiveHandler.GetVehicleRegistrations(countyName);

                        if (registrations != null)
                        {
                            environmentalDataCountyCarFuel =
                                $"Nyregistrerade bensinbilar (2025): {registrations.Petrol:N0}" +
                                $"\nNyregistrerade dieselbilar (2025): {registrations.Diesel:N0}" +
                                $"\nNyregistrerade el-, elhybrid- och laddhybridbilar (2025): {registrations.ElectricHybrid:N0}";
                        }
                        int? totalStress = CountyCollectiveHandler.GetStressSickLeaveTotal(countyName);

                        if (totalStress != null)
                        {
                            environmentalDataStress = $"Startade stress-sjukfall (F43) 2025: {totalStress} st";
                        }
                        if (pc != null)
                        {
                            environmentalDataPostCodes +=
                                "Postnummerområde" +
                                $"\nPostnummer: {pc}";
                        }
                    }
                    else
                    {
                        environmentalDataCountyName += "Län --- Ingen träff.";
                    }
                    var kommun = FindKommunPolygon(lon, lat);

                    if (kommun != null)
                    {
                        string kommunNamn = kommun.Attributes["NAME_2"]?.ToString() ?? "Okänd";

                        environmentalDataKommunNamn +=
                            $"{kommunNamn}";
                        //hooks

                        try
                        {

                            var stats = CountyCollectiveHandler.GetCrimStats(kommunNamn);
                            if (stats != null)
                            {
                                environmentalDataKommunCrimStats = $"Antal brott i komunen\nVåldtäckter: {stats.Rape},\nSexuella övergrepp: {stats.SexualAssault}\nSkadegörelse: {stats.Vandalism}";
                            }
                            else
                            {
                                Log.Error("Kriminaldata", $"{kommunNamn}:null-value return");
                            }
                        }
                        catch (System.Exception exp)
                        {
                            Log.Error("Kriminaldata", $"{exp.Message}");
                        }

                        try
                        {
                            await ShowNearbyMuseumItemsAsync(lon, lat, 50, kommunNamn);
                        }
                        catch (System.Exception ex1)
                        {
                            Log.Error("Museum data", $"{ex1.Message}");
                        }
                        try
                        {
                            double? maleLife = CountyCollectiveHandler.GetMaleLifeExpectancy(kommunNamn);

                            if (maleLife != null)
                            {
                                environmentalDataKommunMaleMeanLife =
                                    $"Medellivslängd män: {maleLife:F1} år";
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            Log.Error("Medellivslängd män", $"{ex2.Message}");
                        }
                        double? rate = CountyCollectiveHandler.GetSickLeaveRate(kommunNamn);
                        if (rate != null)
                        {
                            environmentalDataSickLeave = $"Sjukpenningtal 2.0 (dec mån, 2025): {rate:F2}";
                        }
                        else
                        {
                            Log.Info("Sjukpeningstal", "Inget matchand kommunnanmn återfanns");
                        }
                    }
                    else
                    {
                        environmentalDataKommunNamn += "Kommun ---Ingen träff.";
                    }
                    var civo = FindCivoPolygon(lon, lat);
                    if (civo != null)
                    {
                        string civoName = civo.Attributes["NAMN"]?.ToString() ?? "Okänt";
                        environmentalDataCivo += $"{civoName}";
                    }
                    var deso = FindDeSoPolygon(lon, lat);
                    if (deso != null)
                    {
                        string desoCode = deso.Attributes["desokod"]?.ToString() ?? "Okänt";
                        string kommunCode = deso.Attributes["kommunkod"]?.ToString() ?? "Okänt";
                        environmentalDataDeSo = desoCode;
                        environmentalDataKommunKod = kommunCode;
                    }

                    var la = IdentifyLaArea(lon, lat);

                    if (la != null)
                    {
                        var laName = la.Attributes["Namn"]?.ToString() ?? "Okänt område";
                        var laCode = la.Attributes["Lakod"].ToString() ?? "Okänt område";

                        // environmentalDataLaArea += "LA‑område";
                        environmentalDataLaArea += $"{laCode}";
                        environmentalDataLaArea += $"\n{laName}";

                    }
                    var socken = IdentifySocken(lon, lat);

                    if (socken != null)
                    {
                        var sockenName = socken.Attributes["sockenst_1"]?.ToString() ?? "Okänd socken";

                        //environmentalDataSockenName += "Socken";
                        environmentalDataSockenName += $"{sockenName}";
                    }
                    var resultNuts = IdentifyNuts(lon, lat);
                    if (resultNuts != null)
                    {
                        var (landsdel, nuts) = resultNuts.Value;
                        if (landsdel != null)
                        {
                            environmentalDataLandsdel += $"Landsdel: {landsdel}";
                        }
                        if (nuts != null)
                        {
                            var nutsId = nuts.Attributes["NUTS_ID"]?.ToString() ?? "Okänt NUTS‑ID";

                            environmentalDataNutsId += $"NUTS‑ID: {nutsId}";

                            // Hook för framtida statistik
                            if (nutsId.StartsWith("SE"))
                            {
                                var validNuts = new HashSet<string>
        {
            "SE110","SE121","SE122","SE123","SE124","SE125",
            "SE211","SE212","SE213","SE214","SE221","SE224",
            "SE231","SE232","SE311","SE312","SE313","SE321",
            "SE322","SE331","SE332"
        };

                                if (validNuts.Contains(nutsId))
                                {
                                    var age = CountyNutsAgeHandler.GetAgeForNuts(nutsId);

                                    if (age != null)
                                    {
                                        var (meanAge, medianAge) = age.Value;

                                        environmentalDataCountyAgeDistrib += "Åldersstatistik (SCB 2025)";
                                        environmentalDataCountyAgeDistrib += $"\nMedelålder: {meanAge:F2} år";
                                        environmentalDataCountyAgeDistrib += $"\nMedianålder: {medianAge:F2} år";
                                    }
                                }

                            }
                        }
                    }

                    var studies = studyHandler.GetStudiesNearby(lon, lat, studyHandlerTagFilter);

                    if (studies.Count > 0)
                    {
                        environmentalDataNearbyScientificStudies += "Närliggande studier";

                        foreach (var stud in studies)
                        {
                            string author = stud.Attributes["main_author"]?.ToString() ?? "Okänd";
                            string year = stud.Attributes["year"]?.ToString() ?? "?";
                            string doi = stud.Attributes["doi"]?.ToString() ?? "Ingen DOI";

                            environmentalDataNearbyScientificStudies += $"\n{author} ({year}) – {doi}";
                        }
                    }



                    int[] sats = {
    25544,   // ISS
    40697,   // Sentinel-2A
    43689,   // METOP-C
    33591    // NOAA-19
};
                    if (CheckBoxSatelliteData.IsChecked == true)
                    {
                        environmentalDataSateliteData += "Satellitdata\nKälla: N2YO.com – Real-time satellite tracking API\r\n";

                        foreach (var id in sats)
                        {
                            environmentalDataSateliteData += "\n" + await GetSatelliteInfoAsync(id, lat, lon, n2yoSatleiteApiKey);
                        }

                        environmentalDataSateliteData += "\nKälla: N2YO Satellite Tracking API";
                    }

                    int day = DateTime.Now.DayOfYear;
                    var sunrise = GetAverageSunrise(lat, day);
                    string sunriseText = sunrise.ToString("HH:mm");
                    environmentalDataSunRiseTime += $"Beräknad ungefärlig soluppgång: {sunriseText}";
                    var sunset = GetAverageSunset(lat, day);
                    string sunsetText = sunset.ToString("HH:mm");
                    double k = GetShadowFactor(lat, day);
                    var utcNow = DateTime.UtcNow;
                    double moonDist = GetMoonDistanceKm(utcNow);
                    var (antiLat, antiLon) = GetAntipode(lat, lon);
                    SetProgress(50);
                    var (roadNumber, speedLimit, lan) = await GetRoadInfo(lat, lon);
                    if (!roadNumber.StartsWith("Fel"))
                    {
                        environmentalDataTraficData += "Trafikverket";

                        if (!string.IsNullOrWhiteSpace(roadNumber))
                            environmentalDataTraficDataRoadNumb += $"Vägnummer: {roadNumber}";

                        /* if (lan != -1)
                             environmentalData += $"\nLän: {lan}"; already have this*/

                        if (!string.IsNullOrWhiteSpace(speedLimit))
                            environmentalDataTraficDataSpeedLimit += $"Hastighetsgräns: {speedLimit} km/h";
                    }
                    var forecast = await GetWeather(lat, lon);
                    var now = forecast.timeSeries[0];

                    double temp = now.data.air_temperature;
                    double wind = now.data.wind_speed;
                    double gust = now.data.wind_speed_of_gust;
                    double humidity = now.data.relative_humidity;
                    double pressure = now.data.air_pressure_at_mean_sea_level;
                    double visibility = now.data.visibility_in_air;
                    int symbol = now.data.symbol_code;
                    double? totalcc = now.data.cloud_area_fraction;
                    double? lowcc = now.data.low_type_cloud_area_fraction;
                    double? medcc = now.data.medium_type_cloud_area_fraction;
                    double? highcc = now.data.high_type_cloud_area_fraction;
                    double windDir = now.data.wind_from_direction;
                    double? percepRate = now.data.precipitation_rate_mean;
                    double? altCloudBase = now.data.cloud_base_altitude;
                    double? altCloudTop = now.data.cloud_top_altitude;
                    double? precepSort = now.data.precipitation_sort;

                    double stationLon = forecast.geometry.coordinates[0];   // longitude
                    double stationLat = forecast.geometry.coordinates[1];   // latitude
                    double distToSmhiStation = Haversine(lon, lat, stationLon, stationLat);
                    double waterInAir = PhysicsEngine.WaterContent(temp, humidity);
                    double ljudhastighet = 331.3 + 0.606 * temp;
                    environmentalDataSoundSpeed += $"Ljudhastighet: {ljudhastighet:F1} m/s";

                    double dew = 0;

                    environmentalDataWeatherData += $"StationDst: {distToSmhiStation:F4},\nTemperatur: {temp}°C" + $"\nVind: {wind} m/s" + $"\nVindriktning: {windDir}" + $"\nLuftfuktighet: {humidity}%" + $"\nBeräknad vattenhalt i luften: {waterInAir:F2}g/m3" + $"\nLufttryck: {pressure} hPa";
                    if (percepRate != null && percepRate.HasValue && percepRate != 0)
                    {
                        environmentalDataWeatherData += $"\nMedelnederbörsdstakt: {percepRate}";
                    }
                    if (altCloudBase != null && altCloudBase.HasValue && altCloudBase != 0)
                    {
                        environmentalDataWeatherData += $"\nAlltitud molnbas {altCloudBase} m";
                    }
                    if (altCloudTop != null && altCloudTop.HasValue && altCloudTop != 0)
                    {
                        environmentalDataWeatherData += $"\nAlltitud översta molnskiktet {altCloudTop} m";
                    }
                    if (precepSort != null && precepSort.HasValue)
                    {
                        environmentalDataWeatherData += $"\nNederbördstyp {precepSort}";
                    }
                    if (dew != 0)
                        environmentalDataWeatherData += $"\nDaggpunkt: {dew}°C";
                    else
                    {
                        environmentalDataWeatherData += "\nDaggpunkt: saknas";
                        double dewCalc = PhysicsEngine.DewPoint(temp, humidity);
                        dew = dewCalc;
                        environmentalDataWeatherData += $"\nDaggpunkt (beräknad): {dewCalc:F1}°C";
                    }
                    double rho = PhysicsEngine.AirDensityFull(temp, pressure, dew);
                    environmentalDataWeatherData += $"\nLuftdensitet: {rho:F3} kg/m³";
                    double dirRad = windDir * Math.PI / 180.0;
                    double E = 0.5 * rho * wind * wind;

                    double ex = E * Math.Sin(dirRad);
                    double ey = E * Math.Cos(dirRad);
                    double force = PhysicsEngine.CoriolisParameter(lat);
                    double interstit = PhysicsEngine.InertialPeriodHours(lat);
                    double cf = CentrifugalAcceleration(lat);

                    environmentalDataCentrifugalforce += $"Centrifugal acceleration (utifrån latitud): {cf:F4} N/kg";

                    environmentalDataWindKineticEnergy +=
                    $" Vindens kinetiska energi: E: {E} Pa, Ex: {ex} Pa, Ey: {ey} Pa";
                    environmentalDataCorriolisFrequency +=
                    $" Corriolisfrekvens: {force:E4} s⁻¹\"";
                    environmentalDataCorriolisInterstitialPeriod =
                    $" Interstitialperiod: {interstit:E4}";
                    environmentalDataWeatherDataCloudCoverage =
    $"Total molnighet: {totalcc}/8" +
    $"\nMolnighet (låg nivå): {lowcc}/8" +
    $"\nMolnighet (mellannivå): {medcc}/8" +
    $"\nMolnighet (hög nivå): {highcc}/8";
                    SetProgress(60);
                    double solarElevation = SolarCalculator.GetSolarElevation(lat, lon, DateTime.Now);
                    environmentalDataSolarElevaion += $"Solhöjd: {solarElevation:F1}°";
                    double lunarElevation = PhysicsEngine.GetLunarElevation(lat, lon, DateTime.Now);
                    environmentalDataSolarElevaion += $"\nMånhöjd: {lunarElevation:F1}°";


                    environmentalDataSunSetTime += $"Beräknad ungefärlig solnedgång: {sunsetText}" + $"\nSkugglängdsfaktor (k): {k}" + $"\nMånens avstånd (geocentriskt): {moonDist:0} km" + $"\nAntipodpunkt: Lat {antiLat:F5}, Lon {antiLon:F5}";
                    double coastDist = DistanceToCoast(lon, lat);
                    double coastDir = DirectionToCoast(lon, lat);
                    environmentalDataCoastDistance += $"Avstånd till kust: {coastDist:0} meter";
                    environmentalDataCoastBearing += $"Riktning till kust: {coastDir:0}°";
                    bool inReserve = IsInsideNatureReserve(world.X, world.Y);
                    environmentalDataIsNatureReserveBoolean += $"Naturreservat: {(inReserve ? "Ja" : "Nej")}";
                    var om = await OpenMeteoService.GetAllData(lat, lon);
                    float depth = GetDepth(world.X, world.Y);
                    if (depth < 0)
                    {
                        environmentalDataWaterDepth += $"Depth: {depth}";
                    }

                    environmentalDataOpenMeteoHeader += "--- Open‑Meteo ---";

                    if (om.Elevation != null)
                    {
                        environmentalDataMetersAboveSeaLvl += $"Höjd över havet: {om.Elevation:F0} m";
                        double h = om.Elevation.Value; // m
                        double grav = PhysicsEngine.GravityAtLatitude(lat); // m/s^2

                        double EpPerKg = grav * h;

                        environmentalDataGravitationPotential +=
                            $"Gravitationspotential (mark) Ep/m: {EpPerKg:F0} J/kg";
                    }

                    if (om.Aqi != null)
                        environmentalDataAirCompositionData += $"Luftkvalitet (AQI): {om.Aqi}";

                    if (om.Pm25 != null)
                        environmentalDataAirCompositionData += $"\nPM2.5: {om.Pm25} µg/m³";

                    if (om.Pm10 != null)
                        environmentalDataAirCompositionData += $"\nPM10: {om.Pm10} µg/m³";

                    if (om.O3 != null)
                        environmentalDataAirCompositionData += $"\nOzon (O₃): {om.O3} µg/m³";

                    if (om.Birch != null)
                        environmentalDataPollenData += $"Björkpollen: {om.Birch}";

                    if (om.Grass != null)
                        environmentalDataPollenData += $"\nGräspollen: {om.Grass}";

                    if (om.Mugwort != null)
                        environmentalDataPollenData += $"\nGråbopollen: {om.Mugwort}";
                    double? inlineRefVal = null;
                    if (om.UvMaxToday != null)
                    {
                        environmentalDataUv += $"UV-index max idag: {om.UvMaxToday:F1}";
                        inlineRefVal = om.UvMaxToday;
                    }
                    if (om.UvNow != null && om.UvNow != inlineRefVal && om.UvNow > 0)
                        environmentalDataUv += $"\nUV-index nu: {om.UvNow:F1}";

                    if (om.Ammonia != null)
                        environmentalDataAirCompositionData += $"\nAmonia: {om.Ammonia}";
                    if (om.Methane != null)
                        environmentalDataAirCompositionData += $"\nMetan: {om.Methane}";
                    if (om.So2 != null)
                        environmentalDataAirCompositionData += $"\nSO2: {om.So2}";
                    if (om.Co != null)
                        environmentalDataAirCompositionData += $"\nCO: {om.Co}";
                    if (om.Co2 != null)
                        environmentalDataAirCompositionData += $"\nCO2: {om.Co2}";
                    if (om.No2 != null)
                        environmentalDataAirCompositionData += $"\nNo2: {om.No2}";
                    if (om.AerosolOpticalDepth != null)
                        environmentalDataAirCompositionData += $"\nOptiskt aersolärt djup: {om.AerosolOpticalDepth}";
                    var (station, distKm) = FindNearestStation(lat, lon);

                    if (station != null)
                    {
                        var scraped = await PollenScraper.GetForLocationAsync(station.Slug);
                        if (scraped != null)
                        {
                            environmentalDataPollenKollenHeader += "--- Pollenkollen ---";
                            environmentalDataPollenStationName += $"Pollenstation: {station.Name} ({distKm:F1} km)";

                            if (scraped.Alder != null)
                                environmentalDataPollenValues += $"Al: nivå {scraped.Alder} (0–6)";

                            if (scraped.Hazel != null && environmentalDataPollenValues == "")
                            {
                                environmentalDataPollenValues += $"Hassel: nivå {scraped.Hazel} (0–6)";
                            }
                            else
                            {
                                if (scraped.Hazel != null)
                                    environmentalDataPollenValues += $"\nHassel: nivå {scraped.Hazel} (0–6)";
                            }

                            if (scraped.Grass != null && environmentalDataPollenValues == "")
                            {
                                environmentalDataPollenValues += $"Gräs: nivå {scraped.Grass} (0–6)";
                            }
                            else
                            {
                                if (scraped.Grass != null)
                                    environmentalDataPollenValues += $"\nGräs: nivå {scraped.Grass} (0–6)";
                            }

                            if (scraped.Birch != null && environmentalDataPollenValues == "")
                            {
                                environmentalDataPollenValues += $"Björk: nivå {scraped.Birch} (0–6)";
                            }
                            else
                            {
                                if (scraped.Birch != null)
                                    environmentalDataPollenValues += $"\nBjörk: nivå {scraped.Birch} (0–6)";
                            }

                            if (scraped.Willow != null && environmentalDataPollenValues == "")
                            {
                                environmentalDataPollenValues += $"Sälg/Vide: nivå {scraped.Willow} (0–6)";
                            }
                            else
                            {
                                if (scraped.Willow != null)
                                    environmentalDataPollenValues += $"\nSälg/Vide: nivå {scraped.Willow} (0–6)";
                            }

                            if (scraped.Mugwort != null && environmentalDataPollenValues == "")
                            {
                                environmentalDataPollenValues += $"\nGråbo: nivå {scraped.Mugwort} (0–6)";
                            }
                            else
                            {
                                if (scraped.Mugwort != null)
                                    environmentalDataPollenValues += $"\nGråbo: nivå {scraped.Mugwort} (0–6)";
                            }
                        }

                    }
                    if (info.Layer?.Name == "Vägpassager")
                    {
                        var p = info.Feature;
                        string typ = p["VaPassTyp"]?.ToString() ?? "Okänd";
                        string vagtyp = p["Vagtyp"]?.ToString() ?? "Okänd";
                        string vattendrag = p["VaNamn"]?.ToString() ?? "Okänt";
                        string länk = p["Objektlank"]?.ToString() ?? "";
                        string text =
                            "Viltpassage\n" +
                            $"Vägpassage: {typ}\n" +
                            $"Vägtyp: {vagtyp}\n" +
                            $"Vattendrag: {vattendrag}\n" +
                            $"Protokoll E: {länk}";
                        environmentalDataWildLifeRoadPassageClciked += text;
                        environmetalDataLayerSpecfics += environmentalDataWildLifeRoadPassageClciked;
                    }

                    if (info.Layer?.Name == "Slagfält")
                    {
                        var f = info.Feature;

                        string label = f["battleLabel"]?.ToString() ?? "Okänt slag";
                        string date = f["date"]?.ToString()?.Substring(0, 10) ?? "Okänt datum";
                        string participant = f["participantLabel"]?.ToString() ?? "Okänd part";

                        environmentalDataBattleFieldClicked +=
                            $"Slagfält" +
                            $"\nNamn: {label}" +
                            $"\nDatum: {date}" +
                            $"\nParter: {participant}";
                        environmetalDataLayerSpecfics += environmentalDataBattleFieldClicked;
                    }
                    if (info.Layer?.Name == "Grottor")
                    {
                        var f = info.Feature;

                        string nameCave = f["caveLabel"]?.ToString() ?? "Okänd grotta";
                        string wikidata = f["cave"]?.ToString() ?? "";

                        environmentalDataCaveClicked +=
                            $"Grotta" +
                            $"\nNamn: {nameCave}" +
                            $"\nWikidata: {wikidata}";

                        environmetalDataLayerSpecfics += environmentalDataCaveClicked;
                    }
                    if (info.Layer?.Name == "clientPhotoLayer")
                    {
                        var f = info.Feature;

                        string folder = f["folder"]?.ToString() ?? "";

                        if (!string.IsNullOrWhiteSpace(folder))
                        {
                            string folderPath = Path.Combine(
                                AppContext.BaseDirectory,
                                "plugins",
                                "ImageCollections",
                                folder
                            );

                            if (Directory.Exists(folderPath))
                            {
                                var photoWindow = new PhotoWindow(folderPath, startIndex: 0);
                                photoWindow.Show();
                                return;
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Bildmappen hittades inte:\n{folderPath}",
                                    "Bildmapp saknas",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning
                                );
                            }
                        }
                    }

                    if (vattendistrikpluginbool)
                    {
                        var vdName = GetVattendistrikt(world.X, world.Y);

                        if (vdName != null)
                        {
                            environmentalDataVattenDistrikt += $"Vattendistrikt: {vdName}";
                        }
                    }
                    var nearestWind = FindNearestWind(lon, lat);

                    if (nearestWind != null)
                    {
                        var p = nearestWind.Attributes;

                        string namn = p["PROJNAMN"]?.ToString() ?? "Okänt";
                        string status = p["STATUS"]?.ToString() ?? "Okänd";
                        string nav = p["NAVHOJD"]?.ToString() ?? "–";
                        string rotor = p["ROTDIAMETE"]?.ToString() ?? "–";
                        string effekt = p["MAXEFFEKT"]?.ToString() ?? "–";
                        string prod = p["CALPROD"]?.ToString() ?? "–";
                        string bolag = p["ORGNAMN"]?.ToString() ?? "Okänt";
                        string kommun2 = p["KOMNAMN"]?.ToString() ?? "Okänd";
                        string län = p["LANSNAMN"]?.ToString() ?? "Okänt";
                        string placering = p["PLACERING"]?.ToString() ?? "Okänd";
                        string uppd = p["SenasteUpp"]?.ToString() ?? "Okänt";

                        environmentalDataWindEnergyInVicinity +=
                            "Vindkraftverk (närmsta)" +
                            $"\nProjekt: {namn}" +
                            $"\nStatus: {status}" +
                            $"\nPlacering: {placering}" +
                            $"\nKommun: {kommun2}" +
                            $"\nLän: {län}" +
                            $"\nNavhöjd: {nav} m" +
                            $"\nRotordiameter: {rotor} m" +
                            $"\nMaxeffekt: {effekt} MW" +
                            $"\nBeräknad produktion: {prod} GWh/år" +
                            $"\nBolag: {bolag}" +
                            $"\nSenast uppdaterad: {uppd}";
                    }
                    else
                    {
                        environmentalDataWindEnergyInVicinity += "Vindkraftverk\nInga inom 2 km.";
                    }
                    var nearestHeat = FindNearestHeat(lon, lat);
                    if (nearestHeat != null)
                    {
                        var p = nearestHeat.Attributes;
                        string lambda = p["lambdavarde"]?.ToString() ?? "Okänt";
                        envionmentalDataBedrockHeatTransfer = $"värmeledningsförmåga i berggrunden: {lambda} W/(m·K) (mätning inom 1 km)";
                    }

                    var nearestWater = FindNearestWaterPlant(lon, lat);

                    if (nearestWater != null)
                    {
                        var p = nearestWater.Attributes;

                        string namn = p["uwwName"]?.ToString() ?? "Okänt";
                        string lat2 = p["uwwLttd"]?.ToString() ?? "–";
                        string lon2 = p["uwwLngt"]?.ToString() ?? "–";

                        environmentalDataReningsverkInVicinity +=
                            "Reningsverk (närmsta)" +
                            $"\nNamn: {namn}" +
                            $"\nLat: {lat2}" +
                            $"\nLon: {lon2}";
                    }
                    else
                    {
                        environmentalDataReningsverkInVicinity += "Reningsverk\nInga inom 2 km.";
                    }
                    var result = FindNearestNuclear(lon, lat);

                    if (result.feature != null)
                    {
                        var p = result.feature.Attributes;

                        environmentalDataNuclearInVicinity +=
                            "Kärnkraftverk" +
                            $"\nNamn: {p["name"]}" +
                            $"\nAvstånd: {result.dist:F0} m";
                    }
                    else
                    {
                        environmentalDataNuclearInVicinity += "Kärnkraftverk\nInga inom 50 km.";
                    }
                    //

                    //Återkoppla här     
                    if (ChkSeaTemp.IsChecked == true)
                    {
                        var resultW = await SmhiSeaTemperatureAsync(lat, lon);

                        var tempW = resultW.temp;
                        var nearestSea = resultW.station;
                        var distSea = resultW.distance;
                        var allValues = resultW.allValues;

                        // Bygg loggsträng
                        var sb = new StringBuilder();
                        sb.AppendLine("Närmaste havstemperatur");
                        sb.AppendLine($"Station: {nearestSea.name}");
                        sb.AppendLine($"Avstånd: {distSea:F3}° (~{distSea * 111:F1} km)");
                        sb.AppendLine($"Senaste värde: {tempW} °C");
                        sb.AppendLine();
                        sb.AppendLine("Log idag:");

                        foreach (var v in allValues)
                        {
                            sb.AppendLine(
                                $"{v["date"]}: {v["value"]} °C (depth {v["depth"]}, quality {v["quality"]})");
                        }

                        environmentalDataClosestWaterTemp += sb.ToString();
                    }
                    if (ChkSeaSalinity.IsChecked == true)
                    {
                        var resultS = await SmhiSeaSalinityAsync(lat, lon);

                        var sal = resultS.sal;
                        var nearestSal = resultS.station;
                        var distSal = resultS.distance;
                        var allValues = resultS.allValues;

                        var sb = new StringBuilder();
                        sb.AppendLine("Närmaste salthalt");
                        sb.AppendLine($"Station: {nearestSal.name}");
                        sb.AppendLine($"Avstånd: {distSal:F3}° (~{distSal * 111:F1} km)");
                        sb.AppendLine($"Senaste värde: {sal} PSU");
                        sb.AppendLine();
                        sb.AppendLine("Log senaste månaden:");

                        foreach (var v in allValues)
                        {
                            sb.AppendLine(
                                $"{v["date"]}: {v["value"]} PSU (depth {v["depth"]}, quality {v["quality"]})");
                        }

                        environmentalDataClosestOceanSalinity += "\n\n" + sb.ToString();
                    }
                    //
                    var (well, wellDist) = FindNearestWell(world.X, world.Y);

                    if (well != null)
                    {
                        environmentalDataWellsInVicinity += "SGU Brunnar (närmsta)";
                        environmentalDataWellsInVicinity += $"\nAvstånd: {wellDist:F0} m";

                        // tills vidare antar vi property "depth"
                        var whellDepth = well.Attributes["djup_till"]?.ToString() ?? "Okänd";

                        environmentalDataWellsInVicinity += $"\nDjup: {whellDepth} m";
                    }
                    var nearestChurch = FindNearestChurch(world.X, world.Y);

                    if (nearestChurch != null)
                    {
                        environmentalDataChurchesInVicinity += "Kyrka";

                        double dist = nearestChurch.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataChurchesInVicinity += $"\nAvstånd: {dist:F0} m";
                    }

                    //
                    var resultair = FindNearestAirport(lon, lat);
                    if (resultair.feature != null)
                    {
                        string nameair = resultair.feature.Attributes["airportLabel"]?.ToString() ?? "Okänd";
                        string iata = resultair.feature.Attributes["iata"]?.ToString() ?? "-";
                        string icao = resultair.feature.Attributes["icao"]?.ToString() ?? "-";

                        environmentalDataAirportInVicinity +=
                            $"Närmaste flygplats: {nameair} ({resultair.dist:F1} m)" +
                            $"\nIATA: {iata}, ICAO: {icao}";
                    }

                    string moonPhase = GetMoonPhase(DateTime.UtcNow); environmentalDataCurrentMoonPhase += $"Månfas: {moonPhase}";
                    SetProgress(80);
                    var lc = FindLandcoverPolygon(lon, lat);

                    if (lc != null)
                    {
                        // Läs direkt från GeoJSON-attributet
                        string marktacke = lc.Attributes["class_name"]?.ToString() ?? "Okänd";

                        environmentalDataNerbyLandCoverage +=
                            "Marktäcke" +
                            $"\nTyp: {marktacke}";
                    }
                    var berg = GetBerggrundFeature(world.X, world.Y);
                    if (berg != null)
                    {
                        var attr = berg.Attributes;
                        string litologi = attr["litologi"]?.ToString() ?? "Okänd";
                        string tekt = attr["tekt_enhet"]?.ToString() ?? "Okänd";
                        string under = attr["underenhet"]?.ToString() ?? "Okänd";
                        string legend = attr["legend"]?.ToString() ?? "Okänd";
                        environmentalDataLocalBedgroundDescription +=
                            $"Berggrund" +
                            $"\nLitologi: {litologi}" +
                            $"\nTektonisk enhet: {tekt}" +
                            $"\nUnderenhet: {under}" +
                            $"\nKlassificering: {legend}";
                    }
                    //tätorter
                    Log.Info("Query", "Tätort");
                    var tatort = GetTatort(world.X, world.Y);
                    if (tatort != null)
                    {
                        var attr = tatort.Attributes;
                        string local_name = attr["tatort"]?.ToString() ?? "Missing";
                        string local_code = attr["tatortskod"]?.ToString() ?? "Missing";
                        environmetnalDataTatort = $"\n[TÄTORT]\n{local_name}\nKod: {local_code}";
                        var count = GetHashtagCount(local_name);

                        if (count != null)
                        { //Bör bli separat stadslager eftersom annars andra tätorter kan ta upp klicket och dölja matchning
                            environmentaldataIgHashtags =
                                $"\n[STAD]: {local_name}\nAntal IG hashtags: #{local_name}: {count}";
                        }
                    }

                    //småorter
                    Log.Info("Query", "Småort");
                    var smaort = GetSmaort(world.X, world.Y);
                    if (smaort != null)
                    {
                        var attr = smaort.Attributes;
                        string local_code = attr["smaort"]?.ToString() ?? "Missing";
                        environmetnalDataSmaort = $"\n[SMÅORT]\nKod: {local_code}";

                    }

                    var bioReg = GetBioRegFeatures(world.X, world.Y);
                    if (bioReg != null)
                    {
                        var iu = bioReg.Attributes;
                        string bioRegStr = iu["ngregion"]?.ToString() ?? "Okänd";
                        environmentalDataBiogeographicalRegion += $"Biogeografisk region: {bioRegStr}";
                    }
                    var jord = GetJordartFeature(world.X, world.Y);
                    if (jord != null)
                    {
                        var a = jord.Attributes;
                        string jordart = a["jg2_tx"]?.ToString() ?? "Okänd";
                        environmentalDataSoilType += $"Jordart: {jordart}";
                    }
                    var geo = FindNearestGeokemi(world.X, world.Y);
                    if (geo != null)
                    {
                        var p = geo.Attributes;

                        environmentalDataLocalGeochemistry += "Geokemi (närmsta prov)";

                        foreach (var key in p.GetNames())
                        {
                            var val = p[key]?.ToString() ?? "–";
                            environmentalDataLocalGeochemistry += $"\n{key}: {val}";
                        }
                    }
                    else
                    {
                        environmentalDataLocalGeochemistry += "Geokemi\nInga prov inom 10 km.";
                    }
                    //ComputeGeokemiStats(); //tmp calculus

                    var power = FindNearestPowerTower(world.X, world.Y);

                    if (power != null)
                    {
                        var p = power.Attributes;

                        environmentalDataPowerTowerInVicinity += "Kraftnät (närmsta objekt)\nTorn&Stolpar:";

                        foreach (var key in p.GetNames())
                        {
                            var val = p[key]?.ToString() ?? "–";
                            environmentalDataPowerTowerInVicinity += $"\n{key}: {val}";
                        }

                        // Avstånd i meter
                        double dist = power.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataPowerTowerInVicinity += $"\nAvstånd: {dist:F0} m";
                    }
                    var cable = FindNearestCable(world.X, world.Y);

                    if (cable != null)
                    {
                        var p = cable.Attributes;

                        environmentalDataPowerCableInVicinity += "Kraftkablar:";

                        foreach (var key in p.GetNames())
                        {
                            var val = p[key]?.ToString() ?? "–";
                            environmentalDataPowerCableInVicinity += $"\n{key}: {val}";
                        }

                        double dist = cable.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataPowerCableInVicinity += $"\nAvstånd: {dist:F0} m";
                    }

                    var fStation = FindNearestFireStation(world.X, world.Y);

                    if (fStation != null)
                    {
                        var f = fStation.Attributes;

                        environmentalDataFireStationInVicinity += "Brandstation (närmsta station)";

                        // Avstånd i meter
                        double dist = fStation.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataFireStationInVicinity += $"\nAvstånd: {dist:F0} m";
                    }
                    var harbour = FindNearestHarbour(world.X, world.Y);

                    if (harbour != null)
                    {
                        double distHarbour = harbour.Geometry.Distance(new Point(world.X, world.Y));

                        environmentalDataHarbourInVicinity += "Närmaste hamn";
                        environmentalDataHarbourInVicinity += $"\nAvstånd: {distHarbour:F0} m";
                    }
                    else
                    {
                        environmentalDataHarbourInVicinity += "Närmaste hamn";
                        environmentalDataHarbourInVicinity += "\nInget inom 2.5 mils radie";
                    }
                    var edu = FindNearestEducation(world.X, world.Y);

                    if (edu != null)
                    {
                        double distEdu = edu.Geometry.Distance(new Point(world.X, world.Y));

                        environmentalDataEducationFacilityInVicinity += "Närmaste utbildningscenter";
                        environmentalDataEducationFacilityInVicinity += $"\nAvstånd: {distEdu:F0} m";
                    }
                    else
                    {
                        environmentalDataEducationFacilityInVicinity += "Närmaste utbildningscenter";
                        environmentalDataEducationFacilityInVicinity += "\nInget inom 2.5 mils radie";
                    }

                    var health = FindNearestHealth(world.X, world.Y);

                    if (health != null)
                    {
                        double distHealth = health.Geometry.Distance(new Point(world.X, world.Y));

                        environmentalDataHealthFacilityInVicinity += "Närmaste vårdcentral / hälsocenter";
                        environmentalDataHealthFacilityInVicinity += $"\nAvstånd: {distHealth:F0} m";
                    }
                    else
                    {
                        environmentalDataHealthFacilityInVicinity += "Närmaste vårdcentral / hälsocenter";
                        environmentalDataHealthFacilityInVicinity += "\nInget inom 2.5 mils radie";
                    }

                    float k_value = GetHydraulicK(world.X, world.Y);

                    if (!float.IsNaN(k_value))
                    {
                        environmentalDataHydraulicKInbedground += "Hydraulisk konduktivitet (SGU)";
                        environmentalDataHydraulicKInbedground += $"\nK-värde: {k_value}";
                    }
                    //A
                    var ridge = FindNearestRidge(world.X, world.Y);

                    if (ridge != null)
                    {
                        environmentalDataIceRidgeInVcinity += "Isräfflor (närmsta)";

                        double dist = ridge.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataIceRidgeInVcinity += $"\nAvstånd: {dist:F0} m";

                        // Property "riktning"
                        var direction = ridge.Attributes["riktn"]?.ToString() ?? "Okänd";
                        environmentalDataIceRidgeInVcinity += $"\nRiktning: {direction}°";
                    }
                    //
                    var soil_depth = FindNearestSoilDepth(world.X, world.Y);

                    if (soil_depth != null)
                    {
                        var d = soil_depth.Attributes;
                        string depthTerra = d["djup"]?.ToString() ?? "Okänt";

                        environmentalDataSoilDepthInVicinity += "Jorddjup (närmsta station)";

                        // Avstånd i meter
                        double dist = soil_depth.Geometry.Distance(new Point(world.X, world.Y));
                        environmentalDataSoilDepthInVicinity += $"\nAvstånd: {dist:F0} m";
                        if (depthTerra != null)
                        {
                            if (depthTerra != "Okänt")
                            {
                                environmentalDataSoilDepthInVicinity += $"\n Djup: {depthTerra} m";
                            }
                        }
                    }
                    //A
                    var gv = GetGrundvattenFeature(world.X, world.Y);

                    if (gv != null)
                    {
                        var a = gv.Attributes;
                        string namn = a["magasinsnamn"]?.ToString() ?? "Okänt";
                        string beskrivning = a["lank_magasinsbeskrivning"]?.ToString() ?? "Ingen beskrivning";
                        string bildning = a["grvbildningstyp"]?.ToString() ?? "Okänd";
                        string akvifer = a["akvifertyp"]?.ToString() ?? "Okänd";
                        string position = a["magasinsposition"]?.ToString() ?? "Okänd";
                        string ursprung = a["genes"]?.ToString() ?? "Okänt";
                        string infiltration = a["infiltrationsmojligheter"]?.ToString() ?? "Okänt";
                        environmentalDataGroundWater +=
                            "Grundvattenmagasin" +
                            $"\nNamn: {namn}" +
                            $"\nBildningstyp: {bildning}" +
                            $"\nAkvifertyp: {akvifer}" +
                            $"\nPosition: {position}" +
                            $"\nGeologiskt ursprung: {ursprung}" +
                            $"\nInfiltrationsmöjligheter: {infiltration}";

                        if (!string.IsNullOrWhiteSpace(beskrivning) && beskrivning != "NULL")
                            environmentalDataGroundWater += $"\nBeskrivning/länk: {beskrivning}";
                    }
                    var nearestSound = FindNearestSound(lon, lat);
                    if (nearestSound == null)
                    {
                        environmentalDataRecordedAudioSound += "Ljudlandskap: Inga inspelningar inom 1 km.";
                    }
                    else
                    {
                        environmentalDataRecordedAudioSound +=
                            $"Ljudlandskap: Inspelning {nearestSound.Time:yyyy-MM-dd HH:mm}" +
                            $"\nTryck [Play] för att lyssna.";
                        // Spara för senare uppspelning
                        lastSoundPath = nearestSound.Path;
                    }
                    double g = PhysicsEngine.GravityAtLatitude(lat);
                    environmentalDataGravitation += $"Gravitation: {g:F4} m/s²"; //already have this one?


                    var (riverDist, nearestPoint) = FindNearestRiver(lon, lat);


                    var terrain = await TerrainService.GetTerrainData(lat, lon);
                    environmentalData += "Terräng";
                    //if (terrain.Elevation != null) environmentalData += $"\nHöjd: {terrain.Elevation:F0} m";
                    if (terrain.SlopeDegrees != null) environmentalDataTerrain += $"\nLutning (r=300m): {terrain.SlopeDegrees:F1}°";
                    if (terrain.AspectDegrees != null) environmentalDataTerrain += $"\nRiktning: {terrain.AspectDirection} ({terrain.AspectDegrees:F0}°)";
                    if (terrain.Ndvi != null) environmentalDataLocalNdvi += $"\nNDVI: {terrain.Ndvi:F2}";
                    var (land1, water1) = CalculateLandWaterIndex(lon, lat, 1000);
                    var (land10, water10) = CalculateLandWaterIndex(lon, lat, 10000);



                    environmentalDataLandWaterRatio += "Land/Sötvatten-index";
                    environmentalDataLandWaterRatio += $"\n1 km: Land {land1:F1}%, Sötvatten {water1:F1}%";
                    environmentalDataLandWaterRatio += $"\n10 km: Land {land10:F1}%, Sötvatten {water10:F1}%";
                    environmentalDataClosestRiver += "";

                    if (double.IsNaN(riverDist) || double.IsInfinity(riverDist))
                    {
                        environmentalDataClosestRiver += "Närmaste flod: (fel – kontrollera koordinatsystem)";
                    }
                    else
                    {
                        environmentalDataClosestRiver += $"Närmaste flod - avstånd: {riverDist:F0} m";

                        if (nearestPoint != null)
                        {
                            environmentalDataClosestRiver += $"\nNärmaste flod - närmaste punkt: X={nearestPoint.X:F2}, Y={nearestPoint.Y:F2}";
                        }
                    }

                    int ldi = CalculateLandscapeDiversity(lon, lat);
                    environmentalDataLocalLandscapeDiversity += $"Landskapsdiversitet (1 km): {ldi} klasser";
                    var name = info.Feature["name"]?.ToString() ?? "Okänd plats";
                    var categoryRaw = info.Feature["category"]?.ToString() ?? "Okänd kategori";
                    var category = Capitalize(categoryRaw);
                    var description = info.Feature["description"]?.ToString() ?? "Ingen beskrivning";
                    var picture = info.Feature["picture"]?.ToString() ?? "";
                    var icon = info.Feature["icon"]?.ToString() ?? "";
                    var coord = info.Feature["coord"]?.ToString() ?? "";
                    environmentalData = BuildEnvironmentalOutput();
                    string BuildEnvironmentalOutput()
                    {
                        var sb = new StringBuilder();

                        // LAND
                        sb.AppendLine(environmentalDataCountryName);
                        if (environmentalDataCountryName != null || environmentalDataCountryName == "?" || environmentalDataCountryName == "")
                        {
                            sb.AppendLine("Country code: SE");
                            sb.AppendLine("Area: xxx kvkm");
                        }

                        // REGION
                        sb.AppendLine("\n[REGION]");
                        sb.AppendLine(environmentalDataRegion);

                        // LÄN
                        sb.AppendLine("\n[LÄN]");
                        sb.AppendLine(environmentalDataCountyName);
                        sb.AppendLine(environmentalDataCountyCode);
                        sb.AppendLine(environmentalDataCountyArea);
                        sb.AppendLine(environmentalDataCountyNumbOfMunicipals);
                        sb.AppendLine(environmentalDataCountyPopulation);
                        sb.AppendLine(environmentalDataCountyAgeDistrib);
                        sb.AppendLine(environmentalDataCountyForeignBorn);
                        sb.AppendLine(environmentalDataCountyHousholdCount);
                        sb.AppendLine(environmentalDataCountyCarDensity);
                        sb.AppendLine(environmentalDataCountyTraficDeaths);
                        sb.AppendLine(environemntalDataCountyCarUsage);
                        sb.AppendLine(environmentalDataCountyMcUsage);
                        sb.AppendLine(environmentalDataCountyTruckUsage);
                        sb.AppendLine(environmentalDataCountyBusUsage);
                        sb.AppendLine(environmentalDataCountyCarFuel);
                        sb.AppendLine(environmentalDataCountyCats);
                        sb.AppendLine(environmentalDataCountyDogs);
                        sb.AppendLine(environmentalDataCountyHorses);
                        sb.AppendLine(environmentalDataCountyCattle);
                        sb.AppendLine(environmentalDataCountyHarvest);
                        sb.AppendLine(environmentalDataCountyElectricity);
                        sb.AppendLine(environmentalDataCountyCostalArea);
                        sb.AppendLine(environmentalDataRegionSicknesStats);
                        sb.AppendLine(environmentalDataCountyTbe);
                        sb.AppendLine(environmentalDataCountyHarpest);
                        sb.AppendLine(environmentalDataStress);

                        // KOMMUN
                        sb.AppendLine("\n[KOMMUN]");
                        sb.AppendLine(environmentalDataKommunNamn);
                        sb.AppendLine(environmentalDataKommunKod);
                        sb.AppendLine(environmentalDataKommunCrimStats);
                        sb.AppendLine(environmentalDataKommunMaleMeanLife);
                        sb.AppendLine(environmetalDataKidNames);
                        sb.AppendLine(environmentalDataSickLeave);
                        if (!string.IsNullOrWhiteSpace(environmentalDataSpecificInfo))
                            sb.AppendLine(environmentalDataSpecificInfo);
                        sb.AppendLine("\n[LA-OMRÅDE]");
                        sb.AppendLine(environmentalDataLaArea);
                        sb.AppendLine("\n[DEMOGRAFIKT STATISTIKOMRÅDE]");
                        sb.AppendLine(environmentalDataDeSo);
                        if (!string.IsNullOrWhiteSpace(environmetnalDataTatort))
                            sb.AppendLine(environmetnalDataTatort);
                        if (!string.IsNullOrWhiteSpace(environmetnalDataSmaort))
                            sb.AppendLine(environmetnalDataSmaort);
                        sb.AppendLine("\n[SOCKEN]");
                        sb.AppendLine(environmentalDataSockenName);

                        // STAD
                        if (!string.IsNullOrWhiteSpace(environmentaldataIgHashtags))
                            sb.AppendLine(environmentaldataIgHashtags);

                        // LOKALDATA
                        sb.AppendLine("\n[LOKALDATA]");
                        sb.AppendLine(environmentalDataPostCodes);
                        sb.AppendLine(environmentalDataLandsdel);
                        sb.AppendLine(environmentalDataNutsId);
                        sb.AppendLine(environmentalDataCivo);
                        sb.AppendLine(environmentalDataMetersAboveSeaLvl);

                        // TEMATISKA KATEGORIER
                        sb.AppendLine("\n> Geologi");
                        sb.AppendLine(environmentalDataArsenik);
                        sb.AppendLine(environmentalDataSoilType);
                        sb.AppendLine(environmentalDataSoilDepthInVicinity);
                        sb.AppendLine(environmentalDataLocalGeochemistry);
                        sb.AppendLine(environmentalDataHydraulicKInbedground);
                        if (!string.IsNullOrWhiteSpace(environmentalDataSateliteData))
                            sb.AppendLine(envionmentalDataBedrockHeatTransfer);
                        sb.AppendLine(environmentalDataGroundWater);
                        sb.AppendLine(environmentalDataLocalBedgroundDescription);

                        sb.AppendLine("\n> Biogeografi");
                        sb.AppendLine(environmentalDataBiogeographicalBorders);
                        sb.AppendLine(environmentalDataBiogeographicalRegion);
                        sb.AppendLine(environmentalDataIsNatureReserveBoolean);
                        sb.AppendLine(environmentalDataLocalLandscapeDiversity);
                        sb.AppendLine(environmentalDataLocalNdvi);

                        sb.AppendLine("\n> Infrastruktur");
                        sb.AppendLine(environmentalDataTraficData);
                        sb.AppendLine(environmentalDataTraficDataRoadNumb);
                        sb.AppendLine(environmentalDataTraficDataSpeedLimit);
                        sb.AppendLine(environmentalDataAirportInVicinity);
                        sb.AppendLine(environmentalDataHarbourInVicinity);
                        sb.AppendLine(environmentalDataWindEnergyInVicinity);
                        sb.AppendLine(environmentalDataNuclearInVicinity);
                        sb.AppendLine(environmentalDataWellsInVicinity);
                        sb.AppendLine(environmentalDataChurchesInVicinity);
                        sb.AppendLine(environmentalDataFireStationInVicinity);
                        sb.AppendLine(environmentalDataPowerTowerInVicinity);
                        sb.AppendLine(environmentalDataPowerCableInVicinity);
                        sb.AppendLine(environmentalDataReningsverkInVicinity);
                        sb.AppendLine(environmentalDataHealthFacilityInVicinity);
                        sb.AppendLine(environmentalDataEducationFacilityInVicinity);

                        sb.AppendLine("\n> Väder");
                        sb.AppendLine(environmentalDataWeatherData);
                        sb.AppendLine(environmentalDataWeatherDataCloudCoverage);
                        sb.AppendLine(environmentalDataUv);
                        if (!string.IsNullOrWhiteSpace(environmentalDataPollenData))
                            sb.AppendLine(environmentalDataPollenData);
                        sb.AppendLine(environmentalDataSunRiseTime);
                        sb.AppendLine(environmentalDataSunSetTime);
                        sb.AppendLine(environmentalDataCurrentMoonPhase);
                        sb.AppendLine(environmentalDataAirCompositionData);
                        sb.AppendLine(environmentalDataPollenStationName);
                        sb.AppendLine(environmentalDataPollenValues);


                        sb.AppendLine("\n> Hydrologi");
                        sb.AppendLine(environmentalDataWaterDepth);
                        sb.AppendLine(environmentalDataCoastDistance);
                        sb.AppendLine(environmentalDataCoastBearing);
                        sb.AppendLine(environmentalDataClosestRiver);
                        if (!string.IsNullOrWhiteSpace(environmentalDataClosestWaterTemp))
                            sb.AppendLine(environmentalDataClosestWaterTemp);
                        if (!string.IsNullOrWhiteSpace(environmentalDataClosestOceanSalinity))
                            sb.AppendLine(environmentalDataClosestOceanSalinity);

                        sb.AppendLine("\n> Energi & Fysik");
                        sb.AppendLine(environmentalDataWindKineticEnergy);
                        sb.AppendLine(environmentalDataCorriolisFrequency);
                        sb.AppendLine(environmentalDataCorriolisInterstitialPeriod);
                        sb.AppendLine(environmentalDataGravitation);
                        sb.AppendLine(environmentalDataGravitationPotential);
                        sb.AppendLine(environmentalDataCentrifugalforce);
                        sb.AppendLine(environmentalDataSolarElevaion);
                        sb.AppendLine(environmentalDataSoundSpeed);

                        sb.AppendLine("\n> Studier & Satelliter");
                        if (!string.IsNullOrWhiteSpace(environmentalDataNearbyScientificStudies))
                            sb.AppendLine(environmentalDataNearbyScientificStudies);
                        if (!string.IsNullOrWhiteSpace(environmentalDataSateliteData))
                            sb.AppendLine(environmentalDataSateliteData);

                        //LANDSKAP
                        sb.AppendLine("\n[LANDSKAP]");
                        if (!string.IsNullOrWhiteSpace(environmentalDataHeat))
                            sb.AppendLine(environmentalDataHeat);
                        if (!string.IsNullOrWhiteSpace(environmentalDataFireClass))
                            sb.AppendLine(environmentalDataFireClass);
                        sb.AppendLine(environmentalDataTerrain);
                        sb.AppendLine(environmentalDataVattenDistrikt);
                        sb.AppendLine(environmentalDataIceRidgeInVcinity);
                        sb.AppendLine(environmetalDataLayerSpecfics);
                        sb.AppendLine(environmentalDataLandWaterRatio); //should be improved maybe by selectve reading and higher resolution

                        //ARTER
                        sb.AppendLine("\n[RAPPORTERADE ARTER]");
                        sb.AppendLine(environmentalDataArtDataBanken);

                        //KOORDINATER
                        sb.AppendLine("\n[KOORDINATER]");
                        sb.AppendLine(environmentalDataCoord);

                        return sb.ToString();
                    }

                    lastInfoWindowText = environmentalData;
                    SetProgress(95);
                    var window = new InfoWindow(name, category, description, picture, icon, coord, lat, lon, environmentalData);
                    HideProgress();
                    openInfoWindows.Add(window);
                    window.Closed += (_, __) => openInfoWindows.Remove(window);
                    if (openInfoWindows.Count > MaxInfoWindows) { openInfoWindows[0].Close(); openInfoWindows.RemoveAt(0); }
                    window.Show();
                    window.CopilotRequested += (text) => { OpenCopilotWithContext(text); };
                    window.PlaySoundRequested += OnPlaySoundRequested;

                };
                string before = GetRamUsage();
                GC.Collect();
                string after = GetRamUsage();
                Log.Info("Garbage Collector", $"RAM before {before}, RAM after{after}");

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FEL I KONSTRUKTORN");
            }
            Loaded += async (_, __) =>
            {
                await TimeSerieLauncher("data/geojson/parkslide.geojson"); //debugg.geojson
            };
        }

        private void ToggleCastleLayer(object sender, System.Windows.RoutedEventArgs e)
        {
            if (castleLayer == null) return;
            castleLayer.Enabled = !castleLayer.Enabled
                ; mapControl.Refresh();
        }
        private void Print(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Rendera hela fönstret till en bitmap
                var target = Application.Current.MainWindow;
                var bounds = VisualTreeHelper.GetDescendantBounds(target);

                var dpi = 96d;
                var rtb = new RenderTargetBitmap(
                    (int)bounds.Width,
                    (int)bounds.Height,
                    dpi, dpi,
                    PixelFormats.Pbgra32);

                var dv = new DrawingVisual();
                using (var dc = dv.RenderOpen())
                {
                    var vb = new VisualBrush(target);
                    dc.DrawRectangle(vb, null, new System.Windows.Rect(new System.Windows.Point(), bounds.Size));
                }

                rtb.Render(dv);

                // Skapa ett dokument för utskrift
                var doc = new FixedDocument();
                var pageContent = new PageContent();
                var fixedPage = new FixedPage();

                var image = new System.Windows.Controls.Image
                {
                    Source = rtb,
                    Width = printDialog.PrintableAreaWidth,
                    Height = printDialog.PrintableAreaHeight
                };

                FixedPage.SetLeft(image, 0);
                FixedPage.SetTop(image, 0);

                fixedPage.Children.Add(image);
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);

                printDialog.PrintDocument(doc.DocumentPaginator, "GIS Print");
            }
        }
        private void OpenMoveBank_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void ToggleBotanicalLayer(object sender, System.Windows.RoutedEventArgs e)
        {
            if (botanicalGardensLayer == null) return;
            botanicalGardensLayer.Enabled = !botanicalGardensLayer.Enabled;
            mapControl.Refresh();
        }
        private void ToggleWalkingRoutes(object sender, System.Windows.RoutedEventArgs e)
        {
            if (walkingRoutesLayer == null) return;

            walkingRoutesLayer.Enabled = !walkingRoutesLayer.Enabled;
            mapControl.Refresh();
        }
        private void MapControl_MouseMove(object sender, MouseEventArgs e) //added using system windows input
        {
            var pos = e.GetPosition(mapControl);
            // Skärm → värld
            var worldPos = mapControl.Map.Navigator.Viewport.ScreenToWorld(pos.X, pos.Y); //ViewPort not recognized
            // Värld (Spherical Mercator) → lat/lon
            var lonLat = SphericalMercator.ToLonLat(worldPos.X, worldPos.Y);
            MouseCoordText.Text = $"Lon: {lonLat.lon:F5}, Lat: {lonLat.lat:F5}";
            HighlightNearestIcon(worldPos);
            UpdateFeatureTooltip(worldPos);
            //
            if (measuring && measureStart != null)
            {
                var startLonLat = SphericalMercator.ToLonLat(measureStart.X, measureStart.Y);
                var endLonLat = SphericalMercator.ToLonLat(worldPos.X, worldPos.Y);
                double dist = Haversine(startLonLat.lat, startLonLat.lon, endLonLat.lat, endLonLat.lon);
                string distText = dist < 1000 ? $"{dist:F0} m" : $"{dist / 1000:F2} km";
                LiveDistanceText.Text = distText;
                LiveDistanceText.Visibility = System.Windows.Visibility.Visible;
                DrawMeasureLine(measureStart, worldPos);
            }
        }
        private void EnforceBounds(object sender, EventArgs e)
        {
            var vp = mapControl.Map.Navigator.Viewport;

            var nav = mapControl.Map.Navigator;
            const double MaxResolution = 10000;

            if (nav.Viewport.Resolution > MaxResolution)
            {
                nav.ZoomTo(MaxResolution);
            }

            double x = Math.Max(bounds.MinX, Math.Min(vp.CenterX, bounds.MaxX));
            double y = Math.Max(bounds.MinY, Math.Min(vp.CenterY, bounds.MaxY));

            if (x != vp.CenterX || y != vp.CenterY)
            {
                // Skjut upp korrigeringen tills efter eventet är klart
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mapControl.Map.Navigator.CenterOn(new MPoint(x, y));
                }));
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!_cinematicMode)
                    {
                        RedrawChurchIcons();
                        RedrawRuneIcons();
                        RedrawBeachIcons();
                        RedrawFossilIcons();
                        RedrawStatueIcons();
                    }
                    UpdateUserLocationIcon();
                    UpdateScaleBar();
                    UpdateCameraText();
                    var res = mapControl.Map.Navigator.Viewport.Resolution;
                    ZoomIndicator.Text = $"Resolution: {res:F2} m/px";
                    minimap.Map.Navigator.CenterOn(mapControl.Map.Navigator.Viewport.CenterX, mapControl.Map.Navigator.Viewport.CenterY);
                    UpdateMiniMapViewportBox();
                }));
            }
        }
        private IFeature? FindNearestArsenicPoint(double worldX, double worldY)
        {
            IFeature? nearest = null;
            double minDist = double.MaxValue;

            foreach (var f in arsenicCollection)
            {
                if (f.Geometry is MultiPoint mp && mp.NumGeometries > 0)
                {
                    var pt = mp.Geometries[0] as Point;
                    if (pt == null) continue;

                    double dx = pt.X - worldX;
                    double dy = pt.Y - worldY;
                    double dist = dx * dx + dy * dy;

                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = f;
                    }
                }
            }

            return nearest;
        }
        private void StartMeasure_Click(object sender, RoutedEventArgs e)
        {
            measuring = true;
            measureStart = null;
        }
        private double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // jordens radie i meter

            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            lat1 = lat1 * Math.PI / 180.0;
            lat2 = lat2 * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // meter
        }
        private void DrawMeasureLine(MPoint start, MPoint end)
        {
            overlayCanvas.Children.Clear();

            var vp = mapControl.Map.Navigator.Viewport;

            var s = vp.WorldToScreen(start.X, start.Y);
            var e = vp.WorldToScreen(end.X, end.Y);

            var line = new System.Windows.Shapes.Line
            {
                X1 = s.X,
                Y1 = s.Y,
                X2 = e.X,
                Y2 = e.Y,
                Stroke = System.Windows.Media.Brushes.Yellow,
                StrokeThickness = 2,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 4, 4 }
            };
            overlayCanvas.Children.Add(line);
        }
        private void RedrawChurchIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            // Om vi inte har skapat några bilder ännu: gör det en gång
            if (churchImageControls.Count == 0)
            {
                iconCanvas.Children.Clear();

                foreach (var (world, iconPath) in churchIcons)
                {
                    var screen = vp.WorldToScreen(world.X, world.Y);

                    var img = new Image
                    {
                        Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                        Width = 24,
                        Height = 24,
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(img, screen.X - img.Width / 2);
                    Canvas.SetTop(img, screen.Y - img.Height / 2);

                    churchImageControls.Add(img);
                    iconCanvas.Children.Add(img);
                }
                return;
            }

            // Här återanvänder vi bara befintliga bilder och flyttar dem
            for (int i = 0; i < churchIcons.Count && i < churchImageControls.Count; i++)
            {
                var (world, _) = churchIcons[i];
                var img = churchImageControls[i];

                var screen = vp.WorldToScreen(world.X, world.Y);

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
        }
        private void RedrawRuneIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            // Om inga run-ikoner ännu → skapa dem
            if (runeImageControls.Count == 0)
            {
                foreach (var (world, iconPath) in runeIcons)
                {
                    var screen = vp.WorldToScreen(world.X, world.Y);

                    var img = new Image
                    {
                        Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                        Width = 24,
                        Height = 24,
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(img, screen.X - img.Width / 2);
                    Canvas.SetTop(img, screen.Y - img.Height / 2);

                    runeImageControls.Add(img);
                    iconCanvas.Children.Add(img);
                }

                return;
            }

            // Annars: flytta befintliga ikoner
            for (int i = 0; i < runeIcons.Count && i < runeImageControls.Count; i++)
            {
                var (world, _) = runeIcons[i];
                var img = runeImageControls[i];

                var screen = vp.WorldToScreen(world.X, world.Y);

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
        }
        private void CreateChurchIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            // Om viewport inte är redo ännu – avbryt
            if (double.IsNaN(vp.Width) || double.IsNaN(vp.Height) ||
                vp.Width <= 0 || vp.Height <= 0)
                return;

            iconCanvas.Children.Clear();
            churchImageControls.Clear();

            foreach (var (world, iconPath) in churchIcons)
            {
                var screen = vp.WorldToScreen(world.X, world.Y);

                if (double.IsNaN(screen.X) || double.IsNaN(screen.Y) ||
                    double.IsInfinity(screen.X) || double.IsInfinity(screen.Y))
                    continue;

                var img = new Image
                {
                    Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                    Width = 24,
                    Height = 24,
                    IsHitTestVisible = false,
                    RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
                };
                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
                churchImageControls.Add(img);
                iconCanvas.Children.Add(img);
            }
        }
        private void HighlightNearestIcon(MPoint mouseWorld)
        {
            const double maxPixelDistance = 20; // hur nära musen man måste vara
            var vp = mapControl.Map.Navigator.Viewport;
            Image closestImg = null;
            double closestDist = double.MaxValue;
            for (int i = 0; i < churchIcons.Count; i++)
            {
                var (world, _) = churchIcons[i];
                var img = churchImageControls[i];
                var screen = vp.WorldToScreen(world.X, world.Y);
                double dx = screen.X - Mouse.GetPosition(mapControl).X;
                double dy = screen.Y - Mouse.GetPosition(mapControl).Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestImg = img;
                }
                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
            for (int i = 0; i < churchImageControls.Count; i++)
            {
                var img = churchImageControls[i];
                img.Width = 24;
                img.Height = 24;
                var (world, _) = churchIcons[i];
                var screenReset = vp.WorldToScreen(world.X, world.Y);
                Canvas.SetLeft(img, screenReset.X - img.Width / 2);
                Canvas.SetTop(img, screenReset.Y - img.Height / 2);
            }
            // Highlight nearest if within threshold
            if (closestImg != null && closestDist < maxPixelDistance)
            {
                closestImg.Width = 32;
                closestImg.Height = 32;
                var vp2 = mapControl.Map.Navigator.Viewport;
                var index = churchImageControls.IndexOf(closestImg);
                var (world3, _) = churchIcons[index];
                var screen3 = vp2.WorldToScreen(world3.X, world3.Y);
                Canvas.SetLeft(closestImg, screen3.X - closestImg.Width / 2);
                Canvas.SetTop(closestImg, screen3.Y - closestImg.Height / 2);
            }
        }
        private void CreateRuneIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            if (double.IsNaN(vp.Width) || vp.Width <= 0) return;

            runeImageControls.Clear();

            foreach (var (world, iconPath) in runeIcons)
            {
                var screen = vp.WorldToScreen(world.X, world.Y);

                var img = new Image
                {
                    Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                    Width = 24,
                    Height = 24,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);

                runeImageControls.Add(img);
                iconCanvas.Children.Add(img);
            }
        }
        private void InitOnlineSearchUI()
        {
            // Enkel offline-koll
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                SearchPanel.Visibility = System.Windows.Visibility.Collapsed;
                onlineSearchAvailable = false;
                return;
            }
            // Sätt User-Agent (krav från Nominatim)
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GeoViewSE/1.0 (knutssongustafssonwilliam@gmail.com)");
            SearchPanel.Visibility = System.Windows.Visibility.Visible;
            onlineSearchAvailable = true;
        }
        private async Task<MPoint?> SearchOnlineAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return null;
            if (!onlineSearchAvailable)
                return null;
            try
            {
                string url = $"https://nominatim.openstreetmap.org/search?q={System.Uri.EscapeDataString(query)}&format=json&limit=1";
                var json = await httpClient.GetStringAsync(url);
                var arr = JArray.Parse(json);
                if (arr.Count == 0)
                    return null;
                var first = arr[0];
                double lat = double.Parse(first["lat"]!.ToString(), CultureInfo.InvariantCulture);
                double lon = double.Parse(first["lon"]!.ToString(), CultureInfo.InvariantCulture);
                var merc = SphericalMercator.FromLonLat(lon, lat);
                return new MPoint(merc.x, merc.y);
            }
            catch
            {
                // Om något går fel (offline, timeout, etc) → stäng av sök
                onlineSearchAvailable = false;
                SearchPanel.Visibility = System.Windows.Visibility.Collapsed;
                return null;
            }
        }
        private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            await RunSearchAsync();
        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await RunSearchAsync();
        }
        private async Task RunSearchAsync()
        {
            if (!onlineSearchAvailable)
                return;
            string query = SearchBox.Text;
            if (string.IsNullOrWhiteSpace(query))
                return;
            var world = await SearchOnlineAsync(query);
            if (world == null)
            {
                MessageBox.Show("Ingen träff hittades eller så är du offline.", "Sökning");
                return;
            }
            // Snygg zoom
            mapControl.Map.Navigator.CenterOn(world);
            mapControl.Map.Navigator.ZoomTo(14); // justera efter smak
        }
        public static DateTime GetAverageSunrise(double latitude, int dayOfYear)
        {
            // Om latitud är utanför rimliga gränser
            latitude = Math.Max(-66.0, Math.Min(66.0, latitude));
            // Solens deklination (förenklad modell)
            double decl = 23.45 * Math.Sin((360.0 / 365.0) * (dayOfYear - 81) * Math.PI / 180.0);
            // Omvandla till radianer
            double latRad = latitude * Math.PI / 180.0;
            double decRad = decl * Math.PI / 180.0;
            // Timvinkel vid soluppgång
            double cosH = -Math.Tan(latRad) * Math.Tan(decRad);
            // Polcirkeln: ingen soluppgång vissa dagar
            if (cosH > 1) return DateTime.Today.AddHours(0);   // midnattssol
            if (cosH < -1) return DateTime.Today.AddHours(12); // polarnatt
            double H = Math.Acos(cosH) * 180.0 / Math.PI;
            // Lokal soltid för soluppgång
            double sunriseLocal = 12.0 - (H / 15.0);
            // Avrunda till närmaste 5 minuter
            int minutes = (int)Math.Round(sunriseLocal * 60 / 5.0) * 5;
            return DateTime.Today.Date.AddMinutes(minutes);
        }
        private void MarineButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.marinetraffic.com/en/ais/home/centerx:27.2/centery:60.8/zoom:5";
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                Log.Error("HTTP", "Kunde inte öppna MarineTraffic: " + ex.Message);
                MessageBox.Show("Kunde inte öppna MarineTraffic.\n\n" + ex.Message);
            }
        }
        private void CpuTimer_Tick(object sender, EventArgs e)
        {
            float cpu = cpuCounter.NextValue();
            float gpu = GetGpuUsage();

            if (gpu >= 0)
                GpuText.Text = $"GPU: {gpu:0}%";
            else
                GpuText.Text = "GPU: N/A";

            CpuText.Text = $"CPU: {cpu:0}%";
            GpuText.Text = $"GPU: {gpu:2}%";
        }
        private string GetRamUsage()
        {
            var proc = Process.GetCurrentProcess();
            double mb = proc.WorkingSet64 / (1024.0 * 1024.0);
            return $"{mb:0} MB";
        }
        private double GetFps()
        {
            double seconds = fpsWatch.Elapsed.TotalSeconds;
            double fps = frameCount / seconds;
            // reset
            fpsWatch.Restart();
            frameCount = 0;
            return fps;
        }
        private void PerfTimer_Tick(object sender, EventArgs e)
        {
            float cpu = cpuCounter.NextValue();
            double fps = GetFps();
            PerfRam.Text = $"RAM: {GetRamUsage()}";
            PerfFps.Text = $"FPS: {fps:0}";
        }
        public static double GetShadowFactor(double latitude, int dayOfYear)
        {
            // Solens deklination
            double decl = 23.45 * Math.Sin((360.0 / 365.0) * (dayOfYear - 81) * Math.PI / 180.0);
            double latRad = latitude * Math.PI / 180.0;
            double decRad = decl * Math.PI / 180.0;
            // Solhöjd vid lokal middag
            double sinH = Math.Sin(latRad) * Math.Sin(decRad) +
                          Math.Cos(latRad) * Math.Cos(decRad);
            double h = Math.Asin(sinH); // solhöjd i radianer
            // k = tan(solhöjd)
            double k = Math.Tan(h);
            // avrunda till två decimaler
            return Math.Round(k, 2);
        }
        public static DateTime GetAverageSunset(double latitude, int dayOfYear)
        {
            latitude = Math.Max(-66.0, Math.Min(66.0, latitude));
            double decl = 23.45 * Math.Sin((360.0 / 365.0) * (dayOfYear - 81) * Math.PI / 180.0);
            double latRad = latitude * Math.PI / 180.0;
            double decRad = decl * Math.PI / 180.0;
            double cosH = -Math.Tan(latRad) * Math.Tan(decRad);
            if (cosH > 1) return DateTime.Today.AddHours(24); // midnattssol
            if (cosH < -1) return DateTime.Today.AddHours(12); // polarnatt
            double H = Math.Acos(cosH) * 180.0 / Math.PI;
            double sunsetLocal = 12.0 + (H / 15.0);
            int minutes = (int)Math.Round(sunsetLocal * 60 / 5.0) * 5;

            return DateTime.Today.Date.AddMinutes(minutes);
        }
        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;

            this.Topmost = isPinned;

            PinButton.Content = new Image
            {
                Source = new BitmapImage(new System.Uri(
                    isPinned ? "data/ikoner/pinned.png" : "data/ikoner/unpinned.png",
                    UriKind.Relative))
            };
        }
        public static double GetMoonDistanceKm(DateTime utc)
        {
            // 1. Julianskt datum
            double jd = (utc.ToOADate() + 2415018.5);

            double T = (jd - 2451545.0) / 36525.0;

            // 2. Grundparametrar (Meeus)
            double D = 297.8501921 + 445267.1114034 * T;
            double M = 357.5291092 + 35999.0502909 * T;
            double Mprime = 134.9633964 + 477198.8675055 * T;
            double F = 93.2720950 + 483202.0175233 * T;

            // Omvandla till radianer
            double Dr = D * Math.PI / 180.0;
            double Mr = M * Math.PI / 180.0;
            double Mpr = Mprime * Math.PI / 180.0;
            double Fr = F * Math.PI / 180.0;

            // 3. Geocentriskt avstånd (Meeus kap. 47)
            double distanceKm =
                385000.56
                - 20905.0 * Math.Cos(Mpr)
                - 3699.0 * Math.Cos(2 * Dr - Mpr)
                - 2956.0 * Math.Cos(2 * Dr)
                - 570.0 * Math.Cos(2 * Mpr);

            return Math.Round(distanceKm, 0);
        }
        public static (double lat, double lon) GetAntipode(double lat, double lon)
        {
            double antiLat = -lat;
            double antiLon = lon + 180.0;

            if (antiLon > 180.0)
                antiLon -= 360.0;
            if (antiLon < -180.0)
                antiLon += 360.0;

            return (antiLat, antiLon);
        }
        public async Task<SmhiForecast> GetWeather(double lat, double lon)
        {
            // MessageBox.Show("GETWEATHER CALLED" + lat.ToString() + lon.ToString());
            // Max 6 decimaler – räcker mer än väl
            string latStr = lat.ToString("F6", CultureInfo.InvariantCulture);
            string lonStr = lon.ToString("F6", CultureInfo.InvariantCulture);
            // MessageBox.Show("SMHI test 1");
            string url =
                $"https://opendata-download-metanalys.smhi.se/api/category/mesan2g/version/2/geotype/point/lon/{lonStr}/lat/{latStr}/data.json";

            File.AppendAllText("weather_url_log.txt",
                $"{DateTime.Now}: URL = {url}\n");
            //MessageBox.Show("SMHI test 2");

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            //MessageBox.Show("SMHI test 3");

            var json = await response.Content.ReadAsStringAsync();
            if (debugShowRawWeather)
            {
                File.WriteAllText("smhi_raw.json", json);
                MessageBox.Show(json.Substring(0, Math.Min(json.Length, 5000)),
                                "SMHI RAW JSON (truncated)");
            }
            //MessageBox.Show("SMHI test 4");

            return JsonConvert.DeserializeObject<SmhiForecast>(json);
        }
        private void FlightButton_Click(object sender, RoutedEventArgs e)
        {
            var vp = mapControl.Map.Navigator.Viewport;
            var lonLat = SphericalMercator.ToLonLat(vp.CenterX, vp.CenterY);

            double lat = lonLat.lat;
            double lon = lonLat.lon;
            int zoom = 7; // justera om du vill

            string url = $"https://www.flightradar24.com/63.40,10.53/5";

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        private void OpenMapSelector_Click(object sender, RoutedEventArgs e)
        {
            var win = new MapSelectorWindow();
            win.Show();
        }
        // (3) H = 391 000m,  f=8.6m //satelit
        // (8) H = 378 000m,  f=7.8m //satelit
        // (2) H = 359 000m,  f=3.5m //satelit
        // (9) H = 278 000m,  f=1.09m //satelit

        // (4) H = 6 100m,    f=0.579m //flygbildn

        // (5) H = 129.65m,   f=0.265m //punktmoln <value?>
        // (12)H = 106.83m,   f=0.36m  //punktmoln
        // (11)H = 72.59m,    f=0.52m  //punktmoln
        // (10)H = 26.94m,    f=0.352m //punktmoln

        // (6) H = 22.18m,    f=0.275m //punktmoln <value?>
        // (7) H = 4.11m,     f=0.114m //punktmoln


        // (x) H = 147.93m     f=0.172m pmoln stävlö
        // (f) H = 138.60m     f=0.239m pmoln stävlö
        // (!) H = 76.51m      f=0.243m pmoln stävlö
        // (y) H = 50.25m      f=0.253m pmoln stävlö
        // (a) H = 14.77m,     f=0.288m pmoln stämlö
        // (b) H = 5.90m,      f=0.331m pmoln stävlö 
        private void UpdateCameraText()
        {
            //TP 
            var nav = mapControl.Map.Navigator;
            var vp = nav.Viewport;
            int zoom = GetZoomLevel();
            TxtZoomLevel.Text = $"Zoom: {zoom}";
            var extent = vp.ToExtent();
            if (extent == null) return;

            // 1. Hörn i lat/lon
            var swCorner = SphericalMercator.ToLonLat(extent.MinX, extent.MinY); // botten-vänster
            var seCorner = SphericalMercator.ToLonLat(extent.MaxX, extent.MinY); // botten-höger
            var nwCorner = SphericalMercator.ToLonLat(extent.MinX, extent.MaxY); // topp-vänster

            // 2. Verkliga markmått i meter (Haversine)
            double vh = Haversine(swCorner.lat, swCorner.lon, nwCorner.lat, nwCorner.lon); // nord-syd
            double vw = Haversine(swCorner.lat, swCorner.lon, seCorner.lat, seCorner.lon); // öst-väst

            // 3. Skärmmått i meter (hårdkodade – justera efter din skärm)
            const double sh = 0.223;   // screen height ≈ 20 cm
            const double sw = 0.324;   // screen width  ≈ 30 cm


            // 4. Skydd mot division by zero / orimliga värden
            if (vh <= 0 || vw <= 0 || sh <= 0 || sw <= 0)
            {
                TxtCamera.Text = "–";
                return;
            }

            double vwMin = 115.0; //H=12m
            double vwMax = 3950000.0; //H=47km old value 3950000.0 , vw_corr 4185

            double hMin = 0.003855; //H=47km
            double hMax = 0.0316; //H=12m

            double vwMinCorr = 110.0;
            double vwMaxCorr = 3905700.0; // 4001600.0

            double hMaxCorr = 0.035345454;
            double hMinCorr = 0.00343194;

            double HMin = 12.0;
            double HMax = 47000;

            int zoomIntervalLow = 5;
            int zoomIntervalHigh = 19;
            int zoomStepCount = zoomIntervalHigh - zoomIntervalLow;
            int step = 100 / (zoomIntervalHigh - zoomIntervalLow);
            int zoomLvlReverseOrder = GetZoomLevel();
            int zoomLvl = (1 + zoomIntervalHigh - zoomLvlReverseOrder);
            int HStrengthMax = 100;
            double HStrength = zoomLvl * step;
            double HStrengthProcent =
                (zoomIntervalHigh - zoomLvlReverseOrder) /
                (double)(zoomIntervalHigh - zoomIntervalLow);




            double t_old = (vw - vwMin) / (vwMax - vwMin);//hur stor andel av intervallet% vw+ -> h-
            double t = ((vw - vwMin)) / ((vwMax - vwMin));
            //double h_old = hMin + t * (hMax - hMin) recent hMax + t * (hMax - vw);
            double h = hMin + Math.Abs(1 - t) * (hMax - hMin); //motsvarande motsatta andel av det egna intervallet
            //Log.Info("tmp", $"zomin: {zoomLvlReverseOrder}, zoom trasf{zoomLvl}, HStrength{HStrength},%{HStrengthProcent}, t{t:F2}, %h{h/hMax}");
            double? H = 0;
            if (zoomLvlReverseOrder == 19)
            {
                H = 12 + h * 1.4 * vw / sw;
            }
            else if (zoomLvlReverseOrder == 18)
            {
                H = 8 + h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 17 || zoomLvlReverseOrder == 16)
            {
                H = 5 + h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 13)
            {
                H = 10 + 0.95 * h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 12)
            {
                H = 12 + 0.91 * h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 11)
            {
                H = 12 + 0.8 * h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 10)
            {
                H = 12 + 0.7 * h * vw / sw;
            }
            else if (zoomLvlReverseOrder == 6 || zoomLvlReverseOrder == 7 || zoomLvlReverseOrder == 8 || zoomLvlReverseOrder == 9 || zoomLvlReverseOrder == 10)
            {
                H = 12 + 0.4 * h * vw / sw;
            }
            else
            {
                H = (h * vw) / sw;
            }

            //distortion correctiom for epsg:3857 
            double latSwCenter = 61.398611;
            double latVpCenter = SphericalMercator.ToLonLat(vp.CenterX, vp.CenterY).lat;
            //bottom + topp / 2 = mean distortion
            double S = 1.0 / Math.Cos(latVpCenter * Math.PI / 180.0);   // aktuell lat
            double SC = 1.0 / Math.Cos(latSwCenter * Math.PI / 180.0);   // referenslat
            double Srel = S / SC;
            const double k = 1.0;
            double vwCorr = k * vw * Srel; //finn en konstant k så att variationen i vwCorr är så liten som möjligt, när den är så liten du lyckas skicka med boolean i then metod was zoom yes/no if true then let if statement around height protect from update but run other code, 1.0 ->407-396 North->South 1.02->415-404, 1.04=>423-412
            double t_corr = (vwCorr - vwMin) / (vwMax - vwMin);
            //double h_old = hMin + t * (hMax - hMin);
            double h_corr = hMax + t_corr * (hMin - hMax);
            double H_corrected = h_corr * vw / sw; //vwCorr


            //correction atempt 2 
            double latSwCenterRad = 61.618611 * Math.PI / 180.0; //61.398611
            double latSouthRad = swCorner.lat * Math.PI / 180.0;
            double latNorthRad = nwCorner.lat * Math.PI / 180.0;
            double SC_2 = 1.0 / Math.Cos(latSwCenterRad);
            double S_mean;
            double dLat = latNorthRad - latSouthRad;
            if (Math.Abs(dLat) < 1e-7) // Skydd vid hög zoom / extremt litet spann
            {
                double latVpCenterRad = SphericalMercator.ToLonLat(vp.CenterX, vp.CenterY).lat * Math.PI / 180.0;
                S_mean = 1.0 / Math.Cos(latVpCenterRad);
            }
            else
            {
                // Exakt integral av 1/cos(phi) över [latSouth, latNorth]
                double intNorth = Math.Log(Math.Tan(latNorthRad / 2.0 + Math.PI / 4.0));
                double intSouth = Math.Log(Math.Tan(latSouthRad / 2.0 + Math.PI / 4.0));
                S_mean = (intNorth - intSouth) / dLat;
            }

            // 4. Relativ distorsion jämfört med referenscentrum (Yt-skala)
            double Srel_2 = S_mean / SC_2;// *

            // 5. Beräkna vwCorr med din ursprungliga multiplikation (och k = 1.0) 394-403=9
            double vwCorr_2 = vw * Srel_2; //*
            double t_corr_2 = (vwCorr_2 - vwMinCorr) / (vwMaxCorr - vwMinCorr);//stor andel
            //double h_old = hMin + t * (hMax - hMin);
            double h_corr_2 = hMinCorr + Math.Abs(1 - t_corr_2) * (hMaxCorr - hMinCorr);//litet h
                                                                                        //vwCorr_2 52->44
            double? H_corrected_2 = null;
            if (zoomLvlReverseOrder == 19)
            {
                H_corrected_2 = 12 + h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 18)
            {
                H_corrected_2 = h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 17 || zoomLvlReverseOrder == 16 || zoomLvlReverseOrder == 15)
            {
                H_corrected_2 = h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 13)
            {
                H_corrected_2 = 0.95 * h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 12)
            {
                H_corrected_2 = 0.91 * h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 11)
            {
                H_corrected_2 = 12 + 0.8 * h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 10)
            {
                H_corrected_2 = 12 + 0.62 * h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 9)
            {
                H_corrected_2 = 12 + 0.42 * h_corr_2 * vwCorr_2 / sw;
            }
            else if (zoomLvlReverseOrder == 6 || zoomLvlReverseOrder == 7 || zoomLvlReverseOrder == 8)
            {
                H_corrected_2 = 12 + 0.35 * h_corr_2 * vwCorr_2 / sw;
            }
            else
            {
                H_corrected_2 = 47000.0 + 0.01 * 0.45 * h_corr_2 * vwCorr_2 / sw;
            }

            double area = vw * vh;
            double height2 = 0.4 * (-4.0 + Math.Sqrt(16.0 * area - 112.0)) / 8.0; //Best

            string HeightText = H >= 1000 ? $"{H / 1000.0:F3} km" : $"{H:F0} m";
            string HeightCorTxt = H_corrected_2 >= 1000 ? $"{H_corrected_2 / 1000.0:F3} km" : $"{H_corrected_2:F0} m";
            if (vp.Resolution != ResolutionLast)
            {
                TxtCamera.Text =
                    $"Height: {HeightCorTxt}";
                ResolutionLast = vp.Resolution;
            }
            TxtArea.Text = $"{FormatDistance(vw)} × {FormatDistance(vh)}";
            UpdateMapScaleTextAndDpi(vw, sw);

        }
        //Kalibrering
        // (1) vw=125, h=0.0316 Bing aerail helikopter comparsion
        // (2) vw=395 0000 h=0.003855 Bågmodell

        private void UpdateMapScaleTextAndDpi(double vw, double sw)
        {
            var (rawX, rawY) = client.Dpi.GetRawDpi(this);
            double ppi = rawX;
            double pixelsPerMeter = ppi * 39.37007874015748;   // 1 tum = 25.4 mm → 1000/25.4
            double resolution = mapControl.Map.Navigator.Viewport.Resolution;
            double scaleRatio = vw / sw;
            TxtScale.Text = $"1:{scaleRatio:F0} meter";
            PerfDpi.Text = $"DPI: {ppi:F0}";

        }
        private void UpdateScaleBar()
        {
            var vp = mapControl.Map.Navigator.Viewport;
            if (double.IsNaN(vp.Width) || vp.Width <= 0) return;

            ScaleCanvas.Children.Clear();

            double pixelWidth = 120;

            // världsenheter per pixel
            double metersPerPixel = vp.Resolution;

            double meters = pixelWidth * metersPerPixel;

            // snygg avrundning (1,2,5,10...)
            double niceMeters = NiceScale(meters);

            double nicePixelWidth = niceMeters / metersPerPixel;

            // linje
            var line = new System.Windows.Shapes.Line
            {
                X1 = 0,
                Y1 = 10,
                X2 = nicePixelWidth,
                Y2 = 10,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            // text
            var text = new TextBlock
            {
                Text = niceMeters >= 1000
                    ? $"{niceMeters / 1000:0.#} km"
                    : $"{niceMeters:0} m",
                Margin = new System.Windows.Thickness(0, 14, 0, 0),
                FontSize = 12,
                Foreground = Brushes.Black
            };

            ScaleCanvas.Children.Add(line);
            ScaleCanvas.Children.Add(text);
        }
        private double NiceScale(double meters)
        {
            double[] steps = { 1, 2, 5 };
            double magnitude = Math.Pow(10, Math.Floor(Math.Log10(meters)));

            double best = double.MaxValue;
            double bestDiff = double.MaxValue;

            foreach (var step in steps)
            {
                foreach (var mul in new[] { 0.1, 1, 10 })
                {
                    double candidate = step * magnitude * mul;
                    double diff = Math.Abs(candidate - meters);

                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        best = candidate;
                    }
                }
            }

            return best;
        }
        private double DistanceToCoast(double lon, double lat)
        {
            // Punkt i Mercator
            var point = SphericalMercator.FromLonLat(lon, lat);
            var ntsPoint = new NetTopologySuite.Geometries.Point(point.x, point.y);

            double minDist = double.MaxValue;

            foreach (var f in coastlineFeatures)
            {
                var geom = f.Geometry;
                double d = ntsPoint.Distance(geom);
                if (d < minDist)
                    minDist = d;
            }

            return minDist; // meter
        }
        private double Bearing(MPoint from, MPoint to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;

            double angle = Math.Atan2(dx, dy); // OBS: dx, dy omvänt i Mercator
            double bearing = (angle * 180.0 / Math.PI);

            if (bearing < 0) bearing += 360;

            return bearing;
        }
        private double DirectionToCoast(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var p = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            double minDist = double.MaxValue;
            Coordinate nearest = null;

            foreach (var f in coastlineFeatures)
            {
                var geom = f.Geometry;

                if (geom is LineString ls)
                {
                    for (int i = 0; i < ls.NumPoints - 1; i++)
                    {
                        var a = ls.GetCoordinateN(i);
                        var b = ls.GetCoordinateN(i + 1);

                        var np = NearestPointOnSegment(p, a, b);
                        double d = p.Distance(np);

                        if (d < minDist)
                        {
                            minDist = d;
                            nearest = np.Coordinate;
                        }
                    }
                }
                else if (geom is MultiLineString mls)
                {
                    foreach (LineString ls2 in mls.Geometries)
                    {
                        for (int i = 0; i < ls2.NumPoints - 1; i++)
                        {
                            var a = ls2.GetCoordinateN(i);
                            var b = ls2.GetCoordinateN(i + 1);

                            var np = NearestPointOnSegment(p, a, b);
                            double d = p.Distance(np);

                            if (d < minDist)
                            {
                                minDist = d;
                                nearest = np.Coordinate;
                            }
                        }
                    }
                }
            }

            if (nearest == null) return 0;

            return Bearing(
                new MPoint(merc.x, merc.y),
                new MPoint(nearest.X, nearest.Y)
            );
        }
        private Point NearestPointOnSegment(Point p, Coordinate a, Coordinate b)
        {
            double ax = a.X, ay = a.Y;
            double bx = b.X, by = b.Y;
            double px = p.X, py = p.Y;

            double dx = bx - ax;
            double dy = by - ay;

            if (dx == 0 && dy == 0)
                return new Point(ax, ay);

            double t = ((px - ax) * dx + (py - ay) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            return new Point(ax + t * dx, ay + t * dy);
        }
        private void StartMeasureArea_Click(object sender, RoutedEventArgs e)
        {
            measuringArea = true;
            areaPoints.Clear();
        }
        private void DrawAreaPreview()
        {
            overlayCanvas.Children.Clear();
            if (areaPoints.Count < 2) return;
            var vp = mapControl.Map.Navigator.Viewport;
            for (int i = 0; i < areaPoints.Count - 1; i++)
            {
                var a = vp.WorldToScreen(areaPoints[i].X, areaPoints[i].Y);
                var b = vp.WorldToScreen(areaPoints[i + 1].X, areaPoints[i + 1].Y);
                var line = new System.Windows.Shapes.Line
                {
                    X1 = a.X,
                    Y1 = a.Y,
                    X2 = b.X,
                    Y2 = b.Y,
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 2
                };
                overlayCanvas.Children.Add(line);
            }
        }
        private void FinishAreaMeasurement()
        {
            measuringArea = false;
            var coords = areaPoints
                .Select(p => new Coordinate(p.X, p.Y))
                .ToList();
            coords.Add(coords[0]); // stäng polygonen
            var poly = new NetTopologySuite.Geometries.Polygon(new LinearRing(coords.ToArray()));
            double area = poly.Area; // m² i Mercator
            string areaText = area < 1_000_000
                ? $"{area:F0} m²"
                : $"{area / 1_000_000:F2} km²";


            MessageBox.Show($"Area: {areaText}", "Ytmätning");
            areaPoints.Clear();
            overlayCanvas.Children.Clear();
        }
        private void RedrawBeachIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;
            if (beachImageControls.Count == 0)
            {
                foreach (var (world, iconPath) in beachIcons)
                {
                    var screen = vp.WorldToScreen(world.X, world.Y);
                    var img = new Image
                    {
                        Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                        Width = 24,
                        Height = 24,
                        IsHitTestVisible = false
                    };
                    Canvas.SetLeft(img, screen.X - img.Width / 2);
                    Canvas.SetTop(img, screen.Y - img.Height / 2);
                    beachImageControls.Add(img);
                    iconCanvas.Children.Add(img);
                }
                return;
            }
            // Flytta befintliga ikoner
            for (int i = 0; i < beachIcons.Count && i < beachImageControls.Count; i++)
            {
                var (world, _) = beachIcons[i];
                var img = beachImageControls[i];
                var screen = vp.WorldToScreen(world.X, world.Y);
                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
        }
        private void ToggleBeachesLayer(object sender, RoutedEventArgs e)
        {
            if (beachesLayer == null) return;
            beachesLayer.Enabled = !beachesLayer.Enabled;
            mapControl.Refresh();
        }
        private bool IsInsideNatureReserve(double worldX, double worldY)
        {
            var point = new NetTopologySuite.Geometries.Point(worldX, worldY);

            foreach (var f in reserveFeatures)
            {
                if (f.Geometry is NetTopologySuite.Geometries.Polygon poly)
                {
                    if (poly.Contains(point))
                        return true;
                }
                else if (f.Geometry is MultiPolygon mp)
                {
                    if (mp.Contains(point))
                        return true;
                }
            }
            return false;
        }
        private IFeature? GetTatort(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);

            // Hämta bara kandidater från indexet
            var candidates = tatortsIndex.Query(point.EnvelopeInternal);
            if (candidates.Count != 0)
            {
                foreach (var f in candidates)
                {
                    try
                    {
                        if (f.Geometry.Contains(point))
                            return f;
                    }
                    catch
                    {
                        //
                    }
                }
            }
            else
            {
                Log.Error("Inga tätortsobjekt", ".");
            }
            return null;
        }
        private IFeature? GetSmaort(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);

            // Hämta bara kandidater från indexet
            var candidates = smaortIndex.Query(point.EnvelopeInternal);
            if (candidates.Count != 0)
            {
                foreach (var f in candidates)
                {
                    try
                    {
                        if (f.Geometry.Contains(point))
                            return f;
                    }
                    catch
                    {
                        //
                    }
                }
            }
            else
            {
                Log.Error("Inga småortsobjekt", ".");
            }
            return null;
        }
        private void TogglePassagesLayer(object sender, RoutedEventArgs e)
        {
            if (passagesLayer == null) return;
            passagesLayer.Enabled = !passagesLayer.Enabled;
            mapControl.Refresh();
        }
        private void ToggleBerggrundLayer(object sender, RoutedEventArgs e)
        {
            berggrundLayer.Enabled = !berggrundLayer.Enabled;
            mapControl.Refresh();
        }
        private IFeature? GetBerggrundFeature(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);

            foreach (var f in berggrundFeatures)
            {
                if (f.Geometry.Contains(point))
                    return f;
            }

            return null;
        }
        private IFeature? GetBioRegFeatures(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);

            foreach (var f in bioRegFeatures)
            {
                if (f.Geometry.Contains(point))
                    return f;
            }

            return null;
        }
        private IFeature? GetJordartFeature(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);

            // Hämta bara kandidater från indexet
            var candidates = jordIndex.Query(point.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry.Contains(point))
                        return f;
                }
                catch
                {
                    // Ignorera ogiltiga polygoner
                }
            }

            return null;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Castle layer – Ctrl + 1
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D1)
            {
                RemoveLayer(castleLayer, "Slott-lagret");
            }

            // Botanical gardens – Ctrl + 2
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D2)
            {
                RemoveLayer(botanicalGardensLayer, "Botaniska trädgårdar-lagret");
            }

            // Churches – Ctrl + 3
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D3)
            {
                RemoveLayer(churchesLayer, "Kyrkor-lagret");
            }

            // Rune stones – Ctrl + 4
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D4)
            {
                RemoveLayer(runeLayer, "Runstenar-lagret");
            }

            // Walking routes – Ctrl + 5
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D5)
            {
                RemoveLayer(walkingRoutesLayer, "Vandringsleder-lagret");
            }

            // Beaches – Ctrl + 6
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D6)
            {
                RemoveLayer(beachesLayer, "Badplatser-lagret");
            }

            // Berggrund – Ctrl + 7
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D7)
            {
                RemoveLayer(berggrundLayer, "Berggrund-lagret");
            }
            // Overlay canvas - ctrl + 8
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D8)
            {
                overlayCanvas.Children.Clear();
            }
        }
        private void RemoveLayer(Mapsui.Layers.Layer layer, string name)
        {
            if (layer != null && mapControl.Map.Layers.Contains(layer))
            {
                mapControl.Map.Layers.Remove(layer);
                MessageBox.Show($"{name} har tagits bort från sessionen.");
            }
        }

        private void SetProgress(double value)
        {
            InfoProgress.Value = value;
        }
        private void ShowProgress()
        {
            InfoProgress.Visibility = System.Windows.Visibility.Visible;
            InfoProgress.Value = 0;
        }

        private void HideProgress()
        {
            InfoProgress.Visibility = System.Windows.Visibility.Collapsed;
        }
        private IFeature? GetGrundvattenFeature(double x, double y)
        {
            var point = new Point(x, y);
            var candidates = gvIndex.Query(point.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry.Contains(point))
                        //  MessageBox.Show("GV HIT!");
                        return f;
                }
                catch (System.Exception ex) { MessageBox.Show("GV error: " + ex.Message); }
            }
            return null;
        }
        public static async Task<(string roadNumber, string speedLimit, int lan)> GetRoadInfo(double lat, double lon)
        {
            try
            {
                string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";

                // NVDB kräver lon lat i WGS84-3D-format
                string wgs84 = $"{lon.ToString(CultureInfo.InvariantCulture)} {lat.ToString(CultureInfo.InvariantCulture)}";

                string xmlRequest =
        $@"<REQUEST>
  <LOGIN authenticationkey=""{TrafikverketApiKey}"" />
  <QUERY objecttype=""Vägnummer"" namespace=""Vägdata.NVDB_DK_O"" schemaversion=""1.2"">
    <FILTER>
      <NEAR name=""Geometry.WKT-WGS84-3D"" value=""{wgs84}"" mindistance=""0"" maxdistance=""100"" />
    </FILTER>
  </QUERY>
  <QUERY objecttype=""Hastighetsgräns"" namespace=""Vägdata.NVDB_DK_O"" schemaversion=""1.2"">
    <FILTER>
      <NEAR name=""Geometry.WKT-WGS84-3D"" value=""{wgs84}"" mindistance=""0"" maxdistance=""100"" />
    </FILTER>
  </QUERY>
</REQUEST>";

                using var client = new System.Net.Http.HttpClient();
                var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
                var response = await client.PostAsync(url, content);
                string json = await response.Content.ReadAsStringAsync();

                string road = "Okänt";
                string speed = "Okänd";
                int lan = -1;

                try
                {
                    var root = JsonDocument.Parse(json).RootElement;
                    var results = root.GetProperty("RESPONSE").GetProperty("RESULT");

                    foreach (var result in results.EnumerateArray())
                    {
                        // --- VÄGNUMMER ---
                        if (result.TryGetProperty("Vägnummer", out var roadArray))
                        {
                            foreach (var item in roadArray.EnumerateArray())
                            {
                                int huvud = item.GetProperty("Huvudnummer").GetInt32();
                                int under = item.GetProperty("Undernummer").GetInt32();
                                lan = item.GetProperty("Länstillhörighet").GetInt32();

                                road = under == 0 ? huvud.ToString() : $"{huvud}.{under}";
                            }
                        }

                        // --- HASTIGHETSGRÄNS ---
                        if (result.TryGetProperty("Hastighetsgräns", out var speedArray))
                        {
                            foreach (var item in speedArray.EnumerateArray())
                            {
                                if (item.TryGetProperty("Högsta_tillåtna_hastighet", out var speedValue))
                                {
                                    speed = speedValue.GetString();
                                    break; // ta första träffen
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    return ($"Fel vid JSON-parsning: {ex.Message}", "Fel", -1);
                }

                return (road, speed, lan);
            }
            catch (System.Exception ex)
            {
                return ($"Fel vid API-anrop: {ex.Message}", "Fel", -1);
            }
        }
        private void CreateFossilIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            if (double.IsNaN(vp.Width) || vp.Width <= 0) return;

            fossilImageControls.Clear();

            foreach (var (world, iconPath) in fossilIcons)
            {
                var screen = vp.WorldToScreen(world.X, world.Y);

                var img = new Image
                {
                    Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                    Width = 24,
                    Height = 24,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);

                fossilImageControls.Add(img);
                iconCanvas.Children.Add(img);
            }
        }
        private void RedrawFossilIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            if (fossilImageControls.Count == 0)
            {
                CreateFossilIcons();
                return;
            }

            for (int i = 0; i < fossilIcons.Count && i < fossilImageControls.Count; i++)
            {
                var (world, _) = fossilIcons[i];
                var img = fossilImageControls[i];

                var screen = vp.WorldToScreen(world.X, world.Y);

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
        }
        private void CreateStatueIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;

            if (double.IsNaN(vp.Width) || vp.Width <= 0) return;

            statueImageControls.Clear();

            foreach (var (world, iconPath) in statueIcons)
            {
                var screen = vp.WorldToScreen(world.X, world.Y);

                var img = new Image
                {
                    Source = new BitmapImage(new System.Uri(iconPath, UriKind.RelativeOrAbsolute)),
                    Width = 24,
                    Height = 24,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);

                statueImageControls.Add(img);
                iconCanvas.Children.Add(img);
            }
        }
        private void RedrawStatueIcons()
        {
            var vp = mapControl.Map.Navigator.Viewport;
            if (statueImageControls.Count == 0)
            {
                CreateStatueIcons();
                return;
            }
            for (int i = 0; i < statueIcons.Count && i < statueImageControls.Count; i++)
            {
                var (world, _) = statueIcons[i];
                var img = statueImageControls[i];

                var screen = vp.WorldToScreen(world.X, world.Y);

                Canvas.SetLeft(img, screen.X - img.Width / 2);
                Canvas.SetTop(img, screen.Y - img.Height / 2);
            }
        }
        private void OpenCopilotWithContext(string infoText)
        {
            lastInfoWindowText = infoText;

            CopilotPanel.Visibility = System.Windows.Visibility.Visible;

            GenerateGreeting();
        }

        private void CloseCopilot_Click(object sender, RoutedEventArgs e)
        {
            CopilotPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
        private async void SendToCopilot_Click(object sender, RoutedEventArgs e)
        {
            string userQuestion = CopilotInput.Text;
            if (string.IsNullOrWhiteSpace(userQuestion))
                return;

            CopilotOutput.Text += "\n\n[Du]: " + userQuestion;
            CopilotInput.Clear();

            var directAnswer = CheckDefinitions(userQuestion);

            CopilotOutput.Text += "\n\n[Hjälpredan]: ";

            if (directAnswer != null)
            {
                CopilotOutput.Text += directAnswer;
                return;
            }

            string prompt =
                "Förklara pedagogiskt:\n" +
                userQuestion;

            CopilotOutput.Text += "\nJag har ingen inbyggd information om detta.";
        }



        private string? CheckDefinitions(string question)
        {
            foreach (var pair in _definitions)
            {
                question = question.ToLowerInvariant();
                if (question.Contains(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Value;
                }
            }

            return null;
        }
        private static readonly string[] Greetings =
        {
    "Hej!",
    "Hur kan jag hjälpa dig?",
    "Vad undrar du?",
    "Välkommen!",
    "Ställ gärna en fråga.",
    "Hej, vad vill du veta?"
};

        private void GenerateGreeting()
        {
            Random rnd = new();

            CopilotOutput.Text =
                "[Hjälpredan]: " +
                Greetings[rnd.Next(Greetings.Length)];
        }
        private string GetMoonPhase(DateTime dateUtc)
        {
            // 1) Känd astronomisk nymåne i UTC
            DateTime refNewMoon = new DateTime(2026, 1, 18, 20, 53, 17, DateTimeKind.Utc);

            // 2) Synodisk måncykel i dagar
            const double synodicMonth = 29.5305882;

            // 3) Beräkna antal dagar (inklusive tid) sedan referens
            double daysSince = (dateUtc - refNewMoon).TotalDays;

            // 4) Modulo synodisk period
            double phase = (daysSince % synodicMonth);
            if (phase < 0) phase += synodicMonth;

            // 5) Normalisera till andel och bestäm fas
            double frac = phase / synodicMonth;
            int index = (int)(frac * 8 + 0.5) % 8;

            return index switch
            {
                0 => "Nymåne",
                1 => "Tilltagande skära",
                2 => "Första kvarter",
                3 => "Tilltagande måne",
                4 => "Fullmåne",
                5 => "Avtagande måne",
                6 => "Sista kvarter",
                7 => "Avtagande skära",
                _ => "Okänd"
            };
        }
        private class SoundPoint
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
            public string Path { get; set; }
            public DateTime Time { get; set; }
        }
        private SoundPoint FindNearestSound(double lon, double lat)
        {
            SoundPoint? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var s in soundPoints)
            {
                double d = Haversine(lat, lon, s.Lat, s.Lon);
                if (d < 1000 && d < bestDist)
                {
                    bestDist = d;
                    nearest = s;
                }
            }

            return nearest;
        }
        private void OnPlaySoundRequested()
        {
            Console.WriteLine(Path.GetFullPath(lastSoundPath));
            if (lastSoundPath == null)
            {
                MessageBox.Show("Ingen ljudinspelning vald.");
                return;
            }

            try
            {
                var full = System.IO.Path.GetFullPath(lastSoundPath);
                MessageBox.Show(full);

                mediaPlayer.MediaFailed += (s, e) =>
                {
                    MessageBox.Show("Media failed: " + e.ErrorException?.Message); //cannot find the media file
                };

                mediaPlayer.Open(new System.Uri(lastSoundPath, UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
                isPlayingSound = true;


                ShowSoundOverlay("Spelar upp ljud… Tryck ESC för att stoppa.");
            }
            catch
            {
                MessageBox.Show("Kunde inte spela upp ljudfilen.");
            }
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && isPlayingSound)
            {
                mediaPlayer.Stop();
                isPlayingSound = false;
                HideSoundOverlay();
            }
        }
        private void ShowSoundOverlay(string text)
        {
            SoundOverlay.Text = text;
            SoundOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void HideSoundOverlay()
        {
            SoundOverlay.Visibility = System.Windows.Visibility.Collapsed;
        }
        private IFeature? FindNearestWind(double lon, double lat)
        {
            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var (world, feature) in windPoints)
            {
                var ll = SphericalMercator.ToLonLat(world.X, world.Y);
                double d = Haversine(lat, lon, ll.lat, ll.lon);

                if (d < 2000 && d < bestDist)
                {
                    bestDist = d;
                    nearest = feature;
                }
            }

            return nearest;
        }
        private IFeature? FindNearestHeat(double lon, double lat)
        {
            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var (world, feature) in bedrockPoints)
            {
                var ll = SphericalMercator.ToLonLat(world.X, world.Y);
                double d = Haversine(lat, lon, ll.lat, ll.lon);

                if (d < 2000 && d < bestDist)
                {
                    bestDist = d;
                    nearest = feature;
                }
            }

            return nearest;
        }
        private IFeature? FindNearestWaterPlant(double lon, double lat)
        {
            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var (world, feature) in waterPoints)
            {
                var ll = SphericalMercator.ToLonLat(world.X, world.Y);
                double d = Haversine(lat, lon, ll.lat, ll.lon);

                if (d < 1000 && d < bestDist) // 1 km radie
                {
                    bestDist = d;
                    nearest = feature;
                }
            }

            return nearest;
        }
        private (IFeature? feature, double dist) FindNearestNuclear(double lon, double lat)
        {
            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var (world, feature) in nuclearPoints)
            {
                var ll = SphericalMercator.ToLonLat(world.X, world.Y);
                double d = Haversine(lat, lon, ll.lat, ll.lon);

                if (d < 50000 && d < bestDist) // 50 km radie
                {
                    bestDist = d;
                    nearest = feature;
                }
            }

            return (nearest, bestDist / 1000);
        }
        private (IFeature? feature, double dist) FindNearestAirport(double lon, double lat)
        {
            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var (world, feature) in airportPoints)
            {
                var ll = SphericalMercator.ToLonLat(world.X, world.Y);
                double d = Haversine(lat, lon, ll.lat, ll.lon);

                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = feature;
                }
            }

            return (nearest, bestDist);
        }


        private void ToggleWindLayer_Click(object sender, RoutedEventArgs e)
        {
            if (windLayer != null)
                windLayer.Enabled = !windLayer.Enabled;
        }
        private void ToggleWaterLayer_Click(object sender, RoutedEventArgs e)
        {
            if (waterLayer != null)
                waterLayer.Enabled = !waterLayer.Enabled;
        }
        private IFeature? FindCountyPolygon(double lon, double lat)
        {
            // Konvertera klick till Mercator
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            foreach (var f in lanFeatures)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindRegionPolygon(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new Point(merc.x, merc.y);

            foreach (var f in regionFeatures)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindKommunPolygon(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = kommunIndex.Query(pt.EnvelopeInternal);

            foreach (var f in candidates)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindCivoPolygon(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = civoIndex.Query(pt.EnvelopeInternal);

            foreach (var f in candidates)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindDeSoPolygon(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = desoIndex.Query(pt.EnvelopeInternal);

            foreach (var f in candidates)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindLandcoverPolygon(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            foreach (var f in lcFeatures)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindNearestGeokemi(double x, double y)
        {
            var pt = new Point(x, y);

            // Hämta kandidater via spatialt index
            double searchRadius = 10000; // 10 km
            var env = pt.Buffer(searchRadius).EnvelopeInternal;
            var candidates = geokemiIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            // Max 10 km
            if (bestDist > 10000)
                return null;

            return nearest;
        }
        private void OpenArtfakta_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://fynddata.artdatabanken.se/";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        private void FornsokButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://app.raa.se/open/fornsok/",
                UseShellExecute = true
            });
        }

        public static class SolarCalculator
        {
            public static double GetSolarElevation(double lat, double lon, DateTime time)
            {
                // Konvertera till radianer
                double rad = Math.PI / 180.0;

                // Dag på året
                int dayOfYear = time.DayOfYear;

                // Solens deklination (i radianer)
                double decl = 23.45 * rad * Math.Sin(rad * (360.0 / 365.0 * (dayOfYear - 81)));

                // Tidsvinkel (hour angle)
                double solarTime = time.TimeOfDay.TotalHours + (lon / 15.0);
                double hourAngle = rad * 15.0 * (solarTime - 12.0);

                // Latitud i radianer
                double latRad = lat * rad;

                // Solhöjd
                double sinElevation =
                    Math.Sin(latRad) * Math.Sin(decl) +
                    Math.Cos(latRad) * Math.Cos(decl) * Math.Cos(hourAngle);

                return Math.Asin(sinElevation) / rad; // tillbaka till grader
            }
        }
        public async Task<JArray> GetMsbNewsAsync()
        {
            string url = "https://api.krisinformation.se/v3/news?numberOfNewsArticles=5&includeTest=true&language=sv";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JArray.Parse(json);
        }
        private async Task UpdateMsbOverlayAsync()
        {
            var news = await GetMsbNewsAsync();

            if (news.Count == 0)
            {
                MsbOverlay.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("MSB / Krisinformation:");

            int count = 0;
            foreach (var item in news)
            {
                if (count >= 3) break; // visa bara 3 i overlay
                sb.AppendLine($"• {item["Headline"]}");
                count++;
            }

            MsbText.Text = sb.ToString();
            MsbOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        private async Task TestMsbApi()
        {
            try
            {
                string url = "https://api.krisinformation.se/v3/news?numberOfNewsArticles=5&includeTest=true&language=sv"; var response = await httpClient.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                MessageBox.Show("MSB API svar:\n" + json.Substring(0, Math.Min(800, json.Length)));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Fel vid MSB API:\n" + ex.Message);
            }
        }
        private void CloseMsbOverlay_Click(object sender, RoutedEventArgs e)
        {
            MsbOverlay.Visibility = System.Windows.Visibility.Collapsed;
        }
        private (double landPercent, double waterPercent) CalculateLandWaterIndex(double lon, double lat, double radiusMeters)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new Point(merc.x, merc.y);
            var buffer = pt.Buffer(radiusMeters);

            double waterArea = 0;

            foreach (var f in lcFeatures)
            {
                var geom = f.Geometry;

                if (!geom.EnvelopeInternal.Intersects(buffer.EnvelopeInternal))
                    continue;

                var intersection = geom.Intersection(buffer);
                if (intersection.IsEmpty)
                    continue;

                string cls = f.Attributes["class_name"]?.ToString() ?? "";

                if (cls.Equals("Water Bodies", StringComparison.OrdinalIgnoreCase))
                {
                    waterArea += intersection.Area;
                }
            }

            // TOTAL AREA = buffertens area (GIS-standard)
            double totalArea = buffer.Area;

            double waterPercent = (waterArea / totalArea) * 100.0;
            double landPercent = 100.0 - waterPercent;

            return (landPercent, waterPercent);
        }
        private void RefreshMsbOverlay_Click(object sender, RoutedEventArgs e)
        {
            _ = UpdateMsbOverlayAsync();
        }
        private int CalculateLandscapeDiversity(double lon, double lat, double radiusMeters = 1000)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new Point(merc.x, merc.y);
            var buffer = pt.Buffer(radiusMeters);

            HashSet<string> classes = new HashSet<string>();

            foreach (var f in lcFeatures)
            {
                if (!f.Geometry.EnvelopeInternal.Intersects(buffer.EnvelopeInternal))
                    continue;

                var inter = f.Geometry.Intersection(buffer);
                if (inter.IsEmpty)
                    continue;

                string cls = f.Attributes["class_name"]?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(cls))
                    classes.Add(cls);
            }

            return classes.Count;
        }
        public async Task<string> GetSatelliteInfoAsync(
    int satelliteId,
    double observerLat,
    double observerLon,
    string apiKey)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();

                string url =
                    $"https://api.n2yo.com/rest/v1/satellite/positions/" +
                    $"{satelliteId}/{observerLat}/{observerLon}/0/1&apiKey={apiKey}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return $"Satellit {satelliteId}: API-fel ({response.StatusCode})";

                var json = await response.Content.ReadAsStringAsync();

                var doc = JsonDocument.Parse(json);

                var pos = doc.RootElement
                    .GetProperty("positions")[0];

                double lat = pos.GetProperty("satlatitude").GetDouble();
                double lon = pos.GetProperty("satlongitude").GetDouble();
                double alt = pos.GetProperty("sataltitude").GetDouble();
                double az = pos.GetProperty("azimuth").GetDouble();
                double el = pos.GetProperty("elevation").GetDouble();

                return
                    $"Satellit {satelliteId}:" +
                    $"\nLat: {lat:F4}" +
                    $"\nLon: {lon:F4}" +
                    $"\nHöjd: {alt:F1} km" +
                    $"\nAzimut: {az:F1}°" +
                    $"\nElevation: {el:F1}°";
            }
            catch (System.Exception ex)
            {
                return $"Satellit {satelliteId}: fel vid hämtning ({ex.Message})";
            }
        }
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.ZoomIn(1);
        }
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.ZoomOut(1);
        }
        private void LoadPluginLayers()
        {
            var loader = new PluginLoader();
            var plugins = loader.LoadPlugins();

            foreach (var (plugin, pluginFolder) in plugins)
            {
                foreach (var desc in plugin.GetLayers())
                {
                    TryAddPluginLayer(desc, pluginFolder);
                }
            }
        }
        private void TryAddPluginLayer(PluginLayerDescriptor desc, string pluginFolder)
        {
            try
            {
                var geoJsonPath = Path.Combine(pluginFolder, desc.GeoJsonRelativePath);
                if (!File.Exists(geoJsonPath)) return;

                var json = File.ReadAllText(geoJsonPath, Encoding.UTF8);
                var provider = new GeoJsonProvider(json);

                if (desc.Id == "vattendistrikt")
                {
                    vattendistrikpluginbool = true;
                    var reader = new GeoJsonReader();
                    vattendistriktFeatures = reader.Read<FeatureCollection>(json);
                }

                IStyle style = desc.GeometryType.ToLower() switch
                {
                    "polygon" => new VectorStyle
                    {
                        Fill = new Mapsui.Styles.Brush(Color.FromArgb(80, 80, 80, 255)),
                        Line = new Mapsui.Styles.Pen(Color.FromArgb(200, 0, 0, 120), 1)
                    },
                    "line" => new VectorStyle
                    {
                        Line = new Mapsui.Styles.Pen(Color.Blue, 1.5f)
                    },
                    "point" => new Mapsui.Styles.SymbolStyle
                    {
                        SymbolType = SymbolType.Ellipse,
                        Fill = new Mapsui.Styles.Brush(Color.Red),
                        Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                        SymbolScale = 0.6
                    },
                    _ => new VectorStyle()
                };

                var layer = new Mapsui.Layers.Layer(desc.DisplayName)
                {
                    DataSource = provider,
                    Style = style,
                    Enabled = desc.EnabledByDefault,
                };
                if (desc.Id == "vattendistrikt")
                {
                    layer.Enabled = false;
                }
                mapControl.Map.Layers.Add(layer);
            }
            catch
            {
                // ev. loggning
            }

        }
        public class OcobsRoot
        {
            public List<OcobsStation> station { get; set; }
        }

        public class OcobsStation
        {
            public int id { get; set; }
            public string key { get; set; }
            public string name { get; set; }
            public string owner { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public bool active { get; set; }
        }
        private readonly System.Net.Http.HttpClient httpClient2 = new System.Net.Http.HttpClient();

        public async Task<List<OcobsStation>> GetSeaTempStationsAsync()
        {
            var url = "https://opendata-download-ocobs.smhi.se/api/version/1.0/parameter/5.json";
            var json = await httpClient2.GetStringAsync(url);

            /*MessageBox.Show(json.Substring(0, Math.Min(json.Length, 1500)),
                "OCOBS parameter 5 raw JSON");*/

            var root = JsonConvert.DeserializeObject<OcobsRoot>(json);
            return root.station;
        }
        public OcobsStation FindNearestActiveStation(
    double lat, double lon, List<OcobsStation> stations,
    List<OcobsStation> banlist)
        {
            OcobsStation best = null;
            double bestDist = double.MaxValue;

            foreach (var s in stations)
            {
                /*// logga alla
                MessageBox.Show(
                    $"Station:\n" +
                    $"Id: {s.id}\nKey: {s.key}\nName: {s.name}\n" +
                    $"Lat: {s.latitude}\nLon: {s.longitude}\nActive: {s.active}",
                    "OCOBS Station");*/

                if (!s.active)
                {
                    banlist.Add(s);
                    /*MessageBox.Show(
                        $"Inactive station, add to banlist:\n{s.name} ({s.key})",
                        "OCOBS Banlist");*/
                    continue;
                }

                var d = Math.Sqrt(
                    Math.Pow(lat - s.latitude, 2) +
                    Math.Pow(lon - s.longitude, 2));

                if (d < bestDist)
                {
                    bestDist = d;
                    best = s;
                }
            }

            if (best != null)
            {
                /* MessageBox.Show(
                     $"Nearest ACTIVE station:\n" +
                     $"Id: {best.id}\nKey: {best.key}\nName: {best.name}\n" +
                     $"Lat: {best.latitude}\nLon: {best.longitude}\n" +
                     $"Distance (approx): {bestDist}",
                     "OCOBS Nearest Active");*/
            }
            else
            {
                MessageBox.Show("No active sea temperature station found.", "OCOBS");
            }

            return best;
        }

        public async Task<double?> GetLatestSeaTemperatureAsync(string stationKey)
        {
            var url =
                $"https://opendata-download-ocobs.smhi.se/api/version/1.0/parameter/5/station/{stationKey}/period/latest-day/data.json";

            var json = await httpClient.GetStringAsync(url);

            /*  MessageBox.Show(json.Substring(0, Math.Min(json.Length, 1500)),
                  "OCOBS latest-day raw JSON");*/

            var obj = JObject.Parse(json);

            var values = obj["value"] as JArray;
            if (values == null || values.Count == 0)
            {
                //MessageBox.Show("No values[] in latest-day response.", "OCOBS");
                return null;
            }

            // logga alla värden
            var sb = new StringBuilder();
            foreach (var v in values)
            {
                sb.AppendLine(
                    $"date: {v["date"]}, value: {v["value"]}, depth: {v["depth"]}, quality: {v["quality"]}");
            }
            // MessageBox.Show(sb.ToString(), "OCOBS all temperature values (latest-day)");

            var last = values.Last["value"].Value<double>();
            //MessageBox.Show($"Latest temperature value: {last} °C", "OCOBS");

            return last;
        }

        public async Task<(double? temp, OcobsStation station, double distance, JArray allValues)>
      SmhiSeaTemperatureAsync(double lat, double lon)
        {

            var stations = await GetSeaTempStationsAsync();
            var banlist = new List<OcobsStation>();
            var filtered = FilterStations(stations);

            var nearest = FindNearestActiveStation(lat, lon, filtered, banlist);

            if (nearest == null)
                return (null, null, 0, null);

            var dist = Math.Sqrt(
                Math.Pow(lat - nearest.latitude, 2) +
                Math.Pow(lon - nearest.longitude, 2));


            // Hämta temperatur + logg
            string url = null;
            try
            {
                url =
                   $"https://opendata-download-ocobs.smhi.se/api/version/1.0/parameter/5/station/{nearest.key}/period/latest-day/data.json";

                var json = await httpClient.GetStringAsync(url);


                var obj = JObject.Parse(json);
                var values = obj["value"] as JArray;

                if (values == null || values.Count == 0)
                {
                    MessageBox.Show("No values[] in latest-day response.", "OCOBS");
                    return (null, nearest, dist, null);
                }


                // Logga alla värden
                var sb = new StringBuilder();
                foreach (var v in values)
                {
                    sb.AppendLine(
                        $"date: {v["date"]}, value: {v["value"]}, depth: {v["depth"]}, quality: {v["quality"]}");
                }
                // MessageBox.Show(sb.ToString(), "OCOBS all temperature values (latest-day)");

                var last = values.Last["value"].Value<double>();

                return (last, nearest, dist, values);

            }
            catch (System.Exception ex)
            {// MessageBox.Show($"{ex.Message} url: {url}");
                return (null, nearest, dist, null);
            }

        }
        public List<OcobsStation> FilterStations(List<OcobsStation> stations)
        {
            var filtered = new List<OcobsStation>();

            foreach (var s in stations)
            {
                if (OcobsBanlist.Contains(s.id))
                {
                    // MessageBox.Show($"Banlist skip: {s.name} ({s.id})", "OCOBS Banlist");
                    continue;
                }

                if (!s.active)
                {
                    // MessageBox.Show($"Inactive skip: {s.name} ({s.id})", "OCOBS Inactive");
                    continue;
                }

                filtered.Add(s);
            }
            return filtered;
        }
        public async Task<List<OcobsStation>> GetSeaSalinityStationsAsync()
        {
            var url = "https://opendata-download-ocobs.smhi.se/api/version/1.0/parameter/4.json";
            var json = await httpClient2.GetStringAsync(url);

            /*MessageBox.Show(json.Substring(0, Math.Min(json.Length, 1500)),
                "OCOBS parameter 4 raw JSON");*/

            var root = JsonConvert.DeserializeObject<OcobsRoot>(json);
            return root.station;
        }
        public async Task<(double? sal, JArray allValues)> GetLatestSeaSalinityAsync(string stationKey)
        {
            var url =
                $"https://opendata-download-ocobs.smhi.se/api/version/1.0/parameter/4/station/{stationKey}/period/corrected-archive/data.csv";

            var csv = await httpClient2.GetStringAsync(url);

            /* MessageBox.Show(csv.Substring(0, Math.Min(csv.Length, 1500)),
                 "OCOBS salinity CSV raw");*/

            var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Förväntat format:
            // date;value;quality;depth
            // 2026-03-19T00:00:00Z;6.8;G;0

            var arr = new JArray();

            foreach (var line in lines.Skip(1)) // hoppa header
            {
                var parts = line.Split(';');
                if (parts.Length < 4) continue;

                var obj = new JObject
                {
                    ["date"] = parts[0],
                    ["value"] = double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : (double?)null,
                    ["quality"] = parts[2],
                    ["depth"] = parts[3]
                };

                arr.Add(obj);
            }

            if (arr.Count == 0)
            {
                /*MessageBox.Show("No salinity values parsed from CSV.", "OCOBS");
                return (null, null);*/
            }

            // Logga allt
            var sb = new StringBuilder();
            foreach (var v in arr)
            {
                sb.AppendLine(
                    $"{v["date"]}: {v["value"]} PSU (depth {v["depth"]}, quality {v["quality"]})");
            }
            //MessageBox.Show("Checkpoint last");

            //MessageBox.Show(sb.ToString(), "OCOBS all salinity values (CSV)");
            //MessageBox.Show("Checkpoint not reached");

            // Senaste värdet
            var last = arr.Last["value"]?.Value<double>();
            //MessageBox.Show($"Latest salinity value: {last} PSU", "OCOBS");

            return (last, arr);
        }

        public async Task<(double? sal, OcobsStation station, double distance, JArray allValues)>
            SmhiSeaSalinityAsync(double lat, double lon)
        {
            // MessageBox.Show("Checkpoint 0a");

            var stations = await GetSeaSalinityStationsAsync();
            // MessageBox.Show("Checkpoint 0b");

            var banlist = new List<OcobsStation>();
            var filtered = FilterStations(stations);
            // MessageBox.Show("Checkpoint 0c");

            var nearest = FindNearestActiveStation(lat, lon, filtered, banlist);
            // MessageBox.Show("Checkpoint 0d");

            if (nearest == null)
                return (null, null, 0, null);
            // MessageBox.Show("Checkpoint 0e");

            var dist = Math.Sqrt(
                Math.Pow(lat - nearest.latitude, 2) +
                Math.Pow(lon - nearest.longitude, 2));
            //MessageBox.Show("Checkpoint 0e2");

            /* MessageBox.Show(
                 $"Distance from click to salinity station {nearest.name}: {dist} (deg approx)",
                 "OCOBS Distance");*/
            //  MessageBox.Show("Checkpoint 0f");

            var (sal, allValues) = await GetLatestSeaSalinityAsync(nearest.key);
            // MessageBox.Show("Checkpoint 0g");

            return (sal, nearest, dist, allValues);
        }
        private void PauseRendering()
        {
            CompositionTarget.Rendering -= RenderLoop;
            mapControl.IsEnabled = false;
            renderingPaused = true;
            MessageBox.Show("Rendering pausad (F12 för att återuppta)");
        }

        private void ResumeRendering()
        {
            CompositionTarget.Rendering += RenderLoop;
            mapControl.IsEnabled = true;
            renderingPaused = false;
            MessageBox.Show("Rendering återupptagen");
        }

        private void ToggleRendering()
        {
            if (!renderingPaused)
                PauseRendering();
            else
                ResumeRendering();
        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                e.Handled = true; // stoppa Windows från att sno F12
                ToggleRendering();
            }
        }
        private void SetBaseMap(KnownTileSource source)
        {

            RemoveCinematicLayers();

            // Ta bort gamla baslagret
            var oldBase = mapControl.Map.Layers.FirstOrDefault(l => l.Name == "BaseMap");
            if (oldBase != null)
                mapControl.Map.Layers.Remove(oldBase);

            // Skapa nytt lager
            var newLayer = new TileLayer(KnownTileSources.Create(source))
            {
                Name = "BaseMap"
            };

            // Lägg det längst ner
            mapControl.Map.Layers.Insert(0, newLayer);
        }
        private void CmbBaseMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (CmbBaseMap.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (string.IsNullOrEmpty(selected)) return;
            switch (selected)
            {
                case "OSM":
                    SetBaseMap(KnownTileSource.OpenStreetMap);
                    break;

                case "ESRI Topo":
                    SetBaseMap(KnownTileSource.EsriWorldTopo);
                    break;

                case "ESRI Physical":
                    SetBaseMap(KnownTileSource.EsriWorldPhysical);
                    break;
                case "Bing Aerial":
                    SetBaseMap(KnownTileSource.BingAerial);
                    break;
                case "Stamen Watercolor":
                    MessageBox.Show("initial");
                    SetStamenWatercolor();
                    break;
                case "Blueprint":
                    MessageBox.Show("initial");
                    SetBlueprintMap();
                    break;
                case "Cinematic":
                    SetCinematic();
                    break;
            }
        }
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow();
            win.Owner = this;
            win.ShowDialog();
        }
        public ILayer? GetLayerByName(string name)
        {
            return mapControl.Map.Layers.FirstOrDefault(l => l.Name == name);
        }
        private string? GetVattendistrikt(double worldX, double worldY)
        {
            if (vattendistriktFeatures == null)
                return null;

            var point = new NetTopologySuite.Geometries.Point(worldX, worldY);

            foreach (var f in vattendistriktFeatures)
            {
                if (f.Geometry is NetTopologySuite.Geometries.Polygon poly)
                {
                    if (poly.Contains(point))
                        return f.Attributes["NAME"]?.ToString();
                }
                else if (f.Geometry is MultiPolygon mp)
                {
                    if (mp.Contains(point))
                        return f.Attributes["NAME"]?.ToString();
                }
            }

            return null;
        }

        private int PixelX(double worldX)
        {
            return (int)((worldX - rasterOriginX) / rasterPixelSize);
        }

        private int PixelY(double worldY)
        {
            Debug.WriteLine($"DEBUG rasterOriginY={rasterOriginY}, worldY={worldY}");
            return (int)((rasterOriginY - worldY) / rasterPixelSize);
        }

        private double ComputeEffectiveHabitat(Envelope env, NetTopologySuite.Geometries.Polygon poly)
        {
            int rasterWidth = 70000;
            int rasterHeight = 160000;

            int minPx = PixelX(env.MinX);
            int maxPx = PixelX(env.MaxX);
            int minPy = PixelY(env.MaxY);
            int maxPy = PixelY(env.MinY);

            Debug.WriteLine($"[4] Pixel window raw: minPx={minPx}, maxPx={maxPx}, minPy={minPy}, maxPy={maxPy}");

            minPx = Math.Max(0, minPx);
            minPy = Math.Max(0, minPy);
            maxPx = Math.Min(rasterWidth - 1, maxPx);
            maxPy = Math.Min(rasterHeight - 1, maxPy);

            int width = maxPx - minPx + 1;
            int height = maxPy - minPy + 1;

            if (width <= 0 || height <= 0)
            {
                Debug.WriteLine("Window outside raster!");
                return -1;
            }

            Debug.WriteLine($"[5] Window size: width={width}, height={height}");

            float[,] window = ReadRasterWindow(minPx, minPy, width, height);
            DrawRasterDebugOverlay(minPx, minPy, window, poly);
            double total = 0;
            int count = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double worldX = rasterOriginX + (minPx + x) * rasterPixelSize;
                    double worldY = rasterOriginY - (minPy + y) * rasterPixelSize;

                    // 🔥 TESTA covers istället för contains
                    if (!poly.Covers(new Point(worldX, worldY)))
                        continue;

                    double val = ComputeEffectivePixelLocal(window, x, y);

                    // 🔍 DEBUG
                    Debug.WriteLine($"Pixel ({x},{y}) value={val}");

                    total += val;
                    count++;

                    Debug.WriteLine($"Running total={total}, count={count}");
                }
            }

            Debug.WriteLine($"FINAL count={count}");
            Debug.WriteLine($"FINAL total={total}");

            double result = count > 0 ? total / count : -1;

            Debug.WriteLine($"FINAL average={result}");

            return result;
        }


        private double ComputeEffectivePixelLocal(float[,] window, int px, int py)
        {
            double sum = 0;
            double weightSum = 0;

            int width = window.GetLength(0);
            int height = window.GetLength(1);

            int radius = 5;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int nx = px + dx;
                    int ny = py + dy;

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    double weight = 1.0 / (1.0 + dist);

                    sum += window[nx, ny] * weight;
                    weightSum += weight;
                }
            }

            return sum / weightSum;
        }

        private void FinishHabitatMeasurement()
        {
            Log.Info("Habitat", "[CALC] Startar beräkning av effektiv habitatkvalitet...");
            measuringHabitat = false;

            var coords = habitatPoints
                .Select(p => new Coordinate(p.X, p.Y))
                .ToList();

            Debug.WriteLine("=== HABITAT START ===");
            for (int i = 0; i < coords.Count; i++)
                Debug.WriteLine($"[0] WebMercator point {i}: {coords[i].X}, {coords[i].Y}");

            // 🔥 Sätt PROJ path (KRITISKT)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projPath = Path.Combine(
                baseDir,
                "runtimes",
                "win-x64",
                "native",
                "maxrev.gdal.core.libshared"
            );

            OSGeo.GDAL.Gdal.SetConfigOption("PROJ_LIB", projPath);

            Debug.WriteLine($"proj.db exists: {File.Exists(Path.Combine(projPath, "proj.db"))}");

            // 🔥 Direkt WebMercator → SWEREF99 TM
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857); // WebMercator
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(3006); // SWEREF99 TM
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);



            var transform = new OSGeo.OSR.CoordinateTransformation(source, target);

            var swerefCoords = new List<Coordinate>();

            // REVERSE-CHECK – lägg till detta debyugstycke
            var reverse = new OSGeo.OSR.CoordinateTransformation(target, source); // target=3006, source=3857
            foreach (var c in swerefCoords)
            {
                double[] back = { c.X, c.Y, 0 };
                reverse.TransformPoint(back);
                Debug.WriteLine($"Reverse: SWEREF {c.X:F1},{c.Y:F1} → Mercator {back[0]:F1},{back[1]:F1} (skillnad <0.1m?)");
            }
            ///--------------
            foreach (var c in coords)
            {
                double[] point = new double[] { c.X, c.Y, 0 };

                transform.TransformPoint(point);

                Debug.WriteLine($"[1] SWEREF X/Y: {point[0]}, {point[1]}");

                swerefCoords.Add(new Coordinate(point[0], point[1]));
            }

            // Stäng polygon
            swerefCoords.Add(swerefCoords[0]);

            var polySweref = new NetTopologySuite.Geometries.Polygon(
                new LinearRing(swerefCoords.ToArray())
            );

            var envSweref = polySweref.EnvelopeInternal;

            Debug.WriteLine($"[2] Envelope SWEREF: minX={envSweref.MinX}, maxX={envSweref.MaxX}, minY={envSweref.MinY}, maxY={envSweref.MaxY}");

            double effective = ComputeEffectiveHabitat(envSweref, polySweref);
            Log.Info("Habitat", $"[OK] Habitatkvalitet beräknad: {effective:F1}%");

            MessageBox.Show($"Effektiv habitatkvalitet: {effective:F1} %");


            habitatPoints.Clear();
        }



        private float[,] ReadRasterWindow(int minPx, int minPy, int width, int height)
        {
            var ds = Gdal.Open("data/raster/vegkvot.tif", Access.GA_ReadOnly);
            var band = ds.GetRasterBand(1);
            Console.WriteLine("=== GeoTIFF metadata ===");
            Console.WriteLine($"Storlek: {ds.RasterXSize} × {ds.RasterYSize}");
            Console.WriteLine($"Band:   {ds.RasterCount}");



            short[] buffer = new short[width * height];
            Debug.WriteLine($"buffer length: {buffer.Length}");
            /*//DEbug cmnt out // 🔥 fyll med sentinel
for (int k = 0; k < buffer.Length; k++)
    buffer[k] = -9999; detta test fyller korrekt alla rutor gröna vilket visar att bufern täcker hela området, alltså är cellerna för få eller steglängden fel i inläsningen*/

            //decoompiled constructor public CPLErr ReadRaster(int xOff, int yOff, int xSize, int ySize, short[] buffer, int buf_xSize, int buf_ySize, int pixelSpace, int lineSpace)
            /*original  band.ReadRaster(
                  minPx, minPy, //<--- någon av variablerna måste nog vara fel eftersom exakt hälften av pixlarna är value:0 //eftersom vi det är den nedre hälten av cellerna vi saknar minPy=50%? mnPy*2 gav out of extent crash
                  width, height,
                  buffer,
                  width, height, //samma wifth/height igen? korrekt?
                  1, width //> vid 1:an står represent a 32 bit signed integer, tif är Int16 därför fel? om testar 2=konstigt ; om width*2 så läses exakt den vänstra halvan men inte den högra
              );*/

            int pixelSizeBytes = 2;  // short = 2 bytes
            int lineStrideBytes = width * pixelSizeBytes;

            CPLErr err = band.ReadRaster(
                minPx, minPy,
                width, height,
                buffer,
                width, height,
                pixelSizeBytes,          // pixelSpace = 2 bytes per pixel
                lineStrideBytes          // lineSpace = width * 2 bytes per rad
            );

            if (err != CPLErr.CE_None)
            {
                Debug.WriteLine($"ReadRaster fel: {err}");
                return null;
            }


            //debug
            Debug.WriteLine($"GDAL result: {band.GetDataset()}"); //vi skulle behöva dbugga bandReadraster och se om de har lika många celler som buffer.length
            //--- 

            float[,] result = new float[width, height];
            int i = 0;
            //
            int numbofzero = 0;
            //

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //real line short raw = buffer[i];
                    //test line
                    int index = i;
                    short raw = buffer[index];
                    //---
                    Debug.WriteLine($"RAW raster value: {raw}");
                    Log.Info("Raster", $"[CALC] RAW raster value: {raw}");
                    if (raw < 1) { numbofzero++; Debug.WriteLine($"numbofzerovalues/numbofvalues{numbofzero}/{i}"); }

                    result[x, y] = raw;
                    i++;
                }
            }
            return result;
        }
        private Color GetColor(double val)
        {
            val = Math.Clamp(val, 0, 100);

            // Blå (0) → Grön (100)
            byte r = (byte)(255 - (val * 2.55));
            byte g = (byte)(val * 2.55);
            byte b = 100;

            return new Color(r, g, b, 180); // semi-transparent
        }
        private void DrawRasterDebugOverlay(int minPx, int minPy, float[,] window, NetTopologySuite.Geometries.Polygon poly)
        {
            Debug.WriteLine("[A] DrawRasterDebugOverlay called");
            //overlayCanvas.Children.Clear();
            Debug.WriteLine("[B] overlayCanvas cleared");

            // 🔥 INIT EN GÅNG (lazy)
            if (!boolFlagRefDefined)
            {
                var src = new OSGeo.OSR.SpatialReference("");
                src.ImportFromEPSG(3006);
                src.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

                var dst = new OSGeo.OSR.SpatialReference("");
                dst.ImportFromEPSG(3857);
                dst.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

                _toWebMercator = new OSGeo.OSR.CoordinateTransformation(src, dst);
                boolFlagRefDefined = true;

            }
            int width = window.GetLength(0);
            int height = window.GetLength(1);

            var vp = mapControl.Map.Navigator.Viewport;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float val = window[x, y];

                    double worldX = rasterOriginX + (minPx + x) * rasterPixelSize;
                    double worldY = rasterOriginY - (minPy + y) * rasterPixelSize;

                    double[] pA = { worldX, worldY, 0 };
                    double[] pB = { worldX + rasterPixelSize, worldY - rasterPixelSize, 0 };

                    _toWebMercator.TransformPoint(pA);
                    _toWebMercator.TransformPoint(pB);

                    var p1 = vp.WorldToScreen(pA[0], pA[1]);
                    var p2 = vp.WorldToScreen(pB[0], pB[1]);

                    double left = Math.Min(p1.X, p2.X);
                    double top = Math.Min(p1.Y, p2.Y);
                    double rectWidth = Math.Abs(p2.X - p1.X);
                    double rectHeight = Math.Abs(p2.Y - p1.Y);
                    bool inside = poly.Covers(new NetTopologySuite.Geometries.Point(worldX, worldY));
                    byte c = (byte)(val * 2.55f);

                    var rect = new System.Windows.Shapes.Rectangle
                    {
                        Width = rectWidth,
                        Height = rectHeight,
                        Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(120, 0, c, 0)),
                        Stroke = inside ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 215, 0)) : null,
                        StrokeThickness = inside ? 1.5 : 0.0
                    };

                    Canvas.SetLeft(rect, left);
                    Canvas.SetTop(rect, top);

                    overlayCanvas.Children.Add(rect);
                }

            }
        }
        private void DrawHabitatPreview()
        {
            //overlayCanvas.Children.Clear();
            if (habitatPoints.Count < 2) return;

            var vp = mapControl.Map.Navigator.Viewport;

            for (int i = 0; i < habitatPoints.Count - 1; i++)
            {
                var a = vp.WorldToScreen(habitatPoints[i].X, habitatPoints[i].Y);
                var b = vp.WorldToScreen(habitatPoints[i + 1].X, habitatPoints[i + 1].Y);

                var line = new System.Windows.Shapes.Line
                {
                    X1 = a.X,
                    Y1 = a.Y,
                    X2 = b.X,
                    Y2 = b.Y,
                    Stroke = Brushes.AntiqueWhite,
                    StrokeThickness = 2.0,   // 🔥 float fungerar perfekt
                    SnapsToDevicePixels = true
                };

                overlayCanvas.Children.Add(line);
            }
        }
        public float GetDepth(double worldX, double worldY)
        {
            foreach (var tile in tiles)
            {
                if (worldX >= tile.MinX && worldX <= tile.MaxX &&
                    worldY >= tile.MinY && worldY <= tile.MaxY)
                {
                    return tile.Sample(worldX, worldY);
                }
            }

            return float.NaN; // land eller utanför
        }

        public class BathyTile
        {
            public string FilePath;
            public double MinX, MaxX;
            public double MinY, MaxY;
            public double OriginX, OriginY;
            public double PixelSize;
            public int Width, Height;

            public Dataset Dataset;
            public Band Band;

            public BathyTile(string path)
            {
                FilePath = path;

                Dataset = Gdal.Open(path, Access.GA_ReadOnly);
                Band = Dataset.GetRasterBand(1);

                Width = Dataset.RasterXSize;
                Height = Dataset.RasterYSize;

                // Läs geotransform
                double[] gt = new double[6];
                Dataset.GetGeoTransform(gt);

                OriginX = gt[0];      // top-left X
                OriginY = gt[3];      // top-left Y
                PixelSize = gt[1];    // pixel width (positiv)

                // Beräkna extent
                MinX = OriginX;
                MaxX = OriginX + Width * PixelSize;

                MinY = OriginY - Height * PixelSize;
                MaxY = OriginY;
            }

            public float Sample(double worldX, double worldY)
            {
                int px = (int)((worldX - OriginX) / PixelSize);
                int py = (int)((OriginY - worldY) / PixelSize);

                if (px < 0 || py < 0 || px >= Width || py >= Height)
                    return float.NaN;

                float[] buffer = new float[1];

                Band.ReadRaster(
                    px, py,
                    1, 1,
                    buffer,
                    1, 1,
                    4, 4 // Float32
                );

                return buffer[0];
            }
        }

        private List<BathyTile> tiles = new List<BathyTile>();
        public void LoadBathymetry()
        {
            tiles.Clear();

            tiles.Add(new BathyTile("data/raster/emod_D6_final.tif"));
            tiles.Add(new BathyTile("data/raster/emod_C6_final.tif"));
            // tiles.Add(new BathyTile("data/raster/emod_D7_final.tif"));
            Log.Info("Raster", $"Loaded bathymetry raster");

        }
        private void InitGdal()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // PROJ (för koordinattransformationer)
            var projPath = Path.Combine(
                baseDir,
                "runtimes",
                "win-x64",
                "native",
                "maxrev.gdal.core.libshared"
            );
            Gdal.SetConfigOption("PROJ_LIB", projPath);

            // GDAL data (CSV, SRS, m.m.)
            var gdalData = Path.Combine(projPath, "gdal-data");
            Gdal.SetConfigOption("GDAL_DATA", gdalData);

            // Drivrutiner (GeoTIFF, NetCDF, etc.)
            Gdal.SetConfigOption("GDAL_DRIVER_PATH", projPath);

            // Registrera alla drivrutiner
            Gdal.AllRegister();
            //Ladda rastret för hydrualisk konduktivitet
            // Transformera till SWEREF99
            // 🔥 Direkt WebMercator → SWEREF99 TM
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857); // WebMercator
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(3006); // SWEREF99 TM
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            mercatorToSweref99Tm = new OSGeo.OSR.CoordinateTransformation(source, target);

            LoadHydraulicConductivity();

        }
        private IFeature? FindNearestPowerTower(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 10000; // 10 km
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = powTowers.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private IFeature? FindNearestCable(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 10000; // 30 km
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = cableIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private IFeature? FindNearestFireStation(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 100000; // 10 mil
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = fireIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private string? GetCountyCode(string countyName)
        {
            if (CountyCodes.TryGetValue(countyName, out var code))
                return code;

            return null;
        }
        private IFeature? FindNearestSoilDepth(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 2000; // 2 km
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = soilIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        public class RasterTile
        {
            public string FilePath;
            public double MinX, MaxX;
            public double MinY, MaxY;
            public double OriginX, OriginY;
            public double PixelSize;
            public int Width, Height;
            public Dataset Dataset;
            public Band Band;

            public RasterTile(string path)
            {
                FilePath = path;
                Dataset = Gdal.Open(path, Access.GA_ReadOnly);
                Band = Dataset.GetRasterBand(1);

                Width = Dataset.RasterXSize;
                Height = Dataset.RasterYSize;

                double[] gt = new double[6];
                Dataset.GetGeoTransform(gt);

                OriginX = gt[0];
                OriginY = gt[3];
                PixelSize = gt[1];

                MinX = OriginX;
                MaxX = OriginX + Width * PixelSize;
                MinY = OriginY - Height * PixelSize;
                MaxY = OriginY;
            }

            public float Sample(double x, double y)
            {
                int px = (int)((x - OriginX) / PixelSize);
                int py = (int)((OriginY - y) / PixelSize);

                if (px < 0 || py < 0 || px >= Width || py >= Height)
                    return float.NaN;

                float[] buffer = new float[1];
                Band.ReadRaster(px, py, 1, 1, buffer, 1, 1, 4, 4);
                if (buffer[0] == -9999)
                    return float.NaN;

                return buffer[0];
            }
        }
        private List<RasterTile> sguTiles = new List<RasterTile>();

        public void LoadHydraulicConductivity()
        {
            sguTiles.Clear();
            sguTiles.Add(new RasterTile("data/raster/hyd_konduk_sgu.tif"));
        }
        public float GetHydraulicK(double worldX, double worldY)
        {
            double[] p = new double[] { worldX, worldY, 0 };


            mercatorToSweref99Tm.TransformPoint(p);

            double swX = p[0];
            double swY = p[1];

            foreach (var tile in sguTiles)
            {
                if (swX >= tile.MinX && swX <= tile.MaxX &&
                    swY >= tile.MinY && swY <= tile.MaxY)
                {
                    return tile.Sample(swX, swY);
                }
            }

            return float.NaN;
        }
        private IFeature? FindNearestRidge(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 5000; // 5 km
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = ridgeIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        public class PollenStation
        {
            public string Name { get; set; }
            public string Slug { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
        public static class PollenStations
        {
            public static readonly List<PollenStation> All = new()
    {
        new PollenStation { Name="Borlänge", Slug="borlange", Lat=60.485, Lon=15.437 },
        new PollenStation { Name="Bräkne-Hoby", Slug="brakne-hoby", Lat=56.233, Lon=15.117 },
        new PollenStation { Name="Eskilstuna", Slug="eskilstuna", Lat=59.371, Lon=16.509 },
        new PollenStation { Name="Forshaga", Slug="forshaga", Lat=59.528, Lon=13.481 },
        new PollenStation { Name="Gävle", Slug="gavle", Lat=60.674, Lon=17.141 },
        new PollenStation { Name="Göteborg", Slug="goteborg", Lat=57.708, Lon=11.974 },
        new PollenStation { Name="Hässleholm", Slug="hassleholm", Lat=56.159, Lon=13.766 },
        new PollenStation { Name="Jönköping", Slug="jonkoping", Lat=57.782, Lon=14.161 },
        new PollenStation { Name="Kiruna", Slug="kiruna", Lat=67.855, Lon=20.225 },
        new PollenStation { Name="Kristianstad", Slug="kristianstad", Lat=56.029, Lon=14.156 },
        new PollenStation { Name="Ljusdal", Slug="ljusdal", Lat=61.828, Lon=16.091 },
        new PollenStation { Name="Malmö", Slug="malmo", Lat=55.605, Lon=13.003 },
        new PollenStation { Name="Norrköping", Slug="norrkoping", Lat=58.587, Lon=16.192 },
        new PollenStation { Name="Nässjö", Slug="nassjo", Lat=57.653, Lon=14.694 },
        new PollenStation { Name="Piteå", Slug="pitea", Lat=65.318, Lon=21.479 },
        new PollenStation { Name="Skövde", Slug="skovde", Lat=58.391, Lon=13.846 },
        new PollenStation { Name="Stockholm", Slug="stockholm", Lat=59.329, Lon=18.068 },
        new PollenStation { Name="Storuman", Slug="storuman", Lat=65.095, Lon=17.118 },
        new PollenStation { Name="Sundsvall", Slug="sundsvall", Lat=62.391, Lon=17.306 },
        new PollenStation { Name="Umeå", Slug="umea", Lat=63.825, Lon=20.263 },
        new PollenStation { Name="Visby", Slug="visby", Lat=57.634, Lon=18.294 },
        new PollenStation { Name="Västervik", Slug="vastervik", Lat=57.758, Lon=16.637 },
        new PollenStation { Name="Östersund", Slug="ostersund", Lat=63.179, Lon=14.635 }
    };
        }
        public (PollenStation? station, double distanceKm) FindNearestStation(double lat, double lon) //was static
        {
            PollenStation? best = null;
            double bestDist = double.MaxValue;

            foreach (var s in PollenStations.All)
            {
                double d = Haversine(lat, lon, s.Lat, s.Lon);

                if (d < bestDist)
                {
                    bestDist = d;
                    best = s;
                }
            }

            return (best, bestDist / 1000);
        }
        public (IFeature? well, double distanceMeters) FindNearestWell(double x, double y)
        {
            var pt = new Point(x, y);

            // sökradie i meter2 (10 km)
            double searchRadius = 10000;
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = wellIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);

                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }
            double returnDist = bestDist / 1000;
            return (nearest, bestDist);
        }
        private IFeature? IdentifyLaArea(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = laIndex.Query(pt.EnvelopeInternal);

            foreach (var f in candidates)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        //Tidsserielogik //quick skipp
        public async Task<EnvironmentalResult> GetEnvironmentalDataForPoint(double worldX, double worldY)
        {
            // MessageBox.Show("Inside GetEnvironmentalDataForPoint");
            var result = new EnvironmentalResult();
            result.WorldX = worldX;
            result.WorldY = worldY;

            // 1. Konvertera till lon/lat
            var lonlat = SphericalMercator.ToLonLat(worldX, worldY);
            double lon = lonlat.lon;
            double lat = lonlat.lat;
            result.Lon = lon;
            result.Lat = lat;

            // 2. Hitta län
            var county = FindCountyPolygon(lon, lat);
            if (county != null)
            {
                result.CountyName = county.Attributes["NAME_1"]?.ToString();
            }
            // MessageBox.Show("Controle 1");
            // 3. Hitta kommun
            var kommun = FindKommunPolygon(lon, lat);
            if (kommun != null)
            {
                result.KommunName = kommun.Attributes["NAME_2"]?.ToString();
            }

            // 4. Hitta LA-område
            var la = IdentifyLaArea(lon, lat);
            if (la != null)
            {
                result.LaName = la.Attributes["Namn"]?.ToString();
                result.LaCode = la.Attributes["Lakod"]?.ToString();
            }
            // MessageBox.Show("Controle 2");

            // 5. Hästar
            if (result.CountyName != null)
                result.Horses = CountyHorseHandler.GetHorsesForCounty(result.CountyName);

            // 6. Kor
            if (result.CountyName != null)
                result.Cattle = CountyCattleHandler.GetCattleForCounty(result.CountyName);

            // 7. TBE
            if (result.CountyName != null)
                result.TbeCases = CountyTbeHandler.GetTbeCasesForCounty(result.CountyName);

            // 8. Harpest
            if (result.CountyName != null)
                result.HarpestCases = CountyHarpestHandler.GetHarpestForCounty(result.CountyName);
            // MessageBox.Show("Controle 3");
            // 9. Länsnamn
            if (result.CountyName != null)
            {
                result.CountyArea = CountyAreaHandler.GetAreaForCounty(result.CountyName);
            }

            // 9. Studier
            result.NearbyStudies = studyHandler.GetStudiesNearby(lon, lat, studyHandlerTagFilter);
            //MessageBox.Show("Controle 4");
            // 10. SMHI-väder
            var forecast = await GetWeather(lat, lon);
            var now = forecast.timeSeries[0];
            result.WeatherSummary =
                $"Temp: {now.data.air_temperature}°C, " +
                $"Vind: {now.data.wind_speed} m/s, " +
                $"Fukt: {now.data.relative_humidity}%";
            result.Visibility = now.data.visibility_in_air;
            result.SymbolCode = now.data.symbol_code;
            result.CloudCoverTotal = now.data.cloud_area_fraction;
            result.CloudCoverLow = now.data.low_type_cloud_area_fraction;
            result.CloudCoverMedium = now.data.medium_type_cloud_area_fraction;
            result.CloudCoverHigh = now.data.high_type_cloud_area_fraction;
            result.WindDirection = now.data.wind_from_direction;
            result.PrecipitationRate = now.data.precipitation_rate_mean;
            result.CloudBaseAltitude = now.data.cloud_base_altitude;
            result.CloudTopAltitude = now.data.cloud_top_altitude;
            result.PrecipitationSort = now.data.precipitation_sort;
            double stationLon = forecast.geometry.coordinates[0];
            double stationLat = forecast.geometry.coordinates[1];
            result.DistSmhiWeather = Haversine(lat, lon, stationLat, stationLon);
            result.SmhiWeatherTemp = now.data.air_temperature;


            //MessageBox.Show("SMHI OK");
            //MessageBox.Show($"now.data.relative_humidity:{now.data.relative_humidity}");
            // 11. Open-Meteo
            var om = await OpenMeteoService.GetAllData(lat, lon);
            if (om.Elevation != null) result.Elevation = om.Elevation;
            if (om.Aqi != null) result.Aqi = om.Aqi;
            if (om.Pm25 != null) result.Pm25 = om.Pm25;
            if (om.Pm10 != null) result.Pm10 = om.Pm10;
            if (om.O3 != null) result.O3 = om.O3;
            if (om.UvMaxToday != null) result.UvMaxToday = om.UvMaxToday;
            if (om.Ammonia != null) result.Ammonia = om.Ammonia;
            if (om.Methane != null) result.Methane = om.Methane;
            if (om.So2 != null) result.So2 = om.So2;
            if (om.Co != null) result.Co = om.Co;
            if (om.Co2 != null) result.Co2 = om.Co2;
            if (om.No2 != null) result.No2 = om.No2;
            if (om.AerosolOpticalDepth != null) result.AerosolOpticalDepth = om.AerosolOpticalDepth;

            // 12. Arsenik
            var nearestArsenic = FindNearestArsenicPoint(worldX, worldY);
            if (nearestArsenic != null)
            {
                var props = nearestArsenic.Attributes;
                string arsenikRaw = props["as_ppm"]?.ToString() ?? "okänt";
                result.EnvironmentalSummary = $"Arsenik: {arsenikRaw} ppm";
            }
            if (nearestArsenic != null)
            {
                Point pt = null;

                if (nearestArsenic.Geometry is MultiPoint mp && mp.NumGeometries > 0)
                    pt = mp.Geometries[0] as Point;

                if (pt != null)
                {
                    double dx = pt.X - worldX;
                    double dy = pt.Y - worldY;
                    result.DistArsenic = Math.Sqrt(dx * dx + dy * dy);
                }
            }

            // 13
            var power = FindNearestPowerTower(worldX, worldY);
            if (power != null)
            {
                result.DistPowerTower = power.Geometry.Distance(new Point(worldX, worldY));
            }
            var cable = FindNearestCable(worldX, worldY);
            if (cable != null)
            {
                result.DistPowerCable = cable.Geometry.Distance(new Point(worldX, worldY));
            }
            var fStation = FindNearestFireStation(worldX, worldY);
            if (fStation != null)
            {
                result.DistFireStation = fStation.Geometry.Distance(new Point(worldX, worldY));
            }
            float k_value = GetHydraulicK(worldX, worldY);
            if (!float.IsNaN(k_value))
            {
                result.HydraulicK = k_value;
            }
            var ridge = FindNearestRidge(worldX, worldY);
            if (ridge != null)
            {
                result.DistRidge = ridge.Geometry.Distance(new Point(worldX, worldY));
                result.RidgeDirection = double.TryParse(ridge.Attributes["riktn"]?.ToString(), out double dir) ? dir : null;
            }
            // MessageBox.Show("Checkpoint 0");

            var soil_depth = FindNearestSoilDepth(worldX, worldY);
            if (soil_depth != null)
            {
                result.DistSoilDepth = soil_depth.Geometry.Distance(new Point(worldX, worldY));

                var d = soil_depth.Attributes["djup"]?.ToString();
                if (double.TryParse(d, out double depth))
                    result.SoilDepthMeters = depth;
            }
            //MessageBox.Show("Checkpoint 1");
            var config = ConfigLoader.Load();

            if ((bool)!config.fastrun)
            {
                var resultS = await SmhiSeaSalinityAsync(lat, lon);
                var sal = resultS.sal;
                var nearestSal = resultS.station;
                var distSal = resultS.distance;
                result.SeaSalinity = sal;
                result.DistSeaSalinity = distSal * 111000;
                // MessageBox.Show("Checkpoint 2");
                var resultW = await SmhiSeaTemperatureAsync(lat, lon);
                var tempW = resultW.temp;
                var nearestSea = resultW.station;
                var distSea = resultW.distance;
                result.SeaTemperature = tempW;
                result.DistSeaTemp = distSea * 111000; // grader → meter
                                                       // MessageBox.Show("Checkpoint 3");
            }
            else
            {
                result.SeaSalinity = null;
                result.DistSeaSalinity = null;
                result.SeaTemperature = null;
                result.DistSeaTemp = null;
            }
            if ((bool)!config.fastrun)
            {
                // Hitta närmaste pollenstation
                var (station, distKm) = FindNearestStation(lat, lon);

                if (station != null)
                {
                    result.PollenStationDist = distKm;

                    var scraped = await PollenScraper.GetForLocationAsync(station.Slug);
                    if (scraped != null)
                    {
                        // 0 betyder "ingen rapport" → NULL
                        result.PollenBirch = scraped.Birch > 0 ? scraped.Birch : null;
                        result.PollenGrass = scraped.Grass > 0 ? scraped.Grass : null;
                        result.PollenMugwort = scraped.Mugwort > 0 ? scraped.Mugwort : null;
                        result.PollenAlder = scraped.Alder > 0 ? scraped.Alder : null;
                        result.PollenHazel = scraped.Hazel > 0 ? scraped.Hazel : null;
                        result.PollenWillow = scraped.Willow > 0 ? scraped.Willow : null;
                    }
                }
            }
            else
            {
                // Fastrun → inga pollen
                result.PollenBirch = null;
                result.PollenGrass = null;
                result.PollenMugwort = null;
                result.PollenAlder = null;
                result.PollenHazel = null;
                result.PollenWillow = null;
                result.PollenStationDist = null;
            }
            var geo = FindNearestGeokemi(worldX, worldY);
            if (geo != null)
            {
                result.DistGeokemi = geo.Geometry.Distance(new Point(worldX, worldY));

                var p = geo.Attributes;

                result.Al2O3 = TryDouble(p["al2o3_proc"]);
                result.AsPpm = TryDouble(p["as_ppm"]);
                result.BaO = TryDouble(p["bao_proc"]);
                result.CaO = TryDouble(p["cao_proc"]);
                result.ClPpm = TryDouble(p["cl_ppm"]);
                result.CoPpm = TryDouble(p["co_ppm"]);
                result.CrPpm = TryDouble(p["cr_ppm"]);
                result.CuPpm = TryDouble(p["cu_ppm"]);
                result.Fe2O3 = TryDouble(p["fe2o3_proc"]);
                result.K2O = TryDouble(p["k2o_proc"]);
                result.MgO = TryDouble(p["mgo_proc"]);
                result.MnO = TryDouble(p["mno_proc"]);
                result.MoPpm = TryDouble(p["mo_ppm"]);
                result.Na2O = TryDouble(p["na2o_proc"]);
                result.NiPpm = TryDouble(p["ni_ppm"]);
                result.P2O5 = TryDouble(p["p2o5_proc"]);
                result.PbPpm = TryDouble(p["pb_ppm"]);
                result.RbPpm = TryDouble(p["rb_ppm"]);
                result.SPpm = TryDouble(p["s_ppm"]);
                result.SiO2 = TryDouble(p["sio2_proc"]);
                result.SrPpm = TryDouble(p["sr_ppm"]);
                result.TiO2 = TryDouble(p["tio2_proc"]);
                result.VPpm = TryDouble(p["v_ppm"]);
                result.ZnPpm = TryDouble(p["zn_ppm"]);
                result.ZrPpm = TryDouble(p["zr_ppm"]);
            }
            var berg = GetBerggrundFeature(worldX, worldY);
            if (berg != null)
            {
                var attr = berg.Attributes;
                string litologi = attr["litologi"]?.ToString()?.ToLower() ?? "";
                string lithologyEn = attr["lithology"]?.ToString() ?? "";

                result.Lithology = litologi;
                result.LithologyEn = lithologyEn;

                // Boolean flags (case-insensitive)
                result.IsGranit = litologi.Contains("granit");
                result.IsDiabas = litologi.Contains("diabas");
                result.IsBasalt = litologi.Contains("basalt");
                result.IsAmfibolit = litologi.Contains("amfibolit");
                result.IsSandsten = litologi.Contains("sandsten");
                result.IsKonglomerat = litologi.Contains("konglomerat");
                result.IsLerskiffer = litologi.Contains("lerskiffer") || litologi.Contains("skiffer");
                result.IsKalksten = litologi.Contains("kalksten");
                result.IsLera = litologi.Contains("lera");
                result.IsKol = litologi.Contains("kol");
                result.IsGnejs = litologi.Contains("gnejs");
                result.IsPegmatit = litologi.Contains("pegmatit");
                result.IsSkiffer = litologi.Contains("skiffer");
                result.IsKvartsit = litologi.Contains("kvartsit");
            }
            var jord = GetJordartFeature(worldX, worldY);
            if (jord != null)
            {
                var a = jord.Attributes;
                string jordart = a["jg2_tx"]?.ToString()?.ToLower() ?? "";

                // Boolean flags
                result.IsBerg = jordart.Contains("berg");
                result.IsSilt = jordart.Contains("silt");
                result.IsSand = jordart.Contains("sand");
                result.IsGrus = jordart.Contains("grus");
                result.IsTorv = jordart.Contains("torv");
                result.IsIsalv = jordart.Contains("isälv") || jordart.Contains("isälvssediment");
                result.IsVittringsjord = jordart.Contains("vittringsjord");

                // Morän (alla typer)
                result.IsMoran = jordart.Contains("morän");

                // Moränlera / lerig morän
                result.IsMoränlera = jordart.Contains("moränlera") || jordart.Contains("lerig morän");
            }
            var gv = GetGrundvattenFeature(worldX, worldY);
            result.IsGrundvattenMagasin = gv != null;
            // Exekvera sökningen mot järnvägsindexet
            var rw = GetRailwayFeature(worldX, worldY);
            result.IsNearRailway = rw != null;
            //  Avstånd och riktning till kust
            double coastDist = DistanceToCoast(lon, lat);
            double coastDir = DirectionToCoast(lon, lat);

            result.DistCoast = coastDist;
            result.DirCoast = coastDir;

            var (riverDist, nearestPoint) = FindNearestRiver(lon, lat);
            result.DistRiver = riverDist;

            if (nearestPoint != null)
            {
                var merc = SphericalMercator.FromLonLat(lon, lat);
                result.DirRiver = Bearing(
                    new MPoint(merc.x, merc.y),
                    new MPoint(nearestPoint.X, nearestPoint.Y)
                );
            }
            var sb = GetSubbasinFeature(worldX, worldY);
            if (sb != null)
            {
                result.IsInSubbasin = true;
            }
            else
            {
                result.IsInSubbasin = false;
            }
            if (result.CountyName != null)
            {
                var h = HarvestHandler.GetHarvestForCounty2(result.CountyName);
                if (h != null)
                {
                    result.HarvestHostvete = h.Hostvete;
                    result.HarvestVarvete = h.Varvete;
                    result.HarvestRag = h.Rag;
                    result.HarvestHostkorn = h.Hostkorn;
                    result.HarvestVarkorn = h.Varkorn;
                    result.HarvestHavre = h.Havre;
                    result.HarvestArter = h.Arter;
                    result.HarvestAkerbonor = h.Akerbonor;
                    result.HarvestMatpotatis = h.Matpotatis;
                    result.HarvestPotatisStarkelse = h.PotatisStarkelse;
                    result.HarvestSockerbetor = h.Sockerbetor;
                    result.HarvestHostraps = h.Hostraps;
                    result.HarvestVarraps = h.Varraps;
                    result.HarvestSlattervallTotal = h.SlattervallTotal;
                    result.HarvestSlattervallForsta = h.SlattervallForsta;
                    result.HarvestSlattervallAttervaxt = h.SlattervallAttervaxt;
                }
            }


            return result;
        }

        public class EnvironmentalResult
        {
            public double WorldX { get; set; }
            public double WorldY { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }

            public string CountyName { get; set; }
            public string KommunName { get; set; }
            public string LaName { get; set; }
            public string LaCode { get; set; }

            public int? Horses { get; set; }
            public int? Cattle { get; set; }
            public int? TbeCases { get; set; }
            public int? HarpestCases { get; set; }
            public double? CountyArea { get; set; }

            public List<IFeature> NearbyStudies { get; set; } = new();

            public string WeatherSummary { get; set; }
            public string EnvironmentalSummary { get; set; }

            public double? Elevation { get; set; }
            public double? Aqi { get; set; }
            public double? Pm25 { get; set; }
            public double? Pm10 { get; set; }
            public double? O3 { get; set; }
            public double? UvMaxToday { get; set; }
            public double? Ammonia { get; set; }
            public double? Methane { get; set; }
            public double? So2 { get; set; }
            public double? Co { get; set; }
            public double? Co2 { get; set; }
            public double? No2 { get; set; }
            public double? AerosolOpticalDepth { get; set; }
            public double? Visibility { get; set; }
            public int? SymbolCode { get; set; }
            public double? CloudCoverTotal { get; set; }
            public double? CloudCoverLow { get; set; }
            public double? CloudCoverMedium { get; set; }
            public double? CloudCoverHigh { get; set; }
            public double? WindDirection { get; set; }
            public double? PrecipitationRate { get; set; }
            public double? CloudBaseAltitude { get; set; }
            public double? CloudTopAltitude { get; set; }
            public double? PrecipitationSort { get; set; }
            public double? DistPowerTower { get; set; }
            public double? DistPowerCable { get; set; }
            public double? DistFireStation { get; set; }
            public double? DistRidge { get; set; }
            public double? DistSoilDepth { get; set; }
            public double? SoilDepthMeters { get; set; }
            public double? RidgeDirection { get; set; }
            public double? HydraulicK { get; set; }
            public double? DistSmhiWeather { get; set; }
            public double? SmhiWeatherTemp { get; set; }
            public double? DistSeaTemp { get; set; }
            public double? SeaTemperature { get; set; }
            public double? DistSeaSalinity { get; set; }
            public double? SeaSalinity { get; set; }
            public double? DistArsenic { get; set; }
            public double? Al2O3 { get; set; }
            public double? AsPpm { get; set; }
            public double? BaO { get; set; }
            public double? CaO { get; set; }
            public double? ClPpm { get; set; }
            public double? CoPpm { get; set; }
            public double? CrPpm { get; set; }
            public double? CuPpm { get; set; }
            public double? Fe2O3 { get; set; }
            public double? K2O { get; set; }
            public double? MgO { get; set; }
            public double? MnO { get; set; }
            public double? MoPpm { get; set; }
            public double? Na2O { get; set; }
            public double? NiPpm { get; set; }
            public double? P2O5 { get; set; }
            public double? PbPpm { get; set; }
            public double? RbPpm { get; set; }
            public double? SPpm { get; set; }
            public double? SiO2 { get; set; }
            public double? SrPpm { get; set; }
            public double? TiO2 { get; set; }
            public double? VPpm { get; set; }
            public double? ZnPpm { get; set; }
            public double? ZrPpm { get; set; }
            public double? DistGeokemi { get; set; }
            public string? Lithology { get; set; }
            public string? LithologyEn { get; set; }
            public bool IsGranit { get; set; }
            public bool IsDiabas { get; set; }
            public bool IsBasalt { get; set; }
            public bool IsAmfibolit { get; set; }
            public bool IsSandsten { get; set; }
            public bool IsKonglomerat { get; set; }
            public bool IsLerskiffer { get; set; }
            public bool IsKalksten { get; set; }
            public bool IsLera { get; set; }
            public bool IsKol { get; set; }
            public bool IsGnejs { get; set; }
            public bool IsPegmatit { get; set; }
            public bool IsSkiffer { get; set; }
            public bool IsKvartsit { get; set; }
            public bool IsBerg { get; set; }
            public bool IsSilt { get; set; }
            public bool IsSand { get; set; }
            public bool IsGrus { get; set; }
            public bool IsTorv { get; set; }
            public bool IsMoran { get; set; }
            public bool IsIsalv { get; set; }
            public bool IsVittringsjord { get; set; }
            public bool IsMoränlera { get; set; }
            public bool IsGrundvattenMagasin { get; set; }
            public bool IsNearRailway { get; set; }
            public double? DistCoast { get; set; }
            public double? DirCoast { get; set; }
            public double? DistRiver { get; set; }
            public double? DirRiver { get; set; }
            public bool IsInSubbasin { get; set; } //subbasin=lake
            public double? HarvestHostvete { get; set; }
            public double? HarvestVarvete { get; set; }
            public double? HarvestRag { get; set; }
            public double? HarvestHostkorn { get; set; }
            public double? HarvestVarkorn { get; set; }
            public double? HarvestHavre { get; set; }
            public double? HarvestArter { get; set; }
            public double? HarvestAkerbonor { get; set; }
            public double? HarvestMatpotatis { get; set; }
            public double? HarvestPotatisStarkelse { get; set; }
            public double? HarvestSockerbetor { get; set; }
            public double? HarvestHostraps { get; set; }
            public double? HarvestVarraps { get; set; }
            public double? HarvestSlattervallTotal { get; set; }
            public double? HarvestSlattervallForsta { get; set; }
            public double? HarvestSlattervallAttervaxt { get; set; }
            public double? PollenBirch { get; set; }
            public double? PollenGrass { get; set; }
            public double? PollenMugwort { get; set; }
            public double? PollenAlder { get; set; }
            public double? PollenHazel { get; set; }
            public double? PollenWillow { get; set; }
            public double? PollenStationDist { get; set; }


        }
        public async Task TimeSerieLauncher(string geojsonPath)
        {
            var config = ConfigLoader.Load();
            if (!config.doTimeSeries && !manualActivation)
                return;
            manualActivation = false;

            //if (doTimeSeriesBool!=true && manuelActivation!=true)return -  denna boolean kan exempelvis styras med en config.json vi kan lägga i pluginsfolder och manualactivation bool kan vi sätta med en knapp i settingsWindow som dels sätter bool och sen kallar metoden TimeSerieLuancher
            //MessageBox.Show("Inside TimeSerieLauncher");

            // Initiera databas (skapar filen om den saknas)
            TimeSeriesDatabase.Initialize();

            // Läs GeoJSON
            string json = File.ReadAllText(geojsonPath);
            var reader = new GeoJsonReader();
            var fc = reader.Read<FeatureCollection>(json);

            int index = 0;

            foreach (var f in fc)
            {
                if (f.Geometry is NetTopologySuite.Geometries.Point p)
                {
                    this.Title = $"Processing point {index} / {50000}";
                    index++;
                    string pointId = $"P{index}";

                    // MessageBox.Show($"Running point #{index}");

                    var data = await GetEnvironmentalDataForPoint(p.X, p.Y);

                    // Spara i SQLite
                    TimeSeriesDatabase.Insert(data, pointId);

                    // Debugga parallellt
                    //MessageBox.Show(FormatEnvironmentalResult(data), $"Resultat punkt {index}");

                    // Testkör bara 3 punkter
                    //något slags low-energy visa mig hur många som har beabetats - utan att intefererera som vid msg box
                    if (index >= 50000)
                        break;
                }

            }
            var rows = TimeSeriesReader.ReadAll();
            File.WriteAllText("debug_dbrow.txt", string.Join("\n\n", rows.Select(r =>
    string.Join("\n", r.Select(kv => $"{kv.Key}: {kv.Value}"))
)));

            /*  foreach (var row in rows)
              {
                  string msg = string.Join("\n", row.Select(kv => $"{kv.Key}: {kv.Value}"));
                  MessageBox.Show(msg, "DB Row");

                  //tmp lines
                  File.WriteAllText("debug_dbrow.txt", msg);
                  Process.Start("notepad.exe", "debug_dbrow.txt");
                  //
              }*/


            MessageBox.Show("Tidsserie inskriven i SQLite!");
        }

        private string FormatEnvironmentalResult(EnvironmentalResult r) //debugg only
        {
            var sb = new StringBuilder();

            sb.AppendLine("--- Testresultat ---");
            sb.AppendLine($"WorldX: {r.WorldX}");
            sb.AppendLine($"WorldY: {r.WorldY}");
            sb.AppendLine($"Lon: {r.Lon:F6}");
            sb.AppendLine($"Lat: {r.Lat:F6}");

            sb.AppendLine($"\nLän: {r.CountyName ?? "?"}");
            sb.AppendLine($"Kommun: {r.KommunName ?? "?"}");
            sb.AppendLine($"LA-område: {r.LaName ?? "?"} ({r.LaCode ?? "?"})");

            sb.AppendLine($"\nHästar: {r.Horses?.ToString() ?? "?"}");
            sb.AppendLine($"Kor: {r.Cattle?.ToString() ?? "?"}");
            sb.AppendLine($"TBE-fall: {r.TbeCases?.ToString() ?? "?"}");
            sb.AppendLine($"Harpestfall: {r.HarpestCases?.ToString() ?? "?"}");
            sb.AppendLine($"Areal: {r.CountyArea?.ToString("F0") ?? "?"} km²");

            sb.AppendLine($"\nVäder: {r.WeatherSummary}");
            sb.AppendLine($"Miljö: {r.EnvironmentalSummary}");

            sb.AppendLine($"\nStudier nära: {r.NearbyStudies.Count}");

            return sb.ToString();
        }
        private double? TryDouble(object raw)
        {
            if (raw == null) return null;
            if (double.TryParse(raw.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;
            return null;
        }
        private (double dist, Coordinate nearestPoint) FindNearestRiver(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var p = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            // Query bounding box
            // var candidates = riverIndex.Query(p.EnvelopeInternal);
            double searchRadius = 1000000; // 100 km, floder är långa
            var env = p.Buffer(searchRadius).EnvelopeInternal;

            var candidates = riverIndex.Query(env);


            double minDist = double.MaxValue;
            Coordinate nearest = null;

            foreach (var f in candidates)
            {
                var geom = f.Geometry;

                if (geom is LineString ls)
                {
                    ProcessLineString(ls, p, ref minDist, ref nearest);
                }
                else if (geom is MultiLineString mls)
                {
                    foreach (LineString ls2 in mls.Geometries)
                        ProcessLineString(ls2, p, ref minDist, ref nearest);
                }
            }
            return (minDist, nearest);
        }
        private void ProcessLineString(LineString ls, Point p, ref double minDist, ref Coordinate nearest)
        {
            for (int i = 0; i < ls.NumPoints - 1; i++)
            {
                var a = ls.GetCoordinateN(i);
                var b = ls.GetCoordinateN(i + 1);

                var np = NearestPointOnSegment(p, a, b);
                double d = p.Distance(np);

                if (d < minDist)
                {
                    minDist = d;
                    nearest = np.Coordinate;
                }
            }
        }
        private IFeature? GetSubbasinFeature(double x, double y)
        {
            var p = new Point(x, y);
            var candidates = subbasinIndex.Query(p.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry.Contains(p))
                        return f;
                }
                catch { }
            }
            return null;
        }
        private async void RunAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressText.Text = "Startar analys...";
            Log.Info("Analysis", "Starting analysis");
            var engine = new AnalysisEngine(rows);
            var results = await Task.Run(() =>
            {
                return engine.RunAndReturnResults((i, total) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressText.Text = $"Bearbetar variabel {i} av {total}";
                    });
                });
            });
            MessageBox.Show("Analys klar. Variabler: " + results.Count);
            /*var sb = new StringBuilder();
            foreach (var r in results)
            {
                sb.AppendLine($"{r.Name}: {r.ExplanationDegree}");
            }*/

            //MessageBox.Show(sb.ToString());
            string csvPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "analysis_results.csv"
);

            AnalysisEngine.ExportResultsToCsv(results, csvPath);

            MessageBox.Show("Analys klar! CSV skapad på skrivbordet:\n" + csvPath);
            MessageBox.Show("Startar annalys del 2");
            var interactionResults = engine.RunInteractionAnalysis();
            engine.ExportInteractionResults(interactionResults, "interaction_results.csv");
            MessageBox.Show("Annalys del 2 klar");

        }
        private async void BtnDownloadMindatSweden_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.mindat.org/v1/localities/polygon/";

            // Polygon som täcker hela Sverige (lite generös för att få med allt)
            var polygonData = new
            {
                type = "Polygon",
                coordinates = new[]
                {
            new[]
            {
                new[] { 10.5, 55.0 },
                new[] { 24.5, 55.0 },
                new[] { 24.5, 69.5 },
                new[] { 10.5, 69.5 },
                new[] { 10.5, 55.0 }
            }
        }
            };

            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = System.TimeSpan.FromMinutes(2); // Mindat kan vara lite långsam
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36");
                var response = await client.PostAsJsonAsync(url, new { polygon = polygonData });

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Fel från Mindat: {response.StatusCode}", "Fel");
                    return;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Skapa en korrekt GeoJSON FeatureCollection
                var geojson = new
                {
                    type = "FeatureCollection",
                    features = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(jsonResponse)
                                           .GetProperty("features")
                                           .EnumerateArray()           // Konverterar till enumerable
                                           .ToList()                   // Gör det till en lista
                };

                // Spara filen
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string finalGeoJson = System.Text.Json.JsonSerializer.Serialize(geojson, options);

                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Mindat_Sweden_All_Localities.geojson");

                await File.WriteAllTextAsync(filePath, finalGeoJson);

                MessageBox.Show($"✅ Klar!\n\nHämtade lokaliteter och sparade som:\n{filePath}",
                                "Mindat nedladdning klar");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fel vid hämtning:\n{ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowDailyOverlay()
        {
            string namnsdag = GetNamnsdag();
            bool flaggdag = Flaggdagar();

            NamnsdagText.Text = $"Dagens namnsdag:\n{namnsdag}";

            if (flaggdag)
                FlagImage.Visibility = System.Windows.Visibility.Visible;
            else
                FlagImage.Visibility = System.Windows.Visibility.Collapsed;

            DailyOverlay.Visibility = System.Windows.Visibility.Visible;
        }
        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            DailyOverlay.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void CloseOrtNamnselementOverlay_Click(object sender, RoutedEventArgs e)
        {
            OrtNamnselementOverlay.Visibility = System.Windows.Visibility.Collapsed;
        }
        private string GetNamnsdag()
        {
            var today = DateTime.Today;
            string key = today.ToString("MM-dd");

            var namnsdagar = new Dictionary<string, string>
    {
        { "01-01", "Nyårsdagen" },
        { "01-02", "Svea" },
        { "01-03", "Alfred, Alfrida" },
        { "01-04", "Rut" },
        { "01-05", "Hanna, Hannele" },
        { "01-06", "Kasper, Melker, Baltsar" },
        { "01-07", "August, Augusta" },
        { "01-08", "Erland" },
        { "01-09", "Gunnar, Gunder" },
        { "01-10", "Sigurd, Sigbritt" },
        { "01-11", "Jan, Jannike" },
        { "01-12", "Frideborg, Fridolf" },
        { "01-13", "Knut" },
        { "01-14", "Felix, Felicia" },
        { "01-15", "Laura, Lorentz" },
        { "01-16", "Hjalmar, Helmer" },
        { "01-17", "Anton, Tony" },
        { "01-18", "Hilda, Hildur" },
        { "01-19", "Henrik" },
        { "01-20", "Fabian, Sebastian" },
        { "01-21", "Agnes, Agneta" },
        { "01-22", "Vincent, Viktor" },
        { "01-23", "Frej, Freja" },
        { "01-24", "Erika" },
        { "01-25", "Paul, Pål" },
        { "01-26", "Bodil, Boel" },
        { "01-27", "Göte, Göta" },
        { "01-28", "Karl, Karla" },
        { "01-29", "Diana" },
        { "01-30", "Gunilla, Gunhild" },
        { "01-31", "Ivar, Joar" },

        // ⭐ Jag fortsätter hela året nedan
        { "02-01", "Max, Maximilian" },
        { "02-02", "Kyndelsmässodagen" },
        { "02-03", "Disa, Hjördis" },
        { "02-04", "Ansgar, Anselm" },
        { "02-05", "Agata" },
        { "02-06", "Dorotea, Doris" },
        { "02-07", "Rikard, Dick" },
        { "02-08", "Berta, Bert" },
        { "02-09", "Fanny, Franciska" },
        { "02-10", "Iris" },
        { "02-11", "Yngve, Inge" },
        { "02-12", "Evelina, Evy" },
        { "02-13", "Agne, Ove" },
        { "02-14", "Valentin" },
        { "02-15", "Sigfrid" },
        { "02-16", "Julia, Julius" },
        { "02-17", "Alexandra, Sandra" },
        { "02-18", "Frida, Fritiof" },
        { "02-19", "Gabriella, Ella" },
        { "02-20", "Vivianne" },
        { "02-21", "Hilding" },
        { "02-22", "Pia" },
        { "02-23", "Torsten, Torun" },
        { "02-24", "Mattias, Mats" },
        { "02-25", "Sigvard, Sivert" },
        { "02-26", "Torgny, Torkel" },
        { "02-27", "Lage" },
        { "02-28", "Maria" },
        { "02-29", "Skottdagen" },

        // ⭐ Mars
        { "03-01", "Albin" },
        { "03-02", "Ernst, Erna" },
        { "03-03", "Gunborg, Gunvor" },
        { "03-04", "Adrian, Adriana" },
        { "03-05", "Tora, Tove" },
        { "03-06", "Ebba, Ebbe" },
        { "03-07", "Camilla" },
        { "03-08", "Siv" },
        { "03-09", "Torbjörn, Torleif" },
        { "03-10", "Edla, Ada" },
        { "03-11", "Edvin, Egon" },
        { "03-12", "Viktoria" },
        { "03-13", "Greger" },
        { "03-14", "Matilda, Maud" },
        { "03-15", "Kristoffer, Christel" },
        { "03-16", "Herbert, Gilbert" },
        { "03-17", "Gertrud" },
        { "03-18", "Edvard, Edmund" },
        { "03-19", "Josef, Josefina" },
        { "03-20", "Joakim, Kim" },
        { "03-21", "Bengt" },
        { "03-22", "Kennet, Kent" },
        { "03-23", "Gerda, Gerd" },
        { "03-24", "Gabriel, Rafael" },
        { "03-25", "Marie bebådelsedag" },
        { "03-26", "Emanuel" },
        { "03-27", "Rudolf, Ralf" },
        { "03-28", "Malkolm, Morgan" },
        { "03-29", "Jonas, Jens" },
        { "03-30", "Holger, Holmfrid" },
        { "03-31", "Ester" },

        // ⭐ April
        { "04-01", "Harald, Hervor" },
        { "04-02", "Gudmund, Ingemund" },
        { "04-03", "Ferdinand, Nanna" },
        { "04-04", "Marianne, Marlene" },
        { "04-05", "Irene, Irja" },
        { "04-06", "Vilhelm, William" },
        { "04-07", "Irma, Irmelin" },
        { "04-08", "Nadja, Tanja" },
        { "04-09", "Otto, Ottilia" },
        { "04-10", "Ingvar, Ingvor" },
        { "04-11", "Ulf, Ylva" },
        { "04-12", "Liv" },
        { "04-13", "Artur, Douglas" },
        { "04-14", "Tiburtius" },
        { "04-15", "Olivia, Oliver" },
        { "04-16", "Patrik, Patricia" },
        { "04-17", "Elias, Elis" },
        { "04-18", "Valdemar, Volmar" },
        { "04-19", "Olaus, Ola" },
        { "04-20", "Amalia, Amelie" },
        { "04-21", "Anneli, Annika" },
        { "04-22", "Allan, Glenn" },
        { "04-23", "Georg, Göran" },
        { "04-24", "Vega" },
        { "04-25", "Markus" },
        { "04-26", "Teresia, Terese" },
        { "04-27", "Engelbrekt" },
        { "04-28", "Ture, Tyra" },
        { "04-29", "Tyko" },
        { "04-30", "Valborg" },

        // ⭐ Maj
        { "05-01", "Valborg" },
        { "05-02", "Filip, Filippa" },
        { "05-03", "John, Jane" },
        { "05-04", "Monika, Mona" },
        { "05-05", "Gotthard, Erhard" },
        { "05-06", "Marit, Rita" },
        { "05-07", "Carina, Carita" },
        { "05-08", "Åke" },
        { "05-09", "Reidar, Reidun" },
        { "05-10", "Esbjörn, Styrbjörn" },
        { "05-11", "Märta, Märit" },
        { "05-12", "Charlotta, Lotta" },
        { "05-13", "Linnea, Linn" },
        { "05-14", "Halvard, Halvar" },
        { "05-15", "Sofia, Sonja" },
        { "05-16", "Ronald, Ronny" },
        { "05-17", "Rebecka, Ruben" },
        { "05-18", "Erik" },
        { "05-19", "Maj, Majken" },
        { "05-20", "Karolina, Carola" },
        { "05-21", "Konstantin, Conny" },
        { "05-22", "Hemming, Henning" },
        { "05-23", "Desideria, Desirée" },
        { "05-24", "Ivan, Vanja" },
        { "05-25", "Urban" },
        { "05-26", "Vilhelmina, Vilma" },
        { "05-27", "Beda, Blenda" },
        { "05-28", "Ingeborg, Borghild" },
        { "05-29", "Yvonne, Jeanette" },
        { "05-30", "Vera, Veronika" },
        { "05-31", "Petronella, Pernilla" },

        // ⭐ Juni
        { "06-01", "Gun, Gunnel" },
        { "06-02", "Rutger, Roger" },
        { "06-03", "Ingemar, Gudmar" },
        { "06-04", "Solbritt, Solveig" },
        { "06-05", "Bo" },
        { "06-06", "Gustav, Gösta" },
        { "06-07", "Robert, Robin" },
        { "06-08", "Eivor, Majvor" },
        { "06-09", "Börje, Birger" },
        { "06-10", "Svante, Boris" },
        { "06-11", "Bertil, Berit" },
        { "06-12", "Eskil" },
        { "06-13", "Aina, Aino" },
        { "06-14", "Håkan, Hakon" },
        { "06-15", "Margit, Margot" },
        { "06-16", "Axel, Axelina" },
        { "06-17", "Torborg, Torvald" },
        { "06-18", "Björn, Bjarne" },
        { "06-19", "Germund, Görel" },
        { "06-20", "Linda" },
        { "06-21", "Alf, Alvar" },
        { "06-22", "Paulina, Paula" },
        { "06-23", "Adolf, Alice" },
        { "06-24", "Johannes, John" },
        { "06-25", "David, Salomon" },
        { "06-26", "Rakel, Lea" },
        { "06-27", "Selma, Fingal" },
        { "06-28", "Leo" },
        { "06-29", "Peter, Petra" },
        { "06-30", "Elof, Leif" },

        // ⭐ Juli
        { "07-01", "Aron, Mirjam" },
        { "07-02", "Rosa, Rosita" },
        { "07-03", "Aurora" },
        { "07-04", "Ulrika, Ulla" },
        { "07-05", "Laila, Ritva" },
        { "07-06", "Esaias, Jessika" },
        { "07-07", "Klas" },
        { "07-08", "Kjell" },
        { "07-09", "Jörgen, Örjan" },
        { "07-10", "André, Andrea" },
        { "07-11", "Eleonora, Ellinor" },
        { "07-12", "Herman, Hermine" },
        { "07-13", "Joel, Judit" },
        { "07-14", "Folke" },
        { "07-15", "Ragnhild, Ragnvald" },
        { "07-16", "Reinhold, Reine" },
        { "07-17", "Bruno" },
        { "07-18", "Fredrik, Fritz" },
        { "07-19", "Sara" },
        { "07-20", "Margareta, Greta" },
        { "07-21", "Johanna" },
        { "07-22", "Magdalena, Madeleine" },
        { "07-23", "Emma" },
        { "07-24", "Kristina, Kerstin" },
        { "07-25", "Jakob" },
        { "07-26", "Jesper" },
        { "07-27", "Marta" },
        { "07-28", "Botvid, Seved" },
        { "07-29", "Olof" },
        { "07-30", "Algot" },
        { "07-31", "Helena, Elin" },

        // ⭐ Augusti
        { "08-01", "Per" },
        { "08-02", "Karin, Kajsa" },
        { "08-03", "Tage" },
        { "08-04", "Arne, Arnold" },
        { "08-05", "Ulrik, Alrik" },
        { "08-06", "Alfons, Inez" },
        { "08-07", "Dennis, Denise" },
        { "08-08", "Silvia, Sylvia" },
        { "08-09", "Roland" },
        { "08-10", "Lars" },
        { "08-11", "Susanna" },
        { "08-12", "Klara" },
        { "08-13", "Kaj" },
        { "08-14", "Uno" },
        { "08-15", "Stella, Estelle" },
        { "08-16", "Brynolf" },
        { "08-17", "Verner, Valter" },
        { "08-18", "Ellen, Lena" },
        { "08-19", "Magnus, Måns" },
        { "08-20", "Bernhard, Bernt" },
        { "08-21", "Jon, Jonna" },
        { "08-22", "Henrietta, Henrika" },
        { "08-23", "Signe, Signhild" },
        { "08-24", "Bartolomeus" },
        { "08-25", "Lovisa, Louise" },
        { "08-26", "Östen" },
        { "08-27", "Rolf, Raoul" },
        { "08-28", "Fatima, Leila" },
        { "08-29", "Hans, Hampus" },
        { "08-30", "Albert, Alberta" },
        { "08-31", "Arvid, Vidar" },

        // ⭐ September
        { "09-01", "Samuel, Sam" },
        { "09-02", "Justus, Justina" },
        { "09-03", "Alfhild, Alva" },
        { "09-04", "Gisela" },
        { "09-05", "Adela, Heidi" },
        { "09-06", "Lilian, Lilly" },
        { "09-07", "Kevin, Roy" },
        { "09-08", "Alma, Hulda" },
        { "09-09", "Anita, Annette" },
        { "09-10", "Tord, Turid" },
        { "09-11", "Dagny, Helny" },
        { "09-12", "Åsa, Åslög" },
        { "09-13", "Sture" },
        { "09-14", "Ida" },
        { "09-15", "Sigrid, Siri" },
        { "09-16", "Dag" },
        { "09-17", "Hildegard, Magnhild" },
        { "09-18", "Orvar" },
        { "09-19", "Fredrika" },
        { "09-20", "Elise, Lisa" },
        { "09-21", "Matteus" },
        { "09-22", "Maurits, Moritz" },
        { "09-23", "Tekla" },
{ "09-24", "Gerhard, Gert" },
{ "09-25", "Tryggve" },
{ "09-26", "Enar, Einar" },
{ "09-27", "Dagmar, Rigmor" },
{ "09-28", "Lennart, Leonhard" },
{ "09-29", "Mikael, Mikaela" },
{ "09-30", "Helge" },

// ⭐ Oktober
{ "10-01", "Ragnar, Ragna" },
{ "10-02", "Ludvig, Love" },
{ "10-03", "Evald, Osvald" },
{ "10-04", "Frans, Frank" },
{ "10-05", "Bror" },
{ "10-06", "Jenny, Jennifer" },
{ "10-07", "Birgitta, Britta" },
{ "10-08", "Nils" },
{ "10-09", "Ingrid, Inger" },
{ "10-10", "Harry, Harriet" },
{ "10-11", "Erling, Jarl" },
{ "10-12", "Valfrid, Manfred" },
{ "10-13", "Berit, Birgit" },
{ "10-14", "Stellan" },
{ "10-15", "Hedvig, Hillevi" },
{ "10-16", "Finn" },
{ "10-17", "Antonia, Toini" },
{ "10-18", "Lukas" },
{ "10-19", "Tor, Tore" },
{ "10-20", "Sibylla" },
{ "10-21", "Ursula, Yrsa" },
{ "10-22", "Marika, Marita" },
{ "10-23", "Severin, Sören" },
{ "10-24", "Evert, Eilert" },
{ "10-25", "Inga, Ingalill" },
{ "10-26", "Amanda, Rasmus" },
{ "10-27", "Sabina" },
{ "10-28", "Simon, Simone" },
{ "10-29", "Viola" },
{ "10-30", "Elsa, Isabella" },
{ "10-31", "Edit, Edgar" },

// ⭐ November
{ "11-01", "Alla helgons dag" },
{ "11-02", "Tobias" },
{ "11-03", "Hubert, Hugo" },
{ "11-04", "Sverker" },
{ "11-05", "Eugen, Eugenia" },
{ "11-06", "Gustav Adolf" },
{ "11-07", "Ingegerd, Ingela" },
{ "11-08", "Vendela" },
{ "11-09", "Teodor, Teodora" },
{ "11-10", "Martin, Martina" },
{ "11-11", "Mårten" },
{ "11-12", "Konrad, Kurt" },
{ "11-13", "Kasper" },
{ "11-14", "Emil, Emilia" },
{ "11-15", "Leopold" },
{ "11-16", "Vibeke, Viveka" },
{ "11-17", "Naemi, Naima" },
{ "11-18", "Lydia" },
{ "11-19", "Elisabet, Lisbet" },
{ "11-20", "Pontus, Marina" },
{ "11-21", "Helga, Olga" },
{ "11-22", "Cecilia, Sissela" },
{ "11-23", "Klemens" },
{ "11-24", "Gudrun, Rune" },
{ "11-25", "Katarina, Katja" },
{ "11-26", "Linus" },
{ "11-27", "Astrid, Asta" },
{ "11-28", "Malte" },
{ "11-29", "Sune" },
{ "11-30", "Andreas, Anders" },

// ⭐ December
{ "12-01", "Oskar, Ossian" },
{ "12-02", "Beata, Beatrice" },
{ "12-03", "Lydia" },
{ "12-04", "Barbara, Barbro" },
{ "12-05", "Sven" },
{ "12-06", "Nikolaus" },
{ "12-07", "Angela, Angelika" },
{ "12-08", "Virginia" },
{ "12-09", "Anna" },
{ "12-10", "Malin, Malena" },
{ "12-11", "Daniel, Daniela" },
{ "12-12", "Alexander, Alexis" },
{ "12-13", "Lucia" },
{ "12-14", "Sten, Sixten" },
{ "12-15", "Gottfrid" },
{ "12-16", "Assar" },
{ "12-17", "Stig" },
{ "12-18", "Abraham" },
{ "12-19", "Isak" },
{ "12-20", "Israel, Moses" },
{ "12-21", "Tomas" },
{ "12-22", "Natanael, Jonatan" },
{ "12-23", "Adam" },
{ "12-24", "Eva" },
{ "12-25", "Juldagen" },
{ "12-26", "Stefan" },
{ "12-27", "Johannes" },
{ "12-28", "Benjamin" },
{ "12-29", "Natalia, Natalie" },
{ "12-30", "Abel, Set" },
{ "12-31", "Sylvester" },
    };

            return namnsdagar.TryGetValue(key, out var namn)
                ? namn
                : "Ingen namnsdag hittad";
        }
        private bool Flaggdagar()
        {
            var today = DateTime.Today;
            string key = today.ToString("MM-dd");

            var flaggdagar = new HashSet<string>
    {
    "01-01", // Nyårsdagen
    "01-28", // Konungens namnsdag
    "03-12", // Kronprinsessans namnsdag
    "04-30", // Konungens födelsedag
    "05-01", // Första maj
    "06-06", // Nationaldagen
    "07-14", // Kronprinsessans födelsedag
    "08-08", // Drottningens namnsdag
    "10-24", // FN-dagen
    "11-06", // Gustav Adolfsdagen
    "12-10", // Nobeldagen
    "12-23", // Drottningens födelsedag
    "12-25", // Juldagen
    };

            return flaggdagar.Contains(key);
        }
        private protected void OtherSpecialDays()
        {
            var today = DateTime.Today;
            var specials = new Dictionary<string, (string title, string desc)>
    {
        { "10-04", ("Kanelbullens dag", "Svensk högtidsdag instiftad 1999 av Hembakningsrådet.") },
        { "03-25", ("Våffeldagen", "Traditionell svensk matdag med våfflor sedan 1700-talet.") },
        { "11-14", ("Ostbågens dag", "Inofficiell högtid för ostbågar, firas sedan 2016.") },
        { "01-12", ("Tjugondag Knut", "Dagen då julen kastas ut.") },
        { "02-01", ("Vegetariska dagen", "Uppmärksammar vegetarisk kost och hållbarhet.") },
        { "12-09", ("Pepparkakans dag", "Svensk matdag för pepparkakor.") }
    };

            string key = today.ToString("MM-dd");

            if (specials.TryGetValue(key, out var info))
            {
                SpecialDayTitle.Text = info.title;
                SpecialDaysOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                SpecialDaysOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private IFeature? GetPostcodeFeature(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);
            var candidates = postcodeIndex.Query(point.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry.Contains(point))
                        return f;
                }
                catch { }
            }

            return null;
        }
        public string? GetPostcode(double x, double y)
        {
            var f = GetPostcodeFeature(x, y);
            if (f == null)
                return null;

            var a = f.Attributes;
            return a["ID"]?.ToString();
        }

        private bool IsInsideSweden(double x, double y)
        {
            var point = new NetTopologySuite.Geometries.Point(x, y);
            var candidates = swedenIndex.Query(point.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry.Contains(point))
                        return true;
                }
                catch { }
            }

            return false;
        }
        private IFeature? FindNearestChurch(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 100000; // 10 mil
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = churchFullIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private (double X, double Y) ConvertWebMercatorToSweref(double x, double y)
        {
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857);
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(3006);
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var transform = new OSGeo.OSR.CoordinateTransformation(source, target);

            double[] p = { x, y, 0 };
            transform.TransformPoint(p);

            return (p[0], p[1]);
        }
        private static (double X, double Y) ConvertWebMercatorToRt90(double x, double y)
        {
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857);
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(3021); // RT90 2.5 gon V
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var transform = new OSGeo.OSR.CoordinateTransformation(source, target);

            double[] p = { x, y, 0 };
            transform.TransformPoint(p);

            return (p[0], p[1]);
        }

        private static (double X, double Y) ConvertWebMercatorToUtm33(double x, double y)
        {
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857);
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(32633); // WGS84 UTM zone 33N
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var transform = new OSGeo.OSR.CoordinateTransformation(source, target);

            double[] p = { x, y, 0 };
            transform.TransformPoint(p);

            return (p[0], p[1]);
        }
        private static (double X, double Y) ConvertWebMercatorToEpsg(double x, double y, int epsgTarget)
        {
            var source = new OSGeo.OSR.SpatialReference("");
            source.ImportFromEPSG(3857); // WebMercator
            source.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var target = new OSGeo.OSR.SpatialReference("");
            target.ImportFromEPSG(epsgTarget);
            target.SetAxisMappingStrategy(OSGeo.OSR.AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var transform = new OSGeo.OSR.CoordinateTransformation(source, target);

            double[] p = { x, y, 0 };
            transform.TransformPoint(p);

            return (p[0], p[1]);
        }
        private void MusicToggle_Click(object sender, RoutedEventArgs e)
        {
            if (MusicToggle.IsChecked == true)
            {
                musicPlayer.Play();
                MusicIcon.Source = new BitmapImage(new System.Uri("/data/bilder/music/music_on.png", UriKind.Relative));
            }
            else
            {
                musicPlayer.Pause();
                MusicIcon.Source = new BitmapImage(new System.Uri("/data/bilder/music/music_off.png", UriKind.Relative));
            }
        }
        private IFeature? IdentifySocken(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = sockenIndex.Query(pt.EnvelopeInternal);

            foreach (var f in candidates)
            {
                if (f.Geometry.Contains(pt))
                    return f;
            }

            return null;
        }
        private IFeature? FindNearestEducation(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 25000; // 2.5 mil
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = educationIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private IFeature? FindNearestHealth(double x, double y)
        {
            var pt = new Point(x, y);

            double searchRadius = 25000; // 2.5 mil
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = healthIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private IFeature? FindNearestHarbour(double x, double y)
        {
            if (harbourIndex == null)
                return null;

            var pt = new Point(x, y);

            double searchRadius = 25000; // 2.5 mil
            var env = pt.Buffer(searchRadius).EnvelopeInternal;

            var candidates = harbourIndex.Query(env);

            IFeature? nearest = null;
            double bestDist = double.MaxValue;

            foreach (var f in candidates)
            {
                double d = f.Geometry.Distance(pt);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = f;
                }
            }

            if (bestDist > searchRadius)
                return null;

            return nearest;
        }
        private (string? landsdel, IFeature? feature)? IdentifyNuts(double lon, double lat)
        {
            var merc = SphericalMercator.FromLonLat(lon, lat);
            var pt = new NetTopologySuite.Geometries.Point(merc.x, merc.y);

            var candidates = nutsIndex.Query(pt.EnvelopeInternal);
            string? landsdel = null;

            foreach (var f in candidates)
            {
                if (!f.Geometry.Contains(pt))
                    continue;
                var id = f.Attributes["NUTS_ID"]?.ToString() ?? "Okänt NUTS‑ID";

                if (id == "SE1") landsdel = "Östra Sverige";
                if (id == "SE2") landsdel = "Södra Sverige";
                if (id == "SE3") landsdel = "Norra Sverige";

                var bannedNuts = new HashSet<string>
        {
            "SE","SE1","SE2","SE3","SE4","SE5",
            "SE6","SE7","SE8","SE9","SE10","SE11",
            "SE12","SE13","SE14","SE315","SE16","SE17",
            "SE18","SE19","SE20","SE21"
        };

                if (f.Geometry.Contains(pt) && !bannedNuts.Contains(id))
                    return (landsdel, f);

            }

            return null;
        }

        private void ProcessLineString2(LineString ls, Point p, ref double minDist, ref Coordinate? nearest)
        {
            var distanceOp = new NetTopologySuite.Operation.Distance.DistanceOp(ls, p);
            var closestPoints = distanceOp.NearestPoints();
            double currentDist = distanceOp.Distance();

            if (currentDist < minDist)
            {
                minDist = currentDist;
                nearest = closestPoints[0]; // Hittat en närmare punkt på linjesegmentet!
            }
        }
        private string FormatLineOutput(string name, double dist, Coordinate? nearestPoint)
        {
            string output = $"\n> {name}:";
            if (double.IsNaN(dist) || double.IsInfinity(dist) || dist == double.MaxValue)
            {
                output += "\n  Avstånd: (Hittades inte)";
            }
            else
            {
                double distInKm = dist / 1000.0;
                output += $"\n  Avstånd: {distInKm:F1} km";

                if (nearestPoint != null)
                {
                    // Visar nu Lat/Lon istället för X/Y i meter!
                    output += $"\n  Närmaste punkt (WGS84): Lat={nearestPoint.Y:F4}, Lon={nearestPoint.X:F4}";
                }
            }
            return output;
        }
        private (double dist, Coordinate? nearestPoint) FindNearestLineWGS84(double clickLon, double clickLat, STRtree<IFeature> lineIndex)
        {
            // HÄR ÄR FIXEN: Vi frågar indexet efter hela jordklotet i EPSG:3857-meter!
            var allCandidates = lineIndex.Query(new NetTopologySuite.Geometries.Envelope(-20037508.34, 20037508.34, -20037508.34, 20037508.34));

            double minDistInMeter = double.MaxValue;
            Coordinate? nearestPointWGS84 = null;

            foreach (var f in allCandidates)
            {
                var geom = f.Geometry;
                if (geom == null) continue;

                var lineStrings = NetTopologySuite.Geometries.Utilities.LinearComponentExtracter.GetLines(geom);

                foreach (LineString ls in lineStrings)
                {
                    for (int i = 0; i < ls.Coordinates.Length - 1; i++)
                    {
                        var pt1_3857 = ls.Coordinates[i];
                        var pt2_3857 = ls.Coordinates[i + 1];

                        // 1. Räkna ut närmaste punkt på segmentet i EPSG:3857 (meter) utifrån klickets meter-koordinater
                        var clickMerc = SphericalMercator.FromLonLat(clickLon, clickLat);
                        Coordinate closestSegmentPoint3857 = GetClosestPointOnSegment(clickMerc.x, clickMerc.y, pt1_3857.X, pt1_3857.Y, pt2_3857.X, pt2_3857.Y);

                        // 2. Gör om den närmaste punkten från EPSG:3857-meter till WGS84-grader (Lat/Lon)
                        // (Här använder jag en vanlig invers SphericalMercator-formel, kolla vad din klass heter, t.ex. ToLonLat)
                        var closestWGS84 = SphericalMercator.ToLonLat(closestSegmentPoint3857.X, closestSegmentPoint3857.Y);

                        // 3. Räkna ut det sanna avståndet på jordklotet med din Haversine-metod!
                        double currentDist = Haversine(clickLat, clickLon, closestWGS84.lat, closestWGS84.lon);

                        if (currentDist < minDistInMeter)
                        {
                            minDistInMeter = currentDist;
                            // Spara punkten som WGS84 så att din FormatLineOutput kan visa snygga grader
                            nearestPointWGS84 = new Coordinate(closestWGS84.lon, closestWGS84.lat);
                        }
                    }
                }
            }

            return (minDistInMeter, nearestPointWGS84);
        }

        // Matematisk hjälprotin för att hitta närmaste punkt på ett linjesegment i ett 2D-plan (Lon/Lat)
        private Coordinate GetClosestPointOnSegment(double pX, double pY, double ax, double ay, double bx, double by)
        {
            double abX = bx - ax;
            double abY = by - ay;
            double apX = pX - ax;
            double apY = pY - ay;

            double abLenSq = abX * abX + abY * abY;
            if (abLenSq == 0) return new Coordinate(ax, ay);

            // Projektion
            double t = (apX * abX + apY * abY) / abLenSq;
            t = Math.Max(0, Math.Min(1, t)); // Håll oss inom segmentets start/slut

            return new Coordinate(ax + t * abX, ay + t * abY);
        }
        private readonly Dictionary<string, string> CountyToNutsCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { "Stockholm", "SE110" },
    { "Uppsala", "SE121" },
    { "Södermanland", "SE122" },
    { "Östergötland", "SE123" },
    { "Jönköping", "SE124" },
    { "Kronoberg", "SE125" },
    { "Kalmar", "SE213" },
    { "Gotland", "SE127" },
    { "Blekinge", "SE128" },
    { "Skåne", "SE22" },
    { "Halland", "SE231" },
    { "Västra Götaland", "SE232" }, //Vissa koder här är forfarande fel
    { "Värmland", "SE311" },
    { "Örebro", "SE312" },
    { "Västmanland", "SE125" },
    { "Dalarna", "SE321" },
    { "Gävleborg", "SE322" },
    { "Västernorrland", "SE323" },
    { "Jämtland", "SE331" },
    { "Västerbotten", "SE332" },
    { "Norrbotten", "SE333" }
};
        private string GetRegionCodeByCounty(string countyName)
        {
            if (string.IsNullOrWhiteSpace(countyName))
                return null;

            if (CountyToNutsCode.TryGetValue(countyName, out string nutsCode))
                return nutsCode;

            // Fallback: försök matcha del av namnet
            foreach (var kvp in CountyToNutsCode)
            {
                if (countyName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains(countyName, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }

            return null; // Inget hittat
        }
        private void UpdateUserLocationIcon()
        {
            if (!(userLocation != null)) return;
            if (UiFrozen) return;

            var screenPos = mapControl.Map.Navigator.Viewport.WorldToScreen(userLocation);

            Canvas.SetLeft(UserMarker, screenPos.X - UserMarker.Width / 2);
            Canvas.SetTop(UserMarker, screenPos.Y - UserMarker.Height / 2);

            //userLocationIcon.Visibility = System.Windows.Visibility.Visible;
        }

        public async Task GetUserLocationAsync()
        {
            var access = await Geolocator.RequestAccessAsync();
            if (access != GeolocationAccessStatus.Allowed)
                return;

            var geolocator = new Geolocator { DesiredAccuracyInMeters = 50 };
            var pos = await geolocator.GetGeopositionAsync();

            double lat = pos.Coordinate.Point.Position.Latitude;
            double lon = pos.Coordinate.Point.Position.Longitude;

            // Konvertera till WebMercator (Mapsui)
            var wm = SphericalMercator.FromLonLat(lon, lat);
            userLocation = new MPoint(wm.x, wm.y);
            //MessageBox.Show($"x:{wm.x}, y:{wm.y}"); debugg
            UpdateUserLocationIcon();
        }
        private IFeature? GetRailwayFeature(double x, double y)
        {
            var point = new Point(x, y);
            var candidates = railwayIndex.Query(point.EnvelopeInternal);

            foreach (var f in candidates)
            {
                try
                {
                    if (f.Geometry != null && f.Geometry.Contains(point))
                    {
                        return f;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Railway error: " + ex.Message);
                }
            }
            return null;
        }
        private static Mapsui.Layers.Layer CreateEbhLayer(IEnumerable<Mapsui.IFeature> features, string name, Color fillColor)
        {
            var provider = new MemoryProvider(features);

            var style = new Mapsui.Styles.SymbolStyle
            {
                SymbolType = SymbolType.Ellipse,
                Fill = new Mapsui.Styles.Brush(fillColor),
                Outline = new Mapsui.Styles.Pen(Color.Black, 1),
                SymbolScale = 0.5
            };

            return new Mapsui.Layers.Layer(name)
            {
                DataSource = provider,
                Style = style
            };
        }

        private void SetStamenWatercolor()
        {
            MessageBox.Show("called");
            RemoveAllBaseLayers();

            try
            {
                var attribution = new BruTile.Attribution("© Stamen Design / Stadia Maps");

                var tileSource = new HttpTileSource(
                    new GlobalSphericalMercator(0, 16),
                    "https://tiles.stadiamaps.com/tiles/stamen_watercolor/{z}/{x}/{y}.jpg",
                    name: "Stamen Watercolor",
                    attribution: attribution
                );

                var layer = new TileLayer(tileSource) { Name = "BaseMap", Opacity = 1.0 };
                mapControl.Map.Layers.Insert(0, layer);
                mapControl.Refresh();

                MessageBox.Show("added?");
                System.Diagnostics.Debug.WriteLine("✅ Stamen Watercolor tile source skapad");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Fel Stamen Watercolor: {ex.Message}");
                MessageBox.Show($"Fel vid Watercolor: {ex.Message}");
            }
        }

        private void SetBlueprintMap()
        {
            MessageBox.Show("called");

            RemoveAllBaseLayers();
            var attribution = new BruTile.Attribution(
                "© Stamen Design, under CC BY 3.0",
                "https://stamen.com"
            );
            try
            {
                var tileSource = new HttpTileSource(
                    new GlobalSphericalMercator(0, 19),
                    "https://stamen-tiles-{s}.a.ssl.fastly.net/toner-lite/{z}/{x}/{y}.png",
                    new[] { "a", "b", "c", "d" },
                    name: "Blueprint",
                    attribution: attribution
                );

                var layer = new TileLayer(tileSource)
                {
                    Name = "BaseMap",
                    Opacity = 1.0
                };

                mapControl.Map.Layers.Insert(0, layer);
                mapControl.Refresh();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveAllBaseLayers()
        {
            var baseLayers = mapControl.Map.Layers
                .Where(l => l.Name?.Contains("BaseMap") == true)
                .ToList();

            foreach (var layer in baseLayers)
                mapControl.Map.Layers.Remove(layer);
        }
        private void Oppen3DWiewerWindow(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "LAS-filer (*.las)|*.las|Alla filer (*.*)|*.*",
                InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "three_d")
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Vi skickar med den valda filstigen till fönstret!
                bool fillWalls = true;
                var viewer = new ModelViewerWindow(openFileDialog.FileName, fillWalls);
                viewer.Show();
            }
        }
        private void SetCinematic()
        {
            _cinematicMode = true;
            iconCanvas.Visibility = System.Windows.Visibility.Collapsed;
            locatorCanvas.Visibility = System.Windows.Visibility.Collapsed;
            // 1. Ta bort basemap (tile layers)
            var baseLayers = mapControl.Map.Layers
                .Where(l => l.Name == "BaseMap")
                .ToList();

            foreach (var layer in baseLayers)
                mapControl.Map.Layers.Remove(layer);



            // 2. Ta bort gamla cinematic layers om de finns
            var existingWorld = mapControl.Map.Layers.FirstOrDefault(l => l.Name == "World");
            if (existingWorld != null)
                mapControl.Map.Layers.Remove(existingWorld);

            var existingCities = mapControl.Map.Layers.FirstOrDefault(l => l.Name == "Cities");
            if (existingCities != null)
                mapControl.Map.Layers.Remove(existingCities);

            var existingWater = mapControl.Map.Layers.FirstOrDefault(l => l.Name == "Water");
            if (existingWater != null)
                mapControl.Map.Layers.Remove(existingWater);

            // ta bort andra lager 
            foreach (var layer in mapControl.Map.Layers)
            {
                if (layer.Name != "World" &&
                    layer.Name != "Water" &&
                    layer.Name != "Cities")
                {
                    layer.Enabled = false;
                }
            }

            // 3. WORLD
            var jsonWorld = File.ReadAllText("data/geojson/import/land.geojson", Encoding.UTF8);
            var providerWorld = new GeoJsonProvider(jsonWorld);

            var worldStyle = new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(Color.FromArgb(255, 10, 10, 20)),
                Outline = new Mapsui.Styles.Pen(Color.FromArgb(120, 80, 120, 200), 1)
            };

            var worldLayer = new Mapsui.Layers.Layer("World")
            {
                DataSource = providerWorld,
                Style = worldStyle
            };
            //WATER
            var jsonWater = File.ReadAllText("data/geojson/import/vatten.geojson", Encoding.UTF8);
            var providerWater = new GeoJsonProvider(jsonWater);

            var waterStyle = new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(Color.FromArgb(100, 10, 20, 20)),
                Outline = new Mapsui.Styles.Pen(Color.FromArgb(100, 50, 90, 200), 1)
            };

            var waterLayer = new Mapsui.Layers.Layer("Water")
            {
                DataSource = providerWater,
                Style = waterStyle
            };


            // 5. INSERT ORDER (VIKTIG DEL)

            // World längst ner
            mapControl.Map.Layers.Insert(1, waterLayer);

            //sen water
            mapControl.Map.Layers.Insert(1, worldLayer);


        }
        private void RemoveCinematicLayers()
        {
            if (_cinematicMode)
            {
                _cinematicMode = false;
                iconCanvas.Visibility = System.Windows.Visibility.Visible;
                locatorCanvas.Visibility = System.Windows.Visibility.Visible;
            }

            var names = new[] { "World", "Water", "Cities" };

            foreach (var layer in mapControl.Map.Layers
                .Where(l => names.Contains(l.Name))
                .ToList())
            {
                mapControl.Map.Layers.Remove(layer);
            }
        }
        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            var vp = mapControl.Map.Navigator.Viewport;
            mapControl.Map.Navigator.RotateTo(vp.Rotation - Math.PI / 12);
            UpdateNorthArrow();
        }

        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            var vp = mapControl.Map.Navigator.Viewport;
            mapControl.Map.Navigator.RotateTo(vp.Rotation + Math.PI / 12);
            UpdateNorthArrow();
        }
        private double CentrifugalAcceleration(double latitude)
        {
            const double omega = 7.2921159e-5;
            const double earthRadius = 6378137.0;

            double latRad = latitude * Math.PI / 180.0;

            double r = earthRadius * Math.Cos(latRad);

            return omega * omega * r;
        }
        private void UpdateFeatureTooltip(MPoint mouseWorld)
        {
            var vp = mapControl.Map.Navigator.Viewport;

            const double pixelRadius = 10;
            double tolerance = vp.Resolution * pixelRadius;

            var mousePos = Mouse.GetPosition(mapControl);

            var mouseScreen = Mouse.GetPosition(mapControl);


            var box = new MRect(
                mouseWorld.X - tolerance,
                mouseWorld.Y - tolerance,
                mouseWorld.X + tolerance,
                mouseWorld.Y + tolerance);

            Mapsui.IFeature nearestPointFeature = null;
            string nearestPointLayer = null;
            double nearestPointDist = double.MaxValue;
            Mapsui.IFeature smallestPolygon = null;
            string smallestPolygonLayer = null;
            double smallestArea = double.MaxValue;




            foreach (var layer in mapControl.Map.Layers)
            {
                if (!layer.Enabled)
                    continue;


                // Skippa lager vi aldrig vill hovra
                if (layer.Name == "BaseMap" ||
                    layer.Name == "World" ||
                    layer.Name == "Water")
                    continue;

                var features = layer.GetFeatures(box, vp.Resolution);
                int featureCount = features?.Count() ?? 0;
                var filteredFeatures = features.Where(f => f?.Extent != null &&
            box.Intersects(f.Extent)).ToList();
                foreach (var feature in filteredFeatures)
                {
                    if (feature?.Extent == null)
                        continue;

                    bool isPoint = false;


                    if (feature is Mapsui.Layers.PointFeature pointFeature || feature is NetTopologySuite.Geometries.Point || feature.Extent.Width == 0)
                    {
                        isPoint = true;
                    }

                    if (isPoint)
                    {
                        var center = feature.Extent.Centroid;

                        var screen = vp.WorldToScreen(center.X, center.Y);

                        double dx = screen.X - mouseScreen.X;
                        double dy = screen.Y - mouseScreen.Y;

                        double distSq = Math.Sqrt(dx * dx + dy * dy);
                        if (distSq < nearestPointDist && distSq < 20)
                        {
                            nearestPointDist = distSq;
                            nearestPointFeature = feature;
                            nearestPointLayer = layer.Name;

                        }
                    }
                    else
                    {
                        double area =
                         feature.Extent.Width *
                         feature.Extent.Height;

                        if (area < smallestArea)
                        {
                            smallestArea = area;
                            smallestPolygon = feature;
                            smallestPolygonLayer = layer.Name;
                        }
                    }
                }
            }

            // Prioritera alltid punktlager
            if (nearestPointFeature != null)
            {
                HoverInfoText.Text = $"● {nearestPointLayer}";
                HoverInfoText.Foreground = Brushes.LimeGreen;
                if (nearestPointLayer == "pdbdLayer")
                {
                    string species = "Okänd art";
                    if (nearestPointFeature["name"] != null)
                    {
                        species = " " + nearestPointFeature["name"].ToString();
                    }
                    HoverInfoText.Inlines.Clear();
                    HoverInfoText.Inlines.Add(new Run($"Point PbdbLayer ")
                    {
                        Foreground = Brushes.LimeGreen
                    });
                    if (!string.IsNullOrWhiteSpace(species))
                    {
                        HoverInfoText.Inlines.Add(new Run(species)
                        {
                            Foreground = Brushes.Orange,
                            FontWeight = FontWeights.SemiBold
                        });
                    }
                }
                if (nearestPointLayer == "clientPhotoLayer")
                {
                    string layer = "Okänd bildsamling";
                    if (nearestPointFeature["folder"] != null)
                    {
                        layer = " " + nearestPointFeature["folder"].ToString();
                    }
                    HoverInfoText.Inlines.Clear();
                    HoverInfoText.Inlines.Add(new Run($"Point Client Photo Layer ")
                    {
                        Foreground = Brushes.LimeGreen
                    });
                    if (!string.IsNullOrWhiteSpace(layer))
                    {
                        HoverInfoText.Inlines.Add(new Run(layer)
                        {
                            Foreground = Brushes.Silver,
                            FontWeight = FontWeights.SemiBold
                        });
                    }
                }
                if (nearestPointLayer == "Historiska positioner")
                {
                    string _event = "Okänd historisk data";
                    string src = "Okänd källa";
                    if (nearestPointFeature["Event"] != null)
                    {
                        _event = " " + nearestPointFeature["Event"].ToString();
                        src = "Källa: " + nearestPointFeature["Event"].ToString();
                    }
                    HoverInfoText.Inlines.Clear();
                    HoverInfoText.Inlines.Add(new Run($"Point Historical Layer ")
                    {
                        Foreground = Brushes.RosyBrown
                    });
                    if (!string.IsNullOrWhiteSpace(_event))
                    {
                        HoverInfoText.Inlines.Add(new Run(_event)
                        {
                            Foreground = Brushes.Silver,
                            FontWeight = FontWeights.SemiBold
                        });
                    }
                    var mousePos_ = Mouse.GetPosition(mapControl);
                    var mousePos_x = mousePos.X;
                    var mousePos_y = mousePos.Y;

                    HoverBorder.Margin = new System.Windows.Thickness(mousePos.X, mousePos.Y, 0, 0);
                    HoverBorder.ToolTip = new ToolTip
                    {
                        Background = Brushes.Black,
                        Foreground = Brushes.White,
                        Padding = new System.Windows.Thickness(10),
                        MaxWidth = 400,
                        MaxHeight = 200,
                        FontSize = 14,
                        Content = new TextBlock
                        {
                            Text = src,
                            TextWrapping = System.Windows.TextWrapping.Wrap,
                            Foreground = Brushes.White,
                            FontSize = 14
                        }
                    };

                    ToolTipService.SetPlacement(HoverBorder, System.Windows.Controls.Primitives.PlacementMode.Mouse);
                }
                if (nearestPointLayer == "etymologiLayer")
                {
                    string etym = nearestPointFeature["etym"]?.ToString() ?? "Ingen etymologi";
                    var mousePos_ = Mouse.GetPosition(mapControl);
                    var mousePos_x = mousePos.X;
                    var mousePos_y = mousePos.Y;

                    HoverBorder.Margin = new System.Windows.Thickness(mousePos.X, mousePos.Y, 0, 0);
                    HoverBorder.ToolTip = new ToolTip
                    {
                        Background = Brushes.Black,
                        Foreground = Brushes.White,
                        Padding = new System.Windows.Thickness(10),
                        MaxWidth = 400,
                        MaxHeight = 200,
                        FontSize = 14,
                        Content = new TextBlock
                        {
                            Text = etym,
                            TextWrapping = System.Windows.TextWrapping.Wrap,
                            Foreground = Brushes.White,
                            FontSize = 14
                        }
                    };

                    ToolTipService.SetPlacement(HoverBorder, System.Windows.Controls.Primitives.PlacementMode.Mouse);
                }
                if (nearestPointLayer == "tvLayer")
                {
                    string name = nearestPointFeature["serie"]?.ToString() ?? "Okänd plats";
                    string serie = nearestPointFeature["serie"]?.ToString() ?? "";
                    string year = nearestPointFeature["ar"]?.ToString() ?? "";
                    string plats = nearestPointFeature["namn"]?.ToString() ?? "";
                    string beskrivning = nearestPointFeature["beskrivning"]?.ToString() ?? "";
                    string text = $"Serie: {serie}\nÅr: {year}\nPlats: {name}" +
                        $"\nBeskrivning: {beskrivning}";
                    HoverInfoText.Inlines.Clear();
                    HoverInfoText.Inlines.Add(new Run("TV‑plats")
                    {
                        Foreground = Brushes.DeepSkyBlue,
                        FontWeight = FontWeights.Bold
                    });

                    HoverInfoText.Inlines.Add(new Run(" " + name)
                    {
                        Foreground = Brushes.White
                    });

                    HoverBorder.Margin = new Thickness(mousePos.X, mousePos.Y, 0, 0);
                    HoverBorder.ToolTip = new ToolTip
                    {
                        Background = Brushes.Black,
                        Foreground = Brushes.White,
                        Padding = new Thickness(10),
                        Content = new TextBlock
                        {
                            Text = text,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = Brushes.White
                        }
                    };

                    ToolTipService.SetPlacement(HoverBorder, System.Windows.Controls.Primitives.PlacementMode.Mouse);
                }
                if (nearestPointLayer == "filmLayer")
                {
                    string name = nearestPointFeature["film"]?.ToString() ?? "Okänd plats";
                    string film = nearestPointFeature["film"]?.ToString() ?? "";
                    string year = nearestPointFeature["ar"]?.ToString() ?? "";
                    string director = nearestPointFeature["regissor"]?.ToString() ?? "";
                    string plats = nearestPointFeature["namn"]?.ToString() ?? "";
                    string beskrivning = nearestPointFeature["beskrivning"]?.ToString() ?? "";
                    string text = $"Film: {film}\nÅr: {year}\nRegissör:{director}\nPlats: {name}" +
                        $"\nBeskrivning: {beskrivning}";

                    HoverInfoText.Inlines.Clear();
                    HoverInfoText.Inlines.Add(new Run("Filmplats")
                    {
                        Foreground = Brushes.Orange,
                        FontWeight = FontWeights.Bold
                    });

                    HoverInfoText.Inlines.Add(new Run(" " + name)
                    {
                        Foreground = Brushes.White
                    });

                    HoverBorder.Margin = new Thickness(mousePos.X, mousePos.Y, 0, 0);
                    HoverBorder.ToolTip = new ToolTip
                    {
                        Background = Brushes.Black,
                        Foreground = Brushes.White,
                        Padding = new Thickness(10),
                        Content = new TextBlock
                        {
                            Text = text,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = Brushes.White
                        }
                    };

                    ToolTipService.SetPlacement(HoverBorder, System.Windows.Controls.Primitives.PlacementMode.Mouse);
                }
                if (nearestPointLayer != "etymologiLayer" && nearestPointLayer != "Historiska positioner" && nearestPointLayer != "tvLayer")
                {
                    HoverBorder.ToolTip = null;
                }
                if (nearestPointLayer == "fyrLayer")
                {
                    string fyr = "Okänd fyr";
                    if (nearestPointFeature["name"] != null)
                    {
                        fyr = " " + nearestPointFeature["name"].ToString();
                        HoverInfoText.Inlines.Clear();
                        HoverInfoText.Inlines.Add(new Run($"Point FyrLayer ")
                        {
                            Foreground = Brushes.LimeGreen
                        });
                        if (!string.IsNullOrWhiteSpace(fyr))
                        {
                            HoverInfoText.Inlines.Add(new Run(fyr)
                            {
                                Foreground = Brushes.Gold,
                                FontWeight = FontWeights.SemiBold
                            });
                        }
                    }
                }
                if (nearestPointLayer == "bombLayer")
                {
                    string bomb = "Okänd bomb";
                    if (nearestPointFeature["name"] != null)
                    {
                        bomb = " " + nearestPointFeature["name"].ToString();
                        HoverInfoText.Inlines.Clear();
                        HoverInfoText.Inlines.Add(new Run($"Point BombLayer ")
                        {
                            Foreground = Brushes.LimeGreen
                        });
                        if (!string.IsNullOrWhiteSpace(bomb))
                        {
                            HoverInfoText.Inlines.Add(new Run(bomb)
                            {
                                Foreground = Brushes.Red,
                                FontWeight = FontWeights.SemiBold
                            });
                        }
                    }
                }
                if (nearestPointLayer == "golfLayer")
                {
                    string golf = "Okänd golfbana";
                    if (nearestPointFeature["name"] != null)
                    {
                        golf = " " + nearestPointFeature["name"].ToString();
                        HoverInfoText.Inlines.Clear();
                        HoverInfoText.Inlines.Add(new Run($"Point Golf Layer ")
                        {
                            Foreground = Brushes.LimeGreen
                        });
                        if (!string.IsNullOrWhiteSpace(golf))
                        {
                            HoverInfoText.Inlines.Add(new Run(golf)
                            {
                                Foreground = Brushes.Pink,
                                FontWeight = FontWeights.SemiBold
                            });
                        }
                    }
                }
                return;
            }

            if (smallestPolygon != null)
            {
                if (smallestPolygonLayer != "Sveriges Gräns")
                {
                    HoverInfoText.Text = $"▭ {smallestPolygonLayer}";
                }
                HoverInfoText.Foreground = Brushes.LightBlue;

                return;
            }
            else
            {
                HoverInfoText.Text = "";
            }
        }
        private void ToggleEditMode_Click(object sender, RoutedEventArgs e)
        {
            editMode = !editMode;
        }
        private void SaveClientLayer()
        {
            var fc = new FeatureCollection();

            foreach (var f in clientFeatures.OfType<GeometryFeature>())
            {
                var ntsFeature = new NetTopologySuite.Features.Feature
                {
                    Geometry = f.Geometry,              // GeometryFeature.Geometry är NTS-geometry
                    Attributes = new AttributesTable()  // tom tabell, kan fyllas om du vill
                };
                var keys = new[] { "Created", "Type", "Note" };

                foreach (var key in keys)
                {
                    ntsFeature.Attributes.Add(key, f[key]);
                }

                fc.Add(ntsFeature);
            }

            var writer = new GeoJsonWriter();
            var json = writer.Write(fc);

            Directory.CreateDirectory(Path.GetDirectoryName(ClientLayerPath)!);
            MessageBox.Show($"Saved to path: {ClientLayerPath}");
            File.WriteAllText(ClientLayerPath, json);
            Log.Info("ClientLayer", $"Saved features to clientLayer.geojson");
        }
        public Task<string> AskUserForNoteAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            var window = new System.Windows.Window
            {
                Title = "Anteckning",
                Width = 300,
                Height = 150,
                Content = new TextBox { Margin = new System.Windows.Thickness(10) }
            };

            var textBox = (TextBox)window.Content;

            window.Closed += (s, e) =>
            {
                tcs.TrySetResult(textBox.Text);
            };

            window.Show();

            return tcs.Task;
        }
        // ======================== ISTIDS-LAGER ========================

        // Hjälpfunktion för att skapa ett istidslager (för att slippa upprepa kod)
        private void AddIceLayer(string fileName, string layerName, Mapsui.Styles.Color baseColor, Action<Mapsui.Layers.Layer> setLayerProperty)
        {
            try
            {
                string path = $"data/geojson/import/istid/{fileName}";
                if (!File.Exists(path))
                {
                    Debug.WriteLine($"Varning: Filen saknas: {path}");
                    return;
                }

                var json = File.ReadAllText(path, Encoding.UTF8);
                var provider = new GeoJsonProvider(json);

                var fillColor = new Mapsui.Styles.Color(
            baseColor.R,
            baseColor.G,
            baseColor.B,
            90);

                var outlineColor = new Mapsui.Styles.Color(
                    baseColor.R,
                    baseColor.G,
                    baseColor.B,
                    180);

                var style = new VectorStyle
                {
                    Fill = new Mapsui.Styles.Brush(fillColor),
                    Outline = new Mapsui.Styles.Pen(outlineColor, 1.5),
                };

                var layer = new Mapsui.Layers.Layer(layerName)
                {
                    DataSource = provider,
                    Style = style,
                    Enabled = false
                };
                mapControl.Map.Layers.Add(layer);
                setLayerProperty?.Invoke(layer);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Fel vid inläsning av {fileName}: {ex.Message}");
            }
        }
        private void StartUserMarkerAnimation()
        {
            var anim = new DoubleAnimation
            {
                From = 1.0,
                To = 1.25,
                Duration = System.TimeSpan.FromSeconds(1.2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            PulseTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            PulseTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
        public class DmItem
        {
            public string Owner { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
            public string Thumbnail { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }
        }
        private async Task<List<string>> FetchDmOwnersAsync()
        {
            var url = "https://api.dimu.org/api/owners?country=se&api.key=demo";
            string xml;

            try
            {
                xml = await httpClient.GetStringAsync(url);
            }
            catch (System.Exception ex)
            {
                Log.Error("DigitalMuseum", $"HTTP error: {ex.Message}");
                return new List<string>();
            }

            if (!xml.TrimStart().StartsWith("<?xml"))
            {
                Log.Error("DigitalMuseum", $"Unexpected response: {xml.Substring(0, Math.Min(200, xml.Length))}");
                return new List<string>();
            }

            var owners = new List<string>();

            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);

                var nodes = doc.SelectNodes("//owner/identifier");
                if (nodes != null)
                {
                    foreach (System.Xml.XmlNode node in nodes)
                    {
                        owners.Add(node.InnerText);
                    }
                }

                Log.Info("DigitalMuseum", $"Fetched {owners.Count} owners from XML");
            }
            catch (System.Exception ex)
            {
                Log.Error("DigitalMuseum", $"XML parse error: {ex.Message}");
            }

            return owners;
        }

        private async Task FetchDmObjectsForOwnerAsync(string ownerId)
        {
            string url =
                $"https://api.dimu.org/api/solr/select?q=*:*&fq=identifier.owner:{ownerId}&wt=json&rows=500&api.key=demo";

            var json = await httpClient.GetStringAsync(url);
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            foreach (var doc in data.response.docs)
            {
                if (doc["artifact.coordinate"] == null)
                    continue;

                string coord = (string)doc["artifact.coordinate"];
                var parts = coord.Split(',');

                if (parts.Length != 2)
                    continue;

                if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
                    continue;
                if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    continue;

                var item = new DmItem
                {
                    Owner = ownerId,
                    Identifier = (string)doc["identifier.id"],
                    Title = (string)doc["artifact.ingress.title"] ?? "Okänd titel",
                    Thumbnail = doc["artifact.defaultMediaIdentifier"] != null
                        ? $"https://ems.dimu.org/image/{doc["artifact.defaultMediaIdentifier"]}?dimension=145x105"
                        : "data/ikoner/museum_placeholder.png",
                    Lat = lat,
                    Lon = lon
                };

                _dmItems.Add(item);
            }
            // Logga de första 5 koordinaterna för inspektion
            // Log.Info("DigitalMuseum", $"Owner {ownerId}: added {_dmItems.Count} items so far");
            if (_dmItems.Count > 0 && _dmItems.Count < 6)
            {
                //Log.Info("DigitalMuseum", $"Sample coordinates for owner {ownerId}:");
                foreach (var sample in _dmItems.Take(5))
                {
                    //  Log.Info("DigitalMuseum", $"  {sample.Title} → Lat={sample.Lat:F6}, Lon={sample.Lon:F6}");
                }
            }
        }
        private async Task LoadDigitalMuseumAsync()
        {
            if (_dmLoaded)
                return;

            Log.Info("DigitalMuseum", "Starting full DigitalMuseum load...");

            var owners = await FetchDmOwnersAsync();

            foreach (var owner in owners)
            {
                try
                {
                    await FetchDmObjectsForOwnerAsync(owner);
                }
                catch (System.Exception ex)
                {
                    Log.Error("DigitalMuseum", $"Owner {owner} failed: {ex.Message}");
                }
            }

            _dmLoaded = true;
            Log.Info("DigitalMuseum", $"Finished loading {_dmItems.Count} items with coordinates");
        }

        private void CloseMuseumBanner_Click(object sender, RoutedEventArgs e)
        {
            MuseumBanner.Visibility = System.Windows.Visibility.Collapsed;
            MuseumList.ItemsSource = null;

            Log.Info("DigitalMuseum", "Museum banner closed");
        }
        private void CloseSpecialOverlay_Click(object sender, RoutedEventArgs e)
        {
            SpecialDaysOverlay.Visibility = Visibility.Collapsed;
        }

        private List<DmItem> FindNearbyDmItems(double lon, double lat, double radiusKm)
        {
            var list = new List<(DmItem item, double dist)>();

            int iteration = 0;

            foreach (var item in _dmItems)
            {
                if (iteration == 0)
                {
                    //   Log.Info("[LogWindow]",$"{lon}, {lat}, {item.Lat}, {item.Lon}");
                    iteration++;
                }
                double d = Haversine(lat, lon, item.Lat, item.Lon);
                if (d <= radiusKm * 1000.0)
                    list.Add((item, d));
            }

            return list
                .OrderBy(t => t.dist)
                .Take(10)
                .Select(t => t.item)
                .ToList();
        }
        private async Task ShowNearbyMuseumItemsAsync(double lon, double lat, double radiusKm, string kommun)
        {
            await LoadDigitalMuseumAsync();

            var nearby = FindNearbyDmItems(lon, lat, radiusKm);

            if (nearby.Count < 5 && !string.IsNullOrEmpty(kommun))
            {
                Log.Info("DigitalMuseum", $"Too few coordinate hits ({nearby.Count}), running place search for '{kommun}'...");
                var placeHits = await FetchDmObjectsByPlaceAsync(kommun);
                nearby.AddRange(placeHits);
            }

            MuseumList.ItemsSource = nearby;
            MuseumBanner.Visibility = System.Windows.Visibility.Visible;

            Log.Info("DigitalMuseum", $"Showing {nearby.Count} items near {lon:F4},{lat:F4}");
        }
        private async Task<List<DmItem>> FetchDmObjectsByPlaceAsync(string kommun)
        {
            var results = new List<DmItem>();
            string encodedKommun = System.Uri.EscapeDataString(kommun);
            string url =
                $"https://api.dimu.org/api/solr/select?q=artifact.event.place:({encodedKommun})&wt=json&fq=artifact.hasPictures:true&rows=20&api.key=demo";

            try
            {
                var json = await httpClient.GetStringAsync(url);
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                foreach (var doc in data.response.docs)
                {
                    var item = new DmItem
                    {
                        Owner = (string)doc["identifier.owner"] ?? "",
                        Identifier = (string)doc["identifier.id"] ?? "",
                        Title = (string)doc["artifact.ingress.title"] ?? "Okänd titel",
                        Thumbnail = doc["artifact.defaultMediaIdentifier"] != null
                            ? $"https://ems.dimu.org/image/{doc["artifact.defaultMediaIdentifier"]}?dimension=145x105"
                            : "data/ikoner/museum_placeholder.png",
                        Lat = 0, // ingen koordinat
                        Lon = 0
                    };
                    results.Add(item);
                }

                Log.Info("DigitalMuseum", $"Place search '{encodedKommun}' returned {results.Count} items");
            }
            catch (System.Exception ex)
            {
                Log.Error("DigitalMuseum", $"Place search error for '{encodedKommun}': {ex.Message}");
            }

            return results;
        }
        public class MindatItem
        {
            public string MineralId { get; set; }
            public string Name { get; set; }
            public string Formula { get; set; }
            public string Locality { get; set; }
            public string ImageUrl { get; set; }     // Thumbnail om det finns
            public double Lat { get; set; }
            public double Lon { get; set; }
            public double DistanceMeters { get; set; }
        }
        private async Task ShowNearbyMindatItemsAsync(double lon, double lat, double radiusKm, string placeName = "")
        {
            await LoadMindatDataAsync();

            var nearby = FindNearbyMindatItems(lon, lat, radiusKm);

            if (nearby.Count == 0 && !string.IsNullOrEmpty(placeName))
            {
                // Fallback: sök på platsnamn (om du vill utöka senare)
                // Log.Info("Mindat", $"Inga träffar inom {radiusKm} km, fallback till platsnamn...");
            }

            // Visa banderoll (samma som MuseumBanner)
            MindatList.ItemsSource = nearby;
            MindatBanner.Visibility = System.Windows.Visibility.Visible;

            //Log.Info("Mindat", $"Visar {nearby.Count} mineral/fynd nära ({lat:F4}, {lon:F4})");
        }
        private async Task LoadMindatDataAsync()
        {
            if (_mindatLoaded)
            {
                Log.Info("Mindat", "Already loaded.");
                return;
            }

            try
            {
                Log.Info("Mindat", "Preparing request...");

                var request = new
                {
                    point = new
                    {
                        lat = 62.0,
                        lon = 15.0
                    },
                    distance = "800km",
                    description = "Swedish minerals"
                };

                var json = JsonConvert.SerializeObject(request);

                //  Log.Info("Mindat", $"Request JSON: {json}");

                var content =
                    new StringContent(json, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Token", "ApiKey");

                Log.Info("Mindat", "Authorization header added.");

                Log.Info("Mindat", "Sending request...");

                var response =
                    await httpClient.PostAsync(
                        "https://api.mindat.org/v1/geomin_point_search/",
                        content);

                Log.Info("Mindat",
                    $"HTTP {(int)response.StatusCode} {response.StatusCode}");

                var body =
                    await response.Content.ReadAsStringAsync();

                // Log.Info("Mindat", body);
                // Log.Info("Mindat", $"Authorization scheme: {httpClient.DefaultRequestHeaders.Authorization?.Scheme}");

                response.EnsureSuccessStatusCode();

                _mindatLoaded = true;

                Log.Info("Mindat", "Success.");
            }
            catch (System.Exception ex)
            {
                //  Log.Error("Mindat", ex.ToString());

                throw;
            }
        }
        private List<MindatItem> FindNearbyMindatItems(double lon, double lat, double radiusKm)
        {
            return _mindatItems
                .Where(item => Haversine(lat, lon, item.Lat, item.Lon) <= radiusKm * 1000)
                .OrderBy(item => item.DistanceMeters)
                .Take(12)                    // max antal i banderollen
                .ToList();
        }

        private void CloseMindatBanner_Click(object sender, RoutedEventArgs e)
        {
            MindatBanner.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void OpenDiscorChannel_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void OpenStoryTerra_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
        private async Task<string> LoadSpeciesInRadiusAsync(double lat, double lon, int radiusMeters = 100)
        {
            Log.Info("Artdatbanken API", $"inserts: lon{lon}, lat{lat}");

            //Goal find all species reports within 100m from client click


            var filter = new
            {
                geographics = new
                {
                    geometries = new[]
    {
        new
        {
            type = "Point",
            coordinates = new[] { lon, lat }
        }
    },

                    maxDistanceFromPoint = radiusMeters,
                    considerObservationAccuracy = false
                },

                date = new
                {
                    startDate = DateTime.UtcNow.AddYears(-10).ToString("yyyy-MM-dd"),
                    endDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    dateFilterType = "OnlyStartDate"
                },

                output = new
                {
                    take = 500,
                    include = new[]
                    {
            "point",
            "taxon",
            "event",
            "location"
        }
                }
            };

            var json = JsonConvert.SerializeObject(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

            try
            {
                var response = await httpClient.PostAsync(
                    "https://api.artdatabanken.se/species-observation-system/v1/observations/search",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    Log.Error("Artdataportalen", $"Fel {response.StatusCode}: {errorText}");
                    return "Fel vid hämtning";
                }

                var resultJson = await response.Content.ReadAsStringAsync();
                Log.Info("Artdatabanken", $"Status: {(int)response.StatusCode}");
                dynamic data = JsonConvert.DeserializeObject(resultJson);
                Log.Info("SOS", $"TotalCount = {data.totalCount}");
                Log.Info("SOS", $"Returned = {data.records.Count}");
                if (data.records != null && data.records.Count > 0)
                {
                    Log.Info("SOS first record",
    Newtonsoft.Json.JsonConvert.SerializeObject(
        data.records[0],
        Newtonsoft.Json.Formatting.Indented));
                }
                Log.Info("Artdatabanken API (\"SOS REQUEST", json);
                var observations = new List<SpeciesObservation>();
                Log.Info("SOS", $"Root properties:");
                foreach (var r in data.records)
                {
                    Log.Info("SOS",
                        $"{r.location?.decimalLatitude}, " +
                        $"{r.location?.decimalLongitude}, " +
                        $"accuracy={r.location?.coordinateUncertaintyInMeters}, " +
                        $"{r.datasetName}");
                }

                foreach (var p in ((JObject)data).Properties())
                {
                    Log.Info("SOS", p.Name);
                }
                foreach (var obs in data.records ?? Enumerable.Empty<dynamic>())
                {
                    var so = new SpeciesObservation
                    {
                        Id = (long?)obs.id ?? 0,
                        ScientificName = (string)obs.taxon?.scientificName ?? "Okänd",
                        CommonName =
    (string)obs.taxon?.vernacularName ?? "",
                        Lat = (double?)obs.location?.decimalLatitude ?? 0,
                        Lon = (double?)obs.location?.decimalLongitude ?? 0
                    };
                    observations.Add(so);
                }

                _currentSpeciesObservations = observations; // spara för lager senare

                if (observations.Count == 0)
                    return "Inga rapporterade arter inom 100 meter.";

                var speciesList = observations
                    .Select(o =>
                        string.IsNullOrWhiteSpace(o.CommonName)
                            ? o.ScientificName
                            : $"{o.CommonName} ({o.ScientificName})")
                    .OrderBy(s => s)
                    .ToList();

                return string.Join(", ", speciesList);
            }
            catch (System.Exception ex)
            {
                Log.Error("Artdataportalen", ex.Message);
                return "Fel: " + ex.Message;
            }
        }
        private void UpdateUiForMode()
        {
            bool maximized = WindowState == WindowState.Maximized;

            NavigationPad.Visibility =
                maximized
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed;

            Btn3D.Visibility =
                maximized
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed;

            NorthArrow.Visibility =
                maximized
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed;

            bool show3DControls = maximized && _is3D;

            BtnPitchUp.Visibility =
                show3DControls ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            BtnPitchDown.Visibility =
                show3DControls ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            BtnFlyRoute.Visibility =
    show3DControls ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            RouteSelector.Visibility =
    show3DControls ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            ScaleCanvas.Visibility =
                _is3D ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            overlayCanvas.Visibility =
                _is3D ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            iconCanvas.Visibility =
                _is3D ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            locatorCanvas.Visibility =
                _is3D ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void Window_StateChanged(object? sender, EventArgs e)
        {
            UpdateUiForMode();
        }
        private void PanMap(double dx, double dy)
        {
            var vp = mapControl.Map.Navigator.Viewport;

            var newCenter = new MPoint(
                vp.CenterX + dx,
                vp.CenterY + dy);

            mapControl.Map.Navigator.CenterOn(newCenter);
        }
        private async void BtnPitchUp_Click(object sender, RoutedEventArgs e)
        {
            await CesiumBrowser.EvaluateScriptAsync("increasePitch();");
        }

        private async void BtnPitchDown_Click(object sender, RoutedEventArgs e)
        {
            await CesiumBrowser.EvaluateScriptAsync("decreasePitch();");
        }
        private async void PanUp_Click(object sender, RoutedEventArgs e)
        {

            if (_is3D)
            {
                await CesiumBrowser.EvaluateScriptAsync("moveForward();");
                return;
            }

            double step = mapControl.Map.Navigator.Viewport.Resolution * 100;
            PanMap(0, step);
        }

        private async void PanDown_Click(object sender, RoutedEventArgs e)
        {

            if (_is3D)
            {
                await CesiumBrowser.EvaluateScriptAsync("moveBackward();");
                return;
            }

            double step = mapControl.Map.Navigator.Viewport.Resolution * 100;
            PanMap(0, -step);
        }

        private async void PanLeft_Click(object sender, RoutedEventArgs e)
        {

            if (_is3D)
            {
                await CesiumBrowser.EvaluateScriptAsync("moveLeft();");
                return;
            }

            double step = mapControl.Map.Navigator.Viewport.Resolution * 100;
            PanMap(-step, 0);
        }

        private async void PanRight_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("tmp", "Registered User Right Click");
            if (_is3D)
            {
                Log.Info("tmp", "Tryning to Call js to mov right");
                await CesiumBrowser.EvaluateScriptAsync("moveRight();");
                return;
            }

            double step = mapControl.Map.Navigator.Viewport.Resolution * 100;
            PanMap(step, 0);
        }
        private void Btn3D_Click(object sender, RoutedEventArgs e)
        {
            Toggle3D();
        }
        private async void Toggle3D()
        {
            if (!_is3D)
            {
                PauseRendering();

                mapControl.Visibility = System.Windows.Visibility.Collapsed;

                CesiumBrowser.Visibility = System.Windows.Visibility.Visible;

                _is3D = true;

                await StartCesium();

                await SyncMapsuiCameraToCesium();

                Log.Info("Ceisum/Mapsui Switch", "switched to Cesium");
            }
            else
            {
                CesiumBrowser.Visibility = System.Windows.Visibility.Collapsed;

                mapControl.Visibility = System.Windows.Visibility.Visible;

                ResumeRendering();

                _is3D = false;

                Log.Info("Ceisum/Mapsui Switch", "switched to Mapsui");
            }
            UpdateUiForMode();
        }

        private async Task StartCesium()
        {
            if (_cesiumLoaded)
                return;


            _server = new Server();

            await _server.Start();


            CesiumBrowser.Load(
                $"http://localhost:{_server.Port}/index.html"
            );

            await CesiumBrowser.WaitForInitialLoadAsync();

            CesiumBrowser.ShowDevTools();

            _cesiumLoaded = true;
        }
        private async Task SyncMapsuiCameraToCesium()
        {
            var viewport =
                mapControl.Map.Navigator.Viewport;

            double x = viewport.CenterX;
            double y = viewport.CenterY;

            var lonLat =
       SphericalMercator.ToLonLat(x, y);

            double lon = lonLat.lon;
            double lat = lonLat.lat;

            var vp = mapControl.Map.Navigator.Viewport;

            var extent = vp.ToExtent();
            if (extent == null) return;

            var swCorner = SphericalMercator.ToLonLat(
                    extent.MinX, extent.MinY);

            var seCorner = SphericalMercator.ToLonLat(
                extent.MaxX, extent.MinY);

            var nwCorner = SphericalMercator.ToLonLat(
                extent.MinX, extent.MaxY);

            double vh = Haversine(
    swCorner.lat, swCorner.lon,
    nwCorner.lat, nwCorner.lon);

            double vw = Haversine(
                swCorner.lat, swCorner.lon,
                seCorner.lat, seCorner.lon);
            const double sh = 0.20;
            const double sw = 0.30;
            const double f = 0.00852;

            if (vh <= 0 || vw <= 0 || sh <= 0 || sw <= 0)
            {
                return;
            }
            double height2 = 0.4 * (-4.0 + Math.Sqrt(16.0 * vw * vh - 112.0)) / 8.0;

            //TP
            // 2. Verklig markhöjd i meter
            double cesiumHeight = height2;
            double orientation = viewport.Rotation;


            string js =
            $@"
setCamera(
    {lon.ToString(CultureInfo.InvariantCulture)},
    {lat.ToString(CultureInfo.InvariantCulture)},
    {cesiumHeight.ToString(CultureInfo.InvariantCulture)},
    {orientation.ToString(CultureInfo.InvariantCulture)}
);
";


            await CesiumBrowser.EvaluateScriptAsync(js);
        }
        private async void BtnFlyRoute_Click(object sender, RoutedEventArgs e)
        {
            //combobox switch
            string file =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Cesium",
                    "Routes",
                    "routes",
                    "debug-route.geojson"
                );

            string json = File.ReadAllText(file);

            await SendRouteToCesium(json);
        }
        private async Task SendRouteToCesium(string geoJson)
        {
            using JsonDocument doc =
                JsonDocument.Parse(geoJson);


            var coords =
                new List<double[]>();


            var geometry =
                doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("geometry");


            string type =
                geometry.GetProperty("type").GetString();


            JsonElement points;


            if (type == "LineString")
            {
                points = geometry.GetProperty("coordinates");
            }
            else
            {
                points =
                geometry
                .GetProperty("coordinates")[0];
            }


            foreach (var p in points.EnumerateArray())
            {
                coords.Add(new[]
                {
            p[0].GetDouble(),
            p[1].GetDouble()
        });
            }


            string routeJson =
                System.Text.Json.JsonSerializer.Serialize(coords);


            await CesiumBrowser.EvaluateScriptAsync(
                $"flyRoute({routeJson});"
            );
        }
        //Feature.Visible
        private void UpdateNorthArrow()
        {
            if (NorthArrow == null) return;

            var vp = mapControl.Map.Navigator.Viewport;
            double rotationDegrees = vp.Rotation; // Negativt för att pilen ska peka rätt


            NorthArrowRotation.Angle = rotationDegrees;
        }
        private async Task AddWmsLayerAsync()
        {
            Log.Info("WMS", "called");
            try
            {

                heatProvider = await WmsProvider.CreateAsync(
                   "https://gisapp.msb.se/arcgis/services/Varmekarteringar/Maxtemperatur_2023_2025/MapServer/WMSServer");


                heatProvider.CRS = "EPSG:3857";

                heatProvider.AddLayer("0");
                heatProvider.AddStyle("default");


                var url = heatProvider.GetRequestUrl(
                    heatProvider.GetExtent(),
                    800,
                    600);

                Log.Info("WMS URL", url);

                foreach (var p in heatProvider.GetType().GetProperties())
                {
                    Log.Info("WMS", p.Name);
                }

                heatLayer = new ImageLayer("Värmekarta")
                {
                    DataSource = heatProvider
                };
                Log.Info("WMS", "crated layer");

                mapControl.Map.Layers.Add(heatLayer);
                heatLayer.Enabled = false;

            }
            catch (System.Exception ex_)
            {
                Log.Error("WMS", $"{ex_}");
            }
            try
            {
                fireClassProvider = await WmsProvider.CreateAsync(
    "https://gis-tjanster.mcf.se/arcgis/services/Brandbransleklassificering/Skogsmark/MapServer/WMSServer");

                fireClassProvider.CRS = "EPSG:3857";
                fireClassProvider.AddLayer("0");
                fireClassProvider.AddStyle("default");


                var url = fireClassProvider.GetRequestUrl(
    fireClassProvider.GetExtent(),
    800,
    600);

                Log.Info("WMS URL", url);

                foreach (var p in fireClassProvider.GetType().GetProperties())
                {
                    Log.Info("WMS", p.Name);
                }


                fireClassLayer = new ImageLayer("Brandbränsleklassificering")
                {
                    DataSource = fireClassProvider,
                    Enabled = false
                };

                mapControl.Map.Layers.Add(fireClassLayer);

            }
            catch (System.Exception _ex)
            {
                Log.Error("WMS", $"{_ex}");
            }
            mapControl.Refresh();
        }
        private void CloseBrandLegend_Click(object sender, RoutedEventArgs e)
        {
            BrandLegend.Visibility = System.Windows.Visibility.Collapsed;
        }
        string FormatDistance(double meters)
        {
            if (meters < 1000)
                return $"{meters:F0} m";
            else
                return $"{meters / 1000.0:F2} km";
        }
        private void InitGpuCounters()
        {
            if (gpuInitialized) return;

            gpuCounters.Clear();
            int pid = Environment.ProcessId;

            try
            {
                var category = new PerformanceCounterCategory("GPU Engine");
                string[] instances = category.GetInstanceNames();

                foreach (string instance in instances)
                {
                    if (instance.Contains($"pid_{pid}_") &&
                        instance.Contains("engtype_3D"))
                    {
                        var counter = new PerformanceCounter(
                            "GPU Engine",
                            "Utilization Percentage",
                            instance);

                        counter.NextValue(); // måste anropas en gång först
                        gpuCounters.Add(counter);
                    }
                }
            }
            catch
            {
                // Kategorin finns inte eller saknar rättigheter
            }

            gpuInitialized = true;
        }
        private float GetGpuUsage()
        {
            if (!gpuInitialized || gpuCounters.Count == 0)
                return -1; // signalerar att det inte finns data

            float total = 0;
            foreach (var counter in gpuCounters)
            {
                try
                {
                    total += counter.NextValue();
                }
                catch { }
            }

            return Math.Min(total, 100f);
        }
        int GetZoomLevel()
        {
            double current = mapControl.Map.Navigator.Viewport.Resolution;
            var list = mapControl.Map.Navigator.Resolutions;

            double bestDiff = double.MaxValue;
            int bestIndex = -1;

            for (int i = 0; i < list.Count; i++)
            {
                double diff = Math.Abs(list[i] - current);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        public void Init()
        {
            ConfigureWindow();
            InitGpuCounters();
            InitDatabase();
            ShowDailyOverlay();
            OtherSpecialDays();
            StartUserMarkerAnimation();
            InitBackgroundMap();
            InitializeHistoricalLayer();
        }
        private void ConfigureWindow()
        {
            ShowInTaskbar = false;
            WindowState = WindowState.Minimized;
            StateChanged += Window_StateChanged;
        }
        private void InitDatabase()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "GeoViewSE");
            string _dbPath = Path.Combine(folder, "timeseries.db");


            if (File.Exists(_dbPath))
            {
                rows = TimeSeriesReader.ReadAll()
    .ConvertAll(dict => dict.ToDictionary(
    kvp => kvp.Key,
    kvp => (object?)kvp.Value
    ));
            }
            else
            {
                //rows = new List<Dictionary<string, object?>>();
            }
        }
        private void FlytoHome(object sender, RoutedEventArgs e)
        {
            var config = ConfigLoader.Load();
            try
            {
                double lat = config.homePosLat;
                double lon = config.homePosLon;

                var (x, y) = SphericalMercator.FromLonLat(lon, lat);
                var p = new Mapsui.MPoint(x, y);
                mapControl.Map.Navigator.FlyTo(p, maxResolution: 5000, duration: 800);
            }
            catch (System.Exception ex)
            {
                Log.Error("Config", $"No float assigned values for homePosLat or homePosLon - Skipping fly to home Execution. Error msg: {ex.Message}");
            }

        }
        private readonly Dictionary<string, string> OrtNamnselement =
            new Dictionary<string, string>
            {
        { "ryd", "hygge" },
        { "ås", "berg-/sand-/rullstensås" },
        { "hammar", "bergknall" }
            };
        public void ShowOrtNamnselement()
        {
            OrtNamnselementList.Items.Clear();

            foreach (var item in OrtNamnselement)
            {
                var panel = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Margin = new Thickness(0, 4, 0, 4)
                };

                var name = new TextBlock
                {
                    Text = item.Key,
                    FontWeight = FontWeights.Bold,
                    FontSize = 17,
                    Width = 100
                };

                var description = new TextBlock
                {
                    Text = item.Value,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap
                };

                panel.Children.Add(name);
                panel.Children.Add(description);

                OrtNamnselementList.Items.Add(panel);
            }

            OrtNamnselementOverlay.Visibility = Visibility.Visible;
        }
        private void UpdateMiniMapViewportBox()
        {
            var vp = mapControl.Map.Navigator.Viewport;
            var miniVp = minimap.Map.Navigator.Viewport;

            var bbox = vp.ToExtent();

            // Hörn i Mercator
            var sw = new MPoint(bbox.MinX, bbox.MinY);
            var nw = new MPoint(bbox.MinX, bbox.MaxY);
            var ne = new MPoint(bbox.MaxX, bbox.MaxY);
            var se = new MPoint(bbox.MaxX, bbox.MinY);


            // Polygon
            var feature = new GeometryFeature
            {
                Geometry = new NetTopologySuite.Geometries.Polygon(
                    new LinearRing(new[]
                    {
            new Coordinate(bbox.MinX, bbox.MinY),
            new Coordinate(bbox.MinX, bbox.MaxY),
            new Coordinate(bbox.MaxX, bbox.MaxY),
            new Coordinate(bbox.MaxX, bbox.MinY),
            new Coordinate(bbox.MinX, bbox.MinY)
                    }))
            };

            // Semi-transparent stil
            feature.Styles.Add(new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(Color.FromArgb(80, 255, 255, 0)), // gul, 30% opacity
                Outline = new Mapsui.Styles.Pen(Color.FromArgb(200, 255, 255, 0), 2)
            });

            _minimapViewportLayer.Features = new[] { feature };
            _minimapViewportLayer.DataHasChanged();

            minimap.Refresh();
        }
        private static readonly Dictionary<string, string> hashtagCounts =
    new(StringComparer.OrdinalIgnoreCase)
    {
        ["Askersund"] = "48K",
        ["Avesta"] = "108K",
        ["Boden"] = "464K",
        ["Bollnäs"] = "97.8K",
        ["Borgholm"] = "80.4K",
        ["Borlänge"] = "213K",
        ["Borås"] = "514K",
        ["Broo"] = "173K",
        ["Båstad"] = "224K",
        ["Djursholm"] = "27.4K",
        ["Eksjö"] = "81.8K",
        ["Enköping"] = "125K",
        ["Eskilstuna"] = "480K",
        ["Eslöv"] = "72.3K",
        ["Fagersta"] = "52.7K",
        ["Falkenberg"] = "267K",
        ["Falköping"] = "91K",
        ["Falsterbo"] = "96.7K",
        ["Falun"] = "346K",
        ["Filipstad"] = "36K",
        ["Flen"] = "36.5K",
        ["Gränna"] = "86.1K",
        ["Gävle"] = "473K",
        ["Hagfors"] = "17.9K",
        ["Haparanda"] = "81.8K",
        ["Hedemora"] = "66.7K",
        ["Helsingborg"] = "1M",
        ["Hjo"] = "84.6K",
        ["Hudiksvall"] = "127K",
        ["Huskvarna"] = "103K",
        ["Härnösand"] = "129K",
        ["Hässleholm"] = "85.5K",
        ["Höganäs"] = "153K",
        ["Jönköping"] = "617K",
        ["Kalmar"] = "534K",
        ["Karlshamn"] = "104K",
        ["Karlskoga"] = "115K",
        ["Karlstad"] = "577K",
        ["Katrineholm"] = "124K",
        ["Kiruna"] = "318K",
        ["Kramfors"] = "48.7K",
        ["Kristianopel"] = "5K",
        ["Kristianstad"] = "319K",
        ["Kristinehamn"] = "87.1K",
        ["Kumla"] = "122K",
        ["Kungsbacka"] = "210K",
        ["Kungälv"] = "151K",
        ["Köping"] = "93.7K",
        ["Laholm"] = "83.5K",
        ["Landskrona"] = "215K",
        ["Lidingö"] = "171K",
        ["Lidköping"] = "190K",
        ["Ljungby"] = "80.9K",
        ["Lomma"] = "89.8K",
        ["Ludvika"] = "105K",
        ["Luleå"] = "472K",
        ["Lund"] = "777K",
        ["Lycksele"] = "57.2K",
        ["Lysekil"] = "119K",
        ["Mariefred"] = "99.5K",
        ["Mariestad"] = "114K",
        ["Marstrand"] = "110K",
        ["Mjölby"] = "58.9K",
        ["Motala"] = "180K",
        ["Mölndal"] = "115K",
        ["Mönsterås"] = "41K",
        ["Nacka"] = "207K",
        ["Norrköping"] = "638K",
        ["Norrtälje"] = "200K",
        ["Nybro"] = "56.8K",
        ["Nyköping"] = "245K",
        ["Nynäshamn"] = "125K",
        ["Nässjö"] = "74.8K",
        ["Oskarshamn"] = "173K",
        ["Oxelösund"] = "52.7K",
        ["Piteå"] = "214K",
        ["Ronneby"] = "81K",
        ["Sandviken"] = "134K",
        ["Sigtuna"] = "167K",
        ["Simrishamn"] = "98.7K",
        ["Skanör"] = "79.6K",
        ["Skara"] = "91.6K",
        ["Skellefteå"] = "287K",
        ["Skänninge"] = "13.5K",
        ["Skövde"] = "264K",
        ["Sollefteå"] = "95.1K",
        ["Solna"] = "283K",
        ["Strängnäs"] = "136K",
        ["Strömstad"] = "152K",
        ["Sundbyberg"] = "199K",
        ["Sundsvall"] = "534K",
        ["Säffle"] = "46.9K",
        ["Säter"] = "45.9K",
        ["Sävsjö"] = "21.5K",
        ["Söderhamn"] = "99K",
        ["Söderköping"] = "80.6K",
        ["Södertälje"] = "283K",
        ["Sölvesborg"] = "60K",
        ["Tidaholm"] = "63.6K",
        ["Torshälla"] = "30.8K",
        ["Tranås"] = "49.3K",
        ["Trelleborg"] = "171K",
        ["Trollhättan"] = "267K",
        ["Uddevalla"] = "265K",
        ["Ulricehamn"] = "97.8K",
        ["Umeå"] = "732K",
        ["Uppsala"] = "1.3M",
        ["Vadstena"] = "102K",
        ["Varberg"] = "470K",
        ["Vaxholm"] = "126K",
        ["Vetlanda"] = "73.1K",
        ["Vimmerby"] = "77.3K",
        ["Visby"] = "477K",
        ["Vänersborg"] = "118K",
        ["Värnamo"] = "92.1K",
        ["Västervik"] = "161K",
        ["Västerås"] = "711K",
        ["Växjö"] = "372K",
        ["Ystad"] = "292K",
        ["Åhus"] = "118K",
        ["Åmål"] = "58.1K",
        ["Ängelholm"] = "213K",
        ["Örebro"] = "806K",
        ["Öregrund"] = "68.6K",
        ["Örnsköldsvik"] = "190K",
        ["Östersund"] = "337K",
        ["Östhammar"] = "47.9K"
    };

        private static string? GetHashtagCount(string namn)
        {
            if (string.IsNullOrWhiteSpace(namn))
                return null;

            return hashtagCounts.TryGetValue(namn.Trim(), out var count)
                ? count
                : null;
        }
        public HomoSapienNameData GetHumanNameForLan(string countyName)
        {
            return environmentalDataHomoSapinesName
                .FirstOrDefault(x => x.Lan.Equals(countyName, StringComparison.OrdinalIgnoreCase))
                ?? new HomoSapienNameData
                {
                    Lan = countyName,
                    Flicknamn = "Okänd",
                    Pojknamn = "Okänd"
                };
        }
        public class HomoSapienNameData
        {
            public string Lan { get; set; }
            public string Flicknamn { get; set; }
            public string Pojknamn { get; set; }

        }
        private static readonly List<HomoSapienNameData> environmentalDataHomoSapinesName = new List<HomoSapienNameData>
{
    new HomoSapienNameData { Lan = "Stockholm",        Flicknamn = "Olivia",              Pojknamn = "Oliver" },
    new HomoSapienNameData { Lan = "Uppsala",           Flicknamn = "Alice / Maja",        Pojknamn = "Hugo" },
    new HomoSapienNameData { Lan = "Södermanland",      Flicknamn = "Elsa / Maja",         Pojknamn = "Noah" },
    new HomoSapienNameData { Lan = "Östergötland",      Flicknamn = "Elsa",                Pojknamn = "Alfred / Hugo" },
    new HomoSapienNameData { Lan = "Jönköping",         Flicknamn = "Elsa / Signe",        Pojknamn = "Adam" },
    new HomoSapienNameData { Lan = "Kronoberg",         Flicknamn = "Saga",                Pojknamn = "Olle" },
    new HomoSapienNameData { Lan = "Kalmar",            Flicknamn = "Ellie",               Pojknamn = "Alfred" },
    new HomoSapienNameData { Lan = "Gotland",           Flicknamn = "Olivia",              Pojknamn = "Nils" },
    new HomoSapienNameData { Lan = "Blekinge",          Flicknamn = "Alma / Ellie / Elsa", Pojknamn = "Alfred" },
    new HomoSapienNameData { Lan = "Skåne",             Flicknamn = "Ellie",               Pojknamn = "Noah" },
    new HomoSapienNameData { Lan = "Halland",           Flicknamn = "Alma",                Pojknamn = "Noah" },
    new HomoSapienNameData { Lan = "Västra Götaland",   Flicknamn = "Vera",                Pojknamn = "Noah" },
    new HomoSapienNameData { Lan = "Värmland",          Flicknamn = "Alma / Selma",        Pojknamn = "William" },
    new HomoSapienNameData { Lan = "Örebro",            Flicknamn = "Vera",                Pojknamn = "William" },
    new HomoSapienNameData { Lan = "Västmanland",       Flicknamn = "Elsa",                Pojknamn = "Hugo" },
    new HomoSapienNameData { Lan = "Dalarna",           Flicknamn = "Alma",                Pojknamn = "Nils" },
    new HomoSapienNameData { Lan = "Gävleborg",         Flicknamn = "Elsa",                Pojknamn = "Nils" },
    new HomoSapienNameData { Lan = "Västernorrland",    Flicknamn = "Signe",               Pojknamn = "Hugo / Nils" },
    new HomoSapienNameData { Lan = "Jämtland",          Flicknamn = "Freja / Selma",       Pojknamn = "Nils" },
    new HomoSapienNameData { Lan = "Västerbotten",      Flicknamn = "Vera",                Pojknamn = "Alfred" },
    new HomoSapienNameData { Lan = "Norrbotten",        Flicknamn = "Alma",                Pojknamn = "Alfred" },
};

        private void InitializeHistoricalLayer()
        {
            try
            {
                _historicalLayer = new HistoricalPositionLayer();
                _historicalLayer.LoadFromFile("data/geojson/historia/historicalPositionsPointLayer.geojson");

                Log.Info("Historical layer",
                    $"Laddade punkter, datumintervall {_historicalLayer.MinDate:yyyy-MM-dd} till {_historicalLayer.MaxDate:yyyy-MM-dd}");

                mapControl.Map.Layers.Add(_historicalLayer.MapLayer);


                TimeSlider.SetRange(_historicalLayer.MinDate, _historicalLayer.MaxDate);
                TimeShuttle.SetInitialDate(TimeSlider.CurrentDate);


                _historicalLayer.UpdateForDate(TimeSlider.CurrentDate);


                if (_historicalLayer.MapLayer.Extent != null)
                {
                    Log.Info("slider", "Layer has extent");
                    if(_historicalLayer.MapLayer.Features == null)
                    {
                        Log.Info("slider", "historicalLayer.Maplayer has features");

                    }
                }
                else { Log.Info("slider", "Layer has no extent"); }
            }
            catch (System.Exception ex)
            {
                Log.Error("slider", $"Kunde inte ladda historiskt lager\n{ex}");
            }

            TimeSlider.DateChanged += date =>
            {
                _historicalLayer.UpdateForDate(date);
                TimeShuttle.SetInitialDate(date);
                mapControl.Map.RefreshData();
                mapControl.Refresh();
            };
            TimeShuttle.DateChanged += date =>
            {
                _historicalLayer.UpdateForDate(date);
                TimeSlider.SetPosition(date);
            };

        }
        private void InitBackgroundMap() {
            mapControl.Map = new Map();
            var osmLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
            osmLayer.Name = "BaseMap";
            mapControl.Map.Layers.Add(osmLayer);
        }
    }
}
