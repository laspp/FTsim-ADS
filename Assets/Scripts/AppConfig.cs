using System;
using System.Collections.Generic;

[Serializable]
public class AppConfig
{
    public string PlcAmsNetId;
    public int PlcAdsPort;
    public int NotificationCycleTime;
    public int NotificationMaxDelay;
    public bool ShowFPS;
    public Dictionary<string, string> InputVariableMap;
    public Dictionary<string, string> OutputVariableMap;
    public Dictionary<string, string> TrainingModelSpecific;
}