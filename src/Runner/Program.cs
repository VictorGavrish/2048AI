namespace Runner
{
    using System;
    using System.Drawing.Imaging;

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
                Console.WriteLine(logGrid);
                Console.WriteLine("GAME OVER!");
            }

            Console.ReadLine();
        }
    }
}