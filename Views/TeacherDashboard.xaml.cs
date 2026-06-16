using System;
using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class TeacherDashboard : Window
    {
        public TeacherDashboard()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing teacher dashboard: {ex.Message}", "Initialization Error");
            }
        }

        private void AssignReading_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Assign Reading feature coming soon.", "Feature");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning reading: {ex.Message}", "Error");
            }
        }
    }
}
