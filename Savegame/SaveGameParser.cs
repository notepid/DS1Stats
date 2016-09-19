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

                    var newCharacterInfo = characterInfo;

                    var slotStart = SaveGameOffsets.SLOT_BEGIN + SaveGameOffsets.SLOT_LENGTH * i;
                    saveGameStream.Seek(slotStart, SeekOrigin.Begin);

                    //Character name
                    saveGameStream.Seek(SaveGameOffsets.CHARACTER_NAME, SeekOrigin.Current);
                    var buffer = reader.ReadBytes(32);
                    var characterName = Encoding.Unicode.GetString(FixNullterminatedString(buffer));
                    if (string.IsNullOrEmpty(characterName))
                        continue;
                    newCharacterInfo.Name = characterName;

                    //Number of deaths
                    saveGameStream.Seek(slotStart, SeekOrigin.Begin);
                    saveGameStream.Seek(SaveGameOffsets.CHARACTER_DEATHS, SeekOrigin.Current);
                    newCharacterInfo.Deaths = reader.ReadUInt32();

                    if (newCharacterInfo.SessionStartDeaths == 0)
                        newCharacterInfo.SessionStartDeaths = newCharacterInfo.Deaths;

                    //Character has died
                    if (characterInfo.Deaths != newCharacterInfo.Deaths)
                    {
                        newCharacterInfo.LastDeath = DateTime.UtcNow;
                        newCharacterInfo.SessionDeaths = newCharacterInfo.SessionStartDeaths - newCharacterInfo.Deaths;
                    }

                    characterInfoList[i] = newCharacterInfo;
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
