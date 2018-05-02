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
            Turn = new Turn(RoundType, Players[ActivePlayerIndex].Score, HighScore);
        }

        public List<Player> Players { get; set; }

        public int ActivePlayerIndex { get; set; }

        public int ActivePlayerBrainValue
        {
            get
            {
                return Players[ActivePlayerIndex].Score + Turn.Keep.BrainValue;
            }
        }

        private int NextPlayerIndex
        {
            get
            {
                // Return the next player that is not out.
                int result = -1;
                int playerIndex = PlayerAfterIndex(ActivePlayerIndex);
                while (result < 0 && playerIndex != ActivePlayerIndex)
                {
                    if (!Players[playerIndex].IsOut)
                    {
                        result = playerIndex;
                    }
                    else
                    {
                        playerIndex = PlayerAfterIndex(playerIndex);
                    }
                }
                return result;
            }
        }

        public int StartingPlayerIndex { get; set; }

        public int WinnerPlayerIndex { get; set; }

        public Turn Turn { get; set; }

        public void NextTurn()
        {
            // Add brains to player score if not shotgunned.
            if (!Turn.IsShotgunned())
            {
                Players[ActivePlayerIndex].Score += Turn.Keep.BrainValue;
            }
            // Check for first player over 13. This sets the starting player for the final round.
            if (RoundType == GameRoundType.Regular && Players[ActivePlayerIndex].Score >= 13)
            {
                // It is now the final round.
                RoundType = GameRoundType.FinalRound;
                StartingPlayerIndex = ActivePlayerIndex;
            }
            // Check for last round or tie breaker.
            else if (RoundType == GameRoundType.FinalRound || RoundType == GameRoundType.TieBreaker)
            {
                // All players from the starting player to the active player are out if they are less than the high score.
                for (int i = 0; i <= Players.Count - 1; i++)
                {
                    if (Players[i].Score < HighScore && PlayerIndexIsBetween(i, StartingPlayerIndex, ActivePlayerIndex))
                    {
                        Players[i].IsOut = true;
                    }
                }
                // Check if the round is done.
                if (RoundIsDone)
                {
                    // Set the winning player if there is only one player with the high score.
                    if (Players.Count(x => x.Score == HighScore) == 1)
                    {
                        // Game is over.
                        RoundType = GameRoundType.GameOver;
                        WinnerPlayerIndex = Players.FindIndex(x => x.Score == HighScore);
                        ActivePlayerIndex = WinnerPlayerIndex;
                        Turn = new Turn(GameRoundType.GameOver, Players[ActivePlayerIndex].Score, HighScore);
                    }
                    else
                    {
                        // Final round is done. There is no winner. It is now a tiebreaker starting with the next player.
                        RoundType = GameRoundType.TieBreaker;
                        StartingPlayerIndex = NextPlayerIndex;
                    }
                }
            }
            // Set next player if the game is not over.
            if (RoundType != GameRoundType.GameOver)
            {
                ActivePlayerIndex = NextPlayerIndex;
                Turn = new Turn(RoundType, Players[ActivePlayerIndex].Score, HighScore);
            }
        }

        private bool RoundIsDone
        {
            get
            {
                bool result = false;
                // A "round" only occurs during the final round or tiebreaker.
                if (RoundType == GameRoundType.FinalRound || RoundType == GameRoundType.TieBreaker)
                {
                    // Check if the next player (including any next players that are out) is the starting player.
                    result = PlayerIndexIsBetween(StartingPlayerIndex, PlayerAfterIndex(ActivePlayerIndex), NextPlayerIndex);
                }
                return result;
            }
        }

        private bool PlayerIndexIsBetween(int playerindex, int startPlayerIndex, int endPlayerIndex)
        {
            // Return if the player index is between the start player and the end player (wrappping around to the first player (0) when the end player is before the start player).
            // The comparison includes the start and end players.
            if (startPlayerIndex <= endPlayerIndex)
            {
                return (playerindex >= startPlayerIndex && playerindex <= endPlayerIndex);
            }
            else
            {
                return (playerindex >= startPlayerIndex || playerindex <= endPlayerIndex);
            }
        }

        private int PlayerAfterIndex(int playerIndex)
        {
            return (playerIndex >= Players.Count - 1) ? 0 : playerIndex + 1;
        }

        public int HighScore
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
