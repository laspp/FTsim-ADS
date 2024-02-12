using System;
using System.IO;
using Newtonsoft.Json;

public class ConfigFileManager
{
    public static AppConfig LoadConfig(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException($"Invalid or missing config file.");
        }

        try
        {
            string jsonText = File.ReadAllText(filePath);
            
            AppConfig appConfig = JsonConvert.DeserializeObject<AppConfig>(jsonText);

            return appConfig;
        }
        catch (Exception e)
        {
            throw new Exception($"Error reading config file: {e.Message}");
        }
    }

    public static void SaveConfig(string filePath, AppConfig appConfig)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException($"Invalid or missing config file.");
        }

        try
        {
            string jsonText = JsonConvert.SerializeObject(appConfig, Formatting.Indented);
            File.WriteAllText(filePath, jsonText);                        
        }
        catch (Exception e)
        {
            throw new Exception($"Error writing config file: {e.Message}");
        }
    }
}
