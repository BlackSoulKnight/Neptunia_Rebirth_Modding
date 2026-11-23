using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.SaveFile
{


    [StructLayout(LayoutKind.Explicit,Size = 0x203c)]
    public unsafe struct DungeonSave
    {

        [FieldOffset(0x0)] public ushort StatusBitfield;
        [FieldOffset(0x2)] public ushort DungeonID;
        [FieldOffset(0x4)] public ushort SubWorld;
        [FieldOffset(0xA)] public ushort GatherSpotCollectedBitfield;
        [FieldOffset(0xC)] public ushort EnemyAliveBitfield;
        [FieldOffset(0x2C)] public fixed byte MiniMapData[0x2000]; //functionalty not decomipled yet

        [FieldOffset(0x203b)] public byte lastbyte;

    }
}
