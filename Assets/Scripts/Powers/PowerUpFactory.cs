using System;
using System.Collections.Generic;
using Sabotris.Powers.PowerUps;
using Random = Sabotris.Util.Random;

namespace Sabotris.Powers
{
    public static class PowerUpFactory
    {
        private static readonly Dictionary<Power, Func<PowerUp>> PowerCache = new Dictionary<Power, Func<PowerUp>>()
        {
            {Power.RandomBlock, () => new PowerUpRandomBlock()},
            {Power.ClearLayer, () => new PowerUpClearLayer()},
            {Power.AddLayer, () => new PowerUpAddLayer()},
            // {Power.AdjacentExplode, () => new PowerUpAdjacentExplode()},
            // {Power.XExplode, () => new PowerUpXExplode()},
            // {Power.YExplode, () => new PowerUpYExplode()},
            // {Power.ZExplode, () => new PowerUpZExplode()},
        };

        public static PowerUp CreatePowerUp(Power power)
        {
            if (power == Power.None && Random.Boolean())
                power = (Power) Random.Range(0, (int) Power.Count);

            // power = Power.AddLayer;
            PowerCache.TryGetValue(power, out var powerUp);
            return powerUp?.Invoke();
        }
    }
}