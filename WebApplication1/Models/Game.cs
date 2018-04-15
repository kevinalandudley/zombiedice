﻿using System;

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

            turn = new Turn();

        }



        public List<Player> Players { get; set; }



        public int ActivePlayerIndex { get; set; }



        public int ActivePlayerBrainValue()

        {

            return Players[ActivePlayerIndex].Score + turn.Keep.BrainValue();

        }



        private int NextPlayerIndex()

        {

            // Find the next player that is not out.

            int result = -1;

            // Find the next player in the list after the active player.

            if (ActivePlayerIndex < Players.Count() - 1)

            {

                for (int i = ActivePlayerIndex += 1; i < Players.Count && result < 0; i++)

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



        public Turn turn { get; set; }



        public void NextTurn()

        {

            // Add brains to player score if not shotgunned.

            if (!turn.IsShotgunned())

            {

                Players[ActivePlayerIndex].Score += turn.Keep.BrainValue();

            }

            // Check for first player over 13. This sets the starting player for the last round.

            if (StartingPlayerIndex < 0 && Players[ActivePlayerIndex].Score >= 13)

            {

                StartingPlayerIndex = ActivePlayerIndex;

            }

            else

            {

                // Check if next player is the starting player. This means the last round is done.

                if (NextPlayerIndex() == StartingPlayerIndex)

                {

                    // Final round is done.

                    IsTiebreaker = true;

                    // Flag players as out if they are less than the high score.

                    int highScore = Players.Max(x => x.Score);

                    foreach (var player in Players)

                    {

                        if (player.Score < highScore)

                        {

                            player.IsOut = true;

                        }
                        

                    }
                    StartingPlayerIndex = NextPlayerIndex();
                    // Set the winning player if there is only one player with the high score.

                    if (Players.Count(x => x.Score == highScore) == 1)

                    {

                        WinnerPlayerIndex = Players.FindIndex(x => x.Score == highScore);

                    }

                }

            }

            // Set next player.

            if (!IsDone())

            {

                ActivePlayerIndex = NextPlayerIndex();

                turn = new Turn();

            }

        }



        public bool IsDone()

        {

            return WinnerPlayerIndex != -1;

        }



        public bool IsTiebreaker { get; set; }



        public string message()

        {

            // Check for winner.

            if (WinnerPlayerIndex >= 0)

            {

                return Players[WinnerPlayerIndex].Name + "wins!";

            }

            else

            {

                // Check for last round or tiebreaker.

                if (StartingPlayerIndex >= 0)

                {

                    if (IsTiebreaker)

                    {

                        return "Tiebreaker!";

                    }

                    else

                    {

                        return "Last Round!";

                    }

                }

                else

                {

                    return "";

                }

            }

        }

    }



}