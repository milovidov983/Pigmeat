// Copyright (C) 2020 Emil Sayahi
/*
This file is part of Pigmeat.

    Pigmeat is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pigmeat is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pigmeat.  If not, see <https://www.gnu.org/licenses/>.
*/
namespace Pigmeat.Core.Plugins
{
    /// <summary>
    /// Extend the <c>Pigmeat.Core</c>-based tool.
    /// </summary>
    public interface ICommand
    {
        /// <summary>The name of the command</summary>
        string Name { get; }
        /// <summary>The command's description</summary>
        string Description { get; }
        /// <summary>A command to execute from the tool, with an error code</summary>
        int Execute();
    }

    /// <summary>
    /// Generate additional content programatically.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>Generate content, return error code</summary>
        int Generate();
    }
}