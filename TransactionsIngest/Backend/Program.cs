using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAngular",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAngular"); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/transactions", async (ApplicationDbContext context) =>
    await context.Transactions.ToListAsync());

app.MapPost("/api/import", async (ApplicationDbContext context, Transaction transaction) =>
{
    context.Transactions.Add(transaction);
    await context.SaveChangesAsync();
    return Results.Created($"/products/{transaction.transactionId}", transaction);
});

app.Run();
