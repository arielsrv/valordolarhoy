using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using ValorDolarHoy.Core.Common.Serialization;
using ValorDolarHoy.Core.Middlewares;
using ValorDolarHoy.Mappings;

namespace ValorDolarHoy;

/// <summary>
///     Application
/// </summary>
public abstract class Application
{
    /// <summary>
    ///     Config
    /// </summary>
    protected readonly IConfiguration Configuration;

    /// <summary>
    ///     Application
    /// </summary>
    /// <param name="configuration"></param>
    protected Application(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    ///     Services
    /// </summary>
    /// <param name="services"></param>
    protected abstract void Init(IServiceCollection services);

    /// <summary>
    ///     This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "ValorDolarHoy", Version = "v1" });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // In production, the React files will be served from this directory
        services.AddSpaStaticFiles(spaStaticFilesOptions => { spaStaticFilesOptions.RootPath = "ClientApp/build"; });

        services
            .AddMvc(options =>
            {
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(typeof(ErrorHandlerMiddleware.ErrorModel), 500));
            })
            .AddJsonOptions(Serializer.BuildSettings);

        BuildMapper(services);
        this.Init(services);
    }

    private static void BuildMapper(IServiceCollection services)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        MapperConfigurationExpression config = new()
        {
            LicenseKey = "DEMO-LICENSE-KEY-FOR-TESTING"
        };
        config.AddProfile(new MappingProfile());
        MapperConfiguration mapperConfiguration = new(config, loggerFactory);
        IMapper mapper = mapperConfiguration.CreateMapper();
        services.AddSingleton(mapper);
    }

    /// <summary>
    ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ValorDolarHoy v1"));

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();

        app.UseRouting();

        app.UseMiddleware<ErrorHandlerMiddleware>();

        // app.UseWarmUp();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller}/{action=Index}/{id?}");
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
        });
    }
}