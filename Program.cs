using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.Models;
using Microsoft.OpenApi.Models;
using PmsApi.Utilities;
using PmsApi.Services;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("PmsContext");

// Add services to the container.
//Database service
builder.Services.AddDbContext<PmsapiContext>(opt =>
opt.UseMySql(connString, ServerVersion.AutoDetect(connString)));

//Identity service
builder.Services.AddIdentity<User, Role>()
.AddEntityFrameworkStores<PmsapiContext>()
.AddRoles<Role>()
.AddApiEndpoints()
.AddDefaultTokenProviders();

//Authorization service
builder.Services.AddAuthorization(opt =>
    {
        opt.AddPolicy("IsAdmin", p => p.RequireRole(["Admin"]));
        opt.AddPolicy("IsSuperAdmin", p => p.RequireClaim("SuperAdmin"));
    }
);


//Automapper service
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //this option prevents recursivity from GET responses
});

builder.Services.AddScoped<IUserContextHelper, UserContextHelper>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    opt =>
    {
        opt.AddSecurityDefinition("oauth2",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            }
        );

        opt.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    []
                }
            }
        );
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//access endpoints
app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();