using TMPro;
using Sabotris.Translations;

namespace Sabotris.UI.Menu
{
    public class MenuLobbyListItem : MenuButton
    {
        public TMP_Text playerCountText;
        
        public ulong lobbyId;
        private string _lobbyName;
        private int? _lobbyPlayerCount;
        private int? _maxLobbyPlayers;

        protected override void Start()
        {
            base.Start();

            if (text)
                text.text = LobbyName;

            UpdatePlayerCountText();
        }

        private void UpdatePlayerCountText()
        {
            if (playerCountText)
                playerCountText.text = LobbyPlayerCount == null || MaxLobbyPlayers == null ? "" : Localization.Translate(TranslationKey.UiMenuLobbyItemPlayerCount, LobbyPlayerCount, MaxLobbyPlayers);
        }

        public string LobbyName
        {
            get => _lobbyName;
            set
            {
                if (value == _lobbyName)
                    return;

                _lobbyName = value;

                if (text)
                    text.text = LobbyName;
            }
        }

        public int? LobbyPlayerCount
        {
            get => _lobbyPlayerCount;
            set
            {
                if (value == _lobbyPlayerCount)
                    return;

                _lobbyPlayerCount = value;

                UpdatePlayerCountText();
            }
        }

        public int? MaxLobbyPlayers
        {
            get => _maxLobbyPlayers;
            set
            {
                if (value == _maxLobbyPlayers)
                    return;

                _maxLobbyPlayers = value;

                UpdatePlayerCountText();
            }
        }
    }
}