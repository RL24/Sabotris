namespace UI.Menu
{
    public class MenuLobbyListItem : MenuButton
    {
        public ulong lobbyId;
        private string _lobbyName;

        protected override void Start()
        {
            base.Start();

            if (text)
                text.text = LobbyName;
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
    }
}