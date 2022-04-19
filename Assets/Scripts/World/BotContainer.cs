namespace Sabotris
{
    public class BotContainer : DemoContainer
    {
        protected override int GetDropSpeed()
        {
            return _dropSpeedMs;
        }
    }
}