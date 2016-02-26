namespace Runner
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    using AI2048.AI;
    using AI2048.AI.Agent;
    using AI2048.Game;

    internal class Program
    {
        private static void Main(string[] args)
        {
            //RunGameInBrowser();

            var gameOverStates = new List<LogarithmicGrid>();

            var times = 1;

            for (var i = 0; i < times; i++)
            {
                var logGrid = RunGameInConsole();

                gameOverStates.Add(logGrid);
            }

            foreach (var state in gameOverStates)
            {
                Console.WriteLine(state);
            }
        }

        private static LogarithmicGrid RunGameInConsole()
        {
            var logGrid = new LogarithmicGrid(new byte[4, 4]).AddRandomTile().AddRandomTile();

            var agent = new Agent(logGrid);
            try
            {
                while (true)
                {
                    var move = agent.MakeDecision();
                    logGrid = logGrid.MakeMove(move).AddRandomTile();
                    agent.UpdateGrid(logGrid);
                }
            }
            catch (GameOverException)
            {
                Console.WriteLine("GAME OVER!");
            }

            return logGrid;
        }

        private static void RunGameInBrowser()
        {
            using (var game = new GamePage())
            {
                var agent = new Agent(game.GridState);
                try
                {
                    while (true)
                    {
                        var move = agent.MakeDecision();
                        game.Turn(move);
                        agent.UpdateGrid(game.GridState);
                    }
                }
                catch (GameOverException)
                {
                    game.TakeScreenshot().SaveAsFile("game_" + game.Score + ".png", ImageFormat.Png);
                    Console.WriteLine(game.Score);
                }
            }
        }
    }
}