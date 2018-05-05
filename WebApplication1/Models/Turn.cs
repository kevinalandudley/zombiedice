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
                        // Santa and monkey are never re-added to the cup.
                        if (isFirstFill || (kind != DieKind.Santa && kind != DieKind.Monkey))
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
                case DieKind.Monkey:
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
            // Sort Banana.
            if (SortBanana())
            {
            }
            // Sort Monkey gets brains.
            else if (SortMonkeyGetsBrains())
            {
            }
            // Sort Monkey gets shotguns.
            else if (SortMonkeyGetsShotguns())
            {
            }
            // Sort Energy gets runners.
            else if (SortEnergyGetsRunners())
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
                    if (die.FaceGroup == DieFaceGroup.Brain || die.FaceGroup == DieFaceGroup.Shotgun)
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
            bool result = false;
            if (!HasBanana() && !HasMonkeyGetsBrains() && !HasMonkeyGetsShotguns() && !HasEnergyGetsRunners() && !HasShotgunSavesBrain(DieKind.Hunk, DieKind.Hottie) && !HasShotgunSavesBrain(DieKind.Hottie, DieKind.Hunk))
            {
                if (Hand.DieCount(DieFaceType.Brain) + Hand.DieCount(DieFaceType.Shotgun) == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool HasBanana()
        {
            // Return if hand has a banana.
            return Hand.HasDie(DieFaceType.Banana);
        }

        private bool HasMonkeyGetsBrains()
        {
            // Return if hand has a good monkey and hand has footprints.
            return Hand.HasDie(DieFaceType.GoodMonkey) && Hand.HasDie(DieFaceType.Footprints);
        }

        private bool HasMonkeyGetsShotguns()
        {
            // Return if hand has an evil monkey and hand has footprints.
            return Hand.HasDie(DieFaceType.EvilMonkey) && Hand.HasDie(DieFaceType.Footprints);
        }

        private bool HasEnergyGetsRunners()
        {
            // Return if hand or keep has energy drink and hand has green footprints.
            return (Hand.HasDie(DieFaceType.EnergyDrink) || Keep.HasDie(DieFaceType.EnergyDrink)) &&
                   (Hand.HasDie(DieKind.Green, DieFaceType.Footprints));
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

        bool SortBanana()
        {
            bool result = false;
            if (HasBanana())
            {
                // Return all dice in the hand to the cup.
                foreach (var die in Hand.Dice.ToList())
                {
                    Cup.Dice.Add(die);
                    Hand.Dice.Remove(die);
                }
                result = true;
            }
            return result;
        }

        bool SortMonkeyGetsBrains()
        {
            bool result = false;
            if (HasMonkeyGetsBrains())
            {
                // Change all footprints in the hand to brains.
                foreach (var die in Hand.Dice)
                {
                    if (die.FaceType == DieFaceType.Footprints)
                    {
                        if (die.FlipToBrain())
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        bool SortMonkeyGetsShotguns()
        {
            bool result = false;
            if (HasMonkeyGetsShotguns())
            {
                // Change all footprints in the hand to shotguns.
                foreach (var die in Hand.Dice)
                {
                    if (die.FaceType == DieFaceType.Footprints)
                    {
                        if (die.FlipToShotgun())
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
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
                        if (die.FlipToBrain())
                        {
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
            Banana,
            MonkeyGetsBrains,
            MonkeyGetsShotguns,
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
                    if (HasBanana())
                    {
                        return TurnMessageId.Banana;
                    }
                    else if (HasMonkeyGetsBrains())
                    {
                        return TurnMessageId.MonkeyGetsBrains;
                    }
                    else if (HasMonkeyGetsShotguns())
                    {
                        return TurnMessageId.MonkeyGetsShotguns;
                    }
                    else if (HasEnergyGetsRunners())
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
                    case TurnMessageId.Banana:
                        return "Banana!";
                    case TurnMessageId.MonkeyGetsBrains:
                        return "Monkey Gets Brains!";
                    case TurnMessageId.MonkeyGetsShotguns:
                        return "Monkey Gets Shotguns!";
                    case TurnMessageId.EnergyGetsRunners:
                        return "Energy Gets Runners!";
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
                    case TurnMessageId.FinalRound:
                        return "Last Round!";
                    case TurnMessageId.TieBreaker:
                        return "Tiebreaker!";
                    case TurnMessageId.GameOver:
                        return "";
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
                    case TurnMessageId.Banana:
                    case TurnMessageId.MonkeyGetsBrains:
                    case TurnMessageId.MonkeyGetsShotguns:
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







