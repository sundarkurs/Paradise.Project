(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('aboutController', aboutController);

    aboutController.$inject = ['$http', '$scope'];

    function aboutController($http, $scope) {

        var vm = this;
        vm.message = "Look! I am an about page.";
        
    }

})();




