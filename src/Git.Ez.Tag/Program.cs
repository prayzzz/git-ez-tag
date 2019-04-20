using System.IO;
using System.Threading.Tasks;
using Git.Ez.Tag.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Git.Ez.Tag
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                         .ConfigureHostConfiguration(builder =>
                         {
                             builder.SetBasePath(Directory.GetCurrentDirectory());
                             builder.Add<JsonConfigurationSource>(src =>
                             {
                                 src.FileProvider = new EmbeddedFileProvider(typeof(Program).Assembly, "Git.Ez.Tag");
                                 src.Path = "appsettings.json";
                             });
                         })
                         .ConfigureLogging((context, builder) =>
                         {
                             builder.AddConsole();
                             builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                         })
                         .ConfigureServices((context, services) =>
                         {
                             services.AddSingleton<Git>();
                             services.AddSingleton<EzTag>();
                             services.AddSingleton<NextTagService>();
                             services.AddSingleton<AnnotationService>();
                         })
                         .UseSerilog((context, configuration) =>
                         {
                             configuration.MinimumLevel.Debug();
                             configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
                             configuration.WriteTo.Console(outputTemplate: "[{Level}] {Message:lj}{NewLine}{Exception}");
                         })
                         .RunCommandLineApplicationAsync<EzTag>(args);
        }
    }
}