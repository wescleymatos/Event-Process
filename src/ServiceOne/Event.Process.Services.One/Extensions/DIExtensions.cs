namespace Event.Process.Services.One.Extensions
{
    public static class DIExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection service)
        {
            service.AddScoped<ILog>((serviceProvider) =>
            {
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                if (env.IsDevelopment())
                {
                    return new DsvLog();
                }

                return new PrdLog();
            });
        }
    }
}
