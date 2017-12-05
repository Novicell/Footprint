angular.module("umbraco.resources")
    .factory("ncbtSegmentResource", function($http) {
        return {
            GetById: function (id) {
                return $http.get("backoffice/ncFootprintApi/Segment/GetById?id=" + id);
            },
            GetLightById: function (id) {
                return $http.get("backoffice/ncFootprintApi/Segment/GetLightById?id=" + id);
            },
            GetAllLight: function () {
                return $http.get("backoffice/ncFootprintApi/Segment/GetAllLight");
            },
            Save: function (segment) {
                return $http.post("backoffice/ncFootprintApi/Segment/Save", segment);
            },
            Delete: function (id) {
                return $http.delete("backoffice/ncFootprintApi/Segment/Delete?id=" + id);
            },
            GetVisitors: function (segment) {
                return $http.post("backoffice/ncFootprintApi/Segment/GetVisitors", segment);
            },
            GetNumberOfVisitors: function (segment) {
                return $http.post("backoffice/ncFootprintApi/Segment/GetNumberOfVisitors", segment);
            },
            GetQuery: function (segment) {
                return $http.post("backoffice/ncFootprintApi/Segment/GetQuery", segment);
            }
        };
    });