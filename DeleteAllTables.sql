ALTER TABLE [HangFire].[State] DROP CONSTRAINT [FK_HangFire_State_Job]
GO
ALTER TABLE [HangFire].[JobParameter] DROP CONSTRAINT [FK_HangFire_JobParameter_Job]
GO
ALTER TABLE [dbo].[AspNetUserTokens] DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims] DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
DROP TABLE [HangFire].[State]
GO
DROP TABLE [HangFire].[Set]
GO
DROP TABLE [HangFire].[Server]
GO
DROP TABLE [HangFire].[Schema]
GO
DROP TABLE [HangFire].[List]
GO
DROP TABLE [HangFire].[JobQueue]
GO
DROP TABLE [HangFire].[JobParameter]
GO
DROP TABLE [HangFire].[Job]
GO
DROP TABLE [HangFire].[Hash]
GO
DROP TABLE [HangFire].[Counter]
GO
DROP TABLE [HangFire].[AggregatedCounter]
GO
DROP TABLE [dbo].[AspNetUserTokens]
GO
DROP TABLE [dbo].[AspNetUsers]
GO
DROP TABLE [dbo].[AspNetUserRoles]
GO
DROP TABLE [dbo].[AspNetUserLogins]
GO
DROP TABLE [dbo].[AspNetUserClaims]
GO
DROP TABLE [dbo].[AspNetRoles]
GO
DROP TABLE [dbo].[AspNetRoleClaims]
GO
DROP TABLE [dbo].[__EFMigrationsHistory]
GO
DROP SCHEMA [HangFire]
GO
