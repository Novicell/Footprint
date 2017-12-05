angular.module("umbraco.resources")
    .factory("ncbtSegmentOperatorResource", function($http) {
        return {
            GetById: function(id) {
                return $http.get("backoffice/ncFootprintApi/SegmentOperator/GetById?id=" + id);
            },
            GetAllLight: function() {
                return $http.get("backoffice/ncFootprintApi/SegmentOperator/GetAllLight");
            }
        };
    });