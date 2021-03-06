if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[楼盘信息]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[楼盘信息]
GO

CREATE TABLE [dbo].[楼盘信息] (
	[自动编号] [int] IDENTITY (1, 1) NOT NULL ,
	[区属] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[项目名称] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[项目地址] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[项目类别] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[价格] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[物业开发商] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[开盘日期] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[更新日期] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[电话] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[网址] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[楼盘类型] [varchar] (250) COLLATE Chinese_PRC_CI_AS NULL ,
	[DistrictID] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[InfoType] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[Price] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL 
) ON [PRIMARY]
GO

