angular.module('Fitocracy')
    .service('entrenadorService', function ($http) {

        this.Cambio = function (entrenamiento) {
            var response = $http({
                method: "post",
                url: "/ZonaEntrenadores/UpdateEntrenamiento",
                data: JSON.stringify(entrenamiento),
                dataType: "json"
            }).success(function (data, status, headers, config) {
                response = data;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };
    })