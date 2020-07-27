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
    /// Hook into the tool and library's processes.
    /// </summary>
    public class Hooks
    {
        /// <summary>Immediately after getting the global context</summary>
        public virtual void GlobalAfterInitialization() {}
        /// <summary>Just prior to any rendering</summary>
        public virtual void GlobalPreRender() {}
        /// <summary>After writing the project output to the disk</summary>
        public virtual void GlobalPostWrite() {}
        /// <summary>Whenever a page is initialized</summary>
        public virtual void PagePostInitialization() {}
        /// <summary>Just before rendering a page</summary>
        public virtual void PagePreRender() {}
        /// <summary>After rendering a page, but before writing it to disk</summary>
        public virtual void PagePostRender() {}
        /// <summary>After writing a page to disk</summary>
        public virtual void PagePostWrite() {}
        /// <summary>After the cleanup of a project, usually right before building</summary>
        public virtual void CleanOnObsolete() {}
    }
}