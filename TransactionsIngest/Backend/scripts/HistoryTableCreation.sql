CREATE TABLE IF NOT EXISTS History (
    entryId INTEGER PRIMARY KEY AUTOINCREMENT,
    sourceSystem TEXT NOT NULL,
    transactionId INTEGER,
    updatedField TEXT NOT NULL,
    updatedString TEXT NOT NULL,
    updatedDecimal REAL,
    updateTime TEXT NOT NULL,
    FOREIGN KEY (sourceSystem, transactionId) REFERENCES Transactions(sourceSystem, transactionId)
);