namespace TableauAPI.FilesLogging
{
    /// <summary>
    /// A simple latch.  Stays false until its set to true
    /// </summary>
    internal class SimpleLatch
    {
        public bool Value { get; private set; }

        public void Trigger()
        {
            Value = true;
        }
    }
}