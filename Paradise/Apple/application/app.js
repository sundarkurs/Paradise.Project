(function() {
    'use strict';

    // create the module and name it appleApp
    var appleApp = angular.module('appleApp', ['ngRoute']);

    // configure our routes
    appleApp.config(function ($routeProvider) {
        $routeProvider

            // route for the home page
            .when('/', {
                templateUrl: 'application/home/index.html',
                controller: 'homeController'
            })

            .when('/person', {
                templateUrl: 'application/person/index.html',
                controller: 'personController'
            })

            .when('/person/create', {
                templateUrl: 'application/person/create.html',
                controller: 'personController'
            })

            // route for the about page
            .when('/about', {
                templateUrl: 'application/about/index.html',
                controller: 'aboutController'
            })

            // route for the contact page
            .when('/contact', {
                templateUrl: 'application/contact/index.html',
                controller: 'contactController'
            });
    });

    appleApp.run(function($rootScope) {
        $rootScope.webApiUrl = "http://dev.apple.com/api/person";
    });


})();




