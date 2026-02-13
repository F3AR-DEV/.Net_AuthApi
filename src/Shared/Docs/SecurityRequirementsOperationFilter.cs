using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;


namespace AuthApi.Shared.Docs
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            var isAuthorized = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
            var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);
            if (!isAuthorized || allowAnonymous)
            {
                return;
            }

            operation.Parameters ??= [];
            operation.Security =
            [
                new() {
                {
                    new OpenApiSecuritySchemeReference("Bearer", context.Document),
                    new List<string>()
                }
            },
            new() {
                {
                    new OpenApiSecuritySchemeReference("X-API-Key", context.Document),
                    new List<string>()
                }
            }
            ];



            // var authAttributes = context.MethodInfo.GetCustomAttributes(true)
            //     .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? System.Array.Empty<object>())
            //     .OfType<AuthorizeAttribute>();

            // if (!authAttributes.Any())
            // {
            //     return;
            // }

            // var schemes = authAttributes.Select(a => a.AuthenticationSchemes).Distinct();

            // var requirements = new List<OpenApiSecurityRequirement>();

            // if (schemes.Contains("x-api-key"))
            // {
            //     requirements.Add(new OpenApiSecurityRequirement
            //     {
            //         {
            //             new OpenApiSecuritySchemeReference("x-api-key", new OpenApiDocument()), // Ajuste especulativo basado en Program.cs original
            //             new List<string>()
            //         }
            //     });
            // }
            // else
            // {
            //     requirements.Add(new OpenApiSecurityRequirement
            //     {
            //         {
            //             new OpenApiSecuritySchemeReference("Bearer", new OpenApiDocument()),
            //             new List<string>()
            //         }
            //     });
            // }

            // operation.Security = requirements;
        }
    }
}
