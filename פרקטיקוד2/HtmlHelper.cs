using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace פרקטיקוד2
{
    internal class HtmlHelper
    {
        public static readonly HtmlHelper tags= new HtmlHelper("HtmlTags.json", "HtmlVoidTags.json");
        public static HtmlHelper Tags => tags;
        public string[] AllTags { get; set; }
        public string[] SelfClosingTags { get; set; }
        private HtmlHelper(string allTagsJsonPath, string selfClosingTagsJsonPath)
        {
            LoadTags(allTagsJsonPath, selfClosingTagsJsonPath);
        }
        private void LoadTags(string allTagsJsonPath, string selfClosingTagsJsonPath)
        {
            AllTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText(allTagsJsonPath));
            SelfClosingTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText(selfClosingTagsJsonPath));
        }
    }
}
