#Philly311SMS

This is a demo app that shows how to use the [Philly311 API](http://phlapi.com/open311.html) to enable text message lookups for the status of service requests.

##Modules

* [Open311](https://github.com/mheadd/csharp-open311)
* [NancyFx](http://nancyfx.org/)
* [JSON.NET](http://james.newtonking.com/projects/json-net.aspx)
* [Twilio.NET](https://github.com/twilio/twilio-csharp)

##Installation / Usage

Clone this repo, then open the solution in Visual Studio.
You'll need to install NancyFx, JSON.NET and Twilio.NET via the [NuGet package manager](http://nuget.org/):

<pre>
   PM> Install-Package Nancy.Hosting.Aspnet
   PM> Install-Package Newtonsoft.Json
   PM> Install-Package Twilio
</pre>

The C# class for Open311 is not yet availalbe via NuGet, so [clone the repo](https://github.com/mheadd/csharp-open311), open the solution, build, then move the resultant <code>.dll</code> file into your solution.

You'll need a Twilio account. Add your account credentials in <code>Web.config</code> and your good to go. 

A test instance is currently running and can be accessed by sending a valid Philadelphia service request ID to (215) 600-2137.
