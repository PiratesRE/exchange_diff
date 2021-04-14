using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ShipControl : IStartStop
	{
		protected virtual long MaxGapSize
		{
			get
			{
				return 10L;
			}
		}

		protected virtual FailureTag TagUsedOnLogGapDetected
		{
			get
			{
				return FailureTag.LogGapFatal;
			}
		}

		protected ShipControl(string fromDir, string fromPrefix, long fromNumber, string fromSuffix, ISetBroken setBroken, IReplicaProgress replicaProgress)
		{
			this.m_className = base.GetType().Name;
			this.m_fromDir = fromDir;
			this.m_fromPrefix = fromPrefix;
			this.m_fromNumber = fromNumber;
			this.m_fromSuffix = fromSuffix;
			this.m_shipLogsSetBroken = new ShipLogsSetBroken(setBroken, null);
			this.m_setBroken = this.m_shipLogsSetBroken;
			this.m_replicaProgress = replicaProgress;
			this.m_countdownToGapTest = 6L;
		}

		protected virtual string FromDir
		{
			get
			{
				return this.m_fromDir;
			}
		}

		protected string FromPrefix
		{
			get
			{
				return this.m_fromPrefix;
			}
		}

		protected long FromNumber
		{
			get
			{
				return this.m_fromNumber;
			}
			set
			{
				this.m_fromNumber = value;
			}
		}

		protected string FromSuffix
		{
			get
			{
				return this.m_fromSuffix;
			}
		}

		protected bool PrepareToStopCalled
		{
			get
			{
				return this.m_prepareToStopCalled;
			}
		}

		protected bool Initialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		protected ManualResetEvent GoingIdleEvent
		{
			get
			{
				return this.m_goingIdleEvent;
			}
			set
			{
				this.m_goingIdleEvent = value;
			}
		}

		protected bool ShiplogsActive
		{
			get
			{
				return this.m_active;
			}
		}

		public static string GenerationString(long generationNumber)
		{
			return string.Format("{0:X8}", generationNumber);
		}

		public static long LowestGenerationInDirectory(DirectoryInfo di, string prefix, string suffix, bool ignoreDirectoryMissing = false)
		{
			long result = 0L;
			if (ignoreDirectoryMissing && !di.Exists)
			{
				return 0L;
			}
			using (EseLogEnumerator eseLogEnumerator = new EseLogEnumerator(di, prefix, suffix))
			{
				result = eseLogEnumerator.FindLowestGeneration();
			}
			return result;
		}

		public static long HighestGenerationInDirectory(DirectoryInfo di, string prefix, string suffix)
		{
			long result = 0L;
			using (EseLogEnumerator eseLogEnumerator = new EseLogEnumerator(di, prefix, suffix))
			{
				string text = eseLogEnumerator.FindHighestGenerationLogFile();
				if (!string.IsNullOrEmpty(text))
				{
					ShipControl.GetGenerationNumberFromFilename(Path.GetFileName(text), prefix, out result);
				}
			}
			return result;
		}

		public static bool GenerationAvailableInDirectory(DirectoryInfo di, string prefix, string suffix, long generation)
		{
			string searchPattern = EseHelper.MakeLogfileName(prefix, suffix, generation);
			FileInfo[] files = di.GetFiles(searchPattern);
			return files.Length != 0;
		}

		public void Start()
		{
			this.m_wakeTimer = new TimerState();
			this.m_wakeTimer.M_thisShip = this;
			TimerCallback callback = new TimerCallback(ShipControl.ShipTimeout);
			this.m_wakeTimer.M_thisTimer = new Timer(callback, this.m_wakeTimer, -1, -1);
			ThreadPool.QueueUserWorkItem(new WaitCallback(ShipControl.ShipWorkThread), this);
		}

		public void Stop()
		{
			ExTraceGlobals.ShipLogTracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Entering Stop()", this.m_className);
			lock (this)
			{
				if (this.m_fStopped)
				{
					return;
				}
				if (!this.m_prepareToStopCalled)
				{
					this.PrepareToStop();
				}
				this.StartOfStopTime = DateTime.UtcNow;
				if (this.m_stopEvent == null && this.m_active)
				{
					ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Creating m_stopEvent because m_active is true", this.m_className);
					this.m_stopEvent = new ManualResetEvent(false);
				}
				ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: ShipLog setting m_active=true in Stop()", this.m_className);
				this.m_active = true;
				if (this.m_goingIdleEvent != null)
				{
					this.m_goingIdleEvent.Set();
				}
				this.m_fStopped = true;
			}
			if (this.m_stopEvent != null)
			{
				ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Dispose wait", this.m_className);
				this.m_stopEvent.WaitOne();
				this.m_stopEvent = null;
				ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Dispose wait finished", this.m_className);
			}
			lock (this)
			{
				if (this.m_wakeTimer != null)
				{
					this.m_wakeTimer.Dispose();
					this.m_wakeTimer = null;
				}
			}
			this.EndOfStopTime = DateTime.UtcNow;
			this.StopInternal();
			this.DisposeWatcherState();
			ExTraceGlobals.ShipLogTracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Exiting Stop()", this.m_className);
		}

		public virtual void ShipNotification(long logFileNumber)
		{
			ExTraceGlobals.ShipLogTracer.TraceDebug<long>((long)this.GetHashCode(), "ShipNotification called, logFileNumber = {0}", logFileNumber);
		}

		public abstract Result ShipAction(long fromNumber);

		protected DateTime PrepareToStopTime { get; set; }

		protected DateTime StartOfStopTime { get; set; }

		protected DateTime EndOfStopTime { get; set; }

		public void PrepareToStop()
		{
			ExTraceGlobals.ShipLogTracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Entering PrepareToStop()", this.m_className);
			if (!this.m_prepareToStopCalled)
			{
				this.PrepareToStopTime = DateTime.UtcNow;
			}
			this.m_prepareToStopCalled = true;
			if (this.m_wakeTimer != null)
			{
				this.m_wakeTimer.M_thisTimer.Change(-1, -1);
			}
			this.PrepareToStopInternal();
			ExTraceGlobals.ShipLogTracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Exiting PrepareToStop()", this.m_className);
		}

		public abstract void LogError(string inputFile, Exception ex);

		protected static bool GetGenerationNumberFromFilename(string filename, string prefix, out long generation)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
			return long.TryParse(fileNameWithoutExtension.Substring(prefix.Length), NumberStyles.HexNumber, null, out generation);
		}

		protected abstract void StopInternal();

		protected void RunFileIoOperation(ref Result result, ShipControl.FileIoOperation operation)
		{
			Result innerResult = result;
			try
			{
				Dependencies.Watson.SendReportOnUnhandledException(delegate
				{
					try
					{
						operation();
					}
					catch (FileNotFoundException lastException8)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException8;
					}
					catch (MapiRetryableException lastException9)
					{
						innerResult = Result.ShortWaitRetry;
						this.m_lastException = lastException9;
					}
					catch (FileSharingViolationOnSourceException lastException10)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException10;
					}
					catch (DirectoryNotFoundException lastException11)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException11;
					}
					catch (IOException lastException12)
					{
						innerResult = Result.ShortWaitRetry;
						this.m_lastException = lastException12;
					}
					catch (WebException lastException13)
					{
						innerResult = Result.ShortWaitRetry;
						this.m_lastException = lastException13;
					}
					catch (NetworkRemoteException lastException14)
					{
						this.m_lastException = lastException14;
						innerResult = Result.ShortWaitRetry;
					}
					catch (NetworkTransportException lastException15)
					{
						this.m_lastException = lastException15;
						innerResult = Result.ShortWaitRetry;
					}
					catch (SecurityException lastException16)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException16;
					}
					catch (UnauthorizedAccessException lastException17)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException17;
					}
					catch (TransientException lastException18)
					{
						innerResult = Result.ShortWaitRetry;
						this.m_lastException = lastException18;
					}
					catch (EsentFileAccessDeniedException lastException19)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException19;
					}
					catch (EsentDiskIOException lastException20)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException20;
					}
					catch (EsentFileNotFoundException lastException21)
					{
						innerResult = Result.LongWaitRetry;
						this.m_lastException = lastException21;
					}
					catch (OperationAbortedException lastException22)
					{
						innerResult = Result.GiveUp;
						this.m_lastException = lastException22;
					}
					catch (OperationCanceledException lastException23)
					{
						innerResult = Result.GiveUp;
						this.m_lastException = lastException23;
					}
					catch (ArgumentException lastException24)
					{
						innerResult = Result.GiveUp;
						this.m_lastException = lastException24;
					}
				});
			}
			catch (PathTooLongException lastException)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException;
			}
			catch (NotSupportedException lastException2)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException2;
			}
			catch (ObjectDisposedException lastException3)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException3;
			}
			catch (EsentLogFileCorruptException lastException4)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException4;
			}
			catch (EsentBadLogVersionException lastException5)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException5;
			}
			catch (EsentFileIOBeyondEOFException lastException6)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException6;
			}
			catch (EsentFileInvalidTypeException lastException7)
			{
				result = Result.GiveUp;
				this.m_lastException = lastException7;
			}
			if (innerResult != Result.Success && result != Result.GiveUp)
			{
				result = innerResult;
			}
		}

		protected void ShipLogs()
		{
			this.ShipLogs(false);
		}

		protected void ShipLogs(bool fAttemptFinalCopy)
		{
			int num = 10000;
			Result result = Result.Success;
			bool fBreakOutOfLoop = false;
			ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Entering ShipLogs()", this.m_className);
			Monitor.Enter(this);
			bool monitorHeld = true;
			if (!this.m_active && !this.PrepareToStopCalled)
			{
				if (this.m_fAttemptFinalCopyCalled)
				{
					if (!fAttemptFinalCopy)
					{
						goto IL_376;
					}
				}
				try
				{
					this.m_active = true;
					string filenameToTry = null;
					if (this.m_wakeTimer != null)
					{
						this.m_wakeTimer.M_thisTimer.Change(-1, -1);
					}
					DirectoryInfo di = null;
					di = new DirectoryInfo(this.FromDir);
					do
					{
						this.m_lastException = null;
						if (!this.m_initialized)
						{
							Monitor.Exit(this);
							monitorHeld = false;
							result = this.ShipRestartFindFirst();
							Monitor.Enter(this);
							monitorHeld = true;
							if (result != Result.Success)
							{
								break;
							}
							this.m_initialized = true;
						}
						this.m_notified = false;
						Monitor.Exit(this);
						monitorHeld = false;
						this.RunFileIoOperation(ref result, delegate
						{
							long fromNumber = this.FromNumber;
							filenameToTry = this.GetFilenameFromGenerationNumber(fromNumber);
							ExTraceGlobals.ShipLogTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "{0}: Trying to find file {1}\\{2}", this.m_className, this.FromDir, filenameToTry);
							bool flag = di.GetFiles(filenameToTry).Length == 1;
							if (flag)
							{
								ExTraceGlobals.ShipLogTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: Found file {1}", this.m_className, filenameToTry);
								ExTraceGlobals.PFDTracer.TracePfd<int, string, string>((long)this.GetHashCode(), "PFD CRS {0} {1}: Found file {2}", 18395, this.m_className, filenameToTry);
								this.m_countdownToGapTest = 6L;
								this.TestDelaySleep();
								result = this.ShipAction(fromNumber);
								Monitor.Enter(this);
								monitorHeld = true;
								if (result != Result.Success)
								{
									fBreakOutOfLoop = true;
									return;
								}
								this.m_lastWait = 0;
								this.m_totalWait = 0;
								this.m_fromNumber += 1L;
								return;
							}
							else
							{
								ShipControl <>4__this = this;
								long countdownToGapTest;
								<>4__this.m_countdownToGapTest = (countdownToGapTest = <>4__this.m_countdownToGapTest) - 1L;
								if (countdownToGapTest == 0L)
								{
									this.CheckForGaps(fromNumber);
								}
								Monitor.Enter(this);
								monitorHeld = true;
								if (this.m_lastWait != 0)
								{
									fBreakOutOfLoop = true;
									result = Result.ShortWaitRetry;
									return;
								}
								this.m_totalWait = 0;
								if (!this.m_notified)
								{
									fBreakOutOfLoop = true;
								}
								return;
							}
						});
						if (fBreakOutOfLoop)
						{
							break;
						}
						if (result != Result.Success)
						{
							ExTraceGlobals.ShipLogTracer.TraceError<Result, Exception>((long)this.GetHashCode(), "ShipLogs failed, result={0}:{1}", result, this.m_lastException);
						}
					}
					while (!this.m_prepareToStopCalled && result == Result.Success);
					if (this.m_initialized && result != Result.Success && this.m_lastWait == 0)
					{
						this.LogError(filenameToTry, this.m_lastException);
					}
					return;
				}
				finally
				{
					if (!monitorHeld)
					{
						Monitor.Enter(this);
						monitorHeld = true;
					}
					this.m_active = false;
					if (this.m_goingIdleEvent != null)
					{
						this.m_goingIdleEvent.Set();
					}
					if (!this.m_prepareToStopCalled)
					{
						if (result != Result.Success)
						{
							if (result == Result.ShortWaitRetry)
							{
								num = this.m_lastWait + 1000;
								if (num > 10000)
								{
									num = 10000;
								}
							}
							else if (result == Result.LongWaitRetry)
							{
								num = 10000;
							}
							else if (result == Result.GiveUp)
							{
								num = -1;
							}
							if (this.m_lastShipLogsResult != Result.Success && this.m_lastShipLogsResult != Result.GiveUp && result != Result.GiveUp && this.m_lastException != null)
							{
								this.m_totalWait += this.m_lastWait;
							}
							else
							{
								this.m_totalWait = 0;
							}
							this.m_lastWait = num;
						}
						if (this.m_wakeTimer != null)
						{
							this.m_wakeTimer.M_thisTimer.Change(num, -1);
						}
					}
					if (this.m_stopEvent != null)
					{
						ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Setting stopEvent at end of ShipLogs", this.m_className);
						this.m_stopEvent.Set();
					}
					this.m_lastShipLogsResult = result;
					if (!this.m_prepareToStopCalled && !this.m_setBroken.IsBroken && (this.m_lastShipLogsResult == Result.GiveUp || this.m_totalWait >= 30000))
					{
						this.m_totalWait = 0;
						string text = string.Empty;
						if (this.m_lastException != null)
						{
							text = this.m_lastException.Message;
						}
						this.m_setBroken.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_ShipLogFailed, new string[]
						{
							this.m_className,
							text
						});
					}
					Monitor.Exit(this);
					monitorHeld = false;
				}
			}
			IL_376:
			this.m_notified = true;
			Monitor.Exit(this);
			monitorHeld = false;
		}

		protected string GetFilenameFromGenerationNumber(long generation)
		{
			return this.FromPrefix + ((generation == 0L) ? string.Empty : ShipControl.GenerationString(generation)) + this.FromSuffix;
		}

		protected DateTime GetFiletimeOfLog(string logfile)
		{
			FileInfo fileInfo = new FileInfo(logfile);
			return fileInfo.LastWriteTimeUtc;
		}

		protected virtual bool GapsAreAcceptable()
		{
			return false;
		}

		protected abstract void TestDelaySleep();

		protected abstract Result InitializeStartContext();

		protected abstract void PrepareToStopInternal();

		private static void ShipWorkThread(object source)
		{
			ShipControl shipControl = (ShipControl)source;
			shipControl.ShipLogs();
		}

		private static void ShipDirectoryChange(object source, FileSystemEventArgs logFile)
		{
			WatcherState watcherState = source as WatcherState;
			long num = 0L;
			if (ShipControl.GetGenerationNumberFromFilename(logFile.Name, watcherState.M_thisShip.FromPrefix, out num) && num != 0L)
			{
				watcherState.M_thisShip.ShipNotification(num);
			}
			watcherState.M_thisShip.ShipLogs();
		}

		private static void ShipTimeout(object source)
		{
			TimerState timerState = (TimerState)source;
			timerState.M_thisShip.ShipLogs();
		}

		private void DisposeWatcherState()
		{
			if (this.m_sourceDirWatcher != null)
			{
				this.m_sourceDirWatcher.Dispose();
				this.m_sourceDirWatcher = null;
			}
		}

		private Result ShipRestartFindFirst()
		{
			Result result = Result.Success;
			bool createNewWatcher = false;
			bool fNoFileFound = false;
			ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Entering ShipRestartFindFirst", this.m_className);
			long lowestSource;
			this.RunFileIoOperation(ref result, delegate
			{
				DirectoryInfo di = new DirectoryInfo(this.FromDir);
				if (this.m_sourceDirWatcher == null)
				{
					createNewWatcher = true;
				}
				else if (this.m_sourceDirWatcher.Path != this.FromDir || this.m_sourceDirWatcher.Filter != this.FromPrefix + '*' + this.FromSuffix)
				{
					this.m_sourceDirWatcher.Dispose();
					createNewWatcher = true;
				}
				if (createNewWatcher)
				{
					this.m_sourceDirWatcher = new WatcherState();
					this.m_sourceDirWatcher.M_thisShip = this;
					this.m_sourceDirWatcher.Path = this.FromDir;
					this.m_sourceDirWatcher.Filter = this.FromPrefix + '*' + this.FromSuffix;
					this.m_sourceDirWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite);
					this.m_sourceDirWatcher.Created += ShipControl.ShipDirectoryChange;
					this.m_sourceDirWatcher.Renamed += new RenamedEventHandler(ShipControl.ShipDirectoryChange);
					this.m_sourceDirWatcher.EnableRaisingEvents = true;
				}
				lowestSource = ShipControl.LowestGenerationInDirectory(di, this.FromPrefix, this.FromSuffix, false);
				if (lowestSource == 0L)
				{
					fNoFileFound = true;
					result = Result.LongWaitRetry;
					return;
				}
				if (this.m_fromNumber == 0L)
				{
					this.m_fromNumber = lowestSource;
				}
				else if (this.m_fromNumber < lowestSource)
				{
					result = Result.GiveUp;
					this.m_setBroken.SetBroken(this.TagUsedOnLogGapDetected, ReplayEventLogConstants.Tuple_LogFileGapFound, new string[]
					{
						this.m_fromNumber.ToString()
					});
				}
				if (!this.m_derivedInitialized)
				{
					result = this.InitializeStartContext();
					if (result == Result.Success)
					{
						this.m_derivedInitialized = true;
					}
				}
			});
			if (result != Result.Success && !fNoFileFound)
			{
				ExTraceGlobals.ShipLogTracer.TraceError<Result, Exception>((long)this.GetHashCode(), "ShipRestartFindFirst failed, result={0}:{1}", result, this.m_lastException);
			}
			return result;
		}

		protected virtual void CheckForGaps(long fromNumber)
		{
			if (this.GapsAreAcceptable())
			{
				ExTraceGlobals.ShipLogTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: CheckForGaps() is triggering ShipRestartFindFirst().", this.m_className);
				this.m_initialized = false;
				return;
			}
			new DirectoryInfo(this.FromDir);
			this.m_countdownToGapTest = 6L;
			bool flag = false;
			string path = Path.Combine(this.FromDir, this.GetFilenameFromGenerationNumber(fromNumber));
			long num = 1L;
			while (num <= this.MaxGapSize)
			{
				string path2 = Path.Combine(this.FromDir, this.GetFilenameFromGenerationNumber(fromNumber + num));
				if (File.Exists(path2))
				{
					if (File.Exists(path))
					{
						ExTraceGlobals.ShipLogTracer.TraceDebug<long>((long)this.GetHashCode(), "CheckForGaps() shouldn't be called since fromNumber {0} generation is there.", fromNumber);
						return;
					}
					flag = true;
					break;
				}
				else
				{
					num += 1L;
				}
			}
			if (flag)
			{
				string text = Path.Combine(this.FromDir, this.FromPrefix + ShipControl.GenerationString(fromNumber) + this.FromSuffix);
				ExTraceGlobals.ShipLogTracer.TraceError<string>((long)this.GetHashCode(), "Gap in log file generations after file {0}", text);
				this.m_setBroken.SetBroken(this.TagUsedOnLogGapDetected, ReplayEventLogConstants.Tuple_LogFileGapFound, new string[]
				{
					text
				});
			}
		}

		public const int ERROR_SHARING_VIOLATION = 32;

		protected const int ShortWait = 1000;

		protected const int LongWait = 10000;

		protected const long PeriodForGapTest = 6L;

		private const int MaxTotalWait = 30000;

		protected bool m_derivedInitialized;

		protected volatile bool m_fAttemptFinalCopyCalled;

		protected Exception m_lastException;

		protected string m_className;

		protected ShipLogsSetBroken m_shipLogsSetBroken;

		protected ISetBroken m_setBroken;

		protected IReplicaProgress m_replicaProgress;

		private readonly string m_fromPrefix;

		private readonly string m_fromSuffix;

		private string m_fromDir;

		private long m_fromNumber;

		private bool m_initialized;

		private bool m_active;

		private bool m_notified;

		private volatile bool m_prepareToStopCalled;

		private WatcherState m_sourceDirWatcher;

		private TimerState m_wakeTimer;

		private int m_lastWait;

		private ManualResetEvent m_stopEvent;

		private ManualResetEvent m_goingIdleEvent;

		private Result m_lastShipLogsResult;

		protected long m_countdownToGapTest;

		private bool m_fStopped;

		private int m_totalWait;

		protected delegate void FileIoOperation();
	}
}
