# Config files description

## Common

"PlcAmsNetId": "local", // "local" = "0.0.0.0.0.0"
"PlcAdsPort": 851, // 851 = first PLC in a project, 852 = second ...
"NotificationCycleTime": 20, // How frequently should PLC check for data change. In miliseconds.
"NotificationMaxDelay": 0, // The maximum Delay time for ADS Notifications. In miliseconds.
"ShowFPS": true, // whether to show frames per second on GUI or not

## L2N

N/A

## RR

// Number of generated impulses 
"RotateImpulses": 180,
"ExtendImpulses": 80,
"LiftImpulses": 120,
"GrabImpulses":18,

"PLCCycle" : 0.02  // Target cycle of PLCSIM emulator in seconds 
// This value is used when generating an impulse signal:
// signal is high/low at least for PLC_cycle seconds.
// Increase this number if you experience wrong counting of impulses.