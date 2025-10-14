SELECT TOP (1000) [Id]
      ,[name]
      ,[has_coupon]
  FROM [db_dgf1].[dbo].[opt_BondTypes]


  SELECT kh_kh.id,kh_kh.fid_khazaneh_eslami,kh.sharh+'-'+kh.date_s as sharh_khazaneh , kh_kh.datee, kh_kh.date_s, kh_kh.price, kh_kh.karmozd, kh_kh.tedad, kh_kh.price_kol, k.sharh as sharh_k
  FROM Khazaneh_Eslami kh , Khazaneh_Eslami_Kharid kh_kh , kargozaran k 
  where k.id = kh_kh.fid_kargozaran and kh.taeed = 2 and kh.id = kh_kh.fid_Khazaneh_Eslami and kh_kh.taeed = 2 and kh_kh.fid_khazaneh_eslami = 3

  SELECT kh_kh.id,kh_kh.fid_khazaneh_eslami,kh.sharh+'-'+kh.date_s as sharh_khazaneh , kh_kh.datee, kh_kh.date_s, kh_kh.price, kh_kh.karmozd, kh_kh.tedad, kh_kh.price_kol, k.sharh as sharh_k 
  FROM Khazaneh_Eslami kh , Khazaneh_Eslami_Kharid kh_kh , kargozaran k  
  where k.id = kh_kh.fid_kargozaran and kh.taeed = 2 and kh.id = kh_kh.fid_Khazaneh_Eslami
  and kh_kh.taeed = 2 and kh_kh.fid_khazaneh_eslami =50163 and kh_kh.datee >=14040520 and kh_kh.datee <=14040720 order by kh_kh.datee desc

  --ehsan
  SELECT distinct kh.id , kh.sharh , kh.date_s , kh.datee  
  FROM khazaneh_eslami kh , khazaneh_eslami_kharid kh_kh , kargozaran k 
  where  kh.taeed = 2 and kh.id = kh_kh.fid_khazaneh_eslami and kh_kh.taeed=2 and kh_kh.datee >=13900503 and kh_kh.datee <=14100719 and kh.id =3 
  order by kh.datee
  -----------------
  SELECT kh_kh.id,kh_kh.fid_khazaneh_eslami,kh.sharh+'-'+kh.date_s as sharh_khazaneh , kh_kh.datee, kh_kh.date_s, kh_kh.price, kh_kh.karmozd, kh_kh.tedad, kh_kh.price_kol, k.sharh as sharh_k 
  FROM Khazaneh_Eslami kh , Khazaneh_Eslami_Kharid kh_kh , kargozaran k
  where k.id = kh_kh.fid_kargozaran and kh.taeed = 2 and kh.id = kh_kh.fid_Khazaneh_Eslami and kh_kh.taeed = 2 and kh_kh.fid_khazaneh_eslami =3 and kh_kh.datee >=13900503 and kh_kh.datee <=14100719
  order by kh_kh.datee asc
  ---------------ChatGpt-Completed-------------------
  SELECT 
    kh_kh.id,
    kh_kh.fid_khazaneh_eslami,
    kh.sharh + '-' + kh.date_s AS sharh_khazaneh,
    kh_kh.datee,
    kh_kh.date_s,
    kh_kh.price AS Price_Buy,
    kh_kh.karmozd,
    kh_kh.tedad,
    kh_kh.price_kol,
    k.sharh AS Broker_Name,

    -- ???? ???????????? ???
    (kh_kh.price * kh_kh.tedad) AS InvestedAmount,

    ---- ????? ?????? ?????? (difference between kh.date_s (maturity) and kh_kh.date_s (buy))
    DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)) AS CalendarDays,

    -- ????? ???? (simple yield)
    CASE WHEN kh_kh.price <> 0 
         THEN ROUND(((1000000 - (kh_kh.price * kh_kh.tedad)) / (kh_kh.price * kh_kh.tedad)) * 100, 2)
         ELSE 0 END AS SimpleYield,

    -- YTM (annualized yield)
    CASE WHEN DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)) > 0 AND kh_kh.price <> 0
         THEN ROUND((POWER((1000000.0 / (kh_kh.price * kh_kh.tedad)), (365.0 / DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)))) - 1) * 100, 2)
         ELSE 0 END AS YTM,
		 (kh_kh.tedad * 1000000) AS AmountAtMaturity
