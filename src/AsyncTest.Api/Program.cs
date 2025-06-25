using AsyncTest.Api.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GetProductStandardAsync.DatabaseContext>();
builder.Services.AddScoped<GetProductStandardAsync.Repository>();
builder.Services.AddScoped<GetProductStandardAsync.Handler>();

builder.Services.AddScoped<GetProductOptimizedAsync.DatabaseContext>();
builder.Services.AddScoped<GetProductOptimizedAsync.Repository>();
builder.Services.AddScoped<GetProductOptimizedAsync.Handler>();

var app = builder.Build();

app.MapStandardGetProductEndpoint();
app.MapOptimizedGetProductEndpoint();

app.Run();
