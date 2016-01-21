Welcome to the Footprint documentation.
Footprint is a behavioral targeting and marketing automation plugin for the Umbraco CMS.
https://our.umbraco.org/projects/website-utilities/footprint/
- PS made by www.novicell.dk

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
