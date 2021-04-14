using System;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Cluster.Replay.IO;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class LastLogReplacer
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.IncrementalReseederTracer;
			}
		}

		public static void ReplaceLastLog(IReplayConfiguration config, long logGen, string oldFilePath, string newFilePath)
		{
			string targetLogFilePath = EseHelper.MakeLogFilePath(config, logGen, config.DestinationLogPath);
			string fileName = Path.GetFileName(targetLogFilePath);
			string tempNewFilePath = Path.Combine(config.DestinationLogPath, fileName + ".ReplacementNew");
			string tempOldFilePath = oldFilePath + ".ReplacementOld";
			string e00LogPath = EseHelper.MakeLogFilePath(config, 0L, config.DestinationLogPath);
			Exception ex = LastLogReplacer.HandleExceptions(delegate
			{
				if (!File.Exists(oldFilePath))
				{
					DiagCore.AssertOrWatson(false, "ReplaceLastLog(): File '{0}' not found!", new object[]
					{
						oldFilePath
					});
					throw new LastLogReplacementFailedFileNotFoundException(config.DisplayName, oldFilePath, e00LogPath);
				}
				if (!File.Exists(newFilePath))
				{
					DiagCore.AssertOrWatson(false, "ReplaceLastLog(): File '{0}' not found!", new object[]
					{
						newFilePath
					});
					throw new LastLogReplacementFailedFileNotFoundException(config.DisplayName, newFilePath, e00LogPath);
				}
				if (File.Exists(tempNewFilePath))
				{
					DiagCore.AssertOrWatson(false, "ReplaceLastLog(): File '{0}' was not expected to be found. Was RollbackLastLogIfNecessary() successfully called?", new object[]
					{
						tempNewFilePath
					});
					throw new LastLogReplacementFailedUnexpectedFileFoundException(config.DisplayName, tempNewFilePath, e00LogPath);
				}
				if (File.Exists(tempOldFilePath))
				{
					DiagCore.AssertOrWatson(false, "ReplaceLastLog(): File '{0}' was not expected to be found. Was RollbackLastLogIfNecessary() successfully called?", new object[]
					{
						tempOldFilePath
					});
					throw new LastLogReplacementFailedUnexpectedFileFoundException(config.DisplayName, tempOldFilePath, e00LogPath);
				}
				File.Move(newFilePath, tempNewFilePath);
				File.Move(oldFilePath, tempOldFilePath);
				File.Move(tempNewFilePath, targetLogFilePath);
				LastLogReplacer.MoveTempOldFileToBackupDir(config, tempOldFilePath);
			});
			if (ex != null)
			{
				throw new LastLogReplacementFailedErrorException(config.DisplayName, e00LogPath, ex.Message, ex);
			}
		}

		public static void RollbackLastLogIfNecessary(IReplayConfiguration config)
		{
			Exception ex = LastLogReplacer.HandleExceptions(delegate
			{
				int num = 0;
				int num2 = 0;
				string text = null;
				string text2 = null;
				string text3 = EseHelper.MakeLogFilePath(config, 0L, config.DestinationLogPath);
				string filter = LastLogReplacer.BuildLogFileSearchPattern(config.LogFilePrefix, ".ReplacementNew");
				string filter2 = LastLogReplacer.BuildLogFileSearchPattern(config.LogFilePrefix, ".ReplacementOld");
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(new DirectoryInfo(config.DestinationLogPath), false, false))
				{
					foreach (string text4 in directoryEnumerator.EnumerateFiles(filter, null))
					{
						num++;
						text = text4;
					}
					if (num > 1)
					{
						throw new LastLogReplacementTooManyTempFilesException(config.DisplayName, filter, num, config.DestinationLogPath);
					}
					foreach (string text5 in directoryEnumerator.EnumerateFiles(filter2, null))
					{
						num2++;
						text2 = text5;
					}
					if (num2 > 1)
					{
						throw new LastLogReplacementTooManyTempFilesException(config.DisplayName, filter2, num2, config.DestinationLogPath);
					}
				}
				if (num == 0 && num2 == 0)
				{
					LastLogReplacer.Tracer.TraceDebug<string>(0L, "RollbackLastLogIfNecessary(): '{0}': Case 1: Nothing to do.", config.DisplayName);
					return;
				}
				if (num == 1 && num2 == 0)
				{
					LastLogReplacer.Tracer.TraceDebug<string, string>(0L, "RollbackLastLogIfNecessary(): '{0}': Case 2: Considering to delete temp new file: {1}", config.DisplayName, text);
					LastLogReplacer.DeleteTempNewFile(config, text, text3);
					return;
				}
				if (num == 1 && num2 == 1)
				{
					LastLogReplacer.Tracer.TraceDebug(0L, "RollbackLastLogIfNecessary(): '{0}': Case 3: Rolling back temp file '{1}' to '{2}' and considering to delete temp new file: {3}", new object[]
					{
						config.DisplayName,
						text2,
						text3,
						text
					});
					File.Move(text2, text3);
					LastLogReplacer.DeleteTempNewFile(config, text, text3);
					return;
				}
				if (num == 0 && num2 == 1)
				{
					LastLogReplacer.Tracer.TraceDebug<string, string>(0L, "RollbackLastLogIfNecessary(): '{0}': Case 4: Considering moving out temp old file '{1}' to IgnoredLogs directory.", config.DisplayName, text2);
					LastLogReplacer.DeleteTempOldFile(config, text2);
					return;
				}
				LastLogReplacer.Tracer.TraceError<string, int, int>(0L, "RollbackLastLogIfNecessary(): '{0}': Unexpected temporary files found. tempOldFileCount={1}, tempNewFileCount={2}, e00Exists=false", config.DisplayName, num2, num);
				throw new LastLogReplacementUnexpectedTempFilesException(config.DisplayName, config.DestinationLogPath);
			});
			if (ex != null)
			{
				throw new LastLogReplacementRollbackFailedException(config.DisplayName, ex.Message, ex);
			}
		}

		public static bool AreTemporaryFilesPresent(string logFilePrefix, string destinationLogPath, out Exception exception)
		{
			bool fTempFilesPresent = false;
			exception = LastLogReplacer.HandleExceptions(delegate
			{
				string filter = LastLogReplacer.BuildLogFileSearchPattern(logFilePrefix, ".ReplacementNew");
				string filter2 = LastLogReplacer.BuildLogFileSearchPattern(logFilePrefix, ".ReplacementOld");
				DirectoryInfo directoryInfo = new DirectoryInfo(destinationLogPath);
				if (!directoryInfo.Exists)
				{
					fTempFilesPresent = false;
					return;
				}
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(directoryInfo, false, false))
				{
					fTempFilesPresent = (directoryEnumerator.EnumerateFiles(filter, null).Count<string>() > 0 || directoryEnumerator.EnumerateFiles(filter2, null).Count<string>() > 0);
				}
			});
			return exception == null && fTempFilesPresent;
		}

		public static Exception CleanupTemporaryFiles(string logFilePrefix, string destinationLogPath, out Exception exception)
		{
			exception = LastLogReplacer.HandleExceptions(delegate
			{
				string filter = LastLogReplacer.BuildLogFileSearchPattern(logFilePrefix, ".ReplacementNew");
				string filter2 = LastLogReplacer.BuildLogFileSearchPattern(logFilePrefix, ".ReplacementOld");
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(new DirectoryInfo(destinationLogPath), false, false))
				{
					foreach (string path in directoryEnumerator.EnumerateFiles(filter, null))
					{
						File.Delete(path);
					}
					foreach (string path2 in directoryEnumerator.EnumerateFiles(filter2, null))
					{
						File.Delete(path2);
					}
				}
			});
			return exception;
		}

		private static void DeleteTempOldFile(IReplayConfiguration config, string tempOldFile)
		{
			Exception ex = LastLogReplacer.HandleExceptions(delegate
			{
				long logfileGeneration = EseHelper.GetLogfileGeneration(tempOldFile);
				string text = EseHelper.MakeLogFilePath(config, logfileGeneration, config.DestinationLogPath);
				if (!File.Exists(text))
				{
					string text2 = EseHelper.MakeLogFilePath(config, 0L, config.DestinationLogPath);
					if (!File.Exists(text2))
					{
						DiagCore.AssertOrWatson(false, "DeleteTempOldFile(): tempOldFile '{0}' has generation {1}, but corresponding numbered log '{2}' was not found!", new object[]
						{
							tempOldFile,
							logfileGeneration,
							text
						});
						throw new LastLogReplacementUnexpectedTempFilesException(config.DisplayName, config.DestinationLogPath);
					}
					text = text2;
				}
				LastLogReplacer.Tracer.TraceDebug<string, string>(0L, "DeleteTempOldFile(): '{0}': Found previously replaced file to be: {1}", config.DisplayName, text);
				bool flag = EseHelper.IsLogfileSubset(text, tempOldFile, config.E00LogBackupPath, null, null);
				DiagCore.AssertOrWatson(flag, "LastLogReplacer.RollbackLastLogIfNecessary(): File '{0}' is not a subset of replaced numbered log file '{1}', as expected!", new object[]
				{
					tempOldFile,
					text
				});
				if (!flag)
				{
					throw new LastLogReplacementFileNotSubsetException(config.DisplayName, tempOldFile, text);
				}
				LastLogReplacer.MoveTempOldFileToBackupDir(config, tempOldFile);
			});
			if (ex != null)
			{
				throw new LastLogReplacementTempOldFileNotDeletedException(config.DisplayName, tempOldFile, ex.Message, ex);
			}
		}

		private static void DeleteTempNewFile(IReplayConfiguration config, string tempNewFile, string subsetFile)
		{
			Exception ex = LastLogReplacer.HandleExceptions(delegate
			{
				bool flag = EseHelper.IsLogfileSubset(tempNewFile, subsetFile, config.E00LogBackupPath, null, null);
				DiagCore.AssertOrWatson(flag, "LastLogReplacer.RollbackLastLogIfNecessary(): File '{0}' is not a subset of temporary new file '{1}', as expected!", new object[]
				{
					subsetFile,
					tempNewFile
				});
				if (!flag)
				{
					throw new LastLogReplacementFileNotSubsetException(config.DisplayName, subsetFile, tempNewFile);
				}
				File.Delete(tempNewFile);
				ReplayCrimsonEvents.FileDeleted.Log<string, string, string>(config.DatabaseName, config.ServerName, tempNewFile);
			});
			if (ex != null)
			{
				throw new LastLogReplacementTempNewFileNotDeletedException(config.DisplayName, tempNewFile, ex.Message, ex);
			}
		}

		private static void MoveTempOldFileToBackupDir(IReplayConfiguration config, string tempOldFile)
		{
			string currentDateString = FileOperations.GetCurrentDateString();
			string text = Path.Combine(config.E00LogBackupPath, currentDateString + "E00OutOfDate");
			string text2 = EseHelper.MakeLogFilePath(config, 0L, text);
			FileOperations.CreateDirectoryIfNeeded(text);
			LastLogReplacer.Tracer.TraceDebug<string, string, string>(0L, "MoveTempOldFileToBackupDir(): '{0}': Moving temporary old file '{1}' to backup file '{2}'", config.DisplayName, tempOldFile, text2);
			File.Move(tempOldFile, text2);
		}

		private static string BuildLogFileSearchPattern(string logFilePrefix, string suffix)
		{
			return string.Format("{0}*{1}", logFilePrefix, suffix);
		}

		private static Exception HandleExceptions(Action operation)
		{
			Exception result = null;
			try
			{
				operation();
			}
			catch (EsentErrorException ex)
			{
				result = ex;
			}
			catch (IOException ex2)
			{
				result = ex2;
			}
			catch (SecurityException ex3)
			{
				result = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				result = ex4;
			}
			return result;
		}

		private const string NewLogFileSuffix = ".ReplacementNew";

		private const string OldLogFileSuffix = ".ReplacementOld";
	}
}
