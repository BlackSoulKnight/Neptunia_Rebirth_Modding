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
            return (MemDataBaseCore*)Memory.Instance.Read<nuint>(NeptuniaModder.ModuleBase + 0x443310);
        }
        public static void CopyStruct<T>(T* source,T* destination)
        {
            var buffer = Marshal.SizeOf(typeof(T));
            Buffer.MemoryCopy(source, destination, buffer, buffer);
            
        }
        internal static void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(folder));
                CopyDirectory(folder, dest);
            }
        }


    }
}
