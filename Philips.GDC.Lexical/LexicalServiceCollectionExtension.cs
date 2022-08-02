using Microsoft.Extensions.DependencyInjection;
using Philips.GDC.Interface;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for injecting classes for the service
    /// </summary>
    public static class LexicalServiceCollectionExtension
    {
        /// <summary>
        /// Add necessary inject for Lexical Parser service
        /// </summary>
        /// <param name="services"></param>
        public static void AddLexcialParserServices(this IServiceCollection services)
        {
            services.AddTransient<IFileProcessor, TextFileProcessor>();
            services.AddTransient<ILexicalNodeProcessor, LexicalNodeProcessor>();
            services.AddTransient<ILexicalController, LexicalController>();
            services.AddTransient<IGcdNodeCreator, GcdNodeCreator>();
        }
    }
}