# Welcome to the Footprint documentation.
Footprint is a behavioral targeting and marketing automation plugin for the Umbraco CMS.
https://our.umbraco.org/projects/website-utilities/footprint/
- PS made by www.novicell.dk

## Source and help wanted
At time of writing the source is a bit messy. (A bunch of external libraries was included in the project) If someon has time for cleanup and updating please do so and throw us a pull request. :)

## Footprint using Javascript

The Novicell Footprint Umbraco package provides a JavaScript interface (footprint.js) to the back-end Footprint API for the current visitor.

The library provides the following methods:
```javascript
footprint.currentVisitor.addToSegment(segmentAlias, onSuccess, onError);
footprint.currentVisitor.isInSegment(segmentAlias, onSuccess, onError);
footprint.currentVisitor.setId(visitorId, onSuccess, onError);
footprint.currentVisitor.setProperties({ key1: value1, key2: value2 }, onSuccess, onError);
```
The `onSuccess` and `onError` arguments are (optional) callbacks for the underlying AJAX calls to the back-end.

### Example
Say you have a newsletter form and you would like to add the visitor to the 'newsletter subscribers' segment when the visitor signs up to receive the newsletter.

First, we include the Footprint library:
```html
<script src="App_Plugins/ncFootprint/Api/footprint.js"></script>
```
And say we have a newsletter subscribe form:
```html
<form id="newsletter-subscribe-form">
    <div>
        <label>
            Full name:
            <input type="text" name="name" placeholder="Full name" required />
        </label>
    </div>
    
    <div>
        <label>
            E-mail:
            <input type="text" name="email" placeholder="E-mail address" required />
        </label>
    </div>
    
    <div>
        <label>
            Confirm e-mail:
            <input type="text" name="confirmEmail" placeholder="Confirm e-mail address" required />
        </label>
    </div>
    
    <div>
        <button type="submit">Subscribe</button>
    </div>
</form>
```
We can then use the submit event on the form to send a request to segment the visitor through the JavaScript library:
```javascript
document.getElementById("newsletter-subscribe-form").addEventListener("submit", function (e) {
    console.info("\"submit\" event listener hit.", arguments);
    
    e.preventDefault();
    
    // TODO: Subscribe to newsletter via AJAX or somesuch.
    
    footprint.currentVisitor.addToSegment("newsletter", function (response, xhr) {
        console.info("\"addToSegment\" onSuccess.", arguments);
    }, function (xhr, statusText) {
        console.info("\"addToSegment\" onError.", arguments);
    });
});
```

## Getting started with Footprint.NET
If you do not wish to use the JavaScript library, you can use the .NET library included in the Umbraco package. To import the members of the .NET package, add a reference to the ncBehaviouralTargeting.Library.dll file included in the package.

Below is a quick guide to getting started for developers:

### Registration
In order to register behavioural data on a visitor, you will need to use the `CurrentVisitor` class.

#### User identity
A visitor will be given a unique identifier if they don't already have one. In case another identifier is already on file in a 3rd party system, such as a login system, then the following code should be used to override the users identity:
```csharp
void CurrentVisitor.SetVisitorId(string value);
```
#### Property registration
In order to register behavioural data and custom data for a visitor, the following two methods can be used:
```csharp
void CurrentVisitor.SetProperty(string key, object value);
void CurrentVisitor.SetProperty(Dictionary<string, object> values);
```
#### Quick start guide to content delivery
There are several ways to use a visitors segments to deliver content. A few helpers have been made available for easy use.

##### IPublishedContent extensions
The by far easiest way to deliver segmented content is through segmented properties on content nodes. By building data types on the `Footprint Content` property editor, it's possible to wrap any in-built and custom Umbraco property editors.

When fetching a segmented property, the following extension methods to <code>IPusblishedContent</code> have been made available:

```csharp         
object Model.Content.GetSegmentedValue(string propertyAlias);
T Model.Content.GetSegmentedValue<T>(string propertyAlias);
```
These work just as if you were using Umbracos own `GetPropertyValue` methods. If the property requested isn't a segmented property, it will fall back to Umbracos own `GetPropertyValue` method, so this can be substituted in most cases.

##### Razor
If custom code or logic needs to be executed based on a visitors segments, the following can be used:
```csharp
bool CurrentVisitor.IsInSegment(string segmentAlias);
```

## Data storage options
We support the following data storage options for storing visitor segments:

### Cookies
Adding the `<cookies />` to the Footprint configuration file will enable storing segments in cookies on the client. All the segments the visitor is added to, have a separate cookie with a separate expiration time depending on the Persistence setting of the segment.

### SQL database
Adding the `<sql connectionStringName="my connection string name" />` will enable storing segments in a SQL database. We only use a very small number of standard SQL statements so this should work on most SQL databases. Remember to specify the connection string name from the `<connectionStrings>` element in Web.config, NOT the connection string!

### MongoDB
Add the following element to the Footprint configuration file to enable storing segments (and visitor properties) in MongoDB:
```xml
<mongoDb
    connectionString="mongodb://connection-string-here/myDatabase"
    database="myDatabase"
    collection="myFootprintCollection" />
```
You can add any and all of the storage options! However, if you later add a storage option, that storage option will only collect data from that moment; it will not automagically get all the previous data from another storage option.
