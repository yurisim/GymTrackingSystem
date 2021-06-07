namespace GymTrackingSystem
{
    using System.Windows;
    using GymTrackingSystem.Services.Database;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Interact.EnsureDB();

            InitializeComponent();
        }
    }
}
