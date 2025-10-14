SELECT COUNT(DISTINCT 
    CAST(T.Quantity * T.PricePerUnit  AS NVARCHAR(50))
)
FROM [db_dgf1].[dbo].[opt_Transactions] AS T
INNER JOIN dbo.opt_Bonds AS OB ON OB.Id = T.BondId
where OB.TypeID = 1

SELECT COUNT(DISTINCT 
    CAST(T.tedad * T.price AS NVARCHAR(50))
)
FROM [db_dgf1].[dbo].Khazaneh_Eslami_Kharid AS T
INNER JOIN dbo.Khazaneh_Eslami AS OB ON OB.id = T.fid_khazaneh_eslami;

SELECT 
    CAST(T.Quantity * T.PricePerUnit AS NVARCHAR(50)) + '_' + OB.Symbol AS VirtualKey,
    COUNT(*) AS NumOccurrences
FROM [db_dgf1].[dbo].[opt_Transactions] AS T
INNER JOIN dbo.opt_Bonds AS OB ON OB.Id = T.BondId
GROUP BY CAST(T.Quantity * T.PricePerUnit AS NVARCHAR(50)) + '_' + OB.Symbol
HAVING COUNT(*) > 1
ORDER BY NumOccurrences DESC;


  select quantity + 0 + PricePerUnit
  FROM [db_dgf1].[dbo].[opt_Transactions]

  select *
    FROM [db_dgf1].[dbo].[opt_Transactions] T
	INNER JOIN dbo.opt_Bonds AS OB ON OB.Id = T.BondId
	where TypeID = 1

	SELECT  COUNT(DISTINCT (
    T.tedad * T.price * T.price_kol - T.karmozd))
	FROM [db_dgf1].[dbo].Khazaneh_Eslami_Kharid AS T
	INNER JOIN dbo.Khazaneh_Eslami AS OB ON OB.id = T.fid_khazaneh_eslami;

	SELECT COUNT(DISTINCT 
    CAST(T.tedad AS NVARCHAR(50)) + '_' +
    CAST(T.price AS NVARCHAR(50)) + '_' +
    CAST(T.price_kol AS NVARCHAR(50)) + '_' +
    CAST(T.karmozd AS NVARCHAR(50)) + '_' +
    OB.sharh
)
FROM dbo.Khazaneh_Eslami_Kharid AS T
INNER JOIN dbo.Khazaneh_Eslami AS OB ON OB.id = T.fid_khazaneh_eslami;


SELECT 
    CAST(T.tedad AS NVARCHAR(50)) + '_' +
    CAST(T.price AS NVARCHAR(50)) + '_' +
    CAST(T.price_kol AS NVARCHAR(50)) + '_' +
    CAST(T.karmozd AS NVARCHAR(50)) + '_' +
    OB.sharh AS VirtualKey,
    COUNT(*) AS NumOccurrences
FROM dbo.Khazaneh_Eslami_Kharid AS T
INNER JOIN dbo.Khazaneh_Eslami AS OB ON OB.id = T.fid_khazaneh_eslami
GROUP BY 
    CAST(T.tedad AS NVARCHAR(50)) + '_' +
    CAST(T.price AS NVARCHAR(50)) + '_' +
    CAST(T.price_kol AS NVARCHAR(50)) + '_' +
    CAST(T.karmozd AS NVARCHAR(50)) + '_' +
    OB.sharh
HAVING COUNT(*) > 1
ORDER BY NumOccurrences DESC;


	select * from Khazaneh_Eslami_Kharid where price_kol < 0

	select* from opt_Bonds








	;WITH src AS (
    SELECT
        b.id AS BondId,
        dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))) AS TransactionDate,
        k.price_kol,
        ROW_NUMBER() OVER (
            PARTITION BY b.id, dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8)))
            ORDER BY k.datee DESC
        ) AS rn
    FROM dbo.Khazaneh_Eslami_Kharid AS k
    JOIN dbo.Khazaneh_Eslami AS old_b ON k.fid_khazaneh_eslami = old_b.id
    JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.TypeID = 1
)
UPDATE t
SET t.InvestmentPrice = src.price_kol
FROM dbo.opt_Transactions AS t
JOIN src ON t.BondId = src.BondId
        AND t.TransactionDate = src.TransactionDate
        AND src.rn = 1
WHERE t.TransactionType = 'Buy';




UPDATE t
SET t.InvestmentPrice = src.price_kol
FROM dbo.opt_Transactions AS t
JOIN (
    SELECT 
        b.id AS BondId,
        dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))) AS TransactionDate
        --MAX(k.price_kol) AS price_kol   -- or AVG(), MIN(), etc.
    FROM dbo.Khazaneh_Eslami_Kharid AS k
    JOIN dbo.Khazaneh_Eslami AS old_b ON k.fid_khazaneh_eslami = old_b.id
    JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.TypeID = 1
    --GROUP BY 
    --    b.id,
    --    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8)))
) AS src
  ON t.BondId = src.BondId
 AND t.TransactionDate = src.TransactionDate
WHERE t.TransactionType = 'Buy';



select *
--SET t.InvestmentPrice = src.price_kol
FROM dbo.opt_Transactions AS t
JOIN Khazaneh_Eslami_Kharid kk
on t.commission = kk.karmozd
and t.PricePerUnit = kk.price
and t.quantity = kk.tedad
and t.Status = kk.taeed
and t.BrokerId = kk.fid_kargozaran

SELECT 
    kk.karmozd, kk.price, kk.tedad, kk.taeed, kk.fid_kargozaran,
    COUNT(*) AS DuplicateCount
FROM Khazaneh_Eslami_Kharid kk
GROUP BY kk.karmozd, kk.price, kk.tedad, kk.taeed, kk.fid_kargozaran
HAVING COUNT(*) > 1;

select * from Khazaneh_Eslami_Kharid where price = 595100 and tedad = 500 and taeed = 2 and fid_kargozaran = 3

SELECT t.Id, COUNT(*) AS NumMatches
FROM dbo.opt_Transactions AS t
JOIN kk_dedup AS kk
  ON t.commission = kk.karmozd
 AND t.PricePerUnit = kk.price
 AND t.quantity = kk.tedad
 AND t.Status = kk.taeed
 AND t.BrokerId = kk.fid_kargozaran
GROUP BY t.Id
HAVING COUNT(*) > 1
ORDER BY NumMatches DESC;


WITH kk_dedup AS (
    SELECT 
        karmozd, price, tedad, taeed, fid_kargozaran,
        MAX(price_kol) AS price_kol  -- or whichever aggregate you need
    FROM Khazaneh_Eslami_Kharid
    GROUP BY karmozd, price, tedad, taeed, fid_kargozaran
)
update t
set t.InvestmentPrice = kk.price_kol
FROM dbo.opt_Transactions AS t
JOIN kk_dedup AS kk
  ON t.commission = kk.karmozd
 AND t.PricePerUnit = kk.price
 AND t.quantity = kk.tedad
 AND t.Status = kk.taeed
 AND t.BrokerId = kk.fid_kargozaran;



 select * from opt_Transactions
 select * from Khazaneh_Eslami_Kharid
