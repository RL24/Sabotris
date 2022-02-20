using System;

namespace UI.Menu
{
    public class MenuChatHistoryItem : MenuButton
    {
        public Guid id;
        private string _author;
        private string _message;

        protected override void Start()
        {
            base.Start();

            UpdateText();
        }

        private void UpdateText()
        {
            if (text == null)
                return;

            text.text = $"{Author}: {Message}";
        }

        public string Author
        {
            get => _author;
            set
            {
                if (value == _author)
                    return;

                _author = value;

                UpdateText();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (value == _message)
                    return;

                _message = value;

                UpdateText();
            }
        }
    }
}