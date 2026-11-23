using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.Database.Dungeon
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DB_Dungeon_ItemDrop
    {
        public uint ItemID { get; set; }
        public uint Chance { get; set; }
        public uint MinQuantity { get; set; }
        public uint MaxQuantity { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DungeonMapInfo
    {
        public ushort OverWorld { get; set; }
        public ushort UnWorld { get; set; }
        public ushort SubWorld { get; set; }
        public ushort WorldMapIcon { get; set; }
        public int WorldMapPosX { get; set; }
        public int WorldMapPosY { get; set; }
    }


    [InlineArray(10)]
    public struct TreasureIDs
    {
        private int _firstElement;
    }

    [InlineArray(9)]
    public struct HERB
    {
        private short _firstElement;
    }

    [InlineArray(5)]
    public struct Connections
    {
        private int _firstElement;
    }


    [StructLayout(LayoutKind.Sequential, Size = 0x1d90)]
    public unsafe struct Dungeon
    {
        public ushort DungeonID { get; set; }
        public ushort BGM { get; set; }
        public DungeonMapInfo MapInfo { get; set; }
        public ushort DungeonMap { get; set; }
        public ushort DungeonMapVariant { get; set; }
        public nint NamePointer { get; set; }
        public ushort ScoutDays { get; set; }
        public ushort ScoutTimeout { get; set; }
        public ushort ScoutDuration { get; set; }
        public HERB HERB { get; set; } //unkown
        public TreasureIDs TreasureIds { get; set; }  // Treasure are set in the Treasure.gbin
        public HiddenTreasure HiddenTreasure { get; set; }
        public DungeonEnemies NormalEnemies { get; set; }
        public DungeonEnemies ChangeEnemy { get; set; }
        public DungeonEnemies ChangeDungeonEnemy { get; set; }
        public GatherSpots NormalGather { get; set; }
        public GatherSpots ChangeGather { get; set; }
        public GatherSpots ChangeDungeonGather { get; set; }
        public Connections Connections { get; set; }
    }
}
