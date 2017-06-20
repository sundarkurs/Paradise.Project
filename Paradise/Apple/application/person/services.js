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
                    callback(response.data);
                });
        }

        function create(person, callback) {
            $http.post($rootScope.webApiUrl + '/Create', person)
                .success(function (response) {
                    callback(response);
                });
        }

    }

})();

