angular.module("umbraco.resources")
    .factory("ncbtPropertyResource", function($http) {
        return {
            GetById: function(id) {
                return $http.get("backoffice/ncFootprintApi/Property/GetById?id=" + id);
            },
            GetAllLight: function () {
                return $http.get("backoffice/ncFootprintApi/Property/GetAllLight");
            },
            Save: function (property) {
                return $http.post("backoffice/ncFootprintApi/Property/Save", property);
            },
            Delete: function (id) {
                return $http.delete("backoffice/ncFootprintApi/Property/Delete?id=" + id);
            }
        };
    });