using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("PmsContext");

// Add services to the container.
builder.Services.AddDbContext<PmsapiContext>(opt =>
opt.UseMySql(connString, ServerVersion.AutoDetect(connString)));

builder.Services.AddControllers();
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
