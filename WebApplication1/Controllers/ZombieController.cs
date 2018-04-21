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
                    model.Game.Turn.Draw();
                    break;
                case "Roll":
                    model.Game.Turn.Roll();
                    break;
                case "Sort":
                    model.Game.Turn.Sort();
                    break;
                case "Quit":
                    model.Game.NextTurn();
                    break;
            }
            return View("Index", model);
        }
    }
}
