CREATE TABLE IF NOT EXISTS History (
    entryId INTEGER PRIMARY KEY AUTOINCREMENT,
    sourceSystem TEXT NOT NULL,
    transactionId INTEGER,
    updatedField TEXT NOT NULL,
    newString TEXT NOT NULL,
    oldString TEXT NOT NULL,
    newDecimal REAL,
    oldDecimal REAL,
    updateTime TEXT NOT NULL
);