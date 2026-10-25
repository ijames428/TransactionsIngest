using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

const string TRANSACTION_STATUS_IMPORTED = "imported";
const string TRANSACTION_STATUS_IMMUTABLE = "immutable";
const string TRANSACTION_STATUS_REVOKED = "revoked";

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

async Task<bool> ProcessTransactions(ApplicationDbContext context, SourceSystemImport sourceSystemImport)
{
    Task<bool>[] results = [];

    foreach (Transaction transaction in sourceSystemImport.transactions)
    {
        results.Append(ProcessTransaction(context, transaction));
    }

    await Task.WhenAll(results);

    return true;
}

HistoryEntry GetNewHistoryEntry(string updatedField, Transaction transaction)
{
    HistoryEntry newHistoryEntry = new HistoryEntry();
    newHistoryEntry.sourceSystem = transaction.sourceSystem;
    newHistoryEntry.transactionId = transaction.transactionId;
    newHistoryEntry.updateTime = transaction.transactionTime;
    newHistoryEntry.updatedField = updatedField;

    return newHistoryEntry;
}

async Task<bool> ProcessTransaction(ApplicationDbContext context, Transaction transaction)
{
    bool didAnyDatabasesChange = false;

    Transaction? foundTransaction = await context.Transactions.FirstOrDefaultAsync(entry => entry.sourceSystem == transaction.sourceSystem && entry.transactionId == transaction.transactionId);
    if (foundTransaction == null)
    {
        // add to the database because there's no entry for this transaction.
        transaction.transactionStatus = TRANSACTION_STATUS_IMPORTED;
        context.Transactions.Add(transaction);

        HistoryEntry newHistoryEntry = GetNewHistoryEntry(nameof(transaction.transactionStatus), transaction);
        newHistoryEntry.newString = TRANSACTION_STATUS_IMPORTED;
        context.History.Add(newHistoryEntry);

        didAnyDatabasesChange = true;
    }
    else if (foundTransaction.transactionStatus != TRANSACTION_STATUS_IMMUTABLE && foundTransaction.transactionStatus != TRANSACTION_STATUS_REVOKED)
    {
        // record the changes for the transaction in the History table and update the transaction.
        void AddHistoryEntry_String(string updatedField, string newString, string oldString)
        {
            HistoryEntry newHistoryEntry = GetNewHistoryEntry(updatedField, transaction);
            newHistoryEntry.newString = newString;
            newHistoryEntry.oldString = oldString;

            context.History.Add(newHistoryEntry);

            didAnyDatabasesChange = true;
        }

        void AddHistoryEntry_Decimal(string updatedField, decimal newDecimal, decimal oldDecimal)
        {
            HistoryEntry newHistoryEntry = GetNewHistoryEntry(updatedField, transaction);
            newHistoryEntry.newDecimal = newDecimal;
            newHistoryEntry.oldDecimal = oldDecimal;

            context.History.Add(newHistoryEntry);

            didAnyDatabasesChange = true;
        }

        DateTime transactionDateTime;
        DateTime.TryParse(transaction.transactionTime, out transactionDateTime);
        DateTime foundTransactionDateTime;
        DateTime.TryParse(foundTransaction.transactionTime, out foundTransactionDateTime);
        DateTime immutableTime = foundTransactionDateTime.AddDays(1.0);

        if (transactionDateTime > immutableTime)
        {
            HistoryEntry newHistoryEntry = GetNewHistoryEntry(nameof(transaction.transactionStatus), transaction);
            newHistoryEntry.oldString = foundTransaction.transactionStatus;
            newHistoryEntry.newString = TRANSACTION_STATUS_IMMUTABLE;
            context.History.Add(newHistoryEntry);

            foundTransaction.transactionStatus = TRANSACTION_STATUS_IMMUTABLE;

            didAnyDatabasesChange = true;
        }
        else
        {
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
                AddHistoryEntry_Decimal(nameof(transaction.amount), transaction.amount, foundTransaction.amount);
                foundTransaction.amount = transaction.amount;
            }
        }
    }
    
    if (didAnyDatabasesChange)
    {
        await context.SaveChangesAsync();
        return true;
    }

    return false;
}

async Task<bool> RevokeTransaction(ApplicationDbContext context, Transaction transaction)
{
    HistoryEntry? foundRevokedHistoryEntry = await context.History.FirstOrDefaultAsync(entry => entry.sourceSystem == transaction.sourceSystem && 
                                                                                                entry.transactionId == transaction.transactionId && 
                                                                                                entry.updatedField == nameof(transaction.transactionStatus) &&
                                                                                                entry.newString == TRANSACTION_STATUS_REVOKED);
    if (foundRevokedHistoryEntry != null)
    {
        return true;
    }

    Transaction? foundTransaction = await context.Transactions.FirstOrDefaultAsync(entry => entry.sourceSystem == transaction.sourceSystem && entry.transactionId == transaction.transactionId);
    if (foundTransaction != null)
    {
        HistoryEntry newHistoryEntry = GetNewHistoryEntry(nameof(transaction.transactionStatus), transaction);
        newHistoryEntry.oldString = foundTransaction.transactionStatus;
        newHistoryEntry.newString = TRANSACTION_STATUS_REVOKED;
        context.History.Add(newHistoryEntry);

        foundTransaction.transactionStatus = TRANSACTION_STATUS_REVOKED;
        
        await context.SaveChangesAsync();
    }

    return true;
}

async Task<bool> ProcessRevokedTransactions(ApplicationDbContext context, SourceSystemImport sourceSystemImport)
{
    string sourceSystem = sourceSystemImport.sourceSystem;
    Transaction[] transactions = sourceSystemImport.transactions;

    List<int> excludedKeys = [];
    foreach (Transaction transaction in transactions)
    {
        excludedKeys.Add(transaction.transactionId);
    }

    var cutoffTime = DateTime.UtcNow.AddHours(-24).ToString("O");
    
    List<Transaction> potentialRevokedTransactions = await context.Transactions
                            .FromSqlInterpolated($"SELECT * FROM Transactions WHERE datetime(transactionTime) >= {cutoffTime}")
                            .ToListAsync();

    List<Transaction> revokedTransactions = potentialRevokedTransactions
                            .Where(t => t.sourceSystem == sourceSystem && 
                                        !excludedKeys.Contains(t.transactionId) && 
                                        (t.transactionStatus != TRANSACTION_STATUS_REVOKED || t.transactionStatus != TRANSACTION_STATUS_IMMUTABLE))
                            .ToList();

    Task<bool>[] results = [];
    
    foreach (Transaction revokedTransaction in revokedTransactions)
    {
        results.Append(RevokeTransaction(context, revokedTransaction));
    }

    return true;
}

app.MapPost("/api/import", async (ApplicationDbContext context, SourceSystemImport sourceSystemImport) =>
{
    await ProcessRevokedTransactions(context, sourceSystemImport);
    await ProcessTransactions(context, sourceSystemImport);

    return Results.Ok("Import Complete");
});

app.MapPost("/api/manual_import", async (ApplicationDbContext context, Transaction transaction) =>
{
    bool success = await ProcessTransaction(context, transaction);
    if (success)
    {
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
    await context.Transactions.ExecuteDeleteAsync();
    await context.History.ExecuteDeleteAsync();
    await context.SaveChangesAsync();
    return Results.Ok("reset");
});

app.Run();
