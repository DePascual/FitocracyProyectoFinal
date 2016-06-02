angular.module('Fitocracy')
    .controller('loginCtrl', function ($scope, loginService, $window, $location) {

        //Muestra la ventana emergente
        $("#modalLogin").modal('show');
        $("#modalChangePass").modal('hide');
        $("#modalChangesPassOK").modal('hide');

        //Funciones
        $scope.logear = function (isValid) {
            if (isValid) {
                var usuario = {
                    Username: $scope.uName,
                    Password: $scope.uPass,
                    _id: "null"
                };

                //Envio de datos a loginService
                var getData = loginService.UserLogin(usuario);

                //Respuesta de loginService
                getData.then(function (msg) {
                    if (msg.data == "null") {
                        $("#errorLogin").css('display', 'block');
                        $('#uName').removeClass('ng-valid').addClass('ng-invalid');
                        $('#uPass').removeClass('ng-valid').addClass('ng-invalid');
                    }
                    else {
                        $window.sessionStorage["infoUsuario"] = JSON.stringify(msg.data);
                        $location.path("/ZonaUsuarios");
                        $("#modalLogin").modal('hide');
                    }
                })
            } else {
                alert('Algo va mal en el login');
            }
        };

        $scope.irARegistro = function () {
            $("#modalLogin").modal('hide');
            window.location.href = "#/Registro";
        };
    
        $scope.alertmsg = function () {
            $("#errorLogin").css('display', 'block');
        };

        $scope.cerrar = function () {
            $("#modalLogin").modal('hide');
            window.location.href = "#/Home"
        }

        $scope.cerrarChangePass = function () {
            $("#modalChangePass").modal('hide');
            $("#modalLogin").modal('show');
        }

        $scope.cerrarChangeOk = function () {
            $("#modalChangesPassOK").modal('hide');
            $("#modalLogin").modal('show');
        }


        $scope.irAChangePass = function () {
            $("#modalLogin").modal('hide');
            window.location.href = "#/ChangePass";
        };

       

    });