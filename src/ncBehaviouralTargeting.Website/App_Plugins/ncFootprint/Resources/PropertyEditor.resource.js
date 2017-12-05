angular.module("umbraco.resources")
    .factory("ncbtPropertyEditorResource", function ($http) {
        return {
            GetByAlias: function (alias) {
                return $http.get("backoffice/ncFootprintApi/PropertyEditor/GetByAlias?alias=" + alias);
            }
        };
    });