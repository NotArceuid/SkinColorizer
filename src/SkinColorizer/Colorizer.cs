using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SkinColorizer
{
    public class Colorizer
    {
        private readonly InputService inputService;

        public Colorizer(InputService inputService)
        {
            this.inputService = inputService;

            var skin = inputService.HandleUserInput();
            if (skin is not null)
            {
                var skinElements = GetSkinElements(skin.Path);
                Colorize(skin.HueDegrees, FilterSkinElements(skinElements), skin.OutputDirectory);
            }

            return;
        }

        //I am sinciery sorry for the shitty code.
        private void Colorize(float degrees, List<string> skinElements, string outputDirectory)
        {
            var converter = new ColorSpaceConverter();
            foreach (string skinElementPath in skinElements)
            {
                using var image = Image.Load<Rgba32>(skinElementPath);

                image.ProcessPixelRows(accessor =>
                {
                    Span<Hsl> hsl = new Hsl[accessor.Width];
                    Span<Rgb> rgb = new Rgb[accessor.Width];

                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<Rgba32> row = accessor.GetRowSpan(y);
                        for (int x = 0; x < row.Length; x++)
                        {
                            hsl[x] = converter.ToHsl(row[x]);
                            var newHue = new Hsl(hsl[x].H - hsl[x].H + degrees, hsl[x].S, hsl[x].L);

                            row[x] = converter.ToRgb(newHue);
                        }
                    }

                    image.SaveAsPng(outputDirectory + "/" + Path.GetFileName(skinElementPath));
                });
            }
        }

        private List<string> GetSkinElements(string path)
        {
            var skinElements = Directory.EnumerateFiles(path);
            return skinElements.ToList();
        }

        private List<string> FilterSkinElements(List<string> skinElements)
        {
            var json = JsonConvert.DeserializeObject<SkinElements>(File.ReadAllText("./SkinElements.json"));
            skinElements.RemoveAll(x =>
            {
                return !x.EndsWith(".png");
            });

            var filteredSkinElements = new List<string>();
            foreach (var jsonElements in json.Elements)
            {
                var filteredElements = skinElements.Where(x => x.Contains(jsonElements));
                filteredSkinElements.AddRange(filteredElements);
            }

            return filteredSkinElements;
        }


        public class SkinElements
        {
            [JsonProperty("SkinElements")] public IEnumerable<string> Elements { get; set; }
        }
    }
}