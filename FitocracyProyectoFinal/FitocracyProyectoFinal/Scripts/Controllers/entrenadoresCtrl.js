angular.module('Fitocracy')
    .controller('entrenadoresCtrl', function ($scope, $http, $compile, $window, $location, entrenadorService) {
        if (sessionStorage.getItem("infoEntrenador") != null) {
            var entrenadorSession = JSON.parse(sessionStorage.getItem("infoEntrenador"))

            $scope.entrenador = {
                _id: entrenadorSession._id,
                CoachName: entrenadorSession.Username,
            }

            $('#menuUsu').hide();
            $('#menuCoach').show();
            $('body').addClass('zonaEntrenadores');


            var dropCoachActivo = $('#dropCoach').attr('activo');
            if (dropCoachActivo == "false") {
                $('#dropCoach').attr('activo', 'true');
                $('#dropCoach').append('<a id="dynamic" class="dropdown-toggle pointer" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" style="color:#0079ff; font-weight:bold">' + entrenadorSession.CoachName + '<span class="caret"></span></a>'
                                    + '<ul class="dropdown-menu" id="listCoach">'
                                    + '<li><a class="pointer" id="linkGoHome" value="signOut">Sign Out</a></li>'
                                    + '</ul>');

                $('#linkGoHome').attr('ng-click', 'goHome()');
                $compile($('#linkGoHome'))($scope);
            }

            $('#linkHome a').attr('href', '#/Home');
            $('#linkLogin').hide();
        }


        $scope.goHome = function () {
            $window.sessionStorage.removeItem("infoEntrenador");
            $http.post("/ZonaEntrenadores/SignOut", { "idEntrenador": entrenadorSession._id }).success(function () {
                $('#menuUsu').show();
                $('#menuCoach').hide();
                $('body').removeClass('zonaEntrenadores');

                $('#dropCoach').children().remove();
                $('#dropCoach').attr('activo', 'false');
                $('#linkHome a').attr('href', '#/Home');
                $('#linkLogin').show();
            })
            $location.path("/Home");
        };

        $scope.editEntrenamiento = function (id) {
            $.getJSON('/ZonaEntrenadores/EditEntrenamiento', {"idEntrenamiento": id});
        };

        $scope.cambiarEntrenamiento = function (isValid) {
            if (isValid) {
                $('#idEntrenamiento').val();

                var entrenamiento = {
                    _id: $('#idEntrenamiento').val(),
                    NombreEntrenamiento: $scope.tName,
                    Duracion: $scope.tDuracion,
                    Precio: $scope.tPrecio,
                    Familia: $scope.tFamily,
                    Descripcion: $scope.tDescripcion
                }

                var getData = entrenadorService.Cambio(entrenamiento);
                getData.then(function (msg) {
                    if (msg.data != "False") {
                        $("#modalChangeWorkOK").modal('show');
                    }
                    else {
                        alert('Upsss error!!');
                    }
                });


            }
        };

        $scope.cerrarChangesWorkOK = function () {
            $("#modalChangeWorkOK").modal('hide');
            $location.path("/ZonaEntrenadores");
        };

    })