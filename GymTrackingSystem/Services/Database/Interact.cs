namespace GymTrackingSystem.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using GymTrackingSystem.Properties;

    public static class Interact
    {
        public static void EnsureDB()
        {
            using var gymContext = new GymContext();

            //Ensure database is created
            gymContext.Database.EnsureCreated();

        }

        /// <summary>
        ///     Gets the visits from a specific DoDID, returning a tuple array of DoDID, DateTimeIn, DateTimeOut and the duration
        ///     of their visit.
        /// </summary>
        /// <param name="DoDID"></param>
        /// <returns></returns>
        public static Tuple<double, string, int>[] GetVisitIDs(long DoDID)
        {
            try
            {
                using var gymContext = new GymContext();

                return gymContext.Visits.Where(visit => visit.User.Id == DoDID && visit.DateTimeOut != default)
                                 .Select(visit => new Tuple<double, string, int>(
                                          visit.Id,
                                          visit.DateTimeIn.ToString("g"),
                                          Convert.ToInt32((visit.DateTimeOut - visit.DateTimeIn).TotalMinutes)))
                                 .ToArray();
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return default;
        }

        /// <summary>
        ///     Get the number of people @ the gym between two specific dates/times. NOTE! Will result in an overflow error if
        ///     beyond the bound of an int.
        /// </summary>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public static int GetCounts(DateTime dateBegin, DateTime dateEnd)
        {
            try
            {
                using var gymContext = new GymContext();

                return gymContext.Visits.Count(visit => visit.DateTimeIn >= dateBegin && visit.DateTimeOut <= dateEnd);
            }
            catch (OverflowException)
            {
                // This can occur if the Count function overflows above bounds of Int.
                MessageBox.Show(string.Format(Resources.TooManyResults, dateBegin, dateEnd));
            }
            catch
            {
                MessageBox.Show(Resources.CannotConnect);
            }

            return default;
        }

        public static Tuple<long, string, int, string, string, string>[] GetDetailedCounts(DateTime dateBegin, DateTime dateEnd)
        {
            try
            {
                using var gymContext = new GymContext();

                // These are the active users
                var activeVisits = gymContext.Visits.Where(visit =>
                    visit.DateTimeIn >= dateBegin
                    && (visit.DateTimeOut == default ? DateTime.Now : visit.DateTimeOut)
                    <= dateEnd);

                var formattedVisits = (from visit in activeVisits
                        join user in gymContext.Users on visit.User.Id equals user.Id
                        select new Tuple<long, string, int, string, string, string>(
                            user.Id,
                            user.LastName,
                            Convert.ToInt32(((visit.DateTimeOut == default ? DateTime.Now : visit.DateTimeOut) - visit.DateTimeIn).TotalMinutes),
                            user.Phone,
                            user.Unit,
                            visit.DateTimeIn.ToString()))
                    .ToArray();

                return formattedVisits;
            }
            catch (Exception)
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return default;
        }

        public static List<long> GetValidDoDIDs()
        {
            var result = new List<long>();
            try
            {
                using var gymContext = new GymContext();

                result = gymContext.Users.Select(user => user.Id).ToList();
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return result;
        }

        public static List<string> GetValidUnits()
        {
            var result = new List<string>();
            try
            {
                using var gymContext = new GymContext();

                result = gymContext.Users.Select(user => user.Unit).Distinct().ToList();
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return result;
        }

        public static void RemoveVisit(long DoDID)
        {
            var closeVisit = new Visit();

            try
            {
                using var gymContext = new GymContext();

                closeVisit = gymContext.Visits.First(visit => visit.User.Id == DoDID && visit.DateTimeOut == default);

                closeVisit.DateTimeOut = DateTime.Now;

                UpdateVisit(closeVisit);
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }
        }

        public static List<string> GetValidColors()
        {
            var result = new List<string>();
            try
            {
                using var gymContext = new GymContext();

                result = gymContext.Visits.Select(user => user.BadgeColor).Distinct().ToList();
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return result;
        }

        /// <summary>
        ///     Gets the intersecting visits given a VisitID
        /// </summary>
        /// <param name="visitID"></param>
        /// <returns></returns>
        public static Tuple<long, string>[] GetIntersections(int visitID)
        {
            Tuple<long, string>[] result = default;

            try
            {
                using var gymContext = new GymContext();

                // Check for visitID
                var specificVisit = gymContext.Visits.Find(visitID);

                if (specificVisit != null)
                {
                    // This checks for overlaps, see the following link for a derivation,
                    // https://www.soliantconsulting.com/blog/determining-two-date-ranges-overlap/
                    var foundVisits = gymContext.Visits.Where(visit => visit.DateTimeIn <= specificVisit.DateTimeOut &&
                                                                      visit.DateTimeOut >= specificVisit.DateTimeIn &&
                                                                      visit.Id != visitID);

                    result = (from foundVisit in foundVisits
                                 join user in gymContext.Users on foundVisit.User.Id equals user.Id
                                 select new Tuple<long, string>(user.Id, user.Phone)).Distinct().ToArray();

                    // Gets the list of DoDIDs and phone numbers needed to contact them
                }
            }
            catch
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return result;
        }

        /// <summary>
        ///     Returns a data table containing users that have an active gym session.
        ///     This is defined as the visits where the DateTimeOut is the Default Date. Primarily used for the main display
        ///     screen.
        /// </summary>
        public static Tuple<long, string, int, string, string, string, int>[] GetActiveSessions()
        {
            try
            {
                using var gymContext = new GymContext();

                // These are the active users
                var activeVisits = gymContext.Visits.Where(visit => visit.DateTimeOut.Equals(default));

                var formattedVisits = (from visit in activeVisits
                        join user in gymContext.Users on visit.User.Id equals user.Id
                        select new Tuple<long, string, int, string, string, string, int>(
                            user.Id,
                            user.LastName,
                            Convert.ToInt32((DateTime.Now - visit.DateTimeIn).TotalMinutes),
                            user.Phone,
                            user.Unit,
                            visit.BadgeColor,
                            visit.BadgeNumber))
                    .ToArray();

                return formattedVisits;
            }
            catch (Exception)
            {
                // The only possible error for this is a database connection error.
                MessageBox.Show(Resources.CannotConnect);
            }

            return default;
        }

        /// <summary>
        ///     Saves Visit Data.
        ///     TODO: Make a generic instead of overloaded methods
        /// </summary>
        /// <param name="visitToAdd"></param>
        /// <param name="userToTrack"></param>
        public static void AddVisit(Visit visitToAdd, User userToTrack)
        {
            using var gymContext = new GymContext();

            gymContext.Visits.Add(visitToAdd);
            userToTrack.Visits.Add(visitToAdd);

            gymContext.Users.Attach(userToTrack);

            gymContext.SaveChanges();
        }

        public static Visit RetrieveActiveVisit(long DoDID)
        {
            using var gymContext = new GymContext();

            return gymContext.Visits.First(visit => visit.User.Id == DoDID && visit.DateTimeOut == default);
        }

        /// <summary>
        ///     Saves User Data
        ///     TODO: Make a generic instead of overloaded methods
        /// </summary>
        /// <param name="userToAdd"></param>
        public static void AddUser(User userToAdd)
        {
            using var gymContext = new GymContext();

            gymContext.Users.Add(userToAdd);

            gymContext.SaveChanges();
        }

        /// <summary>
        ///     Saves User Data
        ///     TODO: Make a generic instead of overloaded methods
        /// </summary>
        /// <param name="userToUpdate"></param>
        public static void UpdateUser(User userToUpdate)
        {
            using var gymContext = new GymContext();

            gymContext.Users.Update(userToUpdate);

            gymContext.SaveChanges();
        }

        /// <summary>
        ///     Saves User Data
        ///     TODO: Make a generic instead of overloaded methods
        /// </summary>
        public static void UpdateVisit(Visit visitToUpdate)
        {
            using var gymContext = new GymContext();

            gymContext.Visits.Update(visitToUpdate);

            gymContext.SaveChanges();
        }

        /// <summary>
        ///     Returns a User with the given DoDID. Returns Null if none is found.
        /// </summary>
        /// <param name="DoDID"></param>
        /// <returns></returns>
        public static User GetUserFromDoDID(long DoDID)
        {
            using var gymContext = new GymContext();

            return gymContext.Users.Find(DoDID);
        }
    }
}