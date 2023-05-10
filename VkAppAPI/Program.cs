using BusinessLayer.Implementations;
using BusinessLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authentication;
using VkAppAPI.Handler;
using Microsoft.EntityFrameworkCore;
using DataLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DataManager>(_ => new DataManager(
                                        new EFRepository<User>(new DataLayer.VkApiContext()),
                                        new EFRepository<UserGroup>(new DataLayer.VkApiContext()),
                                        new EFRepository<UserState>(new DataLayer.VkApiContext()),
                                        new EFRepository<UserWithFullInfo>(new DataLayer.VkApiContext()) //dependency injection
));

builder.Services.AddDbContext<VkApiContext>(options => options.UseNpgsql("Host=localhost;Port=5432;Database=VkDB;Username=postgres;Password=qwe123@"));
/*
builder.Services.AddAuthentication("BasicAuthHandler")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthHandler", null); // подключение BaseAuth*/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.UseCors(options =>
{
    options.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.Run();
