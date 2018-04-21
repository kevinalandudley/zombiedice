using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Models
{
    public class DieCollection
    {
        public DieCollection()
        {
            Dice = new List<Die>();
        }

        public List<Die> Dice { get; set; }

        public void Roll()
        {
            foreach (var die in Dice)
            {
                die.Roll();
            }
        }

        public bool HasDie(DieFaceType faceType)
        {
            return Dice.Exists(x => x.FaceType == faceType);
        }

        public bool HasDie(DieKind kind, DieFaceType faceType)
        {
            return Dice.Exists(x => x.Kind == kind && x.FaceType == faceType);
        }

        public int DieCount(DieKind kind)
        {
            return Dice.FindAll(x => x.Kind == kind).Count;
        }

        public int DieCount(DieFaceType faceType)
        {
            return Dice.FindAll(x => x.FaceType == faceType).Count;
        }

        public int DieCount(DieKind kind, DieFaceType faceType)
        {
            return Dice.FindAll(x => x.Kind == kind && x.FaceType == faceType).Count;
        }

        public Die GetDie(DieKind kind, DieFaceType faceType)
        {
            return Dice.FirstOrDefault(x => x.Kind == kind && x.FaceType == faceType);
        }

        public int BrainValue()
        {
            int a = Dice.Sum(x => x.BrainValue);
            return Dice.Sum(x => x.BrainValue);
        }

        public int ShotgunValue()
        {
            return Dice.Sum(x => x.ShotgunValue);
        }

    }
}