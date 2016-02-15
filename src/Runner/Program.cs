namespace Runner
{
    using System;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using AI2048;
    using AI2048.AI;
    using AI2048.AI.Victor;

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new GamePage())
            {
                var agent = new VictorAgent();
                try
                {
                    while (true)
                    {
                        var move = agent.MakeDecision(game.GridState);
                        game.Turn(move);
                    }
                }
                catch (GameOverException)
                {
                    game.TakeScreenshot().SaveAsFile("game_" + game.Score + ".png", ImageFormat.Png);
                    Console.WriteLine(game.Score);
                }
            }

            Console.ReadLine();
        }
    }
}