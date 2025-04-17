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

app.UseCors();


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


//using (var scope = app.Services.CreateScope())
//{
//    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
//    await roleSeeder.SeedAsync();

//    // Caminho do arquivo marker
//    var rolemarkerFilePath = Path.Combine("/app/migrations", "role_seeder_marker.txt");

//    // Verifica se o marker já foi criado
//    if (!File.Exists(rolemarkerFilePath))
//    {
//        // Cria o arquivo marker indicando que o processo foi concluído
//        File.WriteAllText(rolemarkerFilePath, "RoleSeeder process completed.");
//    }
//    else
//    {
//        // Caso já tenha sido criado, apenas loga a informação
//        Console.WriteLine("RoleSeeder marker already exists. Process already completed.");
//    }
//}


//// Código original
//var mediator1 = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();

//var tenancy = new CreateTenancyCommand
//{
//    Name = "Ozos",
//    Email = "contato@ozos.com.br",
//    Url = "ozos.com.br"
//};

//var tenancyResult = await mediator1.Send(tenancy);

//// Caminho do arquivo marker
//var tenancyMarkerFilePath = Path.Combine("/app/migrations", "tenancy_marker.txt");

//// Verifica se o marker já foi criado
//if (!File.Exists(tenancyMarkerFilePath))
//{
//    // Cria o arquivo marker indicando que o processo foi concluído
//    File.WriteAllText(tenancyMarkerFilePath, "Tenancy process completed.");
//}
//else
//{
//    // Caso já tenha sido criado, apenas loga a informação
//    Console.WriteLine("Tenancy marker already exists. Process already completed.");
//}

//var user = new CreateUserCommand()
//{
//    Email = "contato@ozos.com.br",
//    Name = "Equipe Ozos",
//    Password = "Ozos@123456",
//    Role = "Administrator",
//    TenancyDomainId = tenancyResult.Id
//};

//var s = new ChangePasswordCommand { NewPassword = "Ozos@123456", Username = "contato@ozos.com.br" };

//var mediator2 = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
//await mediator2.Send(s);

app.Run();
