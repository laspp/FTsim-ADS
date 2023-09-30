## Description

The package **'Beckhoff.TwinCAT.Ads.Server'** contains the base framework to create your own ADS Server / virtual ADS Device.

## Requirements

- **.NET 6.0**, **.NET Core 3.1**, **.NET Framework 4.61** or **.NET Standard 2.0** compatible SDK or later.
- Latest **TwinCAT 3.1.4024** Build.
- or alternatively for systems where a TwinCAT installation is not running the Nuget package **'Beckhoff.TwinCAT.Ads.AdsRouterConsole'**.
to route ADS communication.
- Installed Nuget package manager (for systems without Visual Studio installation).

## Installation

### TwinCAT Version >= 4024.10

Because the Beckhoff.TwinCAT.Ads Version 5.X uses internal interfaces that are available only from TwinCAT 4024.10 on, an appropriate
version must be installed locally. The package doesn't work with older installations. An alternativ approach for some use cases is
to use the 'Beckhoff.TwinCAT.Ads.AdsRouterConsole' / 'Beckhoff.TwinCAT.TcpIpRouter' packages to establish your own router.

## Version Support lifecycle

| Package | Description | .NET Framework | TwinCAT | Active Support |
|---------|-------------|----------------|---------|-----------------
6.x | Actual package supporting .NET 6.0 | net6.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | X |
5.x | Package supporting .NET 5.0[^3] | net5.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | |
4.x | Package basing on .NET Framework 4.0 | net4 | All | X |

[^1]: Requirement on the Host system. No version limitation in remote system communication.

[^2]: Microsoft support for .NET5 ends with May 8, 2022. Therefore it is recommended to update **Beckhoff.TwinCAT** packages from Version 5 to Version 6.

[Migrate from ASP.NET Core 5.0 to 6.0](https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio)

[migrating to the latest .NET](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration)
[Microsoft .NET support lifecycle](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)

## First Steps

Create your customized ADS Server by deriving the TwinCAT.Ads.Server.AdsServer class. Fill the virtual handlers with your own
code.

```csharp
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

namespace TestServer
{
    /*
     * Extend the AdsServer class to implement your own ADS server.
     */
    public class AdsSampleServer : AdsServer
    {
        /// <summary>
        /// Fixed ADS Port (to be changed ...)
        /// </summary>
        const ushort ADS_PORT = 42;

        /// <summary>
        /// Fixed Name for the ADS Port (change this ...)
        /// </summary>
        const string ADS_PORT_NAME = "AdsSampleServer_Port42";


        /// <summary>
        /// Logger
        /// </summary>
        private ILogger _logger;

        /* Instantiate an ADS server with a fix ADS port assigned by the ADS router.
        */


        public AdsSampleServer(ILogger logger) : base(ADS_PORT, ADS_PORT_NAME)
        {
            _logger = logger;
        }

        // Override Functions to implement customized Server
        ....
    }
}
```

## Further documentation

The actual version of the documentation is available in the Beckhoff Infosys:
[Beckhoff Information System](https://infosys.beckhoff.com/index.php?content=../content/1033/tc3_ads.net/index.html&id=207622008965200265)

## Sample Code

Demo Code for AdsServer implementations can be found here:
[Beckhoff GitHub](https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/tree/main/Sources/ServerSamples)
