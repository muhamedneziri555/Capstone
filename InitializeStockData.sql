-- Initialize stock data for existing products
-- This will create stock records for all three dimensions for each product

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








