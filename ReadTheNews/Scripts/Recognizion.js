if (annyang) {

    annyang.setLanguage('ru');
    var msg = new SpeechSynthesisUtterance();
    var voices = window.speechSynthesis.getVoices();
    msg.voice = voices[10]; 
    msg.voiceURI = 'native';
    msg.volume = 1; 
    msg.rate = 1; 
    msg.pitch = 2; 
    msg.lang = 'ru-RU';
    

    var commands = {
        'привет': function () {
            msg.text = 'привет любитель новостей';
            speechSynthesis.speak(msg);
        },
        'прочитать позже': function () {
            location.replace("http://localhost:9710/RssNews/ReadItLater");
            msg.text = 'вкладка прочитать позже';
            speechSynthesis.speak(msg);
        },
        'избранные новости': function () {
            location.replace("http://localhost:9710/RssNews/MyFavoriteNews");
            msg.text = 'вкладка избранные новости';
            speechSynthesis.speak(msg);
        },
        'мои новости': function () {
            location.replace("http://localhost:9710/RssNews/MyNews");
            msg.text = 'вкладка мои новости';
            speechSynthesis.speak(msg);
        },
        'мои каналы': function () {
            location.replace("http://localhost:9710/RssNews/MyChannels");
            msg.text = 'вкладка мои каналы';
            speechSynthesis.speak(msg);
        },
        'отчеты': function () {
            location.replace("http://localhost:9710/Reports/Index");
            msg.text = 'вкладка отчеты';
            speechSynthesis.speak(msg);
        },
        'категория :name': function (name) {
            location.replace("http://localhost:9710/Reports/GetNewsByCategory?Name=" + name);
            msg.text = 'вкладка' + ':name';
            speechSynthesis.speak(msg);
        }
    };

    annyang.addCommands(commands);

    annyang.start();
}