namespace Event.Process.Services.One.Extensions
{
    public static class DIStartup
    {
        public static void AddDependencyInjection(this IServiceCollection service)
        {
            //service.AddScoped<ILog>((serviceProvider) =>
            //{
            //    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

            //    if (env.IsDevelopment())
            //    {
            //        return new DsvLog();
            //    }

            //    var logger = serviceProvider.GetRequiredService<ILogger<PrdLog>>();
            //    return new PrdLog(logger);
            //});

            var env = service.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                service.AddScoped(typeof(ILog<>), typeof(DsvLog<>));
            }
            else
            {
                service.AddScoped(typeof(ILog<>), typeof(PrdLog<>));
            }
        }
    }
}
