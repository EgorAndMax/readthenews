<<<<<<< HEAD
﻿
=======
﻿//<script src="//cdnjs.cloudflare.com/ajax/libs/annyang/1.6.0/annyang.min.js"></script>

>>>>>>> 234fad4face7083c077a38efa6d1cae117200109
if (annyang) {
    annyang.setLanguage('ru');


<<<<<<< HEAD
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

=======

    var commands = {
        'привет': say
    };



    annyang.addCommands(commands);
    annyang.start();
>>>>>>> 234fad4face7083c077a38efa6d1cae117200109
}
