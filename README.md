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

## WebAppAngular
This is .NET Core web application which is a SPA, using Angular. 
This application demonstrates how an Angular SPA can be secured using Azure AD and how can it call the Web API protected by Azure. 


## CallerWebApi
This API was created to explain how a Web API calling web API scenario. This is middle API and this API gives call to WebApi mentioned above.  This middle API is called by WebApp.

## SampleDaemonApp
This is a console application which calls the protected API. 
This application is registered in Azure and has been granted application permission on the WebApi.

## AzureAdB2C/B2C.WebApi
This is the WebApi secured using Azure AD B2C. 

## AzureAdB2C/B2CWebAngularApp
This is Angular .NET Core Web Application secured using Azure AD B2C. 


# Scenarios
There can be many possible scnearios depending on how the applications are generally structured in real world. 
I have tried to explain some of the scenarios, below are the details.


## Protected Web App Calling Protected Web API 
This scenario is explained in [one of my blog post](https://manojchoudhari.wordpress.com/2020/05/04/securing-net-core-web-app-calling-web-api-using-msal-and-azure-ad/).


## Azure AD Protected Single Page App calling Protected Web API
This scenario is explained in my [blog post](https://manojchoudhari.wordpress.com/2020/05/05/angular-app-and-azure-ad-protected-web-api-using-msal/).

## Azure AD Protected WPF Application calling Protected Web API


## Azure AD B2C SPA calling Protected Web API
This scenario is explained here in my [blog post](https://manojchoudhari.wordpress.com/2020/05/15/angular-app-with-protected-web-apis-using-msal/)

## Azure AD Sample Daemon App calling web API
This scenario is explained here in my [blog post](https://manojchoudhari.wordpress.com/2020/05/26/daemon-app-that-calls-web-api-azure-ad-using-msal/).


# How to Run Samples ?
You will need to register the applications as explained in my blog posts (links are provided in above sections). 
Then update the appsettings.json files with appropriate client ids and tenant ids and client secrets.
Then you can run the Web API and Web Apps to ensure that everything works.

# Dependencies 
These samples are based on MSAL.  .NET projects use below NuGet packages:
- [Microsoft.Identity.Web](https://www.nuget.org/packages/Microsoft.Identity.Web)
- [Microsoft.Identity.Web.UI](https://www.nuget.org/packages/Microsoft.Identity.Web.UI)
- [Microsoft.Identity.Client](https://www.nuget.org/packages/Microsoft.Identity.Client)
