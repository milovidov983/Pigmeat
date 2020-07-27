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