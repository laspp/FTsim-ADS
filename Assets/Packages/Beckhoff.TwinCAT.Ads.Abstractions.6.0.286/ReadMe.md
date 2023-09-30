## Description

The package **'Beckhoff.TwinCAT.Ads.Abstractions'** contains interfaces and base implementations for the **'Beckhoff.TwinCAT.Ads.Server'** and
**'Beckhoff.TwinCAT.Ads'** packages. It is never used standalone and is a dependency of the above-named packages.

## Requirements

- **.NET 6.0**, **.NET Core 3.1**, **.NET Framework 4.61** or **.NET Standard 2.0** compatible SDK or later
- Latest **TwinCAT 3.1.4024** Build
- or alternatively for systems where a TwinCAT installation is not running the Nuget package **'Beckhoff.TwinCAT.Ads.AdsRouterConsole'**.
to route ADS communication.
- Installed Nuget package manager (for systems without Visual Studio installation)

### Version Support lifecycle

| Package | Description | .NET Framework | TwinCAT | Active Support |
|---------|-------------|----------------|---------|-----------------
6.1 | Package basing on .NET 7.0/6.0 | net7.0, net6.0, netstandard2.0 | >= 3.1.4024.10 [^1] | X |
6.0 | Package basing on .NET 6.0 | net6.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | X |
5.x | Package basing on .NET 5.0[^3] | net5.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | |
4.x | Package basing on .NET Framework 4.0 | net4 | All | X |

[^1]: Requirement on the Host system. No version limitation in remote system communication.

[^2]: Microsoft support for .NET5 ends with May 8, 2022. Therefore it is recommended to update **Beckhoff.TwinCAT** packages from Version 5 to Version 6.

[Migrate from ASP.NET Core 5.0 to 6.0](https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio)

[migrating to the latest .NET](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration)
[Microsoft .NET support lifecycle](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)

## Installation

As dependency of other Beckhoff packages

## Further documentation

The actual version of the documentation is available in the Beckhoff Infosys.
[Beckhoff Information System](https://infosys.beckhoff.com/index.php?content=../content/1033/tc3_ads.net/index.html&id=207622008965200265)