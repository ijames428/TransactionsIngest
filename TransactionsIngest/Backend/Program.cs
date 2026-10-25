using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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

// public class Transaction
// {
//     public string sourceSystem { get; set; } = string.Empty;
//     public int transactionId { get; set; }
//     public string cardNumber { get; set; } = string.Empty;
//     public string locationCode { get; set; } = string.Empty;
//     public string productName { get; set; } = string.Empty;
//     public decimal amount { get; set; }
//     public string transactionTime { get; set; } = string.Empty;
//     public string transactionStatus { get; set; } = string.Empty;
// }

// public class HistoryEntry
// {
//     public string sourceSystem { get; set; } = string.Empty;
//     public int transactionId { get; set; }
//     public string updatedField { get; set; } = string.Empty;
//     public string newString { get; set; } = string.Empty;
//     public string oldString { get; set; } = string.Empty;
//     public decimal newDecimal { get; set; }
//     public decimal oldDecimal { get; set; }
//     public string updateTime { get; set; } = string.Empty;
// }

app.MapPost("/api/import", async (ApplicationDbContext context, Transaction transaction) =>
{
    // Transaction? foundTransaction = await context.Transactions.FirstOrDefaultAsync(entry => entry.sourceSystem == transaction.sourceSystem && entry.transactionId == transaction.transactionId);
    // if (foundTransaction == null)
    // {
    //     // add to the database because there's no entry for this transaction.
    //     context.Transactions.Add(transaction);
    // }
    // else
    // {
    //     foundTransaction.amount = transaction.amount;
    // }
    // await context.SaveChangesAsync();
    // return Results.Created($"/products/{transaction.transactionId}", transaction);





    bool didAnyDatabasesChange = false;

    Transaction? foundTransaction = await context.Transactions.FirstOrDefaultAsync(entry => entry.sourceSystem == transaction.sourceSystem && entry.transactionId == transaction.transactionId);
    // Transaction? foundTransaction = await context.Transactions.FindAsync(new object[] { transaction.sourceSystem, transaction.transactionId });
    if (foundTransaction == null)
    {
        // add to the database because there's no entry for this transaction.
        context.Transactions.Add(transaction);

        didAnyDatabasesChange = true;
    }
    else
    {
        // record the changes for the transaction in the History table and update the transaction.
        HistoryEntry GetNewHistoryEntry(string updatedField)
        {
            HistoryEntry newHistoryEntry = new HistoryEntry();
            newHistoryEntry.sourceSystem = transaction.sourceSystem;
            newHistoryEntry.transactionId = transaction.transactionId;
            newHistoryEntry.updateTime = transaction.transactionTime;
            newHistoryEntry.updatedField = updatedField;

            didAnyDatabasesChange = true;

            return newHistoryEntry;
        }

        void AddHistoryEntry_String(string updatedField, string newString, string oldString)
        {
            HistoryEntry newHistoryEntry = GetNewHistoryEntry(updatedField);
            newHistoryEntry.newString = newString;
            newHistoryEntry.oldString = oldString;

            context.History.Add(newHistoryEntry);
        }

        void AddHistoryEntry_Decimal(string updatedField, decimal newDecimal, decimal oldDecimal)
        {
            HistoryEntry newHistoryEntry = GetNewHistoryEntry(updatedField);
            newHistoryEntry.newDecimal = newDecimal;
            newHistoryEntry.oldDecimal = oldDecimal;

            context.History.Add(newHistoryEntry);
        }

        if (transaction.cardNumber != foundTransaction.cardNumber) 
        {
            AddHistoryEntry_String(nameof(transaction.cardNumber), transaction.cardNumber, foundTransaction.cardNumber);
            foundTransaction.cardNumber = transaction.cardNumber;
        }
        if (transaction.locationCode != foundTransaction.locationCode) 
        {
            AddHistoryEntry_String(nameof(transaction.locationCode), transaction.locationCode, foundTransaction.locationCode);
            foundTransaction.locationCode = transaction.locationCode;
        }
        if (transaction.productName != foundTransaction.productName) 
        {
            AddHistoryEntry_String(nameof(transaction.productName), transaction.productName, foundTransaction.productName);
            foundTransaction.productName = transaction.productName;
        }
        if (transaction.amount != foundTransaction.amount) 
        {
            Console.WriteLine(transaction.amount);
            Console.WriteLine(foundTransaction.amount);
            AddHistoryEntry_Decimal(nameof(transaction.amount), transaction.amount, foundTransaction.amount);
            foundTransaction.amount = transaction.amount;
        }
    }
    
    if (didAnyDatabasesChange)
    {
        await context.SaveChangesAsync();
        return Results.Created($"/products/{transaction.transactionId}", transaction);
    }

    return Results.Ok("No update occurred.");
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
