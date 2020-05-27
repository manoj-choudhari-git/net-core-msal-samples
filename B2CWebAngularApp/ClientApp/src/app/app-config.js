"use strict";
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
// this checks if the app is running on IE
exports.isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 ||
    window.navigator.userAgent.indexOf('Trident/') > -1;
/***********************************************************
 * STEP 1 - B2C Policies and User Flows
 ***********************************************************/
exports.b2cPolicies = {
    names: {
        signUpSignIn: "B2C_1_SignUpSignInMFA",
        resetPassword: "B2C_1_Reset",
    },
    authorities: {
        signUpSignIn: {
            authority: "https://samplead.b2clogin.com/samplead.onmicrosoft.com/B2C_1_SignUpSignInMFA"
        },
        resetPassword: {
            authority: "https://samplead.b2clogin.com/samplead.onmicrosoft.com/b2c_1_reset"
        }
    }
};
/***********************************************************
 * STEP 2 - Web API Scopes & URLs
 ***********************************************************/
exports.apiConfig = {
    b2cScopes: ['https://samplead.onmicrosoft.com/sample-api/api-scope'],
    webApi: 'https://localhost:44379/weatherforecast'
};
/***********************************************************
 * STEP 3 - Authentication Configurations
 ***********************************************************/
exports.msalConfig = {
    auth: {
        clientId: "3117a83e-c3bc-482d-82a1-9eee30b73609",
        authority: exports.b2cPolicies.authorities.signUpSignIn.authority,
        redirectUri: "https://localhost:44361/",
        postLogoutRedirectUri: "https://localhost:44361/",
        navigateToLoginRequestUrl: true,
        validateAuthority: false,
    },
    cache: {
        cacheLocation: "localStorage",
        // Set this to "true" to save cache in cookies
        // to address trusted zones limitations in IE
        storeAuthStateInCookie: exports.isIE,
    },
};
/***********************************************************
 * STEP 4 - Scopes Required For Login
 ***********************************************************/
exports.loginRequest = {
    scopes: ['openid', 'profile'],
};
/***********************************************************
 * STEP 5 - Scopes which will be used for the access token
 *          request for your web API
 ***********************************************************/
exports.tokenRequest = {
    scopes: exports.apiConfig.b2cScopes
};
/***********************************************************
 * STEP 6 - MSAL Angular Configurations
 *          Define protected API URLs and required scopes
 ***********************************************************/
exports.protectedResourceMap = [
    [exports.apiConfig.webApi, exports.apiConfig.b2cScopes]
];
/***********************************************************
 * STEP 7 - MSAL Angular specific configurations
 *
 ***********************************************************/
exports.msalAngularConfig = {
    popUp: !exports.isIE,
    consentScopes: __spreadArrays(exports.loginRequest.scopes, exports.tokenRequest.scopes),
    // API calls to these coordinates will NOT activate MSALGuard
    unprotectedResources: [],
    // API calls to these coordinates will activate MSALGuard
    protectedResourceMap: exports.protectedResourceMap,
    extraQueryParameters: {}
};
//# sourceMappingURL=app-config.js.map