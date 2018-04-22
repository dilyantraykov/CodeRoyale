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

            var round = 0;
            // game loop
            while (true)
            {
                var gameContext = new GameContext() { Round = ++round };
                inputs = Console.ReadLine().Split(' ');

                int gold = int.Parse(inputs[0]);
                gameContext.AvailableGold = gold;

                int touchedSite = int.Parse(inputs[1]); // -1 if none
                gameContext.TouchedSiteId = touchedSite;

                for (int i = 0; i < numSites; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int siteId = int.Parse(inputs[0]);
                    int remainingGold = int.Parse(inputs[1]); // used in future leagues
                    int maxMineSize = int.Parse(inputs[2]); // used in future leagues
                    int structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
                    int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
                    int param1 = int.Parse(inputs[5]);
                    int param2 = int.Parse(inputs[6]);

                    var site = sites[siteId];
                    site.Owner = (OwnerType) owner;
                    site.MaximumGoldRate = maxMineSize;
                    site.RemainingGold = remainingGold;
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

                        if (site.Owner == OwnerType.Friendly)
                        {
                            gameContext.MyBarracks.Add(barracks);
                        }
                        else
                        {
                            gameContext.EnemyBarracks.Add(barracks);
                        }
                    }
                    else if (structureType == Constants.TowerIdentifier)
                    {
                        var tower = new Tower()
                        {
                            Id = siteId,
                            Position = site.Position,
                            Radius = site.Radius,
                            Owner = site.Owner,
                            Health = param1,
                            AttackRadius = param2
                        };

                        site = tower;
                        if (site.Owner == OwnerType.Friendly)
                        {
                            gameContext.MyTowers.Add(tower);
                        }
                        else
                        {
                            gameContext.EnemyTowers.Add(tower);
                        }
                    }
                    else if (structureType == Constants.MineIdentifier)
                    {
                        var mine = new Mine()
                        {
                            Id = siteId,
                            Position = site.Position,
                            Radius = site.Radius,
                            Owner = site.Owner,
                            CurrentGoldRate = param1
                        };

                        site = mine;
                        if (site.Owner == OwnerType.Friendly)
                        {
                            gameContext.MyMines.Add(mine);
                        }
                        else
                        {
                            gameContext.EnemyMines.Add(mine);
                        }
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

                    switch (unit.Type)
                    {
                        case UnitType.Queen:
                            if (unit.Owner == OwnerType.Friendly)
                            {
                                gameContext.MyQueen = unit;
                                if (round == 1)
                                {
                                    gameContext.QueenMaxHealth = health;
                                }
                            }
                            else
                            {
                                gameContext.EnemyQueen = unit;
                            }
                            break;
                        case UnitType.Archer:
                            if (unit.Owner == OwnerType.Friendly)
                            {
                                gameContext.MyArchers.Add(unit);
                            }
                            else
                            {
                                gameContext.EnemyArchers.Add(unit);
                            }
                            break;
                        case UnitType.Knight:
                            if (unit.Owner == OwnerType.Friendly)
                            {
                                gameContext.MyKnights.Add(unit);
                            }
                            else
                            {
                                gameContext.EnemyKnights.Add(unit);
                            }
                            break;
                        case UnitType.Giant:
                            if (unit.Owner == OwnerType.Friendly)
                            {
                                gameContext.MyGiants.Add(unit);
                            }
                            else
                            {
                                gameContext.EnemyGiants.Add(unit);
                            }
                            break;
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

        public const int QueenRadius = 30;
        public const int QueenSpeed = 60;

        public const string WaitCommand = "WAIT";
        public const string MoveCommand = "MOVE";
        public const string BuildCommand = "BUILD";
        public const string TrainCommand = "TRAIN";

        public const string BarracksStructure = "BARRACKS";
        public const int BarracksIdentifier = 2;

        public const string TowerStructure = "TOWER";
        public const int TowerIdentifier = 1;

        public const string MineStructure = "MINE";
        public const int MineIdentifier = 0;

        public const string KnightType = "KNIGHT";
        public const string ArcherType = "ARCHER";
        public const string GiantType = "GIANT";

        public const int KnightGroupCost = 80;
        public const int KnightGroupCount = 4;
        public const int KnightSpeed = 100;
        public const int KnightDamage = 1;
        public const int KnightMaxHealth = 30;
        public const int KnightTrainingTime = 5;
        public const int KnightRadius = 20;

        public const int ArcherGroupCost = 100;
        public const int ArcherGroupCount = 2;
        public const int ArcherSpeed = 75;
        public const int ArcherDamageToGiants = 10;
        public const int ArcherDamageToCreeps = 2;
        public const int ArcherMaxHealth = 45;
        public const int ArcherTrainingTime = 8;
        public const int ArcherRadius = 25;

        public const int GiantGroupCost = 140;
        public const int GiantGroupCount = 1;
        public const int GiantSpeed = 50;
        public const int GiantDamage = 80;
        public const int GiantMaxHealth = 200;
        public const int GiantTrainingTime = 10;
        public const int GiantRadius = 40;

        public const int SiteDiscoveryRange = 300;

        public const int TowerBaseDamageToCreeps = 3;
        public const int TowerBaseDamageToQueen = 1;
        public const int TowerBaseDamageRangeGain = 200;
        public const int MaxTowerHealth = 800;
    }

    public class GameContext
    {
        public static Random random = new Random();

        public GameContext()
        {
            this.Sites = new Dictionary<int, Site>();
            this.Units = new List<Unit>();
            this.MyArchers = new List<Unit>();
            this.EnemyArchers = new List<Unit>();
            this.MyKnights = new List<Unit>();
            this.EnemyKnights = new List<Unit>();
            this.MyGiants = new List<Unit>();
            this.EnemyGiants = new List<Unit>();
            this.MyBarracks = new List<Barracks>();
            this.EnemyBarracks = new List<Barracks>();
            this.MyTowers = new List<Tower>();
            this.EnemyTowers = new List<Tower>();
            this.MyMines = new List<Mine>();
            this.EnemyMines = new List<Mine>();
        }

        public int QueenMaxHealth { get; set; }

        public int Round { get; set; }

        public int TouchedSiteId { get; set; }

        public int AvailableGold { get; set; }

        public Dictionary<int, Site> Sites { get; set; }

        public List<Unit> Units { get; set; }

        public Unit MyQueen { get; set; }

        public Unit EnemyQueen { get; set; }

        public List<Unit> MyArchers { get; set; }

        public List<Unit> EnemyArchers { get; set; }

        public List<Unit> MyKnights { get; set; }

        public List<Unit> EnemyKnights { get; set; }

        public List<Unit> MyGiants { get; set; }

        public List<Unit> EnemyGiants { get; set; }

        public List<Tower> MyTowers { get; set; }

        public List<Tower> EnemyTowers { get; set; }

        public List<Barracks> MyBarracks { get; set; }

        public List<Barracks> EnemyBarracks { get; set; }

        public List<Mine> MyMines { get; set; }

        public List<Mine> EnemyMines { get; set; }


        public void ProcessTurn()
        {
            PickCommandForQueen();
        }

        private void PickCommandForQueen()
        {
            var primaryHandlers = new List<ActionsHandler>()
            {
                new BuildStructureHandler(),
                new RunToTowerHandler(),
                new MoveToClosestSiteHandler(),
                new DefaultWaitHandler()
            };

            var trainingHandlers = new List<ActionsHandler>()
            {
                new TrainGiantsHandler(),
                new TrainKnightsHandler(),
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

        public void BuildBarracks(int siteId, UnitType type)
        {
            Console.WriteLine($"{Constants.BuildCommand} {siteId} {Constants.BarracksStructure}-{type.ToString().ToUpper()}");
        }

        public void BuildTower(int siteId)
        {
            Console.WriteLine($"{Constants.BuildCommand} {siteId} {Constants.TowerStructure}");
        }

        public void BuildMine(int siteId)
        {
            Console.WriteLine($"{Constants.BuildCommand} {siteId} {Constants.MineStructure}");
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

    public class RunToTowerHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var queen = context.MyQueen;
            if (context.EnemyKnights.Where(x => MathUtils.GetDistance(x, queen) < Constants.QueenRadius * 4).Count() > 4)
            {
                var closestTower = context.MyTowers.OrderByDescending(x => MathUtils.GetDistance(x, queen)).FirstOrDefault();
                if (closestTower != null)
                {
                    context.Move(closestTower);
                    return true;
                }
            }

            return false;
        }
    }

    internal class BuildStructureHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            if (context.TouchedSiteId != -1 &&
                context.Sites[context.TouchedSiteId].Owner == OwnerType.Neutral)
            {
                if (!context.MyBarracks.Where(x => x.CreepType == CreepType.Knight).Any())
                {
                    context.BuildBarracks(context.TouchedSiteId, UnitType.Knight);
                    return true;
                }
                else
                {
                    if (!context.MyGiants.Any() &&
                        context.EnemyTowers.Any())
                    {
                        context.BuildBarracks(context.TouchedSiteId, UnitType.Giant);
                        return true;
                    }
                    if (context.MyMines.Count < 3 &&
                        !context.EnemyKnights.Any(x => MathUtils.GetDistance(x, context.MyQueen) < Constants.QueenRadius * 2))
                    {
                        context.BuildMine(context.TouchedSiteId);
                        return true;
                    }
                    else
                    {
                        context.BuildTower(context.TouchedSiteId);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    internal class TrainKnightsHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var knightBarracks = context.MyBarracks
                   .Where(x => x.CreepType == CreepType.Knight)
                   .OrderBy(x => MathUtils.GetDistance(x, context.EnemyQueen));

            if (knightBarracks.Any() &&
                context.AvailableGold >= Constants.KnightGroupCost)
            {
                context.Train(new List<int>() { knightBarracks.First().Id });
                context.AvailableGold -= Constants.KnightGroupCost;
                return true;
            }

            return false;
        }
    }

    public class TrainArchersHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var archerBarracks = context.MyBarracks
                   .Where(x => x.CreepType == CreepType.Archer)
                   .OrderBy(x => MathUtils.GetDistance(x, context.MyQueen));

            if (archerBarracks.Any() &&
                context.EnemyKnights.Count / Constants.KnightGroupCount > 1 &&
                context.MyArchers.Count == 0 &&
                context.AvailableGold >= Constants.ArcherGroupCost)
            {
                context.Train(new List<int>() { archerBarracks.First().Id });
                return true;
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
            if (closestSite != null)
            {
                context.Move(closestSite);
                return true;
            }

            return false;
        }
    }

    public class TrainGiantsHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var giantBarracks = context.MyBarracks
                .Where(x => x.CreepType == CreepType.Giant)
                .OrderBy(x => MathUtils.GetDistance(x, context.EnemyQueen));

            if (giantBarracks.Any() &&
                context.EnemyTowers.Count > 0 &&
                context.MyGiants.Count == 0 &&
                context.AvailableGold >= Constants.GiantGroupCost)
            {
                context.Train(new List<int>() { giantBarracks.First().Id });
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
        public int RemainingGold { get; set; }
        public int MaximumGoldRate { get; set; }
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

    public class Tower : Site
    {
        public int Health { get; set; }

        public int AttackRadius { get; set; }
    }

    public class Mine : Site
    {
        public int CurrentGoldRate { get; set; }
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
        Archer = 1,
        Giant = 2
    }

    public enum UnitType
    {
        Queen = -1,
        Knight = 0,
        Archer = 1,
        Giant = 2
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
