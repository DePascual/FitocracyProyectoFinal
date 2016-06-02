angular.module('Fitocracy')
    .controller('registroCtrl', function ($scope, registroService, $window, $location) {

        $("#modalRegistro").modal('show');
 
        $scope.irALogin = function () {
            $("#modalRegistro").modal('hide');
            window.location.href = "#/Login";
        }

        $scope.registrar = function (isValid) {
            if (isValid) {
                var usuario = {
                    Email: $scope.uEmail,
                    Username: $scope.uName,
                    Password: $scope.uPass
                };
              
                var getData = registroService.UserRegistro(usuario);

                getData.then(function (msg) {
                    if (msg.data == "null") {
                        $("#errorRegistro").css('display', 'block');
                    }
                    else {
                        $window.sessionStorage["infoUsuario"] = JSON.stringify(msg.data);
                        $location.path("/ZonaUsuarios");
                        $("#modalRegistro").modal('hide');
                    }
                })

            } else {
                alert('Algo malo pasa')
            }
        }
    })