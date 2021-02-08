using System.Windows;
using GymTrackingSystem.Services.Database;

namespace GymTrackingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Interact.EnsureDB();
        }
    }
}
