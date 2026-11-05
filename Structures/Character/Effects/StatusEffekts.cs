using System.Runtime.InteropServices;

namespace Neptunia_Modding.Structures.Character.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StatusEffekt
    {
        public unsafe int Effektiveness;
        public unsafe int ActiveTime;
    }
}
