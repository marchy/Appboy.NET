Appboy.NET
==========
Appboy.NET is a client library that lets you upload data to the Appboy API. It is a convenience library around the REST API, providing an object model for the API, correct data format / constraints-checking and error handling against the API.


*__NOTE__: Currently the library supports only the functionality for User Attributes (including device push tokens). Event and Purchases is not yet implemented. Feel free to submit a pull request with the functioanlity or [reach out to me](https://github.com/marchy) if you require this functionality and I will do my best to help integrate it.*


Installation
------------
The simplest way to install Appboy.NET is using the Nuget package manager.

NuGet is available for Visual Studio 2010, Visual Studio 2012 and Visual Studio 2013, and you can find instructions for installing the NuGet extension on the [NuGet.org website](http://docs.nuget.org/docs/start-here/installing-nuget).

To install via the Package Manager dialog, right-click your project and click “Manage NuGet Packages….”. Simply search the online catalog for “Appboy.NET” then click the “Install” button. NuGet will download the Appboy.NET library package and its dependencies, and add the proper references to your project.

To install via the Package Manager console, issue the following command:

    Install-Package Appboy.NET
    

Usage
-----
First grab your Company Secret and App Group ID from the [API Integration page](https://dashboard.appboy.com/company_settings/api_settings) in the Appboy dashboard.
Then create a new instance of the Appboy client using your credentials:
```csharp
const string companySecret = "YOUR COMPANY SECRET";
const string appGroupID = "YOUR APP GROUP ID";
var appboyClient = new AppboyClient( companySecret, appGroupID );
```

You then specify data to upload (User Attributes, Events, Purchases) using a builder pattern and send the data whenever you are to submit it to the API.

### User Attributes
You can add attributes for either individual users or lists of users through the fluent builder.

**Example: Add attributes for a single user**
```csharp
// build up user
var user = new UserAttributes( "MY EXTERNAL USER ID" ){
	Email = "overthetop@example.com",
	FirstName = "Mylie",
	LastName = "Cyrus",
	Gender = Gender.Female,
};
appboyClient.BuildUpRequest()
  .AddUserAttributes( user )
  .SendDataAsync( result => {
    Console.WriteLine( "SUCCESS. Num attributes processed: {0}", result.NumAttributesProcessed );
  },
  exception => {
    Console.WriteLine( "ERROR: {0}", exception.Message );
  });
```

**Example: Add attributes for multiple users**
```csharp
// build up user
IEnumerable<UserAttributes> usersToAdd = GetUsersToAdd();
appboyClient.BuildUpRequest()
  .AddUserAttributes( usersToAdd )
  .SendDataAsync( result => {
    Console.WriteLine( "SUCCESS. Num attributes processed: {0}", result.NumAttributesProcessed );
  },
  exception => {
    Console.WriteLine( "ERROR: {0}", exception.Message );
  });
```


#### Custom Attributes
Custom user attributes can be added for any of the following types: **string, int, float, boolean, DateTime**. To add a custom attribute to a user, simply issue the respective **SetCustomXxxxAtribute(..)** method.
```csharp
user.SetCustomStringAttribute( "FavouriteDrink", "Vodka Redbull" );
user.SetCustomIntegerAttribute( "NumDrinksDrankTonight", 3 );
....
```

You can also set multiple attributes at once. This can be useful if you are building up the attributes programatically, perhaps reading them dynamically from a database or some other dynamic source.
```cshaarp
var userAttributes = new Dictionary<string, object> {
  ...
}
user.SetCustomAttributes( userAttributes );
```



Implementation Best Practices
-----------------------------

### Sending out Multiple Requests (Batching)
The Appboy API limits to sending at most 50 Attributes, Events, or Purchase objects per request. If you need to send more data you will need to split up the requests into multiple batches. The builder pattern helps with this as you can create a new builder instance and it will reuse the credentials provided in the initial setup.

The library provides no support for the actual batching or sending out multiple requests. The simplest way to achieve it is to use the synchronous sending interface (**SendData(..)**) so that every request gets sent after the previous one completes. If you need higher performance you can issue multiple requests asynchronously (**SendDataAsync(..)**), however you will have to provide your own mechanism to ensure you don't kick off too many requests at once. This is outside the scope of the library, and you should carefully consider how and whether you should be sending loads of out to the API. There is an API limit of [2000 requests per hour](http://documentation.appboy.com/external-api-custom-event.html#external-apis).




Roadmap / Feature Requests
--------------------------
* Support for synchronous data sending. This should make working with multiple batches simpler if you don't want to multi-thread them (ie: send out multiple requests at once).
* Support for Events
* Support for Purchases
