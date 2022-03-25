namespace Sabotris.Translations.Locales
{
    public class LocaleEnglish : Locale
    {
        public LocaleEnglish() : base("English")
        {
            // Generic menu items
            
            _translations.Add(TranslationKey.UiMenuButtonBack, "Back");
            _translations.Add(TranslationKey.UiMenuButtonApply, "Save");
            _translations.Add(TranslationKey.UiYes, "Yes");
            _translations.Add(TranslationKey.UiNo, "No");
            
            // Main menu
            
            _translations.Add(TranslationKey.UiMenuButtonHostGame, "Host Game");
            _translations.Add(TranslationKey.UiMenuButtonJoinGame, "Join Game");
            _translations.Add(TranslationKey.UiMenuButtonSettings, "Settings");
            _translations.Add(TranslationKey.UiMenuButtonExit, "Exit");
            
            // Host game

            _translations.Add(TranslationKey.UiMenuTitleHostGame, "Host Game");
            _translations.Add(TranslationKey.UiMenuInputPlayFieldSize, "Container Size");
            _translations.Add(TranslationKey.UiMenuInputMaxPlayers, "Max Players");
            _translations.Add(TranslationKey.UiMenuInputBlocksPerShape, "Shape Size");
            _translations.Add(TranslationKey.UiMenuInputGenerateVerticalBlocks, "Vertical Shapes");
            _translations.Add(TranslationKey.UiMenuInputPracticeMode, "Practice");
            _translations.Add(TranslationKey.UiMenuButtonCreateLobby, "Create Lobby");
            
            // Join Game

            _translations.Add(TranslationKey.UiMenuTitleJoinGame, "Join Game");
            _translations.Add(TranslationKey.UiMenuButtonRefresh, "Refresh");
            _translations.Add(TranslationKey.UiMenuNoticeRefreshing, "Refreshing...");
            _translations.Add(TranslationKey.UiMenuNoticeNoLobbies, "No lobbies found");
            _translations.Add(TranslationKey.UiMenuLobbyItemPlayerCount, "{0}/{1} Players");
            
            // Lobby
            
            _translations.Add(TranslationKey.UiMenuTitleLobby, "Lobby");
            _translations.Add(TranslationKey.UiMenuInputEnterText, "Enter Text...");
            _translations.Add(TranslationKey.UiMenuButtonStartGame, "Start Game");
            _translations.Add(TranslationKey.UiMenuDisplayPlayFieldSize, "Container Size: {0}x{0}");
            _translations.Add(TranslationKey.UiMenuDisplayMaxPlayers, "Max Players: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBlocksPerShape, "Shape Size: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, "Vertical Shapes: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPracticeMode, "Practice: {0}");

            // Settings
            
            _translations.Add(TranslationKey.UiMenuTitleSettings, "Settings");
            _translations.Add(TranslationKey.UiMenuButtonVideo, "Video");
            _translations.Add(TranslationKey.UiMenuButtonAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuButtonControls, "Controls");
            _translations.Add(TranslationKey.UiMenuButtonGameplay, "Gameplay");
            
            // Settings Video
            
            _translations.Add(TranslationKey.UiMenuTitleSettingsVideo, "Video");
            _translations.Add(TranslationKey.UiMenuInputAmbientOcclusion, "Ambient Occlusion");
            _translations.Add(TranslationKey.UiMenuInputMenuDof, "Menu Depth of Field");
            _translations.Add(TranslationKey.UiMenuInputMenuDofOff, "Off");
            _translations.Add(TranslationKey.UiMenuInputMenuDofGaussian, "Gaussian");
            _translations.Add(TranslationKey.UiMenuInputMenuDofBokeh, "Bokeh");
            _translations.Add(TranslationKey.UiMenuInputFullscreenMode, "Fullscreen Mode");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreenWindowed, "Fullscreen Windowed");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreen, "Fullscreen");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeWindowed, "Windowed");
            
            // Settings Audio
            
            _translations.Add(TranslationKey.UiMenuTitleSettingsAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuInputMasterVolume, "Master Volume");
            _translations.Add(TranslationKey.UiMenuInputMusicVolume, "Music Volume");
            _translations.Add(TranslationKey.UiMenuInputUiVolume, "UI Volume");
            _translations.Add(TranslationKey.UiMenuInputGameVolume, "Game Volume");
            
            // Settings Controls
            
            _translations.Add(TranslationKey.UiMenuTitleSettingsControls, "Controls");
            _translations.Add(TranslationKey.UiMenuButtonInputBindings, "Input Bindings (Coming Soon)");
            _translations.Add(TranslationKey.UiMenuHeadingGamepad, "Gamepad");
            _translations.Add(TranslationKey.UiMenuHeadingMouse, "Mouse");
            _translations.Add(TranslationKey.UiMenuInputCameraSensitivity, "Camera Sensitivity");
            _translations.Add(TranslationKey.UiMenuInputRotateSensitivity, "Rotate Sensitivity");
            
            // Settings Gameplay
            
            _translations.Add(TranslationKey.UiMenuTitleSettingsGameplay, "Gameplay");
            _translations.Add(TranslationKey.UiMenuHeadingGameSpeeds, "Game Speeds");
            _translations.Add(TranslationKey.UiMenuHeadingMenuSpeeds, "Menu Speeds");
            _translations.Add(TranslationKey.UiMenuInputMoveAnimation, "Move Animation");
            _translations.Add(TranslationKey.UiMenuInputCameraAnimation, "Camera Animation");
            _translations.Add(TranslationKey.UiMenuInputUiAnimation, "UI Animation");
            _translations.Add(TranslationKey.UiMenuInputLanguage, "Language");
            _translations.Add(TranslationKey.UiMenuLanguageEnglish, "English");
            _translations.Add(TranslationKey.UiMenuLanguageFrench, "French");
            _translations.Add(TranslationKey.UiMenuLanguageGerman, "German");
            _translations.Add(TranslationKey.UiMenuLanguageItalian, "Italian");
            _translations.Add(TranslationKey.UiMenuLanguageSpanish, "Spanish");
            
            // Pause
            
            _translations.Add(TranslationKey.UiMenuTitlePause, "Paused");
            _translations.Add(TranslationKey.UiMenuButtonDisconnect, "Disconnect");
            _translations.Add(TranslationKey.UiMenuButtonResume, "Resume");
            
            // Game Over
            
            _translations.Add(TranslationKey.UiMenuTitleGameOver, "Game Over");
            _translations.Add(TranslationKey.UiMenuButtonMainMenu, "Main Menu");
            
            // HUD
            
            _translations.Add(TranslationKey.UiHudPlayerLabel, "Player");
            _translations.Add(TranslationKey.UiHudScoreLabel, "Score");
            
            // Game

            _translations.Add(TranslationKey.GameContainerDropSpeed, "Drop Speed: {0}");
        }
    }
}