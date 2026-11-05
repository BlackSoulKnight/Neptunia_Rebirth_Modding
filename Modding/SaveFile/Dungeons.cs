using Neptunia_Modding.Structures.Database.Dungeon;
using Neptunia_Modding.Structures.SaveFile;
using Neptunia_Modding.Test;
using Reloaded.Memory;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Neptunia_Modding.Modding.Hooks.DungeonHooks;

namespace Neptunia_Modding.Modding.SaveFile
{

    public static unsafe class Dungeons
    {

        public const int DungeonListArraySize = 100;
        public static Dungeon** DungeonList = (Dungeon**)Marshal.AllocHGlobal(sizeof(Dungeon*) * DungeonListArraySize);
        static MemDataBaseCore* MemDataBaseCore => Globals.MemDataBaseCore;

        static Dungeons()
        {

        }
        

        public static void DungeonInit()
        {
            if (MemDataBaseCore->DungeonDBArraySize != 0)
            {
                int i = 0;
                for (; i < MemDataBaseCore->DungeonDBArraySize; i++)
                {
                    DungeonList[i] = MemDataBaseCore->DungeonDBArray[i];
                }
                MemDataBaseCore->DungeonDBArray = DungeonList;
                DungeonList[i] = NewDungeonTest.GetTestDungeon();
            }
        }
        public static void CreateDungeon(Dungeon* dungeonData) {
            //Dungeon* newDungeon = (Dungeon*)Marshal.AllocHGlobal(sizeof(Dungeon));
            //int size = sizeof(Dungeon);
            var newDungeon = dungeonData;
            //Buffer.MemoryCopy(&dungeonData, newDungeon,size,size);
            for (int i = 0; i < MemDataBaseCore->DungeonDBArraySize; i++)
                if (DungeonList[i] == newDungeon) return;
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
