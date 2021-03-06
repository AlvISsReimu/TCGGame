USE [master]
GO
/****** Object:  Database [TCGGame]    Script Date: 2015/1/2 16:10:30 ******/
CREATE DATABASE [TCGGame]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TCGGame', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TCGGame.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TCGGame_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TCGGame_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TCGGame] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TCGGame].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TCGGame] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TCGGame] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TCGGame] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TCGGame] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TCGGame] SET ARITHABORT OFF 
GO
ALTER DATABASE [TCGGame] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TCGGame] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TCGGame] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TCGGame] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TCGGame] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TCGGame] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TCGGame] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TCGGame] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TCGGame] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TCGGame] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TCGGame] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TCGGame] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TCGGame] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TCGGame] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TCGGame] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TCGGame] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TCGGame] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TCGGame] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TCGGame] SET  MULTI_USER 
GO
ALTER DATABASE [TCGGame] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TCGGame] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TCGGame] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TCGGame] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [TCGGame] SET DELAYED_DURABILITY = DISABLED 
GO
USE [TCGGame]
GO
/****** Object:  UserDefinedFunction [dbo].[fun_CalcFactor]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[fun_CalcFactor](@min float, @max float, @rand float)
returns float
as
begin
	declare @factor float
	declare @r float;
	set @factor = @min + (@max-@min)*dbo.fun_Integral(@rand, 0.2);
	return @factor;
end
GO
/****** Object:  UserDefinedFunction [dbo].[fun_Integral]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[fun_Integral](@x float, @delta float)
returns float
as
begin
	declare @y float;
	declare @i float;
	set @y = 0;
	set @i = -10;
	while @i < @x
	begin
		set @y = @y + 0.01 * dbo.fun_NormsDist(@i, 0.5, @delta);
		set @i = @i + 0.01;
	end
	return @y;
end
GO
/****** Object:  UserDefinedFunction [dbo].[fun_NormsDist]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[fun_NormsDist](@x float, @miu float, @delta float)
returns float
as
begin
	declare @result float;
	set @result = EXP((-1)*(POWER(@x-@miu, 2)/(2*POWER(@delta, 2))))/(@delta*SQRT(2*PI()));
	return @result;
end
GO
/****** Object:  Table [dbo].[tbl_Card]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_Card](
	[Card_ID] [int] NOT NULL,
	[Card_Name] [varchar](30) NOT NULL,
	[Card_Power] [int] NOT NULL,
	[Card_Price] [int] NOT NULL,
	[Card_Pack] [int] NOT NULL,
	[Card_Rarity] [varchar](3) NOT NULL,
 CONSTRAINT [PK__tbl_Card__C1F8DC594D201248] PRIMARY KEY CLUSTERED 
(
	[Card_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_HoldCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_HoldCard](
	[HoCa_UserID] [int] NOT NULL,
	[HoCa_CardID] [int] NOT NULL,
	[HoCa_Quantity] [int] NOT NULL,
 CONSTRAINT [PK__tbl_Hold__4E2E6D3990681D9E] PRIMARY KEY CLUSTERED 
(
	[HoCa_UserID] ASC,
	[HoCa_CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Pack]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_Pack](
	[Pack_ID] [int] NOT NULL,
	[Pack_Name] [varchar](4) NOT NULL,
	[Pack_Date] [int] NOT NULL,
	[Pack_Price] [int] NOT NULL,
 CONSTRAINT [PK__tbl_Pack__AA6923075CA0D32A] PRIMARY KEY CLUSTERED 
(
	[Pack_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_Rank]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Rank](
	[Rank_Key] [int] NOT NULL,
	[Rank_Number] [int] NOT NULL,
	[Rank_ID] [int] NOT NULL,
	[Rank_Money] [int] NOT NULL,
	[Rank_HP] [int] NOT NULL,
	[Rank_Time] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_Rank] PRIMARY KEY CLUSTERED 
(
	[Rank_Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_User]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_User](
	[User_ID] [int] NOT NULL,
	[User_Name] [varchar](20) NOT NULL,
	[User_Password] [varchar](32) NOT NULL,
	[User_HP] [int] NOT NULL,
	[User_Money] [int] NOT NULL,
	[User_GameDate] [int] NOT NULL,
	[User_RegDate] [datetime] NOT NULL,
	[User_ManageLevel] [int] NOT NULL,
 CONSTRAINT [PK__tbl_User__C5B19602CEBD4720] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_Card]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Card__Card_P__22AA2996] FOREIGN KEY([Card_Pack])
REFERENCES [dbo].[tbl_Pack] ([Pack_ID])
GO
ALTER TABLE [dbo].[tbl_Card] CHECK CONSTRAINT [FK__tbl_Card__Card_P__22AA2996]
GO
ALTER TABLE [dbo].[tbl_HoldCard]  WITH CHECK ADD  CONSTRAINT [FK__tbl_HoldC__HoCa___182C9B23] FOREIGN KEY([HoCa_UserID])
REFERENCES [dbo].[tbl_User] ([User_ID])
GO
ALTER TABLE [dbo].[tbl_HoldCard] CHECK CONSTRAINT [FK__tbl_HoldC__HoCa___182C9B23]
GO
ALTER TABLE [dbo].[tbl_HoldCard]  WITH CHECK ADD  CONSTRAINT [FK__tbl_HoldC__HoCa___1920BF5C] FOREIGN KEY([HoCa_CardID])
REFERENCES [dbo].[tbl_Card] ([Card_ID])
GO
ALTER TABLE [dbo].[tbl_HoldCard] CHECK CONSTRAINT [FK__tbl_HoldC__HoCa___1920BF5C]
GO
ALTER TABLE [dbo].[tbl_Rank]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Rank__Rank_I__5BA37B27] FOREIGN KEY([Rank_ID])
REFERENCES [dbo].[tbl_User] ([User_ID])
GO
ALTER TABLE [dbo].[tbl_Rank] CHECK CONSTRAINT [FK__tbl_Rank__Rank_I__5BA37B27]
GO
ALTER TABLE [dbo].[tbl_Card]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Card] CHECK  (([Card_ID]>(0)))
GO
ALTER TABLE [dbo].[tbl_Card] CHECK CONSTRAINT [CK_tbl_Card]
GO
ALTER TABLE [dbo].[tbl_Card]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Card_1] CHECK  (([Card_Power]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Card] CHECK CONSTRAINT [CK_tbl_Card_1]
GO
ALTER TABLE [dbo].[tbl_Card]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Card_2] CHECK  (([Card_Price]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Card] CHECK CONSTRAINT [CK_tbl_Card_2]
GO
ALTER TABLE [dbo].[tbl_HoldCard]  WITH CHECK ADD  CONSTRAINT [CK_tbl_HoldCard] CHECK  (([HoCa_Quantity]>(0)))
GO
ALTER TABLE [dbo].[tbl_HoldCard] CHECK CONSTRAINT [CK_tbl_HoldCard]
GO
ALTER TABLE [dbo].[tbl_Pack]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Pack] CHECK  (([Pack_ID]>(0)))
GO
ALTER TABLE [dbo].[tbl_Pack] CHECK CONSTRAINT [CK_tbl_Pack]
GO
ALTER TABLE [dbo].[tbl_Pack]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Pack_1] CHECK  (([Pack_Price]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Pack] CHECK CONSTRAINT [CK_tbl_Pack_1]
GO
ALTER TABLE [dbo].[tbl_Rank]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Rank] CHECK  (([Rank_Number]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Rank] CHECK CONSTRAINT [CK_tbl_Rank]
GO
ALTER TABLE [dbo].[tbl_Rank]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Rank_1] CHECK  (([Rank_Money]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Rank] CHECK CONSTRAINT [CK_tbl_Rank_1]
GO
ALTER TABLE [dbo].[tbl_Rank]  WITH CHECK ADD  CONSTRAINT [CK_tbl_Rank_2] CHECK  (([Rank_HP]>=(0)))
GO
ALTER TABLE [dbo].[tbl_Rank] CHECK CONSTRAINT [CK_tbl_Rank_2]
GO
ALTER TABLE [dbo].[tbl_User]  WITH CHECK ADD  CONSTRAINT [CK_tbl_User] CHECK  (([User_ID]>=(0)))
GO
ALTER TABLE [dbo].[tbl_User] CHECK CONSTRAINT [CK_tbl_User]
GO
ALTER TABLE [dbo].[tbl_User]  WITH CHECK ADD  CONSTRAINT [CK_tbl_User_1] CHECK  (([User_HP]>=(0)))
GO
ALTER TABLE [dbo].[tbl_User] CHECK CONSTRAINT [CK_tbl_User_1]
GO
ALTER TABLE [dbo].[tbl_User]  WITH CHECK ADD  CONSTRAINT [CK_tbl_User_2] CHECK  (([User_Money]>=(0)))
GO
ALTER TABLE [dbo].[tbl_User] CHECK CONSTRAINT [CK_tbl_User_2]
GO
ALTER TABLE [dbo].[tbl_User]  WITH CHECK ADD  CONSTRAINT [CK_tbl_User_3] CHECK  (([User_GameDate]>=(0) AND [User_GameDate]<=(52)))
GO
ALTER TABLE [dbo].[tbl_User] CHECK CONSTRAINT [CK_tbl_User_3]
GO
/****** Object:  StoredProcedure [dbo].[prd_BuyCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_BuyCard]
@card_id int,
@buy_num int,
@buy_price int,
@user_id int
as
begin
	declare @now_money int, @new_money int;
	declare @now_num int, @new_num int;
	select @now_money = User_Money from tbl_User where @user_id = User_ID;
	set @new_money = @now_money - @buy_price * @buy_num;
	update tbl_User set User_Money = @new_money where User_ID = @user_id;

	if exists (select * from tbl_HoldCard where HoCa_UserID = @user_id and HoCa_CardID = @card_id)
	begin
		select @now_num = HoCa_Quantity from tbl_HoldCard where HoCa_UserID = @user_id and HoCa_CardID = @card_id;
		set @new_num = @now_num + @buy_num;
		update tbl_HoldCard set HoCa_Quantity = @new_num where HoCa_UserID = @user_id and HoCa_CardID = @card_id;
	end
	else
	begin
		insert into tbl_HoldCard values(@user_id, @card_id, @buy_num);
	end
end

GO
/****** Object:  StoredProcedure [dbo].[prd_CalcPowerSum]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_CalcPowerSum]
	@user_id int
as
declare @multpower int;
set @multpower = 0;
create table tbl_Temp(Temp_Power int, Temp_Quantity int, Temp_Mult int);
insert into tbl_Temp
select Card_Power, HoCa_Quantity, Card_Power*HoCa_Quantity from tbl_Card, tbl_HoldCard where @user_id = HoCa_UserID and Card_ID = HoCa_CardID;
select @multpower = SUM(Temp_Mult) from tbl_Temp;
drop table tbl_Temp;
return @multpower;
GO
/****** Object:  StoredProcedure [dbo].[prd_Event_GetCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_Event_GetCard]
@user_id int,
@card_name varchar(30) out,
@card_num int out,
@card_rarity varchar(3) out
as
begin
	declare @card_id int;
	set @card_num = floor(rand() * 5) + 1;
	Select top 1 @card_id = Card_ID, @card_name = Card_Name, @card_rarity = Card_Rarity From tbl_Card Order By newid();
	insert into tbl_HoldCard values (@user_id, @card_id, @card_num);
end
GO
/****** Object:  StoredProcedure [dbo].[prd_Event_LoseCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_Event_LoseCard]
@user_id int,
@card_name varchar(30) out,
@losecard_num int out,
@card_rarity varchar(3) out
as
begin
	if exists(select * from tbl_HoldCard where @user_id = HoCa_UserID)
	begin
		declare @card_id int, @card_num int;
		set @losecard_num = floor(rand() * 5) + 1;
		Select top 1 @card_id = HoCa_CardID, @card_num = HoCa_Quantity From tbl_HoldCard where HoCa_UserID = @user_id Order By newid();
		if (@losecard_num > @card_num) begin set @losecard_num = @card_num end
		if (@losecard_num = @card_num)
		begin
			delete from tbl_HoldCard where HoCa_UserID = @user_id and @card_id = HoCa_CardID;
		end
		else
		begin
			update tbl_HoldCard set HoCa_Quantity = HoCa_Quantity - @losecard_num where HoCa_UserID = @user_id and @card_id = HoCa_CardID;
		end
		select @card_name = Card_Name, @card_rarity = Card_Rarity from tbl_Card where Card_ID = @card_id;
	end
end
GO
/****** Object:  StoredProcedure [dbo].[prd_GenCardShop]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_GenCardShop]
as
begin
	declare @card_type_num_sum int, @card_type_num_sell int, @card_type_num_upper_ratio float, @card_type_num_lower_ratio float, @card_type_num_sell_upper float, @card_type_num_sell_lower float, @i int;
	declare @card_id int, @card_name varchar(30), @card_power int, @card_price int, @card_rarity varchar(3);
	set @card_type_num_upper_ratio = 0.7;
	set @card_type_num_lower_ratio = 0.25;
	set @card_type_num_sum = (select COUNT(*) from tbl_Card);
	set @card_type_num_sell_upper = @card_type_num_upper_ratio * @card_type_num_sum;
	set @card_type_num_sell_lower = @card_type_num_lower_ratio * @card_type_num_sum;
	exec prd_GenRand @card_type_num_sell_upper, @card_type_num_sell_lower, @card_type_num_sell out;
	set @i = 0;
	declare @multi int, @quantity int, @new_price int, @min_ratio float, @max_ratio float, @min_quan float, @max_quan float;
	create table tbl_Temp1(id int, name varchar(30), power int, rarity varchar(3), price int, quantity int);
	while @i < @card_type_num_sell
	begin
		Select top 1 @card_id = Card_ID, @card_name = Card_Name, @card_power = Card_Power, @card_price = Card_Price, @card_rarity = Card_Rarity From tbl_Card Order By newid();
		if not exists (Select * From tbl_Temp1 Where @card_id = id)
		begin
			if @card_rarity = 'N' begin set @min_ratio = 0.3; set @max_ratio = 2.5; set @min_quan = 5; set @max_quan = 100; end
			else begin
				if @card_rarity = 'R' begin set @min_ratio = 0.4; set @max_ratio = 2.0; set @min_quan = 5; set @max_quan = 75; end
				else begin
					if @card_rarity = 'SR' begin set @min_ratio = 0.6; set @max_ratio = 1.45; set @min_quan = 1; set @max_quan = 10; end
					else begin
						if @card_rarity = 'UR' begin set @min_ratio = 0.5; set @max_ratio = 1.4; set @min_quan = 1; set @max_quan = 15; end
						else begin
							if @card_rarity = 'UTR' begin set @min_ratio = 0.65; set @max_ratio = 1.4; set @min_quan = 1; set @max_quan = 5; end
							else begin
								if @card_rarity = 'SER' begin set @min_ratio = 0.65; set @max_ratio = 1.5; set @min_quan = 1; set @max_quan = 5; end
								else begin
									if @card_rarity = 'HR' begin set @min_ratio = 0.7; set @max_ratio = 1.5; set @min_quan = 1; set @max_quan = 3; end
									else begin set @min_ratio = 0.75; set @max_ratio = 1.5; set @min_quan = 1; set @max_quan = 2; end
								end
							end
						end
					end
				end
			end
			exec prd_GenRand @max_quan, @min_quan, @quantity out;
			set @new_price = ROUND(dbo.fun_CalcFactor(@min_ratio, @max_ratio, rand()) * @card_price, 0);
			insert into tbl_Temp1 values(@card_id, @card_name, @card_power, @card_rarity, @new_price, @quantity);
			set @i = @i + 1;
		end
	end
	select * from tbl_Temp1;
	drop table tbl_Temp1;
end
GO
/****** Object:  StoredProcedure [dbo].[prd_GenRand]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_GenRand]
@Upper float,
@Lower float,
@Result int out
as
SET @Result = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0);
GO
/****** Object:  StoredProcedure [dbo].[prd_GetHoldCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_GetHoldCard]
    @user_id int
as
begin
    select Card_Name, Card_Power, Card_Rarity, HoCa_Quantity, Card_ID from tbl_Card, tbl_HoldCard where @user_id = HoCa_UserID and Card_ID = HoCa_CardID;
end

GO
/****** Object:  StoredProcedure [dbo].[prd_InitUser]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_InitUser]
@user_id int
as
begin
	update tbl_User set User_Money = 2500, User_HP = 100, User_GameDate = 0 where User_ID = @user_id;
	delete from tbl_HoldCard where HoCa_UserID = @user_id;
end
GO
/****** Object:  StoredProcedure [dbo].[prd_NewCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_NewCard]
@card_name varchar(30),
@card_power int,
@card_price int,
@card_rarity varchar(3),
@pack_name varchar(4)
as
begin
	declare @pack_id int;
	select @pack_id = Pack_ID from tbl_Pack where Pack_Name = @pack_name;
	insert into tbl_Card(Card_Name, Card_Power, Card_Price, Card_Rarity, Card_Pack) values(@card_name, @card_power, @card_price, @card_rarity, @pack_id);
end
GO
/****** Object:  StoredProcedure [dbo].[prd_OpenPack]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_OpenPack]
@user_id int,
@pack_id int
as
begin
	create table tbl_Temp(Temp_Card_ID int, Temp_Card_Name varchar(30), Temp_Card_Rarity varchar(3));
	insert into tbl_Temp Select top 4 Card_ID, Card_Name, Card_Rarity From tbl_Card where Card_Rarity = 'N' and Card_Pack = @pack_id Order By newid();
	declare @random float;
	declare @rate_r float, @rate_sr float, @rate_ur float, @rate_other float;
	set @rate_r = 0.7333;
	set @rate_sr = 0.1;
	set @rate_ur = 0.1333;
	set @random = rand();
	if (@random <= @rate_r)
		insert into tbl_Temp Select top 1 Card_ID, Card_Name, Card_Rarity From tbl_Card where Card_Rarity = 'R' and Card_Pack = @pack_id Order By newid();
	else if (@random <= @rate_sr + @rate_r)
		insert into tbl_Temp Select top 1 Card_ID, Card_Name, Card_Rarity From tbl_Card where Card_Rarity = 'SR' and Card_Pack = @pack_id Order By newid();
	else if (@random <= @rate_ur + @rate_sr + @rate_r)
		insert into tbl_Temp Select top 1 Card_ID, Card_Name, Card_Rarity From tbl_Card where Card_Rarity = 'UR' and Card_Pack = @pack_id Order By newid();
	else
		insert into tbl_Temp Select top 1 Card_ID, Card_Name, Card_Rarity From tbl_Card where Card_Rarity != 'UR' and Card_Rarity != 'SR' and Card_Rarity != 'R' and Card_Rarity != 'N' and Card_Pack = @pack_id Order By newid();
	insert into tbl_HoldCard select @user_id, Temp_Card_ID, 1 from tbl_Temp;
	select * from tbl_Temp;
	drop table tbl_Temp;
end
GO
/****** Object:  StoredProcedure [dbo].[prd_RarityDistribution]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_RarityDistribution]
@pack_id int
as
begin
declare @n_num int, @r_num int, @sr_num int, @ur_num int, @utr_num int, @ser_num int, @hr_num int, @other_num int;
	select @n_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'N';
	select @r_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'R';
	select @sr_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'SR';
	select @ur_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'UR';
	select @utr_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'UTR';
	select @ser_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'SER';
	select @hr_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity = 'HR';
	select @other_num = COUNT(*) from tbl_Card where Card_Pack = @pack_id and Card_Rarity != 'N' and Card_Rarity != 'R' and Card_Rarity != 'SR' and Card_Rarity != 'UR' and Card_Rarity != 'UTR' and Card_Rarity != 'SER' and Card_Rarity != 'HR';
	create table tbl_Temp(Temp_Rarity varchar(5), Temp_Num int);
	insert into tbl_Temp values('N', @n_num);
	insert into tbl_Temp values('R', @r_num);
	insert into tbl_Temp values('SR', @sr_num);
	insert into tbl_Temp values('UR', @ur_num);
	insert into tbl_Temp values('UTR', @utr_num);
	insert into tbl_Temp values('SER', @ser_num);
	insert into tbl_Temp values('HR', @hr_num);
	insert into tbl_Temp values('Other', @other_num);
	select * from tbl_Temp;
	drop table tbl_Temp;
end
GO
/****** Object:  StoredProcedure [dbo].[prd_SaveCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_SaveCard]
@card_id int,
@card_name varchar(30),
@card_power int,
@card_price int,
@card_rarity varchar(3),
@pack_name varchar(4)
as
begin
	declare @pack_id int;
	select @pack_id = Pack_ID from tbl_Pack where Pack_Name = @pack_name;
	update tbl_Card set Card_Name = @card_name, Card_Power = @card_power, Card_Price = @card_price, Card_Rarity = @card_rarity, Card_Pack = @pack_id where Card_ID = @card_id;
end
GO
/****** Object:  StoredProcedure [dbo].[prd_SellCard]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[prd_SellCard]
@card_id int,
@sell_num int,
@sell_price int,
@user_id int
as
declare @now_num int, @new_num int, @now_money int, @new_money int;
select @now_num = HoCa_Quantity from tbl_HoldCard where HoCa_CardID = @card_id and HoCa_UserID = @user_id;
if @now_num <= @sell_num begin
delete from tbl_HoldCard where HoCa_CardID = @card_id;
end
else begin
set @new_num = @now_num - @sell_num;
update tbl_HoldCard set HoCa_Quantity = @new_num where HoCa_CardID = @card_id and HoCa_UserID = @user_id;
end
select @now_money = User_Money from tbl_User where @user_id = User_ID;
set @new_money = @now_money + @sell_price * @sell_num;
update tbl_User set User_Money = @new_money where @user_id = User_ID;
GO
/****** Object:  Trigger [dbo].[tgr_Card_D]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[tgr_Card_D] on [dbo].[tbl_Card]
instead of delete
as
begin
	declare @card_id int, @i int;
	select @card_id = Card_ID from deleted;
	delete from tbl_HoldCard where HoCa_CardID = @card_id;
	delete from tbl_Card where Card_ID = @card_id;
end
GO
/****** Object:  Trigger [dbo].[tgr_Card_I]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[tgr_Card_I] on [dbo].[tbl_Card]
instead of insert
as
begin
	declare @card_id int, @card_name varchar(30), @card_power int, @card_price int, @card_rarity varchar(3), @card_pack int, @i int;
	select @card_name = Card_Name, @card_power = Card_Power, @card_price = Card_Price, @card_rarity = Card_Rarity, @card_pack = Card_Pack from inserted;
	if not exists(select * from tbl_Card)
		set @card_id = 1;
	else begin
		set @i = 1;
		while exists (select * from tbl_Card where Card_ID = @i)
			set @i = @i + 1;
		set @card_id = @i;
	end
	insert into tbl_Card values(@card_id, @card_name, @card_power, @card_price, @card_pack, @card_rarity);
end
GO
/****** Object:  Trigger [dbo].[trg_HoldCard_I]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[trg_HoldCard_I] on [dbo].[tbl_HoldCard] instead of insert
as
declare @card_id int, @user_id int, @quantity int;
declare mycursor cursor for select HoCa_UserID, HoCa_CardID, HoCa_Quantity from inserted;
open mycursor;
fetch next from mycursor into @user_id, @card_id, @quantity
while @@fetch_status = 0
begin
	if exists(select * from tbl_HoldCard where @card_id = HoCa_CardID and @user_id = HoCa_UserID)
	begin
		update tbl_HoldCard set HoCa_Quantity = HoCa_Quantity + @quantity where @card_id = HoCa_CardID and @user_id = HoCa_UserID;
	end
	else
	begin
		insert into tbl_HoldCard values(@user_id, @card_id, @quantity);
	end
	fetch next from mycursor into @user_id, @card_id, @quantity
end
close mycursor;
deallocate mycursor;
GO
/****** Object:  Trigger [dbo].[tgr_Rank_I]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[tgr_Rank_I] on [dbo].[tbl_Rank]
instead of insert as
begin
	declare @user_id int, @user_hp int, @user_money int, @key int;
	select @user_id = Rank_ID from inserted;
	if exists(select * from tbl_Rank)
		select @key = MAX(Rank_Key) + 1 from tbl_Rank;
	else
		set @key = 1;
	select @user_hp = User_HP, @user_money = User_Money from tbl_User where @user_id = User_ID;
	insert into tbl_Rank values(@key, 0, @user_id, @user_money, @user_hp, GETDATE());
	declare @i int, @j int, @rank int, @money int;
    set @j = 1;
    select  @i = count(*) from tbl_Rank;
    DECLARE mycursor CURSOR FOR SELECT Rank_Key, Rank_Money FROM tbl_Rank;
    OPEN mycursor;
    while @j <= @i
    begin
		FETCH NEXT FROM mycursor into @key, @money;
		select @rank = COUNT(*) + 1 from tbl_Rank where @money < Rank_Money;
		update tbl_Rank set Rank_Number = @rank where @key = Rank_Key;
		set @j = @j + 1;
    end
    CLOSE mycursor;
    DEALLOCATE mycursor;
end
GO
/****** Object:  Trigger [dbo].[trg_User_I]    Script Date: 2015/1/2 16:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE trigger [dbo].[trg_User_I] on [dbo].[tbl_User] instead of insert as
declare @id int, @name varchar(20), @password varchar(32), @i int;
select @name = User_Name, @password = User_Password from inserted;

if not exists(select * from tbl_User)
	set @id = 1;
else begin
	set @i = 1;
	while exists (select * from tbl_User where User_ID = @i)
		set @i = @i + 1;
	set @id = @i;
end

insert into tbl_User(User_ID, User_Name, User_Password, User_HP, User_Money, User_GameDate, User_RegDate, User_ManageLevel) values (@id, @name, @password, 100, 2500, 0, getDate(), 0);

GO
USE [master]
GO
ALTER DATABASE [TCGGame] SET  READ_WRITE 
GO
