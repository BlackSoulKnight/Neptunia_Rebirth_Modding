using Neptunia_Modding.Modding.SaveFile;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;

namespace Neptunia_Modding.Modding.Hooks
{
    public unsafe class SaveGameHooks
    {
        public static List<IAsmHook> _asmHooks = new();

        public static IReverseWrapper<LoadSaveGame> _onLoadSaveGame;
        public static IReverseWrapper<InitSaveGame> _onInitSaveGame;
        public static IReverseWrapper<SaveSaveGame> _onSaveSaveGame;
        public static int SaveFileNumber => Memory.Instance.Read<int>(Mod.ModuleBase+ 0x8EB3FC);

        [Function(new FunctionAttribute.Register[] { }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int LoadSaveGame();
        public static unsafe int OnLoadSaveGame()
        {
            Console.WriteLine($"Loading SaveGame {SaveFileNumber}");
            CustomSaveFile.DoLoadGame(SaveFileNumber);
            return 0;
        }

        [Function(new FunctionAttribute.Register[] { }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int SaveSaveGame();
        public static unsafe int OnSaveSaveGame()
        {
            Console.WriteLine($"Save SaveGame {SaveFileNumber}");
            CustomSaveFile.DoSaveGame(SaveFileNumber);
            return 0;
        }




        [Function(new FunctionAttribute.Register[] { }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int InitSaveGame();
        public static unsafe int OnInitSaveGame()
        {
            Console.WriteLine($"Initalitze SaveGame");
            CustomSaveFile.DoInitGame();
            return 0;
        }

        public static void SetupHooks(IReloadedHooks hooks)
        {
            if (hooks == null) return;
            nuint offset = 0;

            if (FunctionScanner.FindFunction("Load Game", "55 8B EC 56 8B 35 ?? ?? ?? ?? 68 D0 B0 0C 00", out offset))
            {
                string[] Event = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnLoadSaveGame, out _onLoadSaveGame)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(Event, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
            }

            if (FunctionScanner.FindFunction("Save Game", "55 8B EC 81 7D ?? D0 B0 0C 00", out offset))
            {
                string[] Event = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnSaveSaveGame, out _onSaveSaveGame)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(Event, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
            }

            if (FunctionScanner.FindFunction("New Game", "53 8B DC 83 EC 08 83 E4 F0 83 C4 04 55 8B 6B ?? 89 6C 24 ?? 8B EC 83 EC 28 8B 0D", out offset))
            {
                string[] Event = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnInitSaveGame, out _onInitSaveGame)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(Event, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
            }

        }

    }
}
