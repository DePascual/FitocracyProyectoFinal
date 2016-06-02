angular.module('Fitocracy')
    .service('registroService', function ($http) {

        this.UserRegistro = function (User) {

            var response = $http({
                method: "post",
                url: "/Home/Registro",
                data: JSON.stringify(User),
                dataType: "json"
            }).success(function (data, status, headers, config) {
                response = data;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        }
    })