(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('contactController', contactController);

    contactController.$inject = ['$http', '$scope'];

    function contactController($http, $scope) {
        $scope.message = 'Contact us! JK. This is just a demo.';
    }

})();




