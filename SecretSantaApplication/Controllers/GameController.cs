using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Extensions;
using SecretSantaApplication.Models;
using AppContext = SecretSantaApplication.Data.AppContext;

namespace SecretSantaApplication.Controllers
{
    public class GameController : Controller
    {
        private readonly AppContext _appContext;

        public GameController(AppContext appContext)
        {
            _appContext = appContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Start()
        {
            _appContext.SecretSantas.RemoveRange(_appContext.SecretSantas);

            var players = _appContext.Users.ToList();
            var targets = GetSantaTargets(players);
            foreach (var pairs in targets)
            {
                _appContext.Add(new SecretSanta
                {
                    Santa = pairs.Item1.EmailAddress,
                    Target = pairs.Item2.EmailAddress
                });
                _appContext.SaveChanges();
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