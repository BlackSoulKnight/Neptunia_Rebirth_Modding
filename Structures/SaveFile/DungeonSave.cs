using Neptunia_DataStructs.Structures.Character.Attacks;
using Neptunia_DataStructs.Structures.Character.Effects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neptunia_DataStructs.Structures.SaveFile
{


    [StructLayout(LayoutKind.Explicit,Size = 0x203c)]
    public unsafe struct DungeonSave
    {

        [FieldOffset(0x0)] public ushort StatusBitfield;
        [FieldOffset(0x2)] public ushort DungeonID;
        [FieldOffset(0x4)] public ushort SubWorld;
        [FieldOffset(0xA)] public ushort GatherSpotCollectedBitfield;
        [FieldOffset(0xC)] public ushort EnemyAliveBitfield;

    }
}
