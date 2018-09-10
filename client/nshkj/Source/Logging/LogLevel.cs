using System;

namespace nshkj
{
	/// <summary>
	/// Defines levels for log messages, in ascendant order of importance
	/// </summary>
	public enum LogLevel : ushort {
		Error = 0,
		Warning = 1,
		Informational = 2,
		Verbose = 3,
		Debug = 4,
	}

	/// <summary>
	/// Provides extension methods to the LogLevel struct
	/// </summary>
	public static class LogLevelExtensions {
		/// <summary>
		/// Get the short string representing the log level associated to a message
		/// </summary>
		/// <param name="level">This log level</param>
		/// <returns>A string of a maximum of 5 characters representing the log level, or an empty string if no such level is implemented</returns>
		public static string ToShortString(this LogLevel level) {
			switch (level) {
				case LogLevel.Error:
					return "FAIL";
				case LogLevel.Warning:
					return "WARN";
				case LogLevel.Informational:
					return "INFO";
				case LogLevel.Verbose:
					return "VERB";
				case LogLevel.Debug:
					return "DBUG";
			}

			return String.Empty;
		}

		/// <summary>
		/// Get the Consolecolor associated to a particular log level
		/// </summary>
		/// <param name="level">This log level</param>
		/// <returns>The ConsoleColor associated to this log level or ConsoleColor.Gray if no such level is implemented</returns>
		public static ConsoleColor GetAssociatedConsoleColor(this LogLevel level) {
			switch (level) {
				case LogLevel.Error:
					return ConsoleColor.Red;
				case LogLevel.Warning:
					return ConsoleColor.Yellow;
				case LogLevel.Informational:
					return ConsoleColor.Cyan;
				case LogLevel.Verbose:
					return ConsoleColor.Gray;
				case LogLevel.Debug:
					return ConsoleColor.DarkGray;
			}

			return ConsoleColor.Gray;
		}
	}
}