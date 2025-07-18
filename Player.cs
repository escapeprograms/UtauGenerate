using System;
using OpenUtau.Core.Ustx;
using OpenUtau.Core.Format;
using OpenUtau.Core;
using OpenUtau.Audio;
using OpenUtau.Classic;
using OpenUtau.Core.Util;
using System.Threading.Tasks;

namespace UtauGenerate
{
    public class Player
    {
        public UProject project = DocManager.Inst.Project;
        public UVoicePart part = new UVoicePart();
        public Player()
        {
            

            //initialize and load stuff
            Thread mainThread = Thread.CurrentThread;
            TaskScheduler mainScheduler = TaskScheduler.Default;
            DocManager.Inst.Initialize(mainThread, mainScheduler); // Initialize DocManager to load phonemizers and singers

            SingerManager.Inst.SearchAllSingers();
            var singers = ClassicSingerLoader.FindAllSingers();
            Console.WriteLine($"Found {singers.Count()} singers");

            var teto = singers.First();  //singers.FirstOrDefault(s => s.Name.Contains("Teto"));
            if (teto != null)
            {
                Console.WriteLine($"Found Teto: {teto.Id} - {teto.Name}");
            }
            else
            {
                throw new Exception("TETO not found.");
            }

            //load wavtool and resampler
            ToolsManager.Inst.SearchResamplers(); //TODO: make it actually load stuff
            ToolsManager.Inst.SearchWavtools();

            // Create a new UProject
            project = DocManager.Inst.Project;
            project.name = "My New Project";
            project.ustxVersion = new Version(0, 6);
            Ustx.AddDefaultExpressions(project);

            //assign singer and phonemizer
            project.tracks[0].singer = teto.Id;
            project.tracks[0].phonemizer = "OpenUtau.Plugin.Builtin.EnXSampaPhonemizer";
            //"OpenUtau.Plugin.Builtin.JapaneseCVVCPhonemizer"
            //"OpenUtau.Plugin.Builtin.ArpasingPlusPhonemizer";
            //"OpenUtau.Core.DefaultPhonemizer"
            //"OpenUtau.Plugin.Builtin.EnXSampaPhonemizer"
            project.tracks[0].AfterLoad(project); // load the singer + phonemes

            project.tracks[0].Phonemizer.SetSinger(project.tracks[0].Singer);
            // await Task.Sleep(2000); //WAIT TO LOAD ALL THE SHI

            resetParts();

            

            
        }

        public void resetParts()
        {
            //reset parts
            project.parts.Clear();
            part = new UVoicePart();
            part.trackNo = 0;
            part.position = 0;      // Start at the beginning
            part.Duration = 6000;    // Duration in ticks (adjust as needed)
            part.name = "Main Melody Skibidi";

            project.parts.Add(part);
        }

        public void addNote(int position, int duration, int tone, string lyric)
        {
            UNote note = project.CreateNote();
            note.position = position;      // Start at the specified position
            note.duration = duration;    // Duration in ticks
            note.tone = tone;         // MIDI number for the note
            note.lyric = lyric;       // Assign lyric to the note

            // Add the note to the voice part
            part.notes.Add(note);
        }

        public void validate()
        {
            project.ValidateFull();
        }

        public void testAudio()
        {
            //initialize audio output
            PlaybackManager.Inst.AudioOutput = new NAudioOutput(); //MiniAudioOutput()
            PlaybackManager.Inst.PlayTestSound();
        }
        public void play()
        {
            validate();
            //initialize audio output
            PlaybackManager.Inst.AudioOutput = new NAudioOutput(); //MiniAudioOutput()
            PlaybackManager.Inst.Play(project, 0);
        }

        public async Task exportWav(string outputPath)
        {
            validate();
            PlaybackManager.Inst.AudioOutput = new NAudioOutput();
            try
            {
                await PlaybackManager.Inst.RenderToFiles(project, outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public void exportUstx(string outputPath)
        {
            validate();
            project.BeforeSave();
            Ustx.Save(outputPath, project);
        }
    }
}