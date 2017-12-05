angular.module("umbraco.resources")
    .factory("ncbtQueryResource", function ($http) {
        var baseUrl = "backoffice/ncFootprintApi/Query";

        return {
            GetEndpoint: function() {
                return baseUrl;
            },

            Search: function (excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases, page) {
                if (typeof excludeIncompleteProfiles === "undefined") {
                    excludeIncompleteProfiles = false;
                }

                if (typeof excludeNcbtProperties === "undefined") {
                    excludeNcbtProperties = false;
                }

                if (segmentAliases) {
                    segmentAliases = segmentAliases.join(",");
                } else {
                    segmentAliases = "";
                }

                if (typeof page === "undefined") {
                    page = 0;
                }

                return $http.get(baseUrl
                    + "/Search"
                    + "?excludeIncompleteProfiles=" + excludeIncompleteProfiles
                    + "&excludeNcbtProperties=" + excludeNcbtProperties
                    + "&segmentAliases=" + segmentAliases
                    + "&page=" + page
                    + "&cache=" + Date.now());
            }
        };
    });