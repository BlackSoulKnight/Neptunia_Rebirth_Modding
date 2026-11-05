using Neptunia_Modding.Structures.SaveFile;
using Reloaded.Memory.Pointers;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Neptunia_Modding.Modding.SaveFile
{
    public static unsafe class CustomSaveFile
    {
        public static string Path => Directory.GetCurrentDirectory()+ "\\Modding";
        public static DungeonSave*[] DungeonSaveList => DungeonSaveManager.DungeonSaveList;

        public delegate void SaveGame(int saveFile);
        public delegate void InitSaveGame();
        public static event SaveGame OnSaveGame;
        public static event SaveGame OnLoadGame;
        public static event InitSaveGame OnInitGame;

        static CustomSaveFile()
        {
            DungeonSaveManager.OnSaveGame();
        }

        public static void DoLoadGame(int Savefile) {
            OnLoadGame?.Invoke(Savefile);
        }

        public static void DoSaveGame(int saveFile)
        {
            OnSaveGame?.Invoke(saveFile);
        }

        public static void DoInitGame() {
            OnInitGame?.Invoke();
        }
    }
}
