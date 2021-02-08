using System;
using System.Linq;

namespace GymTrackingSystem.Services
{
    public static class Process
    {
        /// <summary>
        ///     Given a raw scan, parse out the DoDID. It is a long because int is too small for DoDIDs.
        /// </summary>
        /// <param name="rawInput"></param>
        public static long GetDoDID(string rawInput)
        {
            // This returns a DODId that is in base 32
            var rawDoDID = rawInput.Substring(8, 7).ToUpper();

            return Convert.ToInt64(ConvertToBase10(rawDoDID, 32));
        }

        /// <summary>
        ///     Converts an arbitrary base into base 10.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputBase"></param>
        /// <returns></returns>
        private static double ConvertToBase10(string input, int inputBase)
        {
            var lengthOfInput = input.Length;

            return Enumerable.Range(0, lengthOfInput)
                             .Select(spot => GetValue(input[spot]) * Math.Pow(inputBase, lengthOfInput - spot - 1))
                             .Sum();
        }

        /// <summary>
        ///     Converts a character into its integer equivalent.
        ///     Supports 0 -> 9 and A -> z such that
        ///     A = 10 & Z = 35,  a = 36 and so forth
        /// </summary>
        /// <param name="charToConvert"></param>
        /// <returns></returns>
        private static int GetValue(char charToConvert)
        {
            return char.IsDigit(charToConvert)
                       ? charToConvert - '0'
                       : char.IsUpper(charToConvert)
                           ? charToConvert - 'A' + 10
                           : charToConvert - 'a' + 36;
        }
    }
}