using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class Logger : ILogger, IDisposable
	{
		private Logger(string prefix, string directory, Action<LocalizedString> writeVerbose)
		{
			this.writeVerbose = writeVerbose;
			if (Configuration.EnableLogging)
			{
				DateTime utcNow = DateTime.UtcNow;
				this.logFile = string.Format("{0}\\{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}.log", new object[]
				{
					directory,
					prefix,
					utcNow.Month,
					utcNow.Day,
					utcNow.Year,
					utcNow.Hour,
					utcNow.Minute,
					utcNow.Second,
					utcNow.Ticks
				});
				this.logFileForData = this.logFile.Replace(".log", ".extra.log");
			}
		}

		public static ILogger Create(Action<LocalizedString> writeVerbose)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(ConfigurationContext.Setup.LoggingPath, "Update-HybridConfiguration"));
			if (Configuration.EnableLogging && !directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			return new Logger("HybridConfiguration", directoryInfo.FullName, writeVerbose);
		}

		public void Log(LocalizedString text)
		{
			this.LogInformation(text.ToString());
		}

		public void Log(Exception e)
		{
			this.LogError(e.Message);
		}

		public void LogError(string information)
		{
			this.LogMessage("  ERROR", information, null);
		}

		public void LogWarning(string information)
		{
			this.LogMessage("WARNING", information, null);
		}

		public void LogInformation(string information)
		{
			this.LogMessage("   INFO", information, null);
		}

		public void LogInformation(string information, string data)
		{
			this.LogMessage("   INFO", information, data);
		}

		public void Dispose()
		{
		}

		public override string ToString()
		{
			return this.logFile;
		}

		private void LogMessage(string prefix, string information, string data = null)
		{
			if (this.writeVerbose != null)
			{
				this.writeVerbose(new LocalizedString(string.Format("{0}:{1}", prefix, information)));
			}
			if (Configuration.EnableLogging)
			{
				DateTime utcNow = DateTime.UtcNow;
				string lineHeader = string.Format("[{0:00}/{1:00}/{2} {3:00}:{4:00}:{5:00}] {6} : ", new object[]
				{
					utcNow.Month,
					utcNow.Day,
					utcNow.Year,
					utcNow.Hour,
					utcNow.Minute,
					utcNow.Second,
					prefix
				});
				string[] lines = information.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				Logger.Write(this.logFile, lineHeader, lines, null);
				if (!string.IsNullOrEmpty(data))
				{
					Logger.Write(this.logFileForData, lineHeader, lines, data);
				}
			}
		}

		private static void Write(string logFile, string lineHeader, string[] lines, string data)
		{
			using (StreamWriter streamWriter = new StreamWriter(logFile, true, Encoding.UTF8))
			{
				if (lines.Length == 0)
				{
					streamWriter.WriteLine(lineHeader);
				}
				else
				{
					streamWriter.WriteLine("{0}{1}", lineHeader, lines[0]);
				}
				if (lines.Length > 1)
				{
					string arg = new string(' ', lineHeader.Length);
					for (int i = 1; i < lines.Length; i++)
					{
						streamWriter.WriteLine("{0}{1}", arg, lines[i]);
					}
				}
				if (!string.IsNullOrEmpty(data))
				{
					streamWriter.WriteLine(data);
				}
			}
		}

		private const string LogFilePrefix = "HybridConfiguration";

		private const string LoggingSubDir = "Update-HybridConfiguration";

		private const string ErrorPrefix = "  ERROR";

		private const string WarningPrefix = "WARNING";

		private const string InformationPrefix = "   INFO";

		private readonly string logFile;

		private readonly string logFileForData;

		private Action<LocalizedString> writeVerbose;
	}
}
