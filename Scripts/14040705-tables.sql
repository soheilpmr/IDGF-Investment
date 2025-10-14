-- Table: opt_BondTypes
CREATE TABLE [dbo].[opt_BondTypes](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL UNIQUE,
    [Has_Coupon] [bit] NOT NULL,
    CONSTRAINT [PK_opt_BondTypes] PRIMARY KEY CLUSTERED ([id] ASC)
);
GO

-- Table: opt_Brokers
CREATE TABLE [dbo].[opt_Brokers](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](200) NOT NULL UNIQUE,
    CONSTRAINT [PK_opt_Brokers] PRIMARY KEY CLUSTERED ([id] ASC)
);
GO

-- Table: opt_Bonds
CREATE TABLE [dbo].[opt_Bonds](
    [Id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
    [Type_id] [int] NOT NULL,
    [Symbol] [nvarchar](100) NOT NULL UNIQUE,
    [Issue_date] [date] NULL,
    [Maturity_date] [date] NOT NULL,
    [Face_value] [numeric](18, 0) NOT NULL DEFAULT (1000000),
    [Coupon_rate_percent] [numeric](5, 2) NULL,
    CONSTRAINT [PK_opt_Bonds] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_opt_Bonds_BondTypes] FOREIGN KEY([type_id]) REFERENCES [dbo].[opt_BondTypes] ([id])
);
GO

-- Table: opt_Transactions
CREATE TABLE [dbo].[opt_Transactions](
    [Id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
    [Bond_id] [numeric](18, 0) NOT NULL,
    [Broker_id] [int] NULL,
    [Transaction_date] [date] NOT NULL,
    [Transaction_type] [nvarchar](4) NOT NULL, -- 'Buy' or 'Sell'
    [Quantity] [numeric](18, 0) NOT NULL,
    [PricePerUnit] [numeric](18, 0) NOT NULL,
    [Commission] [numeric](18, 0) NOT NULL,
    [YtmAtTransaction] [numeric](10, 2) NULL,
	[Status] [smallint] NOT NULL,
    CONSTRAINT [PK_opt_Transactions] PRIMARY KEY CLUSTERED ([id] ASC),[commission]
    CONSTRAINT [FK_opt_Transactions_Bonds] FOREIGN KEY([bond_id]) REFERENCES [dbo].[opt_Bonds] ([id]),
    CONSTRAINT [FK_opt_Transactions_Brokers] FOREIGN KEY([broker_id]) REFERENCES [dbo].[opt_Brokers] ([id]),
    CONSTRAINT [CHK_opt_TransactionType] CHECK (transaction_type IN ('Buy', 'Sell'))
);
GO

ALTER TABLE [dbo].[opt_Transactions] ADD  CONSTRAINT [DF_opt_Transactions_Status]  DEFAULT ((2)) FOR [Status]

-- Table: opt_CouponPayments
CREATE TABLE [dbo].[opt_CouponPayments](
    [Id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
    [Bond_id] [numeric](18, 0) NOT NULL,
    [Payment_date] [date] NOT NULL,
    [Amount_per_unit] [numeric](18, 0) NOT NULL,
    CONSTRAINT [PK_opt_CouponPayments] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_opt_CouponPayments_Bonds] FOREIGN KEY([bond_id]) REFERENCES [dbo].[opt_Bonds] ([id])
);
GO

------------------------------------------------------------------Add vw_transactionBasic----------------------------------------------------------
alter VIEW [dbo].[vw_TransactionBasic] AS
SELECT 
    t.Id,
    b.Id AS BondId,
    b.Symbol,
    b.IssueDate,
    b.MaturityDate,
    br.Name AS BrokerName,
    t.PricePerUnit,
    t.Quantity,
	t.InvestmentPrice,
    b.FaceValue,
    t.TransactionDate,
    t.Commission,
    t.Status,
    t.TransactionType
FROM dbo.opt_Transactions t
JOIN dbo.opt_Bonds b ON b.Id = t.BondId
LEFT JOIN dbo.opt_Brokers br ON br.Id = t.BrokerId
where 
   t.TransactionType = 'Buy'
    AND t.Status = 2
    AND b.TypeID = 1   

	---------------------------------------------------------------ALter [opt_Transactions] (Add column InvestmentPrice )---------------------------------------------------------------
ALTER TABLE [dbo].[opt_Transactions] 
add InvestmentPrice [numeric](18, 0) NULL