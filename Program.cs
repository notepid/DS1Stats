﻿using System;
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
            var saveGameFileName = "arntor.sl2";
            var saveGameFile = Path.Combine(saveGameDirectory, saveGameFileName);
            _outputDirectory = Directory.GetCurrentDirectory();

            Console.WriteLine($"Using {saveGameFile}");

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
            
            //Haxy way to wait for the file to be unlocked after an update
            var attemptNumber = 0;
            while (true)
            {
                if (++attemptNumber > maxNumberOfTries)
                {
                    Console.WriteLine($"Could not get read-mode on savegame file. Aborting attempt...");
                    return;
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

            //Buffer the save game so we don't block it
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            fileStream.Close();

            _characterInfoList = SaveGameParser.ParseSaveGame(memoryStream, characterInfoList);

            foreach (var characterInfo in _characterInfoList)
            {
                Console.WriteLine($"Saving data for {characterInfo.Value.Name} in slot #{characterInfo.Key}");
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_deaths.txt"), characterInfo.Value.Deaths.ToString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_name.txt"), characterInfo.Value.Name);
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_time_since_last_death.txt"), characterInfo.Value.LastDeath.ToLocalTime().ToLongTimeString());
                File.WriteAllText(Path.Combine(outputDirectory, $"ds1counter_{characterInfo.Key}_session_deaths.txt"), characterInfo.Value.SessionDeaths.ToString());

                File.WriteAllText(Path.Combine(outputDirectory, "ds1counter.json"), JsonConvert.SerializeObject(characterInfoList, Formatting.Indented));
            }
        }
    }
}