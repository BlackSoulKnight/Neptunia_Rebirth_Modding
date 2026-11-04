
using Neptunia_DataStructs.Structures.Database.Dungeon;
using Neptunia_DataStructs.Structures.SaveFile;
using System.Runtime.InteropServices;
using Reloaded.Memory;
using System.Runtime.CompilerServices;

namespace Neptunia_DataStructs.Modding.SaveFile
{

    public static unsafe class Dungeons
    {
        public static DungeonSave*[] DungeonSaveList = new DungeonSave*[100];
        public static int MaxDungeon;
        public static MemDataBaseCore* MemDataBaseCore => GetBaseCore();

        static MemDataBaseCore* GetBaseCore()
        {
            return (MemDataBaseCore*)Memory.Instance.Read<nuint>(Mod.ModuleBase + 0x443310);
        }
        public static DungeonSave* GetDungeonSave(ushort dungeonID)
        {
            for (int i = 0; i < DungeonSaveList.Length; i++)
            {
                if (DungeonSaveList[i] == null)
                    return null;
                if (DungeonSaveList[i]->DungeonID == dungeonID)
                        return DungeonSaveList[i];
            }
            return null;
        }
        public static bool IsDungeonSaveInList(ushort dungeonID) => GetDungeonSave(dungeonID) != null;

        public static Dungeon* GetDungeon(ushort dungeonID)
        {
            for (int i = 0; i < MemDataBaseCore->DungeonDBArraySize; i++)
            {
                Dungeon* entry = MemDataBaseCore->DungeonDBArray[i];
                if (MemDataBaseCore->DungeonDBArray[i]->DungeonID == dungeonID)
                    return MemDataBaseCore->DungeonDBArray[i];
            }
            return null;
        }
        public static bool AddDungeonSave(Dungeon* dungeon,bool flag)
        {
            if (dungeon == null || IsDungeonSaveInList(dungeon->DungeonID))
                return false;

            return true;
        }
        public static DungeonSave* CreateDungeonSave(ushort dungeonID)
        {
            if (dungeonID == 99)
                dungeonID = 0xc9;
            DungeonSave* result = GetDungeonSave(dungeonID);
            if (result != null) return result;
            Dungeon* dungeonDBEntry = GetDungeon(dungeonID);
            if (dungeonDBEntry == null) return null;

            result = (DungeonSave*)Marshal.AllocHGlobal(sizeof(DungeonSave));
            DungeonSaveList[MaxDungeon] = result;
            result->DungeonID = dungeonID;
            result->SubWorld = dungeonDBEntry->World[2];
            MaxDungeon++;
            return result;
        }
        public static DungeonSave* GetDungeonSaveOrCreateNew(ushort dungeonID)
        {
            DungeonSave * result = GetDungeonSave(dungeonID);

            if (result == null)
                result = CreateDungeonSave(dungeonID);

            return result;
        }



    }
}
