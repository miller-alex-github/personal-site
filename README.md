www.miller-alex.de
============

My personal website built in ASP.NET MVC for .NET Core in C#

## Functionality

It includes the following features:

*  Appointment notification service: It allows me to be notified via email about my friends birthday or some important appointments.


## Technical details

The following technology is used to develop this website:

* Language: C#
* IDE: Visual Studio 2019
* Platform: .NET Core 2.1 / ASP.NET Core
* SQL Server as RDBMS
* Entity Framework Core 3.0
	* Migrations
* HTML5 / CSS3
* Hangfire www.hangfire.io (perform background processing)
* Bootstrap www.getbootstrap.com (front-end component library)
* Dependency Injection
* SMTP
* Security: 
	* Protect against clickjacking attacks.
	* Prevents the browser from doing MIME-type sniffing
	* Prevents XSS attacks