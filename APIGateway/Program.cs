namespace APIGateway
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        IWebHostBuilder builder = new WebHostBuilder();
    //        builder.ConfigureServices(s =>
    //        {
    //            s.AddSingleton(builder);
    //        });
    //        builder.UseKestrel()
    //               .UseContentRoot(Directory.GetCurrentDirectory())
    //               .UseStartup<Startup>()
    //               .UseUrls("http://localhost:9000");

    //        var host = builder.Build();
    //        host.Run();
    //    }
    //}
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
