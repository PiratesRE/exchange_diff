using System;
using System.IO;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class AgedOutDirectoryHelper
	{
		public static void DeleteSkippedLogs(string ignoredLogsDirectory, string databaseName, bool deleteAllFiles)
		{
			string text = Path.Combine(ignoredLogsDirectory, "SkippedLogs");
			if (!Directory.Exists(text))
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>(0L, "DeleteSkippedLogs(): Skipped logs directory '{0}' for database '{1}' doesn't exist.", text, databaseName);
				return;
			}
			foreach (string path in Directory.GetDirectories(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				if ((deleteAllFiles || DateTime.UtcNow.Subtract(directoryInfo.CreationTimeUtc) > TimeSpan.FromSeconds((double)RegistryParameters.SkippedLogsDeleteAfterAgeInSecs)) && DirectoryOperations.TryDeleteDirectoryRecursively(directoryInfo) == null)
				{
					ReplayEventLogConstants.Tuple_DeletedSkippedLogsDirectory.LogEvent(null, new object[]
					{
						databaseName,
						directoryInfo.FullName
					});
				}
			}
		}

		public static void MoveLogFiles(ReplayConfiguration config, FileState fileState, ISetBroken setBroken, long corruptLogGen)
		{
			string text = Path.Combine(config.E00LogBackupPath, "SkippedLogs");
			Exception ex = DirectoryOperations.TryCreateDirectory(text);
			if (ex != null)
			{
				setBroken.SetBroken(FailureTag.Configuration, ReplayEventLogConstants.Tuple_FailedToCreateDirectory, ex, new string[]
				{
					text,
					ex.ToString()
				});
				return;
			}
			string currentDateString = FileOperations.GetCurrentDateString();
			string text2 = Path.Combine(text, currentDateString);
			ex = DirectoryOperations.TryCreateDirectory(text2);
			if (ex != null)
			{
				setBroken.SetBroken(FailureTag.Configuration, ReplayEventLogConstants.Tuple_FailedToCreateDirectory, ex, new string[]
				{
					text2,
					ex.ToString()
				});
				return;
			}
			string destinationLogPath = config.DestinationLogPath;
			string text3 = string.Empty;
			string path = string.Empty;
			try
			{
				ReplayEventLogConstants.Tuple_MovingFilesToRestartLogStream.LogEvent(null, new object[]
				{
					config.DatabaseName,
					EseHelper.MakeLogfileName(config.LogFilePrefix, config.LogFileSuffix, corruptLogGen),
					text2
				});
				foreach (string text4 in Directory.GetFiles(destinationLogPath, "*." + config.LogExtension))
				{
					text3 = text4;
					path = Path.GetFileName(text4);
					long logfileGenerationFromFilePath = EseHelper.GetLogfileGenerationFromFilePath(text4, config.LogFilePrefix);
					if (logfileGenerationFromFilePath >= fileState.LowestGenerationRequired)
					{
						ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>(0L, "MoveLogFiles(): Cannot move logfile '{0}' because it is required by the database.", text4);
					}
					else
					{
						File.Move(text4, Path.Combine(text2, path));
					}
				}
				foreach (string text5 in Directory.GetFiles(destinationLogPath, "*.jsl"))
				{
					text3 = text5;
					path = Path.GetFileName(text5);
					File.Move(text5, Path.Combine(text2, path));
				}
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>(0L, "Moved log files successfully from '{0}'", config.DestinationLogPath);
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string, IOException>(0L, "Moving file '{0}' to '{1}' failed with exception: {2}", text3, text2, ex2);
				setBroken.SetBroken(ReplicaInstance.IOExceptionToFailureTag(ex2), ReplayEventLogConstants.Tuple_CouldNotDeleteLogFile, ex2, new string[]
				{
					text3,
					ex2.ToString()
				});
			}
			catch (UnauthorizedAccessException ex3)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string, UnauthorizedAccessException>(0L, "Moving file '{0}' to '{1}' failed with exception: {2}", text3, text2, ex3);
				setBroken.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_CouldNotDeleteLogFile, ex3, new string[]
				{
					text3,
					ex3.ToString()
				});
			}
		}

		public const string SkippedLogsFolderName = "SkippedLogs";
	}
}
