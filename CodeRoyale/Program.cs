namespace CodeRoyale
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /**
     * Auto-generated code below aims at helping you parse
     * the standard input according to the problem statement.
     **/
    class Player
    {
        static void Main(string[] args)
        {
            string[] inputs;
            int numSites = int.Parse(Console.ReadLine());
            var sites = new Dictionary<int, Site>();
            for (int i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int siteId = int.Parse(inputs[0]);
                int x = int.Parse(inputs[1]);
                int y = int.Parse(inputs[2]);
                int radius = int.Parse(inputs[3]);
                var site = new Site()
                {
                    Id = siteId,
                    Position = new Position(x, y),
                    Radius = radius
                };

                sites.Add(siteId, site);
            }

            // game loop
            while (true)
            {
                var gameContext = new GameContext();
                inputs = Console.ReadLine().Split(' ');

                int gold = int.Parse(inputs[0]);
                gameContext.AvailableGold = gold;

                int touchedSite = int.Parse(inputs[1]); // -1 if none
                gameContext.TouchedSiteId = touchedSite;

                for (int i = 0; i < numSites; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int siteId = int.Parse(inputs[0]);
                    int ignore1 = int.Parse(inputs[1]); // used in future leagues
                    int ignore2 = int.Parse(inputs[2]); // used in future leagues
                    int structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
                    int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
                    int param1 = int.Parse(inputs[5]);
                    int param2 = int.Parse(inputs[6]);

                    var site = sites[siteId];
                    site.Owner = (OwnerType) owner;
                    if (structureType == Constants.BarracksIdentifier)
                    {
                        var barracks = new Barracks()
                        {
                            Id = siteId,
                            Position = site.Position,
                            Radius = site.Radius,
                            Owner = site.Owner,
                            TrainingCooldown = param1,
                            CreepType = (CreepType) param2
                        };

                        site = barracks;
                    }

                    gameContext.Sites.Add(siteId, site);
                }
                int numUnits = int.Parse(Console.ReadLine());
                for (int i = 0; i < numUnits; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int x = int.Parse(inputs[0]);
                    int y = int.Parse(inputs[1]);
                    int owner = int.Parse(inputs[2]);
                    int unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
                    int health = int.Parse(inputs[4]);

                    var unit = new Unit()
                    {
                        Position = new Position(x, y),
                        Owner = (OwnerType) owner,
                        Type = (UnitType) unitType,
                        Health = health
                    };

                    gameContext.Units.Add(unit);

                    if (unit.Type == UnitType.Queen)
                    {
                        if (unit.Owner == OwnerType.Friendly)
                        {
                            gameContext.MyQueen = unit;
                        }
                        else
                        {
                            gameContext.EnemyQueen = unit;
                        }
                    }
                }

                gameContext.ProcessTurn();
            }
        }
    }

    public static class Constants
    {
        public const int FieldWidth = 1920;
        public const int FieldHeight = 1000;
        public const int MaxTurns = 200;

        public const int StartingGold = 100;
        public const int GoldPerTurn = 10;

        public const int QueenRadius = 30;
        public const int QueenMoveRange = 60;
        public const int QueenMaxHealth = 100;

        public const string WaitCommand = "WAIT";
        public const string MoveCommand = "MOVE";
        public const string BuildCommand = "BUILD";
        public const string TrainCommand = "TRAIN";

        public const string BarracksStructure = "BARRACKS";
        public const int BarracksIdentifier = 2;

        public const string KnightType = "KNIGHT";
        public const string ArcherType = "ARCHER";
        public const string GiantType = "GIANT";

        public const int KnightGroupCost = 80;
        public const int ArcherGroupCost = 100;
    }

    public class GameContext
    {
        public static Random random = new Random();

        public GameContext()
        {
            this.Sites = new Dictionary<int, Site>();
            this.Units = new List<Unit>();
        }

        public int TouchedSiteId { get; set; }

        public int AvailableGold { get; set; }

        public Dictionary<int, Site> Sites { get; set; }

        public List<Unit> Units { get; set; }

        public Unit MyQueen { get; set; }

        public Unit EnemyQueen { get; set; }

        public void ProcessTurn()
        {
            PickCommandForQueen();
        }

        private void PickCommandForQueen()
        {
            var primaryHandlers = new List<ActionsHandler>()
            {
                new BuildBarracksHandler(),
                new MoveToClosestSiteHandler(),
                new DefaultWaitHandler()
            };

            var trainingHandlers = new List<ActionsHandler>()
            {
                new TrainAllBarracksHandler(),
                new DefaultTrainingHandler()
            };

            this.ProcessHandlers(primaryHandlers);
            this.ProcessHandlers(trainingHandlers);
        }

        public void ProcessHandlers(List<ActionsHandler> handlers)
        {
            for (int i = 1; i < handlers.Count; i++)
            {
                handlers[i - 1].SetSuccessor(handlers[i]);
            }

            handlers[0].ProcessTurn(this);
        }

        public void Wait()
        {
            Console.WriteLine(Constants.WaitCommand);
        }

        public void Train(IEnumerable<int> siteIds)
        {
            var command = Constants.TrainCommand + " " + string.Join(" ", siteIds);
            Console.WriteLine(command.Trim());
        }

        public void Build(int siteId, UnitType type)
        {
            Console.WriteLine($"{Constants.BuildCommand} {siteId} {Constants.BarracksStructure}-{type.ToString().ToUpper()}");
        }

        public void Move(Position position)
        {
            Console.WriteLine($"{Constants.MoveCommand} {position.X} {position.Y}");
        }

        public void Move(Entity entity)
        {
            Console.WriteLine($"{Constants.MoveCommand} {entity.Position.X} {entity.Position.Y}");
        }
    }

    public class BuildBarracksHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var queen = context.MyQueen;
            if (context.TouchedSiteId != -1)
            {
                var site = context.Sites[context.TouchedSiteId];
                if (site.Owner == OwnerType.Neutral)
                {
                    var type = (UnitType) GameContext.random.Next(0, 2);
                    context.Build(site.Id, type);
                    return true;
                }
            }

            return false;
        }
    }

    public class MoveToClosestSiteHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var queen = context.MyQueen;
            var closestSite = context.Sites.Values
                .Where(x => x.Owner == OwnerType.Neutral)
                .OrderBy(x => MathUtils.GetDistance(queen, x)).FirstOrDefault();
            context.Move(closestSite);
            return true;
        }
    }

    public class TrainAllBarracksHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var barracks = context.Sites.Values
                .Where(x => x is Barracks && x.Owner == OwnerType.Friendly)
                .Select(x => new
                {
                    Id = x.Id,
                    Cost = (x as Barracks).CreepCost
                });
            if (barracks.Any())
            {
                var barracksIds = new List<int>();

                foreach (var item in barracks)
                {
                    if (context.AvailableGold >= item.Cost)
                    {
                        barracksIds.Add(item.Id);
                        context.AvailableGold -= item.Cost;
                    }
                }

                context.Train(barracksIds);
                return true;
            }
            return false;
        }
    }

    public class DefaultTrainingHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            context.Train(Enumerable.Empty<int>());
            return true;
        }
    }

    public class DefaultWaitHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            context.Wait();
            return true;
        }
    }

    public abstract class ActionsHandler
    {
        protected ActionsHandler successor;

        public void SetSuccessor(ActionsHandler successor)
        {
            this.successor = successor;
        }

        public void ProcessTurn(GameContext context)
        {
            var result = this.CanProcessTurn(context);
            if (result)
            {
                return;
            }
            else if (successor != null)
            {
                successor.ProcessTurn(context);
            }
        }

        public abstract bool CanProcessTurn(GameContext context);
    }

    public class Site : Entity
    {
        public int Id { get; set; }
        public int Radius { get; set; }
        public OwnerType Owner { get; set; }
    }

    public class Barracks : Site
    {
        public int TrainingCooldown { get; set; }
        public CreepType CreepType { get; set; }

        public int CreepCost
        {
            get { return CreepType == CreepType.Archer ? Constants.ArcherGroupCost : Constants.KnightGroupCost; }
        }
    }

    public class Unit : Entity
    {

        public OwnerType Owner { get; set; }

        public UnitType Type { get; set; }

        public int Health { get; set; }
    }
    
    public class Entity
    {
        public Position Position { get; set; }
    }

    public enum OwnerType
    {
        Neutral = -1,
        Friendly = 0,
        Enemy = 1
    }

    public enum CreepType
    {
        Knight = 0,
        Archer = 1
    }

    public enum UnitType
    {
        Queen = -1,
        Knight = 0,
        Archer = 1
    }

    public static class MathUtils
    {
        public static double GetDistance(Entity a, Entity b)
        {
            return GetDistance(a.Position, b.Position);
        }

        public static double GetDistance(Position a, Position b)
        {
            var distance = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
            return distance;
        }

        public static Position GetPointAlongLine(Position p2, Position p1, double distance)
        {
            Position vector = new Position(p2.X - p1.X, p2.Y - p1.Y);
            double c = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            double a = distance / c;

            var newX = (int) (p1.X + vector.X * a);
            var newY = (int) (p1.Y + vector.Y * a);

            return new Position(newX, newY);
        }
    }

    public struct Position
    {
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(Object other)
        {
            return other is Position && Equals((Position) other);
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", X, Y);
        }
    }
}
