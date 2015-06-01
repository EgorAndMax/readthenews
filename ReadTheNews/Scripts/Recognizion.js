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
        /*
        'вкладка *term': function(term)
        {
            if (term == 'прочитать позже')
            {
                location.replace("http://localhost:9710/RssNews/ReadItLater");
                msg.text = 'вкладка прочитать позже';
                speechSynthesis.speak(msg);
            }
            else if (term == 'избранные новости')
            {
                location.replace("http://localhost:9710/RssNews/MyFavoriteNews");
                msg.text = 'вкладка избранные новости';
                speechSynthesis.speak(msg);
            }
            else if (term == 'мои новости')
            {
                location.replace("http://localhost:9710/RssNews/MyNews");
                msg.text = 'вкладка мои новости';
                speechSynthesis.speak(msg);
            }
            else if (term == 'мои каналы')
            {
                location.replace("http://localhost:9710/RssNews/MyChannels");
                msg.text = 'вкладка мои каналы';
                speechSynthesis.speak(msg);
            }
            else if (term == 'каналы')
            {
                location.replace("http://localhost:9710/RssNews/Channels");
                msg.text = 'вкладка каналы';
                speechSynthesis.speak(msg);
            }
            else if (term == 'отчеты')
            {
                location.replace("http://localhost:9710/Reports/Index");
                msg.text = 'вкладка отчеты';
                speechSynthesis.speak(msg);
            }

        },
        */
        'категория :name': function (name) {
            if (name == 'мир' || name == 'спорт' || name == 'бывший ссср' || name == 'россия' || name == 'силовые структуры' || name == 'новости' || name == 'из жизни' || name == 'культура' || name == 'приложения') {
                location.replace("http://localhost:9710/RssNews/GetNewsByCategory?Name=" + name);
                msg.text = 'категория ' + name;
                speechSynthesis.speak(msg);
            }
            else {
                msg.text = 'категория не найдена';
                speechSynthesis.speak(msg);
            }
        },
        'обзорщик *command': function (command) {
            if (command == 'как дела') {
                msg.text = 'всё отлично';
                speechSynthesis.speak(msg);
            }
            else if (command == 'в чем смысл жизни') {
                msg.text = 'сорок два';
                speechSynthesis.speak(msg);
            }
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
        }
        
    };

    annyang.addCommands(commands);

    annyang.start();
}