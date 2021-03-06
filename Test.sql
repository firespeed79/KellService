USE [master]
GO
/****** Object:  Database [Test]    Script Date: 05/28/2016 18:23:54 ******/
CREATE DATABASE [Test] ON  PRIMARY 
( NAME = N'Test', FILENAME = N'D:\sql\Data\Test.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Test_log', FILENAME = N'D:\sql\Data\Test_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Test] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Test].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Test] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [Test] SET ANSI_NULLS OFF
GO
ALTER DATABASE [Test] SET ANSI_PADDING OFF
GO
ALTER DATABASE [Test] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [Test] SET ARITHABORT OFF
GO
ALTER DATABASE [Test] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [Test] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [Test] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [Test] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [Test] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [Test] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [Test] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [Test] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [Test] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [Test] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [Test] SET  DISABLE_BROKER
GO
ALTER DATABASE [Test] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [Test] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [Test] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [Test] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [Test] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [Test] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [Test] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [Test] SET  READ_WRITE
GO
ALTER DATABASE [Test] SET RECOVERY FULL
GO
ALTER DATABASE [Test] SET  MULTI_USER
GO
ALTER DATABASE [Test] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [Test] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'Test', N'ON'
GO
USE [Test]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 05/28/2016 18:23:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Student](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[classId] [int] NOT NULL,
 CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Student] ON
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (1, N'kell', 1)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (2, N'bill', 1)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (3, N'mary', 2)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (4, N'dortar', 2)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (5, N'wertra', 3)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (6, N'asay', 4)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (7, N'trewer', 5)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (8, N'stephen', 6)
INSERT [dbo].[Student] ([id], [name], [classId]) VALUES (9, N'gethe', 7)
SET IDENTITY_INSERT [dbo].[Student] OFF
/****** Object:  Table [dbo].[Class]    Script Date: 05/28/2016 18:23:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Class](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[className] [varchar](50) NOT NULL,
	[gradeCount] [int] NOT NULL,
 CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Class] ON
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (1, N'11', 1)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (2, N'12', 1)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (3, N'13', 1)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (4, N'21', 2)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (5, N'22', 2)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (6, N'31', 3)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (7, N'32', 3)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (8, N'41', 4)
INSERT [dbo].[Class] ([id], [className], [gradeCount]) VALUES (9, N'51', 5)
SET IDENTITY_INSERT [dbo].[Class] OFF
/****** Object:  Default [DF_Student_classId]    Script Date: 05/28/2016 18:23:56 ******/
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF_Student_classId]  DEFAULT ((0)) FOR [classId]
GO
/****** Object:  Default [DF_Class_gradeCount]    Script Date: 05/28/2016 18:23:56 ******/
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF_Class_gradeCount]  DEFAULT ((1)) FOR [gradeCount]
GO
