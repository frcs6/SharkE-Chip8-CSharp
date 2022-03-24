using CommandLine;

namespace SharkE.Chip8.MonoGame
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Arguments>(args)
                .WithParsed(arguments =>
                {
                    using var game = new Emulator(arguments);
                    game.Run();
                });
        }
    }
}
