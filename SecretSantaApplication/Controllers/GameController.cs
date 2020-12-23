using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Extensions;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class GameController : Controller
    {
        private readonly Db_AppContext _dbAppContext;

        public GameController(Db_AppContext dbAppContext)
        {
            _dbAppContext = dbAppContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Start()
        {
            _dbAppContext.SecretSantas.RemoveRange(_dbAppContext.SecretSantas);

            var players = _dbAppContext.Users.ToList();
            var targets = GetSantaTargets(players);
            foreach (var pairs in targets)
            {
                _dbAppContext.Add(new SecretSanta
                {
                    Santa = pairs.Item1.EmailAddress,
                    Target = pairs.Item2.EmailAddress
                });
                _dbAppContext.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        private static IEnumerable<(T, T)> GetSantaTargets<T>(List<T> players)
        {
            var targets = new List<(T, T)>();
            var shuffledPlayers = players.Shuffle().ToList();
            for (int i = 0; i < shuffledPlayers.Count; i++)
            {
                if (i == shuffledPlayers.Count - 1)
                {
                    targets.Add((shuffledPlayers[i], shuffledPlayers[0]));
                    break;
                }

                targets.Add((shuffledPlayers[i], shuffledPlayers[i + 1]));
            }

            return targets;
        }
    }
}