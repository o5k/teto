using System;
using System.Collections.Generic;
using System.Linq;

namespace Teto {
    /// <summary>
    /// A schedule for tasks to run in.
    /// </summary>
    public class Schedule {
        /// <summary>
        /// The years in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedYears { get; private set; }
        /// <summary>
        /// The months in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedMonths { get; private set; }
        /// <summary>
        /// The days of the week in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedDOWs { get; private set; }
        /// <summary>
        /// The days of the month in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedDays { get; private set; }
        /// <summary>
        /// The hours in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedHours { get; private set; }
        /// <summary>
        /// The minutes in which the schedule is allowed to run, or an empty array if all are allowed.
        /// </summary>
        public int[] AllowedMinutes { get; private set; }
        /// <summary>
        /// An action that can be run by the scheduler.
        /// </summary>
        public delegate void Action();
        /// <summary>
        /// The action to be run.
        /// </summary>
        private Action action;

        /// <summary>
        /// Construct a schedule from its string arguments and a delegate action.
        /// </summary>
        /// <param name="y">The string representation of the allowed years.</param>
        /// <param name="mo">The string representation of the allowed months.</param>
        /// <param name="dow">The string representation of the allowed days of the week.</param>
        /// <param name="d">The string representation of the allowed days of the month.</param>
        /// <param name="h">The string representation of the allowed hours.</param>
        /// <param name="m">The string representation of the allowed minutes.</param>
        /// <param name="action">The action to be run.</param>
        public Schedule(string y, string mo, string dow, string d, string h, string m, Action action) {
            AllowedYears = ParseArgument(y);
            AllowedMonths = ParseArgument(mo);
            AllowedDOWs = ParseArgument(dow);
            AllowedDays = ParseArgument(d);
            AllowedHours = ParseArgument(h);
            AllowedMinutes = ParseArgument(m);

            if (AllowedMinutes.Length == 0) {
                The.Warning("The schedule's minutes parameter is set to post every minute. Double-check if that's what you intended?");
            }

            this.action = action;
        }

        /// <summary>
        /// Parse a string argument to an integer array.
        /// </summary>
        /// <param name="argument">The string argument to be parsed (either *, a number, or comma-seperated numbers)</param>
        /// <returns>The int[] of allowed values, an empty int[] if all are allowed, or an int[1] { -1 } if parsing failed.</returns>
        static int[] ParseArgument(string argument) {
            if (argument == "*") {
                return new int[0];
            }

            if (argument.All(o => char.IsDigit(o))) {
                return new int[1] { int.Parse(argument) };
            }

            if (argument.Contains(",")) {
                List<int> args = new List<int>();
                string[] pcs = argument.Split(',');

                foreach (string pc in pcs) {
                    string tpc = pc.Trim();

                    if (!tpc.All(o => char.IsDigit(o))) {
                        continue;
                    }

                    args.Add(int.Parse(tpc));
                }

                return args.ToArray();
            }

            The.Failure($"Failed to parse schedule value '{ argument }'. The schedule will not run.");
            return new int[] { -1 };
        }

        /// <summary>
        /// Check whether the schedule is allowed to run now.
        /// </summary>
        /// <returns>True if the schedule may run.</returns>
        public bool Test() {
            DateTime d = DateTime.Now;

            if (AllowedYears.Length > 0 && !AllowedYears.Contains(d.Year)) { return false; }
            if (AllowedMonths.Length > 0 && !AllowedMonths.Contains(d.Month)) { return false; }
            if (AllowedDOWs.Length > 0 && !AllowedDOWs.Contains((int)d.DayOfWeek)) { return false; }
            if (AllowedDays.Length > 0 && !AllowedDays.Contains(d.Day)) { return false; }
            if (AllowedHours.Length > 0 && !AllowedHours.Contains(d.Hour)) { return false; }
            if (AllowedMinutes.Length > 0 && !AllowedMinutes.Contains(d.Minute)) { return false; }

            return true;
        }

        /// <summary>
        /// Attempt to run the schedule now.
        /// </summary>
        /// <returns>Whether the schedule has run.</returns>
        public bool Run() {
            if (Test()) {
                action();
                return true;
            }
            return false;
        }
    }
}
