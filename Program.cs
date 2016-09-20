using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using DS1Counter.Models;
using DS1Counter.Savegame;
using Newtonsoft.Json;

namespace DS1Counter
{
    class Program
    {
        private static Dictionary<int, CharacterInfo> _characterInfoList;
        private static string _outputDirectory;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            var saveGameDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"NBGI\DarkSouls\");
            var saveGameFileName = "DRAKS0005.sl2";
            var saveGameFile = Path.Combine(saveGameDirectory, saveGameFileName);
            _outputDirectory = Directory.GetCurrentDirectory();

            Console.WriteLine($"Using {saveGameFile}");

            if (!File.Exists(saveGameFile))
            {
                Console.WriteLine("Could not find savegame file...");
                Console.ReadKey();
                return;
            }

            var fwatcher = new FileSystemWatcher
            {
                Path = saveGameDirectory,
                Filter = saveGameFileName,
                NotifyFilter = NotifyFilters.LastWrite
            };
            fwatcher.Changed += FwatcherOnChanged;
            fwatcher.EnableRaisingEvents = true;

            //build initial info
            _characterInfoList = new Dictionary<int, CharacterInfo>();
            DoParse(saveGameFile, _outputDirectory, _characterInfoList);

            //Timer to update "time since last death"
            var timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 1000,
                Enabled = true
            };
            timer.Elapsed += TimerOnElapsed;

            Console.WriteLine("Monitoring save game file for changes. Press Q to quit.");
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (var characterInfo in _characterInfoList)
            {
                var timeSpan = (DateTime.UtcNow - characterInfo.Value.LastDeath);
                File.WriteAllText(Path.Combine(_outputDirectory, $"ds1counter_{characterInfo.Key}_timespan_last_death.txt"), timeSpan.ToString(@"hh\:mm\:ss"));
            }
        }

        private static void FwatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Console.WriteLine($"Detected changes in file {fileSystemEventArgs.FullPath} Parsing...");
            DoParse(fileSystemEventArgs.FullPath, _outputDirectory, _characterInfoList);
        }

        private static void DoParse(string saveGameFile, string outputDirectory, Dictionary<int, CharacterInfo> characterInfoList, int maxNumberOfTries = 20)
        {
            FileStream fileStream;

            if (!TryOpenFile(saveGameFile, maxNumberOfTries, out fileStream))
            {
                Console.WriteLine("Could not get read-mode on savegame file. Aborting attempt...");
                return;
            }

            //Buffer the save game so we don't block it
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            fileStream.Close();

            _characterInfoList = SaveGameParser.ParseSaveGame(memoryStream, characterInfoList);

            WriteOutputFiles(outputDirectory, characterInfoList);
        }

        private static void WriteOutputFiles(string outputDirectory, Dictionary<int, CharacterInfo> characterInfoList)
        {
            foreach (var characterInfo in _characterInfoList)
            {
                Console.WriteLine($"Saving data to files for {characterInfo.Value.Name} in slot #{characterInfo.Key}");
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_deaths.txt"), characterInfo.Value.Deaths.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_name.txt"), characterInfo.Value.Name);
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_time_last_death.txt"), characterInfo.Value.LastDeath.ToLocalTime().ToLongTimeString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_session_deaths.txt"), characterInfo.Value.SessionDeaths.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_session_deaths_total_deaths.txt"), $"{characterInfo.Value.SessionDeaths} / {characterInfo.Value.Deaths}");
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_average_life_length.txt"), characterInfo.Value.CalculateAverageLifeLength().ToString(@"hh\:mm\:ss"));

                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Vitality.txt"), characterInfo.Value.Vitality.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Attunement.txt"), characterInfo.Value.Attunement.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Endurance.txt"), characterInfo.Value.Endurance.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Strength.txt"), characterInfo.Value.Strength.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Dexterity.txt"), characterInfo.Value.Dexterity.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Intelligence.txt"), characterInfo.Value.Intelligence.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Faith.txt"), characterInfo.Value.Faith.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Humanity.txt"), characterInfo.Value.Humanity.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Resistance.txt"), characterInfo.Value.Resistance.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Level.txt"), characterInfo.Value.Level.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_Souls.txt"), characterInfo.Value.Souls.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_stats_EarnedSouls.txt"), characterInfo.Value.EarnedSouls.ToString());

                File.WriteAllText(Path.Combine(outputDirectory, "ds1counter.json"), JsonConvert.SerializeObject(characterInfoList, Formatting.Indented));
            }
        }

        private static bool TryOpenFile(string saveGameFile, int maxNumberOfTries, out FileStream fileStream)
        {
            //Haxy way to wait for the file to be unlocked after an update
            var attemptNumber = 0;
            while (true)
            {
                if (++attemptNumber > maxNumberOfTries)
                {
                    fileStream = null;
                    return false;
                }

                try
                {
                    fileStream = File.Open(saveGameFile, FileMode.Open);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }
            return true;
        }
    }
}
