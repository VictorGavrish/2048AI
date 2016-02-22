namespace Runner
{
    using System;
    using System.Collections.Generic;

    using AI2048.AI;
    using AI2048.AI.Agent;
    using AI2048.Game;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ////using (var game = new GamePage())
            ////{
            ////    var agent = new Agent();
            ////    try
            ////    {
            ////        while (true)
            ////        {
            ////            var move = agent.MakeDecision(game.GridState);
            ////            game.Turn(move);
            ////        }
            ////    }
            ////    catch (GameOverException)
            ////    {
            ////        game.TakeScreenshot().SaveAsFile("game_" + game.Score + ".png", ImageFormat.Png);
            ////        Console.WriteLine(game.Score);
            ////    }
            ////}

            var gameOverStates = new List<LogarithmicGrid>();

            for (int i = 0; i < 10; i++)
            {
                var logGrid = new LogarithmicGrid(new byte[4, 4]).AddRandomTile().AddRandomTile();

                var agent = new Agent();
                try
                {
                    while (true)
                    {
                        var move = agent.MakeDecision(logGrid);
                        logGrid = logGrid.MakeMove(move).AddRandomTile();
                    }
                }
                catch (GameOverException)
                {
                    Console.WriteLine("GAME OVER!");
                    gameOverStates.Add(logGrid);
                }
            }

            foreach (var state in gameOverStates)
            {
                Console.WriteLine(state);
            }
        }
    }
}