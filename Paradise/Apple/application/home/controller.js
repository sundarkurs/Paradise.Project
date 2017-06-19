(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('homeController', homeController);

    homeController.$inject = ['$http', '$scope'];

    function homeController($http, $scope) {
        var vm = this;
        vm.message = "Look! I am an about page.";
    }

})();




