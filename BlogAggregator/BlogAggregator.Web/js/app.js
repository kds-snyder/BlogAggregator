angular.module('app', ['directive.g+signin', 'LocalStorageModule', 'ngMaterial', 'ngMessages', 'ngResource', 'ngSanitize', 'ui.router'])
                .config(function ($httpProvider, $mdThemingProvider, $stateProvider, $urlRouterProvider) {

    $httpProvider.interceptors.push('authInterceptorService');

    $mdThemingProvider.theme('default')
        .primaryPalette('cyan')
        .accentPalette('green');
    $mdThemingProvider.theme('toast-error');
    $mdThemingProvider.theme('toast-success');

    $urlRouterProvider.otherwise('/app/posts');

    $stateProvider
        .state('login', { url: '/login', templateUrl: '/templates/authentication/login.html', controller: 'LoginController', authenticate: false })

         .state('admin', { url: '/admin', templateUrl: '/templates/administration/admin.html', controller: 'AdminController' })
            .state('admin.blogs', { url: '/blogs', templateUrl: '/templates/administration/blogs.html', controller: 'AdminBlogsController', authenticate: false })
            .state('admin.users', { url: '/users', templateUrl: '/templates/administration/users.html', controller: 'AdminUsersController', authenticate: false })
 
        .state('app', { url: '/app', templateUrl: '/templates/app/app.html', controller: 'AppController' })
            .state('app.posts', { url: '/posts', templateUrl: '/templates/app/posts.html', controller: 'PostsController', authenticate: false })
            .state('app.addblog', { url: '/addblog', templateUrl: '/templates/app/addblog.html', controller: 'AddBlogController', authenticate: false })

});

//API link
//angular.module('app').value('apiUrl', 'https://microsoft-apiappc17fc6a308404673b96dabcfab653613.azurewebsites.net/');
angular.module('app').value('apiUrl', 'http://localhost:3000/');

// Application client ID
angular.module('app').value('appClientID', '620597621300-lt136jc3pa1i94v0cbqgtjvpsjda00hd.apps.googleusercontent.com');

// Load authentication data
angular.module('app').run(function ($rootScope, authService, $state) {
    authService.loadAuthData();
   
    $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
        if (toState.authenticate && !authService.authentication.isAuthenticated
            && !authService.authentication.isAuthorized) {
            $state.go('login');
            event.preventDefault();
        }
    });
    
});


