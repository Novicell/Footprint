angular.module("umbraco.resources")
    .factory("ncbtSegmentUtilityResource", function($http) {
        return {
            GetDataTypes: function(id) {
                return $http.get("backoffice/ncFootprintApi/SegmentUtility/GetDataTypes");
            },
            GetOperatorTypes: function() {
                return $http.get("backoffice/ncFootprintApi/SegmentUtility/GetOperatorTypes");
            },
            GetProperties: function () {
                return $http.get("backoffice/ncFootprintApi/SegmentUtility/GetProperties");
            }
        };
    });