namespace Sabotris.Translations.Locales
{
    public class LocaleFrench : Locale
    {
        public LocaleFrench() : base("French")
        {
            // Generic menu items

            _translations.Add(TranslationKey.UiMenuButtonBack, "Arrière");
            _translations.Add(TranslationKey.UiMenuButtonApply, "Enregistrer les Paramètres");
            _translations.Add(TranslationKey.UiYes, "Oui");
            _translations.Add(TranslationKey.UiNo, "Non");

            // Main menu

            _translations.Add(TranslationKey.UiMenuButtonHostGame, "Jeu d'hôte");
            _translations.Add(TranslationKey.UiMenuButtonJoinGame, "Rejoindre un Jeu");
            _translations.Add(TranslationKey.UiMenuButtonSettings, "Réglages");
            _translations.Add(TranslationKey.UiMenuButtonExit, "Sortir");

            // Host game

            _translations.Add(TranslationKey.UiMenuTitleHostGame, "Jeu d'hôte");
            _translations.Add(TranslationKey.UiMenuInputBotCount, "Nombre d'ai");
            _translations.Add(TranslationKey.UiMenuInputBotDifficulty, "Difficulté AI");
            _translations.Add(TranslationKey.UiMenuInputPlayFieldSize, "Taille du Conteneur");
            _translations.Add(TranslationKey.UiMenuInputMaxPlayers, "Le Maximum de Joueurs");
            _translations.Add(TranslationKey.UiMenuInputBlocksPerShape, "Taille de Forme");
            _translations.Add(TranslationKey.UiMenuInputGenerateVerticalBlocks, "Formes Verticales");
            _translations.Add(TranslationKey.UiMenuInputPracticeMode, "Entraine Toi");
            _translations.Add(TranslationKey.UiMenuButtonCreateLobby, "Créer un Jeu");

            // Join Game

            _translations.Add(TranslationKey.UiMenuTitleJoinGame, "Rejoindre un Jeu");
            _translations.Add(TranslationKey.UiMenuButtonRefresh, "Rafraîchir");
            _translations.Add(TranslationKey.UiMenuNoticeRefreshing, "Rafraîchissant...");
            _translations.Add(TranslationKey.UiMenuNoticeNoLobbies, "Aucun Lobbie Trouvé");
            _translations.Add(TranslationKey.UiMenuLobbyItemPlayerCount, "{0}/{1} Joueurs");

            // Lobby

            _translations.Add(TranslationKey.UiMenuTitleLobby, "Hall");
            _translations.Add(TranslationKey.UiMenuInputEnterText, "Entrez du Texte...");
            _translations.Add(TranslationKey.UiMenuButtonStartGame, "Démarrer Jeu");
            _translations.Add(TranslationKey.UiMenuDisplayBotCount, "Nombre d'ai: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBotDifficulty, "Difficulté AI: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPlayFieldSize, "Taille du Conteneur: {0}x{0}");
            _translations.Add(TranslationKey.UiMenuDisplayMaxPlayers, "Max Joueurs: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayBlocksPerShape, "Taille de la Forme: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, "Blocs Verticaux: {0}");
            _translations.Add(TranslationKey.UiMenuDisplayPracticeMode, "Pratique: {0}");

            // Settings

            _translations.Add(TranslationKey.UiMenuTitleSettings, "Réglages");
            _translations.Add(TranslationKey.UiMenuButtonVideo, "Vidéo");
            _translations.Add(TranslationKey.UiMenuButtonAudio, "l'Audio");
            _translations.Add(TranslationKey.UiMenuButtonControls, "Contrôles");
            _translations.Add(TranslationKey.UiMenuButtonGameplay, "Configuration du Jeu");

            // Settings Video

            _translations.Add(TranslationKey.UiMenuTitleSettingsVideo, "Vidéo");
            _translations.Add(TranslationKey.UiMenuInputAmbientOcclusion, "Occlusion Ambiante");
            _translations.Add(TranslationKey.UiMenuInputMenuDof, "DOF du Menu");
            _translations.Add(TranslationKey.UiMenuInputMenuDofOff, "Désactivé");
            _translations.Add(TranslationKey.UiMenuInputMenuDofGaussian, "Gaussien");
            _translations.Add(TranslationKey.UiMenuInputMenuDofBokeh, "Bokeh");
            _translations.Add(TranslationKey.UiMenuInputFullscreenMode, "Mode Plein écran");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreenWindowed, "Fenêtre Fullscreen");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeFullscreen, "Plein écran");
            _translations.Add(TranslationKey.UiMenuInputFullscreenModeWindowed, "La Fenêtre");

