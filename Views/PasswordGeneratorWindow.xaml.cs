using System;
using System.Windows;
using ScholasticaReader.Models;
using ScholasticaReader.Services;

namespace ScholasticaReader.Views
{
    public partial class PasswordGeneratorWindow : Window
    {
        private readonly PasswordGeneratorService _passwordService;

        public PasswordGeneratorWindow()
        {
            try
            {
                InitializeComponent();
                _passwordService = new PasswordGeneratorService();
                LengthSlider.ValueChanged += (s, e) => UpdateLengthDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing password generator: {ex.Message}", "Initialization Error");
            }
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate at least one option is selected
                if (!IncludeUppercaseCheckBox.IsChecked.HasValue || !IncludeUppercaseCheckBox.IsChecked.Value &&
                    !IncludeLowercaseCheckBox.IsChecked.HasValue || !IncludeLowercaseCheckBox.IsChecked.Value &&
                    !IncludeNumbersCheckBox.IsChecked.HasValue || !IncludeNumbersCheckBox.IsChecked.Value &&
                    !IncludeSpecialCheckBox.IsChecked.HasValue || !IncludeSpecialCheckBox.IsChecked.Value)
                {
                    MessageBox.Show("Please select at least one character type.", "Validation Error");
                    return;
                }

                var policy = new PasswordPolicy
                {
                    Length = (int)LengthSlider.Value,
                    IncludeUppercase = IncludeUppercaseCheckBox.IsChecked.GetValueOrDefault(true),
                    IncludeLowercase = IncludeLowercaseCheckBox.IsChecked.GetValueOrDefault(true),
                    IncludeNumbers = IncludeNumbersCheckBox.IsChecked.GetValueOrDefault(true),
                    IncludeSpecialChars = IncludeSpecialCheckBox.IsChecked.GetValueOrDefault(true),
                    ExcludeChars = ExcludeCharsBox.Text
                };

                string password = _passwordService.GeneratePassword(policy);
                GeneratedPasswordBox.Text = password;

                // Check strength
                bool isStrong = _passwordService.ValidatePasswordStrength(password);
                StrengthIndicator.Text = isStrong ? "Strength: Strong ✓" : "Strength: Moderate";
                StrengthIndicator.Foreground = isStrong ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Orange;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating password: {ex.Message}", "Generation Error");
            }
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GeneratedPasswordBox.Text != "Click Generate" && !string.IsNullOrEmpty(GeneratedPasswordBox.Text))
                {
                    Clipboard.SetText(GeneratedPasswordBox.Text);
                    MessageBox.Show("Password copied to clipboard!", "Success");
                }
                else
                {
                    MessageBox.Show("Please generate a password first.", "Information");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying password: {ex.Message}", "Copy Error");
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GeneratedPasswordBox.Text = "Click Generate";
                StrengthIndicator.Text = "Strength: ---";
                StrengthIndicator.Foreground = System.Windows.Media.Brushes.Gray;
                LengthSlider.Value = 16;
                ExcludeCharsBox.Clear();
                IncludeUppercaseCheckBox.IsChecked = true;
                IncludeLowercaseCheckBox.IsChecked = true;
                IncludeNumbersCheckBox.IsChecked = true;
                IncludeSpecialCheckBox.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing form: {ex.Message}", "Clear Error");
            }
        }

        private void UpdateLengthDisplay()
        {
            LengthDisplay.Text = $"Length: {(int)LengthSlider.Value}";
        }
    }
}
