'use strict';
/* global gapi:false */

angular
  .module('googleSignIn', [])
  .value('version', '0.1')
  .directive('googleSignIn', function () {
    var render = function($scope) {
      var callback = $scope.callback();
      gapi.auth.authorize(
        {'client_id': $scope.clientId, 'scope': $scope.scope, 'cookie_policy': 'single_host_origin', 'immediate': false},
        callback);
    };
    var link = function($scope, el) {
      el.on('click', function() {
        render($scope);
      });
    };
    var controller = function() {
      var po, s;
      po = document.createElement('script');
      po.type = 'text/javascript';
      po.async = true;
      po.src = 'https://apis.google.com/js/client:plusone.js';
      s = document.getElementsByTagName('script')[0];
      s.parentNode.insertBefore(po, s);
    };
    var template = '' +
      '<div id=\'google-sign-in\' class=\'google-sign-in\'>' +
        '<div id=\'gSignInButton\' class=\'gSignInButton\'>' +
          '<span class=\'icon\'></span>' +
          '<span class=\'buttonText\'>{{text}}</span>' +
        '</div>' +
      '</div>';
    return {
      restrict: 'E',
      scope: {
        clientId: '@',
        scope: '@',
        text: '@',
        callback: '&'
      },
      link: link,
      controller: controller,
      template: template
    };
  });
