using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class RoutingTableLogFileManager
	{
		internal static string LogFilePath
		{
			get
			{
				if (RoutingTableLogFileManager.directory == null)
				{
					return RoutingTableLogFileManager.directoryPath;
				}
				return RoutingTableLogFileManager.directory.FullName;
			}
		}

		public static string CleanupLogsAndGetLogFileName(DateTime time, RoutingContextCore context)
		{
			if (!context.Dependencies.IsProcessShuttingDown)
			{
				RoutingTableLogFileManager.CleanupLogFiles(context);
			}
			int num = Interlocked.Increment(ref RoutingTableLogFileManager.sequenceNumber) % 256;
			string text = time.ToString(CultureInfo.InvariantCulture).Replace(':', '_');
			text = text.Replace('/', '_');
			return string.Concat(new string[]
			{
				RoutingTableLogFileManager.LogFilePath,
				"\\",
				RoutingTableLogFileManager.GetProcessRolePrefix(context.GetProcessRoleForDiagnostics()),
				"RoutingConfig#",
				num.ToString(),
				"@",
				text,
				".xml"
			});
		}

		public static void HandleTransportServerConfigChange(TransportServerConfiguration transportServerConfiguration)
		{
			RoutingDiag.Tracer.TraceDebug(0L, "Transport server config change detected");
			RoutingTableLogFileManager.CreateLogDirectory(transportServerConfiguration.TransportServer.RoutingTableLogPath);
		}

		private static void CleanupLogFiles(RoutingContextCore context)
		{
			FileInfo[] array = null;
			bool flag = false;
			if (RoutingTableLogFileManager.directory != null)
			{
				try
				{
					array = RoutingTableLogFileManager.directory.GetFiles(RoutingTableLogFileManager.GetProcessRolePrefix(context.GetProcessRoleForDiagnostics()) + "RoutingConfig#*.xml");
					goto IL_45;
				}
				catch (DirectoryNotFoundException)
				{
					RoutingDiag.Tracer.TraceError(0L, "Directory does not exist; attempting to create it");
					flag = true;
					goto IL_45;
				}
			}
			flag = true;
			IL_45:
			if (flag)
			{
				RoutingTableLogFileManager.CreateLogDirectory(context.Dependencies.LogPath);
				return;
			}
			Array.Sort<FileInfo>(array, RoutingTableLogFileManager.compareFileCreationTimeDelegate);
			int num = 0;
			while (num < array.Length && RoutingTableLogFileManager.IsFileTooOld(array[num], context.Dependencies.MaxLogFileAge))
			{
				RoutingTableLogFileManager.DeleteFile(array[num]);
				num++;
			}
			if (context.Dependencies.MaxLogDirectorySize.IsUnlimited)
			{
				return;
			}
			ulong num2 = 0UL;
			int i;
			for (i = array.Length - 1; i >= num; i--)
			{
				num2 += (ulong)array[i].Length;
				if (num2 >= context.Dependencies.MaxLogDirectorySize.Value.ToBytes())
				{
					break;
				}
			}
			for (int j = num; j <= i; j++)
			{
				RoutingTableLogFileManager.DeleteFile(array[j]);
			}
		}

		private static bool IsFileTooOld(FileInfo file, EnhancedTimeSpan maxLogFileAge)
		{
			TimeSpan t = DateTime.UtcNow.Subtract(file.CreationTimeUtc);
			return t > maxLogFileAge;
		}

		private static void DeleteFile(FileInfo file)
		{
			try
			{
				file.Delete();
			}
			catch (UnauthorizedAccessException ex)
			{
				RoutingDiag.Tracer.TraceError<string, UnauthorizedAccessException>(0L, "Can't delete routing config file {0}, Exception: {1}", file.FullName, ex);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogDeletionFailure, null, new object[]
				{
					file.FullName,
					ex.ToString(),
					ex
				});
			}
			catch (IOException ex2)
			{
				RoutingDiag.Tracer.TraceError<string, IOException>(0L, "Can't delete routing config file {0}, Exception: {1}", file.FullName, ex2);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogDeletionFailure, null, new object[]
				{
					file.FullName,
					ex2.ToString(),
					ex2
				});
			}
		}

		private static void CreateLogDirectory(LocalLongFullPath logPath)
		{
			string text;
			if (logPath == null || string.IsNullOrEmpty(logPath.PathName))
			{
				text = ConfigurationContext.Setup.InstallPath + "\\TransportRoles\\Logs\\Routing";
			}
			else
			{
				text = logPath.PathName;
			}
			RoutingTableLogFileManager.directoryPath = text;
			RoutingDiag.Tracer.TraceDebug<string>(0L, "Creating log file directory {0}", text);
			try
			{
				RoutingTableLogFileManager.directory = Log.CreateLogDirectory(text);
			}
			catch (UnauthorizedAccessException ex)
			{
				RoutingDiag.Tracer.TraceError<string, UnauthorizedAccessException>(0L, "Can't create routing log dir {0}, Exception: {1}", text, ex);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogCreationFailure, null, new object[]
				{
					text,
					ex.ToString(),
					ex
				});
			}
			catch (IOException ex2)
			{
				RoutingDiag.Tracer.TraceError<string, IOException>(0L, "Can't create routing log dir {0}, Exception: {1}", text, ex2);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogCreationFailure, null, new object[]
				{
					text,
					ex2.ToString(),
					ex2
				});
			}
		}

		private static int CompareFileCreationTime(FileInfo x, FileInfo y)
		{
			return DateTime.Compare(x.CreationTimeUtc, y.CreationTimeUtc);
		}

		private static string GetProcessRolePrefix(ProcessTransportRole role)
		{
			switch (role)
			{
			case ProcessTransportRole.Hub:
				return "H";
			case ProcessTransportRole.Edge:
				return string.Empty;
			case ProcessTransportRole.FrontEnd:
				return "F";
			case ProcessTransportRole.MailboxSubmission:
				return "MS";
			case ProcessTransportRole.MailboxDelivery:
				return "MD";
			default:
				throw new ArgumentOutOfRangeException("role", role, "Unexpected role value: " + role);
			}
		}

		private const string LogFilePrefix = "RoutingConfig#";

		private const string LogFilePattern = "RoutingConfig#*.xml";

		private const int MaxLogFileNumber = 256;

		private static int sequenceNumber;

		private static DirectoryInfo directory;

		private static string directoryPath = string.Empty;

		private static Comparison<FileInfo> compareFileCreationTimeDelegate = new Comparison<FileInfo>(RoutingTableLogFileManager.CompareFileCreationTime);
	}
}
