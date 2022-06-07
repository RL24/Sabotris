using System.Collections.Generic;

namespace Sabotris.Translations
{
    public enum TranslationKey
    {
        GENERIC = 0x00,
        UiMenuButtonBack = 0x01,
        UiMenuButtonApply = 0x02,
        UiYes = 0x03,
        UiNo = 0x04,

        MAIN_MENU = 0x10,
        UiMenuButtonHostGame = 0x11,
        UiMenuButtonJoinGame = 0x12,
        UiMenuButtonSettings = 0x13,
        UiMenuButtonExit = 0x14,

        HOST_GAME = 0x20,
        UiMenuTitleHostGame = 0x21,
        UiMenuInputPlayFieldSize = 0x22,
        UiMenuInputMaxPlayers = 0x23,
        UiMenuInputBlocksPerShape = 0x24,
        UiMenuInputGenerateVerticalBlocks = 0x25,
        UiMenuInputPracticeMode = 0x26,
        UiMenuButtonCreateLobby = 0x27,
        UiMenuInputBotCount = 0x28,
        UiMenuInputBotDifficulty = 0x29,
        UiMenuInputPowerUps = 0x2A,
        UiMenuInputPowerUpAutoPickDelay = 0x2B,

        JOIN_GAME = 0x30,
        UiMenuTitleJoinGame = 0x31,
        UiMenuButtonRefresh = 0x32,
        UiMenuNoticeRefreshing = 0x301,
        UiMenuNoticeNoLobbies = 0x302,
        UiMenuLobbyItemPlayerCount = 0x33,

        LOBBY = 0x310,
        UiMenuTitleLobby = 0x311,
        UiMenuInputEnterText = 0x312,
        UiMenuButtonStartGame = 0x313,
        UiMenuDisplayPlayFieldSize = 0x314,
        UiMenuDisplayMaxPlayers = 0x315,
        UiMenuDisplayBlocksPerShape = 0x316,
        UiMenuDisplayGenerateVerticalBlocks = 0x317,
        UiMenuDisplayPracticeMode = 0x318,
        UiMenuDisplayBotCount = 0x319,
        UiMenuDisplayBotDifficulty = 0x320,
        UiMenuDisplayPowerUps = 0x321,
        UiMenuDisplayPowerUpAutoPickDelay = 0x322,

        SETTINGS = 0x40,
        UiMenuTitleSettings = 0x41,
        UiMenuButtonVideo = 0x42,
        UiMenuButtonAudio = 0x43,
        UiMenuButtonControls = 0x44,
        UiMenuButtonGameplay = 0x45,

        SETTINGS_VIDEO = 0x50,
        UiMenuTitleSettingsVideo = 0x51,
        UiMenuInputAmbientOcclusion = 0x52,
        UiMenuInputMenuDof = 0x53,
        UiMenuInputMenuDofOff = 0x530,
        UiMenuInputMenuDofGaussian = 0x531,
        UiMenuInputMenuDofBokeh = 0x532,
        UiMenuInputFullscreenMode = 0x54,
        UiMenuInputFullscreenModeFullscreenWindowed = 0x540,
        UiMenuInputFullscreenModeFullscreen = 0x541,
        UiMenuInputFullscreenModeWindowed = 0x542,

        SETTINGS_AUDIO = 0x60,
        UiMenuTitleSettingsAudio = 0x61,
        UiMenuInputMasterVolume = 0x62,
        UiMenuInputMusicVolume = 0x63,
        UiMenuInputUiVolume = 0x64,
        UiMenuInputGameVolume = 0x65,

        SETTINGS_CONTROLS = 0x70,
        UiMenuTitleSettingsControls = 0x71,
        UiMenuButtonInputBindings = 0x72,
        UiMenuHeadingGamepad = 0x73,
        UiMenuHeadingMouse = 0x74,
        UiMenuInputCameraSensitivity = 0x75,
        UiMenuInputRotateSensitivity = 0x76,

        SETTINGS_GAMEPLAY = 0x80,
        UiMenuTitleSettingsGameplay = 0x81,
        UiMenuHeadingGameSpeeds = 0x82,
        UiMenuHeadingMenuSpeeds = 0x83,
        UiMenuInputMoveAnimation = 0x84,
        UiMenuInputCameraAnimation = 0x85,
        UiMenuInputUiAnimation = 0x86,
        UiMenuInputLanguage = 0x87,
        UiMenuLanguageEnglish = 0x870,
        UiMenuLanguageFrench = 0x871,
        UiMenuLanguageGerman = 0x872,
        UiMenuLanguageItalian = 0x873,
        UiMenuLanguageSpanish = 0x874,

        PAUSE = 0x90,
        UiMenuTitlePause = 0x91,
        UiMenuButtonDisconnect = 0x92,
        UiMenuButtonResume = 0x93,

        GAME_OVER = 0xA0,
        UiMenuTitleGameOver = 0xA1,
        UiMenuButtonMainMenu = 0xA2,

        HUD = 0xB0,
        UiHudPlayerLabel = 0xB1,
        UiHudScoreLabel = 0xB2,
        UiHudPowerUpLabel = 0xB3,
        UiHudSelectContainerLabel = 0xB4,
        UiHudSelectContainerSubLabel = 0xB5,
        UiHudSelectContainerTimerLabel = 0xB6,

        GAME = 0xC0,
        GameContainerDropSpeed = 0xC1,
        
        POWER_UPS = 0xD0,
        PowerUpRandomBlock = 0xD1,
        PowerUpClearLayer = 0xD2,
        PowerUpAddLayer = 0xD3,
        PowerUpRandomShape = 0xD4
    }

    public abstract class Locale
    {
        protected Dictionary<TranslationKey, string> _translations = new Dictionary<TranslationKey, string>();

        public string Translate(TranslationKey key)
        {
            return !_translations.TryGetValue(key, out var val) ? key.ToString() : val;
        }
    }
}