//<script src="//cdnjs.cloudflare.com/ajax/libs/annyang/1.6.0/annyang.min.js"></script>

if (annyang) {
    annyang.setLanguage('ru');



    var commands = {
        'привет': say
    };



    annyang.addCommands(commands);
    annyang.start();
}
