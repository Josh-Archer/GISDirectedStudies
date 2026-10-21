using System;
using System.Collections.Generic;

namespace DirectedStudies
{
    // Pure helpers for validating rows read from the input file.
    // Kept free of WinForms dependencies so they can be unit-tested.
    public static class InputValidation
    {
        // input is new double[366][][], so valid slot indices are 0..365.
        public const int MinTime = 0;
        public const int MaxTime = 365;

        // A row needs X, Y, time and C: indices 0, 1, 2 and 3 (time at index 2).
        public const int RequiredColumns = 4;

        public static bool IsValidTime(double time)
        {
            return !double.IsNaN(time)
                && !double.IsInfinity(time)
                && time >= MinTime
                && time <= MaxTime;
        }

        public static bool HasRequiredColumns(double[] row)
        {
            return row != null && row.Length >= RequiredColumns;
        }

        public static bool IsValidRow(double[] row)
        {
            return HasRequiredColumns(row) && IsValidTime(row[2]);
        }

        // Returns false (without throwing) for blank, short, non-numeric
        // or out-of-range rows. Previously a short row threw at arr[2]
        // inside the catch block that was meant to insert the first
        // count, and an out-of-range time threw when used as an index
        // into the fixed 366-slot array.
        public static bool TryParseRow(string line, out double[] row)
        {
            row = null;
            if (String.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] parts = line.Split('\t');
            if (parts.Length < RequiredColumns)
            {
                return false;
            }

            double[] parsed = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                double value;
                if (!Double.TryParse(parts[i], out value))
                {
                    return false;
                }
                parsed[i] = value;
            }

            if (!IsValidTime(parsed[2]))
            {
                return false;
            }

            row = parsed;
            return true;
        }

        public static void IncrementCount(Dictionary<double, int?> times, double time)
        {
            int? count;
            if (times.TryGetValue(time, out count) && count.HasValue)
            {
                times[time] = count.Value + 1;
            }
            else
            {
                times[time] = 1;
            }
        }
    }
}
