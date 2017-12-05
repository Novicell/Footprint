angular.module("umbraco.resources")
    .factory("ncbtDashboardResource", function ($http) {
        return {
            Get: function () {
                return $http.get("backoffice/ncFootprintApi/Dashboard/Get?cache=" + Date.now());
            }
        };
    });