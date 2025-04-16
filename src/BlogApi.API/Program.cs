using BlogApi.API.IoC;
using BlogApi.API.Middlewares;
using BlogApi.Application.Auth.Commands.ChangePassword;
using BlogApi.Application.Infrastructure.Data.IoC;
using BlogApi.Application.Infrastructure.Identity.DataSeeders;
using BlogApi.Application.IoC;
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

using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedAsync();
}


//var tenancy = new CreateTenancyCommand
//{
//    Name = "Ozos"
//};

//var mediator1 = app.Services.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
//var tenancyResult = await mediator1.Send(tenancy);


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
