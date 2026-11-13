using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.Database.Character.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StatusEffekt
    {
        public unsafe int Effektiveness;
        public unsafe int ActiveTime;
    }
}
