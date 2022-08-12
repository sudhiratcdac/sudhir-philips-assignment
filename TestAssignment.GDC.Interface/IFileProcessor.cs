namespace TestAssignment.GDC.Interface
{
    /// <summary>
    /// Responsible for reading and writing from/to file
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Check if file path is valid
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <returns>True: If path is valid false otherwise</returns>
        (bool IsValid, string ErrorMessage) IsValidFile(string filePath);

        /// <summary>
        /// Read file line by line and return the same as soon as it got read
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <returns>IAsyncEnumerable with the line that has been read</returns>
        IAsyncEnumerable<string> ReadLinesAsync(string filePath);

        /// <summary>
        /// Convert the path to XML and write content to it
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="content">XML content to be written to file</param>
        /// <returns></returns>
        Task WriteAsync(string filePath, string content);
    }
}