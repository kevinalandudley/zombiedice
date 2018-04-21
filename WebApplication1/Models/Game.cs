using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Game
    {

        public Game()
        {
            Players = new List<Player>();
            Players.Add(new Player("Kevin"));
            Players.Add(new Player("Joel"));
            Players.Add(new Player("Mom"));
            Players.Add(new Player("Dad"));


            Players[0].Score = 11;
            Players[1].Score = 11;
            Players[2].Score = 11;
            Players[3].Score = 11;


            ActivePlayerIndex = 0;
            StartingPlayerIndex = -1;
            WinnerPlayerIndex = -1;
            RoundType = GameRoundType.Regular;
            Turn = new Turn(RoundType, HighScore);
        }

        public List<Player> Players { get; set; }

        public int ActivePlayerIndex { get; set; }

        public int ActivePlayerBrainValue()
        {
            return Players[ActivePlayerIndex].Score + Turn.Keep.BrainValue();
        }

        private int NextPlayerIndex()
        {
            // Find the next player that is not out.
            int result = -1;
            // Find the next player in the list after the active player.
            if (ActivePlayerIndex < Players.Count() - 1)
            {
                for (int i = ActivePlayerIndex + 1; i < Players.Count && result < 0; i++)
                {
                    if (!Players[i].IsOut)
                    {
                        result = i;
                    }
                }
            }
            // Check for no player found.
            if (result < 0)
            {
                // Find the first player in the list before the active player.
                for (int i = 0; i < ActivePlayerIndex && result < 0; i++)
                {
                    if (!Players[i].IsOut)
                    {
                        result = i;
                    }
                }
            }
            return result;
        }

        public int StartingPlayerIndex { get; set; }

        public int WinnerPlayerIndex { get; set; }

        public Turn Turn { get; set; }

        public void NextTurn()
        {
            // Add brains to player score if not shotgunned.
            if (!Turn.IsShotgunned())
            {
                Players[ActivePlayerIndex].Score += Turn.Keep.BrainValue();
            }
            // Check for first player over 13. This sets the starting player for the last round.
            if (StartingPlayerIndex < 0 && Players[ActivePlayerIndex].Score >= 13)
            {
                // It is now the final round.
                RoundType = GameRoundType.FinalRound;
                StartingPlayerIndex = ActivePlayerIndex;
            }
            else
            {
                // Check last round or tie breaker.
                if (RoundType == GameRoundType.FinalRound || RoundType == GameRoundType.TieBreaker)
                {
                    // Flag player as out if they are less than the high score.
                    if (Players[ActivePlayerIndex].Score < HighScore)
                    {
                        Players[ActivePlayerIndex].IsOut = true;
                    }
                    // Check if next player is the starting player. This means the round is done.
                    if (NextPlayerIndex() == StartingPlayerIndex)
                    {
                        // Set the winning player if there is only one player with the high score.
                        if (Players.Count(x => x.Score == HighScore) == 1)
                        {
                            // Game is over.
                            RoundType = GameRoundType.GameOver;
                            WinnerPlayerIndex = Players.FindIndex(x => x.Score == HighScore);
                        }
                        else
                        {
                            // Final round is done. There is no winner. It is now a tiebreaker.
                            RoundType = GameRoundType.TieBreaker;
                            // Reset the starting player.
                            StartingPlayerIndex = NextPlayerIndex();
                        }
                    }
                }
            }
            // Set next player if the game is not over.
            if (RoundType != GameRoundType.GameOver)
            {
                ActivePlayerIndex = NextPlayerIndex();
                Turn = new Turn(RoundType, HighScore);
            }
            else
            {
                Turn.RoundType = RoundType;
            }
        }

        private int HighScore
        {
            get
            {
                return Players.Max(x => x.Score);
            }
        }


        public GameRoundType RoundType { get; set; }

    }

    public enum GameRoundType
    {
        Regular,
        FinalRound,
        TieBreaker,
        GameOver
    }
}
