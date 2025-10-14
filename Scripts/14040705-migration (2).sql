-- *** STEP 1: Populate opt_BondTypes and opt_Brokers ***
-- پر کردن جداول پایه
SET IDENTITY_INSERT dbo.opt_BondTypes ON;
INSERT INTO dbo.opt_BondTypes (id, name, has_coupon) VALUES
(1, N'اسناد خزانه اسلامی', 0),
(2, N'خزانه کوپن', 1),
(3, N'اوراق اجاره دولتی', 1),
(4, N'اوراق مرابحه', 1),
(5, N'اوراق مشارکت', 0)
SET IDENTITY_INSERT dbo.opt_BondTypes OFF;
GO

--delete from opt_BondTypes

SET IDENTITY_INSERT dbo.opt_Brokers ON;
INSERT INTO dbo.opt_Brokers(id, name)
SELECT id, sharh FROM dbo.Kargozaran; -- Suffix _Top10 removed
SET IDENTITY_INSERT dbo.opt_Brokers OFF;
GO

--delete from opt_Brokers

-- *** STEP 2 (Final & Robust Version): Migrate Master Bond Data into 'opt_Bonds' Table ***
-- این نسخه پیشرفته، نمادهای تکراری را شناسایی کرده و برایشان نماد جدید و یکتا می‌سازد

-- استفاده از CTE برای جمع‌آوری و شماره‌گذاری تمام اوراق
;WITH AllBonds AS (
    -- Step A: Gather all bonds from old tables into one unified list
    -- مرحله الف: تمام اوراق از جداول قدیمی را در یک لیست واحد جمع‌آوری می‌کنیم
    SELECT
        1 AS type_id,
        sharh,
        TRY_CONVERT(DATE, STUFF(STUFF(CAST(datee AS VARCHAR(8)), 5, 0, '/'), 8, 0, '/')) AS maturity_date,
        'Khazaneh_Eslami' AS source_table, -- نام جدول منبع برای ساخت کلید یکتا
        id AS source_id -- آی‌دی رکورد در جدول منبع
    FROM dbo.Khazaneh_Eslami
    UNION ALL
    SELECT
        2 AS type_id,
        sharh, 
        TRY_CONVERT(DATE, STUFF(STUFF(CAST(datee AS VARCHAR(8)), 5, 0, '/'), 8, 0, '/')),
        'Khazaneh_Copon',
        id
    FROM dbo.Khazaneh_Copon
    UNION ALL
    SELECT
        3 AS type_id,
        sharh, 
        TRY_CONVERT(DATE, STUFF(STUFF(CAST(datee AS VARCHAR(8)), 5, 0, '/'), 8, 0, '/')),
        'Ejare_Dulat', 
        id
    FROM dbo.Ejare_Dulat
    UNION ALL
    SELECT
        4 AS type_id, 
        sharh,
        TRY_CONVERT(DATE, STUFF(STUFF(CAST(datee AS VARCHAR(8)), 5, 0, '/'), 8, 0, '/')),
        'Morabehe',
        id
    FROM dbo.Morabehe
    union  ALL
     SELECT
        5 AS type_id,
        onvan,
        TRY_CONVERT(DATE, STUFF(STUFF(CAST(date_sarresid AS VARCHAR(8)), 5, 0, '/'), 8, 0, '/')),
        'Mosharekat',
        id
    FROM dbo.Mosharekat
),
NumberedBonds AS (
    -- Step B: Assign a row number to each bond based on its 'sharh'. Duplicates will get rn > 1
    -- مرحله ب: به ازای هر نماد تکراری، یک شماره ردیف اختصاص می‌دهیم. رکوردهای تکراری شماره بزرگتر از 1 می‌گیرند
    SELECT
        *,
        ROW_NUMBER() OVER(PARTITION BY sharh ORDER BY source_id) AS rn
    FROM AllBonds
)

