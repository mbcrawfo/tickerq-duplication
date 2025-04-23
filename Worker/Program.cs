using Microsoft.EntityFrameworkCore;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TickerQDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddTickerQ(options =>
{
    options.AddOperationalStore<TickerQDbContext>();
});

var app = builder.Build();

app.UseTickerQ();

await app.RunAsync();
