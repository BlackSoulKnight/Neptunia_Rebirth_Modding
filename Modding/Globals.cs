using Neptunia_Modding.Structures.SaveFile;
using Reloaded.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Modding
{
    public static unsafe class Globals
    {
        public static MemDataBaseCore* MemDataBaseCore => GetBaseCore();

        static MemDataBaseCore* GetBaseCore()
        {
            return (MemDataBaseCore*)Memory.Instance.Read<nuint>(Mod.ModuleBase + 0x443310);
        }

    }
}
