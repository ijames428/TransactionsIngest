using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/transactions", async (ApplicationDbContext context) =>
    await context.Transactions.ToListAsync())
.WithName("GetTransactions");

app.MapPost("/transactions", async (ApplicationDbContext context, Transaction transaction) =>
{
    context.Transactions.Add(transaction);
    await context.SaveChangesAsync();
    return Results.Created($"/products/{transaction.TransactionId}", transaction);
})
.WithName("PostTransactions");

app.MapPost("/test", async (ApplicationDbContext context, Transaction transaction) =>
{
    Console.WriteLine("Test happened.");
})
.WithName("RunTest");

app.Run();
