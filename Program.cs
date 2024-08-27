using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("PmsContext");

// Add services to the container.

//Identity Auth service
builder.Services.AddIdentity<User, Role>()
.AddEntityFrameworkStores<PmsapiContext>()
.AddApiEndpoints()
.AddDefaultTokenProviders();

//Database service
builder.Services.AddDbContext<PmsapiContext>(opt =>
opt.UseMySql(connString, ServerVersion.AutoDetect(connString)));

//Automapper service
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //this option prevents recursivity from GET responses
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();