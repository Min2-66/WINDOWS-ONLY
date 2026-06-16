namespace ScholasticaReader.Models
{
    public class PasswordPolicy
    {
        public int Length { get; set; } = 16;
        public bool IncludeUppercase { get; set; } = true;
        public bool IncludeLowercase { get; set; } = true;
        public bool IncludeNumbers { get; set; } = true;
        public bool IncludeSpecialChars { get; set; } = true;
        public string? ExcludeChars { get; set; }
    }
}
