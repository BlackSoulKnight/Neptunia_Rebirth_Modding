using Neptunia_Modding.Structures.SaveFile;
using Reloaded.Memory;
using System.Runtime.InteropServices;


namespace Neptunia_Modding.Modding
{
    public static unsafe class Util
    {
        public static MemDataBaseCore* MemDataBaseCore => GetBaseCore();

        static MemDataBaseCore* GetBaseCore()
        {
            return (MemDataBaseCore*)Memory.Instance.Read<nuint>(Mod.ModuleBase + 0x443310);
        }
        public static void CopyStruct<T>(T* source,T* destination)
        {
            var buffer = Marshal.SizeOf(typeof(T));
            Buffer.MemoryCopy(source, destination, buffer, buffer);
            
        }

    }
}
