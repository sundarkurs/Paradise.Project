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

})();




