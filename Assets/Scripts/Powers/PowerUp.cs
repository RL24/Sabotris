using System;
using Sabotris.Translations;
using Sabotris.Worlds;

namespace Sabotris.Powers
{
    public abstract class PowerUp
    {
        public abstract Power GetPower();

        public abstract void Use(Container activatingContainer);

        public override string ToString()
        {
            Enum.TryParse<TranslationKey>($"PowerUp{GetPower().ToString()}", out var translationKey);
            return Localization.Translate(translationKey);
        }
    }
}