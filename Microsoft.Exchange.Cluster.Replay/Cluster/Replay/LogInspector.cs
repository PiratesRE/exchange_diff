using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogInspector : ShipControl, IInspectLog
	{
		protected override long MaxGapSize
		{
			get
			{
				return 100L;
			}
		}

		protected override void CheckForGaps(long fromNumber)
		{
			this.m_countdownToGapTest = 6L;
			string text = Path.Combine(this.FromDir, base.GetFilenameFromGenerationNumber(fromNumber));
			string searchPattern = this.Config.LogFilePrefix + "*." + this.Config.LogExtension;
			string[] files = Directory.GetFiles(this.FromDir, searchPattern);
			if (files.Length > 0)
			{
				string text2 = files[0];
				if (SharedHelper.StringIEquals(this.Config.LogFilePrefix, Path.GetFileNameWithoutExtension(text2)))
				{
					if (files.Length == 1)
					{
						return;
					}
					text2 = files[1];
				}
				if (File.Exists(text))
				{
					ExTraceGlobals.LogInspectorTracer.TraceError<long>((long)this.GetHashCode(), "CheckForGaps() shouldn't be called since fromNumber {0} generation is there.", fromNumber);
					return;
				}
				ExTraceGlobals.LogInspectorTracer.TraceError<string>((long)this.GetHashCode(), "Gap in log file generations after file {0}", text);
				string text3 = string.Format("LogGap:'{0}' Length={1}", text2, files.Length);
				this.m_setBroken.SetBroken(FailureTag.AlertOnly, ReplayEventLogConstants.Tuple_LogInspectorFailed, new string[]
				{
					text,
					text3
				});
			}
		}

		public LogInspector(IPerfmonCounters perfmonCounters, IReplayConfiguration replayConfig, string logfileBaseName, string logfileSuffix, string replayDir, FileState fileState, ILogTruncater logTruncater, ISetBroken setBroken, ISetGeneration setGeneration, IReplicaProgress replicaProgress, NetworkPath netPath) : this(perfmonCounters, replayConfig, logfileBaseName, logfileSuffix, replayDir, fileState, logTruncater, setBroken, setGeneration, replicaProgress, netPath, false)
		{
		}

		public LogInspector(IPerfmonCounters perfmonCounters, IReplayConfiguration replayConfig, string logfileBaseName, string logfileSuffix, string replayDir, FileState fileState, ILogTruncater logTruncater, ISetBroken setBroken, ISetGeneration setGeneration, IReplicaProgress replicaProgress, NetworkPath netPath, bool runningAcll) : base(replayConfig.LogInspectorPath, logfileBaseName, 0L, logfileSuffix, setBroken, replicaProgress)
		{
			this.Config = replayConfig;
			this.m_logTruncater = logTruncater;
			this.m_replayDir = replayDir;
			this.m_fileState = fileState;
			this.m_setGeneration = setGeneration;
			this.m_logVerifier = new LogVerifier(logfileBaseName);
			ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "LogInspector initialized - inspectDir = {0}, logfileBaseName = {1}, replayDir = {2}, fileState = {3}", new object[]
			{
				replayConfig.LogInspectorPath,
				logfileBaseName,
				replayDir,
				fileState
			});
			ExTraceGlobals.PFDTracer.TracePfd((long)this.GetHashCode(), "LogInspector initialized - inspectDir = {0}, logfileBaseName = {1}, replayDir = {2}, fileState = {3}", new object[]
			{
				replayConfig.LogInspectorPath,
				logfileBaseName,
				replayDir,
				fileState
			});
			this.Config.ReplayState.InspectorGenerationNumber = this.m_fileState.HighestGenerationPresent;
			this.m_logSource = LogSource.Construct(replayConfig, perfmonCounters, netPath, LogSource.GetLogShipTimeoutInMsec(runningAcll));
			if (0L != this.m_fileState.HighestGenerationPresent)
			{
				this.m_logContinuityChecker.Initialize(this.m_fileState.HighestGenerationPresent, this.Config.DestinationLogPath, this.Config.LogFilePrefix, logfileSuffix);
			}
		}

		public void SetLogShipACLLTimeoutMs()
		{
			this.m_logSource.SetTimeoutInMsec(LogSource.GetLogShipTimeoutInMsec(true));
		}

		public ILogTruncater LogTruncater
		{
			get
			{
				return this.m_logTruncater;
			}
		}

		internal bool UseSuspendLock
		{
			set
			{
				this.m_fUseSuspendLock = value;
			}
		}

		private LogVerifier LogVerifier
		{
			get
			{
				return this.m_logVerifier;
			}
		}

		public bool InspectLog(long logfileNumber, bool fRecopyOnFailure)
		{
			LocalizedString localizedString;
			return this.InspectLog(logfileNumber, fRecopyOnFailure, out localizedString);
		}

		public bool InspectLog(long logfileNumber, bool fRecopyOnFailure, out LocalizedString errorString)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2246454589U);
			errorString = LocalizedString.Empty;
			bool result;
			lock (this)
			{
				try
				{
					result = this.InspectLogInternal(logfileNumber, fRecopyOnFailure);
				}
				catch (FileCheckException ex)
				{
					ExTraceGlobals.LogInspectorTracer.TraceError<FileCheckException>((long)this.GetHashCode(), "FileCheckException thrown in InspectLogInternal. Exception is: {0}", ex);
					this.m_setBroken.SetBroken(ReplicaInstance.TargetFileCheckerExceptionToFailureTag(ex), ReplayEventLogConstants.Tuple_FileCheckError, ex, new string[]
					{
						ex.ToString()
					});
					result = false;
				}
				catch (EsentErrorException ex2)
				{
					base.GetFilenameFromGenerationNumber(logfileNumber);
					this.m_setBroken.SetBroken(ReplicaInstance.TargetIsamErrorExceptionToFailureTag(ex2), ReplayEventLogConstants.Tuple_IsamException, ex2, new string[]
					{
						ex2.ToString(),
						logfileNumber.ToString()
					});
					result = false;
				}
				catch (OperationAbortedException)
				{
					ExTraceGlobals.LogInspectorTracer.TraceError((long)this.GetHashCode(), "Inspector is now stopping (PrepareToStopCalled).");
					result = false;
				}
				finally
				{
					if (this.m_setBroken.IsBroken)
					{
						errorString = this.m_setBroken.ErrorMessage;
					}
					if (base.PrepareToStopCalled)
					{
						errorString = ReplayStrings.PrepareToStopCalled;
					}
				}
			}
			return result;
		}

		public override Result ShipAction(long logfileNumber)
		{
			StateLock stateLock = null;
			if (this.m_fUseSuspendLock)
			{
				stateLock = this.Config.ReplayState.SuspendLock;
				LockOwner lockOwner;
				if (!stateLock.TryEnter(LockOwner.Component, false, out lockOwner))
				{
					ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "unable to get SuspendLock, restarting the instance");
					if (lockOwner != LockOwner.AttemptCopyLastLogs)
					{
						ExTraceGlobals.LogInspectorTracer.TraceDebug<LockOwner>((long)this.GetHashCode(), "unable to get SuspendLock, current owner: {0}, stopping the instance", lockOwner);
						this.m_setBroken.RestartInstanceSoon(true);
					}
					else
					{
						this.m_setBroken.RestartInstanceSoon(false);
					}
					return Result.LongWaitRetry;
				}
			}
			Result result;
			try
			{
				result = (this.InspectLog(logfileNumber, true) ? Result.Success : Result.GiveUp);
			}
			finally
			{
				if (stateLock != null)
				{
					stateLock.Leave(LockOwner.Component);
				}
			}
			return result;
		}

		public override void LogError(string inputFile, Exception ex)
		{
			ExTraceGlobals.LogInspectorTracer.TraceError<string, Exception>((long)this.GetHashCode(), "LogInspector error Input file is: {0}" + Environment.NewLine + "Exception received was: {1}", inputFile, ex);
		}

		public bool InspectFinalLogsDuringAcll(out LocalizedString errorString)
		{
			ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "InspectFinalLogsDuringAcll() called");
			errorString = LocalizedString.Empty;
			this.m_fAttemptFinalCopyCalled = true;
			lock (this)
			{
				if (base.ShiplogsActive)
				{
					ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "Another ShipLogs() is running, creating GoingIdleEvent");
					base.GoingIdleEvent = new ManualResetEvent(false);
				}
			}
			if (base.GoingIdleEvent != null)
			{
				ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "InspectFinalLogsDuringAcll is waiting on GoingIdleEvent");
				base.GoingIdleEvent.WaitOne();
				ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "InspectFinalLogsDuringAcll: GoingIdleEvent was set");
			}
			ExTraceGlobals.LogInspectorTracer.TraceDebug((long)this.GetHashCode(), "InspectFinalLogsDuringAcll: now running ShipLogs()");
			base.ShipLogs(true);
			if (this.m_setBroken.IsBroken)
			{
				errorString = this.m_setBroken.ErrorMessage;
				return false;
			}
			if (base.PrepareToStopCalled)
			{
				errorString = ReplayStrings.PrepareToStopCalled;
				return false;
			}
			return true;
		}

		internal static bool CheckLogHeader(long logfileNumber, string logfileInspect, FileState fileState, LogContinuityChecker continuityChecker, out LocalizedString error)
		{
			error = LocalizedString.Empty;
			bool result;
			try
			{
				result = LogInspector.CheckLogHeaderInternal(logfileNumber, logfileInspect, fileState, continuityChecker, out error);
			}
			catch (EsentErrorException ex)
			{
				ReplayEventLogConstants.Tuple_IsamException.LogEvent(logfileInspect, new object[]
				{
					"unknown",
					ex.Message,
					logfileInspect
				});
				result = false;
			}
			return result;
		}

		internal static bool VerifyLogTask(long logfileNumber, string logfileInspect, FileState fileState, LogVerifier logVerifier, LogContinuityChecker continuityChecker, out LocalizedString error)
		{
			error = LocalizedString.Empty;
			if (logVerifier == null)
			{
				throw new ArgumentNullException("logVerifier");
			}
			EsentErrorException ex = logVerifier.Verify(logfileInspect);
			if (ex != null)
			{
				error = new LocalizedString(ex.Message);
				return false;
			}
			return LogInspector.CheckLogHeaderInternal(logfileNumber, logfileInspect, fileState, continuityChecker, out error);
		}

		private static void ReportCorruptLog(string logFilename, string errorMsg)
		{
			ReplayEventLogConstants.Tuple_InspectorDetectedCorruptLog.LogEvent(logFilename, new object[]
			{
				logFilename,
				errorMsg
			});
		}

		internal static bool VerifyLogStatic(long logfileNumber, LogSource logSource, string logfileInspect, bool fRecopyOnFailure, FileState filestate, LogVerifier logVerifier, LogContinuityChecker continuityChecker, LogInspector.CheckStopDelegate checkStopPending, out LocalizedString error)
		{
			ulong num = fRecopyOnFailure ? 3UL : 1UL;
			bool flag = false;
			bool flag2 = false;
			error = LocalizedString.Empty;
			for (ulong num2 = 0UL; num2 < num; num2 += 1UL)
			{
				bool flag3 = false;
				if (flag2 && fRecopyOnFailure)
				{
					LogInspector.RecopyCorruptLog(logfileNumber, logSource, logfileInspect);
					flag3 = true;
				}
				checkStopPending();
				EsentErrorException ex = logVerifier.Verify(logfileInspect);
				if (ex != null)
				{
					ExTraceGlobals.LogInspectorTracer.TraceError<string, EsentErrorException>(0L, "Inspection of logfile {0} failed: {1}", logfileInspect, ex);
					error = new LocalizedString(ex.Message);
					if (!(ex is EsentFileAccessDeniedException))
					{
						flag2 = true;
						LogInspector.ReportCorruptLog(logfileInspect, error);
					}
				}
				else
				{
					flag2 = false;
					checkStopPending();
					flag = LogInspector.CheckLogHeader(logfileNumber, logfileInspect, filestate, continuityChecker, out error);
					if (!flag)
					{
						LogInspector.ReportCorruptLog(logfileInspect, error);
					}
					else
					{
						if (flag3)
						{
							ReplayEventLogConstants.Tuple_InspectorFixedCorruptLog.LogEvent(logfileInspect, new object[]
							{
								logfileInspect
							});
							break;
						}
						break;
					}
				}
			}
			return flag;
		}

		internal void SetReportingCallback(ISetBroken setBroken)
		{
			this.m_shipLogsSetBroken.SetReportingCallbacksForAcll(setBroken, null);
		}

		protected override void TestDelaySleep()
		{
			int logInspectorDelayInMsec = RegistryParameters.LogInspectorDelayInMsec;
			if (logInspectorDelayInMsec > 0)
			{
				Thread.Sleep(logInspectorDelayInMsec);
			}
		}

		protected override Result InitializeStartContext()
		{
			return Result.Success;
		}

		protected override void PrepareToStopInternal()
		{
			ExTraceGlobals.LogInspectorTracer.TraceError((long)this.GetHashCode(), "PrepareToStopInternal called");
		}

		protected override void StopInternal()
		{
			ExTraceGlobals.LogInspectorTracer.TraceError((long)this.GetHashCode(), "StopInternal called");
			this.m_logSource.Cancel();
			this.LogVerifier.Term();
		}

		private static void RecopyCorruptLog(long logfileNumber, LogSource logSource, string logfileInspect)
		{
			ExTraceGlobals.LogInspectorTracer.TraceError<string>(0L, "Initiating re-copy of corrupt logfile {0}", logfileInspect);
			logSource.CopyLog(logfileNumber, logfileInspect);
		}

		private static bool CheckLogHeaderInternal(long logfileNumber, string logfileInspect, FileState fileState, LogContinuityChecker continuityChecker, out LocalizedString error)
		{
			error = LocalizedString.Empty;
			JET_LOGINFOMISC jet_LOGINFOMISC;
			long logfileGeneration = EseHelper.GetLogfileGeneration(logfileInspect, out jet_LOGINFOMISC);
			if (0L == logfileNumber && 0L != fileState.HighestGenerationPresent && fileState.HighestGenerationPresent + 1L != logfileGeneration)
			{
				ExTraceGlobals.LogInspectorTracer.TraceError<string, long, long>(0L, "Inspection of logfile {0} detected corruption (generation number {1} mismatch with highest logfile present {2})", logfileInspect, logfileGeneration, fileState.HighestGenerationPresent);
				error = ReplayStrings.LogInspectorE00OutOfSequence(logfileInspect, logfileGeneration, fileState.HighestGenerationPresent);
				return false;
			}
			if (logfileNumber != 0L && logfileNumber != logfileGeneration)
			{
				ExTraceGlobals.LogInspectorTracer.TraceError<string, long>(0L, "Inspection of logfile {0} detected corruption (generation number {1} mismatch)", logfileInspect, logfileGeneration);
				error = ReplayStrings.LogInspectorGenerationMismatch(logfileInspect, logfileGeneration, logfileNumber);
				return false;
			}
			if (fileState.LogfileSignature != null)
			{
				if (!jet_LOGINFOMISC.signLog.Equals(fileState.LogfileSignature.Value))
				{
					ExTraceGlobals.LogInspectorTracer.TraceError<string, JET_SIGNATURE, JET_SIGNATURE>(0L, "Inspection of logfile {0} detected a signature mismatch (signature is {1}, expected {2})", logfileInspect, jet_LOGINFOMISC.signLog, fileState.LogfileSignature.Value);
					error = ReplayStrings.LogInspectorSignatureMismatch(logfileInspect, logfileGeneration);
					return false;
				}
			}
			else
			{
				ExTraceGlobals.LogInspectorTracer.TraceDebug<JET_SIGNATURE, string>(0L, "Setting logfile signature to {0} from logfile {1}", jet_LOGINFOMISC.signLog, logfileInspect);
				fileState.LogfileSignature = new JET_SIGNATURE?(jet_LOGINFOMISC.signLog);
			}
			if (!continuityChecker.Examine(jet_LOGINFOMISC, logfileInspect, out error))
			{
				ExTraceGlobals.LogInspectorTracer.TraceError<string, LocalizedString>(0L, "Inspection of logfile {0} detected discontinuity: {1}", logfileInspect, error);
				return false;
			}
			return true;
		}

		private bool InspectLogInternal(long logfileNumber, bool fRecopyOnFailure)
		{
			ExTraceGlobals.LogInspectorTracer.TraceDebug<long>((long)this.GetHashCode(), "LogInspector called, logfileNumber = {0}", logfileNumber);
			ExTraceGlobals.PFDTracer.TracePfd<int, long>((long)this.GetHashCode(), "PFD CRS {0} LogInspector called, logfileNumber = {1}", 26269, logfileNumber);
			string filenameFromGenerationNumber = base.GetFilenameFromGenerationNumber(logfileNumber);
			string text = Path.Combine(this.FromDir, filenameFromGenerationNumber);
			string logfileReplay = Path.Combine(this.m_replayDir, filenameFromGenerationNumber);
			bool result = false;
			try
			{
				this.VerifyLog(logfileNumber, text, fRecopyOnFailure);
				this.MoveLogfile(logfileNumber, text, logfileReplay);
				if (!this.m_fAttemptFinalCopyCalled && this.LogTruncater != null)
				{
					this.LogTruncater.RecordInspectorGeneration(logfileNumber);
				}
				long num;
				if (logfileNumber != 0L)
				{
					num = logfileNumber;
				}
				else
				{
					num = this.m_fileState.HighestGenerationPresent + 1L;
				}
				this.UpdateGenerationPresent(num);
				if (!this.m_firstLogReported)
				{
					ReplayEventLogConstants.Tuple_ReplicaInstanceLogCopied.LogEvent(null, new object[]
					{
						this.Config.Name,
						this.Config.ServerName,
						Environment.MachineName,
						num
					});
					this.m_firstLogReported = true;
				}
				this.m_replicaProgress.ReportOneLogCopied();
				result = true;
			}
			catch (LogInspectorFailedException ex)
			{
				if (ex.InnerException is IOException)
				{
					this.m_setBroken.SetBroken(ReplicaInstance.IOExceptionToFailureTag((IOException)ex.InnerException), ReplayEventLogConstants.Tuple_LogInspectorFailed, ex, new string[]
					{
						text,
						ex.Message
					});
				}
				else
				{
					this.m_setBroken.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogInspectorFailed, ex, new string[]
					{
						text,
						ex.Message
					});
				}
			}
			return result;
		}

		private void CheckStopPending()
		{
			if (base.PrepareToStopCalled)
			{
				throw new OperationAbortedException();
			}
		}

		private void VerifyLog(long logfileNumber, string logfileInspect, bool fRecopyOnFailure)
		{
			LocalizedString value;
			if (!LogInspector.VerifyLogStatic(logfileNumber, this.m_logSource, logfileInspect, fRecopyOnFailure, this.m_fileState, this.m_logVerifier, this.m_logContinuityChecker, new LogInspector.CheckStopDelegate(this.CheckStopPending), out value))
			{
				ExTraceGlobals.LogInspectorTracer.TraceError<string>((long)this.GetHashCode(), "LogInspector has failed on {0} too many times, giving up...", logfileInspect);
				throw new LogInspectorFailedException(value);
			}
		}

		private void UpdateGenerationPresent(long inspectedGeneration)
		{
			long highestGenerationPresent = Math.Max(this.m_fileState.HighestGenerationPresent, inspectedGeneration);
			long lowestGenerationPresent;
			if (0L == this.m_fileState.LowestGenerationPresent)
			{
				lowestGenerationPresent = inspectedGeneration;
			}
			else
			{
				lowestGenerationPresent = Math.Min(this.m_fileState.LowestGenerationPresent, inspectedGeneration);
			}
			this.m_fileState.SetLowestAndHighestGenerationsPresent(lowestGenerationPresent, highestGenerationPresent);
			this.m_fileState.InternalCheckLogfileSignature();
		}

		private void MoveLogfile(long logfileNumber, string logfileInspect, string logfileReplay)
		{
			try
			{
				if (File.Exists(logfileReplay))
				{
					ExTraceGlobals.LogInspectorTracer.TraceDebug<string>((long)this.GetHashCode(), "Deleting pre-existing logfile {0}", logfileReplay);
					ExTraceGlobals.PFDTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD CRS {0} Deleting pre-existing logfile {1}", 32731, logfileReplay);
					File.Delete(logfileReplay);
				}
				DateTime filetimeOfLog = base.GetFiletimeOfLog(logfileInspect);
				ExTraceGlobals.LogInspectorTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Moving inspected logfile {0} to {1}", logfileInspect, logfileReplay);
				ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} Moving inspected logfile {1} to {2}", 18077, logfileInspect, logfileReplay);
				File.Move(logfileInspect, logfileReplay);
				this.m_setGeneration.SetInspectGeneration(logfileNumber, filetimeOfLog);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.LogInspectorTracer.TraceError<string, string, IOException>((long)this.GetHashCode(), "Fail to move logfile from {0} to {1} with exception {2}", logfileInspect, logfileReplay, ex);
				throw new LogInspectorCouldNotMoveLogFileException(logfileInspect, logfileReplay, ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string, UnauthorizedAccessException>((long)this.GetHashCode(), "Fail to move logfile from {0} to {1} with exception {2}", logfileInspect, logfileReplay, ex2);
				throw new LogInspectorCouldNotMoveLogFileException(logfileInspect, logfileReplay, ex2.Message, ex2);
			}
		}

		private const ulong RecopyRetries = 3UL;

		private const int DelLogFileDelay = 500;

		internal readonly IReplayConfiguration Config;

		private readonly string m_replayDir;

		private readonly FileState m_fileState;

		private readonly LogVerifier m_logVerifier;

		private readonly ILogTruncater m_logTruncater;

		private bool m_fUseSuspendLock = true;

		private LogSource m_logSource;

		private bool m_firstLogReported;

		private LogContinuityChecker m_logContinuityChecker = new LogContinuityChecker();

		private ISetGeneration m_setGeneration;

		internal delegate void CheckStopDelegate();
	}
}
