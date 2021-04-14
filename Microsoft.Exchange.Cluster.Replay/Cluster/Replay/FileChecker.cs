using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class FileChecker : IFileChecker
	{
		public FileChecker(string name, string logfileDirectory, string systemDirectory, string logfilePrefix, string logfileSuffix, string databaseFullFilePath, Guid databaseGuid)
		{
			this.m_name = name;
			this.m_logfileDirectory = logfileDirectory;
			this.m_systemDirectory = systemDirectory;
			this.m_logfilePrefix = logfilePrefix;
			this.m_logfileSuffix = logfileSuffix;
			this.m_databaseFullFilePath = databaseFullFilePath;
			this.m_databaseGuid = databaseGuid;
			this.m_stopCalled = false;
			this.m_fileState = new FileState();
			this.Reset();
		}

		public void TryUpdateActiveDatabaseLogfileSignature()
		{
			if (File.Exists(this.m_databaseFullFilePath))
			{
				JET_DBINFOMISC jet_DBINFOMISC = new JET_DBINFOMISC();
				if (FileChecker.GetActiveDatabaseFileInfo(this.m_databaseFullFilePath, this.m_name, this.m_databaseGuid, ref jet_DBINFOMISC))
				{
					this.FileState.LogfileSignature = new JET_SIGNATURE?(jet_DBINFOMISC.signLog);
				}
			}
		}

		public bool RunChecks()
		{
			return this.RunChecks(null);
		}

		public bool RunChecks(LogRepair repair)
		{
			return this.RunChecks(repair, false);
		}

		public bool RunChecks(LogRepair repair, bool forceDeleteCheckPointFile)
		{
			bool result;
			try
			{
				this.CheckPrepareToStop();
				this.Reset();
				this.CheckDatabase();
				this.CheckPrepareToStop();
				this.GetLowestAndHighestGenerationNumbers();
				this.LogFileState();
				this.CheckPrepareToStop();
				if (repair != null)
				{
					repair.BeginRepairAttempt();
					repair.CheckForDivergence(this.FileState);
				}
				this.CheckLogfiles(repair);
				this.CheckPrepareToStop();
				this.CheckE00Log();
				this.CheckPrepareToStop();
				this.GetDatabaseBackupInfo();
				this.CheckPrepareToStop();
				this.CheckCheckpoint(forceDeleteCheckPointFile);
				this.CheckPrepareToStop();
				this.InternalCheck();
				this.InternalCheckLogfileSignature();
				ExTraceGlobals.FileCheckerTracer.TraceDebug<FileState>((long)this.GetHashCode(), "RunChecks is successful. FileState is: {0}", this.FileState);
				result = true;
			}
			catch (OperationAbortedException)
			{
				ExTraceGlobals.FileCheckerTracer.TraceDebug((long)this.GetHashCode(), "RunChecks was cancelled");
				this.Reset();
				result = false;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new FileCheckIOErrorException(ex.Message, ex);
			}
			catch (IOException ex2)
			{
				throw new FileCheckIOErrorException(ex2.Message, ex2);
			}
			catch (EsentErrorException ex3)
			{
				throw new FileCheckIsamErrorException(ex3.Message, ex3);
			}
			return result;
		}

		public bool RecalculateRequiredGenerations(ref JET_DBINFOMISC dbinfo)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug<FileState>((long)this.GetHashCode(), "Entering RecalculateRequiredGenerations. FileState is: {0}", this.FileState);
			this.InternalCheck();
			this.CheckDatabase(ref dbinfo);
			this.GetDatabaseBackupInfo(ref dbinfo);
			this.InternalCheck();
			ExTraceGlobals.FileCheckerTracer.TraceDebug<FileState>((long)this.GetHashCode(), "Leaving RecalculateRequiredGenerations. FileState is: {0}", this.FileState);
			return true;
		}

		public bool RecalculateRequiredGenerations()
		{
			bool result;
			try
			{
				ExTraceGlobals.FileCheckerTracer.TraceDebug<FileState>((long)this.GetHashCode(), "Entering RecalculateRequiredGenerations. FileState is: {0}", this.FileState);
				this.CheckDatabase();
				this.GetDatabaseBackupInfo();
				ExTraceGlobals.FileCheckerTracer.TraceDebug<FileState>((long)this.GetHashCode(), "Leaving RecalculateRequiredGenerations. FileState is: {0}", this.FileState);
				result = true;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new FileCheckIOErrorException(ex.Message, ex);
			}
			catch (IOException ex2)
			{
				throw new FileCheckIOErrorException(ex2.Message, ex2);
			}
			catch (EsentErrorException ex3)
			{
				throw new FileCheckIsamErrorException(ex3.Message, ex3);
			}
			return result;
		}

		public bool CheckRequiredLogfilesForPassiveOrInconsistentDatabase(bool checkForReplay)
		{
			if (checkForReplay)
			{
				FileChecker.InternalCheck(this.FileState.LogfileSignature != null, "Logfile signature should be set", new object[0]);
			}
			try
			{
				bool flag = false;
				long lowestGenerationRequired = this.FileState.LowestGenerationRequired;
				long num = this.FileState.HighestGenerationRequired;
				JET_LOGINFOMISC jet_LOGINFOMISC;
				if (this.TryGetE00LogInfo(out jet_LOGINFOMISC) && (long)jet_LOGINFOMISC.ulGeneration == num)
				{
					flag = true;
					num -= 1L;
					if (lowestGenerationRequired > num)
					{
						this.CheckE00Log();
						return true;
					}
				}
				this.CheckLogfiles(lowestGenerationRequired, num, null);
				if (flag)
				{
					this.CheckE00Log();
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new FileCheckIOErrorException(ex.Message, ex);
			}
			catch (IOException ex2)
			{
				throw new FileCheckIOErrorException(ex2.Message, ex2);
			}
			catch (EsentErrorException ex3)
			{
				throw new FileCheckIsamErrorException(ex3.Message, ex3);
			}
			catch (OperationAbortedException)
			{
				return false;
			}
			return true;
		}

		public bool CheckRequiredLogFilesForDatabaseMountable()
		{
			try
			{
				if (this.FileState.ConsistentDatabase)
				{
					ExTraceGlobals.FileCheckerTracer.TraceDebug<string>((long)this.GetHashCode(), "CheckRequiredLogFilesForDatabaseMountable(): Database '{0}': EDB is consistent, so skipping required log file validation.", this.m_name);
					return true;
				}
				if (!this.FileState.RequiredLogfilesArePresent())
				{
					ExTraceGlobals.FileCheckerTracer.TraceError((long)this.GetHashCode(), "CheckRequiredLogFilesForDatabaseMountable(): Database '{0}': Required logfiles are not present. Replay requires generations {1} through {2}. Found generations {3} through {4}. Replay cannot be run until all required logfiles are present.", new object[]
					{
						this.m_name,
						this.FileState.LowestGenerationRequired,
						this.FileState.HighestGenerationRequired,
						this.FileState.LowestGenerationPresent,
						this.FileState.HighestGenerationPresent
					});
					return false;
				}
				if (!this.CheckRequiredLogfilesForPassiveOrInconsistentDatabase(false))
				{
					return false;
				}
			}
			catch (FileCheckLogfileMissingException arg)
			{
				ExTraceGlobals.FileCheckerTracer.TraceError<string, FileCheckLogfileMissingException>((long)this.GetHashCode(), "CheckRequiredLogFilesForDatabaseMountable(): Database '{0}' : CheckRequiredLogs got FileCheckLogfileMissingException: {1}. It will return false", this.m_name, arg);
				return false;
			}
			return true;
		}

		public void CheckCheckpoint()
		{
			this.CheckCheckpoint(false);
		}

		public void CheckCheckpoint(bool forceDeleteCheckPointFile)
		{
			this.InternalCheck();
			try
			{
				string text = Path.Combine(this.m_systemDirectory, EseHelper.MakeCheckpointFileName(this.m_logfilePrefix));
				if (!File.Exists(text))
				{
					return;
				}
				FileChecker.DeleteCheckpointFileReason deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.Unknown;
				long num = 0L;
				try
				{
					num = EseHelper.DumpCheckpoint(text);
				}
				catch (EsentCheckpointCorruptException)
				{
					deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.Corrupted;
				}
				catch (EsentDiskIOException)
				{
					deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.Corrupted;
				}
				if (deleteCheckpointFileReason == FileChecker.DeleteCheckpointFileReason.Unknown)
				{
					if (forceDeleteCheckPointFile)
					{
						ExTraceGlobals.FileCheckerTracer.TraceDebug((long)this.GetHashCode(), "Forcefully deleting the checkpoint file.");
						deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.Force;
					}
					else if (0L != this.FileState.LowestGenerationRequired && num > this.FileState.LowestGenerationRequired && !this.FileState.ConsistentDatabase)
					{
						deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.TooAdvanced;
					}
					else if (0L != this.FileState.LowestGenerationPresent && this.FileState.LowestGenerationPresent <= this.FileState.LowestGenerationRequired && num < this.FileState.LowestGenerationPresent)
					{
						deleteCheckpointFileReason = FileChecker.DeleteCheckpointFileReason.TooFarBehindAndLogMissing;
					}
					else
					{
						this.FileState.CheckpointGeneration = num;
					}
				}
				if (deleteCheckpointFileReason != FileChecker.DeleteCheckpointFileReason.Unknown)
				{
					this.FileState.CheckpointGeneration = 0L;
					this.DeleteCheckpoint(text, num, deleteCheckpointFileReason);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new FileCheckIOErrorException(ex.Message, ex);
			}
			catch (IOException ex2)
			{
				throw new FileCheckIOErrorException(ex2.Message, ex2);
			}
			catch (EsentErrorException ex3)
			{
				throw new FileCheckIsamErrorException(ex3.Message, ex3);
			}
			this.InternalCheck();
		}

		public void PrepareToStop()
		{
			this.m_stopCalled = true;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "FileState: {0}", new object[]
			{
				this.FileState
			});
		}

		public FileState FileState
		{
			get
			{
				return this.m_fileState;
			}
		}

		private static void TestInternalCheck()
		{
			FileChecker.InternalCheck(true, "This InternalCheck should not fire", new object[0]);
			try
			{
				FileChecker.InternalCheck(false, "This InternalCheck should fire", new object[0]);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckInternalErrorException", new object[0]);
			}
			catch (FileCheckInternalErrorException)
			{
			}
		}

		private static void TestNoFiles(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestNoFiles");
			fileChecker.Reset();
			bool flag = fileChecker.RunChecks();
			DiagCore.RetailAssert(flag, "FileChecker.RunChecks returned {0}", new object[]
			{
				flag
			});
			DiagCore.RetailAssert(0L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.LastGenerationBackedUp, "LastGenerationBackedUp should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.CheckpointGeneration, "CheckpointGeneration should be 0.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature == null, "LogfileSignature should not be set.", new object[0]);
		}

		private static void TestCreateFakeLogfile(string logDirectory, string logfilePrefix, string logfileSuffix, long generation)
		{
			string path = Path.Combine(logDirectory, EseHelper.MakeLogfileName(logfilePrefix, logfileSuffix, generation));
			using (File.Create(path))
			{
			}
		}

		private static void TestGetLogFileInfo()
		{
			try
			{
				FileChecker.GetLogFileInfo("nonexistantlog.log");
				DiagCore.RetailAssert(false, "Should have thrown EsentFileNotFoundException.", new object[0]);
			}
			catch (EsentFileNotFoundException)
			{
			}
			string tempFileName = Path.GetTempFileName();
			try
			{
				FileChecker.GetLogFileInfo(tempFileName);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckCorruptFileException)
			{
			}
			File.Delete(tempFileName);
		}

		private static void TestGetPassiveDatabaseFileInfo()
		{
			try
			{
				FileChecker.GetPassiveDatabaseFileInfo("nonexistantdatabase.edb", "nonexistantdatabase", Guid.Empty, null);
				DiagCore.RetailAssert(false, "Should have thrown EsentFileNotFoundException.", new object[0]);
			}
			catch (EsentFileNotFoundException)
			{
			}
			string tempFileName = Path.GetTempFileName();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(tempFileName);
			try
			{
				FileChecker.GetPassiveDatabaseFileInfo(tempFileName, fileNameWithoutExtension, Guid.Empty, null);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckCorruptFileException)
			{
			}
			File.Delete(tempFileName);
		}

		private static void TestGetLowestAndHighestGenerationNumbers(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestGetLowestAndHighestGenerationNumbers");
			fileChecker.Reset();
			string logfilePrefix = fileChecker.m_logfilePrefix;
			string logfileSuffix = fileChecker.m_logfileSuffix;
			string logfileDirectory = fileChecker.m_logfileDirectory;
			fileChecker.GetLowestAndHighestGenerationNumbers();
			DiagCore.RetailAssert(0L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent should be 0.", new object[0]);
			FileChecker.TestCreateFakeLogfile(logfileDirectory, logfilePrefix, logfileSuffix, 1048570L);
			fileChecker.GetLowestAndHighestGenerationNumbers();
			DiagCore.RetailAssert(1048570L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent not set.", new object[0]);
			DiagCore.RetailAssert(1048570L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent not set.", new object[0]);
			fileChecker.Reset();
			for (long num = 1048571L; num <= 1048643L; num += 1L)
			{
				FileChecker.TestCreateFakeLogfile(logfileDirectory, logfilePrefix, logfileSuffix, num);
			}
			fileChecker.GetLowestAndHighestGenerationNumbers();
			DiagCore.RetailAssert(1048570L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent not set.", new object[0]);
			DiagCore.RetailAssert(1048643L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent not set.", new object[0]);
			fileChecker.Reset();
			DiagCore.RetailAssert(0L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent not reset.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent not reset.", new object[0]);
			string path = Path.Combine(logfileDirectory, EseHelper.MakeLogfileName(logfilePrefix, logfileSuffix, 1048606L));
			File.Delete(path);
			fileChecker.GetLowestAndHighestGenerationNumbers();
			DiagCore.RetailAssert(1048570L == fileChecker.FileState.LowestGenerationPresent, "LowestGenerationPresent not set.", new object[0]);
			DiagCore.RetailAssert(1048643L == fileChecker.FileState.HighestGenerationPresent, "HighestGenerationPresent not set.", new object[0]);
		}

		private static void TestCheckLogfileGeneration(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckLogfileGeneration");
			fileChecker.Reset();
			JET_LOGINFOMISC jet_LOGINFOMISC = new JET_LOGINFOMISC();
			jet_LOGINFOMISC.ulGeneration = 7;
			FileChecker.CheckLogfileGeneration("test.log", 7L, jet_LOGINFOMISC);
			try
			{
				FileChecker.CheckLogfileGeneration("test.log", 6L, jet_LOGINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckException)
			{
			}
		}

		private static void TestCheckLogfileCreationTime(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckLogfileCreationTime");
			fileChecker.Reset();
			DateTime utcNow = DateTime.UtcNow;
			JET_LOGINFOMISC jet_LOGINFOMISC = new JET_LOGINFOMISC();
			jet_LOGINFOMISC.logtimePreviousGeneration = new JET_LOGTIME(utcNow);
			FileChecker.CheckLogfileCreationTime("test.log", utcNow, ref jet_LOGINFOMISC);
			try
			{
				FileChecker.CheckLogfileCreationTime("test.log", utcNow.AddSeconds(10.0), ref jet_LOGINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckException)
			{
			}
		}

		private static void TestSetLogfileSignature(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestSetLogfileSignature");
			fileChecker.Reset();
			JET_SIGNATURE jet_SIGNATURE = new JET_SIGNATURE(102965, new DateTime?(DateTime.UtcNow), "localhost");
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature == null, "Logfile signature is already set.", new object[0]);
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature != null, "Logfile signature isn't set.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature.Equals(jet_SIGNATURE), "Logfile signature not set properly.", new object[0]);
		}

		private static void TestCheckLogfileSignature(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckLogfileSignature");
			fileChecker.Reset();
			JET_SIGNATURE value = new JET_SIGNATURE(1, new DateTime?(DateTime.UtcNow), "lokalhost");
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(value);
			JET_LOGINFOMISC jet_LOGINFOMISC = new JET_LOGINFOMISC();
			jet_LOGINFOMISC.signLog = fileChecker.FileState.LogfileSignature.Value;
			fileChecker.CheckLogfileSignature("test.log", ref jet_LOGINFOMISC);
			try
			{
				JET_SIGNATURE signLog = new JET_SIGNATURE((int)(jet_LOGINFOMISC.signLog.ulRandom + 7U), jet_LOGINFOMISC.signLog.logtimeCreate.ToDateTime(), "localhost");
				jet_LOGINFOMISC.signLog = signLog;
				fileChecker.CheckLogfileSignature("test.log", ref jet_LOGINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckException)
			{
			}
		}

		private static void TestCheckDatabaseLogfileSignature(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckDatabaseLogfileSignature");
			fileChecker.Reset();
			JET_SIGNATURE value = new JET_SIGNATURE(65535, new DateTime?(DateTime.UtcNow), "localhost");
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(value);
			JET_DBINFOMISC jet_DBINFOMISC = new JET_DBINFOMISC();
			jet_DBINFOMISC.dbstate = JET_dbstate.DirtyShutdown;
			jet_DBINFOMISC.signLog = fileChecker.FileState.LogfileSignature.Value;
			fileChecker.CheckDatabaseLogfileSignature("test.edb", ref jet_DBINFOMISC);
			try
			{
				JET_SIGNATURE signLog = new JET_SIGNATURE((int)(jet_DBINFOMISC.signLog.ulRandom - 68U), jet_DBINFOMISC.signLog.logtimeCreate.ToDateTime(), "localhost");
				jet_DBINFOMISC.signLog = signLog;
				fileChecker.CheckDatabaseLogfileSignature("test.edb", ref jet_DBINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckException)
			{
			}
		}

		private static void TestStop(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestStop");
			fileChecker.Reset();
			fileChecker.PrepareToStop();
			bool flag = fileChecker.RunChecks();
			DiagCore.RetailAssert(!flag, "PrepareToStop() was called but RunChecks() completed.", new object[0]);
		}

		private static void TestCheckDatabase(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckDatabase");
			fileChecker.Reset();
			JET_DBINFOMISC jet_DBINFOMISC = new JET_DBINFOMISC();
			jet_DBINFOMISC.dbstate = JET_dbstate.JustCreated;
			try
			{
				fileChecker.CheckDatabase(ref jet_DBINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException.", new object[0]);
			}
			catch (FileCheckException)
			{
			}
			fileChecker.Reset();
			jet_DBINFOMISC.dbstate = JET_dbstate.CleanShutdown;
			fileChecker.CheckDatabase(ref jet_DBINFOMISC);
			JET_SIGNATURE jet_SIGNATURE = new JET_SIGNATURE(0, new DateTime?(DateTime.UtcNow), "localhost");
			jet_DBINFOMISC.dbstate = JET_dbstate.DirtyShutdown;
			jet_DBINFOMISC.signLog = jet_SIGNATURE;
			jet_DBINFOMISC.genMinRequired = 19;
			jet_DBINFOMISC.genMaxRequired = 21;
			fileChecker.CheckDatabase(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(19L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be set.", new object[0]);
			DiagCore.RetailAssert(21L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be set.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature != null, "Logfile signature isn't set.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature.Equals(jet_SIGNATURE), "Logfile signature not set properly.", new object[0]);
			fileChecker.Reset();
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			jet_DBINFOMISC.genMinRequired = 114;
			jet_DBINFOMISC.genMaxRequired = 114;
			fileChecker.CheckDatabase(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(114L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be set.", new object[0]);
			DiagCore.RetailAssert(114L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be set.", new object[0]);
			fileChecker.Reset();
			jet_DBINFOMISC.dbstate = JET_dbstate.DirtyShutdown;
			jet_DBINFOMISC.genMinRequired = 37;
			jet_DBINFOMISC.genMaxRequired = 35;
			try
			{
				fileChecker.CheckDatabase(ref jet_DBINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException", new object[0]);
			}
			catch (FileCheckException)
			{
			}
			fileChecker.Reset();
			jet_DBINFOMISC.dbstate = JET_dbstate.DirtyShutdown;
			jet_DBINFOMISC.genMinRequired = 0;
			jet_DBINFOMISC.genMaxRequired = 0;
			try
			{
				fileChecker.CheckDatabase(ref jet_DBINFOMISC);
				DiagCore.RetailAssert(false, "Should have thrown FileCheckException", new object[0]);
			}
			catch (FileCheckException)
			{
			}
		}

		private static void TestCheckE00Log(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestCheckE00Log");
			fileChecker.Reset();
			JET_SIGNATURE jet_SIGNATURE = new JET_SIGNATURE(344865, new DateTime?(DateTime.UtcNow), "localhost");
			JET_LOGINFOMISC jet_LOGINFOMISC = new JET_LOGINFOMISC();
			jet_LOGINFOMISC.signLog = jet_SIGNATURE;
			fileChecker.CheckE00Log("test\\E00.log", ref jet_LOGINFOMISC);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature != null, "Logfile signature isn't set.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LogfileSignature.Equals(jet_SIGNATURE), "Logfile signature not set properly.", new object[0]);
			fileChecker.CheckE00Log("test\\E00.log", ref jet_LOGINFOMISC);
		}

		private static void TestGetDatabaseBackupInfo(FileChecker fileChecker)
		{
			ExTraceGlobals.FileCheckerTracer.TraceDebug(0L, "FileChecker.TestGetDatabaseBackupInfo");
			fileChecker.Reset();
			JET_SIGNATURE jet_SIGNATURE = new JET_SIGNATURE(1545, new DateTime?(DateTime.UtcNow), "localhost");
			JET_DBINFOMISC jet_DBINFOMISC = new JET_DBINFOMISC();
			jet_DBINFOMISC.signLog = jet_SIGNATURE;
			jet_DBINFOMISC.dbstate = JET_dbstate.CleanShutdown;
			jet_DBINFOMISC.bkinfoFullPrev = new JET_BKINFO
			{
				genHigh = 9,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			jet_DBINFOMISC.bkinfoIncPrev = new JET_BKINFO
			{
				genHigh = 1,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			jet_DBINFOMISC.bkinfoDiffPrev = new JET_BKINFO
			{
				genHigh = 4,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			jet_DBINFOMISC.bkinfoCopyPrev = new JET_BKINFO
			{
				genHigh = 6,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			JET_LGPOS lgposConsistent = jet_DBINFOMISC.lgposConsistent;
			jet_DBINFOMISC.lgposConsistent = new JET_LGPOS
			{
				ib = lgposConsistent.ib,
				isec = lgposConsistent.isec,
				lGeneration = 12
			};
			fileChecker.GetDatabaseBackupInfo(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(0L == fileChecker.FileState.LastGenerationBackedUp, "Logfile signature not set but LastGenerationBackedUp is", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 0.", new object[0]);
			DiagCore.RetailAssert(0L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 0.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestFullBackupTime == null, "LatestFullBackupTime should not be set", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestIncrementalBackupTime == null, "LatestIncrementalBackupTime should not be set", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestDifferentialBackupTime == null, "LatestDifferentialBackupTime should not be set", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestCopyBackupTime == null, "LatestCopyBackupTime should not be set", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.SnapshotBackup == null, "SnapshotBackup should not be set", new object[0]);
			fileChecker.Reset();
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			jet_DBINFOMISC.dbstate = JET_dbstate.CleanShutdown;
			jet_DBINFOMISC.bkinfoFullPrev = new JET_BKINFO
			{
				genHigh = 5,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			jet_DBINFOMISC.bkinfoIncPrev = new JET_BKINFO
			{
				genHigh = 0,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow.AddSeconds(10.0), false)
			};
			jet_DBINFOMISC.bkinfoDiffPrev = new JET_BKINFO
			{
				genHigh = 0,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			jet_DBINFOMISC.bkinfoCopyPrev = new JET_BKINFO
			{
				genHigh = 6,
				bklogtimeMark = new JET_BKLOGTIME(DateTime.UtcNow, false)
			};
			lgposConsistent = jet_DBINFOMISC.lgposConsistent;
			jet_DBINFOMISC.lgposConsistent = new JET_LGPOS
			{
				ib = lgposConsistent.ib,
				isec = lgposConsistent.isec,
				lGeneration = 11
			};
			fileChecker.GetDatabaseBackupInfo(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(5L == fileChecker.FileState.LastGenerationBackedUp, "LastGenerationBackedUp is {0}, expected 5", new object[]
			{
				fileChecker.FileState.LastGenerationBackedUp
			});
			DiagCore.RetailAssert(11L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 11.", new object[0]);
			DiagCore.RetailAssert(11L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 11.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestFullBackupTime == jet_DBINFOMISC.bkinfoFullPrev.bklogtimeMark.ToDateTime(), "LatestFullBackupTime not set correctly ({0}, expected {1})", new object[]
			{
				fileChecker.FileState.LatestFullBackupTime,
				jet_DBINFOMISC.bkinfoFullPrev.bklogtimeMark
			});
			DiagCore.RetailAssert(fileChecker.FileState.LatestIncrementalBackupTime == null, "LatestIncrementalBackupTime should not be set", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestDifferentialBackupTime == jet_DBINFOMISC.bkinfoDiffPrev.bklogtimeMark.ToDateTime(), "LatestDifferentialBackupTime not set correctly ({0}, expected {1})", new object[]
			{
				fileChecker.FileState.LatestDifferentialBackupTime,
				jet_DBINFOMISC.bkinfoDiffPrev.bklogtimeMark
			});
			DiagCore.RetailAssert(fileChecker.FileState.LatestCopyBackupTime == jet_DBINFOMISC.bkinfoCopyPrev.bklogtimeMark.ToDateTime(), "LatestCopyBackupTime not set correctly ({0}, expected {1})", new object[]
			{
				fileChecker.FileState.LatestCopyBackupTime,
				jet_DBINFOMISC.bkinfoCopyPrev.bklogtimeMark
			});
			fileChecker.Reset();
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			jet_DBINFOMISC.dbstate = JET_dbstate.CleanShutdown;
			JET_BKINFO bkinfoFullPrev = jet_DBINFOMISC.bkinfoFullPrev;
			bkinfoFullPrev.genHigh = 5;
			jet_DBINFOMISC.bkinfoFullPrev = bkinfoFullPrev;
			JET_BKINFO bkinfoIncPrev = jet_DBINFOMISC.bkinfoIncPrev;
			bkinfoIncPrev.genHigh = 4;
			jet_DBINFOMISC.bkinfoIncPrev = bkinfoIncPrev;
			JET_LGPOS lgposConsistent2 = jet_DBINFOMISC.lgposConsistent;
			lgposConsistent2.lGeneration = 11;
			jet_DBINFOMISC.lgposConsistent = lgposConsistent2;
			fileChecker.GetDatabaseBackupInfo(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(5L == fileChecker.FileState.LastGenerationBackedUp, "LastGenerationBackedUp is {0}, expected 5", new object[]
			{
				fileChecker.FileState.LastGenerationBackedUp
			});
			DiagCore.RetailAssert(11L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 11.", new object[0]);
			DiagCore.RetailAssert(11L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 11.", new object[0]);
			DiagCore.RetailAssert(fileChecker.FileState.LatestFullBackupTime == jet_DBINFOMISC.bkinfoFullPrev.bklogtimeMark.ToDateTime(), "LatestFullBackupTime not set correctly ({0}, expected {1})", new object[]
			{
				fileChecker.FileState.LatestFullBackupTime,
				jet_DBINFOMISC.bkinfoFullPrev.bklogtimeMark
			});
			DiagCore.RetailAssert(fileChecker.FileState.LatestIncrementalBackupTime == jet_DBINFOMISC.bkinfoIncPrev.bklogtimeMark.ToDateTime(), "LatestIncrementalBackupTime not set correctly ({0}, expected {1})", new object[]
			{
				fileChecker.FileState.LatestIncrementalBackupTime,
				jet_DBINFOMISC.bkinfoIncPrev.bklogtimeMark
			});
			fileChecker.Reset();
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			jet_DBINFOMISC.dbstate = JET_dbstate.CleanShutdown;
			bkinfoFullPrev = jet_DBINFOMISC.bkinfoFullPrev;
			bkinfoFullPrev.genHigh = 5;
			jet_DBINFOMISC.bkinfoFullPrev = bkinfoFullPrev;
			bkinfoIncPrev = jet_DBINFOMISC.bkinfoIncPrev;
			bkinfoIncPrev.genHigh = 7;
			jet_DBINFOMISC.bkinfoIncPrev = bkinfoIncPrev;
			lgposConsistent2 = jet_DBINFOMISC.lgposConsistent;
			lgposConsistent2.lGeneration = 11;
			jet_DBINFOMISC.lgposConsistent = lgposConsistent2;
			fileChecker.GetDatabaseBackupInfo(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(7L == fileChecker.FileState.LastGenerationBackedUp, "LastGenerationBackedUp is {0}, expected 7", new object[]
			{
				fileChecker.FileState.LastGenerationBackedUp
			});
			DiagCore.RetailAssert(11L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 11.", new object[0]);
			DiagCore.RetailAssert(11L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 11.", new object[0]);
			fileChecker.Reset();
			fileChecker.FileState.LogfileSignature = new JET_SIGNATURE?(jet_SIGNATURE);
			jet_DBINFOMISC.dbstate = JET_dbstate.DirtyShutdown;
			jet_DBINFOMISC.genMinRequired = 39;
			jet_DBINFOMISC.genMaxRequired = 40;
			bkinfoFullPrev = jet_DBINFOMISC.bkinfoFullPrev;
			bkinfoFullPrev.genHigh = 8;
			jet_DBINFOMISC.bkinfoFullPrev = bkinfoFullPrev;
			bkinfoIncPrev = jet_DBINFOMISC.bkinfoIncPrev;
			bkinfoIncPrev.genHigh = 8;
			jet_DBINFOMISC.bkinfoIncPrev = bkinfoIncPrev;
			lgposConsistent2 = jet_DBINFOMISC.lgposConsistent;
			lgposConsistent2.lGeneration = 11;
			jet_DBINFOMISC.lgposConsistent = lgposConsistent2;
			fileChecker.CheckDatabase(ref jet_DBINFOMISC);
			fileChecker.GetDatabaseBackupInfo(ref jet_DBINFOMISC);
			DiagCore.RetailAssert(8L == fileChecker.FileState.LastGenerationBackedUp, "LastGenerationBackedUp is {0}, expected 8", new object[]
			{
				fileChecker.FileState.LastGenerationBackedUp
			});
			DiagCore.RetailAssert(39L == fileChecker.FileState.LowestGenerationRequired, "LowestGenerationRequired should be 39.", new object[0]);
			DiagCore.RetailAssert(40L == fileChecker.FileState.HighestGenerationRequired, "HighestGenerationRequired should be 40.", new object[0]);
		}

		private static JET_LOGINFOMISC GetLogFileInfo(string logfilePath)
		{
			JET_LOGINFOMISC result;
			try
			{
				JET_LOGINFOMISC jet_LOGINFOMISC;
				UnpublishedApi.JetGetLogFileInfo(logfilePath, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
				result = jet_LOGINFOMISC;
			}
			catch (EsentFileAccessDeniedException innerException)
			{
				throw new FileCheckAccessDeniedException(logfilePath, innerException);
			}
			catch (EsentLogFileCorruptException ex)
			{
				throw new FileCheckCorruptFileException(logfilePath, ex.Message, ex);
			}
			catch (EsentBadLogVersionException ex2)
			{
				throw new FileCheckCorruptFileException(logfilePath, ex2.Message, ex2);
			}
			catch (EsentFileIOBeyondEOFException ex3)
			{
				throw new FileCheckCorruptFileException(logfilePath, ex3.Message, ex3);
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				throw new FileCheckCorruptFileException(logfilePath, ex4.Message, ex4);
			}
			return result;
		}

		internal static JET_DBINFOMISC GetPassiveDatabaseFileInfo(string databaseFullFilePath, string databaseName, Guid databaseGuid, string localNodeName)
		{
			JET_DBINFOMISC jet_DBINFOMISC = new JET_DBINFOMISC();
			if (string.IsNullOrEmpty(localNodeName))
			{
				localNodeName = Dependencies.ManagementClassHelper.LocalMachineName;
			}
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(localNodeName))
			{
				try
				{
					ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid>(0L, "FileChecker.GetPassiveDatabaseFileInfo: Making LogReplayRequest RPC to store for database {0}", databaseGuid);
					uint arg;
					IPagePatchReply pagePatchReply;
					uint[] array;
					newStoreControllerInstance.LogReplayRequest(databaseGuid, 0U, 0U, out arg, out jet_DBINFOMISC, out pagePatchReply, out array);
					if (jet_DBINFOMISC.ulVersion != 0)
					{
						ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid, uint, JET_DBINFOMISC>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} returned nextLogToReplay={1}, dbinfo={2}", databaseGuid, arg, jet_DBINFOMISC);
						return jet_DBINFOMISC;
					}
					ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} got invalid dbinfo. Trying to dismount the database.", databaseGuid);
				}
				catch (MapiExceptionMdbOffline mapiExceptionMdbOffline)
				{
					ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid, string>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} got a MapiExceptionMdbOffline: {1}.", databaseGuid, mapiExceptionMdbOffline.ToString());
				}
				catch (MapiExceptionNotFound mapiExceptionNotFound)
				{
					ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid, string>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} got a MapiExceptionNotFound: {1}.", databaseGuid, mapiExceptionNotFound.ToString());
				}
				catch (MapiRetryableException ex)
				{
					ExTraceGlobals.FileCheckerTracer.TraceError<Guid, string>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} got a MapiRetryableException: {1}.", databaseGuid, ex.ToString());
				}
				catch (MapiPermanentException ex2)
				{
					ExTraceGlobals.FileCheckerTracer.TraceError<Guid, string>(0L, "FileChecker.GetPassiveDatabaseFileInfo: LogReplayRequest rpc to store for database {0} got a MapiRetryableException: {1}.", databaseGuid, ex2.ToString());
				}
			}
			try
			{
				return FileChecker.GetDatabaseFileInfoFromDisk(databaseFullFilePath);
			}
			catch (FileCheckAccessDeniedException ex3)
			{
				ReplayCrimsonEvents.FileCheckerNeedsToDismountDatabase.Log<string, Guid, string>(databaseName, databaseGuid, ex3.Error);
				Exception ex4 = AmStoreHelper.Dismount(databaseGuid, UnmountFlags.SkipCacheFlush);
				if (ex4 != null)
				{
					ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid, string>(0L, "FileChecker.GetPassiveDatabaseFileInfo: Dismount rpc to store for database {0} failed.", databaseGuid, ex4.ToString());
					throw new FileCheckAccessDeniedDismountFailedException(databaseFullFilePath, ex4.Message, ex3);
				}
			}
			return FileChecker.GetDatabaseFileInfoFromDisk(databaseFullFilePath);
		}

		internal static bool GetActiveDatabaseFileInfo(string databaseFullFilePath, string databaseName, Guid databaseGuid, ref JET_DBINFOMISC dbinfo)
		{
			bool result = false;
			try
			{
				try
				{
					using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(null))
					{
						newStoreControllerInstance.GetDatabaseInformation(databaseGuid, out dbinfo);
						result = true;
					}
				}
				catch (MapiExceptionMdbOffline arg)
				{
					ExTraceGlobals.FileCheckerTracer.TraceError<Guid, MapiExceptionMdbOffline>(0L, "FileChecker.GetActiveDatabaseFileInfo: GetDatabaseInformation rpc to store for database {0} threw exception: {1}", databaseGuid, arg);
					try
					{
						dbinfo = FileChecker.GetDatabaseFileInfoFromDisk(databaseFullFilePath);
						result = true;
					}
					catch (FileCheckAccessDeniedException arg2)
					{
						ExTraceGlobals.FileCheckerTracer.TraceError<string, FileCheckAccessDeniedException>(0L, "FileChecker.GetActiveDatabaseFileInfo: GetDatabaseFileInfoFromDisk for location {0} threw exception: {1}", databaseFullFilePath, arg2);
						using (IStoreRpc newStoreControllerInstance2 = Dependencies.GetNewStoreControllerInstance(null))
						{
							newStoreControllerInstance2.GetDatabaseInformation(databaseGuid, out dbinfo);
							result = true;
						}
					}
				}
			}
			catch (LocalizedException arg3)
			{
				ExTraceGlobals.FileCheckerTracer.TraceError<Guid, LocalizedException>(0L, "FileChecker.GetActiveDatabaseFileInfo rpc to store for database {0} threw exception: {1}", databaseGuid, arg3);
			}
			ExTraceGlobals.FileCheckerTracer.TraceDebug<Guid, JET_DBINFOMISC>(0L, "FileChecker.GetActiveDatabaseFileInfo: GetDatabaseInformation rpc to store for database {0}, dbinfo={1}", databaseGuid, dbinfo);
			return result;
		}

		private static JET_DBINFOMISC GetDatabaseFileInfoFromDisk(string databaseFullFilePath)
		{
			JET_DBINFOMISC result;
			try
			{
				JET_DBINFOMISC jet_DBINFOMISC;
				Api.JetGetDatabaseFileInfo(databaseFullFilePath, out jet_DBINFOMISC, JET_DbInfo.Misc);
				result = jet_DBINFOMISC;
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new FileCheckCorruptFileException(databaseFullFilePath, ex.Message, ex);
			}
			catch (EsentFileAccessDeniedException innerException)
			{
				throw new FileCheckAccessDeniedException(databaseFullFilePath, innerException);
			}
			catch (EsentDiskIOException ex2)
			{
				throw new FileCheckCorruptFileException(databaseFullFilePath, ex2.Message, ex2);
			}
			catch (EsentCorruptionException ex3)
			{
				throw new FileCheckCorruptFileException(databaseFullFilePath, ex3.Message, ex3);
			}
			catch (EsentFileInvalidTypeException ex4)
			{
				throw new FileCheckCorruptFileException(databaseFullFilePath, ex4.Message, ex4);
			}
			return result;
		}

		private static void CheckLogfileGeneration(string logfilePath, long generation, JET_LOGINFOMISC loginfo)
		{
			if ((long)loginfo.ulGeneration != generation)
			{
				throw new FileCheckLogfileGenerationException(logfilePath, (long)loginfo.ulGeneration, generation);
			}
		}

		private static void CheckLogfileCreationTime(string logfilePath, DateTime previousGenerationCreationTime, ref JET_LOGINFOMISC loginfo)
		{
			if (loginfo.logtimePreviousGeneration.ToDateTime() != previousGenerationCreationTime)
			{
				throw new FileCheckLogfileCreationTimeException(logfilePath, loginfo.logtimePreviousGeneration.ToDateTime() ?? DateTime.MinValue, previousGenerationCreationTime);
			}
		}

		private static void InternalCheck(bool condition, string formatString, params object[] messageArgs)
		{
			if (!condition)
			{
				string text = string.Format(formatString, messageArgs);
				string stackTrace = Environment.StackTrace;
				ExTraceGlobals.FileCheckerTracer.TraceError<string, string>(0L, "FileChecker internal check failed. Message is {0}, callstack is {1}", text, stackTrace);
				throw new FileCheckInternalErrorException(text);
			}
		}

		private void Reset()
		{
			this.m_fileState.Reset();
			this.InternalCheck();
		}

		private void GetLowestAndHighestGenerationNumbers()
		{
			this.InternalCheck();
			FileChecker.InternalCheck(0L == this.FileState.LowestGenerationPresent, "LowestGeneration present has been set.", new object[0]);
			FileChecker.InternalCheck(0L == this.FileState.HighestGenerationPresent, "HighestGenerationPresent has been set.", new object[0]);
			DirectoryInfo di = new DirectoryInfo(this.m_logfileDirectory);
			long lowestGenerationPresent = ShipControl.LowestGenerationInDirectory(di, this.m_logfilePrefix, this.m_logfileSuffix, false);
			long highestGenerationPresent = ShipControl.HighestGenerationInDirectory(di, this.m_logfilePrefix, this.m_logfileSuffix);
			this.FileState.SetLowestAndHighestGenerationsPresent(lowestGenerationPresent, highestGenerationPresent);
			this.InternalCheck();
		}

		private void CheckLogfileSignature(string logfilePath, ref JET_LOGINFOMISC loginfo)
		{
			FileChecker.InternalCheck(this.FileState.LogfileSignature != null, "Logfile signature hasn't been set", new object[0]);
			if (!loginfo.signLog.Equals(this.FileState.LogfileSignature))
			{
				throw new FileCheckLogfileSignatureException(logfilePath, loginfo.signLog.ToString(), this.FileState.LogfileSignature.ToString());
			}
		}

		private void CheckDatabaseLogfileSignature(string databasePath, ref JET_DBINFOMISC dbinfo)
		{
			FileChecker.InternalCheck(JET_dbstate.DirtyShutdown == dbinfo.dbstate || (JET_dbstate)7 == dbinfo.dbstate || (JET_dbstate)6 == dbinfo.dbstate, "Expected an inconsistent (Dirty/DirtyAndPatched shutdown) or inc-reseeding database.", new object[0]);
			FileChecker.InternalCheck(this.FileState.LogfileSignature != null, "Logfile signature hasn't been set", new object[0]);
			if (!dbinfo.signLog.Equals(this.FileState.LogfileSignature))
			{
				throw new FileCheckDatabaseLogfileSignatureException(databasePath, dbinfo.signLog.ToString(), this.FileState.LogfileSignature.ToString());
			}
		}

		private void CheckLogfiles(long minimumGeneration, long maximumGeneration, LogRepair repair)
		{
			this.InternalCheck();
			FileChecker.InternalCheck(minimumGeneration <= maximumGeneration, "minimumGeneration({0}) > maximumGeneration({1})", new object[]
			{
				minimumGeneration,
				maximumGeneration
			});
			if (0L == minimumGeneration)
			{
				return;
			}
			ExTraceGlobals.FileCheckerTracer.TraceDebug<long, long>((long)this.GetHashCode(), "Checking logfiles from generation {0} to generation {1}", minimumGeneration, maximumGeneration);
			DateTime? dateTime = null;
			string text = null;
			for (long num = minimumGeneration; num <= maximumGeneration; num += 1L)
			{
				this.CheckPrepareToStop();
				text = Path.Combine(this.m_logfileDirectory, EseHelper.MakeLogfileName(this.m_logfilePrefix, this.m_logfileSuffix, num));
				try
				{
					if (repair != null)
					{
						repair.CheckAndRepair(num);
					}
					JET_LOGINFOMISC logFileInfo = FileChecker.GetLogFileInfo(text);
					if (this.FileState.LogfileSignature == null)
					{
						this.FileState.LogfileSignature = new JET_SIGNATURE?(logFileInfo.signLog);
					}
					this.CheckLogfileSignature(text, ref logFileInfo);
					FileChecker.CheckLogfileGeneration(text, num, logFileInfo);
					if (dateTime != null)
					{
						FileChecker.CheckLogfileCreationTime(text, dateTime.Value, ref logFileInfo);
					}
					dateTime = logFileInfo.logtimeCreate.ToDateTime();
				}
				catch (EsentFileNotFoundException)
				{
					if (!File.Exists(this.m_databaseFullFilePath))
					{
						throw new FileCheckEDBMissingException(this.m_databaseFullFilePath);
					}
					if (this.FileState.IsGenerationInRequiredRange(num) && num < this.FileState.HighestGenerationPresent)
					{
						throw new FileCheckRequiredLogfileGapException(text);
					}
					throw new FileCheckLogfileMissingException(text);
				}
				catch (EsentInvalidPathException)
				{
					throw new FileCheckLogfileMissingException(text);
				}
				catch (EsentErrorException ex)
				{
					throw new LogFileCheckException(text, ex.Message, ex);
				}
				catch (IOException ex2)
				{
					throw new LogFileCheckException(text, ex2.Message, ex2);
				}
				catch (FileCheckCorruptFileException ex3)
				{
					throw new LogFileCheckException(text, ex3.Message, ex3);
				}
				catch (FileCheckException ex4)
				{
					throw new LogFileCheckException(text, ex4.Message, ex4);
				}
			}
			this.InternalCheck();
		}

		private void CheckLogfiles(LogRepair repair)
		{
			if (0L == this.FileState.HighestGenerationPresent)
			{
				return;
			}
			long num;
			long num2;
			if (this.FileState.ConsistentDatabase)
			{
				num = this.FileState.HighestGenerationPresent;
				num2 = num;
			}
			else
			{
				num = Math.Min(this.FileState.HighestGenerationPresent, Math.Max(this.m_committedLogGenDuringCheck, this.FileState.HighestGenerationRequired) + 2L);
				num2 = Math.Min(num, Math.Max(this.FileState.LowestGenerationPresent, this.FileState.LowestGenerationRequired));
				if (repair != null && !this.DoesCheckpointExist())
				{
					num2 = Math.Min(num2, this.FileState.LowestGenerationPresent);
				}
			}
			this.CheckLogfiles(num2, num, repair);
		}

		private void CheckE00Log(string e00LogPath, ref JET_LOGINFOMISC loginfo)
		{
			this.InternalCheck();
			if (0L != this.FileState.HighestGenerationPresent)
			{
				FileChecker.CheckLogfileGeneration(e00LogPath, this.FileState.HighestGenerationPresent + 1L, loginfo);
				FileChecker.InternalCheck(this.FileState.LogfileSignature != null, "Logfile signature should be set.", new object[0]);
				this.CheckLogfileSignature(e00LogPath, ref loginfo);
				FileChecker.CheckLogfileCreationTime(e00LogPath, FileChecker.GetLogFileInfo(Path.Combine(this.m_logfileDirectory, EseHelper.MakeLogfileName(this.m_logfilePrefix, this.m_logfileSuffix, this.FileState.HighestGenerationPresent))).logtimeCreate.ToDateTime() ?? DateTime.MinValue, ref loginfo);
			}
			else if (this.FileState.LogfileSignature == null)
			{
				FileChecker.InternalCheck(0L == this.FileState.LowestGenerationPresent, "Logfiles found but signature not set.", new object[0]);
				FileChecker.InternalCheck(0L == this.FileState.HighestGenerationPresent, "Logfiles found but signature not set.", new object[0]);
				FileChecker.InternalCheck(0L == this.FileState.LowestGenerationRequired, "Logfiles found but signature not set.", new object[0]);
				FileChecker.InternalCheck(0L == this.FileState.LowestGenerationRequired, "Logfiles found but signature not set.", new object[0]);
				this.FileState.LogfileSignature = new JET_SIGNATURE?(loginfo.signLog);
			}
			this.FileState.SetE00LogGeneration((long)loginfo.ulGeneration);
			this.InternalCheck();
		}

		private bool TryGetE00LogInfo(out JET_LOGINFOMISC logInfo)
		{
			this.InternalCheck();
			string text = Path.Combine(this.m_logfileDirectory, EseHelper.MakeLogfileName(this.m_logfilePrefix, this.m_logfileSuffix));
			logInfo = new JET_LOGINFOMISC();
			if (File.Exists(text))
			{
				logInfo = FileChecker.GetLogFileInfo(text);
				return true;
			}
			return false;
		}

		private void CheckE00Log()
		{
			this.InternalCheck();
			string e00LogPath = Path.Combine(this.m_logfileDirectory, EseHelper.MakeLogfileName(this.m_logfilePrefix, this.m_logfileSuffix));
			JET_LOGINFOMISC jet_LOGINFOMISC;
			if (this.TryGetE00LogInfo(out jet_LOGINFOMISC))
			{
				this.CheckE00Log(e00LogPath, ref jet_LOGINFOMISC);
				this.InternalCheck();
				return;
			}
		}

		private void CheckInconsistentDatabase(JET_DBINFOMISC dbinfo)
		{
			FileChecker.InternalCheck(JET_dbstate.DirtyShutdown == dbinfo.dbstate || (JET_dbstate)7 == dbinfo.dbstate || (JET_dbstate)6 == dbinfo.dbstate, "Expected an inconsistent (Dirty/DirtyAndPatched shutdown) or inc-reseeding database.", new object[0]);
			if (this.FileState.LogfileSignature == null)
			{
				this.FileState.LogfileSignature = new JET_SIGNATURE?(dbinfo.signLog);
			}
			if (dbinfo.genMinRequired == 0)
			{
				throw new FileCheckRequiredGenerationCorruptException(this.m_databaseFullFilePath, (long)dbinfo.genMinRequired, (long)dbinfo.genMaxRequired);
			}
			if (dbinfo.genMinRequired > dbinfo.genMaxRequired)
			{
				throw new FileCheckRequiredGenerationCorruptException(this.m_databaseFullFilePath, (long)dbinfo.genMinRequired, (long)dbinfo.genMaxRequired);
			}
			this.FileState.SetLowestAndHighestGenerationsRequired((long)dbinfo.genMinRequired, (long)dbinfo.genMaxRequired, false);
			this.m_committedLogGenDuringCheck = (long)dbinfo.genCommitted;
			this.CheckDatabaseLogfileSignature(this.m_databaseFullFilePath, ref dbinfo);
			FileChecker.InternalCheck(0L != this.FileState.LowestGenerationRequired, "LowestGenerationRequired shouldn't be 0.", new object[0]);
			FileChecker.InternalCheck(0L != this.FileState.HighestGenerationRequired, "HighestGenerationRequired shouldn't be 0.", new object[0]);
			FileChecker.InternalCheck(this.FileState.LogfileSignature != null, "Logfile signature should be set.", new object[0]);
		}

		private void CheckConsistentDatabase(JET_DBINFOMISC dbinfo)
		{
			FileChecker.InternalCheck(JET_dbstate.CleanShutdown == dbinfo.dbstate, "Expected a consistent database.", new object[0]);
			this.FileState.SetLowestAndHighestGenerationsRequired(0L, 0L, true);
		}

		private void CheckDatabase(ref JET_DBINFOMISC dbinfo)
		{
			this.InternalCheck();
			switch (dbinfo.dbstate)
			{
			case JET_dbstate.JustCreated:
				throw new FileCheckJustCreatedEDBException(this.m_databaseFullFilePath);
			case JET_dbstate.DirtyShutdown:
			case (JET_dbstate)6:
			case (JET_dbstate)7:
				this.CheckInconsistentDatabase(dbinfo);
				goto IL_71;
			case JET_dbstate.CleanShutdown:
				this.CheckConsistentDatabase(dbinfo);
				goto IL_71;
			}
			throw new FileCheckInvalidDatabaseStateException(this.m_databaseFullFilePath, dbinfo.dbstate.ToString());
			IL_71:
			this.InternalCheck();
		}

		private void CheckDatabase()
		{
			this.InternalCheck();
			FileChecker.InternalCheck(null != this.m_databaseFullFilePath, "m_database is null", new object[0]);
			if (File.Exists(this.m_databaseFullFilePath))
			{
				JET_DBINFOMISC passiveDatabaseFileInfo = FileChecker.GetPassiveDatabaseFileInfo(this.m_databaseFullFilePath, this.m_name, this.m_databaseGuid, null);
				this.CheckDatabase(ref passiveDatabaseFileInfo);
				this.InternalCheck();
				return;
			}
		}

		private void GetDatabaseBackupInfo(ref JET_DBINFOMISC dbinfo)
		{
			this.InternalCheck();
			if (this.FileState.LogfileSignature != null && this.FileState.LogfileSignature.Equals(dbinfo.signLog))
			{
				if (JET_dbstate.CleanShutdown == dbinfo.dbstate)
				{
					this.FileState.SetLowestAndHighestGenerationsRequired((long)dbinfo.lgposConsistent.lGeneration, (long)dbinfo.lgposConsistent.lGeneration, true);
				}
				else
				{
					FileChecker.InternalCheck(0L != this.FileState.LowestGenerationRequired, "LowestGenerationRequired must be set", new object[0]);
				}
				this.FileState.LastGenerationBackedUp = (long)Math.Max(dbinfo.bkinfoFullPrev.genHigh, dbinfo.bkinfoIncPrev.genHigh);
				if (dbinfo.bkinfoFullPrev.genHigh > 0)
				{
					this.FileState.SnapshotBackup = new bool?(dbinfo.bkinfoFullPrev.bklogtimeMark.fOSSnapshot);
					this.FileState.SnapshotLatestFullBackup = new bool?(dbinfo.bkinfoFullPrev.bklogtimeMark.fOSSnapshot);
					this.FileState.LatestFullBackupTime = dbinfo.bkinfoFullPrev.bklogtimeMark.ToDateTime();
				}
				if (dbinfo.bkinfoIncPrev.genHigh > 0)
				{
					FileChecker.InternalCheck(this.FileState.LatestFullBackupTime == dbinfo.bkinfoFullPrev.bklogtimeMark.ToDateTime(), "LatestFullBackupTime should be set", new object[0]);
					this.FileState.SnapshotLatestIncrementalBackup = new bool?(dbinfo.bkinfoIncPrev.bklogtimeMark.fOSSnapshot);
					this.FileState.LatestIncrementalBackupTime = dbinfo.bkinfoIncPrev.bklogtimeMark.ToDateTime();
				}
				if (dbinfo.bkinfoDiffPrev.genHigh > 0)
				{
					FileChecker.InternalCheck(this.FileState.LatestFullBackupTime == dbinfo.bkinfoFullPrev.bklogtimeMark.ToDateTime(), "LatestFullBackupTime should be set", new object[0]);
					this.FileState.SnapshotLatestDifferentialBackup = new bool?(dbinfo.bkinfoDiffPrev.bklogtimeMark.fOSSnapshot);
					this.FileState.LatestDifferentialBackupTime = dbinfo.bkinfoDiffPrev.bklogtimeMark.ToDateTime();
				}
				if (dbinfo.bkinfoCopyPrev.genHigh > 0)
				{
					this.FileState.SnapshotLatestCopyBackup = new bool?(dbinfo.bkinfoCopyPrev.bklogtimeMark.fOSSnapshot);
					this.FileState.LatestCopyBackupTime = dbinfo.bkinfoCopyPrev.bklogtimeMark.ToDateTime();
				}
			}
			else
			{
				FileChecker.InternalCheck(JET_dbstate.CleanShutdown == dbinfo.dbstate, "Database must be consistent.", new object[0]);
			}
			this.InternalCheck();
		}

		private void GetDatabaseBackupInfo()
		{
			this.InternalCheck();
			FileChecker.InternalCheck(null != this.m_databaseFullFilePath, "m_database is null", new object[0]);
			if (File.Exists(this.m_databaseFullFilePath))
			{
				JET_DBINFOMISC passiveDatabaseFileInfo = FileChecker.GetPassiveDatabaseFileInfo(this.m_databaseFullFilePath, this.m_name, this.m_databaseGuid, null);
				this.GetDatabaseBackupInfo(ref passiveDatabaseFileInfo);
				this.InternalCheck();
				return;
			}
		}

		private bool DoesCheckpointExist()
		{
			string path = Path.Combine(this.m_systemDirectory, EseHelper.MakeCheckpointFileName(this.m_logfilePrefix));
			return File.Exists(path);
		}

		private void DeleteCheckpoint(string checkpointFilePath, long checkpointGeneration, FileChecker.DeleteCheckpointFileReason deleteCheckpointFileReason)
		{
			LocalizedString localizedString = LocalizedString.Empty;
			switch (deleteCheckpointFileReason)
			{
			case FileChecker.DeleteCheckpointFileReason.Force:
				localizedString = ReplayStrings.DeleteChkptReasonForce(checkpointGeneration);
				break;
			case FileChecker.DeleteCheckpointFileReason.Corrupted:
				localizedString = ReplayStrings.DeleteChkptReasonCorrupted;
				break;
			case FileChecker.DeleteCheckpointFileReason.TooAdvanced:
				localizedString = ReplayStrings.DeleteChkptReasonTooAdvanced(checkpointGeneration);
				break;
			case FileChecker.DeleteCheckpointFileReason.TooFarBehindAndLogMissing:
				localizedString = ReplayStrings.DeleteChkptReasonTooFarBehindAndLogMissing(checkpointGeneration);
				break;
			}
			ExTraceGlobals.FileCheckerTracer.TraceError((long)this.GetHashCode(), "Checkpoint file {0} is generation {1} when minimum required generation of database {2} is {3}. Deleting (DeleteCheckpointFileReason={4})...", new object[]
			{
				checkpointFilePath,
				checkpointGeneration,
				this.m_databaseFullFilePath,
				this.FileState.LowestGenerationRequired,
				deleteCheckpointFileReason
			});
			ReplayEventLogConstants.Tuple_CheckpointDeleted.LogEvent(null, new object[]
			{
				this.m_name,
				checkpointFilePath,
				this.m_databaseFullFilePath,
				this.FileState.LowestGenerationRequired,
				localizedString
			});
			try
			{
				File.Delete(checkpointFilePath);
			}
			catch (IOException ex)
			{
				throw new FileCheckUnableToDeleteCheckpointException(checkpointFilePath, ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex2)
			{
				throw new FileCheckUnableToDeleteCheckpointException(checkpointFilePath, ex2.Message, ex2);
			}
		}

		private void InternalCheck()
		{
			this.m_fileState.InternalCheck();
		}

		private void InternalCheckLogfileSignature()
		{
			this.m_fileState.InternalCheckLogfileSignature();
		}

		private void CheckPrepareToStop()
		{
			if (this.m_stopCalled)
			{
				throw new OperationAbortedException();
			}
		}

		private void LogFileState()
		{
			ReplayCrimsonEvents.LogsInRequiredRange.Log<string, string, long, long, long, long>(this.m_name, Environment.MachineName, this.m_fileState.LowestGenerationRequired, this.m_fileState.HighestGenerationRequired, this.m_fileState.LowestGenerationPresent, this.m_fileState.HighestGenerationPresent);
		}

		private readonly string m_name;

		private readonly string m_logfileDirectory;

		private readonly string m_systemDirectory;

		private readonly string m_logfilePrefix;

		private readonly string m_logfileSuffix;

		private readonly string m_databaseFullFilePath;

		private readonly Guid m_databaseGuid;

		private readonly FileState m_fileState;

		private long m_committedLogGenDuringCheck;

		private volatile bool m_stopCalled;

		private enum DeleteCheckpointFileReason
		{
			Unknown,
			Force,
			Corrupted,
			TooAdvanced,
			TooFarBehindAndLogMissing
		}
	}
}
