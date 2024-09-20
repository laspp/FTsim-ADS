using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using TwinCAT;
using TwinCAT.ValueAccess;

public class Communication : MonoBehaviour
{

    public AppConfig appConfig;  // Parsed JSON configuration from configFile
    public StatusBar panelStatusBar;

    private AdsClient adsClient;
    private string configFilePath;
    private bool isConfigFileRead = false;
    private bool isConnectedToPlc = false;
    private bool areSymbolsMapped = false;
    private int tagCycleTime; // On how many miliseconds should PLC check for data change.
    private int tagMaxDelay;  // The maximum Delay time for ADS Notifications.

    // A mapping from PLC tag names (values in appConfig.OutputVariableMap) to Symbol
    private Dictionary<string, IValueSymbol> outputPlcTagToSymbol = new();
    // A mapping from PLC tag names to read value
    private Dictionary<string, bool> outputPlcTagToValue = new();
    // A mapping from tag names from config file (keys in appConfig.InputVariableMap) to Symbol
    private Dictionary<string, IValueSymbol> inputPlcTagToSymbol = new();

    // Error set for storing messages about symbol mapping
    private HashSet<string> errorSetSymbols = new();
    // How many error messages are stored in the set. Set this to 0 to turn limit off.
    private int errorSetSymbolsLimit = 0;


    void Awake()
    {
        Application.runInBackground = true;

        //Debug.Log("Communication: connecting to Beckhoff PLC over ADS ...\n");

        try
        {
            ConfigFileLoad();

            tagCycleTime = appConfig.NotificationCycleTime;
            tagMaxDelay = appConfig.NotificationMaxDelay;

            PlcConnect();
            PlcFetchSymbols();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Communication: error during initialization: {ex.Message}");
        }
    }

    void Start()
    {
        InvokeRepeating(nameof(ErrorsDisplayConfig), 1f, 1f);
        InvokeRepeating(nameof(ErrorsDisplayConnection), 1f, 1f);  //function name, init delay, repeat frequency
        InvokeRepeating(nameof(ErrorsDisplaySymbols), 1f, 1f);
    }

