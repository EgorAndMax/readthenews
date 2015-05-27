
if (annyang) {
    annyang.setLanguage('ru');


    var commands = {
        'категория :name': function(name)
        {
            alert(name);
            console.log("start");
            var href = "http://localhost:9710/RssNews/GetNewsByCategory?name=" + name;
            console.log(href);
            location.replace(href);
            console.log("finish");
        },
        'избранное': function () {
   
            var href = "http://localhost:9710/RssNews/MyFavoriteNews";
            alert(href);
            location.replace(href);
        },
        'новости': function () {
            var href = "http://localhost:9710/RssNews/MyNews";
            alert(href);        
            location.replace(href);
        }

    };


    annyang.addCommands(commands);
    annyang.start();

}
