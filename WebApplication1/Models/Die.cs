using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public class Die
    {

        public Die()
        {
            Kind = DieKind.NotSet;
        }

        public Die(DieKind kind)
        {
            Kind = kind;
            Face = Faces[0];
        }

        private static readonly Random _RandomGenerator = new System.Random();

        public DieKind Kind { get; set; }

        public List<DieFace> Faces
        {
            get
            {
                switch (Kind)
                {
                    case DieKind.Green:
                        return new List<DieFace> { DieFace.Brain, DieFace.Brain, DieFace.Brain, DieFace.Footprints, DieFace.Footprints, DieFace.Shotgun };
                    case DieKind.Yellow:
                        return new List<DieFace> { DieFace.Brain, DieFace.Brain, DieFace.Footprints, DieFace.Footprints, DieFace.Shotgun, DieFace.Shotgun };
                    case DieKind.Red:
                        return new List<DieFace> { DieFace.Brain, DieFace.Footprints, DieFace.Footprints, DieFace.Shotgun, DieFace.Shotgun, DieFace.Shotgun };
                    case DieKind.Hunk:
                        return new List<DieFace> { DieFace.DoubleBrain, DieFace.Footprints, DieFace.Footprints, DieFace.Shotgun, DieFace.Shotgun, DieFace.DoubleShotgun };
                    case DieKind.Hottie:
                        return new List<DieFace> { DieFace.Brain, DieFace.Heelprints, DieFace.Heelprints, DieFace.Heelprints, DieFace.Shotgun, DieFace.Shotgun };
                    case DieKind.Santa:
                        return new List<DieFace> { DieFace.DoubleBrain, DieFace.Brain, DieFace.Footprints, DieFace.Shotgun, DieFace.EnergyDrink, DieFace.Helmet };
                    case DieKind.Monkey:
                        return new List<DieFace> { DieFace.Brain, DieFace.Monkeyprints, DieFace.Shotgun, DieFace.GoodMonkey, DieFace.EvilMonkey, DieFace.Banana };
                    default:
                        return null;
                }
            }
            set { ; }
        }

        public DieFace Face { get; set; }

        public DieFaceType FaceType
        {
            get
            {
                switch (Face)
                {
                    case DieFace.Brain:
                    case DieFace.DoubleBrain:
                        return DieFaceType.Brain;
                    case DieFace.Footprints:
                    case DieFace.Heelprints:
                    case DieFace.Monkeyprints:
                        return DieFaceType.Footprints;
                    case DieFace.Shotgun:
                    case DieFace.DoubleShotgun:
                        return DieFaceType.Shotgun;
                    case DieFace.EnergyDrink:
                        return DieFaceType.EnergyDrink;
                    case DieFace.Helmet:
                        return DieFaceType.Helmet;
                    case DieFace.GoodMonkey:
                        return DieFaceType.GoodMonkey;
                    case DieFace.EvilMonkey:
                        return DieFaceType.EvilMonkey;
                    case DieFace.Banana:
                        return DieFaceType.Banana;
                    default:
                        return DieFaceType.NotSet;
                }
            }
        }

        public DieFaceGroup FaceGroup
        {
            get
            {
                switch (FaceType)
                {
                    case DieFaceType.Brain:
                    case DieFaceType.EnergyDrink:
                        return DieFaceGroup.Brain;
                    case DieFaceType.Shotgun:
                    case DieFaceType.Helmet:
                        return DieFaceGroup.Shotgun;
                    default:
                        return DieFaceGroup.None;
                }
            }
        }

        public string ImageName
        {
            get
            {
                return Kind.ToString() + Face.ToString() + ".gif";
            }
        }

        public bool FlipToBrain()
        {
            bool result = false;
            // Change face to brain.
            if (Faces.Contains(DieFace.Brain))
            {
                Face = DieFace.Brain;
                result = true;
            }
            else if (Faces.Contains(DieFace.DoubleBrain))
            {
                Face = DieFace.DoubleBrain;
                result = true;
            }
            return result;
        }

        public bool FlipToShotgun()
        {
            bool result = false;
            // Change face to shotgun.
            if (Faces.Contains(DieFace.Shotgun))
            {
                Face = DieFace.Shotgun;
                result = true;
            }
            else if (Faces.Contains(DieFace.DoubleShotgun))
            {
                Face = DieFace.DoubleShotgun;
                result = true;
            }
            return result;
        }

        public int BrainValue
        {
            get
            {
                switch (Face)
                {
                    case DieFace.Brain:
                        return 1;
                    case DieFace.DoubleBrain:
                        return 2;
                    default:
                        return 0;
                }
            }
        }

        public int ShotgunValue
        {
            get
            {
                switch (Face)
                {
                    case DieFace.Shotgun:
                        return 1;
                    case DieFace.DoubleShotgun:
                        return 2;
                    case DieFace.Helmet:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        public void Roll()
        {
            Face = Faces.ElementAt(_RandomGenerator.Next(0, Faces.Count));
        }

    }

    public enum DieKind
    {
        NotSet,
        Green,
        Yellow,
        Red,
        Hunk,
        Hottie,
        Santa,
        Monkey
    }

    public enum DieFace
    {
        Brain,
        Footprints,
        Shotgun,
        DoubleBrain,
        DoubleShotgun,
        Heelprints,
        EnergyDrink,
        Helmet,
        Monkeyprints,
        GoodMonkey,
        EvilMonkey,
        Banana
    }

    public enum DieFaceType
    {
        NotSet,
        Brain,
        Footprints,
        Shotgun,
        EnergyDrink,
        Helmet,
        GoodMonkey,
        EvilMonkey,
        Banana
    }

    public enum DieFaceGroup
    {
        None,
        Brain,
        Shotgun,
    }

}