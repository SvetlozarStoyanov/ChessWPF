namespace ChessWPF.Singleton
{
    public sealed class BackgroundSingleton
    {
        private static BackgroundSingleton instance = null;
        private static readonly object padlock = new object();

        public BackgroundSingleton()
        {

        }

        public static BackgroundSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BackgroundSingleton();
                    }
                    return instance;
                }
            }
        }
    }
}
