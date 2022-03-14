using TMPro;

namespace Sabotris.UI.Menu
{
    public class MenuLobbyListItem : MenuButton
    {
        public TMP_Text playerCountText;
        
        public ulong lobbyId;
        private string _lobbyName;
        private int? _lobbyPlayerCount;

        protected override void Start()
        {
            base.Start();

            if (text)
                text.text = LobbyName;

            if (playerCountText)
                playerCountText.text = LobbyPlayerCount == null ? "" : $"{LobbyPlayerCount} Player{(LobbyPlayerCount > 1 ? "s" : "")}";
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

                if (playerCountText)
                    playerCountText.text = LobbyPlayerCount == null ? "" : $"{LobbyPlayerCount} Player{(LobbyPlayerCount > 1 ? "s" : "")}";
            }
        }
    }
}