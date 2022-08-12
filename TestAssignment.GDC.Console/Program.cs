// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestAssignment.GDC.Interface;
using TestAssignment.GDC.Lexical;
using TestAssignment.GDC.Utilities;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json");
IConfiguration Configuration = configuration.Build();

using IHost host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        services.AddLexcialParserServices();
                        services.AddSingleton(Configuration);
                        services.AddSingleton<IApplicationConfiguration, GdcApplicationConfiguration>();
                    })
                    .Build();
Start(host.Services);

host.RunAsync();


static void Start(IServiceProvider service)
{
    string filePath;
    var lexicalAnalyzer = service.GetService<ILexicalController>();
    Console.WriteLine($"Enter input nodes in string format: ");
    filePath = Console.In.ReadToEnd();
    lexicalAnalyzer.Parse(filePath).ConfigureAwait(false).GetAwaiter().GetResult();
    Console.ReadKey();
}


