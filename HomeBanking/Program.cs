using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<HomeBankingContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

//autenticaci�n
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
          options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
          options.LoginPath = new PathString("/index.html");
      });

//autorizaci�n
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy => 
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Admin") ||
            context.User.HasClaim(c => c.Type == "Client"));
    });

    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    //Aqui obtenemos todos los services registrados en la App
    var services = scope.ServiceProvider;
    try
    {
        // En este paso buscamos un service que este con la clase HomeBankingContext
        var context = services.GetRequiredService<HomeBankingContext>();
        DbInitializer.Initialize(context); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la informaci�n a la base de datos!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthentication();    

app.UseAuthorization();

app.Run();
