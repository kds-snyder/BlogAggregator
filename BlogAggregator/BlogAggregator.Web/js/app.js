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

         .state('app', { url: '/app', templateUrl: '/templates/app/app.html', controller: 'AppController' })
            .state('app.admin', { url: '/admin', templateUrl: '/templates/app/administration/admin.html', controller: 'AdminController' })
                .state('app.admin.blogs', { url: '/blogs', templateUrl: '/templates/app/administration/blogs.html', controller: 'AdminBlogsController', authenticate: true })
                .state('app.admin.users', { url: '/users', templateUrl: '/templates/app/administration/users.html', controller: 'AdminUsersController', authenticate: true })
 
            .state('app.addblog', { url: '/addblog', templateUrl: '/templates/app/addblog.html', controller: 'AddBlogController', authenticate: false })

            .state('app.posts', { url: '/posts', templateUrl: '/templates/app/posts.html', controller: 'PostsController', authenticate: false })
});

//API link
//angular.module('app').value('apiUrl', 'http://blogaggregator.azurewebsites.net/');
angular.module('app').value('apiUrl', 'http://localhost:3000/');

// Load authentication data
angular.module('app').run(function ($rootScope, authService, $state) {
    authService.loadAuthData();
   
    $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {       
        if (toState.authenticate && !(authService.authentication.isAuthenticated
            && authService.authentication.isAuthorized)) {
            $state.go('login');
            event.preventDefault();
        }
    });
    
});


