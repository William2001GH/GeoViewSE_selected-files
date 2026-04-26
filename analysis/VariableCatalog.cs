using System;
using System.Collections.Generic;
using System.Text;
using static GeoViewSE_Linnaeus.analysis.AnalysisiEngine;

namespace GeoViewSE_Linnaeus.Analysis
{
    public static class VariableCatalog
    {
        public static readonly List<AnalysisEngine.VariableMeta> Variables =
            new List<AnalysisEngine.VariableMeta>
            {
                // --- WEATHER (OpenMeteo + SMHI) ---
                new AnalysisEngine.VariableMeta { Name = "Temperature", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "Wind", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "Humidity", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "Aqi", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "Pm25", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "Pm10", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "O3", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "UvMaxToday", Kind = AnalysisEngine.VariableKind.Continuous },

                // --- SMHI WEATHER ---
                new AnalysisEngine.VariableMeta { Name = "SmhiWeatherTemp", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "DistSmhiWeather" },

                // --- SMHI SEA ---
                new AnalysisEngine.VariableMeta { Name = "SeaTemperature", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "DistSeaTemp" },
                new AnalysisEngine.VariableMeta { Name = "SeaSalinity", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "DistSeaSalinity" },

                // --- POLLEN (Pollenkollen) ---
                new AnalysisEngine.VariableMeta { Name = "PollenBirch", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },
                new AnalysisEngine.VariableMeta { Name = "PollenGrass", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },
                new AnalysisEngine.VariableMeta { Name = "PollenMugwort", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },
                new AnalysisEngine.VariableMeta { Name = "PollenAlder", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },
                new AnalysisEngine.VariableMeta { Name = "PollenHazel", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },
                new AnalysisEngine.VariableMeta { Name = "PollenWillow", Kind = AnalysisEngine.VariableKind.Continuous, DistancePartner = "PollenStationDistKm" },

                // --- HYDROLOGY ---
                new AnalysisEngine.VariableMeta { Name = "DistRiver", Kind = AnalysisEngine.VariableKind.Distance },
                new AnalysisEngine.VariableMeta { Name = "DirRiver", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "DistCoast", Kind = AnalysisEngine.VariableKind.Distance },
                new AnalysisEngine.VariableMeta { Name = "DirCoast", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "IsGrundvattenMagasin", Kind = AnalysisEngine.VariableKind.Boolean },

                // --- SUBBASINS ---
                new AnalysisEngine.VariableMeta { Name = "IsInSubbasin", Kind = AnalysisEngine.VariableKind.Boolean },

                // --- SOIL DEPTH ---
                new AnalysisEngine.VariableMeta { Name = "SoilDepthMeters", Kind = AnalysisEngine.VariableKind.Continuous },
                new AnalysisEngine.VariableMeta { Name = "DistSoilDepth", Kind = AnalysisEngine.VariableKind.Distance },

                // --- RIDGES ---
                new AnalysisEngine.VariableMeta { Name = "DistRidge", Kind = AnalysisEngine.VariableKind.Distance },
                new AnalysisEngine.VariableMeta { Name = "RidgeDirection", Kind = AnalysisEngine.VariableKind.Continuous },

                // --- POWER INFRA ---
                new AnalysisEngine.VariableMeta { Name = "DistPowerTower", Kind = AnalysisEngine.VariableKind.Distance },
                new AnalysisEngine.VariableMeta { Name = "DistPowerCable", Kind = AnalysisEngine.VariableKind.Distance },
                new AnalysisEngine.VariableMeta { Name = "DistFireStation", Kind = AnalysisEngine.VariableKind.Distance },

                // --- GEOLOGY (boolean lithology) ---
                new AnalysisEngine.VariableMeta { Name = "IsGranit", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsDiabas", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsBasalt", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsAmfibolit", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsSandsten", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsKonglomerat", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsLerskiffer", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsKalksten", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsLera", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsKol", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsGnejs", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsPegmatit", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsSkiffer", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsKvartsit", Kind = AnalysisEngine.VariableKind.Boolean },

                // --- SOIL TYPES ---
                new AnalysisEngine.VariableMeta { Name = "IsBerg", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsSilt", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsSand", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsGrus", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsTorv", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsMoran", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsIsalv", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsVittringsjord", Kind = AnalysisEngine.VariableKind.Boolean },
                new AnalysisEngine.VariableMeta { Name = "IsMoranlera", Kind = AnalysisEngine.VariableKind.Boolean },

                // --- GEOCHEMISTRY ---
                new AnalysisEngine.VariableMeta { Name = "Al2O3", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "AsPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "BaO", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "CaO", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "ClPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "CoPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "CrPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "CuPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "Fe2O3", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "K2O", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "MgO", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "MnO", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "MoPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "Na2O", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "NiPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "P2O5", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "PbPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "RbPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "SPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "SiO2", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "SrPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "TiO2", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "VPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "ZnPpm", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "ZrPpm", Kind = AnalysisEngine.VariableKind.Constant },

                // --- HARVEST ---
                new AnalysisEngine.VariableMeta { Name = "HarvestHostvete", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestVarvete", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestRag", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestHostkorn", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestVarkorn", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestHavre", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestArter", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestAkerbonor", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestMatpotatis", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestPotatisStarkelse", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestSockerbetor", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestHostraps", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestVarraps", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestSlattervallTotal", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestSlattervallForsta", Kind = AnalysisEngine.VariableKind.Constant },
                new AnalysisEngine.VariableMeta { Name = "HarvestSlattervallAttervaxt", Kind = AnalysisEngine.VariableKind.Constant },
            };
    }
}

