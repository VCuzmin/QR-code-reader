using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace QRCodeReader.Api.Swagger
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>        
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "QR code reader Api",
                        Version = "v1"
                    }
                );

                c.CustomSchemaIds(type => type.ToString());
            });

            return services;
        }
    }
}