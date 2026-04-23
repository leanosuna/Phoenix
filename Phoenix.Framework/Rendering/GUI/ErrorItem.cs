namespace Phoenix.Framework.Rendering.GUI
{
    internal class ErrorItem
    {
        public int Count { get; set; } = 0;
        public float CurrentTime { get; set; } = 0;
        public float MaxTime { get; set; } = 0;
        
        internal ErrorItem() { }

    }
}