-- Step C: Insert into the new table, creating a new symbol if it's a duplicate
-- مرحله ج: درج در جدول جدید. اگر رکورد تکراری بود (rn > 1)، یک نماد جدید برایش می‌سازیم
INSERT INTO dbo.opt_Bonds (type_id, symbol, maturity_date)
SELECT
    type_id,
    CASE
        WHEN rn = 1 THEN sharh -- If it's the first occurrence, use the original symbol
        ELSE sharh + N'_' + source_table + N'_' + CAST(source_id AS NVARCHAR(20)) -- If it's a duplicate, create a new unique symbol
    END AS symbol,
    maturity_date
FROM NumberedBonds;
GO

select * from opt_Bonds
--===============================================================================
--SELECT *
--FROM opt_Bonds
--WHERE symbol LIKE '%\_%' ESCAPE '\';--رکورد های یونیک با symbol name متفاوت
--==-=-=============================================================================
--SELECT * FROM Khazaneh_Eslami WHERE ID = 20090

--select * from Khazaneh_Eslami_Kharid WHERE fid_khazaneh_eslami not in( 20055, 20052, 20077, 40119, 40121, 50181, 20090) and id not in 
--(
--select distinct kharidID from ##temp
--)--اونایی که ایدی خزانع خریدشون وجود خارجی نداره

--select * from Khazaneh_Eslami where id in ( select distinct fid_khazaneh_eslami from ##temp)

go
--=================================اینزرت دیتا تو جدول تمپ===========
--drop table if exists ##temp
--SELECT
--    k.id as kharidID,
--    k.fid_khazaneh_eslami as fid_khazaneh_eslami,
--    b.id as BondId,
--    k.fid_kargozaran as fid_kargozaran,
--    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8)))  as datet,
--    'Buy' as TransactionType, -- Transaction Type: Buy
--    k.tedad as tedad,
--    k.price as price,
--    k.karmozd as karmozd,
--    NULL as ytm -- YTM is not available in this old table
--    INTO ##temp
--FROM dbo.Khazaneh_Eslami_Kharid AS k
--JOIN dbo.Khazaneh_Eslami AS old_b 
--    ON k.fid_khazaneh_eslami = old_b.id
--JOIN dbo.opt_Bonds AS b 
--    ON b.symbol = old_b.sharh 
--   AND b.type_id = 1
--WHERE k.fid_khazaneh_eslami NOT IN (20055, 20052, 20077, 40119, 40121, 50181, 20090);
--=================================================================================
go
--==============================================================================
--select * from Khazaneh_Eslami_Kharid WHERE fid_khazaneh_eslami in( 20055, 20052, 20077, 40119, 40121, 50181, 20090) --74038
--select * from  dbo.Khazaneh_Copon_Foroush --3
--select * from  dbo.Khazaneh_Copon_Kharid --6
--===============================================================================
--select *
----RIGHT(
----            B.symbol, 
----            CHARINDEX('_', REVERSE(B.symbol) + '_') - 1
----        ) 
--        from opt_Bonds B
--        where LEN(B.symbol) - LEN(REPLACE(B.symbol, '_', '')) = 3--اوراق با symbola-name  خاص
--=====================================================================================
-- *** STEP 3 (Corrected): Migrate ALL transactions into the single 'opt_Transactions' Table ***
-- در این نسخه، با استفاده از COALESCE جلوی خطای NULL برای تاریخ گرفته می‌شود
--======================================================================================
INSERT INTO dbo.opt_Transactions (BondId,BrokerId,TransactionDate,TransactionType,Quantity,PricePerUnit,Commission, YtmAtTransaction, InvestmentPrice)
-- All Purchases
SELECT
    --distinct(k.id),
    b.id,
    k.fid_kargozaran,
    -- Using COALESCE to provide a default date if the conversion fails
    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))),
    'Buy', -- Transaction Type: Buy
    k.tedad,
    k.price,
    k.karmozd,
    NULL, -- YTM is not available in this old table
	k.price_kol
