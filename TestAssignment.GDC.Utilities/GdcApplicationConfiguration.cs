using Microsoft.Extensions.Configuration;


namespace TestAssignment.GDC.Utilities
{
    /// <summary>
    /// Responsible for reading configuration from appsettings.json
    /// </summary>
    public class GdcApplicationConfiguration : IApplicationConfiguration
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialize Microsoft extension's configuration 
        /// </summary>
        /// <param name="configuration"></param>
        public GdcApplicationConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        ///<inheritdoc/>
        public int MaxXmlProcessor
        {
            get
            {
                int.TryParse(_configuration["MaxXmlProcessor"], out int maxXmlProcessor);
                return maxXmlProcessor == 0 ? 1 : maxXmlProcessor;
            }
        }

        ///<inheritdoc/>
        public string RootNodeName
        {
            get
            {
                return _configuration["RootNodeName"] ?? "gedcom";
            }
        }
    }
}