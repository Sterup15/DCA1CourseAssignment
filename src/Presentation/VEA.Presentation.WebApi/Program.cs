using VEA.Core.Application.DependencyInjection;
using VEA.Core.QueryContracts.DependencyInjection;
using VEA.Infrastructure.EfcPersistence.DependencyInjection;
using VEA.Infrastructure.EfcQueries.DependencyInjection;
using VEA.Presentation.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var writeConnectionString = builder.Configuration.GetConnectionString("WriteDb")
    ?? "Data Source=VEAWrite.db";
var readConnectionString = builder.Configuration.GetConnectionString("ReadDb")
    ?? "Data Source=VEARead.db";

builder.Services.AddEfcPersistence(writeConnectionString);
builder.Services.AddEfcQueries(readConnectionString);
builder.Services.AddApplicationHandlers(typeof(VEA.Core.Application.DependencyInjection.ApplicationServiceExtensions).Assembly);
builder.Services.AddCommandDispatcher();
builder.Services.AddQueryDispatcher();
builder.Services.AddWebApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
