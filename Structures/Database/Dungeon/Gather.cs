using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;


namespace Neptunia_Modding.Structures.Database.Dungeon
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DB_Dungeon_GatherSpot
    {
        public uint SpotID { get; set; }
        public DB_Dungeon_ItemDrop ItemDrop1 { get; set; }
        public DB_Dungeon_ItemDrop ItemDrop2 { get; set; }
        public DB_Dungeon_ItemDrop ItemDrop3 { get; set; }
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
