using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DS1Counter.Models;

namespace DS1Counter.Savegame
{
    public class SaveGameParser
    {
        public static Dictionary<int, CharacterInfo> ParseSaveGame(MemoryStream saveGameStream, Dictionary<int,CharacterInfo> characterInfoList)
        {
            using (var reader = new BinaryReader(saveGameStream))
            {
                for (var i = 0; i < 10; i++)
                {
                    CharacterInfo characterInfo;
                    if (!characterInfoList.TryGetValue(i, out characterInfo))
                        characterInfo = new CharacterInfo();

                    var slotStart = SaveGameOffsets.SLOT_BEGIN + SaveGameOffsets.SLOT_LENGTH * i;
                    reader.BaseStream.Seek(slotStart, SeekOrigin.Begin);

                    //Character name
                    reader.BaseStream.Seek(SaveGameOffsets.CHARACTER_NAME, SeekOrigin.Current);
                    var buffer = reader.ReadBytes(32);
                    var characterName = Encoding.Unicode.GetString(FixNullterminatedString(buffer)).Trim();
                    if (string.IsNullOrEmpty(characterName))
                        continue;
                    characterInfo.Name = characterName;

                    //Number of deaths
                    reader.BaseStream.Seek(slotStart, SeekOrigin.Begin);
                    reader.BaseStream.Seek(SaveGameOffsets.CHARACTER_DEATHS, SeekOrigin.Current);
                    var previousDeaths = characterInfo.Deaths;
                    characterInfo.Deaths = reader.ReadUInt32();

                    if (characterInfo.SessionStartDeaths == 0)
                        characterInfo.SessionStartDeaths = characterInfo.Deaths;

                    characterInfo.SessionDeaths = characterInfo.Deaths - characterInfo.SessionStartDeaths;
                    
                    //Character has died
                    if (previousDeaths != characterInfo.Deaths)
                    {
                        characterInfo.LastDeath = DateTime.UtcNow;
                        Console.WriteLine($"Death detected for {characterInfo.Name}");
                    }

                    characterInfoList[i] = characterInfo;
                }
            }

            return characterInfoList;
        }

        public static byte[] FixNullterminatedString(byte[] bytes)
        {
            var buffer = new byte[bytes.Length];

            int stringLength;
            for (stringLength = 0; stringLength < bytes.Length; stringLength += 2)
            {
                if (bytes[stringLength] == 0) break;
                buffer[stringLength] = bytes[stringLength];
                buffer[stringLength + 1] = bytes[stringLength + 1];
            }

            var result = new byte[stringLength];

            for (var i = 0; i < stringLength; i++)
            {
                result[i] = buffer[i];
            }

            return result;
        }
    }
}
