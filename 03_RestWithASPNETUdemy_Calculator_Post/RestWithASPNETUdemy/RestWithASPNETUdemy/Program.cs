using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Configuration;
using RestWithASPNETUdemy.Hypermedia.Abstract.Enricher;
using RestWithASPNETUdemy.Hypermedia.Filters;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Repository.Generic;
using RestWithASPNETUdemy.Services;
using RestWithASPNETUdemy.Services.Implementations;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



// Add services to the container.

builder.Services.AddControllers();

//Pegando a string de connexção
var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version())));

//Configurando HATEOAS
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
            Version = "v1",
            Description = "API RESTful developed in course 'REST API's From 0 to Azure with ASP .NET Core 5 and Docker'",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "David Ribeiro",
                Url = new Uri("https://github.com/davidcaliari")
            }
        });
});

//Configurando para preecher assim que startar a aplicação o token configurations
var tokenConfigurations = new TokenConfiguration();
new ConfigureFromConfigurationOptions<TokenConfiguration>(
        builder.Configuration.GetSection("TokenConfigurations")
    )
    .Configure(tokenConfigurations);

//Injetando o token
builder.Services.AddSingleton(tokenConfigurations);
//Injetando autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenConfigurations.Issuer,
            ValidAudience = tokenConfigurations.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
        };
    });

//Injetando autorização
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});

//Adicionando CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

//Dependency Injection
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
builder.Services.AddScoped<ILoginBusiness, LoginBusinessImplementation>();
builder.Services.AddScoped<IFileBusiness, FileBusinessImplementation>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

//Verifica se é development e executa as migrations
if (app.Environment.IsDevelopment())
{
    //MigrateDatabase(connection);
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

//Adicionando o Cors
app.UseCors();

app.MapControllers();
app.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");

app.Run();
