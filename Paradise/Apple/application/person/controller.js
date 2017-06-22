(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('personController', personController);

    personController.$inject = ['$http', '$scope', 'personService'];

    function personController($http, $scope, personService) {
        
        $scope.persons = [];
        $scope.person = {};
        $scope.create = create;
        $scope.personMessage = "from scope";
        
        (function initController() {
            getAll();
        })();

        function getAll() {
            $("#spanMessage").show();
            personService.GetAll(function (response) {
                $("#spanMessage").hide();
                $scope.persons = response;
            });
        };

        function create() {
            personService.Create(this.person, function (response) {
                debugger;
                $scope.persons = response;
            });
        }
    }

})();




