# Pigmeat

[![Pigmeat Branding](https://github.com/MadeByEmil/Pigmeat/raw/master/branding/Yellow/Pigmeat%20Banner.png)](https://MadeByEmil.github.io/pigmeat-website)

[![NuGet](https://img.shields.io/nuget/v/Pigmeat)](https://www.nuget.org/packages/Pigmeat)
[![Snapcraft](https://snapcraft.io//pigmeat/badge.svg)](https://snapcraft.io/pigmeat)
[![Snapcraft](https://snapcraft.io//pigmeat/trending.svg?name=0)](https://snapcraft.io/pigmeat)

[![.NET Core (deb)](https://github.com/MadeByEmil/Pigmeat/workflows/.NET%20Core%20(deb)/badge.svg)](https://github.com/MadeByEmil/Pigmeat/actions?query=workflow%3A%22.NET+Core+%28deb%29%22)
[![.NET Core (rpm)](https://github.com/MadeByEmil/Pigmeat/workflows/.NET%20Core%20(rpm)/badge.svg)](https://github.com/MadeByEmil/Pigmeat/actions?query=workflow%3A%22.NET+Core+%28rpm%29%22)

[![.NET Core (Windows)](https://github.com/MadeByEmil/Pigmeat/workflows/.NET%20Core%20(Windows)/badge.svg)](https://github.com/MadeByEmil/Pigmeat/actions?query=workflow%3A%22.NET+Core+%28Windows%29%22)
[![.NET Core (macOS)](https://github.com/MadeByEmil/Pigmeat/workflows/.NET%20Core%20(macOS)/badge.svg)](https://github.com/MadeByEmil/Pigmeat/actions?query=workflow%3A%22.NET+Core+%28macOS%29%22)

Pigmeat is a static content publishing tool for the modern web. It takes data in the form of JSON or YAML, plugs it into a Liquid context, wraps it up in some HTML & CSS, and spits out a website that can be hosted on your favorite server. If you don't know what any of that means, it's OK. Point is, it's easy to learn and use. Pigmeat is for corporations, organizations, friends, peers, bloggers, hobbyists, and everyone under the sun.

## Pigmeat will get you online
 Pigmeat does *exactly* what it's told. It doesn't have any tricks to learn or exceptions to the rules. It just works.
 With Pigmeat, there's no more digging through cluttered bits of reused HTML, or updating old databases with strange syntax.
 You can focus on the only thing that matters to you: being online.

## Getting started
 * Download Pigmeat
   * EXPERIMENTAL: Download the latest build artifact from our [commits](https://github.com/MadeByEmil/Pigmeat/commits/master) page that corresponds to your operating system.
   * STABLE: Download the [latest release](https://github.com/MadeByEmil/Pigmeat/releases/latest) from our [releases](https://github.com/MadeByEmil/Pigmeat/releases) page that corresponds to your operating system.
      * [![Get it from the Snap Store](https://snapcraft.io/static/images/badges/en/snap-store-white.svg)](https://snapcraft.io/pigmeat) (see: [#38](https://github.com/MadeByEmil/Pigmeat/issues/38))
      * Pigmeat can also be install as a .NET Core global tool (`dotnet tool install Pigmeat -g`)
 * Read up on the [documentation](https://github.com/MadeByEmil/Pigmeat/wiki)
 * Take a look at [other people's Pigmeat projects](https://github.com/topics/pigmeat)

## Need help?
 If you're in need of assistance, you can reach code contributors on our GitHub repository.
 Check our [issues](https://github.com/MadeByEmil/Pigmeat/issues) page to see if your problem has already been resolved. If not, [raise a new issue](https://github.com/MadeByEmil/Pigmeat/issues/new/choose).

## Building from source
  Pre-requisites:
  - [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

  #### Windows
  ```
  dotnet build --configuration Release --runtime win-x64
  ```
  Outputs to ```.\bin\Release\netcoreapp3.1```

  #### macOS
  ```
  dotnet build --configuration Release --runtime osx-x64
  ```
  Outputs to ```./bin/Release/netcoreapp3.1```

  #### Linux
  ```
  dotnet build --configuration Release --runtime linux-x64
  ```
  Outputs to ```./bin/Release/netcoreapp3.1```

## Credits
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/c0403d9ba4494e7c820394cf9bafa917)](https://app.codacy.com/gh/MadeByEmil/Pigmeat?utm_source=github.com&utm_medium=referral&utm_content=MadeByEmil/Pigmeat&utm_campaign=Badge_Grade_Dashboard)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FMadeByEmil%2FPigmeat.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FMadeByEmil%2FPigmeat?ref=badge_shield)

 Authored by [Emil Sayahi](https://emsa.cf) ([@MadeByEmil](https://github.com/MadeByEmil))

 View all [contributors](https://github.com/MadeByEmil/Pigmeat/graphs/contributors)

---
View [dependency report](https://app.fossa.com/reports/f07fb746-aeba-448f-a419-10d3ffe1567c)

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FMadeByEmil%2FPigmeat.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FMadeByEmil%2FPigmeat?ref=badge_large)

---
Copyright (C) 2020 Emil Sayahi

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3.


This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.


You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
