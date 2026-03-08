using System.Text.Json;
using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Services;

public class ConfigService : IConfigService
{
    //home/user/.local/share/Shelly
    private static readonly string ConfigFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Shelly");

    private static readonly string ConfigPath = Path.Combine(ConfigFolder, "settings.json");

    private ShellyConfig? _config = null;

    public event EventHandler<ShellyConfig>? ConfigSaved;

    public void SaveConfig(ShellyConfig config)
    {
        if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

        _config = config;
        var json = JsonSerializer.Serialize(config, ShellyGtkJsonContext.Default.ShellyConfig);
        File.WriteAllText(ConfigPath, json);
        ConfigSaved?.Invoke(this, config);
    }

    public ShellyConfig LoadConfig()
    {
        try
        {
            if (_config != null)
            {
                return _config;
            }

            if (!File.Exists(ConfigPath)) return new ShellyConfig();
            var json = File.ReadAllText(ConfigPath);
            Console.WriteLine(ConfigPath);
            _config = JsonSerializer.Deserialize(json, ShellyGtkJsonContext.Default.ShellyConfig) ?? new ShellyConfig();
            return _config;
        }
        catch
        {
            return new ShellyConfig();
        }
    }
}