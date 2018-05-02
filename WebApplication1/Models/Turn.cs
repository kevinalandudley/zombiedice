using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Turn
    {

        public Turn()
            : this(GameRoundType.Regular, 0, 0)
        {
        }

        public Turn(GameRoundType roundType, int playerBrainValue, int highScore)
        {
            RoundType = roundType;
            PlayerBrainValue = playerBrainValue;
            HighScore = highScore;
            LastAction = Action.Start;
            Cup = new DieCollection();
            Hand = new DieCollection();
            Keep = new DieCollection();
        }

        private static readonly Random randomGenerator = new System.Random();

        public GameRoundType RoundType { get; set; }

        public int HighScore { get; set; }

        public int PlayerBrainValue { get; set; }

        public DieCollection Cup { get; set; }
        private DieCollection _Cup
        {
            get
            {
                // Fill the cup if it is empty.
                if (Cup.Dice.Count == 0)
                {
                    // This is the first time the cup is filled if there are no dice in the hand or keep.
                    bool isFirstFill = Hand.Dice.Count + Keep.Dice.Count == 0;
                    foreach (DieKind kind in System.Enum.GetValues(typeof(DieKind)))
                    {
                        // Santa is never re-added to the cup.
                        if (isFirstFill || kind != DieKind.Santa)
                        {
                            // Fill the cup with the maximum number of dice for this die type.
                            // Dice in the hand and shotguns in the keep are not re-added to the cup.
                            while (CupMaxDieCount(kind) > (Cup.DieCount(kind) + Hand.DieCount(kind) + Keep.DieCount(kind, DieFaceType.Shotgun)))
                            {
                                Cup.Dice.Add(new Die(kind));
                            }
                        }
                    }
                }
                return Cup;
            }
        }

        private int CupMaxDieCount(DieKind kind)
        {
            switch (kind)
            {
                case DieKind.Green:
                    return 5;
                case DieKind.Yellow:
                    return 2;
                case DieKind.Red:
                    return 3;
                case DieKind.Hunk:
                    return 1;
                case DieKind.Hottie:
                    return 1;
                case DieKind.Santa:
                    return 1;
                default:
                    return 0;
            }
        }

        public DieCollection Hand { get; set; }

        public DieCollection Keep { get; set; }

        public void Draw()
        {
            while (Hand.Dice.Count < 3 && _Cup.Dice.Count > 0)
            {
                // Get a random die from the cup.
                Die die = _Cup.Dice[randomGenerator.Next(0, _Cup.Dice.Count)];
                // Roll the die so that it starts with a random face in the hand.
                die.Roll();
                // Add the die to the hand and remove it from the cup.
                Hand.Dice.Add(die);
                Cup.Dice.Remove(die);
            }
            // Set last action.
            LastAction = Action.Draw;
        }

        public void Roll()
        {
            // Roll the hand.
            Hand.Roll();
            // Set last action.
            LastAction = Action.Roll;
        }

        public void Sort()
        {
            // Sort Energy gets runners.
            if (SortEnergyGetsRunners())
            {
            }
            // Sort Hunk saves Hottie.
            else if (SortShotgunSavesBrain(DieKind.Hunk, DieKind.Hottie))
            {
            }
            // Sort Hottie saves Hunk.
            else if (SortShotgunSavesBrain(DieKind.Hottie, DieKind.Hunk))
            {
            }
            else
            {
                // Move kept dice from the hand to the keep.
                foreach (var die in Hand.Dice.ToList())
                {
                    // Footprints are not removed from the hand.
                    if (die.FaceType != DieFaceType.Footprints)
                    {
                        Keep.Dice.Add(die);
                        Hand.Dice.Remove(die);
                    }
                }

            }
            // Set last action.
            LastAction = Action.Sort;
        }

        private bool IsSorted()
        {
            // Hand is sorted if there are only footprints left and no other sort actions are needed.
            return Hand.DieCount(DieFaceType.Footprints) == Hand.Dice.Count && !HasEnergyGetsRunners() && 
                   !HasShotgunSavesBrain(DieKind.Hunk, DieKind.Hottie) && !HasShotgunSavesBrain(DieKind.Hottie, DieKind.Hunk);
        }

        private bool HasEnergyGetsRunners()
        {
            // Return if hand or keep has energy drink and hand has green footprints.
            return (Hand.HasDie(DieFaceType.EnergyDrink) || Keep.HasDie(DieFaceType.EnergyDrink)) &&
                   (Hand.Dice.Exists(x => x.Kind == DieKind.Green && x.FaceType == DieFaceType.Footprints));
        }

        private bool HasShotgunSavesBrain(DieKind shotgunDieKind, DieKind brainDieKind)
        {
            // Return if hand has shotgun for the shotgun type and hand or keep has brain for the brain type.
            return (Hand.HasDie(shotgunDieKind, DieFaceType.Shotgun) && 
                   (Hand.HasDie(brainDieKind, DieFaceType.Brain) || Keep.HasDie(brainDieKind, DieFaceType.Brain)));
        }

        public bool IsShotgunned()
        {
            // Return if shotguns in hand and keep are over maximum shotgun value.
            return Hand.ShotgunValue + Keep.ShotgunValue >= 3;
        }

        bool SortEnergyGetsRunners()
        {
            bool result = false;
            if (HasEnergyGetsRunners())
            {
                // Change all green footprints in the hand to brains.
                foreach (var die in Hand.Dice)
                {
                    if (die.Kind == DieKind.Green && die.FaceType == DieFaceType.Footprints)
                    {
                        if (die.Faces.Contains(DieFace.Brain))
                        {
                            die.Face = DieFace.Brain;
                            result = true;
                        }
                        else if (die.Faces.Contains(DieFace.DoubleBrain))
                        {
                            die.Face = DieFace.DoubleBrain;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        bool SortShotgunSavesBrain(DieKind shotgunDieKind, DieKind brainDieKind)
        {
            bool result = false;
            // Check if hand has shotgun for the shotgun type.
            if (HasShotgunSavesBrain(shotgunDieKind, brainDieKind))
            {
                // Check if hand or keep has brain for the brain type.
                // Move the saved brain into the cup.
                Die brainDie = Hand.GetDie(brainDieKind, DieFaceType.Brain);
                if (brainDie != null)
                {
                    Cup.Dice.Add(brainDie);
                    Hand.Dice.Remove(brainDie);
                    result = true;
                }
                else
                {
                    brainDie = Keep.GetDie(brainDieKind, DieFaceType.Brain);
                    if (brainDie != null)
                    {
                        Cup.Dice.Add(brainDie);
                        Keep.Dice.Remove(brainDie);
                        result = true;
                    }
                }
            }
            return result;
        }

        public enum Action
        {
            Start,
            Draw,
            Roll,
            Sort,
            Quit
        }

        public Action LastAction { get; set; }

        public Action NextAction
        {
            get
            {
                switch (LastAction)
                {
                    case Action.Start:
                        return Action.Draw;
                    case Action.Draw:
                        return Action.Roll;
                    case Action.Roll:
                        // If all dice in the hand are footprints there is nothing to sort, so re-roll.
                        return (Hand.IsAll(DieFaceType.Footprints)) ? Action.Sort : Action.Roll;
                    case Action.Sort:
                        // Check if hand is sorted.
                        if (IsSorted())
                        {
                            // Next action is to draw unless player is shotgunned.
                            return (IsShotgunned()) ? Action.Quit : Action.Draw;
                        }
                        else
                        {
                            return Action.Sort;
                        }
                    case Action.Quit:
                        return Action.Quit;
                    default:
                        return Action.Draw;
                }
            }
        }

        public string NextActionName
        {
            get
            {
                switch (NextAction)
                {
                    case Action.Draw:
                        return "Draw";
                    case Action.Roll:
                        return "Roll";
                    case Action.Sort:
                        return "Sort";
                    case Action.Quit:
                        return "Quit";
                    default:
                        return "";
                }
            }
        }

        public bool AllowQuit()
        {
            bool result = false;
            // Always allow quit if the next action is quit.
            if (NextAction == Action.Quit)
            {
                result = true;
            }
            else
            {
                // Don't allow quit if in the final round or tiebreaker unless tied or leading.
                if ((RoundType != GameRoundType.FinalRound && RoundType != GameRoundType.TieBreaker) || PlayerBrainValue + BrainValue >= HighScore)
                {
                    // Allow quit if next action is draw unless it is the start of the turn.
                    if (NextAction == Action.Draw && LastAction != Action.Start)
                    {
                        result = true;
                    }
                    // Allow quit when re-rolling.
                    else if (NextAction == Action.Roll && LastAction != Action.Draw)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private enum TurnMessageId
        {
            None,
            EnergyGetsRunners,
            HunkSavesHottie,
            HottieSavesHunk,
            Shotgunned,
            Leading,
            Tied,
            FinalRound,
            TieBreaker,
            GameOver
        }

        private TurnMessageId MessageId
        {
            get
            {
                // Get sort message.
                if (NextAction == Action.Sort || NextAction == Action.Quit)
                {
                    if (HasEnergyGetsRunners())
                    {
                        return TurnMessageId.EnergyGetsRunners;
                    }
                    else if (HasShotgunSavesBrain(DieKind.Hunk, DieKind.Hottie))
                    {
                        return TurnMessageId.HunkSavesHottie;
                    }
                    else if (HasShotgunSavesBrain(DieKind.Hottie, DieKind.Hunk))
                    {
                        return TurnMessageId.HottieSavesHunk;
                    }
                    else if (IsShotgunned())
                    {
                        return TurnMessageId.Shotgunned;
                    }
                    else
                    {
                        // Get round message.
                        return RoundMessageId;
                    }
                }
                else
                {
                    // Get round message.
                    return RoundMessageId;
                }
            }
        }

        private TurnMessageId RoundMessageId
        {
            get
            {
                TurnMessageId result = TurnMessageId.None;
                // Message ID for game over.
                if (RoundType == GameRoundType.GameOver)
                {
                    result = TurnMessageId.GameOver;
                }
                // Message ID for final round or tiebreaker and leading or tied.
                else if ((RoundType == GameRoundType.FinalRound || RoundType == GameRoundType.TieBreaker || PlayerBrainValue + BrainValue >= 13) && AllowQuit())
                {
                    if (PlayerBrainValue + BrainValue == HighScore)
                    {
                        result = TurnMessageId.Tied;
                    }
                    else if (PlayerBrainValue + BrainValue > HighScore)
                    {
                        result = TurnMessageId.Leading;
                    }
                }
                if (result == TurnMessageId.None)
                {
                    // Message ID for round type.
                    switch (RoundType)
                    {
                        case GameRoundType.GameOver:
                            return TurnMessageId.GameOver;
                        case GameRoundType.FinalRound:
                            return TurnMessageId.FinalRound;
                        case GameRoundType.TieBreaker:
                            return TurnMessageId.TieBreaker;
                        default:
                            return TurnMessageId.None;
                    }
                }
                return result;
            }
        }

        public string Message
        {
            get
            {
                switch (MessageId)
                {
                    case TurnMessageId.EnergyGetsRunners:
                        return "Energy!";
                    case TurnMessageId.HunkSavesHottie:
                        return "Hunk Saves Hottie!";
                    case TurnMessageId.HottieSavesHunk:
                        return "Hottie Saves Hunk!";
                    case TurnMessageId.Shotgunned:
                        return "Shotgunned!";
                    case TurnMessageId.Tied:
                        return "Tied!";
                    case TurnMessageId.Leading:
                        return "Leading!";
                    case TurnMessageId.GameOver:
                        return "";
                    case TurnMessageId.FinalRound:
                        return "Last Round!";
                    case TurnMessageId.TieBreaker:
                        return "Tiebreaker!";
                    default:
                        return "";
                }
            }
        }

        public enum TurnMessageType
        {
            SortAction,
            Standing,
            RoundStatus
        }

        public TurnMessageType MessageType
        {
            get
            {
                switch (MessageId)
                {
                    case TurnMessageId.EnergyGetsRunners:
                    case TurnMessageId.HunkSavesHottie:
                    case TurnMessageId.HottieSavesHunk:
                    case TurnMessageId.Shotgunned:
                        return TurnMessageType.SortAction;
                    case TurnMessageId.Tied:
                    case TurnMessageId.Leading:
                        return TurnMessageType.Standing;
                    default:
                        return TurnMessageType.RoundStatus;
                }
            }
        }

        public int BrainValue
        {
            get
            {
                return Keep.BrainValue;
            }
        }

        public int ShotgunValue
        {
            get
            {
                return Keep.ShotgunValue;
            }
        }

        public int TurnBrainValue
        {
            get
            {
                return Hand.BrainValue + Keep.BrainValue;
            }
        }

    }
}


// Monkey Die
//
// 1. Good Monkey  - Turns runnners to brain
// 2. Evil Monkey  - Turns runnners to shotgun
// 3. Banana       - Reroll
// 4. Brain        
// 5. Shotgun
// 6. Monkey Paw (footprints)

// Cops & Robbers
// 
// Cop
// 1. Cop - Brains turn to runners
// 2. Shield - Saves one shotgun (like helmet)
// 3. Jail - Turn is over - score brains in keep and ignore in the hand
// 4. Footprints
// 5. Brain
// 6. Brain 

// Robber
// 1. Robber - Steals one brain
// 2. Shotgun - Steals shield (if available) or kills cop if rolled otherwise regular shotgun
// 3. Footprints
// 4. Footprints
// 5. Brain
// 6. Double brain







