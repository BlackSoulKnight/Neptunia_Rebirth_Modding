using Neptunia_Modding.Modding.SaveFile;
using Neptunia_Modding.Structures.Database.Dungeon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Neptunia_Modding.Modding.CustomContent
{
    public enum AdditionalContentTypes
    {
        Dungeon = 1
    }

    internal partial class AdditionalContent
    {
        const string ContentInfo = "info.json";

        protected List<AdditionalContent> Content = new();

        protected int ModID = 0;
        protected string BaseDirectory = "";

        public delegate void LoadFiles();

        internal static List<AdditionalContent> CreateAdditionalContent(ModInfo mod,string path)
        {
            List<AdditionalContent> result = new();
            string[] foundContent = Directory.GetFiles(mod.BaseDirectory, ContentInfo, SearchOption.AllDirectories);
            
            List<string> contentDirectories = new();
            foreach (string file in foundContent)
            {
                Console.WriteLine($"Found new content at: {file}");
                contentDirectories.Add(Path.GetDirectoryName(file)!);
            }

            foreach (string contentDirectory in contentDirectories)
            {
                
                string type = "";
                using (StreamReader file = File.OpenText(Path.Combine(contentDirectory, ContentInfo)))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    var o2 = (JObject)JToken.ReadFrom(reader);
                    if (o2.ContainsKey("Type"))
                        type = o2["Type"]!.ToString();
                }
                if (!Enum.IsDefined(typeof(AdditionalContentTypes), type))
                    continue;

                var defType = Enum.Parse<AdditionalContentTypes>(type);
                AdditionalContent content = null;
                switch (defType)
                {
                    case AdditionalContentTypes.Dungeon:
                        content = new CustomDungeon(mod, contentDirectory);
                        Dungeons.OnLoadDungeons += content.LoadMod;
                        break;
                }
                if(content != null)
                    result.Add(content);
            }

            return result;
        }

        protected AdditionalContent(ModInfo modInfo,string contentBaseDirectory) { 
            ModID = modInfo.ModID;
            BaseDirectory = contentBaseDirectory;
        }

        protected virtual void LoadMod() { }
    }
}
