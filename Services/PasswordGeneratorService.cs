using System;
using System.Text;
using System.Security.Cryptography;

namespace ScholasticaReader.Services
{
    public class PasswordGeneratorService
    {
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string NumberChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        public string GeneratePassword(Models.PasswordPolicy policy)
        {
            if (policy == null || policy.Length < 4)
                throw new ArgumentException("Password length must be at least 4 characters");

            try
            {
                string availableChars = BuildCharacterSet(policy);
                if (string.IsNullOrEmpty(availableChars))
                    throw new ArgumentException("At least one character type must be selected");

                StringBuilder password = new StringBuilder();
                using (var rng = new RNGCryptoServiceProvider())
                {
                    byte[] randomBytes = new byte[policy.Length];
                    rng.GetBytes(randomBytes);

                    foreach (byte b in randomBytes)
                    {
                        int index = b % availableChars.Length;
                        password.Append(availableChars[index]);
                    }
                }

                return password.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating password: {ex.Message}");
                throw;
            }
        }

        private string BuildCharacterSet(Models.PasswordPolicy policy)
        {
            StringBuilder charSet = new StringBuilder();

            if (policy.IncludeUppercase)
                charSet.Append(UppercaseChars);
            if (policy.IncludeLowercase)
                charSet.Append(LowercaseChars);
            if (policy.IncludeNumbers)
                charSet.Append(NumberChars);
            if (policy.IncludeSpecialChars)
                charSet.Append(SpecialChars);

            string result = charSet.ToString();

            // Remove excluded characters if specified
            if (!string.IsNullOrEmpty(policy.ExcludeChars))
            {
                foreach (char c in policy.ExcludeChars)
                {
                    result = result.Replace(c.ToString(), "");
                }
            }

            return result;
        }

        public bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false, hasLower = false, hasNumber = false, hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasNumber = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasNumber && hasSpecial;
        }
    }
}
