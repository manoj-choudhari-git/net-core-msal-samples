import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MsalModule, MsalInterceptor } from '@azure/msal-angular';	

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AuthService } from './auth.service';

export const protectedResourceMap: [string, string[]][] = [
  ['https://graph.microsoft.com/v1.0/me', ['user.read']]
];


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    MsalModule.forRoot(
      {
        auth: {
          clientId: 'd471db94-db21-4103-aefa-679cd7435745', // This is your client ID
          authority: 'https://login.microsoftonline.com/da41245a5-11b3-996c-00a8-4d99re19f292', // This is your tenant ID
          redirectUri: 'https://localhost:44380/',  // This is your redirect URI
          postLogoutRedirectUri: "https://localhost:44380/",
          navigateToLoginRequestUrl: true,
        },
        cache: {
          cacheLocation: 'localStorage',
          storeAuthStateInCookie: false, // Set to true for Internet Explorer 11
        },
      },
      {
        popUp: false,
        consentScopes: [
          "user.read",
          "openid",
          "profile",
          "api://5e999e55-a661-4982-b897-965480492129/access_as_user"
        ],
        unprotectedResources: ["https://www.microsoft.com/en-us/"],
        protectedResourceMap:[
          ['https://localhost:44389/weatherforecast', ['api://5e999e55-a661-4982-b897-965480492129/access_as_user']],
          ['https://graph.microsoft.com/v1.0/me', ['user.read']]
        ],
        extraQueryParameters: {}
      }
    ),
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    AuthService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