FROM 
    Khazaneh_Eslami kh
    INNER JOIN Khazaneh_Eslami_Kharid kh_kh ON kh.id = kh_kh.fid_Khazaneh_Eslami
    INNER JOIN kargozaran k ON k.id = kh_kh.fid_kargozaran
WHERE 
    kh.taeed = 2
    AND kh_kh.taeed = 2
    AND kh_kh.fid_khazaneh_eslami = 3
    AND kh_kh.datee BETWEEN 13900503 AND 14100719
ORDER BY 
    kh_kh.datee ASC;
-----------------------------------------------------------------------------------
SELECT 
    kh_kh.id AS [?????],  -- ID
    kh_kh.fid_khazaneh_eslami AS [?? ????? ??????], 
    kh.sharh + '-' + kh.date_s AS [??? ?????], 
    kh_kh.datee AS [????? ????],
    kh_kh.date_s AS [????? ???? (???)],
    kh.date_s AS [????? ??????],
    k.sharh AS [???????],

    -- ?? ???? ????????????????
    (kh_kh.price * kh_kh.tedad) AS [???? ????????????????],

    -- ?? ????? ????
    kh_kh.tedad AS [????? ????],

    -- ?? ???? ????
    kh_kh.price AS [???? ????],

    -- ?? ???? ?? ??????
    (kh_kh.tedad * 1000000) AS [???? ?? ??????],

    -- ?? ????? ?????? ?????? ??? ???? ? ??????
    DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)) AS [????? ?????? ??????],

	-- ????? ???? ?????? (%)
	CASE 
		WHEN kh_kh.price > 0 
			 AND DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)) > 0 
		THEN 
			ROUND(
				((1000000.0 - kh_kh.price) / kh_kh.price) 
				* (365.0 / DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee))) 
				* 100, 
			2)
		ELSE 0 
	END AS [????? ???? (%)],

    -- ?? YTM (????? ??????)
    CASE 
        WHEN kh_kh.price > 0 AND DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)) > 0 THEN 
            ROUND(
                (POWER((1000000.0 / kh_kh.price), (365.0 / DATEDIFF(DAY, dbo.ShamsiToGregorian(kh_kh.datee), dbo.ShamsiToGregorian(kh.datee)))) - 1) * 100,
            2)
        ELSE 0 
    END AS [?????? ?? ?????? (YTM %)],

    -- ?? ??????
    kh_kh.karmozd AS [??????],

    -- ?? ???? ?? (from table)
    kh_kh.price_kol AS [???? ?? ????]

FROM 
    dbo.Khazaneh_Eslami kh
    INNER JOIN dbo.Khazaneh_Eslami_Kharid kh_kh 
        ON kh.id = kh_kh.fid_Khazaneh_Eslami
    INNER JOIN dbo.kargozaran k 
        ON k.id = kh_kh.fid_kargozaran
WHERE 
    kh.taeed = 2
    AND kh_kh.taeed = 2
    AND kh_kh.fid_khazaneh_eslami = 3  -- ?? Filter by Khazaneh ID
    --AND kh_kh.datee BETWEEN 13900503 AND 14100719  -- ?? Shamsi dates (you may need to convert)
ORDER BY 
    kh_kh.datee ASC;
------------------------------------------------New Table--------------------------------------------------------
USE db_dgf1;
GO

