using Newtonsoft.Json;
using System.IO;

namespace SkinColorizer.Models
{
    public class SkinData
    {
        public float HueDegrees { get; set; }
        public string Path { get; set; }
        public string OutputDirectory { get; set; }
        public string ColoredSkinName { get; set; }

    }
}