// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Philips.GDC.Interface;
using Philips.GDC.Lexical;
using Philips.GDC.Utilities;

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
    do
    {
        Console.WriteLine($"Enter file path (Exit to terminate the application): ");
        filePath = Console.ReadLine();
        lexicalAnalyzer.Parse(filePath).ConfigureAwait(false).GetAwaiter().GetResult();
    }
    while (!filePath.Equals("exit", StringComparison.InvariantCultureIgnoreCase));
}