            // Settings Audio

            _translations.Add(TranslationKey.UiMenuTitleSettingsAudio, "l'Audio");
            _translations.Add(TranslationKey.UiMenuInputMasterVolume, "Volume Principal");
            _translations.Add(TranslationKey.UiMenuInputMusicVolume, "Volume de la Musique");
            _translations.Add(TranslationKey.UiMenuInputUiVolume, "UI Volume");
            _translations.Add(TranslationKey.UiMenuInputGameVolume, "Volume de Jeu");

            // Settings Controls

            _translations.Add(TranslationKey.UiMenuTitleSettingsControls, "Contrôles");
            _translations.Add(TranslationKey.UiMenuButtonInputBindings, "Entrée Config (Prochainement)");
            _translations.Add(TranslationKey.UiMenuHeadingGamepad, "Gamepad");
            _translations.Add(TranslationKey.UiMenuHeadingMouse, "Souris");
            _translations.Add(TranslationKey.UiMenuInputCameraSensitivity, "Sensibilité de la Caméra.");
            _translations.Add(TranslationKey.UiMenuInputRotateSensitivity, "Sensibilité en Rotation");

            // Settings Gameplay

            _translations.Add(TranslationKey.UiMenuTitleSettingsGameplay, "Configuration du Jeu");
            _translations.Add(TranslationKey.UiMenuHeadingGameSpeeds, "Vitesses de Jeu");
            _translations.Add(TranslationKey.UiMenuHeadingMenuSpeeds, "Vitesses de Menu");
            _translations.Add(TranslationKey.UiMenuInputMoveAnimation, "Traduire l'Animation");
            _translations.Add(TranslationKey.UiMenuInputCameraAnimation, "Animation de la Caméra");
            _translations.Add(TranslationKey.UiMenuInputUiAnimation, "Animation UI");
            _translations.Add(TranslationKey.UiMenuInputLanguage, "Langue");
            _translations.Add(TranslationKey.UiMenuLanguageEnglish, "Anglais");
            _translations.Add(TranslationKey.UiMenuLanguageFrench, "Français");
            _translations.Add(TranslationKey.UiMenuLanguageGerman, "Allemand");
            _translations.Add(TranslationKey.UiMenuLanguageItalian, "Italien");
            _translations.Add(TranslationKey.UiMenuLanguageSpanish, "Espagnol");

            // Pause

            _translations.Add(TranslationKey.UiMenuTitlePause, "Pause");
            _translations.Add(TranslationKey.UiMenuButtonDisconnect, "Déconnecter");
            _translations.Add(TranslationKey.UiMenuButtonResume, "Continuer");

            // Game Over

            _translations.Add(TranslationKey.UiMenuTitleGameOver, "Jeu Terminé");
            _translations.Add(TranslationKey.UiMenuButtonMainMenu, "Menu Principal");

            // HUD

            _translations.Add(TranslationKey.UiHudPlayerLabel, "Joueur");
            _translations.Add(TranslationKey.UiHudScoreLabel, "But");
            _translations.Add(TranslationKey.UiHudPowerUpLabel, "Pouvoirs");
            _translations.Add(TranslationKey.UiHudSelectContainerLabel, "Sélectionner un Joueur");
            _translations.Add(TranslationKey.UiHudSelectContainerSubLabel, "Pour saboter, capacité: {0}");
            _translations.Add(TranslationKey.UiHudSelectContainerTimerLabel, "Temps restant: {0}s");

            // Game

            _translations.Add(TranslationKey.GameContainerDropSpeed, "Drop Vitesse: {0}");
            
            // Power Up

            _translations.Add(TranslationKey.PowerUpRandomBlock, "Bloc Aléatoire");
            _translations.Add(TranslationKey.PowerUpClearLayer, "Couche Claire");
            _translations.Add(TranslationKey.PowerUpAddLayer, "Ajouter la Couche");
        }
    }
}