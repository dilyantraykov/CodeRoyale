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
            var queenMaxHealth = 0;
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
                    site.Id = siteId;
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
                            MaximumGoldRate = site.MaximumGoldRate,
                            RemainingGold = site.RemainingGold,
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

                    if (site.Owner == OwnerType.Friendly)
                    {
                        gameContext.MySites.Add(site);
                    }
                    else
                    {
                        gameContext.EnemySites.Add(site);
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

                    if (unit.Owner == OwnerType.Friendly && unit.Type != UnitType.Queen)
                    {
                        gameContext.MyUnits.Add(unit);
                    }
                    else if (unit.Type != UnitType.Queen)
                    {
                        gameContext.EnemyUnits.Add(unit);
                    }

                    switch (unit.Type)
                    {
                        case UnitType.Queen:
                            unit.Speed = Constants.QueenSpeed;
                            if (unit.Owner == OwnerType.Friendly)
                            {
                                gameContext.MyQueen = unit;
                                if (round == 1)
                                {
                                    queenMaxHealth = health;
                                }

                                gameContext.QueenMaxHealth = queenMaxHealth;
                            }
                            else
                            {
                                gameContext.EnemyQueen = unit;
                            }
                            break;
                        case UnitType.Archer:
                            unit.Speed = Constants.ArcherSpeed;
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
                            unit.Speed = Constants.KnightSpeed;
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
                            unit.Speed = Constants.GiantSpeed;
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
            this.MySites = new List<Site>();
            this.EnemySites = new List<Site>();
            this.MyUnits = new List<Unit>();
            this.EnemyUnits = new List<Unit>();
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

        public Site TouchedSite
        {
            get { return this.TouchedSiteId == -1 ? null : this.Sites[this.TouchedSiteId]; }
        }

        public int AvailableGold { get; set; }

        public Dictionary<int, Site> Sites { get; set; }

        public List<Site> MySites { get; set; }

        public List<Site> EnemySites { get; set; }

        public List<Unit> MyUnits { get; set; }

        public List<Unit> EnemyUnits { get; set; }

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
                new BuildTowerWhenEnemyKnightsAreTraining(),
                //new BuildTowerAgainstApproachingKnights(),
                new RunToTowerHandler(),
                new ReinforceTowersHandler(),
                //new ReplaceTowerWithMine(),
                new UpgradeMine(),
                new MoveBarracksCloserToQueen(),
                new BuildStructureHandler(),
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

        public void BuildTower(Site site)
        {
            this.BuildTower(site.Id);
        }

        public void BuildTower(int siteId)
        {
            Console.WriteLine($"{Constants.BuildCommand} {siteId} {Constants.TowerStructure}");
        }

        public void BuildMine(Site site)
        {
            this.BuildMine(site.Id);
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

        public bool AreThereEnemyTowersInRange(Entity entity)
        {
            return this.EnemyTowers.Any(t => t.GetDistanceTo(entity) <= t.AttackRadius);
        }

        public double GetApproximateDamageToQueenBasedOnBarracks()
        {
            var closestEnemyBaracks = this.EnemyBarracks.OrderBy(b => b.GetDistanceTo(this.MyQueen)).FirstOrDefault();
            if (closestEnemyBaracks == null)
            {
                return 0;
            }
            else
            {
                var distance = closestEnemyBaracks.GetDistanceTo(this.MyQueen);
                var healthLeftAtContact = Constants.KnightMaxHealth - distance / Constants.KnightSpeed;
                var approximateDamage = healthLeftAtContact;
                return approximateDamage;
            }

        }
    }

    public class ReplaceTowerWithMine : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var closeTowers = context.MyTowers.Where(x => x.GetDistanceTo(context.MyQueen) < 500 &&
            !context.AreThereEnemyTowersInRange(x));

            if (context.MyMines.Count == 0 && closeTowers.Any())
            {
                context.BuildMine(closeTowers.First());
                return true;
            }

            return false;
        }
    }

    public class BuildTowerAgainstApproachingKnights : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var queen = context.MyQueen;
            var approachingKnights = context.EnemyKnights
                   .Where(x => x.GetDistanceTo(queen) < Constants.KnightSpeed * 10)
                   .OrderBy(k => k.GetDistanceTo(queen));
            var neutralSites = context.Sites.Values.Where(s => s.Owner != OwnerType.Enemy);
            var closeNeutralSite = neutralSites.OrderBy(s => s.GetDistanceTo(queen)).FirstOrDefault();
            var closestKnight = approachingKnights.FirstOrDefault();
            if (closeNeutralSite == null || closestKnight == null)
            {
                return false;
            }

            var isClosestSiteInRightDirection = closeNeutralSite.GetDistanceTo(approachingKnights.First()) > queen.GetDistanceTo(approachingKnights.First());
            var isClosestSiteATower = closeNeutralSite is Tower;
            if (approachingKnights.Count() >= 2 &&
                context.MyTowers.Where(t => t.IsInRadius(approachingKnights.First())).Count() < 2 &&
                isClosestSiteInRightDirection &&
                !isClosestSiteATower)
            {
                context.BuildTower(closeNeutralSite);
                return true;
            }

            return false;
        }
    }

    public class BuildTowerWhenEnemyKnightsAreTraining : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var enemyBaracks = context.EnemyBarracks.Where(b => b.CreepType == CreepType.Knight && b.TrainingCooldown > 0);
            var approximateDamageToQueen = context.GetApproximateDamageToQueenBasedOnBarracks();
            var potentialKnightUnit = new Unit()
            {
                Position = context.MyQueen.Position,
                Type = UnitType.Knight,
                Speed = Constants.KnightSpeed
            };

            var totalDamageDonePerKnight = context.MyTowers.Sum(t => t.CalculateTotalDamageToUnit(potentialKnightUnit));

            Console.Error.WriteLine(approximateDamageToQueen);
            Console.Error.WriteLine(totalDamageDonePerKnight);
            var notEnoughTowers = approximateDamageToQueen > totalDamageDonePerKnight;

            if (enemyBaracks.Count() > 0 && !context.MyTowers.Any())
            {
                var closestSite = context.Sites.Values.Where(s => s.Owner == OwnerType.Neutral)
                                                        .OrderBy(s => s.GetDistanceTo(context.MyQueen))
                                                        .FirstOrDefault();
                if (closestSite != null)
                {
                    context.BuildTower(closestSite);
                    return true;
                }
            }

            if (notEnoughTowers)
            {
                var closestSite = context.Sites.Values.Where(s => !(s is Tower))
                                                        .OrderBy(s => s.GetDistanceTo(context.MyQueen))
                                                        .FirstOrDefault();
                if (closestSite != null)
                {
                    context.BuildTower(closestSite);
                    return true;
                }
            }

            return false;
        }
    }

    public class MoveBarracksCloserToQueen : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var closeNonEnemySites = context.Sites.Values.Where(x => x.Owner != OwnerType.Enemy &&
                                                                x.GetDistanceTo(context.MyQueen) < 500 &&
                                                                !context.AreThereEnemyTowersInRange(x))
                                                                .OrderBy(s => s.GetDistanceTo(context.MyQueen));
            if (closeNonEnemySites.Any())
            {
                foreach (var site in closeNonEnemySites)
                {
                    var knightBarracks = context.MyBarracks.Where(b => b.CreepType == CreepType.Knight);
                    var isSiteCloserThanOtherBarracks = knightBarracks.Any(b => b.GetDistanceTo(context.EnemyQueen) > site.GetDistanceTo(context.EnemyQueen) + 500);
                    if (isSiteCloserThanOtherBarracks && knightBarracks.Count() <= 1)
                    {
                        context.BuildBarracks(site.Id, UnitType.Knight);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class UpgradeMine : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var weakMines = context.MyMines.Where(t => t.CurrentGoldRate < t.MaximumGoldRate);
            if (weakMines.Count() > 0)
            {
                var targetMine = weakMines.OrderBy(t => t.GetDistanceTo(context.MyQueen)).First();
                context.BuildMine(targetMine);
                return true;
            }

            return false;
        }
    }

    public class ReinforceTowersHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var weakTowers = context.MyTowers
                .Where(t => t.Health < Constants.MaxTowerHealth * 0.6 &&
                !context.EnemyTowers.Any(e => e.GetDistanceTo(t) <= e.AttackRadius));
            var neutralSites = context.Sites.Values.Where(x => x.Owner == OwnerType.Neutral);

            if (weakTowers.Count() > 0 &&
                !neutralSites.Any(x => x.GetDistanceTo(context.MyQueen) < Constants.QueenSpeed * 2) &&
                !context.MyUnits.Any(x => x.GetDistanceTo(context.MyQueen) < Constants.QueenRadius))
            {
                var targetTower = weakTowers.OrderBy(t => t.GetDistanceTo(context.MyQueen)).First();
                context.BuildTower(targetTower);
                return true;
            }

            var touchedTower = context.TouchedSite as Tower;
            if (touchedTower != null &&
                touchedTower.Health < Constants.MaxTowerHealth * 0.9)
            {
                context.BuildTower(touchedTower);
                return true;
            }

            return false;
        }
    }

    public class RunToTowerHandler : ActionsHandler
    {
        public override bool CanProcessTurn(GameContext context)
        {
            var queen = context.MyQueen;
            var approachingKnights = context.EnemyKnights
                .Where(x => x.GetDistanceTo(queen) < Constants.KnightRadius * 10)
                .OrderBy(k => k.GetDistanceTo(queen));
            Entity threateningTower = context.EnemyTowers.FirstOrDefault(t => t.GetDistanceTo(queen) <= t.AttackRadius &&
                context.MyUnits.Where(u => u.GetDistanceTo(t) <= t.AttackRadius).Count() <= 2);
            if (approachingKnights.Count() > 0 || threateningTower != null)
            {
                Entity threat = threateningTower ?? approachingKnights.FirstOrDefault();

                var safeSites = context.MySites
                    .Where(x => !context.EnemyTowers.Any(t => t.IsInRadius(x)))
                    .OrderByDescending(s => context.MyTowers.Where(t => t.IsInRadius(s)).Count())
                    .ThenByDescending(s => s.GetDistanceTo(threat));

                var closestSafeSite = safeSites.FirstOrDefault();

                if (closestSafeSite != null)
                {
                    var isGiantNextToTower = context.EnemyGiants.Any(g => context.MyTowers.Any(t => g.GetDistanceTo(t) < Constants.GiantSpeed * 3));
                    if (queen.Health > context.QueenMaxHealth / 4 &&
                        isGiantNextToTower)
                    {
                        return false;
                    }

                    context.BuildTower(closestSafeSite);
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
            if (context.TouchedSite != null)
            {
                var site = context.TouchedSite;
                if (site.Owner == OwnerType.Neutral)
                {
                    if (context.MyMines.Sum(m => m.CurrentGoldRate) < 3 &&
                        !context.EnemyKnights.Any() &&
                        site.RemainingGold > 0)
                    {
                        context.BuildMine(site.Id);
                        return true;
                    }

                    if (context.EnemyKnights.Any() &&
                        !context.MyTowers.Any(t => t.GetDistanceTo(context.MyQueen) < 500))
                    {
                        context.BuildTower(site.Id);
                        return true;
                    }

                    if (!context.MyBarracks.Where(x => x.CreepType == CreepType.Knight).Any())
                    {
                        context.BuildBarracks(site.Id, UnitType.Knight);
                        return true;
                    }

                    if (!context.MyBarracks.Where(x => x.CreepType == CreepType.Giant).Any() &&
                        context.EnemyTowers.Count > context.MyTowers.Count)
                    {
                        context.BuildBarracks(site.Id, UnitType.Giant);
                        return true;
                    }

                    if (!context.EnemyKnights.Any(x => MathUtils.GetDistance(x, context.MyQueen) < Constants.QueenRadius * 2) &&
                        site.RemainingGold > 0)
                    {
                        context.BuildMine(site.Id);
                        return true;
                    }
                    else
                    {
                        context.BuildTower(site.Id);
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
                .Where(x => x.Owner == OwnerType.Neutral &&
                !context.AreThereEnemyTowersInRange(x))
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
                context.MyGiants.Count == 0)
            {
                if (context.AvailableGold < Constants.GiantGroupCost)
                {
                    context.Train(Enumerable.Empty<int>());
                }
                else
                {
                    context.Train(new List<int>() { giantBarracks.First().Id });
                }

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
                Console.Error.WriteLine(this.ToString());
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

        public bool IsInRadius(Entity entity)
        {
            return MathUtils.GetDistance(this, entity) <= this.AttackRadius;
        }

        public double CalculateTotalDamageToUnit(Unit unit)
        {
            var distance = this.GetDistanceTo(unit);
            double damage = 0;

            while (distance > 0)
            {
                damage += this.CalculateDamage(unit, distance);
                distance -= unit.Speed;
            }

            return damage;
        }

        public double CalculateDamage(Unit unit, double distance)
        {
            var damage = Constants.TowerBaseDamageToCreeps;
            if (unit.Type == UnitType.Queen)
            {
                damage = Constants.TowerBaseDamageToQueen;
            }

            if (distance > this.AttackRadius)
            {
                return 0;
            }

            while (distance > 0)
            {
                damage += 1;
                distance -= 200;
            }

            return damage;
        }
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

        public int Speed { get; set; }
    }

    public class Entity
    {
        public Position Position { get; set; }

        public double GetDistanceTo(Entity entity)
        {
            return MathUtils.GetDistance(this, entity);
        }
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