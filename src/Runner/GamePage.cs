﻿namespace Runner
{
    using System;
    using System.Linq;
    using System.Threading;

    using AI2048.Game;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Remote;

    public class GamePage : IDisposable
    {
        private readonly RemoteWebDriver driver;

        private readonly IWebElement gameEl;

        public GamePage()
        {
            this.driver = new FirefoxDriver();
            this.driver.Navigate().GoToUrl("http://gabrielecirulli.github.io/2048/");

            this.gameEl = this.driver.FindElementByClassName("game-container");
        }

        public bool CanMove => this.driver.FindElementByClassName("game-message").Displayed == false;

        /// <summary>
        /// numeration starts from Upper Left corner
        /// </summary>
        /// <remarks>
        /// tile format is: <div class="tile tile-32 tile-position-2-1 tile-merged">32</div>
        /// </remarks>
        public LogarithmicGrid GridState => this.RetryOnSeleniumException(
            () =>
                {
                    var grid = new int[4, 4];

                    var tiles = this.driver.FindElementsByClassName("tile");
                    foreach (var tl in tiles)
                    {
                        var positionStr =
                            tl.GetAttribute("class")
                                .Split(' ')
                                .First(c => c.StartsWith("tile-position-"))
                                .Replace("tile-position-", string.Empty);
                        var x = int.Parse(positionStr[2].ToString()) - 1;
                        var y = int.Parse(positionStr[0].ToString()) - 1;
                        grid[x, y] = int.Parse(tl.Text);
                    }

                    return new LogarithmicGrid(grid);
                });

        /// <remarks>
        /// tile format is: <div class="tile tile-32 tile-position-2-1 tile-merged">32</div>
        /// </remarks>
        public LogarithmicGrid GridStateNoNew => this.RetryOnSeleniumException(
            () =>
                {
                    var grid = new int[4, 4];

                    var tiles =
                        this.driver.FindElementsByClassName("tile")
                            .Where(t => !t.GetAttribute("class").Contains("tile-new"));
                    foreach (var tl in tiles)
                    {
                        var positionStr =
                            tl.GetAttribute("class")
                                .Split(' ')
                                .First(c => c.StartsWith("tile-position-"))
                                .Replace("tile-position-", string.Empty);
                        var x = int.Parse(positionStr[2].ToString()) - 1;
                        var y = int.Parse(positionStr[0].ToString()) - 1;
                        grid[x, y] = int.Parse(tl.Text);
                    }

                    return new LogarithmicGrid(grid);
                });

        public string Score
            =>
                this.driver.FindElement(By.ClassName("score-container"))
                    .Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0];

        public void Dispose()
        {
            this.driver.Quit();
        }

        public Screenshot TakeScreenshot()
        {
            return this.driver.GetScreenshot();
        }

        public void Turn(Move move)
        {
            switch (move)
            {
                case Move.Left:
                    this.gameEl.SendKeys(Keys.Left);
                    break;
                case Move.Right:
                    this.gameEl.SendKeys(Keys.Right);
                    break;
                case Move.Up:
                    this.gameEl.SendKeys(Keys.Up);
                    break;
                case Move.Down:
                    this.gameEl.SendKeys(Keys.Down);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move));
            }
        }

        private T RetryOnSeleniumException<T>(Func<T> func, int retry = 1)
        {
            try
            {
                return func();
            }
            catch (StaleElementReferenceException)
            {
                Thread.Sleep(200);
                return this.RetryOnSeleniumException(func, retry + 1);
            }
            catch (FormatException)
            {
                // the world is not yet ready, trying again
                Thread.Sleep(200);
                return this.RetryOnSeleniumException(func, retry + 1);
            }
        }
    }
}