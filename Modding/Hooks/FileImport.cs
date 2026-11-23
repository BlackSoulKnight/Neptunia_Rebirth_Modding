using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Neptunia_Modding.Modding.Hooks
{
    internal class FileImport
    {
        public static List<IAsmHook> _asmHooks = new();
        public unsafe static Dictionary<string, Dictionary<int, nint>> ResourcesPointers = new();
        public unsafe static Dictionary<string,Dictionary<int, nint>> Test= new();
        public unsafe static Dictionary<string,IntPtr> FileHandleList= new();


        public static IReverseWrapper<ReadFromPac> _onReadFromPac;
        public static IReverseWrapper<CreateFileHandler> _onCreateFileHandler;
        public static IReverseWrapper<MemoryLib> _onMemoryLib;

        [DllImport("kernel32.dll", EntryPoint = "ReadFile")]
        static unsafe extern bool ReadFile(
              [In] IntPtr handler,
              [Out] IntPtr lpBuffer,
              [In] uint nNumberOfBytesToRead,
              [Out, Optional] IntPtr lpNumberOfBytesRead,
              [In, Out, Optional] IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW")]
        static unsafe extern IntPtr CreateFileW(
              [In] IntPtr lpFileName,
              [In] int dwDesiredAccess,
              [In] int dwShareMode,
              [In, Optional] IntPtr lpSecurityAttributes,
              [In] int dwCreationDisposition,
              [In] int dwFlagsAndAttributs,
              [In,Optional] IntPtr hTemplateFile
            );





        [Function(new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx, FunctionAttribute.Register.edi, FunctionAttribute.Register.ebx, FunctionAttribute.Register.ecx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public unsafe delegate bool ReadFromPac(nint eax, nint edx, uint edi, nint ebx, nint ecx);
        public static unsafe bool OnReadFromPac(nint hFile, nint buffer, uint size, nint sizeRead, nint info)
        {
            if (size == 28)
                return ReadFile(hFile, buffer, size, sizeRead, 0);      //header for large files?
            string filePath = Marshal.PtrToStringUTF8((info + 0x14)) ?? "";
            int filePart = ((int*)info)[76];
            Console.WriteLine($"{filePath} {((int*)info)[76]}");

            if (ResourcesPointers.ContainsKey(filePath))
            {
                if (ResourcesPointers[filePath].ContainsKey(filePart))
                {
                    ReadFile(hFile, buffer, size, sizeRead, 0);
                    for (int i = 0; i < size; i++)
                        if (((byte*)ResourcesPointers[filePath][filePart])[i] != ((byte*)buffer)[i])
                            Console.WriteLine($"PANIC {filePath}:{filePart}");
                    Buffer.MemoryCopy((void*)ResourcesPointers[filePath][filePart], (void*)buffer, size, size);
                    return true;
                }
            }
            else
                ResourcesPointers[filePath] = new();
            var result = ReadFile(hFile, buffer, size, sizeRead, 0);
            if (!result)
                return result;
            var memory = (nint)Marshal.AllocHGlobal((nint)size);
            Buffer.MemoryCopy((void*)buffer, (void*)memory, size, size);
            ResourcesPointers[filePath].Add(filePart, memory);
            return true;
        }


        [Function(CallingConventions.Stdcall)]
        public unsafe delegate IntPtr CreateFileHandler(IntPtr lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition,int dwFlagsAndAttributs,IntPtr hTemplateFile);
        public static unsafe IntPtr OnCreateFileHandler(IntPtr lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributs, IntPtr hTemplateFile)
        {
            
            string file = Marshal.PtrToStringUni(lpFileName);
            if(FileHandleList.ContainsKey(file))
                return FileHandleList[file];

            Console.WriteLine($"Open new File Handle for {file}");
            IntPtr newHandle = CreateFileW(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributs, hTemplateFile);
            FileHandleList.Add(file, newHandle);
            return newHandle;
        }

        [Function(new[] { FunctionAttribute.Register.eax }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public unsafe delegate int MemoryLib(nint eax);
        public static unsafe int OnMemoryLib(nint file)
        {
            Console.WriteLine(Marshal.PtrToStringUTF8(file+16));
            return 0;
        }


        public unsafe static void SetupHooks(IReloadedHooks hooks)
        {
            if (hooks == null) return;
            // Game functions
            nuint offset = 0;
            nuint jmp = 0;



            if (FunctionScanner.FindFunction("Create FileHandler", "FF 15 ?? ?? ?? ?? 8B F0 83 FE FF 75", out offset))
            {
                string[] code =
                {
                    "use32",
                    $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnCreateFileHandler, out _onCreateFileHandler)}",
                    "mov esi,eax",
                };
                _asmHooks.Add(hooks.CreateAsmHook(code, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());

            }

            if (FunctionScanner.FindFunction("No Close FileHandler", "FF 70 ?? FF 15 ?? ?? ?? ?? 8B 47", out offset))
            {
                string[] code =
                {
                    "use32",
                };
                _asmHooks.Add(hooks.CreateAsmHook(code, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            }

            if (FunctionScanner.FindFunction("Find Resource Location", "FF 15 ?? ?? ?? ?? 85 C0 75 ?? C7 43 ?? 03 00 00 00", out offset))
            {
                string[] code =
                {
                    "use32",
                    "add esp,0x14",
                    "push ebx",
                    "push ecx",
                    "push edx",
                    "push ebp",
                    "push esi",
                    "push edi",
                    "mov ecx,edi",
                    "mov ebx,eax",
                    "mov eax,[ebp-0x1C]",
                    "mov edx,[ebp-0x20]",
                    "mov edi,[ebp-0x18]",
                    $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnReadFromPac, out _onReadFromPac)}",
                    "pop edi",
                    "pop esi",
                    "pop ebp",
                    "pop edx",
                    "pop ecx",
                    "pop ebx",
                    "test eax,eax",
                };
                _asmHooks.Add(hooks.CreateAsmHook(code, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());

            }

            if(FunctionScanner.FindFunction("Resource Lib", "89 83 ?? ?? ?? ?? 83 BB ?? ?? ?? ?? 00 74", out offset))
            {
                string[] code =
                {
                "use32",
                "pushad",
                "pushfd",
                "mov eax,ebx",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnMemoryLib, out _onMemoryLib)}",
                "popfd",
                "popad",
                };
                _asmHooks.Add(hooks.CreateAsmHook(code, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
            }


        }

    }
}
