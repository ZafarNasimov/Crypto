using BCrypt.Net;

public static class Authentication
{

    public static string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    public static bool VerifyPassword(string enteredPassword, string storedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash); 
    }

    public static string MaskPassword()
    {
        var pass = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        return pass;
    }
}

