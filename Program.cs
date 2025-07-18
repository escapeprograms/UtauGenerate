System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //language support


var player = new UtauGenerate.Player();

Thread.Sleep(3000);
Console.WriteLine("Player initialized.");

player.addNote(0, 500, 60, "hello");
player.addNote(500, 1000, 58, "everyone");
player.addNote(1500, 250, 61, "i m");
player.addNote(1750, 500, 62, "going");
player.addNote(2250, 250, 60, "to");
player.addNote(2500, 800, 63, "kill");
player.addNote(3300, 800, 62, "you");

player.addNote(5300, 500, 69, "skibidi");

player.play();

Thread.Sleep(9000);


string savePath = @"..\..\..\outputs\output_please.ustx";
player.exportUstx(savePath);

string outputWav = @"..\..\..\outputs\output_audio.wav";
await player.exportWav(outputWav);