    private void PlcConnect()
    {
        try
        {
            adsClient = new AdsClient();
            adsClient.Connect(appConfig.PlcAmsNetId, appConfig.PlcAdsPort);

            if (adsClient.IsConnected)
            {
                //Debug.Log($"PlcConnect: connected to PLC ({adsClient.Address}).");
                isConnectedToPlc = true;
                panelStatusBar.SetStatusBarText($"Successfully connected to PLC ({adsClient.Address}).");
            }
            else
            {
                throw new Exception($"Cannot connect to PLC with address {appConfig.PlcAmsNetId}, {appConfig.PlcAdsPort}.");
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError($"PlcConnect: {ex.Message}");
            ErrorsAdd($"PlcConnect: {ex.Message}");
            isConnectedToPlc = false;
        }

    }

    private void PlcFetchSymbols()
    {
        bool symbolsFetchedOK = true;
        try
        {
            ISymbolLoader symbolLoader = SymbolLoaderFactory.Create(adsClient, SymbolLoaderSettings.Default);

            // Inputs
            foreach (string plcTag in appConfig.InputVariableMap.Values)
            {
                IValueSymbol symbol = null;
                try
                {
                    symbol = (IValueSymbol)symbolLoader.Symbols[plcTag];
                }
                catch
                {
                    //Debug.LogError($"DBG PlcFetchSymbols: Cannot map tag '{plcTag}'; does it exists on PLC? ");
                    ErrorsAdd(plcTag);
                    symbolsFetchedOK = false;
                }

                inputPlcTagToSymbol[plcTag] = symbol;
            }

            // Outputs
            foreach (string plcTag in appConfig.OutputVariableMap.Values)
            {
                IValueSymbol symbol = null;
                try
                {
                    symbol = (IValueSymbol)symbolLoader.Symbols[plcTag];
                    SubscribeToSymbol(symbol);
                }
                catch
                {
                    Debug.LogError($"PlcFetchSymbols: Cannot map tag '{plcTag}'; does it exists on PLC? ");
                    ErrorsAdd(plcTag);
                    symbolsFetchedOK = false;
                }

                outputPlcTagToSymbol[plcTag] = symbol;
                outputPlcTagToValue[plcTag] = false;
            }

            areSymbolsMapped = symbolsFetchedOK;
        }
        catch
        {
            //Debug.LogError($"PlcFetchSymbols: {ex.Message}");
            areSymbolsMapped = false;
        }

    }
    private void ErrorsAdd(string message)
    {
        if (errorSetSymbolsLimit == 0 || errorSetSymbols.Count < errorSetSymbolsLimit)
        {
            errorSetSymbols.Add(message);
        }
    }

    private void ErrorsDisplayConfig()
    {
        // Check for errors and show a dialog with list
        if (!isConfigFileRead)
        {
            if (GameObject.FindWithTag("Dialog_error_config") == null)
            {
                Dialog.MessageBox(
                    "Dialog_error_config",
                    "Config file error",
                    $"The config file cannot be read. Attempting to read file:\n'{configFilePath}'",
                    "Retry", () => { Awake(); }, widthMax: 300, heightMax: 120
                    );
            }
        }
    }

    private void ErrorsDisplayConnection()
    {
        // Check for errors and show a dialog with list
        if (isConfigFileRead && !isConnectedToPlc)
        {
            if (GameObject.FindWithTag("Dialog_error_PLC_connection") == null)
            {
                Dialog.MessageBox(
                    "Dialog_error_PLC_connection",
                    "Connection error",
                    $"The connection with the Beckhoff PLC cannot be established. Address in the config file is:\n{appConfig.PlcAmsNetId}, {appConfig.PlcAdsPort}",
                    "Retry", () => { Awake(); }, widthMax: 300, heightMax: 120
                    );
            }
        }
    }

    private void ErrorsDisplaySymbols()
    {
        // Check for errors and show a dialog with list
        if (isConfigFileRead && isConnectedToPlc && errorSetSymbols.Count != 0)
        {
            if (GameObject.FindWithTag("Dialog_error_list") == null)
            {
                string text = "";
                foreach (var errorMessage in errorSetSymbols)
                {
                    text += "  - " + errorMessage + "\n";
                }
                DialogWithScroll.MessageBox(
                    "Dialog_error_list",
                    "Tags mapping error",
                    text,
                    "Retry", () => { ErrorsClear(); ConfigFileLoad(); PlcFetchSymbols(); },
                    widthMax: 300, heightMax: 300,
                    scrollHeight: 300,
                    preScrollText: "There were errors when mapping IO variables. Do the following variables exist on PLC?\n"
                    );
            }
        }
    }

    private void ErrorsClear()
    {
        // Clear errors in a set
        errorSetSymbols.Clear();
    }

    void OnApplicationQuit()
    {
        // Unsubscribe from all events and clean up resources
        UnsubscribeFromAllSymbols();
        // Disconnect and dispose
        adsClient?.Disconnect();
        adsClient?.Dispose();
    }

    private void SubscribeToSymbol(IValueSymbol symbol)
    {
        symbol.NotificationSettings = new NotificationSettings(AdsTransMode.OnChange, tagCycleTime, tagMaxDelay);
        symbol.ValueChanged += SymbolOnValueChanged;
    }
    private void ConfigFileLoad()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string configFile = "config-" + sceneName + ".json";
        try
        {
            configFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, configFile);
            appConfig = ConfigFileManager.LoadConfig(configFilePath);
            isConfigFileRead = true;
        }
        catch (Exception)
        {
            isConfigFileRead = false;
            throw;
        }
    }

    public void ConfigFileSave()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string configFile = "config-" + sceneName + ".json";
        try
        {
            configFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, configFile);
            ConfigFileManager.SaveConfig(configFilePath, appConfig);
            panelStatusBar.SetStatusBarText($"Configuration saved to {configFile}.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async void WriteToPlc(string tag, bool valueToWrite)
    {
        if (isConnectedToPlc && areSymbolsMapped)
        {
            CancellationToken cancel = CancellationToken.None;
            try
            {
                string plcTag = appConfig.InputVariableMap[tag];
                IValueSymbol symbol = inputPlcTagToSymbol[plcTag];
                ResultWriteAccess resultWrite = await symbol.WriteValueAsync(valueToWrite, cancel);
                //Debug.Log($"{tag} ::: write success? {resultWrite.Succeeded}");
            }
            catch
            {
                //Debug.LogError($"WriteToPlc: error during writing tag '{tag}' to PLC: {ex.Message}");
                ErrorsAdd($"Cannot write tag '{tag}' on PLC.");
            }
        }
        else
        {
            //Debug.LogError($"WriteToPlc: cannot write tag {tag} to PLC, either there is no connection or mapping errors exist.");
            //ErrorsAdd($"WriteToPlc: cannot write tag '{tag}' to PLC, either there is no connection or mapping errors exist.");
        }

    }



    public void WriteToPlc(string tag, int valueToWrite)
    {
        // Convert int to bool: 0 is false, any other value is true
        WriteToPlc(tag, valueToWrite != 0);
    }

    private void UnsubscribeFromAllSymbols()
    {
        foreach (var symbol in outputPlcTagToSymbol.Values)
        {
            symbol.ValueChanged -= SymbolOnValueChanged;
        }
    }

    public bool GetTagValue(string tag)
    {
        try
        {
            string plcTag = appConfig.OutputVariableMap[tag];
            bool value = outputPlcTagToValue[plcTag];
            return value;
        }
        catch (Exception ex)
        {
            //Debug.LogError($"Communication: error getting value from dictionary: {ex.Message}");
            ErrorsAdd(ex.Message);
            return false;
        }
    }

    private void SymbolOnValueChanged(object sender, ValueChangedEventArgs e)
    {
        Symbol sym = (Symbol)e.Symbol;
        bool val = (bool)e.Value;
        outputPlcTagToValue[sym.InstancePath] = val;
        //Debug.Log($"ADS notification: {sym.InstancePath} value changed to {val}");
    }

}