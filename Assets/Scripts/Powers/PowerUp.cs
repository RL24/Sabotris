using UnityEngine;

namespace Sabotris.Powers
{
    public class PowerUp
    {
        public Power Power { get; set; }

        public PowerUp()
        {
            Power = (Power) Random.Range(0, 5);
        }
        
        public PowerUp(Power power)
        {
            Power = power;
        }
    }
}