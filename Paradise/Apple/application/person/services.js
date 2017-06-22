(function () {
    'use strict';

    angular
        .module('appleApp')
        .factory('personService', personService);

    personService.$inject = ['$http', '$rootScope'];

    function personService($http, $rootScope) {
        var service = {};
        service.GetAll = getAll;
        service.Create = create;

        return service;

        function getAll(callback) {
            $http.get($rootScope.webApiUrl + "/GetAll")
                .then(function (response) {
                    debugger;
                    callback(response.data);
                });

            //$http.get($rootScope.webApiUrl + "/GetAll", {timeout : 120})
            //    .then(function (response) {
            //        console.log('success');
            //        callback(response.data);
            //    }, function (data) {
            //        console.log("error handle");
            //    });

            //$http.get($rootScope.webApiUrl + "/GetAll", { timeout: 120 })
            //    .success(function (response) {
            //        debugger;
            //        console.log("success");
            //        callback(response);
            //    })
            //    .error(function (response) {
            //        debugger;
            //        console.log("error");
            //    });


        }

        function create(person, callback) {
            $http.post($rootScope.webApiUrl + '/Create', person)
                .success(function (response) {
                    callback(response);
                });
        }

    }

})();

