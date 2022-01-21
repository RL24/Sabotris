namespace Sabotris.Util
{
    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public Pair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}