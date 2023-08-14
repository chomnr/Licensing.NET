using License_Server;
using License_Server.Services.Licensing;
using Licensing_Server.Services.Licensing;
using Licensing_System;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MicrosoftSQL"))
);

builder.Services.AddScoped<ILicenseProvider, LicenseProvider>();
builder.Services.AddScoped<ILicenseProcessor, LicenseProcessor>();

// Add services to the containers
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
