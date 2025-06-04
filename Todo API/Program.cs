using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using Todo.Application.Interfaces.Repositories;
using Todo.Application.Services;
using Todo.Domain.Entities;
using Todo.Domain.Interfaces;
using Todo.Domain.Interfaces.Services;
using Todo.Infrastructure;
using Todo.Infrastructure.Repositories;
using Todo_API.Extensions;
using Todo_API.Middlewares;


//Disable claim mapping as deafult xml 
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.CreateSwaggerDocumentation();

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = "roles";
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddRateLimiter();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IRepositoryBase<User, string>), typeof(RepositoryBase<User, string>));
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped(typeof(IRepositoryBase<TodoItem, int>), typeof(RepositoryBase<TodoItem, int>));

//Registering open generic type in DI
//builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>)); 


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoItemService, TodoItemService>();



builder.Services.AddControllers();


//Add database
builder.Services.AddDbContext<TodoAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDoc();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserCreationMiddleware>();

app.UseRateLimiter();

app.MapControllers();

app.Run();
