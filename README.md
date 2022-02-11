www.miller-alex.de
============

My personal website built in ASP.NET MVC for .NET Core in C#

## Functionality

It includes the following features:

*  Appointment notification service: It allows me to be notified via email about my friends birthday or some important appointments.

## Application Architecture

The application is build based on the microservices architecture. The below figure shows the high level design of Back-end architecture.

![GitHub Architecture](/design/soa_architecture.png)

**API Gateway.** The API gateway is the entry point for clients. Instead of calling services directly, clients call the API gateway, which forwards the call to the appropriate services on the back end.

## Technical details

The following technology is used to develop this website:

* Language: C#
* IDE: Visual Studio 2022
* Platform: .NET6 / ASP.NET Core
* Postman (www.getpostman.com) is used to test micro services API
* SQL Server as RDBMS
* LINQ
* Entity Framework Core 6
	* Migrations
* HTML5 / CSS3
* Hangfire www.hangfire.io (perform background processing)
* Bootstrap www.getbootstrap.com (front-end component library)
* Polly www.github.com/App-vNext/Polly (allows to express resilience and transient fault handling policies)
* Dependency Injection
* SMTP
* Swagger (swagger.io) allows to describe the structure of the RESTfull API.
* Autommaper (automapper.org) to map models to DTO (Data Transfer Object)
* Ocelot (github.com/ThreeMammals/Ocelot) is a .NET API Gateway used for microservices.
* Refit (github.com/reactiveui/refit) The automatic type-safe REST library for .NET Core, Xamarin and .NET
* Security: 
	* Protect against clickjacking attacks.
	* Prevents the browser from doing MIME-type sniffing
	* Prevents XSS attacks
	* RFC 7519 JSON Web Token (JWT) is used to authenticate by micro services API
	* Proxy server
	* HTTPS (Hyper Text Transfer Protocol Secure) = HTTP + SSL/TLS Certificate
	* HSTS (HTTP Strict Transport Security) https://joonasw.net/view/hsts-in-aspnet-core
	* WebDAV module of IIS (I removed this module from IIS via web.config to resolve the issue with blocked REST methods (DELETE, UPDATE etc.)

* Automated Tests
	* Unit Tests use of xUnit. As for naming conventions, I use this: http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
	* Moq (https://github.com/Moq/moq4) 
	* FluentAssertions (https://fluentassertions.com)
	* Integration Tests 
* Continuous Delivery 
	* GitHub Actions (https://help.github.com/en/actions/automating-your-workflow-with-github-actions)

## Software Design Patterns
* **Model View Controller** to developing UI which divides the related program logic into three separate layers. 
* **Circuit Breaker** to handle faults, such as slow network connections and timeouts to improve the stability of application.