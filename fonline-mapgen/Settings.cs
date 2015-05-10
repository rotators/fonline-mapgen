using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;

public class MapperSettings
{
    public PathSettings Paths { get; set; }
    public UISettings UI { get; set; }
    public ViewSettings View { get; set; }
    public PerformanceSettings Performance { get; set; }
    public bool GLMode { get; set;}

    public MapperSettings()
    {
        Paths = new PathSettings();
        UI = new UISettings();
        View = new ViewSettings();
        Performance = new PerformanceSettings();

        Paths.DataDirs = new List<string>();
        Paths.DataFiles = new List<string>();
    }
}

public class PathSettings
{
    public string BaseDir { get; set; }
    public string MapsDir { get; set; }
    public string CritterTypes { get; set; }
    public string FOOBJ { get; set; }
    public string CritterProtos { get; set; }
    public string ItemProtos { get; set; }
    public List<String> DataFiles { get; set; }
    public List<String> DataDirs { get; set; }
}

public class UISettings
{
    public bool LoadLastMap { get; set; }
    public bool Debug { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public OverlaySettings Overlay { get; set; }
}

public class OverlaySettings
{
    public string CritterFormat { get; set; }
    public string SceneryFormat { get; set; }
}

public class PerformanceSettings
{
    public bool CacheResources { get; set; }
    public bool FastRendering  { get; set; }
}

public class ViewSettings
{
    public bool Tiles { get; set; }
    public bool Critters { get; set; }
    public bool Walls { get; set; }
    public bool Roofs { get; set; }
    public bool Scenery { get; set; }
    public bool Items { get; set; }
}

internal static class SettingsManager
{
    public static Logger logger;
    public static readonly string SettingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");

    public static void Init()
    {
        logger = LogManager.GetCurrentClassLogger();
    }

    public static MapperSettings LoadSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            logger.Error("Unable to find settings on {0}", SettingsPath);
            return null;
        }

        logger.Info("Loading settings from {0}", SettingsPath);
        string jsonSettings = File.ReadAllText(SettingsPath);
        logger.Info("Read settings file.");
        var settings = JsonConvert.DeserializeObject<MapperSettings>(jsonSettings);
        logger.Info("Deserialized settings.");
        return settings;
    }

    public static void SaveSettings(MapperSettings settings)
    {
        logger.Info("Serializing settings.");
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        logger.Info("Saving settings to {0}.", SettingsPath);
        File.WriteAllText(SettingsPath, json);
    }
}