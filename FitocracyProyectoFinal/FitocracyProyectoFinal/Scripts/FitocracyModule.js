var Fitocracy = angular.module('Fitocracy', ['ngRoute']);

var configFunction = function ($routeProvider, $locationProvider) {
    $routeProvider.
         when('/', {
             templateUrl: 'Home/Home',
         })
        .when('/Index', {
            templateUrl: 'Home/Index',
        })
        .when('/Home', {
            templateUrl: 'Home/Home',
        })
        .when('/Coach', {
            templateUrl: 'Coach/Home',
            //controller: 'entrenadoresCtrl'
        })
         //.when('/Coach/detalleEntrenamiento', {
         //    templateUrl: function (params) {
         //        return 'Coach/detalleEntrenamiento?idEntrenador=' + params.idEntrenador + '&idEntrenamiento=' + params.idEntrenamiento;
         //    }
         //})
        .when('/detalleEntrenamiento', {
            templateUrl: 'Coach/detalleEntrenamiento',
        })
        .when('/About', {
            templateUrl: 'Home/About',
        })
        .when('/Login', {
            templateUrl: 'Home/Login',
            controller: 'loginCtrl'
        })
        .when('/Registro', {
            templateUrl: 'Home/Registro',
            controller: 'registroCtrl'
        })
         .when('/ChangePass', {
             templateUrl: 'Home/ChangePass',
             controller: 'changePassCtrl'
         })


         .when('/ZonaUsuarios', {
             templateUrl: 'ZonaUsuarios/Home',
             controller: 'userCtrl'
         })
        .when('/Home_ZU', {
            templateUrl: 'ZonaUsuarios/Home',
        })
         .when('/You', {
             templateUrl: 'ZonaUsuarios/You',
             controller: 'youCtrl'
         })
        .when('/Track', {
            templateUrl: 'ZonaUsuarios/Track',
            controller: 'trackCtrl'
        })
        .when('/Connect', {
            templateUrl: 'ZonaUsuarios/Connect',
        })
        .when('/Leaders', {
            templateUrl: 'ZonaUsuarios/Leaders',
        })
        .when('/ZonaUsuarios/SignOut', {
            templateUrl: function (params) {
                return 'ZonaUsuarios/SignOut?idUsuario=' + params.idUsuario;
            },
            controller: 'userCtrl'
        })
         .when('/WorkoutDoneAlert', {
             templateUrl: 'ZonaUsuarios/WorkoutDoneAlert',
         })

        .when('/ZonaEntrenadores', {
            templateUrl: 'ZonaEntrenadores/Home',
            controller: 'entrenadoresCtrl'
        });



    //$locationProvider.html5Mode(true).hashPrefix('!');
}
configFunction.$inject = ['$routeProvider', '$locationProvider'];

var compareTo = function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
};

Fitocracy.directive("compareTo", compareTo);

Fitocracy.config(configFunction);