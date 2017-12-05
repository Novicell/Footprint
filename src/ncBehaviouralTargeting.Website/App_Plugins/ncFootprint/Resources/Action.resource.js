angular.module("umbraco.resources")
    .factory("ncbtActionResource", function($http) {
        return {
            GetById: function(id) {
                return $http.get("backoffice/ncFootprintApi/Action/GetById?id=" + id);
            },
            GetAllLight: function () {
                return $http.get("backoffice/ncFootprintApi/Action/GetAllLight");
            },
            Save: function (action) {
                return $http.post("backoffice/ncFootprintApi/Action/Save", action);
            },
            Delete: function (id) {
                return $http.delete("backoffice/ncFootprintApi/Action/Delete?id=" + id);
            }
        };
    });