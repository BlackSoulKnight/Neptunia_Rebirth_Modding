using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Test
{
    internal unsafe class NewDungeonTest
    {
        public static Dungeon* TestDungeon = null;

        public static Dungeon* GetTestDungeon()
        {
            if (TestDungeon != null) return TestDungeon;
            TestDungeon = (Dungeon*)Marshal.AllocHGlobal(sizeof(Dungeon));
            var target = Dungeons.GetDungeon(1);
            Buffer.MemoryCopy(target, TestDungeon, sizeof(Dungeon), sizeof(Dungeon));
            TestDungeon->DungeonLayout = 220;
            TestDungeon->DungeonLayoutVariant = 1;
            TestDungeon->DungeonID = 100;
            TestDungeon->WorldMapPosY = 0x26E;
            TestDungeon->NamePointer = (nuint)Marshal.StringToHGlobalAnsi("Extra Dungeon");
            Dungeons.CreateDungeon(TestDungeon);
            return TestDungeon;

        }

    }
}
