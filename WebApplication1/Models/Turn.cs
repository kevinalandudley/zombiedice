using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Turn
    {

        public Turn()
        {
            _NextAction = Action.Draw;
            _Cup = new DieCollection();
            Hand = new DieCollection();
            Keep = new DieCollection();
        }

        private static readonly Random randomGenerator = new System.Random();

        public DieCollection _Cup;
        public DieCollection Cup
        {
            get
            {
                // Fill the cup if it is empty.
                if (_Cup.Dice.Count == 0)
                {
                    foreach (DieKind kind in System.Enum.GetValues(typeof(DieKind)))
                    {
                        // Santa is never re-added to the cup.
                        if (kind != DieKind.Santa || !CupWasFilled)
                        {
                            // Fill the cup with the maximum number of dice for for this die type.
                            // Dice in the hand and shotguns in the keep are not re-added to the cup.
                            while (CupMaxDieCount(kind) > (_Cup.DieCount(kind) + Hand.DieCount(kind) + Keep.DieCount(kind, DieFaceType.Shotgun)))
                            {
                                _Cup.Dice.Add(new Die(kind));
                            }
                        }
                    }
                    CupWasFilled = true;
                }
                return _Cup;
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

        public bool CupWasFilled { get; set; }

        public DieCollection Hand { get; set; }

        public DieCollection Keep { get; set; }

        public void Draw()
        {
            while (Hand.Dice.Count < 3)
            {
                // Get a random die from the cup.
                Die die = Cup.Dice[randomGenerator.Next(0, Cup.Dice.Count)];
                // Roll the die so that it starts with a random face in the hand.
                die.Roll();
                // Add the die to the hand and remove it from the cup.
                Hand.Dice.Add(die);
                _Cup.Dice.Remove(die);
            }
            // Set the next action.
            _NextAction = Action.Roll;
        }

        public void Roll()
        {
            // Roll the hand.
            Hand.Roll();
            // Set the next action.
            if (Hand.Dice.FindAll(x => x.FaceType == DieFaceType.Footprints).Count() >= Hand.Dice.Count())
            {
                // All dice in the hand are footprints, so there is nothing to sort.
                _NextAction = Action.DrawQuit;
            }
            else
            {
                _NextAction = Action.Sort;
            }
        }

        public void Sort()
        {
            // Check for Energy flips runners.
            if (SortEnergyGetsRunners())
            {
                Message = "Energy Gets Runners!";
                _NextAction = Action.Sort;
            }
            else
            {
                // Check for Hunk saves Hottie.
                if (SortShotgunSavesBrain(DieKind.Hunk, DieKind.Hottie))
                {
                    Message = "Hunk Saves Hottie!";
                    _NextAction = Action.Sort;
                }
                else
                {
                    // Check for Hottie saves Hunk.
                    if (SortShotgunSavesBrain(DieKind.Hottie, DieKind.Hunk))
                    {
                        Message = "Hottie Saves Hunk!";
                        _NextAction = Action.Sort;
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
                        // Check if shotgunned.
                        if (IsShotgunned())
                        {
                            Message = "Shotgunned!";
                            _NextAction = Action.Quit;
                        }
                        else
                        {
                            // Set the next action.
                            _NextAction = Action.DrawQuit;
                        }
                    }
                }
            }
        }

        public bool IsShotgunned()
        {
            return Keep.ShotgunValue() >= 3;
        }

        bool SortEnergyGetsRunners()
        {
            bool result = false;
            // Check if hand or keep has Energy.
            if (Hand.HasDie(DieFaceType.EnergyDrink) || Keep.HasDie(DieFaceType.EnergyDrink))
            {
                // Flip all green footprints in the hand to brains.
                foreach (var die in Hand.Dice)
                {
                    if (die.Kind == DieKind.Green && die.FaceType == DieFaceType.Footprints)
                    {
                        if (die.Faces.Contains(DieFace.Brain))
                        {
                            die.Face = DieFace.Brain;
                            result = true;
                        }
                        else
                        {
                            if (die.Faces.Contains(DieFace.DoubleBrain))
                            {
                                die.Face = DieFace.DoubleBrain;
                                result = true;
                            }
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
            if (Hand.GetDie(shotgunDieKind, DieFaceType.Shotgun) != null)
            {
                // Check if hand or keep has brain for the brain type.
                // Move the saved brain into the cup.
                Die brainDie = Hand.GetDie(brainDieKind, DieFaceType.Brain);
                if (brainDie != null)
                {
                    _Cup.Dice.Add(brainDie);
                    Hand.Dice.Remove(brainDie);
                    result = true;
                }
                else
                {
                    brainDie = Keep.GetDie(brainDieKind, DieFaceType.Brain);
                    if (brainDie != null)
                    {
                        _Cup.Dice.Add(brainDie);
                        Keep.Dice.Remove(brainDie);
                        result = true;
                    }
                }
            }
            return result;
        }

        public string Message;

        public int BrainValue()
        {
            return Keep.BrainValue();
        }

        public int ShotgunValue()
        {
            return Keep.ShotgunValue();
        }

        public enum Action
        {
            Draw,
            DrawQuit,
            Roll,
            Sort,
            Quit
        }

        private Action _NextAction;
        public Action NextAction
        {
            get
            {
                return _NextAction;
            }
        }

        public string NextActionName
        {
            get
            {
                switch (NextAction)
                {
                    case WebApplication1.Models.Turn.Action.Draw:
                    case WebApplication1.Models.Turn.Action.DrawQuit:
                        return "Draw";
                    case WebApplication1.Models.Turn.Action.Roll:
                        return "Roll";
                    case WebApplication1.Models.Turn.Action.Sort:
                        return "Sort";
                    default:
                        return "";
                }
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
// 1. Jail - Turn is over - score brains in keep and ignore in the hand
// 2. Police - Brains turn to runners
// 3. Shield - Saves one shotgun (like helmet)
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







