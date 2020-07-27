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