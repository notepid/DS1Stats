using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DS1Stats.Models;

namespace DS1Stats.Savegame
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

                    //Character name
                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_NAME, SeekOrigin.Begin);
                    var buffer = reader.ReadBytes(32);
                    var characterName = Encoding.Unicode.GetString(FixNullterminatedString(buffer)).Trim();
                    if (string.IsNullOrEmpty(characterName))
                        continue;
                    characterInfo.Name = characterName;

                    //Number of deaths
                    var previousDeaths = characterInfo.Deaths;
                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_DEATHS, SeekOrigin.Begin);
                    characterInfo.Deaths = reader.ReadUInt32();

                    if (characterInfo.SessionStartDeaths == 0)
                        characterInfo.SessionStartDeaths = characterInfo.Deaths;

                    characterInfo.SessionDeaths = characterInfo.Deaths - characterInfo.SessionStartDeaths;
                    
                    //Character has died
                    if (previousDeaths != characterInfo.Deaths)
                    {
                        Console.WriteLine($"Death detected for {characterInfo.Name}");

                        var lifeSpan = (DateTime.UtcNow - characterInfo.LastDeath);
                        characterInfo.LifeDurations.Add(lifeSpan);
                        characterInfo.LastDeath = DateTime.UtcNow;
                    }

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Vitality, SeekOrigin.Begin);
                    characterInfo.Vitality = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Attunement, SeekOrigin.Begin);
                    characterInfo.Attunement = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Endurance, SeekOrigin.Begin);
                    characterInfo.Endurance = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Strength, SeekOrigin.Begin);
                    characterInfo.Strength = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Dexterity, SeekOrigin.Begin);
                    characterInfo.Dexterity = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Intelligence, SeekOrigin.Begin);
                    characterInfo.Intelligence = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Faith, SeekOrigin.Begin);
                    characterInfo.Faith = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Humanity, SeekOrigin.Begin);
                    characterInfo.Humanity = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Resistance, SeekOrigin.Begin);
                    characterInfo.Resistance = reader.ReadInt64();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Level, SeekOrigin.Begin);
                    characterInfo.Level = reader.ReadInt32();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Souls, SeekOrigin.Begin);
                    characterInfo.Souls = reader.ReadInt32();

                    reader.BaseStream.Seek(slotStart + SaveGameOffsets.CHARACTER_Earned_Souls, SeekOrigin.Begin);
                    characterInfo.EarnedSouls = reader.ReadInt64();

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
