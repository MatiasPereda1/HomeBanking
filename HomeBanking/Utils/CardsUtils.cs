using System.Security.Cryptography;

namespace HomeBanking.Utils
{
    public static class CardUtils
    {
        public static int RandomNumber(int length)
        {
            return RandomNumberGenerator.GetInt32(1, (int) Math.Pow(10, length));
        }
    }
}
