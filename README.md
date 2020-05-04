# net-core-msal-samples  Repository

This repository contains the samples created by me for explaining how MSAL can be used to protect our applications and APIs. 
Currently the solution contians below projects:

## WebApi
This project is the default Web API template (.NET Core 3.1) generated using Visual Studio 2019.   It has a weather forecast API.  
I have just added few configurations to make it secure so that only authenticated users can call this API.
You can find more details about the steps at [this blog post](https://manojchoudhari.wordpress.com/2020/04/29/secure-your-web-api-using-azure-ad-and-msal/).

## WebApp
This is Razors View web application (.NET Core 3.1).  I have added a simple button on index.cshtml.  
The button click invokes the protected web API from WebApi project. 
You can find more details about how to protect web API at [this blog post](https://manojchoudhari.wordpress.com/2020/05/01/secure-net-core-web-app-using-azure-ad-and-msal/).

## WebAppMvc
This is MVC Web Application created in .NET Core 3.1.  
The index action of HomeController tries to call Weather Forecast API from WebApi project.
You can find more details about how to protect web API at [this blog post](https://manojchoudhari.wordpress.com/2020/05/01/secure-net-core-web-app-using-azure-ad-and-msal/).

# Scenarios
There can be many possible scnearios depending on how the applications are generally structured in real world. 
I have tried to explain some of the scenarios, below are the details.

## Protected Web App Calling Protected Web API 
This scenario is explained in [one of my blog post](https://manojchoudhari.wordpress.com/2020/05/04/securing-net-core-web-app-calling-web-api-using-msal-and-azure-ad/).

## Protected Single Page App calling Protected Web API

## Protected WPF Application calling Protected Web API


# How to Run Samples ?
You will need to register the applications as explained in my blog posts (links are provided in above sections). 
Then update the appsettings.json files with appropriate client ids and tenant ids and client secrets.
Then you can run the Web API and Web Apps to ensure that everything works.

# Dependencies 
These samples are based on MSAL.  .NET projects use below NuGet packages:
- [Microsoft.Identity.Web](https://www.nuget.org/packages/Microsoft.Identity.Web)
- [Microsoft.Identity.Web.UI](https://www.nuget.org/packages/Microsoft.Identity.Web.UI)
