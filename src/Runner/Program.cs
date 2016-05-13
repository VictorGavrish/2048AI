namespace Runner
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;

    using AI2048.AI;
    using AI2048.AI.Agent;
    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.Game;

    using NodaTime;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var logGrid = RunGameInConsole();

            File.AppendAllText("results.txt", logGrid.ToString());

            Console.WriteLine(logGrid);
        }

        private static LogarithmicGrid RunGameInConsole()
        {
            //var logGrid = new LogarithmicGrid(new byte[4, 4]).AddRandomTile().AddRandomTile();

            var logGrid = new LogarithmicGrid(new int[,]
            {
                { 16, 4, 2, 8 },
                { 16, 128, 16, 4 },
                { 2, 512, 32, 0 },
                { 0, 8, 0, 0 }
            });

            var agent = new Agent(logGrid, new ProbabilityLimitedExpectiMaxerFactory(), new OvolveHeuristic());
            try
            {
                int counter = 0;

                while (true)
                {
                    counter++;

                    var startTime = SystemClock.Instance.Now;

                    Console.WriteLine("Start next move calculation...");

                    var result = agent.MakeDecision();

                    var elapsed = SystemClock.Instance.Now - startTime;

                    Console.Clear();

                    Console.WriteLine("End move calcualtion, time taken: {0}", elapsed.ToString("ss.fff", CultureInfo.InvariantCulture));
                    Console.WriteLine();

                    Console.WriteLine(result);

                    PrintTimings(agent);

                    logGrid = logGrid.MakeMove(result.BestMove).AddRandomTile();
                    agent.UpdateGrid(logGrid);
                }
            }
            catch (GameOverException)
            {
                Console.WriteLine("GAME OVER!");
            }

            return logGrid;
        }

        private static void PrintTimings(Agent agent)
        {
            foreach (var kvp in agent.Timings)
            {
                var humanValue = kvp.Key == 0 ? 0 : 2 << (kvp.Key - 1);
                Console.WriteLine($"{humanValue,5}: {kvp.Value.ToString("mm:ss.fff", CultureInfo.InvariantCulture)}");
            }

            Console.WriteLine($"Time passed this game: {(SystemClock.Instance.Now - agent.Start).ToString("mm:ss.fff", CultureInfo.InvariantCulture)}");
        }

        private static void RunGameInBrowser()
        {
            using (var game = new GamePage())
            {
                var agent = new Agent(game.GridState, new ProbabilityLimitedExpectiMaxerFactory(), new VictorHeuristic());
                try
                {
                    while (true)
                    {
                        var result = agent.MakeDecision();
                        game.Turn(result.BestMove);
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