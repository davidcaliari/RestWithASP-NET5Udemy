using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Data.Contract;
using RestWithASPNETUdemy.Data.Converter.Contract.Implementations;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Hypermedia.Abstract.Enricher;
using RestWithASPNETUdemy.Hypermedia.Filters;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Repository.Generic;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



// Add services to the container.

builder.Services.AddControllers();

//Pegando a string de connexção
var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version())));

var filterOptions = new HyperMediaFilterOptions();
filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());
builder.Services.AddSingleton(filterOptions);

//Versioning API
builder.Services.AddApiVersioning();

//Adicionando Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "REST API's From 0 to Azure with ASP >NET Core 5 and Docker",
            Version = "v1" ,
            Description = "API RESTful developed in course 'REST API's From 0 to Azure with ASP .NET Core 5 and Docker'",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "David Ribeiro",
                Url = new Uri("https://github.com/davidcaliari")
            }
        });
});

//Dependency Injection
builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

//Verifica se é development e executa as migrations
if (app.Environment.IsDevelopment())
{
    MigrateDatabase(connection);
    /*
    using(var scope = app.Services.CreateScope())
    {
        var evolveDbContext = scope.ServiceProvider.GetRequiredService<MySQLContext>();
        evolveDbContext.Database.Migrate();
    }
    */

}

void MigrateDatabase(string connection)
{
    try
    {
        var evolveConnection = new MySqlConnector.MySqlConnection(connection);
        var evolve = new Evolve.Evolve(evolveConnection, msg => Log.Information(msg))
        {
            Locations = new List<string> { "db/migrations", "db/dataset" },
            IsEraseDisabled = true

        };
        evolve.Migrate();
    }
    catch (Exception ex)
    {
        Log.Error("Database migration failde", ex);
        throw;
    }
}



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//Responsável por gerar o Json da documentação.
app.UseSwagger();

//Responsável por gerar o HTML para acessos as rotas.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "REST API's From 0 to Azure with ASP >NET Core 5 and Docker - v1");
});

//Configurando a swagger page.
var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");
app.UseRewriter(option);

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");

app.Run();
