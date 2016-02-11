﻿namespace AI2048.Tests
{
    using System;
    using System.Linq;

    using AI2048.Game;

    using NUnit.Framework;

    [TestFixture]
    public class EmulaturSelfCheckTests
    {
        [Test]
        public void RunSimulation()
        {
            using (var game = new GamePage())
            {
                Console.WriteLine(game.GridState);
                game.Turn(Move.Up);
                Console.WriteLine("Up");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Left);
                Console.WriteLine("Left");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Down);
                Console.WriteLine("Down");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Right);
                Console.WriteLine("Right");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Up);
                Console.WriteLine("Up");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Left);
                Console.WriteLine("Left");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Down);
                Console.WriteLine("Down");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Right);
                Console.WriteLine("Right");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Up);
                Console.WriteLine("Up");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Left);
                Console.WriteLine("Left");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Down);
                Console.WriteLine("Down");
                Console.WriteLine(game.GridState);
                game.Turn(Move.Right);
                Console.WriteLine("Right");
                Console.WriteLine(game.GridState);
            }
        }

        [Test]
        public void RunGameLogic_AgentTurnSelfCheck()
        {
            using (var game = new GamePage())
            {
                var moves = new[]
                {
                    Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, 
                    Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, 
                    Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right
                };

                foreach (var move in moves)
                {
                    var prevState = game.GridState;
                    var expected = GameLogic.MakeMove(prevState, move);
                    game.Turn(move);

                    var actual = game.GridStateNoNew;

                    if (expected.ToString() != actual.ToString())
                    {
                        Console.WriteLine("Failed selfcheck");
                        Console.WriteLine(prevState);
                        Console.WriteLine(move);
                        Console.WriteLine(actual);
                    }
                }
            }
        }

        [Test]
        public void RunGameLogic_WorldTurnSelfCheck()
        {
            using (var game = new GamePage())
            {
                var moves = new[]
                {
                    Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, 
                    Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right, Move.Up, Move.Left, 
                    Move.Down, Move.Right, Move.Up, Move.Left, Move.Down, Move.Right
                };

                foreach (var move in moves)
                {
                    game.Turn(move);
                    if (game.NewEl == 4)
                    {
                        continue;
                    }

                    var state = game.GridStateNoNew;
                    var possibleStates = GameLogic.NextPossibleWorldStates(state);

                    var actual = game.GridState;
                    if (possibleStates.All(s => s.ToString() != actual.ToString()))
                    {
                        Console.WriteLine("Failed selfcheck");
                        Console.WriteLine(game.GridStateNoNew);
                        Console.WriteLine(actual);
                    }
                }
            }
        }
    }
}