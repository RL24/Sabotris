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
            {Power.RandomShape, () => new PowerUpRandomShape()},
            {Power.ClearLayer, () => new PowerUpClearLayer()},
            {Power.AddLayer, () => new PowerUpAddLayer()}
        };

        public static PowerUp CreatePowerUp(Power? power)
        {
            if (power == null && Random.Boolean())
                power = (Power) Random.Range(0, (int) Power.Count - 1);

            PowerCache.TryGetValue(power ?? Power.None, out var powerUp);
            return powerUp?.Invoke();
        }
    }
}