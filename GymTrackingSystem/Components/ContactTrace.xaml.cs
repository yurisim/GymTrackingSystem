namespace GymTrackingSystem.Components
{
    using System.Windows;
    using System.Windows.Controls;
    using GymTrackingSystem.Services.Database;
    using MaterialDesignThemes.Wpf;

    /// <summary>
    /// Interaction logic for ContactTrace.xaml
    /// </summary>
    public partial class ContactTrace
    {
        public ContactTrace()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(cboSearchDoDID.Text, out var searchDoDID))
            {
                var results = Interact.GetVisitIDs(searchDoDID);

                GridVisits.ItemsSource = results;

                lblVisitsOfDoDID.Content = string.Format(Properties.Resources.SearchDialog, searchDoDID, results.Length, "DoDID");

                HintAssist.SetHint(cboSearchDoDID, "Search DoDID");
            }
            else
            {
                HintAssist.SetHint(cboSearchDoDID, "Please Enter a Valid DoDID");

                cboSearchDoDID.Focus();
            }
        }

        private void btnTrace_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSearchVisitID.Text, out var visitID))
            {
                var results = Interact.GetIntersections(visitID);

                GridIntersections.ItemsSource = results;

                lblContactsOfVisitID.Content = string.Format(Properties.Resources.SearchDialog, visitID, results?.Length ?? 0, "VisitID");

                HintAssist.SetHint(txtSearchVisitID, "Search VisitID");
            }
            else
            {
                HintAssist.SetHint(txtSearchVisitID, "Please Enter a Valid VisitID");

                txtSearchVisitID.Focus();
            }
        }

        private void cboSearchDoDID_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = Interact.GetValidDoDIDs();
        }
    }
}
