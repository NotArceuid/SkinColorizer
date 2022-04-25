namespace SkinColorizer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureServices();
        }

        static void ConfigureServices()
        {
            Console.InputEncoding = System.Text.Encoding.Unicode; 
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        
            var inputService = new InputService(Console.WriteLine, Console.ReadLine);
            var colorizer = new Colorizer(inputService);
        }
    }
}
