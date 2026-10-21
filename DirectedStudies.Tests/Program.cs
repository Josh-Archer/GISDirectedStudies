using System;
using System.Collections.Generic;
using DirectedStudies;

namespace DirectedStudies.Tests
{
    // Dependency-free test runner: `dotnet run --project DirectedStudies.Tests`
    // exits 0 when every check passes, 1 otherwise. Covers issue #6:
    // a time outside 0..365 and a short line must not throw.
    internal static class Program
    {
        private static int _failures;

        private static void Check(bool condition, string name)
        {
            Console.WriteLine((condition ? "PASS" : "FAIL") + ": " + name);
            if (!condition)
            {
                _failures++;
            }
        }

        private static int Main()
        {
            double[] row;

            // Time range boundaries (input has 366 slots: indices 0..365).
            Check(InputValidation.IsValidTime(0), "time 0 is valid");
            Check(InputValidation.IsValidTime(365), "time 365 is valid");
            Check(!InputValidation.IsValidTime(-1), "time -1 is rejected");
            Check(!InputValidation.IsValidTime(366), "time 366 is rejected");
            Check(!InputValidation.IsValidTime(1000), "time 1000 is rejected");
            Check(!InputValidation.IsValidTime(double.NaN), "time NaN is rejected");

            // Out-of-range rows are skipped, not thrown (issue #6).
            Check(!InputValidation.TryParseRow("1\t2\t366\t9", out row) && row == null,
                "row with time 366 is skipped without throwing");
            Check(!InputValidation.TryParseRow("1\t2\t-1\t9", out row) && row == null,
                "row with time -1 is skipped without throwing");

            // Short lines are skipped, not thrown. Previously arr[2] threw
            // inside the catch meant to insert the first count, and the
            // catch re-accessed arr[2].
            Check(!InputValidation.TryParseRow("1\t2", out row) && row == null,
                "short line (2 columns) is skipped without throwing");
            Check(!InputValidation.TryParseRow("1\t2\t3", out row) && row == null,
                "line missing C column (3 columns) is skipped without throwing");
            Check(!InputValidation.TryParseRow("", out row) && row == null,
                "empty line is skipped without throwing");
            Check(!InputValidation.TryParseRow(null, out row) && row == null,
                "null line is skipped without throwing");
            Check(!InputValidation.TryParseRow("a\tb\tc\td", out row) && row == null,
                "non-numeric line is skipped without throwing");

            // Valid rows still parse.
            Check(InputValidation.TryParseRow("1\t2\t0\t9", out row) && row.Length == 4 && row[2] == 0,
                "valid row at time 0 parses");
            Check(InputValidation.TryParseRow("1\t2\t365\t9", out row) && row.Length == 4 && row[2] == 365,
                "valid row at time 365 parses");

            // First count is inserted without relying on an exception.
            Dictionary<double, int?> times = new Dictionary<double, int?>();
            InputValidation.IncrementCount(times, 5);
            Check(times[5] == 1, "first count for a time is 1");
            InputValidation.IncrementCount(times, 5);
            Check(times[5] == 2, "second count for a time is 2");

            Console.WriteLine(_failures == 0 ? "All tests passed." : _failures + " test(s) failed.");
            return _failures == 0 ? 0 : 1;
        }
    }
}
