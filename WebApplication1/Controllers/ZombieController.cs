using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ZombieController : Controller
    {
        // GET: Zombie
        public ActionResult Index()
        {
            return View(new ZombieModel());
        }

        [HttpPost]
        public ActionResult NextAction(ZombieModel model, string command)
        {
            switch (command)
            {
                case "Draw":
                    model.game.turn.Draw();
                    break;
                case "Roll":
                    model.game.turn.Roll();
                    break;
                case "Sort":
                    model.game.turn.Sort();
                    break;
                case "Quit":
                    model.game.NextTurn();
                    break;
            }
            return View("Index", model);
        }
    }
}
