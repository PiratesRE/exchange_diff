using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics
{
	public class Log
	{
		public Log(string fileNamePrefix, LogHeaderFormatter headerFormatter, string logComponent) : this(fileNamePrefix, headerFormatter, logComponent, true)
		{
		}

		public Log(string fileNamePrefix, LogHeaderFormatter headerFormatter, string logComponent, bool handleKnownExceptions)
		{
			this.headerFormatter = headerFormatter;
			this.fileNamePrefix = fileNamePrefix;
			this.logComponent = logComponent;
			this.handleKnownExceptions = handleKnownExceptions;
			this.maxWaitAppendCount = 50;
		}

		public static ExEventLog EventLog
		{
			get
			{
				return Log.eventLogger;
			}
		}

		public bool TestHelper_ForceLogFileRollOver { get; set; }

		internal LogDirectory LogDirectory
		{
			get
			{
				return this.logDirectory;
			}
		}

		public static DirectoryInfo CreateLogDirectory(string path)
		{
			return Log.CreateLogDirectory(new DirectoryInfo(path));
		}

		public void Flush()
		{
			try
			{
				if (this.logDirectory != null)
				{
					this.logDirectory.Flush();
				}
			}
			catch (IOException ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<LogDirectory, string>(30569, (long)this.GetHashCode(), "Failed to Flush the LogDirectory {0}. Error {1}", this.logDirectory, ex.Message);
				this.logDirectory.Close();
				this.logDirectory = null;
			}
		}

		public void Close()
		{
			try
			{
				if (this.logDirectory != null)
				{
					this.logDirectory.Close();
				}
			}
			catch (IOException ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<LogDirectory, string>(30569, (long)this.GetHashCode(), "Failed to Close the LogDirectory {0}. Error {1}", this.logDirectory, ex.Message);
			}
			finally
			{
				this.logDirectory = null;
			}
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize)
		{
			this.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, 0, TimeSpan.MaxValue);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.Configure(path, maxAge, LogFileRollOver.Daily, maxDirectorySize, maxLogFileSize, false, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, string note)
		{
			this.Configure(path, maxAge, LogFileRollOver.Daily, maxDirectorySize, maxLogFileSize, false, bufferSize, streamFlushInterval, note, false);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, string note, bool flushToDisk)
		{
			this.Configure(path, maxAge, LogFileRollOver.Daily, maxDirectorySize, maxLogFileSize, false, bufferSize, streamFlushInterval, note, flushToDisk);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk)
		{
			this.Configure(path, maxAge, LogFileRollOver.Daily, maxDirectorySize, maxLogFileSize, false, bufferSize, streamFlushInterval, flushToDisk);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver)
		{
			this.Configure(path, logFileRollOver, 0, TimeSpan.MaxValue);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.Configure(path, TimeSpan.MaxValue, logFileRollOver, 0L, 0L, false, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge)
		{
			this.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, enforceAccurateAge, 0, TimeSpan.MaxValue);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver, TimeSpan maxAge)
		{
			this.Configure(path, maxAge, logFileRollOver, 0L, 0L, false, 0, TimeSpan.MaxValue);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.Configure(path, maxAge, LogFileRollOver.Daily, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval, LogFileRollOver logFileRollOver)
		{
			this.Configure(path, maxAge, logFileRollOver, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval);
		}

		public void Append(IEnumerable<LogRowFormatter> rows, int timestampField)
		{
			lock (this.logLock)
			{
				foreach (LogRowFormatter row in rows)
				{
					this.Append(row, timestampField);
				}
			}
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
			this.Append(row, timestampField, DateTime.MinValue);
		}

		public void Append(LogRowFormatter row, int timestampField, DateTime timeStamp)
		{
			Exception ex = null;
			string text = null;
			lock (this.logLock)
			{
				if (this.failedToCreateDirectory)
				{
					ExTraceGlobals.CommonTracer.TraceError<string>(26473, (long)this.GetHashCode(), "Cannot append to the {0} logs because we failed to create the logging directory", this.logComponent);
					if (!this.handleKnownExceptions)
					{
						throw new InvalidOperationException("Not configured");
					}
					return;
				}
				else
				{
					if (this.handleKnownExceptions && this.donotAppend && ++this.donotAppendCount < this.maxWaitAppendCount)
					{
						ExTraceGlobals.CommonTracer.TraceError<string, int>(22377, (long)this.GetHashCode(), "Not appending to the {0} logs. Failed to log in previous attempts. Will wait {1} times before retrying", this.logComponent, this.maxWaitAppendCount - this.donotAppendCount);
						return;
					}
					if (this.logDirectory == null)
					{
						throw new InvalidOperationException("Cannot append to a closed log.");
					}
					this.donotAppend = false;
					DateTime utcNow = DateTime.UtcNow;
					if (timestampField >= 0)
					{
						row[timestampField] = ((timeStamp != DateTime.MinValue) ? timeStamp : utcNow);
					}
					try
					{
						Stream logFile = this.logDirectory.GetLogFile(utcNow, this.headerFormatter, this.TestHelper_ForceLogFileRollOver);
						if (logFile == null)
						{
							this.donotAppend = true;
							this.donotAppendCount = 1;
							text = "Couldn't create a new log file";
						}
						else
						{
							row.Write(logFile);
							LogDirectory.BufferedStream bufferedStream = logFile as LogDirectory.BufferedStream;
							if (bufferedStream != null && bufferedStream.BufferSize == 0)
							{
								bufferedStream.Flush();
							}
						}
					}
					catch (IOException ex2)
					{
						ex = ex2;
						this.donotAppend = true;
						this.donotAppendCount = 1;
						text = ex2.Message;
					}
					catch (UnauthorizedAccessException ex3)
					{
						ex = ex3;
						this.donotAppend = true;
						this.donotAppendCount = 1;
						text = ex3.Message;
					}
				}
			}
			if (this.donotAppend)
			{
				Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToAppendLog, this.logDirectory.FullName, new object[]
				{
					this.logComponent,
					text
				});
			}
			if (!this.handleKnownExceptions && ex != null)
			{
				throw new LogException("log append failed", ex);
			}
		}

		private static DirectorySecurity GetDefaultDirectorySecurity()
		{
			if (Log.defaultDirectorySecurity == null)
			{
				DirectorySecurity directorySecurity = new DirectorySecurity();
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					directorySecurity.SetOwner(current.User);
				}
				for (int i = 0; i < Log.DirectoryAccessRules.Length; i++)
				{
					directorySecurity.AddAccessRule(Log.DirectoryAccessRules[i]);
				}
				Interlocked.CompareExchange<DirectorySecurity>(ref Log.defaultDirectorySecurity, directorySecurity, null);
			}
			return Log.defaultDirectorySecurity;
		}

		private static DirectoryInfo CreateLogDirectory(DirectoryInfo directory)
		{
			if (!directory.Exists)
			{
				if (directory.Parent != null)
				{
					Log.CreateLogDirectory(directory.Parent);
				}
				Log.InternalCreateLogDirectory(directory.FullName);
			}
			return directory;
		}

		private static DirectoryInfo InternalCreateLogDirectory(string path)
		{
			DirectoryInfo directoryInfo = Directory.CreateDirectory(path, Log.GetDefaultDirectorySecurity());
			DirectorySecurity accessControl = Directory.GetAccessControl(path);
			accessControl.SetAccessRuleProtection(false, true);
			directoryInfo.SetAccessControl(accessControl);
			return directoryInfo;
		}

		private void Configure(string path, TimeSpan maxAge, LogFileRollOver logFileRollOver, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.Configure(path, maxAge, logFileRollOver, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval, string.Empty, false);
		}

		private void Configure(string path, TimeSpan maxAge, LogFileRollOver logFileRollOver, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk)
		{
			this.Configure(path, maxAge, logFileRollOver, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval, string.Empty, false);
		}

		private void Configure(string path, TimeSpan maxAge, LogFileRollOver logFileRollOver, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval, string note, bool flushToDisk)
		{
			if (path == this.path && maxAge == this.maxAge && logFileRollOver == this.logFileRollOver && maxDirectorySize == this.maxDirectorySize && maxLogFileSize == this.maxLogFileSize && enforceAccurateAge == this.enforceAccurateAge && bufferSize == this.bufferSize && streamFlushInterval == this.streamFlushInterval && flushToDisk == this.flushToDisk)
			{
				return;
			}
			Exception ex = null;
			string text = null;
			lock (this.logLock)
			{
				this.Close();
				try
				{
					text = Path.GetFullPath(path);
					if (maxLogFileSize == 0L)
					{
						this.logDirectory = new LogDirectory(text, this.fileNamePrefix, (maxAge == TimeSpan.Zero) ? TimeSpan.MaxValue : maxAge, logFileRollOver, this.logComponent, note, bufferSize, streamFlushInterval, flushToDisk);
					}
					else
					{
						this.logDirectory = new LogDirectory(text, this.fileNamePrefix, (maxAge == TimeSpan.Zero) ? TimeSpan.MaxValue : maxAge, maxLogFileSize, maxDirectorySize, this.logComponent, enforceAccurateAge, note, bufferSize, streamFlushInterval, flushToDisk);
					}
					this.failedToCreateDirectory = false;
					this.donotAppendCount = 0;
					this.donotAppend = false;
					this.path = path;
					this.maxAge = maxAge;
					this.logFileRollOver = logFileRollOver;
					this.maxDirectorySize = maxDirectorySize;
					this.maxLogFileSize = maxLogFileSize;
					this.enforceAccurateAge = enforceAccurateAge;
					this.bufferSize = bufferSize;
					this.streamFlushInterval = streamFlushInterval;
					this.flushToDisk = flushToDisk;
				}
				catch (DirectoryNotFoundException ex2)
				{
					ex = ex2;
					this.logDirectory = null;
					this.failedToCreateDirectory = true;
					Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToCreateDirectory, null, new object[]
					{
						this.logComponent,
						text,
						ex2.Message
					});
				}
				catch (ArgumentException ex3)
				{
					ex = ex3;
					this.logDirectory = null;
					this.failedToCreateDirectory = true;
					Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToCreateDirectory, null, new object[]
					{
						this.logComponent,
						text,
						ex3.Message
					});
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
					this.logDirectory = null;
					this.failedToCreateDirectory = true;
					Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToCreateDirectory, null, new object[]
					{
						this.logComponent,
						text,
						ex4.Message
					});
				}
				catch (IOException ex5)
				{
					ex = ex5;
					this.logDirectory = null;
					this.failedToCreateDirectory = true;
					Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToCreateDirectory, null, new object[]
					{
						this.logComponent,
						text,
						ex5.Message
					});
				}
				catch (InvalidOperationException ex6)
				{
					if (Marshal.GetLastWin32Error() != 122)
					{
						throw;
					}
					ex = ex6;
					this.logDirectory = null;
					this.failedToCreateDirectory = true;
					Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToCreateDirectory, null, new object[]
					{
						this.logComponent,
						text,
						ex6.Message
					});
				}
			}
			if (!this.handleKnownExceptions && ex != null)
			{
				throw new LogException("log config failed", ex);
			}
		}

		private static readonly FileSystemAccessRule[] DirectoryAccessRules = new FileSystemAccessRule[]
		{
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow)
		};

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.CommonTracer.Category, "MSExchange Common");

		private static DirectorySecurity defaultDirectorySecurity;

		private readonly string fileNamePrefix;

		private readonly LogHeaderFormatter headerFormatter;

		private readonly int maxWaitAppendCount;

		private readonly object logLock = new object();

		private readonly string logComponent;

		private readonly bool handleKnownExceptions;

		private LogDirectory logDirectory;

		private bool donotAppend;

		private int donotAppendCount;

		private bool failedToCreateDirectory;

		private string path;

		private TimeSpan maxAge;

		private LogFileRollOver logFileRollOver;

		private long maxDirectorySize;

		private long maxLogFileSize;

		private bool enforceAccurateAge;

		private int bufferSize;

		private TimeSpan streamFlushInterval;

		private bool flushToDisk;
	}
}
