using System;
using System.Collections.Generic;

namespace LottoCalc
{
    public enum PoolType
    {
        Combined,
        Separate
    }

    public abstract class Pool
    {
        protected Pool(PoolType type, int pick, int min, int max)
        {
            Type = type;
            Pick = pick;
            Min = min;
            Max = max;
        }

        public PoolType Type { get; private set; }
        public int Pick { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
    }

    public class CombinedPool : Pool
    {
        public CombinedPool(int pick, int min, int max) : base(PoolType.Combined, pick, min, max)
        {
        }
    }

    public class SeparatePool : Pool
    {
        public SeparatePool(int pick, int min, int max) : base(PoolType.Separate, pick, min, max)
        {
        }
    }

    public class Game
    {
        public Game(string name, Pool poolPrincipal, Pool poolSecondary)
        {
            Name = name;
            PoolPrincipal = poolPrincipal;
            PoolSecondary = poolSecondary;
        }

        public Game(string name, Pool poolPrincipal) : this(name, poolPrincipal, null)
        {
        }

        public string Name { get; set; }
        public Pool PoolPrincipal { get; set; }
        public Pool PoolSecondary { get; set; }
    }

    public class Result
    {
        public int[] Principal { get; set; }
        public int[] Secondary { get; set; }
    }

    public static class Calculation
    {
        public static Result Execute(Game game)
        {
            var random = new Random();

            return new Result
            {
                Principal = GetValues(random, game.PoolPrincipal),
                Secondary = GetValues(random, game.PoolSecondary)
            };
        }

        private static int[] GetValues(Random random, Pool pool)
        {
            if (pool == null)
            {
                return new int[0];
            }

            var poolCount = pool.Max - pool.Min + 1;
            var pickCount = pool.Pick;
            var poolItems = new List<int>(poolCount);
            var pickItems = new List<int>(pickCount);

            for (int i = 0; i < poolCount; i++)
            {
                poolItems.Add(i + pool.Min);
            }

            for (int i = 0; i < pickCount; i++)
            {
                var index = random.Next(poolItems.Count);
                var value = poolItems[index];
                pickItems.Add(value);

                if (pool.Type == PoolType.Combined)
                {
                    poolItems.RemoveAt(index);
                }
            }

            return pickItems.ToArray();
        }
    }
}
