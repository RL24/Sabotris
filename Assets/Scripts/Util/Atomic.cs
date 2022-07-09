namespace Sabotris.Util
{
    public class Atomic<T>
    {
        public T Value { get; set; }

        public Atomic(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }
    }
}