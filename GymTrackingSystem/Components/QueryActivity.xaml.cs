namespace GymTrackingSystem.Components
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using GymTrackingSystem.Services.Database;

    /// <summary>
    /// Interaction logic for QueryActivity.xaml
    /// </summary>
    public partial class QueryActivity
    {
        public QueryActivity()
        {
            InitializeComponent();
            QueryData();
        }

        /// <summary>
        /// Event to query Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryData();
        }

        private void QueryData()
        {
            var dateBeginValue = DateBegin.SelectedDate.GetValueOrDefault();

            var dateEndValue = DateEnd.SelectedDate.GetValueOrDefault();

            lblQueryDialog.Content = Properties.Resources.SelectDatesActivity;

            if (dateEndValue == default || dateEndValue < dateBeginValue) return;

            var countActivity = Interact.GetDetailedCounts(dateBeginValue, dateEndValue);

            lblQueryDialog.Content = string.Format(Properties.Resources.DateQuery, countActivity.Length);

            grdVisits.ItemsSource = countActivity;
        }

        private void DateShortcut_Selection(object sender, SelectionChangedEventArgs e)
        {
            var dateMode = lstDateRanges.SelectedIndex;

            switch (dateMode)
            {
                // Today
                case 0:
                    DateBegin.SelectedDate = DateTime.Today;
                    DateEnd.SelectedDate = DateTime.Today.AddDays(1);

                    DateBegin.IsEnabled = DateEnd.IsEnabled = false;
                    QueryData();
                    break;
                // Yesterday
                case 1:
                    DateBegin.SelectedDate = DateTime.Today.AddDays(-1);
                    DateEnd.SelectedDate = DateTime.Today;

                    DateBegin.IsEnabled = DateEnd.IsEnabled = false;
                    QueryData();
                    break;
                // Last 7 Days
                case 2:
                    DateBegin.SelectedDate = DateTime.Today.AddDays(-7);
                    DateEnd.SelectedDate = DateTime.Today.AddDays(1);

                    DateBegin.IsEnabled = DateEnd.IsEnabled = false;
                    QueryData();
                    break;
                // 30 days Ago
                case 3:
                    DateBegin.SelectedDate = DateTime.Today.AddDays(-30);
                    DateEnd.SelectedDate = DateTime.Today.AddDays(1);

                    DateBegin.IsEnabled = DateEnd.IsEnabled = false;
                    QueryData();
                    break;
                // Custom
                case 4:
                    DateBegin.SelectedDate = DateEnd.SelectedDate = null;

                    DateBegin.IsEnabled = DateEnd.IsEnabled = true;
                    lblQueryDialog.Content = Properties.Resources.SelectDatesActivity;
                    break;
            }


        }
    }
}
