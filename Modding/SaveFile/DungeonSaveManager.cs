using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Modding.SaveFile
{
    public static unsafe class DungeonSaveManager
    {
        public static DungeonSave*[] DungeonSaveList;
        public static readonly int DungeonSaveSize = sizeof(DungeonSave);
        static int DungeonListArraySize => Dungeons.DungeonListArraySize;
        public static readonly DungeonSave DefaultDungeonSave = new();
        static MemDataBaseCore* MemDataBaseCore => Globals.MemDataBaseCore;
        static DungeonSaveManager()
        {
            var saves = new DungeonSave*[DungeonListArraySize];
            for (int i = 0; i < DungeonListArraySize; i++)
            {
                saves[i] = GetDefaultDungeonSave();
            }
            DungeonSaveList = saves;

        }
        public static void OnSaveGame()
        {
            CustomSaveFile.OnSaveGame += SaveToFile;
            CustomSaveFile.OnLoadGame += LoadFromFile;
            CustomSaveFile.OnInitGame += InitSaveGame;
        }

        public static void ResetDungeonEntities()
        {
            for (int i = 0; i < DungeonListArraySize; i++)
            {
                var dungeon = DungeonSaveList[i];
                if (dungeon == null) break;
                dungeon->GatherSpotCollectedBitfield = 0;
                dungeon->EnemyAliveBitfield = 0;
                dungeon->StatusBitfield &= 0xffef;
            }
        }
        public static bool IsDungeonSaveInList(ushort dungeonID) => GetDungeonSave(dungeonID) != null;
        public static DungeonSave* GetDefaultDungeonSave()
        {
            var result = (DungeonSave*)Marshal.AllocHGlobal(sizeof(DungeonSave));
            fixed (DungeonSave* de = &DefaultDungeonSave)
                Buffer.MemoryCopy(de, result, sizeof(DungeonSave), sizeof(DungeonSave));
            return result;
        }
        public static DungeonSave* CreateDungeonSave(ushort dungeonID)
        {
            if (dungeonID == 99)
                dungeonID = 0xc9;
            DungeonSave* result = GetDungeonSave(dungeonID);
            if (result != null) return result;
            Dungeon* dungeonDBEntry = Dungeons.GetDungeon(dungeonID);
            if (dungeonDBEntry == null) return null;

            result = GetDefaultDungeonSave();
            int i = 0;
            for(; i < DungeonListArraySize; i++)
            {
                if (DungeonSaveList[i]->DungeonID == 0) break;
            }
            DungeonSaveList[i] = result;
            result->DungeonID = dungeonID;
            result->SubWorld = dungeonDBEntry->SubWorld;
            return result;
        }
        public static DungeonSave* GetDungeonSaveOrCreateNew(ushort dungeonID)
        {
            DungeonSave* result = GetDungeonSave(dungeonID);

            if (result == null)
                result = CreateDungeonSave(dungeonID);

            return result;
        }
        public static DungeonSave* GetDungeonSave(ushort dungeonID)
        {
            for (int i = 0; i < MemDataBaseCore->DungeonSaveArraySize; i++)
            {
                if (DungeonSaveList[i] == null) continue;
                if (DungeonSaveList[i]->DungeonID == dungeonID)
                    return DungeonSaveList[i];
            }
            return null;
        }
        public static bool ValidateDungeon(int dungeonId, int subworld)
        {
            for (int i = 0; i < DungeonListArraySize; i++)
            {
                var dungeon = DungeonSaveList[i];
                if (dungeon == null) break;

                if (((short)dungeon->StatusBitfield & 2) != 0 && dungeon->SubWorld == subworld)
                {
                    //if DungeonFile exist with ID dungeonID
                    var dungeonDB = Dungeons.GetDungeon(dungeon->DungeonID);
                    if (dungeonDB != null)
                        return true;
                }
            }
            return false;
        }
        public static void AddStellaDungeon(Dungeon* dungeon)
        {

        }
        public static bool AddDungeon(Dungeon* dungeon, bool enable)
        {
            if (dungeon == null) return false;
            var dungeonId = dungeon->DungeonID;
            if (dungeonId == 0) return false;
            var dungeonSave = GetDungeonSave(dungeonId);
            if (dungeonSave == null)
            {
                dungeonSave = CreateDungeonSave(dungeonId);
            }
            if (enable)
            {
                if ((dungeonSave->StatusBitfield & 2) == 0)
                    dungeonSave->StatusBitfield |= 0xf;
            }
            else
                dungeonSave->StatusBitfield &= 0xFFFD;
            AddStellaDungeon(dungeon);
            return true;
        }
        public static void SaveToFile(int saveFile)
        {
            var path = CustomSaveFile.Path;
            string filePath = $"\\Dungeons\\Test_{saveFile}.sav";
            var file = File.Create(path + filePath);
            for (int i = 0; i < Dungeons.DungeonListArraySize; i++)
            {
                var entry = DungeonSaveList[i];
                var buffer = new byte[Marshal.SizeOf(typeof(DungeonSave))];
                fixed (void* d = &buffer[0])
                {
                    Buffer.MemoryCopy(entry, d, buffer.Length, buffer.Length);
                }
                file.Write(buffer);
            }
            file.Close();
        }
        public static void LoadFromFile(int saveFile)
        {
            InitSaveGame();
            var path = CustomSaveFile.Path;
            string filePath = $"\\Dungeons\\Test_{saveFile}.sav";
            if (!File.Exists(path + filePath)) return;
            var file = File.ReadAllBytes(path + filePath);
            int index = 0;
            var size = sizeof(DungeonSave);
            while (size * index + size < file.Length)
            {
                ReadOnlySpan<byte> entry = new Span<byte>(file, index * size, size);
                var buffer = entry.ToArray();
                DungeonSave* save = DungeonSaveList[index];
                fixed (byte* ptr = buffer)
                {
                    Buffer.MemoryCopy(ptr, save, size, size);
                }
                DungeonSaveList[index] = save;
                index++;
            }
        }
        public static void InitSaveGame()
        {
            var s = new DungeonSave();
            for(int i = 0; i< DungeonListArraySize; i++)
            {
                Buffer.MemoryCopy(&s, DungeonSaveList[i], DungeonSaveSize, DungeonSaveSize);
            }
        }

    }
}
