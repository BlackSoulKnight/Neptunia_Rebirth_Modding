using Neptunia_Modding.Structures.Database.Dungeon;
using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.SaveFile
{
    [StructLayout(LayoutKind.Explicit, Size = 0xcf398)]
    public unsafe struct MemDataBaseCore
    {

        [FieldOffset(0x1032C)] public int DungeonSaveArraySize;
        [FieldOffset(0xcb12c)] public Dungeon** DungeonDBArray;
        [FieldOffset(0xcb130)] public int DungeonDBArraySize;


    }
}
