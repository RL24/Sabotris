namespace Sabotris.Worlds
{
    public class DemoContainer : ControlledContainer
    {
        protected override void Start()
        {
            base.Start();

            OnEnable();
        }

        protected void OnEnable()
        {
            if (!ControllingShape)
                StartDropping();
        }

        protected override int GetDropSpeed()
        {
            return 1000;
        }
    }
}