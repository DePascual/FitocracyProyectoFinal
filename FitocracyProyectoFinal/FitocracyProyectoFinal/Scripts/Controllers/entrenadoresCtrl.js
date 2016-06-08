angular.module('Fitocracy')
    .controller('entrenadoresCtrl', function ($scope, $http, $compile, $window, $location) {
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
                                    + '<li><a class="pointer" value="custom" href="#/Alerts">Alerts</a></li>'
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
                $('li[coach=true]').hide();
                $('#dropCoach').children().remove();
                $('#dropCoach').attr('activo', 'false');
                $('#linkHome a').attr('href', '#/Home');
                $('#linkLogin').show();
            })
            $location.path("/Home");
        };

        $scope.editEntrenamiento = function (id) {
            alert(id);
            $.getJSON('/ZonaEntrenadores/EditEntrenamiento', {"idEntrenamiento": id});
        };

    })