using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Neptunia_Modding.Structures.Database.Dungeon
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DB_Dungeon_GatherSpot
    {
        public uint SpotID;
        public DB_Dungeon_ItemDrop ItemDrop1;
        public DB_Dungeon_ItemDrop ItemDrop2;
        public DB_Dungeon_ItemDrop ItemDrop3;
    }
    [InlineArray(5)]
    public struct HiddenTreasure
    {
        private DB_Dungeon_GatherSpot _firstElement;
    }

    [InlineArray(15)]
    public struct GatherSpots
    {
        private DB_Dungeon_GatherSpot _firstElement;
    }
}
