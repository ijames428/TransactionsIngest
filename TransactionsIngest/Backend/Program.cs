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

app.MapPost("/api/import", async (ApplicationDbContext context, Transaction transaction) =>
{
    context.Transactions.Add(transaction);
    await context.SaveChangesAsync();
    return Results.Created($"/products/{transaction.transactionId}", transaction);
});

app.MapGet("/api/transactions", async (ApplicationDbContext context) =>
{
    var transactions = await context.Transactions.ToListAsync();
    return Results.Ok(transactions);
});

app.MapGet("/api/history", async (ApplicationDbContext context) =>
{
    var history_entries = await context.History.ToListAsync();
    return Results.Ok(history_entries);
});

app.MapPost("/api/reset", async (ApplicationDbContext context, Transaction transaction) =>
{
    Console.WriteLine("Reset");
    await context.Transactions.ExecuteDeleteAsync();
    await context.History.ExecuteDeleteAsync();
    await context.SaveChangesAsync();
    return Results.Ok("reset");
});

app.Run();
