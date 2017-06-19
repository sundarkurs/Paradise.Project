(function () {
    'use strict';

    angular
        .module('appleApp')
        .factory('personService', personService);

    personService.$inject = ['$http', '$rootScope'];

    function personService($http, $rootScope) {
        var service = {};
        var apiPrefix = "http://dev.apple.com/api/person";
        service.GetAll = getAll;

        return service;

        function getAll(callback) {
            $http.get($rootScope.webApiUrl + "/GetAll")
                .then(function (response) {
                    callback(response.data);
                });
        }

    }

})();

