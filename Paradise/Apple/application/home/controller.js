(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('homeController', homeController);

    homeController.$inject = ['$http', '$scope'];

    function homeController($http, $scope) {
        $scope.message = 'Everyone come and see how good I look!';
    }

})();




