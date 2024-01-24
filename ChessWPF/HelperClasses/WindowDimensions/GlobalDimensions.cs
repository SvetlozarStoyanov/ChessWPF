namespace ChessWPF.HelperClasses.WindowDimensions
{
    public static class GlobalDimensions
    {
        private static double height;
        private static double width;
        
        public static double Height
        {
            get => height > 0 ? height : System.Windows.SystemParameters.PrimaryScreenHeight;
            set => height = value > 0 ? value : 0;
        }
        public static double Width
        {
            get => width > 0 ? width : System.Windows.SystemParameters.PrimaryScreenWidth;
            set => width = value > 0 ? value : 0;
        }
    }
}
