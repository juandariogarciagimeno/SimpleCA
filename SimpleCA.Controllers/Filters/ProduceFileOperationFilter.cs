using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SimpleCA.Controllers.Filters
{
    public class ProduceFileOperationFilter : IOperationFilter
    {
        private static string[] FileMediaTypes = ["application/zip", "application/pkix-cr"];
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var r in operation.Responses)
            {
                if (r.Key == "200")
                {
                    foreach (var c in r.Value.Content)
                    {
                        var attrs = context.MethodInfo.GetCustomAttributes<ProduceFileDescriptorAttribute>();
                        if (attrs != null && attrs.Count() > 0)
                        {
                            var attr = attrs.FirstOrDefault(x => x.ContentType == c.Key);
                            if (attr != null)
                            {
                                c.Value.Schema = new OpenApiSchema()
                                {
                                    Format = "binary",
                                    Type = "string",
                                    Title = "file",
                                    Description = attr.Description,
                                    Example = new OpenApiString(attr.ExampleFileName)
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
