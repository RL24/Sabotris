namespace Sabotris.Translations.Locales
{
    public class LocaleItalian : Locale
    {
        public LocaleItalian() : base("Italian")
        {
            // Generic menu items

            _translations.Add(TranslationKey.UiMenuButtonBack, "Ritorno");
            _translations.Add(TranslationKey.UiMenuButtonApply, "Salva");
            _translations.Add(TranslationKey.UiYes, "Sì");
            _translations.Add(TranslationKey.UiNo, "No");

            // Main menu

            _translations.Add(TranslationKey.UiMenuButtonHostGame, "Gioco Ospitante");
            _translations.Add(TranslationKey.UiMenuButtonJoinGame, "Unisciti a un Gioco");
            _translations.Add(TranslationKey.UiMenuButtonSettings, "Impostazioni");
            _translations.Add(TranslationKey.UiMenuButtonExit, "Uscita");

            // Host game

            _translations.Add(TranslationKey.UiMenuTitleHostGame, "Gioco Ospitante");
            _translations.Add(TranslationKey.UiMenuInputBotCount, "Numero di ai");
            _translations.Add(TranslationKey.UiMenuInputBotDifficulty, "Difficoltà di ai");
            _translations.Add(TranslationKey.UiMenuInputPlayFieldSize, "Dimensione del Contenitore");
            _translations.Add(TranslationKey.UiMenuInputMaxPlayers, "Giocatori Massimi");
            _translations.Add(TranslationKey.UiMenuInputBlocksPerShape, "Dimensione della Forma");
            _translations.Add(TranslationKey.UiMenuInputGenerateVerticalBlocks, "Forme Verticali");
            _translations.Add(TranslationKey.UiMenuInputPracticeMode, "Pratica");
            _translations.Add(TranslationKey.UiMenuButtonCreateLobby, "Creare Lobby");

            // Join Game

            _translations.Add(TranslationKey.UiMenuTitleJoinGame, "Unisciti al Gioco");
            _translations.Add(TranslationKey.UiMenuButtonRefresh, "Ricaricare");
            _translations.Add(TranslationKey.UiMenuNoticeRefreshing, "Rinfrescante...");
            _translations.Add(TranslationKey.UiMenuNoticeNoLobbies, "Nessuna Lobby Trovata");
            _translations.Add(TranslationKey.UiMenuLobbyItemPlayerCount, "{0}/{1} Giocatori");

            // Lobby

            _translations.Add(TranslationKey.UiMenuTitleLobby, "Atrio");
            _translations.Add(TranslationKey.UiMenuInputEnterText, "Inserire il Testo...");
            _translations.Add(TranslationKey.UiMenuButtonStartGame, "Inizia il Gioco");
            _translations.Add(TranslationKey.UiMenuDisplayBotCount, "Numero di ai: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBotDifficulty, "Difficoltà di ai: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPlayFieldSize, "Dimensione del Contenitore: {0}x{0}");
            _translations.Add(TranslationKey.UiMenuDisplayMaxPlayers, "Giocatori Massimi: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBlocksPerShape, "Dimensione della Forma: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, "Forme Verticali: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPracticeMode, "Pratica: {0}");

            // Settings

            _translations.Add(TranslationKey.UiMenuTitleSettings, "Impostazioni");
            _translations.Add(TranslationKey.UiMenuButtonVideo, "Video");
            _translations.Add(TranslationKey.UiMenuButtonAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuButtonControls, "Controlli");
            _translations.Add(TranslationKey.UiMenuButtonGameplay, "Gameplay");

            // Settings Video

            _translations.Add(TranslationKey.UiMenuTitleSettingsVideo, "Video");
            _translations.Add(TranslationKey.UiMenuInputAmbientOcclusion, "Occlusione Ambientale");
            _translations.Add(TranslationKey.UiMenuInputMenuDof, "Menu Profondità di Campo");
            _translations.Add(TranslationKey.UiMenuInputMenuDofOff, "Spento");
            _translations.Add(TranslationKey.UiMenuInputMenuDofGaussian, "Gaussiano");
            _translations.Add(TranslationKey.UiMenuInputMenuDofBokeh, "Bokeh");
            _translations.Add(TranslationKey.UiMenuInputFullscreenMode, "Modalità Schermo Intero");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreenWindowed, "Finestra a Schermo Intero");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreen, "A Schermo Intero");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeWindowed, "Finestra");

            // Settings Audio

            _translations.Add(TranslationKey.UiMenuTitleSettingsAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuInputMasterVolume, "Volume Principale");
            _translations.Add(TranslationKey.UiMenuInputMusicVolume, "Volume Musicale");
            _translations.Add(TranslationKey.UiMenuInputUiVolume, "Volume UI");
            _translations.Add(TranslationKey.UiMenuInputGameVolume, "Volume del Gioco");

            // Settings Controls

            _translations.Add(TranslationKey.UiMenuTitleSettingsControls, "Controlli");
            _translations.Add(TranslationKey.UiMenuButtonInputBindings, "Binding Input (in Arrivo)");
            _translations.Add(TranslationKey.UiMenuHeadingGamepad, "Gamepad");
            _translations.Add(TranslationKey.UiMenuHeadingMouse, "Topo");
            _translations.Add(TranslationKey.UiMenuInputCameraSensitivity, "Sensibilità della Fotocamera");
            _translations.Add(TranslationKey.UiMenuInputRotateSensitivity, "Ruota la Sensibilità");

            // Settings Gameplay

            _translations.Add(TranslationKey.UiMenuTitleSettingsGameplay, "Gameplay");
            _translations.Add(TranslationKey.UiMenuHeadingGameSpeeds, "Velocità di Gioco");
            _translations.Add(TranslationKey.UiMenuHeadingMenuSpeeds, "Velocità del Menu");
            _translations.Add(TranslationKey.UiMenuInputMoveAnimation, "Traduci l'Animazione");
            _translations.Add(TranslationKey.UiMenuInputCameraAnimation, "Animazione della Fotocamera");
            _translations.Add(TranslationKey.UiMenuInputUiAnimation, "Animazione UI");
            _translations.Add(TranslationKey.UiMenuInputLanguage, "Lingua");
            _translations.Add(TranslationKey.UiMenuLanguageEnglish, "Inglese");
            _translations.Add(TranslationKey.UiMenuLanguageFrench, "Francese");
            _translations.Add(TranslationKey.UiMenuLanguageGerman, "Tedesco");
            _translations.Add(TranslationKey.UiMenuLanguageItalian, "Italiano");
            _translations.Add(TranslationKey.UiMenuLanguageSpanish, "Spagnolo");

            // Pause

            _translations.Add(TranslationKey.UiMenuTitlePause, "In Pausa");
            _translations.Add(TranslationKey.UiMenuButtonDisconnect, "Disconnessione");
            _translations.Add(TranslationKey.UiMenuButtonResume, "Riprendere");

            // Game Over

            _translations.Add(TranslationKey.UiMenuTitleGameOver, "Game Over");
            _translations.Add(TranslationKey.UiMenuButtonMainMenu, "Menu Principale");

            // HUD

            _translations.Add(TranslationKey.UiHudPlayerLabel, "Giocatore");
            _translations.Add(TranslationKey.UiHudScoreLabel, "Punto");

            // Game

            _translations.Add(TranslationKey.GameContainerDropSpeed, "Velocità Inferiore: {0}");
        }
    }
}