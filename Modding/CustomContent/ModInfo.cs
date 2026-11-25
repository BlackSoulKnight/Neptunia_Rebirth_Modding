using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptunia_Modding.Modding.CustomContent
{
    internal class ModInfo
    {
        const string Manifest = "manifest.json";

        public readonly int ModID = 0;
        public readonly List<AdditionalContent> AdditionalContents = new();
        public readonly string ModName = "";
        public readonly string BaseDirectory = "";

        internal static List<ModInfo> LoadMods()
        {
            var mods = new List<ModInfo>();


            var modDirectories = Directory.GetFiles(NeptuniaModder.ModFolder + "/CustomContent", Manifest, SearchOption.AllDirectories);
            foreach (var modDirectory in modDirectories)
            {
                var mod = new ModInfo(Path.GetDirectoryName(modDirectory)!);
            }

            return mods;
        }

        public ModInfo(string path) {
            // 
            ModID = 1;
            ModName = "EPIC MOD";
            BaseDirectory = path;
           
            using (StreamReader file = File.OpenText(Path.Combine(path ,Manifest)))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var o2 = (JObject)JToken.ReadFrom(reader);
                if (o2.ContainsKey("Name"))
                    ModName = o2["Name"]!.ToString();
                if (o2.ContainsKey("ModID"))
                    ModID = (int)o2["ModID"]!;
            }
            AdditionalContents = AdditionalContent.CreateAdditionalContent(this, BaseDirectory);
        }

    }
}
