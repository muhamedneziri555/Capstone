-- Create ProductStocks table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductStocks]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductStocks] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [ProductId] int NOT NULL,
        [Dimension] nvarchar(20) NOT NULL,
        [StockQuantity] int NOT NULL,
        [ReservedQuantity] int NOT NULL DEFAULT 0,
        [LastUpdated] datetime2 NOT NULL,
        CONSTRAINT [PK_ProductStocks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductStocks_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );

    -- Create index for better performance
    CREATE INDEX [IX_ProductStocks_ProductId] ON [ProductStocks] ([ProductId]);
    CREATE INDEX [IX_ProductStocks_Dimension] ON [ProductStocks] ([Dimension]);
END

-- Initialize stock data for existing products
INSERT INTO ProductStocks (ProductId, Dimension, StockQuantity, ReservedQuantity, LastUpdated)
SELECT 
    p.Id as ProductId,
    '120x170' as Dimension,
    10 as StockQuantity,  -- Default stock of 10 for each dimension
    0 as ReservedQuantity,
    GETDATE() as LastUpdated
FROM Products p
WHERE NOT EXISTS (
    SELECT 1 FROM ProductStocks ps 
    WHERE ps.ProductId = p.Id AND ps.Dimension = '120x170'
);

INSERT INTO ProductStocks (ProductId, Dimension, StockQuantity, ReservedQuantity, LastUpdated)
SELECT 
    p.Id as ProductId,
    '150x220' as Dimension,
    10 as StockQuantity,  -- Default stock of 10 for each dimension
    0 as ReservedQuantity,
    GETDATE() as LastUpdated
FROM Products p
WHERE NOT EXISTS (
    SELECT 1 FROM ProductStocks ps 
    WHERE ps.ProductId = p.Id AND ps.Dimension = '150x220'
);

INSERT INTO ProductStocks (ProductId, Dimension, StockQuantity, ReservedQuantity, LastUpdated)
SELECT 
    p.Id as ProductId,
    '200x290' as Dimension,
    10 as StockQuantity,  -- Default stock of 10 for each dimension
    0 as ReservedQuantity,
    GETDATE() as LastUpdated
FROM Products p
WHERE NOT EXISTS (
    SELECT 1 FROM ProductStocks ps 
    WHERE ps.ProductId = p.Id AND ps.Dimension = '200x290'
);








