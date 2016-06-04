angular.module('Fitocracy')
    .controller('trackCtrl', function ($scope, trackService, $window, $compile) {
        recuperaPreMadeWorkouts();
        recuperaAllTracks();

        $scope.buscar = function () {
            var textoBusqueda = {
                texto: $scope.textBusqueda
            }

            var getBusqueda = trackService.buscadorTracks(textoBusqueda);
            getBusqueda.then(function (msg) {
                $scope.tracksEncontrados = msg.data;
            });
            $('#divBusqueda').show();
        };

        $scope.showDiv = function (divId, obj) {

            switch (divId) {
                case "preMadeWorkouts":
                    sessionStorage.getItem("workouts") == null ? recuperaPreMadeWorkouts() : $scope.preMadeWorkouts = JSON.parse(sessionStorage.getItem("workouts"));
                    break;

                case "recentWorkouts":
                    recuperaWorkoutsUsu();
                    break;

                case "allTracks":
                    sessionStorage.getItem("allTracks") == null ? recuperaAllTracks() : $scope.allTracks = JSON.parse(sessionStorage.getItem("allTracks"));
                    break;

                case "customWorkouts":
                    recuperaCustomWorkouts();
                    break;

            }

            var links = $('#subMenu').find('a[show]');

            $.each(links, function (pos, el) {
                if ($(this).attr('show') == 'true') {
                    var idDiv = $(this).attr('id').split('link_')[1];
                    $('#' + idDiv).hide();
                    $('#link_' + idDiv).attr('show', 'false');
                    $('#link_' + idDiv).children().removeClass("fa fa-chevron-down").addClass("fa fa-chevron-up");
                }
            })

            var visible = obj.target.attributes.show.value;
            if (visible == 'false') {
                $('#' + divId).show();
                $('#link_' + divId).attr('show', 'true');
                $('#link_' + divId).children().removeClass("fa fa-chevron-up").addClass("fa fa-chevron-down");
            } else {
                $('#' + divId).hide();
                $('#link_' + divId).attr('show', 'false');
                $('#link_' + divId).children().removeClass("fa fa-chevron-down").addClass("fa fa-chevron-up");
            }
        };

        $scope.descripcionEnPartial = function (divId, obj) {
            var idHandler = obj.target.attributes.data.value;

            if (divId == 'customWorkouts') {
                $.each($scope.customWorkouts, function (pos, el) {
                    if (el._id == idHandler) {
                        $scope.workout = el;
                        $scope.Tracks = el.Tracks;
                    }
                })
            } else {
                $.each($scope.preMadeWorkouts, function (pos, el) {
                    if (el._id == idHandler) {
                        $scope.workout = el;
                        $scope.Tracks = el.Tracks;
                    }
                })
            }

            $('#vistaParcial').load("http://" + window.location.host + "/PartialsViews/TablaDatosWorkout", { "workout": $scope.workout, "tracks": $scope.Tracks });
        };

        $scope.trackEnPartial = function (obj) {

            if ($('#divBusqueda').css('display') == 'block') {
                $('#divBusqueda').hide()
            };

            var idHandler = obj.target.attributes.data.value;
            $.each($scope.allTracks, function (pos, el) {
                if (el._id == idHandler) {
                    $scope.Track = el;

                    $('#vistaParcial').empty();
                    var ruta = "<iframe src=" + $scope.Track.Link + " allowfullscreen='' frameborder='0' style='height:500px; width:100%'></iframe>";
                    $('#vistaParcial').append(ruta);
                }
            })
        };




        $scope.workoutsFactory = function () {
            $('#vistaParcial').load("http://" + window.location.host + "/PartialsViews/workoutsFactory", function () {
                $compile($('#vistaParcial'))($scope);
            });
        };

        function recuperaPreMadeWorkouts() {
            var getData = trackService.recuperaPreMadeWorkouts();
            getData.then(function (msg) {
                $scope.preMadeWorkouts = msg.data;

                if (window.location.host == "localhost:1284") {
                    sessionStorage["workouts"] = JSON.stringify(msg.data);
                } else {
                    sessionStorage["workouts"] = msg.data;
                }

            })
        };

        function recuperaWorkoutsUsu() {
            var getData2 = trackService.recuperaWorkoutsUsu();
            getData2.then(function (msg) {
                $scope.recentWorkouts = msg.data;
            })
        };

        function recuperaAllTracks() {
            var getData3 = trackService.recuperaAllTracks();
            getData3.then(function (msg) {
                $scope.allTracks = msg.data;
                if (window.location.host == "localhost:1284") {
                    sessionStorage["allTracks"] = JSON.stringify(msg.data);
                } else {
                    sessionStorage["allTracks"] = msg.data;
                }
            })
        };

        function recuperaCustomWorkouts() {
            var getData5 = trackService.recuperaCustomWorkouts();
            getData5.then(function (msg) {
                $scope.customWorkouts = msg.data;
            })
        };


        $scope.myWorks = []

        $scope.Save = function () {
            var id;
            var link;
            $('#selectTrack option').each(function (pos, el) {
                if ($(this).val() == $('#selectTrack').val()) {
                    id = $(this).attr('id');
                    link = $(this).attr('link');
                }
            });

            $scope.myWorks.push({
                _id: id,
                Nombre: $scope.newWork.Work,
                Link: link,
                Series: $scope.newWork.Set,
                Repeticiones: $scope.newWork.Rep
            })
        };

        $scope.GuardaMyWork = function () {
            $scope.Workout = {
                Nombre: $scope.wName,
                Tracks: $scope.myWorks
            }

            var getData4 = trackService.guardaMyWork($scope.Workout);
            getData4.then(function (msg) {
                if (msg.data != "False") {
                    alert('Guardado')
                } else {
                    alert('Error guardando')
                }
            })
        }
    })