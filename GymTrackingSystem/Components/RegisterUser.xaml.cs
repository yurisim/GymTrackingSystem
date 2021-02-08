using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GymTrackingSystem.Services;
using GymTrackingSystem.Services.Database;

namespace GymTrackingSystem.Components
{
    /// <summary>
    /// Interaction logic for RegisterUser.xaml
    /// </summary>
    public partial class RegisterUser
    {
        /// <summary>
        ///     Number of People Allowed in Facility at Once
        ///     TODO: Make this a application wide variable
        /// </summary>
        private const int Capacity = 43;

        /// <summary>
        /// Global User object that is created/modified by form.
        /// </summary>
        private readonly User CurrentUser = new User();

        /// <summary>
        /// Constructor for component
        /// </summary>
        public RegisterUser()
        {
            InitializeComponent();

            stkInput.IsEnabled = false;
            cboBadgeNumber.Text = "1".PadLeft(2,'0');
        }

        private void ConfigurePage()
        {
            // Communicate with the Database & get Active Sessions
            var activeSessions = Interact.GetActiveSessions();

            //// Setup Dashboard
            barCapacity.Maximum = Capacity; 
            barCapacity.Value = activeSessions.Count();
            lblProgress.Content = $"Capacity: {barCapacity.Value} / {barCapacity.Maximum}";

            // This will display the users on the DataGrid
            grdUsers.ItemsSource = activeSessions;

            btnAdd.IsDefault = false;
            btnSubmit.IsDefault = true;
            txtScan.Focus();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // We grab their input and immediately delete in input text box
            var scanInput = txtScan.Password.ToUpper();
            txtScan.Clear();

            try
            {
                // DoDID is stripped out
                CurrentUser.Id = Process.GetDoDID(scanInput);

                //Look at list of "Active" Sessions
                var activeSessions = Interact.GetActiveSessions();

                var blnCheckOut = activeSessions.Any(session => session.Item1 == CurrentUser.Id);

                // Is a user checking out?
                if (blnCheckOut)
                {
                    // This will checkout a user by filling their DateTimeOut
                    var visitLeaving = Interact.RetrieveActiveVisit(CurrentUser.Id);

                    visitLeaving.DateTimeOut = DateTime.Now;

                    Interact.UpdateVisit(visitLeaving);

                    ConfigurePage();

                    txtScan.Focus();
                }
                else
                {
                    // Last Name
                    var name = scanInput.Substring(35, 26).Trim().ToLower();
                    txtLastName.Text = name.First().ToString().ToUpper() + name.Substring(1);

                    // Current Time, we store the "long time" in the DB for use, but display the "short time" for the user
                    txtTime.Text = DateTime.Now.ToShortTimeString();

                    // Try to find a user with that DoDID
                    var checkUser = Interact.GetUserFromDoDID(CurrentUser.Id);

                    stkInput.IsEnabled = true;
                    stkScan.IsEnabled = false;

                    if (checkUser != null)
                    {
                        // Last Name, in case here's a better one in the system
                        txtLastName.Text = checkUser.LastName;

                        txtPhone.Text = checkUser.Phone;

                        cboUnit.Text = checkUser.Unit;
                    }
                    else
                    {
                        txtPhone.Focus();
                    }

                    btnSubmit.IsDefault = false;
                    btnAdd.IsDefault = true;
                }
            }
            catch
            {
                txtScan.Focus();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Make DataRow for new visit

            var badgeNumber = Convert.ToInt32(cboBadgeNumber.Text);

            var newVisit = new Visit
            {
                User = CurrentUser,
                DateTimeIn = DateTime.Now,
                BadgeColor = cboBadgeColor.Text,
                BadgeNumber = badgeNumber
            };

            // Check if this is a new user or a returning user. This will return null if it doesn't find anyone with that DoDID
            var foundUser = Interact.GetUserFromDoDID(CurrentUser.Id);

            // If we find ANY record, that means it's a returning user
            if (foundUser != null)
            {
                // TODO: Only update this data set if changes are made to the input fields

                foundUser.LastName = txtLastName.Text;
                foundUser.Unit = cboUnit.Text;
                foundUser.Phone = txtPhone.Text;

                Interact.UpdateUser(foundUser);
                newVisit.User = foundUser;
            }
            else
            {
                var newUser = new User
                {
                    Id = CurrentUser.Id,
                    LastName = txtLastName.Text,
                    Unit = cboUnit.Text,
                    Phone = txtPhone.Text
                };

                Interact.AddUser(newUser);
                newVisit.User = newUser;
            }

            Interact.AddVisit(newVisit, newVisit.User);

            cboBadgeNumber.Text = (badgeNumber + 1).ToString().PadLeft(2,'0');

            ConfigurePage();

            ResetFields();
        }

        private void ResetFields()
        {
            txtLastName.Clear();
            txtPhone.Clear();
            cboUnit.Text = string.Empty;

            stkInput.IsEnabled = false;
            stkScan.IsEnabled = true;

            txtScan.Focus();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ConfigurePage();
        }

        private void cmbUnit_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ComboBox) sender).ItemsSource = Interact.GetValidUnits();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void cboBadgeColor_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = Interact.GetValidColors();
        }
    }
}
