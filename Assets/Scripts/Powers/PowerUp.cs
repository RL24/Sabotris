using UnityEngine;

namespace Sabotris.Powers
{
    public class PowerUp
    {
        private Power _power;

        public PowerUp()
        {
            _power = (Power) Random.Range(0, 5);
        }
        
        public PowerUp(Power power)
        {
            _power = power;
        }
    }
}