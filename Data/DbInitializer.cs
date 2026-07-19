using BackLog.Models;

namespace BackLog.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Games.Any())
            {
                return;
            }
            else
            {
                var games = new Game[]
                {
                    new Game { Title = "Yakuza: Like a Dragon", Year = 2020, Genre = Genre.RPG, Platform = Platform.PC, Status = Status.Completed},
                    new Game { Title = "Dragon Quest XI S: Echoes of an Elusive Age", Year = 2019, Genre = Genre.RPG, Platform = Platform.Switch, Status = Status.Started },
                    new Game { Title = "Cuphead", Year = 2017, Genre = Genre.Shooter, Platform = Platform.Switch, Status = Status.FullyCompleted},
                    new Game { Title = "Elden Ring", Year = 2022, Genre = Genre.RPG, Platform = Platform.PC, Status = Status.Started},
                    new Game { Title = "Super Mario Galaxy", Year = 2007, Genre = Genre.Platformer, Platform = Platform.Switch, Status = Status.Completed},
                    new Game { Title = "Bowser's Fury", Year = 2021, Genre = Genre.Platformer, Platform = Platform.Switch, Status = Status.FullyCompleted},
                    new Game { Title = "Like a Dragon: Infinite Wealth", Year = 2024, Genre = Genre.RPG, Platform = Platform.PC, Status = Status.NotStarted}
                };

                context.Games.AddRange(games);
                context.SaveChanges();
            }

        


        }

    }
}
