using System.Collections.Generic;
using Sabotris.Translations.Locales;

namespace Sabotris.Translations
{
    public enum LocaleKey
    {
        English,
        French,
        German,
        Italian,
        Spanish
    }

    public static class Localization
    {
        public static LocaleKey CurrentLocale { get; set; } = LocaleKey.English;

        private static readonly Dictionary<LocaleKey, Locale> Translations = new Dictionary<LocaleKey, Locale>()
        {
            {LocaleKey.English, new LocaleEnglish()},
            {LocaleKey.French, new LocaleFrench()},
            {LocaleKey.German, new LocaleGerman()},
            {LocaleKey.Italian, new LocaleItalian()},
            {LocaleKey.Spanish, new LocaleSpanish()},
        };

        public static string Translate(TranslationKey key)
        {
            return Translations[CurrentLocale].Translate(key);
        }

        public static string Translate(TranslationKey key, params object[] args)
        {
            return string.Format(Translations[CurrentLocale].Translate(key), args);
        }
    }
}