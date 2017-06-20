(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('personController', personController);

    personController.$inject = ['$http', '$scope', 'personService'];

    function personController($http, $scope, personService) {
        var vm = this;
        vm.persons = [];
        vm.person = {};

        vm.personMessage = "Everyone come and see how good I look!";
        vm.create = create;
        //$scope.message = 'Everyone come and see how good I look!';

        (function initController() {
            getAll();
        })();

        function getAll() {
            personService.GetAll(function (response) {
                vm.persons = response;
            });
        };

        function create() {
            personService.Create(this.person, function (response) {
                debugger;
                vm.persons = response;
            });
        }
    }

})();




