angular.module('Fitocracy')
    .service('youService', function ($http) {

        this.UpdateUser = function (User) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/UpdateUser",
                data: JSON.stringify(User),
                dataType: "json"
            }).success(function (data, status, headers, config) {
                response = data;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.UpdatePass = function (cambioPass) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/UpdatePassword",
                data: {'passOld' : cambioPass.PassOld, 'passNew': cambioPass.PassNew},
                dataType: "json"
            }).success(function (data, status, headers, config) {
                response = data;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.SendMessage = function (mensaje) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/Message",
                data: { 'areaMessage': mensaje },
                dataType: "json"
            }).success(function (data, status, headers, config) {
                response = data;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };


    })