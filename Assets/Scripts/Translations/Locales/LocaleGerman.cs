namespace Sabotris.Translations.Locales
{
    public class LocaleGerman : Locale
    {
        public LocaleGerman() : base("German")
        {
            // Generic menu items

            _translations.Add(TranslationKey.UiMenuButtonBack, "Zurück");
            _translations.Add(TranslationKey.UiMenuButtonApply, "Annehmen");
            _translations.Add(TranslationKey.UiYes, "Jawohl");
            _translations.Add(TranslationKey.UiNo, "Nein");

            // Main menu

            _translations.Add(TranslationKey.UiMenuButtonHostGame, "Neues Spiel");
            _translations.Add(TranslationKey.UiMenuButtonJoinGame, "Spiel Beitreten");
            _translations.Add(TranslationKey.UiMenuButtonSettings, "Einstellungen");
            _translations.Add(TranslationKey.UiMenuButtonExit, "Ausgang");

            // Host game

            _translations.Add(TranslationKey.UiMenuTitleHostGame, "Neues Spiel");
            _translations.Add(TranslationKey.UiMenuInputBotCount, "Anzahl der AI");
            _translations.Add(TranslationKey.UiMenuInputBotDifficulty, "AI Schwierigkeitsgrad");
            _translations.Add(TranslationKey.UiMenuInputPlayFieldSize, "Behältergröße");
            _translations.Add(TranslationKey.UiMenuInputMaxPlayers, "Maximale Spieleranzahl");
            _translations.Add(TranslationKey.UiMenuInputBlocksPerShape, "Formgröße.");
            _translations.Add(TranslationKey.UiMenuInputGenerateVerticalBlocks, "Vertikale Formen");
            _translations.Add(TranslationKey.UiMenuInputPracticeMode, "Üben");
            _translations.Add(TranslationKey.UiMenuButtonCreateLobby, "Spiel Erstellen");

            // Join Game

            _translations.Add(TranslationKey.UiMenuTitleJoinGame, "Spiel Beitreten");
            _translations.Add(TranslationKey.UiMenuButtonRefresh, "Neu Laden");
            _translations.Add(TranslationKey.UiMenuNoticeRefreshing, "Erfrischend...");
            _translations.Add(TranslationKey.UiMenuNoticeNoLobbies, "Keine Lobbys Gefunden.");
            _translations.Add(TranslationKey.UiMenuLobbyItemPlayerCount, "{0}/{1} Spieler");

            // Lobby

            _translations.Add(TranslationKey.UiMenuTitleLobby, "Empfangshalle");
            _translations.Add(TranslationKey.UiMenuInputEnterText, "Text Eingeben...");
            _translations.Add(TranslationKey.UiMenuButtonStartGame, "Spiel Starten");
            _translations.Add(TranslationKey.UiMenuDisplayBotCount, "Anzahl der AI: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBotDifficulty, "AI Schwierigkeitsgrad: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPlayFieldSize, "Behältergröße: {0}x{0}");
            _translations.Add(TranslationKey.UiMenuDisplayMaxPlayers, "Max-Spieler: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBlocksPerShape, "Formgröße: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, "Vertikale Blöcke: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPracticeMode, "Üben: {0}");

            // Settings

            _translations.Add(TranslationKey.UiMenuTitleSettings, "Einstellungen");
            _translations.Add(TranslationKey.UiMenuButtonVideo, "Video");
            _translations.Add(TranslationKey.UiMenuButtonAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuButtonControls, "Steuerelemente");
            _translations.Add(TranslationKey.UiMenuButtonGameplay, "Spielkonfiguration");

            // Settings Video

            _translations.Add(TranslationKey.UiMenuTitleSettingsVideo, "Video");
            _translations.Add(TranslationKey.UiMenuInputAmbientOcclusion, "Umgebungsakklusion");
            _translations.Add(TranslationKey.UiMenuInputMenuDof, "Menü DOF");
            _translations.Add(TranslationKey.UiMenuInputMenuDofOff, "Aus");
            _translations.Add(TranslationKey.UiMenuInputMenuDofGaussian, "Gaußsche");
            _translations.Add(TranslationKey.UiMenuInputMenuDofBokeh, "Bokeh");
            _translations.Add(TranslationKey.UiMenuInputFullscreenMode, "Vollbildmodus");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreenWindowed, "Fenster Vollbild");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreen, "Vollbild");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeWindowed, "Fenster");

            // Settings Audio

            _translations.Add(TranslationKey.UiMenuTitleSettingsAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuInputMasterVolume, "Hauptvolumen");
            _translations.Add(TranslationKey.UiMenuInputMusicVolume, "Musiklautstärke");
            _translations.Add(TranslationKey.UiMenuInputUiVolume, "UI Volumen");
            _translations.Add(TranslationKey.UiMenuInputGameVolume, "Spielvolumen");

            // Settings Controls

            _translations.Add(TranslationKey.UiMenuTitleSettingsControls, "Steuerelemente");
            _translations.Add(TranslationKey.UiMenuButtonInputBindings, "Bindungen (in Kürze)");
            _translations.Add(TranslationKey.UiMenuHeadingGamepad, "Gamepad");
            _translations.Add(TranslationKey.UiMenuHeadingMouse, "Maus");
            _translations.Add(TranslationKey.UiMenuInputCameraSensitivity, "Kameraempfindlichkeit");
            _translations.Add(TranslationKey.UiMenuInputRotateSensitivity, "Rotierende Empfindlichkeit");

            // Settings Gameplay

            _translations.Add(TranslationKey.UiMenuTitleSettingsGameplay, "Spielkonfiguration");
            _translations.Add(TranslationKey.UiMenuHeadingGameSpeeds, "Spielgeschwindigkeiten");
            _translations.Add(TranslationKey.UiMenuHeadingMenuSpeeds, "Menügeschwindigkeiten");
            _translations.Add(TranslationKey.UiMenuInputMoveAnimation, "Animation übersetzen");
            _translations.Add(TranslationKey.UiMenuInputCameraAnimation, "Kameraanimation");
            _translations.Add(TranslationKey.UiMenuInputUiAnimation, "UI-Animation");
            _translations.Add(TranslationKey.UiMenuInputLanguage, "Sprache");
            _translations.Add(TranslationKey.UiMenuLanguageEnglish, "Englisch");
            _translations.Add(TranslationKey.UiMenuLanguageFrench, "Französisch");
            _translations.Add(TranslationKey.UiMenuLanguageGerman, "Deutsch");
            _translations.Add(TranslationKey.UiMenuLanguageItalian, "Italienisch");
            _translations.Add(TranslationKey.UiMenuLanguageSpanish, "Spanisch");

            // Pause

            _translations.Add(TranslationKey.UiMenuTitlePause, "Pause");
            _translations.Add(TranslationKey.UiMenuButtonDisconnect, "Verlassen");
            _translations.Add(TranslationKey.UiMenuButtonResume, "Fortsetzen");

            // Game Over

            _translations.Add(TranslationKey.UiMenuTitleGameOver, "Spiel ist Aus");
            _translations.Add(TranslationKey.UiMenuButtonMainMenu, "Hauptmenü");

            // HUD

            _translations.Add(TranslationKey.UiHudPlayerLabel, "Spieler");
            _translations.Add(TranslationKey.UiHudScoreLabel, "Punkte");
            _translations.Add(TranslationKey.UiHudPowerUpLabel, "Kräfte");
            _translations.Add(TranslationKey.UiHudSelectContainerLabel, "Spieler Auswählen");
            _translations.Add(TranslationKey.UiHudSelectContainerSubLabel, "Sabotage, Fähigkeit: {0}");
            _translations.Add(TranslationKey.UiHudSelectContainerTimerLabel, "Verbleibende Zeit: {0}s");

            // Game

            _translations.Add(TranslationKey.GameContainerDropSpeed, "Tropfengeschwindigkeit: {0}");
            
            // Power Up

            _translations.Add(TranslationKey.PowerUpRandomBlock, "Zufallsblock");
            _translations.Add(TranslationKey.PowerUpClearLayer, "Klare Schicht");
            _translations.Add(TranslationKey.PowerUpAddLayer, "Schicht Hinzufügen");
        }
    }
}