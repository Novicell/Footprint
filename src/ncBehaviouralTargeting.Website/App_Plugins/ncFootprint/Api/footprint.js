"use strict";

var footprint = function () {
    var prepareXhr = function (xhr, onSuccess, onError) {
        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 300) {
                if (onSuccess) {
                    onSuccess(xhr.response, xhr);
                }
            } else if (xhr.status >= 400) {
                if (onError) {
                    onError(xhr, xhr.statusText);
                }
            }
        };

        if (onError) {
            xhr.onerror = function () {
                onError(xhr, xhr.statusText);
            };
        }
    };

    var baseUrl = "/umbraco/ncFootprintApi";
    var currentVisitorBaseUrl = baseUrl + "/CurrentVisitor";

    return {
        currentVisitor: {
            setBaseUrl: function (newBaseUrl) {
                if (newBaseUrl) {
                    currentVisitorBaseUrl = newBaseUrl;
                }
            },

            addToSegment: function (segmentAlias, onSuccess, onError) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", currentVisitorBaseUrl + "/AddToSegment?segmentAlias=" + segmentAlias);
                xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                prepareXhr(xhr, onSuccess, onError);
                xhr.send();
            },

            setProperties: function (properties, onSuccess, onError) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", currentVisitorBaseUrl + "/SetProperties");
                xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
                prepareXhr(xhr, onSuccess, onError);
                xhr.send(JSON.stringify(properties));
            },

            isInSegment: function (segmentAlias, onSuccess, onError) {
                var xhr = new XMLHttpRequest();
                xhr.open("GET", encodeURI(currentVisitorBaseUrl + "/IsInSegment?segmentAlias=" + segmentAlias));
                xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xhr.setRequestHeader("Accept", "application/json");
                prepareXhr(xhr, onSuccess, onError);
                xhr.send();
            },

            setId: function (visitorId, onSuccess, onError) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", encodeURI(currentVisitorBaseUrl + "/SetId?visitorId=" + visitorId));
                xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                prepareXhr(xhr, onSuccess, onError);
                xhr.send();
            }
        }
    };
}();
