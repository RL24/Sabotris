namespace Sabotris.Translations.Locales
{
    public class LocaleSpanish : Locale
    {
        public LocaleSpanish() : base("Spanish")
        {
            // Generic menu items	
	
            _translations.Add(TranslationKey.UiMenuButtonBack, "Regreso");
            _translations.Add(TranslationKey.UiMenuButtonApply, "Salvar");
            _translations.Add(TranslationKey.UiYes, "sí");
            _translations.Add(TranslationKey.UiNo, "No");

            // Main menu	

            _translations.Add(TranslationKey.UiMenuButtonHostGame, "Empezar Juego");
            _translations.Add(TranslationKey.UiMenuButtonJoinGame, "Únete a un Juego");
            _translations.Add(TranslationKey.UiMenuButtonSettings, "Ajustes");
            _translations.Add(TranslationKey.UiMenuButtonExit, "Salir del Juego");

            // Host game	

            _translations.Add(TranslationKey.UiMenuTitleHostGame, "Empezar Juego");
            _translations.Add(TranslationKey.UiMenuInputPlayFieldSize, "Tamaño de Contenedor");
            _translations.Add(TranslationKey.UiMenuInputMaxPlayers, "Jugadores Máximos");
            _translations.Add(TranslationKey.UiMenuInputBlocksPerShape, "Tamaño de la Forma");
            _translations.Add(TranslationKey.UiMenuInputGenerateVerticalBlocks, "Formas Verticales");
            _translations.Add(TranslationKey.UiMenuInputPracticeMode, "Práctica");
            _translations.Add(TranslationKey.UiMenuButtonCreateLobby, "Crear Lobby");

            // Join Game	

            _translations.Add(TranslationKey.UiMenuTitleJoinGame, "Únete a un Juego");
            _translations.Add(TranslationKey.UiMenuButtonRefresh, "Actualizar Lista");
            _translations.Add(TranslationKey.UiMenuNoticeRefreshing, "Refrescante...");
            _translations.Add(TranslationKey.UiMenuNoticeNoLobbies, "No se Encontraron los Lobbies");
            _translations.Add(TranslationKey.UiMenuLobbyItemPlayerCount, "{0}/{1} Jugadores");

            // Lobby	

            _translations.Add(TranslationKey.UiMenuTitleLobby, "Vestíbulo");
            _translations.Add(TranslationKey.UiMenuInputEnterText, "Ingrese Texto...");
            _translations.Add(TranslationKey.UiMenuButtonStartGame, "Empezar Juego");
            _translations.Add(TranslationKey.UiMenuDisplayPlayFieldSize, "Tamaño del Contenedor: {0}x{0}");
            _translations.Add(TranslationKey.UiMenuDisplayMaxPlayers, "Jugadores Máximos: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBlocksPerShape, "Tamaño de la Forma: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, "Bloques Verticales: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPracticeMode, "Práctica: {0}");

            // Settings	

            _translations.Add(TranslationKey.UiMenuTitleSettings, "Ajustes");
            _translations.Add(TranslationKey.UiMenuButtonVideo, "Video");
            _translations.Add(TranslationKey.UiMenuButtonAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuButtonControls, "Aporte");
            _translations.Add(TranslationKey.UiMenuButtonGameplay, "Configuración de Juegos");

            // Settings Video	

            _translations.Add(TranslationKey.UiMenuTitleSettingsVideo, "Video");
            _translations.Add(TranslationKey.UiMenuInputAmbientOcclusion, "Oclusión Ambiental");
            _translations.Add(TranslationKey.UiMenuInputMenuDof, "DOF de Menú");
            _translations.Add(TranslationKey.UiMenuInputMenuDofOff, "Apagado");
            _translations.Add(TranslationKey.UiMenuInputMenuDofGaussian, "Gaussiano");
            _translations.Add(TranslationKey.UiMenuInputMenuDofBokeh, "Bokeh");
            _translations.Add(TranslationKey.UiMenuInputFullscreenMode, "Modo de Pantalla Completa");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreenWindowed, "Ventana de Pantalla Completa");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreen, "Pantalla Completa");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeWindowed, "Ventana");

            // Settings Audio	

            _translations.Add(TranslationKey.UiMenuTitleSettingsAudio, "Audio");
            _translations.Add(TranslationKey.UiMenuInputMasterVolume, "Volumen Principal");
            _translations.Add(TranslationKey.UiMenuInputMusicVolume, "Volumen de la Música");
            _translations.Add(TranslationKey.UiMenuInputUiVolume, "Volumen de UI");
            _translations.Add(TranslationKey.UiMenuInputGameVolume, "Volumen de Juego");

            // Settings Controls	

            _translations.Add(TranslationKey.UiMenuTitleSettingsControls, "Aporte");
            _translations.Add(TranslationKey.UiMenuButtonInputBindings, "Configuración de Entrada (próximamente)");
            _translations.Add(TranslationKey.UiMenuHeadingGamepad, "Gamepad");
            _translations.Add(TranslationKey.UiMenuHeadingMouse, "Ratón");
            _translations.Add(TranslationKey.UiMenuInputCameraSensitivity, "Sensibilidad de la Cámara");
            _translations.Add(TranslationKey.UiMenuInputRotateSensitivity, "Sensibilidad Giratoria");

            // Settings Gameplay	

            _translations.Add(TranslationKey.UiMenuTitleSettingsGameplay, "Configuración de Juegos");
            _translations.Add(TranslationKey.UiMenuHeadingGameSpeeds, "Velocidades de Juego");
            _translations.Add(TranslationKey.UiMenuHeadingMenuSpeeds, "Velocidades del Menú");
            _translations.Add(TranslationKey.UiMenuInputMoveAnimation, "Traducir Animación");
            _translations.Add(TranslationKey.UiMenuInputCameraAnimation, "Animación de Cámara");
            _translations.Add(TranslationKey.UiMenuInputUiAnimation, "Animación de UI");
            _translations.Add(TranslationKey.UiMenuInputLanguage, "Idioma");
            _translations.Add(TranslationKey.UiMenuLanguageEnglish, "Inglés");
            _translations.Add(TranslationKey.UiMenuLanguageFrench, "Francés");
            _translations.Add(TranslationKey.UiMenuLanguageGerman, "Alemán");
            _translations.Add(TranslationKey.UiMenuLanguageItalian, "Italiano");
            _translations.Add(TranslationKey.UiMenuLanguageSpanish, "Español");

            // Pause	

            _translations.Add(TranslationKey.UiMenuTitlePause, "Pausa");
            _translations.Add(TranslationKey.UiMenuButtonDisconnect, "Desconectar");
            _translations.Add(TranslationKey.UiMenuButtonResume, "Reanudar");

            // Game Over	

            _translations.Add(TranslationKey.UiMenuTitleGameOver, "Juego Terminado");
            _translations.Add(TranslationKey.UiMenuButtonMainMenu, "Menú Principal");

            // HUD	

            _translations.Add(TranslationKey.UiHudPlayerLabel, "Jugador");
            _translations.Add(TranslationKey.UiHudScoreLabel, "Puntaje");

            // Game	

            _translations.Add(TranslationKey.GameContainerDropSpeed, "Velocidad de Gota: {0}");
        }
    }
}