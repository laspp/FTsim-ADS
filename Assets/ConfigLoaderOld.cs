using System;
using UnityEngine;
using Newtonsoft.Json;

public class ConfigLoaderOld
{
    public static AppConfig LoadConfig(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            throw new ArgumentException($"Invalid or missing config file.");
        }

        try
        {

            // Deserialize the JSON data into an AppConfig object using JsonUtility
            //AppConfig appConfig = JsonUtility.FromJson<AppConfig>(jsonText);

            AppConfig appConfig = JsonConvert.DeserializeObject<AppConfig>(jsonText);

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
