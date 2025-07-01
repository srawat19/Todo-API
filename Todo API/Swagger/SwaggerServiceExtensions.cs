using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Todo.Domain.DTOs;
using Todo_API.Swagger.Filters;

namespace Todo_API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection CreateSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "ToDo API",
                    Version = "1.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Shivangi Rawat",
                        Email = "api-support-team@gmail.com"
                    },
                    Description = "This RESTful API integrated with Azure AD Auth allows users to manage their to-do list"

                });

                s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://login.microsoftonline.com/{tenant-id}/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri("https://login.microsoftonline.com/{tenant-id}/oauth2/v2.0/token"),

                            Scopes = new Dictionary<string, string>
                            {
                                { "api://{backend-app-id}/access-as-app-user", "Access ToDo Api" }

                            }

                        }

                    },
                    Description = "Used Azure AD OAuth2 with Authorization Code + PKCE"

                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="oauth2"}
                    },
                        new []{ "access-as-app-user" }
                    }

                });

                string nm = typeof(PaginatedResponseDTO<TodoDTO>).Name;   //PaginatedResponseDTO`1 //1 is the number of argument

                //Modifying the schema for model bcz by default swagger was flattening the names which was
                // not easy to understand. it showed ex - PaginatedResponseDTOTodoDTO
                //wanted it to be clearer denoted PaginatedResponseDTO id of type Todo DTO 
                s.CustomSchemaIds(type =>
                {
                    if (type.IsGenericType)
                    {

                        var genericTypeName = type.GetGenericTypeDefinition().Name;

                        genericTypeName = genericTypeName.Contains('`') ? genericTypeName.Substring(0, genericTypeName.IndexOf('`')) : genericTypeName;

                        var argument = string.Join("Of", type.GenericTypeArguments.Select(x => x.Name));

                        return $"{genericTypeName}Of{argument}";
                    }

                    return type.Name;
                });

                //Adding Schema 
                s.SchemaFilter<PaginatedResponseDTOFilter>();

                s.EnableAnnotations();

                //Including XML comments
                s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));



            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDoc(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseSwagger(s => s.RouteTemplate = "todoApi/docs/{documentName}/swagger.json");
            appBuilder.UseSwaggerUI(action =>
            {
                action.SwaggerEndpoint("/todoApi/docs/v1/swagger.json", "Todo API v1");
                action.RoutePrefix = "todoApi/docs";
                action.OAuthUsePkce();
                action.OAuthClientId("<your-frontend-app-id>");

            });

            return appBuilder;
        }
    }
}
