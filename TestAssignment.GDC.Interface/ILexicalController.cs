namespace TestAssignment.GDC.Interface
{
    /// <summary>
    /// Responsible for parsing string to the NodeInput class
    /// </summary>
    public interface ILexicalController
    {
        /// <summary>
        /// Parse the content of the path specified into the XML document
        /// </summary>
        /// <param name="sourceStringOrFilePath">path of the file</param>
        /// <returns></returns>
        Task Parse(string sourceStringOrFilePath);
    }
}