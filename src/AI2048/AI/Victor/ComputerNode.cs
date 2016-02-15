namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class ComputerNode
    {
        private readonly PlayerNode parentNode;

        public Grid State { get; }

        public ComputerNode(Grid state, PlayerNode parentNode)
        {
            this.parentNode = parentNode;
            this.State = state;

            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount);
            this.childrenLazy = new Lazy<PlayerNode[]>(() => this.GetChildren().ToArray());
        }

        public PlayerNode RootPlayerNode => this.parentNode.RootPlayerNode;
        
        public IDictionary<Grid, PlayerNode> KnownPlayerNodes => this.parentNode.KnownPlayerNodes;

        public IDictionary<Grid, ComputerNode> KnownComputerNodes => this.parentNode.KnownComputerNodes;

        public PlayerNode[] Children => this.childrenLazy.Value;

        public bool GameOver => this.parentNode.GameOver;

        private bool isTerminal;
        public bool IsTerminal => this.GameOver || this.isTerminal || this.parentNode.IsTernminal;
        public void MakeTerminal()
        {
            this.isTerminal = true;
        }

        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private readonly Lazy<int> emptyCellCountLazy;
        private int GetEmptyCellCount() => this.State.EmptyCellsNo;

        private readonly Lazy<PlayerNode[]> childrenLazy;
        private IEnumerable<PlayerNode> GetChildren()
        {
            var possibleStates = GameLogic.NextPossibleWorldStates(this.State);
            foreach (var possibleState in possibleStates)
            {
                PlayerNode playerNode;
                if (!this.KnownPlayerNodes.TryGetValue(possibleState, out playerNode))
                {
                    playerNode = new PlayerNode(possibleState, this);
                    this.KnownPlayerNodes.Add(possibleState, playerNode);
                }

                yield return playerNode;
            }
        }
    }
}