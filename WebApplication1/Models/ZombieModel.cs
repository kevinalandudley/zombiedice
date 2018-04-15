using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ZombieModel
    {

        public ZombieModel()
        {
            game = new Game();
        }

        public virtual Game game { get; set; }

    }
}