FROM dbo.Khazaneh_Eslami_Kharid AS k
JOIN dbo.Khazaneh_Eslami AS old_b ON k.fid_khazaneh_eslami = old_b.id
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 1
 and k.fid_khazaneh_eslami not in( 20055, 20052, 20077, 40119, 40121, 50181, 20090) 
 --7 مورد وجود از خزانه خرید اسلامی وجود خارجی نداشت
UNION ALL
SELECT
--distinct(k.id),
    b.id,
    k.fid_kargozaran,
    -- Using COALESCE to provide a default date if the conversion fails
   dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))) ,
    'Buy', -- Transaction Type: Buy
    k.tedad,
    k.price,
    k.karmozd,
    NULL, -- YTM is not available in this old table
	k.price_kol
FROM dbo.Khazaneh_Eslami_Kharid AS k
--JOIN dbo.Khazaneh_Eslami AS old_b ON k.fid_khazaneh_eslami = old_b.id
JOIN dbo.opt_Bonds AS b 
ON b.type_id = 1
AND TRY_CAST(
        RIGHT(
            B.symbol, 
            CHARINDEX('_', REVERSE(B.symbol) + '_') - 1
        ) AS BIGINT
      ) = K.fid_khazaneh_eslami
  where LEN(B.symbol) - LEN(REPLACE(B.symbol, '_', '')) = 3
  union all
SELECT
    b.id,
    k.fid_kargozaran,
    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))) ,
    'Buy', -- Transaction Type: Buy
    k.tedad,
    k.price,
    k.karmozd,
    k.ytm,
		k.price_kol
FROM dbo.Khazaneh_Copon_Kharid AS k
JOIN dbo.Khazaneh_Copon AS old_b ON k.fid_khazaneh_Copon = old_b.id
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 2

-- All Sales (now appended to the same table)
UNION ALL
SELECT
    b.id,
    s.fid_kargozaran,
   dbo.ShamsiToGregorian(CAST(s.datee AS CHAR(8))) ,
    'Sell', -- Transaction Type: Sell
    s.tedad,
    s.price,
    s.karmozd,
    s.ytm,
		k.price_kol
FROM dbo.Khazaneh_Copon_Foroush AS s
JOIN dbo.Khazaneh_Copon AS old_b ON s.fid_khazaneh_copon = old_b.id
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 2
go
--------------------------------------------------------------------------------------------------------------------
-- NEW Migrations (Ejare Dulat, Morabehe, Mosharekat Purchases)
INSERT INTO dbo.opt_Transactions (BondId,BrokerId,TransactionDate,TransactionType,Quantity,PricePerUnit,Commission, YtmAtTransaction, InvestmentPrice)
-- All Purchases (Ejare Dulat)
SELECT
    b.id,
    NULL AS broker_id, -- Not available in Ejare_Dulat_Kharid
    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))),
    'Buy', -- Transaction Type: Buy
    k.tedad,
    k.price AS price_per_unit,
    k.karmozd AS commission,
    NULL AS ytm_at_transaction, -- Not available
		k.price_kol
FROM dbo.Ejare_Dulat_Kharid AS k
-- ASSUMPTION: Joining to an old bond table named Ejare_Dulat
JOIN dbo.Ejare_Dulat AS old_b ON k.fid_ejare_dulat = old_b.id 
-- ASSUMPTION: Joining to opt_Bonds on symbol and a specific type_id (e.g., type_id = 3)
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 3 -- NOTE: Use the correct type_id for Ejare Dulat Bonds
--دیتا ندارد
UNION ALL
-- All Purchases (Morabehe)
SELECT
    b.id,
    NULL AS broker_id, -- Not available in Morabehe_Kharid
    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))),
    'Buy', -- Transaction Type: Buy
    k.tedad,
    k.price AS price_per_unit,
    k.karmozd AS commission,
    NULL AS ytm_at_transaction, -- Not available
		k.price_kol
