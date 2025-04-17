# .NET Core C# Headless Data Sync Console App - SKY API

To demonstrate how to synchronize data with Blackbaud records using [.NET Core](https://www.microsoft.com/net/core/platform), this repository contains the code for a headless app.

This sample SKY API application fetches modified records using a `last_modified` date or `sort_token` parameter at 1-minute intervals.  Records returned from this query should be synchronized to a custom data source.

The app also creates a local JSON file to store OAuth tokens and query parameters to be used for subsequent SKY API requests. You may choose an alternate approach of how to store this information in a production application.

In the event of token expiration (currently, OAuth access tokens expire in 60 minutes), the application handles a `401 - Unauthorized` error from SKY API by using the stored refresh token to acquire a new access token. To automatically refresh without any UI interaction necessary, a new refresh token is also returned and stored for use later.

For more information, see the [Synchronize Data to a Custom Data Source with SKY API](https://developer.blackbaud.com/skyapi/docs/in-depth-topics/synchronize-data) in-depth topic.

## Run locally

- Download and install [.NET Core SDK](https://www.microsoft.com/net/core/)
- Open Terminal/Command Prompt and enter
```
$  git clone https://github.com/blackbaud/skyapi-headless-data-sync.git
$  cd skyapi-headless-data-sync
$  dotnet restore
```
- Update placeholder values in **appsettings.json** (all required).
```
{
    "AppSettings": {
        "AuthClientId": "YOUR_APPLICATION_ID",
        "AuthClientSecret": "YOUR_APPLICATION_SECRET",
        "SkyApiSubscriptionKey": "YOUR_SKYAPI_KEY"
    }
}
```
Instead of populating `appsettings.json`, you could create environment variables instead.
- On Windows, enter
```
set BBHeadlessDataSync_AppSettings__AuthClientId=YOUR_APP_ID
set BBHeadlessDataSync_AppSettings__AuthClientSecret=YOUR_APP_SECRET
set BBHeadlessDataSync_AppSettings__SkyApiSubscriptionKey=YOUR_SKYAPI_KEY
```
- On macOS, enter
```
export BBHeadlessDataSync_AppSettings__AuthClientId=YOUR_APP_ID
export BBHeadlessDataSync_AppSettings__AuthClientSecret=YOUR_APP_SECRET
export BBHeadlessDataSync_AppSettings__SkyApiSubscriptionKey=YOUR_SKYAPI_KEY
```
- Obtain intial refresh token by authorizing your app using the [OAuth 2.0 Authorization Code Flow](https://developer.blackbaud.com/skyapi/docs/authorization/auth-code-flow).
  - You may develop your own utility app or use an existing utility, such as [Postman](https://www.postman.com), to get a new OAuth 2.0 access/refresh token.
  - There are also [SKY API Auth Code Flow tutorials](https://developer.blackbaud.com/skyapi/docs/authorization/auth-code-flow/code-samples) available that can walk you through the process of acquiring a access/refresh token.
- For the first time, run the console app providing the refresh token as an argument.
```
dotnet run --refreshtoken <refreshtoken>
```
- The initial refresh token is exchanged for a new access token and refresh token, so it can be discarded. There is no need to provide a refresh token as an argument for subsequent sessions.
```
dotnet run
```

## Extend app capabilities

Currently, this app demonstrates how to query for modified constituent records using SKY API.  You can also extend the app to provide more sync capability.

- Add a new interface method to [IDataSyncService](https://github.com/blackbaud/skyapi-headless-data-sync/blob/master/Services/DataSync/IDataSyncService.cs) (e.g. `Task<bool> SyncGiftData();`).
- Add a new partial `DataSyncService` class (e.g. `DataSyncService.Gift.cs`) and implementation for querying an additional API for modified records.

To implement and hook up your own custom data source for synchronization, update the [UpdateConstituentData](https://github.com/blackbaud/skyapi-headless-data-sync/blob/master/Services/DataSync/DataSyncService.Constituent.cs) function to parse the provided `JObject` that contains modified constituents.
