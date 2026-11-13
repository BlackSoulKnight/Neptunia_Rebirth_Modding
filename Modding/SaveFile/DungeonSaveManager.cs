using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Memory;
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
        //private static DungeonSave*[] DungeonSaveList;
        //private static Dictionary<int,IntPtr> Saves = new();
        private static ObjectDictonary<int,DungeonSave> Saves = new();

        public static readonly int DungeonSaveSize = sizeof(DungeonSave);
        static int DungeonListArraySize => Dungeons.DungeonListArraySize;
        public static readonly DungeonSave DefaultDungeonSave = new();
        static MemDataBaseCore* MemDataBaseCore => Util.MemDataBaseCore;
        static DungeonSaveManager()
        {
            var saves = new DungeonSave*[DungeonListArraySize];
        }
        public static void OnSaveGame()
        {
            CustomSaveFile.OnSaveGame += SaveToFile;
            CustomSaveFile.OnLoadGame += LoadFromFile;
            CustomSaveFile.OnInitGame += InitSaveGame;
        }

        public static DungeonSave* GetDungeonSave(ushort dungeonID)
        {
            if(!Saves.ContainsKey(dungeonID))
                return null;
            return Saves[dungeonID];
        }

        public static void ResetDungeonEntities()
        {
            foreach(IntPtr dungeon in Saves.Values)
            {
                var save = (DungeonSave*)dungeon;
                save->GatherSpotCollectedBitfield = 0;
                save->EnemyAliveBitfield = 0;
                save->StatusBitfield &= 0xffef;
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
            var result = GetDungeonSave(dungeonID);
            if (  result != null) return result;

            Dungeon* dungeonDBEntry = Dungeons.GetDungeon(dungeonID);
            if (dungeonDBEntry == null) return null;

            result = Saves.CreateDefault(dungeonID);
            result->DungeonID = dungeonID;
            result->SubWorld = dungeonDBEntry->SubWorld;
            return result;
        }

        public static DungeonSave* GetDungeonSaveOrCreateNew(ushort dungeonID)
        {
            DungeonSave* result;
            return (result = GetDungeonSave(dungeonID))!=null ? result : CreateDungeonSave(dungeonID);
        }
        public static bool ValidateDungeon(int dungeonId, int subworld)
        {
            foreach(var dungeon in Saves.Values) {
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
        public static int GetDungeonCountOnMap(int subworld)
        {
            int result = 0;
            foreach(var dungeon in Saves.Values)
            {
                if (((short)dungeon->StatusBitfield & 2) != 0 && (dungeon->SubWorld == subworld || subworld == 0))
                    result++;
            }
            return result;
        }
        public static int PlaceDungeonOnMap(uint subworld, nuint par2, uint maxCount)
        {
            Console.WriteLine($"Something pointer {par2.ToString("X")}");
            int result = 0;

            foreach (var dungeon in Saves.Values)
            {
                if (((short)dungeon->StatusBitfield & 2) != 0 && (dungeon->SubWorld == subworld || subworld == 0))
                {
                    var id = dungeon->DungeonID;
                    if (id == 99)
                        id = 0xc9;
                    //checkDungeonDB
                    Dungeon* dun = Dungeons.GetDungeon(id);
                    if (dun != null)
                    {
                        result++;
                        if (maxCount < result) return result;
                        //var ptr = Memory.Instance.Read<nuint>(par2);
                        Memory.Instance.Write(par2, (nuint)dun);
                        par2 += 4;
                    }

                }
            }
            return result;
        }
        public static void SaveToFile(int saveFile)
        {
            var path = CustomSaveFile.Path;
            string filePath = $"\\Dungeons\\Test_{saveFile}.sav";
            var file = File.Create(path + filePath);
            foreach(var dungeon in Saves.Values)
            {
                var buffer = new byte[Marshal.SizeOf(typeof(DungeonSave))];
                fixed (void* d = &buffer[0])
                    Util.CopyStruct(dungeon,(DungeonSave*)d);
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
            while (size * index + size <= file.Length)
            {
                ReadOnlySpan<byte> entry = new Span<byte>(file, index * size, size);
                var buffer = entry.ToArray();
                fixed (void* ptr = buffer)
                {
                    var dungeon = (DungeonSave*) ptr;
                    var save = Saves.CreateDefault(dungeon->DungeonID);
                    Util.CopyStruct((DungeonSave*)ptr, save);
                }
                index++;
            }
        }
        public static void InitSaveGame()
        {
            Saves.Clear();

        }

    }
}
