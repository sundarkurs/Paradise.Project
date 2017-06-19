(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('contactController', contactController);

    contactController.$inject = ['$http', '$scope'];

    function contactController($http, $scope) {
        var vm = this;
        vm.message = "Look! I am an about page.";
    }

})();




