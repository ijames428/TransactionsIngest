CREATE TABLE IF NOT EXISTS transactions (
    sourceSystem TEXT NOT NULL,
    transactionID INTEGER,
    cardNumber TEXT NOT NULL,
    locationCode TEXT NOT NULL,
    productName TEXT NOT NULL,
    amount REAL,
    transactionTime, TEXT NOT NULL
    PRIMARY KEY (sourceSystem, transactionID)
);