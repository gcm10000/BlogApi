using BlogApi.API.IoC;
using BlogApi.Application.Infrastructure.Data.IoC;
using BlogApi.Application.Infrastructure.Identity.DataSeeders;
using BlogApi.Application.IoC;
using BlogApi.Application.Users.Commands.CreateUser;
using BlogApi.Infrastructure.Identity.IoC;
using MediatR;
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

using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}


//var user = new CreateUserCommand()
//{
//    Email = "contato@ozos.com.br",
//    Name = "Equipe Ozos",
//    Password = "Ozos@123456",
//    Role = "Administrator",

//};
//var mediator = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
//await mediator.Send(user);

app.Run();