FROM dbo.Morabehe_Kharid AS k
-- ASSUMPTION: Joining to an old bond table named Morabehe
JOIN dbo.Morabehe AS old_b ON k.fid_Morabehe = old_b.id 
-- ASSUMPTION: Joining to opt_Bonds on symbol and a specific type_id (e.g., type_id = 4)
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 4 -- NOTE: Use the correct type_id for Morabehe Bonds
--یک مورد با fid_morabehe = 140006 در جدول مرابحه وجود ندارد
--select * from Morabehe
--select * from Morabehe_Kharid


--UNION ALL
---- All Purchases (Mosharekat)
--SELECT
--    b.id,
--    NULL AS broker_id, -- Not available in Mosharekat_Kharid
--    dbo.ShamsiToGregorian(CAST(k.datee AS CHAR(8))),
--    'Buy', -- Transaction Type: Buy
--    -- NOTE: Mosharekat_Kharid schema does NOT have 'tedad' (quantity) or 'karmozd' (commission).
--    -- Assuming a default quantity of 1 for now, but this needs confirmation.
--    1 AS quantity, -- ASSUMPTION: Using 1 as quantity; needs confirmation on how quantity is stored.
--    k.price AS price_per_unit,
--    NULL AS commission, -- Not available
--    NULL AS ytm_at_transaction -- Not available
--FROM dbo.Mosharekat_Kharid AS k
---- ASSUMPTION: Joining to an old bond table named Mosharekat
--JOIN dbo.Mosharekat AS old_b ON k.fid_mosharekat = old_b.id 
---- ASSUMPTION: Joining to opt_Bonds on symbol and a specific type_id (e.g., type_id = 5)
--JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.onvan AND b.type_id = 5 -- NOTE: Use the correct type_id for Mosharekat Bonds

--------------------------------------------------------------------------------------------------------------------

GO

-- *** STEP 4: Migrate Coupon Payment Data into 'opt_CouponPayments' ***
INSERT INTO dbo.opt_CouponPayments (bond_id, payment_date, amount_per_unit)
SELECT
    b.id,
   dbo.ShamsiToGregorian(CAST(cp.datee AS CHAR(8))),
    cp.value
FROM dbo.Khazaneh_Copon_Paydate AS cp -- Suffix _Top10 removed
JOIN dbo.Khazaneh_Copon AS old_b ON cp.fid_khazaneh_copon = old_b.id
JOIN dbo.opt_Bonds AS b ON b.symbol = old_b.sharh AND b.type_id = 2;
GO

PRINT 'Migration to the final, optimized schema (opt_ tables) is complete!';
GO

--select * FROM dbo.Khazaneh_Copon_Paydate 


----------------------------------------------------Renaming Column Name(Pascal-Case)--------------------------------------------
ALTER TABLE dbo.opt_Transactions 
DROP CONSTRAINT CHK_opt_TransactionType;

-- Step 2: Rename the column
EXEC sp_rename 'dbo.opt_Transactions.transaction_type', 'TransactionType', 'COLUMN';

-- Step 3: Recreate the check constraint (with new name if you want)
ALTER TABLE dbo.opt_Transactions
ADD CONSTRAINT CHK_opt_Transaction_Type 
CHECK (TransactionType IN ('Sell', 'Buy'));
---------------------------------------------------------Update MaturityDate In [opt_Bonds] ---------------------------------------------------------------------

  update [opt_Bonds]
  set MaturityDate = dbo.ShamsiToGregorian(REPLACE(MaturityDate, '-', ''))
  where Id != 1
  -------------------------------------------------------------------------------------------------------------------------------------------------------------
  --------------------------------------------------------Add-InvestmentPrice-To-NewTables----------------------------------------------------------------------------------
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
	 AND t.BrokerId = kk.fid_kargozara
 -----------------------------------------------------------------------------------------------------------------------------------------------------------------