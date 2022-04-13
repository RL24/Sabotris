namespace Sabotris.Powers
{
    public abstract class PowerUp
    {
        public abstract Power GetPower();
        
        public abstract void Use(Container activatingContainer);
    }
}