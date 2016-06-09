angular.module('Fitocracy')
    .controller('userCtrl', function ($scope, $http, $compile, $window, $location) {

        if (sessionStorage.getItem("infoUsuario") != null) {
            var usuSession = JSON.parse(sessionStorage.getItem("infoUsuario"))

            $scope.usuario = {
                _id: usuSession._id,
                Username: usuSession.Username,
                Email: usuSession.Email
            }

            $('li[usu=true]').show();

            var dropUsuActivo = $('#dropUsu').attr('activo');
            if (dropUsuActivo == "false") {
                $('#dropUsu').attr('activo', 'true');
                $('#dropUsu').append('<a id="dynamic" class="dropdown-toggle pointer" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" style="color:#0079ff; font-weight:bold">' + usuSession.Username + '<span class="caret"></span></a>'
                                    + '<ul class="dropdown-menu" id="listUsu">'
                                    + '<li><a class="pointer" value="custom" href="#/You">Custom You</a></li>'
                                    + '<li><a class="pointer" id="linkGoHome" value="signOut">Sign Out</a></li>'
                                    + '</ul>');

                $('#linkGoHome').attr('ng-click', 'goHome()');
                $compile($('#linkGoHome'))($scope);
            }

            $('#linkHome a').attr('href', '#/Home_ZU');
            $('#linkLogin').hide();
        }


        $scope.goHome = function () {
            $window.sessionStorage.removeItem("infoUsuario");
            $http.post("/ZonaUsuarios/SignOut", { "idUsuario": usuSession._id }).success(function () {
                $('li[usu=true]').hide();
                $('#dropUsu').children().remove();
                $('#dropUsu').attr('activo', 'false');
                $('#linkHome a').attr('href', '#/Home');
                $('#linkLogin').show();
            })
            $location.path("/Home");
        };

        $scope.goHomeUsu = function () {
            $location.path("/Home_ZU");
        };

        $scope.goCoach = function () {
            $location.path("/Coach");
        };

        
    })