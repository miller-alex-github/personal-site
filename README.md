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
* LINQ
* Entity Framework Core 3.0
	* Migrations
* HTML5 / CSS3
* Hangfire www.hangfire.io (perform background processing)
* Bootstrap www.getbootstrap.com (front-end component library)
* Polly www.github.com/App-vNext/Polly (allows to express resilience and transient fault handling policies)
* Dependency Injection
* SMTP
* Swagger (wagger.io) allows to describe the structure of the RESTfull API.
* Autommaper (automapper.org) to map models to DTO (Data Transfer Object)
* Security: 
	* Protect against clickjacking attacks.
	* Prevents the browser from doing MIME-type sniffing
	* Prevents XSS attacks
* Automated Tests
	* Unit Tests use of xUnit. As for naming conventions, I use this: http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
	* Integration Tests 