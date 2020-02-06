using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Servirtium.Demo.PlanetService
{
    public static class PlanetServiceFactory
    {

        public static IHost Create(int port) =>
            Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                //If a port is specified, override urls with specified port, listening on all available hosts, for HTTP.
                webBuilder.UseUrls($"http://*:{port}");
                webBuilder.ConfigureServices((ctx, services) => services.AddControllers());
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    //app.UseMvc();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }).Build();
    }
}
