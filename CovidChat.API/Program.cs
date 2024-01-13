using CovidChart.API.Hubs;
using CovidChart.API.Models;
using CovidChart.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CovidService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        //builder.AllowAnyOrigin();
        builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true);
        /*builder.WithOrigins("https://localhost:7119/").AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((hosts) => true);
         */
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr"));
});
builder.Services.AddSignalR();

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
app.UseCors("CorsPolicy");
app.MapHub<CovidHub>("/CovidHub");

app.Run();
