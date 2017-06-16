(function () {
    'use strict';

    angular
        .module('appleApp')
        .controller('aboutController', aboutController);

    aboutController.$inject = ['$http', '$scope'];

    function aboutController($http, $scope) {
        $scope.message = 'Look! I am an about page.';
    }

})();




