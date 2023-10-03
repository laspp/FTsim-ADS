using System;
using UnityEngine;
using Newtonsoft.Json;

public class ConfigLoader2
{
    public static AppConfig LoadConfig(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException($"Invalid or missing config file.");
        }

        try
        {

            string path = Application.streamingAssetsPath + filename;

            TextAsset jsonText = Resources.Load(path) as TextAsset;

            AppConfig appConfig = JsonConvert.DeserializeObject<AppConfig>(jsonText.text);

            // Assign the settings from the loaded AppConfig object
            Debug.Log($"AppConfig: {appConfig.InputVariableMap["ToggleSwitch"]}");

            return appConfig;
        }
        catch (Exception e)
        {
            throw new Exception($"Error reading config file: {e.Message}");
        }
    }
}
