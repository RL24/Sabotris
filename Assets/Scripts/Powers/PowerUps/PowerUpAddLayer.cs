using System.Collections;
using UnityEngine;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpAddLayer : PowerUp
    {
        public override Power GetPower()
        {
            return Power.AddLayer;
        }

        public override void Use(Container activatingContainer)
        {
            activatingContainer.cameraController.SetSelectingContainer(OnSelectedContainer, activatingContainer, new[] {activatingContainer});
        }

        private IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            yield return new WaitForSeconds(1);
        }
    }
}