angular.module('Fitocracy')
    .controller('changePassCtrl', function ($scope, loginService, $window, $location) {

        $("#modalChangePass").modal('show');

        $scope.generaNuevaPass = function (isValid) {
            if (isValid) {
                var getData2 = loginService.NuevaPass($scope.uEmail);
                getData2.then(function (msg) {
                    if (msg.data != "False") {
                        alert('ok!')
                        $("#modalChangePass").modal('hide');
                        $('#modalChangesPassOK').modal('show');
                    } else {
                        alert('fallo')
                    }
                })
            }

        };

        $scope.cerrar = function () {
            $("#modalChangePass").modal('hide');
            window.location.href = "#/Home"
        };

        $scope.cerrarAlert = function () {
            $("#modalChangesPassOK").modal('hide');
            window.location.href = "#/Login";
        };
    })