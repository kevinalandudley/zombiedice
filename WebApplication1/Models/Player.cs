using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Player
    {

        public Player() 
        {
        }

        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int Score { get; set; }

        public bool IsOut { get; set; }

    }
}