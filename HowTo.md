
## HowTo's

1. How to connect to the Localdb using **SQL Server Management Studio**.
   While working on an ASP.NET Core web application, I was using LocalDB, but when I tried to connect to it and modifying the data, but I couldn’t find it.
   One way to doing it when you start Windows PowerShell and execute: **& 'C:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe' info mssqllocaldb**
   In the outputs look for the pipe name instance like follow: **np:\\.\pipe\LOCALDB#9306681B\tsql\query**. Use it for server name on SSMS. 
		