SELECT 
    t.Id AS [شناسه],
    b.Id AS [کد اوراق],
    b.Symbol AS [نماد],
    b.IssueDate AS [تاریخ انتشار],
    b.MaturityDate AS [تاریخ سررسید],
    br.Name AS [کارگزار],
    
 
    -- 💰 مبلغ سرمایه‌گذاری‌شده
    (t.PricePerUnit * t.Quantity) AS 'مبلغ سرمایه‌گذاری‌شده',

    -- 📄 تعداد برگه
    t.Quantity AS 'تعداد برگه',

     -- 💵 قیمت خرید
    t.PricePerUnit AS 'قیمت خرید',

   -- 📈 مبلغ در سررسید
    (t.Quantity * b.FaceValue) AS 'مبلغ در سررسید',

     -- 📅 تعداد روزهای تقویمی بین خرید و سررسید
    DATEDIFF(DAY, t.TransactionDate, b.MaturityDate) AS [????? ?????? ??????],

    -- 🧮 بازده ساده (%)
    CASE 
        WHEN t.PricePerUnit > 0 AND DATEDIFF(DAY, t.TransactionDate, b.MaturityDate) > 0 THEN 
            ROUND(
                ((b.FaceValue - t.PricePerUnit) / t.PricePerUnit) 
                * (365.0 / DATEDIFF(DAY, t.TransactionDate, b.MaturityDate)) 
                * 100, 
            2)
        ELSE 0 
    END AS 'بازده ساده (%)',

    -- 📊 YTM (بازده سالانه)
    CASE 
        WHEN t.PricePerUnit > 0 AND DATEDIFF(DAY, t.TransactionDate, b.MaturityDate) > 0 THEN 
            ROUND(
                (POWER((b.FaceValue / t.PricePerUnit), (365.0 / DATEDIFF(DAY, t.TransactionDate, b.MaturityDate))) - 1) * 100,
            2)
        ELSE 0 
    END AS 'YTM (بازده سالانه)',

     -- 💸 کارمزد
    t.Commission AS 'کارمزد',

    -- 🏦 مبلغ کل (from table)
    (t.PricePerUnit * t.Quantity + t.Commission) AS ' مبلغ کل',

     -- 🔍 وضعیت تراکنش
    CASE 
        WHEN t.Status = 1 THEN N'در حال بررسی'
        WHEN t.Status = 2 THEN N'تأیید شده'
        WHEN t.Status = 3 THEN N'رد شده'
        ELSE N'نامشخص'
    END AS [وضعیت],

    -- 📆 نوع تراکنش
    t.TransactionType AS [??? ??????]

FROM 
    dbo.opt_Transactions t
    INNER JOIN dbo.opt_Bonds b ON b.Id = t.BondId
    LEFT JOIN dbo.opt_Brokers br ON br.Id = t.BrokerId

WHERE 
    t.TransactionType = 'Buy'
    AND t.Status = 2
    AND b.TypeID = 1   -- ?? ????? ??? ????? (????? fid_khazaneh_eslami)
    --AND t.TransactionDate BETWEEN '2025-08-12' AND '2026-07-09' -- ?????? ????? 1401/01/03 ?? 1405/04/18
	AND t.BondId = 9
	--and Quantity = 3600
ORDER BY 
    t.TransactionDate ASC;

------------------------------------------------New Table--------------------------------------------------------

SELECT 
            [Khazaneh_Eslami_Kharid].[datee] as [investmentDate],
            [Khazaneh_Eslami_Kharid].[date_s] as [investmentDateString] ,
            [Khazaneh_Eslami_Kharid].[price], 
            [Khazaneh_Eslami_Kharid].[karmozd],
            [Khazaneh_Eslami_Kharid].[tedad], 
            [Khazaneh_Eslami_Kharid].[price_kol],
            [Khazaneh_Eslami_Kharid].[taeed],
            [Khazaneh_Eslami].[sharh] AS [sharh],
            [Khazaneh_Eslami].[datee] AS [sareResid], 
            [Khazaneh_Eslami].[date_s] AS [sareResidString],
            [Khazaneh_Eslami].[taeed] AS [taeed],
            [Kargozaran].[sharh] AS kargozar
            FROM [Khazaneh_Eslami_Kharid] AS [Khazaneh_Eslami_Kharid] 
            JOIN [Khazaneh_Eslami] AS [Khazaneh_Eslami] ON [Khazaneh_Eslami_Kharid].[fid_khazaneh_eslami] = [Khazaneh_Eslami].[id] 
            JOIN kargozaran on Kargozaran.id = Khazaneh_Eslami_Kharid.fid_kargozaran
            --WHERE [Khazaneh_Eslami].[datee] LIKE N'%${date}%' AND  [Khazaneh_Eslami].[sharh] LIKE N'%${name}%'  AND [Kargozaran].[sharh] LIKE N'%${broker}%'
            ORDER BY [Khazaneh_Eslami_Kharid].[datee] DESC;

			select 

  select *
  from farabource 
  where noe = 1 
  and datee = {this.txt_Date_Final.Text.Replace("/", '')} 
  and sharh like'%{this.lb_Date_Sarresid.Text.Replace("/", "").Substring(2)}%' order by datee desc

  --price-kol = ???? ?????? ????? ??? 
  --tedad = ????? ????
  --price = ???? ????
  --sharh-khazane = ????
  --datee=????? ?????? ????? 
  --
  select * from Khazaneh_Eslami where id = 3


  select * from Khazaneh_Eslami_Kharid