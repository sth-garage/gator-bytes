USE [master]
GO

/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [gatorBytesServiceLogin]    Script Date: 12/18/2025 11:12:19 AM ******/
CREATE LOGIN [gatorBytesServiceLogin] WITH PASSWORD=N'BGP8a7eNnfzDGyZQJrWMi4XgwLQZJNKh/pOuis9eqqs=', DEFAULT_DATABASE=[GatorBytes], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [gatorBytesServiceLogin] DISABLE
GO

ALTER SERVER ROLE [sysadmin] ADD MEMBER [gatorBytesServiceLogin]
GO


