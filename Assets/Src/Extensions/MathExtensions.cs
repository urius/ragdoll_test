namespace Src.Extensions
{
    public static class MathExtensions
    {
        public static int Sign(this int number)
        {
            return number > 0 ? 1 : -1;
        }
        
        public static int Sign(this float number)
        {
            return number > 0 ? 1 : -1;
        }
    }
}