using System.Text.RegularExpressions;
using SkinColorizer.Common;
using SkinColorizer.Common.Enums;

namespace SkinColorizer
{
    public class InputService
    {

        private readonly Func<string> inputProvider;
        private readonly Action<string> outputProvider;

        public InputService(Action<string> outputProvider, Func<string> inputProvider)
        {
            this.outputProvider = outputProvider;
            this.inputProvider = inputProvider;
        }

        public ColorizerOptions UserInputHandler()
        {
            var options = new ColorizerOptions();

            string path = GetPath();
            if (string.IsNullOrWhiteSpace(path)) return null;
            string outputPath = GetOutputPath(path);
            if (string.IsNullOrWhiteSpace(outputPath)) return null;

            int hue = GetHueDegrees();

            options.Path = path;
            options.OutputDirectory = outputPath;
            options.HueDegrees = hue;

            return options;
        }

        private string GetPath()
        {
            outputProvider("Please enter your osu skin directory");
            var directory = inputProvider();

            bool doesSkinExist = File.Exists(directory + @"\skin.ini");
            if (VerifyDirectory(directory) && File.Exists(directory + @"\skin.ini"))
            {
                return directory;
            }

            outputProvider("Invalid Path");
            return string.Empty;
        }

        private string GetOutputPath(string skinDirPath)
        {
            outputProvider("Please enter your output directory name");
            var name = inputProvider();

            if (string.IsNullOrWhiteSpace(name))
            {
                outputProvider("Invalid Name");
                return string.Empty;
            }

            string skinDir = Path.GetDirectoryName(skinDirPath);
            string dir = skinDir + @"\" + name;
            Directory.CreateDirectory(dir);
            return dir;
        }

        private bool VerifyDirectory(string directory) => Regex.IsMatch(directory, @"((?:[^/]*/)*)(.*)") &&
                                                                Directory.Exists(directory);

        private int GetHueDegrees()
        {
            outputProvider("Please enter the color you want: [1] Red, [2] Green, [3] Blue, [4] Purple, [5] Custom Color");
            var isSuccessful = int.TryParse(inputProvider(), out int value);

            if (isSuccessful)
            {
                int result = value switch
                {
                    1 => (int)HueDegrees.Red,
                    2 => (int)HueDegrees.Green,
                    3 => (int)HueDegrees.Blue,
                    4 => (int)HueDegrees.Purple,
                    5 => CustomColorHandler(),
                    _ => InvalidChoiceHandler()
                };

                return result;
            }

            return InvalidChoiceHandler();

        }

        private int CustomColorHandler()
        {
            outputProvider("Please enter the custom color you want in hue degrees");
            var isSuccessful = int.TryParse(inputProvider(), out int value);

            if (isSuccessful)
            {
                if (value < 0 || value > 360)
                {
                    return InvalidChoiceHandler();
                }
                return value;
            }

            return InvalidChoiceHandler();
        }

        private int InvalidChoiceHandler()
        {
            outputProvider($"Your Choice is invalid");
            return 0;
        }

    }
}