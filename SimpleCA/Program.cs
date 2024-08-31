using Asp.Versioning;
using Microsoft.OpenApi.Models;
using SimpleCA.API.BackgroundServices;
using SimpleCA.API.Global;
using SimpleCA.Controllers.Filters;
using SimpleCA.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.ResolveConflictingActions(a => a.First());
    o.OperationFilter<ProduceFileOperationFilter>();
    //o.OperationFilter<FileUploadOperationFilter>();
});
builder.Services.AddRouting(o => o.LowercaseUrls = true);

builder.Services.AddUseCases();
builder.Services.AddServices();

builder.Services.AddApiVersioning(v =>
{
    v.DefaultApiVersion = new ApiVersion(builder.Configuration.GetValue<int>("DefaultApiVersion"));
    v.ReportApiVersions = true;
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(o =>
{
    o.GroupNameFormat = "'v'V";
    o.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHostedService<CreateCAHostedService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandling>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();