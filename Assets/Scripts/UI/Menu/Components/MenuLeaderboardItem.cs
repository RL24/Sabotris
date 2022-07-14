using TMPro;

namespace Sabotris.UI.Menu
{
    public class MenuLeaderboardItem : MenuButton
    {
        public TMP_Text playerHighscoreText;

        public string playerName;
        public string playerHighscore;

        protected override void Start()
        {
            base.Start();

            if (text)
                text.text = playerName;

            if (playerHighscoreText)
                playerHighscoreText.text = playerHighscore;
        }
    }
}