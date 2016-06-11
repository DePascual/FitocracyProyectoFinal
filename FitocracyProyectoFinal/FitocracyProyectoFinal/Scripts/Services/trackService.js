﻿angular.module('Fitocracy')
    .service('trackService', function ($http) {

        this.recuperaPreMadeWorkouts = function () {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/recuperaWorkouts",
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.recuperaWorkoutsUsu = function () {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/recuperaWorkoutsUsu",
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.recuperaAllTracks = function () {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/recuperaAllTracks",
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };
      
        this.buscadorTracks = function (textoBusqueda) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/buscadorTracks",
                data: { "textoBusqueda": textoBusqueda.texto },
                dataType: "json"
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.guardaMyWork = function (Workout) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/GuardaMyWork",
                data: JSON.stringify(Workout),
                dataType: "json"
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.borrarMyWork = function (idMyWork) {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/BorraMyWork",
                data: { "idMyWork": idMyWork },
                dataType: "json"
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.recuperaCustomWorkouts = function () {
            var response = $http({
                method: "post",
                url: "/ZonaUsuarios/recuperaCustomWorkouts",
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };


        this.recuperaEntrenamiento = function (entrenamiento) {
            var response = $http({
                method: "post",
                url: "/Coach/recuperaEntrenamiento",
                data: { "idEntrenamiento": entrenamiento },
                dataType: "json"
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

        this.Compra = function (tarjetaUsuario, idEntrenamiento) {
            var response = $http({
                method: "post",
                url: "/Coach/compraEntrenamiento",
                data: JSON.stringify(tarjetaUsuario) ,
                dataType: "json"
            }).success(function (result) {
                response = result;
            }).error(function (data, status, headers, config) {
                response = status;
            });
            return response;
        };

    })