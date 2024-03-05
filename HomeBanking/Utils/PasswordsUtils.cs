using System.Security.Cryptography;
using System.Text;

namespace HomeBanking.Utils
{
    public class PasswordsUtils
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convierte la contraseña en bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Calcula el hash de la contraseña
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convierte el hash en una cadena hexadecimal
                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));
                }

                return hashStringBuilder.ToString();
            }
        }
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Verifica si el hash de la contraseña ingresada coincide con el almacenado
            return HashPassword(enteredPassword) == storedHash;
        }
    }
}
