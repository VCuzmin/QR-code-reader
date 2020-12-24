using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QRCodeReader.Api.Swagger;
using QRCodeReader.Application.Commands;
using System;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace QRCodeReader.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllers().AddNewtonsoftJson();
            services.AddMediatR(typeof(DecodeQrImage));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddProblemDetails(ConfigureProblemDetails);
            services.AddSwagger();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));

            services.Scan(scan => scan.FromAssemblyOf<DecodeQrImage>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        [Obsolete]
        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = context => true;
            options.MapStatusCode = statusCode => new StatusCodeProblemDetails(statusCode);

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.Map<NotImplementedException>(ex =>
                new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));

            options.Map<ValidationException>(ex =>
                new ExceptionProblemDetails(ex, StatusCodes.Status422UnprocessableEntity));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(cors =>
            {
                cors
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });

            app.UseProblemDetails();
            app.UseAuthentication();
            app.UseRouting();
            //app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QR code reader Api");
            });
        }
    }
}