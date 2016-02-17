namespace Runner
{
    using System;
    using System.Drawing.Imaging;

    using AI2048;
    using AI2048.AI;
    using AI2048.AI.Agent;

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new GamePage())
            {
                var agent = new Agent();
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