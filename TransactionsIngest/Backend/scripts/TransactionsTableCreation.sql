CREATE TABLE IF NOT EXISTS Transactions (
    sourceSystem TEXT NOT NULL,
    transactionId INTEGER,
    cardNumber TEXT NOT NULL,
    locationCode TEXT NOT NULL,
    productName TEXT NOT NULL,
    amount REAL,
    transactionTime TEXT NOT NULL,
    transactionStatus TEXT NOT NULL,
    PRIMARY KEY (sourceSystem, transactionId)
);