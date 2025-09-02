using DeveloperHub.API.Extensions;
using DeveloperHub.Application.Extensions;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Application.Mappings;
using DeveloperHub.Application.Validators;
using DeveloperHub.Infrastructure;
using DeveloperHub.Infrastructure.Extensions;
using DeveloperHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "DeveloperHub API",
		Version = "v1",
		Description = "API for managing developer projects and collaboration"
	});

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization header using the Bearer scheme"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
		{
			new OpenApiSecurityScheme {
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

//CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

var configuration = builder.Configuration;

// Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);


// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtConfig["Issuer"],
		ValidAudience = jwtConfig["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});

// Authorization
builder.Services.AddAuthorization();

// HttpContext
builder.Services.AddHttpContextAccessor();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// SendGrid Email Service
builder.Services.AddSingleton<IEmailService>(sp =>
	new SendGridEmailService(
		builder.Configuration["SendGrid:ApiKey"]!,
		builder.Configuration["SendGrid:FromEmail"]!,
		builder.Configuration["SendGrid:FromName"]!
	)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCustomExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<DeveloperHubDbContext>();

	context.Database.Migrate();
}

app.Run();