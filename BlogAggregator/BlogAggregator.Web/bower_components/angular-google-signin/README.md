angular-google-signin
=====================

Angular module to integrate google signin button

Installation
------------

````
bower install angular-google-sigin
````

Then in your angular module add googleSignIn as a dependency.

Usage
----

Insert the directive inside your view:

```html
<google-sign-in client-id="your_client_id" scope="your_scope" text="your_text_inside_the_button" callback="callback">
</google-sign-in>
```

Define the callback

```javascript
'use strict';

angular
  .module('YourModule')
  .controller('YourController', function($scope) {
    $scope.callback = function(token) {

    };
  });
```

## Options

client-id: [Create a client ID](https://developers.google.com/+/web/signin/add-button-javascript#step_1_create_a_client_id_and_client_secret)

scope: [Available scopes](https://developers.google.com/+/api/oauth)

text: String

callback: A function with the valid token as argument

