using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
public class BearerOperationFilter : IOperationFilter {
 public void Apply(OpenApiOperation operation, OperationFilterContext context) {
  var anonymous = context.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true) || context.MethodInfo.DeclaringType!.IsDefined(typeof(AllowAnonymousAttribute), true);
  var authorized = context.MethodInfo.IsDefined(typeof(AuthorizeAttribute), true) || context.MethodInfo.DeclaringType!.IsDefined(typeof(AuthorizeAttribute), true);
  if (!anonymous && authorized) operation.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = [] }];
 }
}
