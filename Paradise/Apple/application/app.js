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
                controller: 'mainController'
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

    // create the controller and inject Angular's $scope
    appleApp.controller('mainController', function ($scope) {
        // create a message to display in our view
        $scope.message = 'Everyone come and see how good I look!';
    });

    appleApp.controller('aboutController', function ($scope) {
        $scope.message = 'Look! I am an about page.';
    });

    appleApp.controller('contactController', function ($scope) {
        $scope.message = 'Contact us! JK. This is just a demo.';
    });

})();




