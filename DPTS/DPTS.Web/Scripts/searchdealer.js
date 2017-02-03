(function ($) {
    $.fn.displaySearchResultOnMap = function (searchData, enteredZipcode, enteredRadius) {
        var elementName = this;
        var map;
        var infoWindow;
        var lat, lng;
        var geocoder;

        function getLocation() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(geoSuccess, geoError);
            }
            else {
                console.log("Geolocation is not supported");
            }
        }

        var geoSuccess = function (position) {
            console.log(position);
            lat = position.coords.latitude;
            lng = position.coords.longitude;
            geoCodeZipCodeAndDrawCircle(true);
            //init();
        };

        var geoError = function (error) {
            console.log('Error occurred. Error code: ' + error.code);
            // error.code can be:
            //   0: unknown error
            //   1: permission denied
            //   2: position unavailable (error response from location provider)
            //   3: timed out
            geoCodeZipCodeAndDrawCircle(false);

            //geocoder = new google.maps.Geocoder();
            //geocoder.geocode({ 'address': enteredZipcode }, function (results, status) {
            //    if (status == google.maps.GeocoderStatus.OK) {
            //        lat = results[0].geometry.location.lat();
            //        lng = results[0].geometry.location.lng();
            //        init();
            //        map.setCenter(results[0].geometry.location);
            //        addMarkerAndInfoWindow(lat,
            //            lng, "Entered Zipcode Location: " + enteredZipcode, enteredZipcode);
            //        addCircle(lat, lng);

            //    } else {
            //        console.log("Geocode was not successful for the following reason: " + status);
            //    }
            //});
        };

        function geoCodeZipCodeAndDrawCircle(isGetCurrentLocationSuccess) {
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': enteredZipcode }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (!isGetCurrentLocationSuccess) {
                        lat = results[0].geometry.location.lat();
                        lng = results[0].geometry.location.lng();
                    }
                    init();
                    map.setCenter(results[0].geometry.location);

                    var content;

                    if (isGetCurrentLocationSuccess) {
                        content = '<div id="iw-container">' +
                                   '<div class="iw-content">' +
                                    '<div class="iw-subTitle">Your Current Location</div>' +
                                  '</div>' +
                                   '<div class="iw-bottom-gradient"></div>' +
                                 '</div>';
                        addMarkerAndInfoWindow(lat, lng, "You", content);
                        addCircle(lat, lng);

                        content = '<div id="iw-container">' +
                                   '<div class="iw-content">' +
                                    '<div class="iw-subTitle">Entered ZipCode</div>' +
                                    '<p>' + enteredZipcode + '</p>' +
                                  '</div>' +
                                  '<div class="iw-bottom-gradient"></div>' +
                                '</div>';

                        addMarkerAndInfoWindow(results[0].geometry.location.lat(),
                        results[0].geometry.location.lng(), "Entered Zipcode Location: " + enteredZipcode, content);
                        addCircle(results[0].geometry.location.lat(), results[0].geometry.location.lng());
                    }
                    else {
                        content = '<div id="iw-container">' +
                                 '<div class="iw-content">' +
                                    '<div class="iw-subTitle">Entered ZipCode</div>' +
                                    '<p>' + enteredZipcode + '</p>' +
                                  '</div>' +
                                 '<div class="iw-bottom-gradient"></div>' +
                               '</div>';
                        addMarkerAndInfoWindow(lat,
                            lng, "Entered Zipcode Location: " + enteredZipcode, content);
                        addCircle(lat, lng);
                    }
                } else {
                    console.log("Geocode was not successful for the following reason: " + status);
                }
            });
        }


        function addCircle(lat, lng) {
            var circleOptions = {
                strokeColor: '#0177b3',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#12a3eb',
                fillOpacity: 0.35,
                map: map,
                center: new google.maps.LatLng(lat, lng),
                radius: enteredRadius / 0.00062137,
            };

            var cityCircle = new google.maps.Circle(circleOptions);
        }

        function init() {
            console.log(elementName.attr("id"));
            var mapDiv = document.getElementById(elementName.attr("id"));

            // Initialize map
            map = new google.maps.Map(mapDiv, {
                center: new google.maps.LatLng(lat, lng),
                zoom: 18,
                mapTypeId: 'roadmap',
                mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU }
            });

            // Add marker for persons position
            addMarkerAndInfoWindow(lat, lng, "You");

            // Display searchr result
            displaySearchResult();
        }

        function addMarkerAndInfoWindow(latitude, longitude, title, content) {
            var marker = new google.maps.Marker({
                position: new google.maps.LatLng(
                              parseFloat(latitude),
                            parseFloat(longitude)),
                map: map,
                animation: google.maps.Animation.DROP,
                title: title,
                pixelOffset: new google.maps.Size(100, 140)
            });
            var infowindow = new google.maps.InfoWindow({
                content: content
            });
            marker.addListener('click', function () {
                infowindow.open(map, marker);
            });
        }

        function geoCodeAddress(address, title, content) {
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    map.setCenter(results[0].geometry.location);
                    addMarkerAndInfoWindow(results[0].geometry.location.lat(),
                        results[0].geometry.location.lng(), title, content);

                } else {
                    console.log("Geocode was not successful for the following reason: " + status);
                }
            });
        }

        function displaySearchResult() {
            if (searchData.length > 0) {

                if (searchData.length >= 2)
                    map.setZoom(8);

                $.each(searchData, function (index, value) {
                  //  var addressInfo = $.trim(value.Address1) + ", " + value.City + ", " + value.State + ", " + value.ZipCode;
                    //var website = value.WebSite == "Not available" ? 'Not available' : '<a href=' + value.WebSite + ' target="_blank">' + value.WebSite + '</a>';
                  //  var isNFSCertifed = value.NFICertificationNumber != null ? value.NFICertificationNumber : "None";



                    //var content = '<div id="iw-container">' +
                    //                 '<div class="iw-title">' + value.CompanyName + '</div>' +
                    //                 '<div class="iw-content">' +
                    //                   '<div class="iw-subTitle">Address</div>' +
                    //                   '<p>' + addressInfo + '</p>' +
                    //                   '<div class="iw-subTitle">Phone Number</div>' +
                    //                   '<p>' + value.PhoneNumber + '</p>' +
                    //                   '<div class="iw-subTitle">Website</div>' +
                    //                   '<p> ' + website + '</p>' +
                    //                   '<div class="iw-subTitle">NFI Certification Number</div>' +
                    //                   '<p> ' + value.NFICertificationNumber + '</p>' +
                    //                 '</div>' +
                    //                 '<div class="iw-bottom-gradient"></div>' +
                    //               '</div>';
                    geoCodeAddress(addressInfo, value.CompanyName, "temp");
                });
            }
        }

        getLocation();
    };
})(jQuery);
