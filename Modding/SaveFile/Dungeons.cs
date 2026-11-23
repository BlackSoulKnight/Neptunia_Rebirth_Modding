using Neptunia_Modding.Modding.CustomContent;
using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Neptunia_Modding.Test;
using Reloaded.Hooks.Definitions.Structs;
using System.Runtime.InteropServices;

namespace Neptunia_Modding.Modding.SaveFile
{

    public static unsafe class Dungeons
    {

        public static int DungeonListArraySize { get; private set; } = 100; 
        public static Dungeon** DungeonList = (Dungeon**)Marshal.AllocHGlobal(sizeof(Dungeon*) * DungeonListArraySize);
        static MemDataBaseCore* MemDataBaseCore => Util.MemDataBaseCore;
        static int CurrentSize => MemDataBaseCore->DungeonDBArraySize;
        internal readonly static ObjectDictonary<int, byte> NameList = new();
        public static event AdditionalContent.LoadFiles OnLoadDungeons;
        static bool Init = false;
        static Dungeons()
        {

        }
        
        public static void DungeonInit()
        {
            if (!Init && CurrentSize != 0)
            {
                Init = true;
                int i = 0;
                for (; i < MemDataBaseCore->DungeonDBArraySize; i++)
                {
                    DungeonList[i] = MemDataBaseCore->DungeonDBArray[i];
                }
                MemDataBaseCore->DungeonDBArray = DungeonList;
                OnLoadDungeons?.Invoke();
            }
        }

        static void IncreaseListCapacity()
        {
            DungeonListArraySize *= 2;
            Dungeon** newList = (Dungeon**)Marshal.AllocHGlobal(sizeof(Dungeon*) * DungeonListArraySize);
            for (int i = 0; i < CurrentSize; i++)
            {
                newList[i] = DungeonList[i];
            }
            Marshal.FreeHGlobal((nint)DungeonList);
            DungeonList = newList;
        }

        public static void CreateDungeon(Dungeon* dungeonData) {
            if (dungeonData == null)
                return;
            int size = sizeof(Dungeon);
            var newDungeon = dungeonData;
            for (int i = 0; i < MemDataBaseCore->DungeonDBArraySize; i++)
                if (DungeonList[i]->DungeonID == newDungeon->DungeonID)
                    if (DungeonList[i] != newDungeon)
                    {
                        Marshal.FreeHGlobal((nint)(DungeonList[i]));
                        DungeonList[i] = newDungeon;
                    }
            if (CurrentSize == DungeonListArraySize)
                IncreaseListCapacity();

            DungeonList[MemDataBaseCore->DungeonDBArraySize] = newDungeon;
            Console.WriteLine(((nuint)newDungeon).ToString("X"));
            MemDataBaseCore->DungeonDBArraySize++;
        }

        public static Dungeon* GetDungeon(ushort dungeonID)
        {
            if (DungeonList[0] == null)
                DungeonInit();
            for (int i = 0; i < MemDataBaseCore->DungeonDBArraySize; i++)
            {
                Dungeon* entry = MemDataBaseCore->DungeonDBArray[i];
                if (MemDataBaseCore->DungeonDBArray[i]->DungeonID == dungeonID)
                    return MemDataBaseCore->DungeonDBArray[i];
            }
            return null;
        }




    }
}
