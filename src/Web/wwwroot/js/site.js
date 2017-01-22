$(document).ready(function () {

    var jqxhr = $.getJSON("/api/beers", function (data) {
        $.getJSON("/api/Releases", function (sysData) {
            ko.applyBindings(new ViewModel(data, sysData));
        })
        

    })

    var ViewModel = function (data, sysData) {
        var self = this;
        self.filter = ko.observable("");
        data.forEach(function (pub) {
            pub.filteredBeers = ko.computed(function () {
                
                var filter = self.filter();
                if (!filter || filter == "None") {
                    var ret = pub.beers;

                    return ret;
                } else {
                    var ret = ko.utils.arrayFilter(pub.beers, function (i) {
                        //console.log(i);
                        return i.name.toLowerCase().search(filter.toLowerCase()) >= 0 || (i.type != null && i.type.toLowerCase().search(filter.toLowerCase()) >= 0) || (i.brewery != null && i.brewery.toLowerCase().search(filter.toLowerCase())) >= 0;
                    });

                    return ret;
                }
            });

        })
        sysData.forEach(function (pub) {
            pub.filteredBeers = ko.computed(function () {

                var filter = self.filter();
                if (!filter || filter == "None") {
                    var ret = pub.beers;

                    return ret;
                } else {
                    var ret = ko.utils.arrayFilter(pub.beers, function (i) {
                        //console.log(i);
                        return i.name.toLowerCase().search(filter.toLowerCase()) >= 0 || (i.type != null && i.type.toLowerCase().search(filter.toLowerCase()) >= 0) || (i.brewery != null && i.brewery.toLowerCase().search(filter.toLowerCase())) >= 0;
                    });

                    return ret;
                }
            });
            var d = new Date(pub.date);
            var datestring = d.getDate() + "/" + (d.getMonth() + 1) + " " + d.getFullYear();

            pub.name = pub.type + " " + datestring;

        })
        self.update = function () {
            $.get("api/beers/update", function (data) {
                location.reload();
            });
        }
        self.pubs = ko.observableArray(data);
        self.releases = ko.observableArray(sysData);
        self.pubMode = ko.observable(true);
        self.modeText = ko.observable("Systembolaget");
        self.changeMode = function () {
            self.pubMode(!self.pubMode());
            if (self.pubMode()) {
                self.modeText("Systembolaget");
            } else {
                self.modeText("Pubbar")
            }
        }
    }
});