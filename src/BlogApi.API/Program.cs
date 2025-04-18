using BlogApi.API.IoC;
using BlogApi.API.Middlewares;
using BlogApi.Application.Auth.Commands.ChangePassword;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Data.IoC;
using BlogApi.Application.Infrastructure.Identity.DataSeeders;
using BlogApi.Application.IoC;
using BlogApi.Application.Tenancies.Commands.CreateTenancy;
using BlogApi.Infrastructure.Identity.Data;
using BlogApi.Infrastructure.Identity.IoC;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.ConfigureWebService(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.ConfigureData(builder.Configuration);
builder.Services.ConfigureIdentity(builder.Configuration);

var app = builder.Build();

app.UseCors();

var DATA_DATABASE = app.Configuration.GetConnectionString("DATA_DATABASE");
var IDENTITY_DATABASE = app.Configuration.GetConnectionString("IDENTITY_DATABASE");
var ENVIRONMENT_VARIABLE_FRONT_END_URL = app.Configuration["ENVIRONMENT_VARIABLE_FRONT_END_URL"];
Console.WriteLine($"DATA_DATABASE: {DATA_DATABASE}");
Console.WriteLine($"IDENTITY_DATABASE: {IDENTITY_DATABASE}");
Console.WriteLine($"ENVIRONMENT_VARIABLE_FRONT_END_URL: {ENVIRONMENT_VARIABLE_FRONT_END_URL}");


app.UseStaticFiles();

app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();

//// Verifica se o arquivo de marcação da primeira execução existe
//if (!File.Exists("blog_migration_marker.txt"))
//{
//    // Rodar as migrações automaticamente na primeira execução
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
//        await dbContext.Database.MigrateAsync();
//    }

//    // Cria um arquivo para marcar que as migrações já foram executadas
//    File.Create("blog_migration_marker.txt").Dispose();
//}


//// Verifica se o arquivo de marcação da primeira execução existe
//if (!File.Exists("identity_migration_marker.txt"))
//{
//    // Rodar as migrações automaticamente na primeira execução
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
//        await dbContext.Database.MigrateAsync();
//    }

//    // Cria um arquivo para marcar que as migrações já foram executadas
//    File.Create("identity_migration_marker.txt").Dispose();
//}


using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}


// Código original
var blogDbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<BlogDbContext>();

var anyTenancy = await blogDbContext.Tenancies.AnyAsync(x => x.Name == "Ozos");
if (!anyTenancy)
{
    var mediator1 = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();

    var tenancy = new CreateTenancyCommand
    {
        Name = "Ozos",
        AdministratorEmail = "contato@ozos.com.br",
        Url = "ozos.com.br"
    };

    var tenancyResult = await mediator1.Send(tenancy);
}




app.Run();
