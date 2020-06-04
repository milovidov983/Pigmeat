# -*- Mode:Python; indent-tabs-mode:nil; tab-width:4 -*-
#
# Copyright (C) 2017 Canonical Ltd
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License version 3 as
# published by the Free Software Foundation.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.

"""The dotnet plugin is used to build dotnet core runtime parts.'

The plugin uses the dotnet SDK to install dependencies from nuget
and follows standard semantics from a dotnet core project.

This plugin uses the common plugin keywords as well as those for "sources".
For more information check the 'plugins' topic for the former and the
'sources' topic for the latter.

The plugin will take into account the following build-attributes:

    - debug: builds using a Debug configuration.

"""

import os
import shutil
import fnmatch
import urllib.request
import json
import platform

import snapcraft
from snapcraft import sources
from snapcraft import formatting_utils
from snapcraft.internal import errors
from typing import List

_DOTNET_RELEASE_METADATA_URL = (
    #"https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json"
    "https://dotnetcli.azureedge.net/dotnet/release-metadata/releases-index.json"
)  # noqa
_SDK_DEFAULT = "3.1"

_SDK_ARCH = ["armhf", "arm64", "i386", "amd64"]


class DotNetBadArchitectureError(snapcraft.internal.errors.SnapcraftError):

    fmt = (
        "Failed to prepare the .NET SDK: "
        "The architecture {architecture!r} is not supported. "
        "Supported architectures are: {supported}."
    )

    def __init__(self, *, architecture: str, supported: List[str]) -> None:
        super().__init__(
            architecture=architecture,
            supported=formatting_utils.humanize_list(supported, "and"),
        )


class DotNetBadReleaseDataError(snapcraft.internal.errors.SnapcraftError):

    fmt = (
        "Failed to prepare the .NET SDK: "
        "An error occurred while fetching the version details "
        "for {version!r}. Check that the version is correct."
    )

    def __init__(self, version):
        super().__init__(version=version)


class DotNetPlugin(snapcraft.BasePlugin):
    @classmethod
    def schema(cls):
        schema = super().schema()

        schema["properties"]["dotnet-sdk-version"] = {
            "type": "string",
            "default": _SDK_DEFAULT,
        }
        schema["required"] = ["source"]

        return schema

    @classmethod
    def get_pull_properties(cls):
        # Inform Snapcraft of the properties associated with pulling. If these
        # change in the YAML Snapcraft will consider the build step dirty.
        return ["dotnet-sdk-version"]

    def __init__(self, name, options, project):
        super().__init__(name, options, project)

        self._dotnet_targets = {
            'armhf': 'arm',
            'arm64': 'arm64',
            'i386': 'x86',
            'amd64': 'x64'
        }

        self._dotnet_dir = os.path.join(self.partdir, "dotnet")
        self._dotnet_sdk_dir = os.path.join(self._dotnet_dir, "sdk")

        self._setup_base_tools(project.info.base)

        self._sdk = self._get_sdk()
        self._dotnet_cmd = os.path.join(self._dotnet_sdk_dir, "dotnet")

    def _setup_base_tools(self, base):
        if base == "core20":
            self.stage_packages.extend(
                [
                    "libcurl4",
                    "libicu66",
                    "liblttng-ust0",
                ]
            )
        elif base == "core18":
            self.stage_packages.extend(
                [
                    "libcurl4",
                    "libicu60",
                    "liblttng-ust0",
                ]
            )
        else:
            self.stage_packages.extend(
                [
                    "libcurl3",
                    "libicu55",
                    "liblttng-ust0",
                ]
            )
        if self.project.is_cross_compiling:
            i = 0
            for s in self.stage_packages:
                self.stage_packages[i] = "{}:{}".format(s, self.project.deb_arch)
                i = i + 1

    def _get_sdk(self):
        #sdk_arch = self.project.deb_arch
        arch_map = {
            'arm': 'armhf',
            'armv7l': 'armhf',
            'aarch64': 'arm64',
            'i386': 'i386',
            'x86_64': 'amd64'
        }
        sdk_arch = arch_map.get(platform.machine())
        if sdk_arch not in _SDK_ARCH:
            raise DotNetBadArchitectureError(architecture=sdk_arch, supported=_SDK_ARCH)
        # TODO: Make this a class that takes care of retrieving the infos
        sdk_info = self._get_sdk_info(self.options.dotnet_sdk_version, sdk_arch)

        sdk_url = sdk_info["package_url"]
        return sources.Tar(
            sdk_url, self._dotnet_sdk_dir, source_checksum=sdk_info["checksum"]
        )

    def pull(self):
        super().pull()

        os.makedirs(self._dotnet_sdk_dir, exist_ok=True)

        self._sdk.pull()

    def clean_pull(self):
        super().clean_pull()

        # Remove the dotnet directory (if any)
        if os.path.exists(self._dotnet_dir):
            shutil.rmtree(self._dotnet_dir)

    def build(self):
        super().build()

        if "debug" in self.options.build_attributes:
            configuration = "Debug"
        else:
            configuration = "Release"

        self.run([self._dotnet_cmd, "build", "-c", configuration])

        publish_cmd = [
            self._dotnet_cmd,
            "publish",
            "-c",
            configuration,
            "-o",
            self.installdir,
        ]
        # Build command for self-contained application
        publish_cmd += ["--self-contained", "-r", "linux-{}".format(self._dotnet_targets.get(self.project.deb_arch))]
        self.run(publish_cmd)

        # Workaround to set the right permission for the executable.
        appname = os.path.join(self.installdir, self._get_appname())
        if os.path.exists(appname):
            os.chmod(appname, 0o755)

    def enable_cross_compilation(self):
        pass

    def _get_appname(self):
        for file in os.listdir(self.builddir):
            if fnmatch.fnmatch(file, "*.??proj"):
                return os.path.splitext(file)[0]

    def _get_version_metadata(self, url):
        jsonData = self._get_dotnet_release_metadata(url)
        sdk_version = jsonData["latest-sdk"]
        package_data = list(
            filter(lambda x: x.get("sdk").get("version") == sdk_version, jsonData["releases"])
        )

        if not package_data:
            raise DotNetBadReleaseDataError(sdk_version)

        return package_data[0]

    def _get_channel_metadata(self, version):
        jsonData = self._get_dotnet_release_metadata(_DOTNET_RELEASE_METADATA_URL)

        package_data = list(
            filter(lambda x: x.get("channel-version") == version, jsonData["releases-index"])
        )

        if not package_data:
            raise DotNetBadReleaseDataError(version)

        return package_data[0]

    def _get_dotnet_release_metadata(self, url):
        package_metadata = []

        req = urllib.request.Request(url)
        r = urllib.request.urlopen(req).read()
        package_metadata = json.loads(r.decode("utf-8"))

        return package_metadata

    def _get_sdk_info(self, version, arch):
        channel = self._get_channel_metadata(version)
        metadata = self._get_version_metadata(channel["releases.json"])

        package_data = list(
            filter(lambda x: x.get("rid") == "linux-{}".format(self._dotnet_targets.get(arch)), metadata["sdk"]["files"])
        )

        sdk_package_url = package_data[0]["url"]
        sdk_checksum = package_data[0]["hash"]

        return {"package_url": sdk_package_url, "checksum": "sha512/{}".format(sdk_checksum).lower()}

    def env(self, root):
        # Update the PATH only during the Build and Install step
        if root == self.installdir:
            return ["PATH={}:$PATH".format(self._dotnet_sdk_dir)]
        else:
            return []