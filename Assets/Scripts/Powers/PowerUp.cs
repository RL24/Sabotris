using System;
using System.Collections;
using Sabotris.Translations;
using Sabotris.Worlds;

namespace Sabotris.Powers
{
    public abstract class PowerUp
    {
        protected Container ActivatingContainer, SelectingContainer;
        
        public abstract Power GetPower();

        public void Use(Container activatingContainer)
        {
            activatingContainer.cameraController.SetSelectingContainer(this, OnSelectedContainer, activatingContainer, new[] {activatingContainer});
        }

        protected abstract IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer);

        public virtual void Update()
        {
        }

        public override string ToString()
        {
            Enum.TryParse<TranslationKey>($"PowerUp{GetPower().ToString()}", out var translationKey);
            return Localization.Translate(translationKey);
        }
    }
}