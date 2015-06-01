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
            location.href = "ReadItLater";
            msg.text = 'вкладка прочитать позже';
            speechSynthesis.speak(msg);
        },
        'избранные новости': function () {
            location.href = "MyFavoriteNews";
            msg.text = 'вкладка избранные новости';
            speechSynthesis.speak(msg);
        },
        'мои новости': function () {
            location.href = "MyNews";
            msg.text = 'вкладка мои новости';
            speechSynthesis.speak(msg);
        },
        'мои каналы': function () {
            location.href = "MyChannels";
            msg.text = 'вкладка мои каналы';
            speechSynthesis.speak(msg);
        },
    };

    annyang.addCommands(commands);

    annyang.start();
}