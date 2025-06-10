CREATE TABLE Users (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    FeePercentage DECIMAL(5,2) NOT NULL
);

CREATE TABLE Assets (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    Code VARCHAR(50) NOT NULL,
    Name VARCHAR(255) NOT NULL
);

CREATE TABLE Trades (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    AssetId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(15,4) NOT NULL,
    TradeType INT NOT NULL,
    Fee DECIMAL(15,4) NOT NULL,
    TradeTime DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (AssetId) REFERENCES Assets(Id),
    INDEX idx_trade_user_asset_time (UserId, AssetId, TradeTime),
    INDEX idx_trade_user (UserId)
);

CREATE TABLE Quotes (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    AssetId CHAR(36) NOT NULL,
    UnitPrice DECIMAL(15,4) NOT NULL,
    QuoteTime DATETIME NOT NULL,
    FOREIGN KEY (AssetId) REFERENCES Assets(Id),
    INDEX idx_quote_asset_time (AssetId, QuoteTime)
);

CREATE TABLE Positions (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    AssetId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    AvgPrice DECIMAL(15,4) NOT NULL,
    PnL DECIMAL(15,4) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (AssetId) REFERENCES Assets(Id)
);

CREATE TABLE Dividends (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    AssetId CHAR(36) NOT NULL,
    DividendType INT NOT NULL,
    ValuePerShare DECIMAL(15,4) NOT NULL,
    ExDate DATE NOT NULL,
    PaymentDate DATE NOT NULL,
    FOREIGN KEY (AssetId) REFERENCES Assets(Id)
);
