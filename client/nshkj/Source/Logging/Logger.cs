using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace nshkj
{
	public class Logger {
		/// <summary>
		/// Number of stack frames to skip for retrieving calling method on log messages
		/// </summary>
		protected const uint SkipFrames = 1;

		/// <summary>
		/// Number of ticks transcurred before the current log message
		/// </summary>
		private long previousTicks = -1;

		/// <summary>
		/// Provides a list of streams where each log message is to be forwarded
		/// </summary>
		public List<Stream> Streams {
			get;
			set;
		}

		/// <summary>
		/// Creates a new Logger instance
		/// </summary>
		public Logger() {
			this.Streams = new List<Stream>();
		}

		/// <summary>
		/// Writes a message to the logger
		/// </summary>
		/// <param name="level">Logging level</param>
		/// <param name="format">Message format, compatible with String.Format()</param>
		/// <param name="args">Arguments used to format the message</param>
		public void WriteLine(LogLevel level, string format, params object[] args) {
			// ticks to be compared for displaying time diff
			long ticks = DateTime.Now.Ticks;

			// get calling method
			MethodBase method = new StackFrame((int)SkipFrames).GetMethod();
			string methodName;

			// format method as "<enclosing type>::<method name>"
			if (!ReferenceEquals(null, method)) {
				methodName = $"{method.DeclaringType.Name}.{method.Name}";
			} else {
				methodName = "????";
			}

			// format message as "[<milliseconds diff> <level> <method>] <message>"
			string msg = String.Format("[{0:00.0000} {1} {2}] {3}",
				TimeSpan.FromTicks(previousTicks == -1 ? 0 : ticks - previousTicks).TotalSeconds,  // zero if no previous tick count is set, otherwise the difference of ticks
				level.ToShortString(),
				methodName,
				String.Format(format, args)) + Environment.NewLine;

			// update ticks for next message
			previousTicks = ticks;

			// write message to console
			Console.ForegroundColor = level.GetAssociatedConsoleColor();
			Console.Write(msg);

			// write message to each stream
			this.Streams.ForEach(stream => {
				stream.Write(Encoding.UTF8.GetBytes(msg), 0, msg.Length);
				stream.FlushAsync();
			});
		}
	}
}
