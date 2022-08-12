using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TTestAssignment.GDC.Lexical;

namespace TestAssignment.GDC.Lexical
{
    [ExcludeFromCodeCoverage]

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
            services.AddTransient<IFileProcessor, StringProcessor>();
            //services.AddTransient<IFileProcessor, TextFileProcessor>();
            services.AddTransient<ILexicalNodeProcessor, LexicalNodeProcessor>();
            services.AddTransient<ILexicalController, LexicalController>();
            services.AddTransient<IGcdNodeCreator, GcdNodeCreator>();
        }
    }
}