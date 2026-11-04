using Neptunia_DataStructs.Structures.Character.Attacks;
using Neptunia_DataStructs.Structures.Character.Effects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neptunia_DataStructs.Structures.Database.Dungeon
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DB_Dungeon_ItemDrop
    {
        public uint ItemID;
        public uint Chance;
        public uint MinQuantity;
        public uint MaxQuantity;
    }




    [StructLayout(LayoutKind.Explicit, Size = 0x1d90)]
    public unsafe struct Dungeon
    {
        [FieldOffset(0x0)] public ushort DungeonID;
        [FieldOffset(0x2)] public ushort BGM;
        [FieldOffset(0x4)] public fixed ushort World[3]; //Overworld,???,Subworld
        [FieldOffset(0xA)] public ushort WorldMapIcon;
        [FieldOffset(0xC)] public ushort WorldMapNamePlate;
        //[FieldOffset(0xe)] public ushort ;
        [FieldOffset(0x10)] public int WorldMapPosX;
        [FieldOffset(0x14)] public int WorldMapPosY;
        [FieldOffset(0x18)] public nuint NamePointer;
        [FieldOffset(0x1C)] public int ScoutDays;

        [FieldOffset(0x1E)] public ushort DungeonLayout;
        [FieldOffset(0x20)] public ushort DungeonLayoutVariant;
        //[FieldOffset(0x1E)] public int ;
        //[FieldOffset(0x20)] public int ;
        [FieldOffset(0x22)] public fixed ushort HERB[9]; //unkown
        [FieldOffset(0x34)] public fixed int Flags[9]; //something about event flags
        [FieldOffset(0x5C)] public HiddenTreasure HiddenTreasure;
        [FieldOffset(0x160)] public DungeonEnemies Enemy1;
        [FieldOffset(0x7B4)] public DungeonEnemies Enemy2;
        [FieldOffset(0xE08)] public DungeonEnemies Enemy3;
        [FieldOffset(0x145C)] public GatherSpots Gather1;
        [FieldOffset(0x1768)] public GatherSpots Gather2;
        [FieldOffset(0x1a74)] public GatherSpots Gather3;
        //[FieldOffset(0x1d80)] public int ;
        //[FieldOffset(0x1d84)] public int ;
        //[FieldOffset(0x1d88)] public int ;
        //[FieldOffset(0x1d8C)] public int ;
        //[FieldOffset(0x1d90)] public int ;

    }
}
