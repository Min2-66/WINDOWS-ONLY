using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace ScholasticaReader.Services
{
    public class LicenseService
    {
        private readonly string dbPath;
        private DateTime? licenceExpiry;
        private bool isPremium = false;

        public LicenseService()
        {
            try
            {
                dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ScholasticaReader", "license.db");
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? string.Empty);
                InitializeDatabase();
                LoadLicenseStatus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing LicenseService: {ex.Message}");
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    var connectionString = $"Data Source={dbPath}";
                    using (var connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = @"
                            CREATE TABLE Licenses (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                HWID TEXT NOT NULL,
                                ActivationCode TEXT NOT NULL,
                                ExpiryDate TEXT NOT NULL,
                                IsPremium INTEGER NOT NULL
                            );
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        private void LoadLicenseStatus()
        {
            try
            {
                var connectionString = $"Data Source={dbPath}";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT ExpiryDate, IsPremium FROM Licenses ORDER BY Id DESC LIMIT 1";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (DateTime.TryParse(reader.GetString(0), out DateTime expiry))
                            {
                                licenceExpiry = expiry;
                            }
                            isPremium = reader.GetInt32(1) == 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading license status: {ex.Message}");
            }
        }

        public bool ValidateLicense()
        {
            if (licenceExpiry == null || licenceExpiry <= DateTime.Now)
            {
                return ShowActivationDialog();
            }
            return true;
        }

        private bool ShowActivationDialog()
        {
            try
            {
                string hwid = HWIDHelper.GetHWID();
                string message = $"Your Hardware ID (HWID):\n{hwid}\n\nSend this to the developer to get an activation code.\n\nEnter the code below:";
                string? code = Microsoft.VisualBasic.Interaction.InputBox(message, "Activation", "");
                if (string.IsNullOrEmpty(code))
                    return false;

                if (VerifyActivationCode(hwid, code))
                {
                    SaveLicense(code);
                    LoadLicenseStatus();
                    return true;
                }
                else
                {
                    MessageBox.Show("Invalid activation code or it has expired.", "Activation Failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in activation dialog: {ex.Message}");
                return false;
            }
        }

        private bool VerifyActivationCode(string hwid, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return false;

                string decrypted = SecurityService.DecryptString(code);
                if (string.IsNullOrEmpty(decrypted))
                    return false;
                    
                var parts = decrypted.Split('|');
                if (parts.Length < 2) 
                    return false;
                if (parts[0] != hwid) 
                    return false;
                
                if (!DateTime.TryParse(parts[1], out DateTime expiry))
                    return false;
                    
                if (expiry <= DateTime.Now) 
                    return false;
                    
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying activation code: {ex.Message}");
                return false;
            }
        }

        private void SaveLicense(string code)
        {
            try
            {
                string decrypted = SecurityService.DecryptString(code);
                if (string.IsNullOrEmpty(decrypted))
                    return;
                    
                var parts = decrypted.Split('|');
                
                if (parts.Length < 2 || !DateTime.TryParse(parts[1], out DateTime expiry))
                    return;
                    
                bool premium = parts.Length > 2 && parts[2] == "Premium";

                var connectionString = $"Data Source={dbPath}";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO Licenses (HWID, ActivationCode, ExpiryDate, IsPremium)
                        VALUES (@hwid, @code, @expiry, @premium)
                    ";
                    cmd.Parameters.AddWithValue("@hwid", HWIDHelper.GetHWID());
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@expiry", expiry.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@premium", premium ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving license: {ex.Message}");
            }
        }

        public bool IsPremium => isPremium;
        public DateTime? ExpiryDate => licenceExpiry;
    }
}
