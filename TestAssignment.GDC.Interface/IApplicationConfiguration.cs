namespace TestAssignment.GDC.Interface
{
    /// <summary>
    /// Responsible for reading configuration from appsettings.json
    /// </summary>
    public interface IApplicationConfiguration
    {
        /// <summary>
        /// Maximum number of the XML processor that would convert string to XML node
        /// </summary>
        int MaxXmlProcessor { get; }

        /// <summary>
        /// Root name of the XML file being created
        /// </summary>
        string RootNodeName { get; }
    }
}
