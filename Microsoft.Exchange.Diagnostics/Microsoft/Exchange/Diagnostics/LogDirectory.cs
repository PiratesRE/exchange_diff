using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics
{
	internal class LogDirectory
	{
		public LogDirectory(string path, string prefix, TimeSpan maxAge, long maxLogFileSize, long maxDirectorySize, string logComponent) : this(path, prefix, maxAge, maxLogFileSize, maxDirectorySize, logComponent, false, 0, TimeSpan.MaxValue)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval) : this(path, prefix, maxAge, maxLogFileSize, maxDirectorySize, logComponent, false, string.Empty, bufferSize, streamFlushInterval)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk) : this(path, prefix, maxAge, LogFileRollOver.Daily, maxLogFileSize, maxDirectorySize, logComponent, enforceAccurateAge, string.Empty, bufferSize, streamFlushInterval, flushToDisk)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, LogFileRollOver logFileRollOver, string logComponent, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk) : this(path, prefix, maxAge, logFileRollOver, 0L, 0L, logComponent, false, string.Empty, bufferSize, streamFlushInterval, flushToDisk)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, LogFileRollOver logFileRollOver, string logComponent, string note, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk) : this(path, prefix, maxAge, logFileRollOver, 0L, 0L, logComponent, false, note, bufferSize, streamFlushInterval, flushToDisk)
		{
		}

		public LogDirectory(string path, string prefix, LogFileRollOver logFileRollOver, string logComponent, int bufferSize, TimeSpan streamFlushInterval) : this(path, prefix, TimeSpan.MaxValue, logFileRollOver, 0L, 0L, logComponent, false, string.Empty, bufferSize, streamFlushInterval)
		{
		}

		public LogDirectory(string path, string prefix, LogFileRollOver logFileRollOver, string logComponent, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk) : this(path, prefix, TimeSpan.MaxValue, logFileRollOver, 0L, 0L, logComponent, false, string.Empty, bufferSize, streamFlushInterval, flushToDisk)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, string note, int bufferSize, TimeSpan streamFlushInterval) : this(path, prefix, maxAge, LogFileRollOver.Daily, maxLogFileSize, maxDirectorySize, logComponent, enforceAccurateAge, note, bufferSize, streamFlushInterval)
		{
		}

		public LogDirectory(string path, string prefix, TimeSpan maxAge, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, string note, int bufferSize, TimeSpan streamFlushInterval, bool flushtodisk) : this(path, prefix, maxAge, LogFileRollOver.Daily, maxLogFileSize, maxDirectorySize, logComponent, enforceAccurateAge, note, bufferSize, streamFlushInterval, flushtodisk)
		{
		}

		private LogDirectory(string path, string prefix, TimeSpan maxAge, LogFileRollOver logFileRollOver, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, string note, int bufferSize, TimeSpan streamFlushInterval) : this(path, prefix, maxAge, logFileRollOver, maxLogFileSize, maxDirectorySize, logComponent, enforceAccurateAge, note, bufferSize, streamFlushInterval, false)
		{
		}

		private LogDirectory(string path, string prefix, TimeSpan maxAge, LogFileRollOver logFileRollOver, long maxLogFileSize, long maxDirectorySize, string logComponent, bool enforceAccurateAge, string note, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk)
		{
			if (streamFlushInterval <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("streamFlushInterval", streamFlushInterval, "streamFlushInterval should be greater than zero");
			}
			if (bufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", bufferSize, "buffer size must be non-negative");
			}
			this.bufferLength = bufferSize;
			this.directory = Log.CreateLogDirectory(path);
			this.prefix = prefix;
			this.maxAge = maxAge;
			this.logFileRollOver = logFileRollOver;
			this.maxLogFileSize = maxLogFileSize;
			this.maxDirectorySize = maxDirectorySize;
			this.logComponent = logComponent;
			this.enforceAccurateAge = enforceAccurateAge;
			this.note = (note ?? string.Empty);
			this.flushToDisk = flushToDisk;
			this.streamFlushInterval = streamFlushInterval;
			if (maxLogFileSize != 0L)
			{
				this.matcher = new Regex(string.Concat(new string[]
				{
					"^",
					Regex.Escape(prefix),
					enforceAccurateAge ? "(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})(?<hour>\\d{0,2})-(?<instance>\\d+)(?<note>.*)" : "(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})-(?<instance>\\d+)(?<note>.*)",
					Regex.Escape(".LOG"),
					"$"
				}), RegexOptions.IgnoreCase);
				this.production = (enforceAccurateAge ? "{0}{1:yyyyMMddHH}-{2:d}{3}{4}" : "{0}{1:yyyyMMdd}-{2:d}{3}{4}");
				this.dirTemplate = (enforceAccurateAge ? (this.prefix + "????????*-*.LOG") : (this.prefix + "????????-*.LOG"));
				return;
			}
			switch (this.LogFileRollOver)
			{
			case LogFileRollOver.Hourly:
				this.matcher = new Regex(string.Concat(new string[]
				{
					"^",
					Regex.Escape(prefix),
					"(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})(?<hour>\\d{2})-(?<instance>\\d+)(?<note>.*)",
					Regex.Escape(".LOG"),
					"$"
				}), RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this.production = "{0}{1:yyyyMMddHH}-{2:d}{3}{4}";
				this.dirTemplate = this.prefix + "??????????-*.LOG";
				return;
			case LogFileRollOver.Daily:
				this.matcher = new Regex(string.Concat(new string[]
				{
					"^",
					Regex.Escape(prefix),
					"(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})-(?<instance>\\d+)(?<note>.*)",
					Regex.Escape(".LOG"),
					"$"
				}), RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this.production = "{0}{1:yyyyMMdd}-{2:d}{3}{4}";
				this.dirTemplate = this.prefix + "????????-*.LOG";
				return;
			case LogFileRollOver.Weekly:
				this.matcher = new Regex(string.Concat(new string[]
				{
					"^",
					Regex.Escape(prefix),
					"(?<year>\\d{4})(?<month>\\d{2})W(?<week>\\d{1})-(?<instance>\\d+)(?<note>.*)",
					Regex.Escape(".LOG"),
					"$"
				}), RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this.production = "{0}{1:yyyyMM}W{5}-{2:d}{3}{4}";
				this.dirTemplate = this.prefix + "??????W?-*.LOG";
				return;
			case LogFileRollOver.Monthly:
				this.matcher = new Regex(string.Concat(new string[]
				{
					"^",
					Regex.Escape(prefix),
					"(?<year>\\d{4})(?<month>\\d{2})-(?<instance>\\d+)(?<note>.*)",
					Regex.Escape(".LOG"),
					"$"
				}), RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this.production = "{0}{1:yyyyMM}-{2:d}{3}{4}";
				this.dirTemplate = this.prefix + "??????-*.LOG";
				return;
			default:
				throw new InvalidOperationException("The code should never be hit.");
			}
		}

		public event LogDirectory.OnDirSizeQuotaExceededHandler OnDirSizeQuotaExceeded;

		internal LogFileRollOver LogFileRollOver
		{
			get
			{
				return this.logFileRollOver;
			}
		}

		internal string FullName
		{
			get
			{
				return this.directory.FullName;
			}
		}

		private string Note
		{
			get
			{
				return this.note;
			}
		}

		private bool EnforceAccurateAge
		{
			get
			{
				return this.enforceAccurateAge;
			}
		}

		private long MaxLogFileSize
		{
			get
			{
				return this.maxLogFileSize;
			}
		}

		public Stream GetLogFile(DateTime forDate)
		{
			return this.GetLogFile(forDate, null, false);
		}

		public Stream GetLogFile(DateTime forDate, LogHeaderFormatter logHeaderFormatter, bool forceLogFileRollOver)
		{
			if (this.logFile != null)
			{
				try
				{
					if (forceLogFileRollOver || !this.logFileId.SameLogSeries(forDate, this) || (this.maxLogFileSize > 0L && this.logFile.Position > this.maxLogFileSize))
					{
						this.logFile.Close();
						this.logFile = null;
					}
				}
				catch (ObjectDisposedException)
				{
					this.logFile = null;
				}
			}
			if (this.logFile == null)
			{
				lock (this.fileRollOverLock)
				{
					if (this.logFile == null)
					{
						SortedList<LogDirectory.LogFileId, FileInfo> logFileList = this.GetLogFileList();
						int num = this.EnforceDirectorySizeQuota(logFileList);
						if (num > 0)
						{
							Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_DeleteLogDueToQuota, null, new object[]
							{
								this.logComponent,
								this.FullName,
								this.maxDirectorySize,
								num
							});
							LogDirectory.OnDirSizeQuotaExceededHandler onDirSizeQuotaExceeded = this.OnDirSizeQuotaExceeded;
							if (onDirSizeQuotaExceeded != null)
							{
								onDirSizeQuotaExceeded(this.logComponent, this.FullName, this.maxDirectorySize, num);
							}
						}
						if (this.maxAge < TimeSpan.MaxValue)
						{
							this.DeleteOldLogFiles(logFileList, num, forDate - this.maxAge);
						}
						bool startNewLog = forceLogFileRollOver || (logHeaderFormatter != null && logHeaderFormatter.CsvOption == LogHeaderCsvOption.CsvStrict);
						bool flag2 = false;
						this.logFileId = this.GenerateLogFileId(logFileList, forDate, startNewLog, out flag2);
						int i = 0;
						while (i < 20)
						{
							try
							{
								this.logFile = new LogDirectory.BufferedStream(this.GetLogFileNameFromId(this.logFileId), this.bufferLength, this.flushToDisk);
								if (this.maxLogFileSize > 0L && this.logFile.Position > this.maxLogFileSize)
								{
									this.logFile.Close();
									this.logFile = null;
								}
							}
							catch (IOException)
							{
								this.logFile = null;
							}
							if (this.logFile == null)
							{
								this.logFileId = this.logFileId.Next;
								flag2 = true;
								i++;
							}
							else
							{
								if (logHeaderFormatter != null && flag2)
								{
									logHeaderFormatter.Write(this.logFile, forDate);
								}
								if (this.streamFlushTimer == null && this.streamFlushInterval != TimeSpan.MaxValue)
								{
									this.streamFlushTimer = new Timer(new TimerCallback(this.FlushStream), null, TimeSpan.Zero, this.streamFlushInterval);
									break;
								}
								break;
							}
						}
					}
				}
			}
			return this.logFile;
		}

		public void Flush()
		{
			this.FlushStream(null);
		}

		public void Close()
		{
			if (this.streamFlushTimer != null)
			{
				this.streamFlushTimer.Dispose();
				this.streamFlushTimer = null;
			}
			if (this.logFile != null)
			{
				this.logFile.Close();
				this.logFile = null;
			}
		}

		private static DateTime GetFirstDateForWeek(int year, int month, int week)
		{
			if (week < 1 || week > 6)
			{
				throw new ArgumentOutOfRangeException("week", "Week number is out of range");
			}
			DateTime result = new DateTime(year, month, 1);
			int num = 7 + CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - result.DayOfWeek;
			if (week > 1)
			{
				result = result.AddDays((double)(num + 7 * (week - 2)));
			}
			if (result.Month != month)
			{
				throw new ArgumentOutOfRangeException("week", "Week number is out of range for the month");
			}
			return result;
		}

		private static int GetWeekNumber(DateTime date)
		{
			DateTime dateTime = new DateTime(date.Year, date.Month, 1);
			int num = 7 + CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - dateTime.DayOfWeek;
			if (date.Day <= num)
			{
				return 1;
			}
			return 2 + (date.Day - num - 1) / 7;
		}

		private static bool IsDiskFullException(IOException e)
		{
			return Marshal.GetHRForException(e) == -2147024784;
		}

		private LogDirectory.LogFileId GetLogFileIdFromName(string filename)
		{
			Match match = this.matcher.Match(filename);
			if (!match.Success)
			{
				return null;
			}
			int hour = 0;
			int instance;
			DateTime date;
			string value2;
			try
			{
				int year = Convert.ToInt32(match.Groups["year"].Captures[0].Value, CultureInfo.InvariantCulture);
				int month = Convert.ToInt32(match.Groups["month"].Captures[0].Value, CultureInfo.InvariantCulture);
				instance = Convert.ToInt32(match.Groups["instance"].Captures[0].Value, CultureInfo.InvariantCulture);
				if (this.maxLogFileSize == 0L)
				{
					switch (this.LogFileRollOver)
					{
					case LogFileRollOver.Hourly:
					{
						int day = Convert.ToInt32(match.Groups["day"].Captures[0].Value, CultureInfo.InvariantCulture);
						hour = Convert.ToInt32(match.Groups["hour"].Captures[0].Value, CultureInfo.InvariantCulture);
						date = new DateTime(year, month, day, hour, 0, 0);
						break;
					}
					case LogFileRollOver.Daily:
					{
						int day = Convert.ToInt32(match.Groups["day"].Captures[0].Value, CultureInfo.InvariantCulture);
						date = new DateTime(year, month, day);
						break;
					}
					case LogFileRollOver.Weekly:
					{
						int week = Convert.ToInt32(match.Groups["week"].Captures[0].Value, CultureInfo.InvariantCulture);
						date = LogDirectory.GetFirstDateForWeek(year, month, week);
						break;
					}
					case LogFileRollOver.Monthly:
						date = new DateTime(year, month, 1);
						break;
					default:
						throw new InvalidOperationException("The code should never be hit.");
					}
				}
				else
				{
					int day = Convert.ToInt32(match.Groups["day"].Captures[0].Value, CultureInfo.InvariantCulture);
					if (this.enforceAccurateAge)
					{
						string value = match.Groups["hour"].Captures[0].Value;
						if (!string.IsNullOrEmpty(value))
						{
							hour = Convert.ToInt32(value, CultureInfo.InvariantCulture);
						}
					}
					date = (this.enforceAccurateAge ? new DateTime(year, month, day, hour, 0, 0) : new DateTime(year, month, day));
				}
				value2 = match.Groups["note"].Captures[0].Value;
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			catch (FormatException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			return new LogDirectory.LogFileId(date, value2, instance);
		}

		private SortedList<LogDirectory.LogFileId, FileInfo> GetLogFileList()
		{
			SortedList<LogDirectory.LogFileId, FileInfo> sortedList = new SortedList<LogDirectory.LogFileId, FileInfo>();
			FileInfo[] files = this.directory.GetFiles(this.dirTemplate);
			for (int i = 0; i < files.Length; i++)
			{
				LogDirectory.LogFileId logFileIdFromName = this.GetLogFileIdFromName(files[i].Name);
				if (logFileIdFromName != null)
				{
					sortedList[logFileIdFromName] = files[i];
				}
			}
			return sortedList;
		}

		private int EnforceDirectorySizeQuota(SortedList<LogDirectory.LogFileId, FileInfo> logFiles)
		{
			long num = 0L;
			int i = logFiles.Count - 1;
			if (this.maxDirectorySize == 0L)
			{
				return 0;
			}
			while (i >= 0)
			{
				long length = logFiles.Values[i].Length;
				if (num + length >= this.maxDirectorySize)
				{
					break;
				}
				num += length;
				i--;
			}
			int result = i + 1;
			while (i >= 0)
			{
				FileInfo fileInfo = logFiles.Values[i];
				try
				{
					fileInfo.Delete();
				}
				catch (IOException)
				{
				}
				i--;
			}
			return result;
		}

		private void DeleteOldLogFiles(SortedList<LogDirectory.LogFileId, FileInfo> logFiles, int start, DateTime earliestToKeep)
		{
			int num = 0;
			int num2 = start;
			while (num2 < logFiles.Count && logFiles.Keys[num2].Date < earliestToKeep)
			{
				try
				{
					logFiles.Values[num2].Delete();
					num++;
				}
				catch (IOException)
				{
				}
				num2++;
			}
			if (num > 0)
			{
				Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_DeleteOldLog, null, new object[]
				{
					this.logComponent,
					this.FullName,
					earliestToKeep
				});
			}
		}

		private string GetLogFileNameFromId(LogDirectory.LogFileId id)
		{
			int weekNumber = LogDirectory.GetWeekNumber(id.Date);
			return Path.Combine(this.FullName, string.Format(CultureInfo.InvariantCulture, this.production, new object[]
			{
				this.prefix,
				id.Date,
				id.Instance,
				this.note,
				".LOG",
				weekNumber
			}));
		}

		private LogDirectory.LogFileId GenerateLogFileId(SortedList<LogDirectory.LogFileId, FileInfo> logFiles, DateTime date, bool startNewLog, out bool newLogStarted)
		{
			int i = logFiles.Count - 1;
			bool flag = false;
			LogDirectory.LogFileId logFileId = null;
			newLogStarted = true;
			while (i >= 0)
			{
				logFileId = logFiles.Keys[i];
				if (logFileId.SameLogSeries(date, this))
				{
					flag = true;
					break;
				}
				i--;
			}
			if (!flag)
			{
				return new LogDirectory.LogFileId(date, this.note, 1);
			}
			if (this.maxLogFileSize > 0L && logFiles.Values[i].Length >= this.maxLogFileSize)
			{
				startNewLog = true;
			}
			newLogStarted = startNewLog;
			if (!startNewLog)
			{
				return logFileId;
			}
			return logFileId.Next;
		}

		private void FlushStream(object state)
		{
			Stream stream = this.logFile;
			if (stream != null)
			{
				try
				{
					stream.Flush();
				}
				catch (IOException ex)
				{
					if (!LogDirectory.IsDiskFullException(ex))
					{
						Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_FailedToAppendLog, null, new object[]
						{
							this.logComponent,
							ex.Message
						});
					}
				}
			}
		}

		private const int MaxRecreateAttempts = 20;

		private const string Extension = ".LOG";

		private readonly int bufferLength;

		private string production;

		private DirectoryInfo directory;

		private string prefix;

		private string dirTemplate;

		private TimeSpan maxAge;

		private LogFileRollOver logFileRollOver;

		private long maxLogFileSize;

		private long maxDirectorySize;

		private Stream logFile;

		private LogDirectory.LogFileId logFileId;

		private Regex matcher;

		private string logComponent;

		private bool enforceAccurateAge;

		private string note;

		private TimeSpan streamFlushInterval;

		private Timer streamFlushTimer;

		private bool flushToDisk;

		private object fileRollOverLock = new object();

		public delegate void OnDirSizeQuotaExceededHandler(string component, string directory, long maxDirectorySize, int trimmed);

		internal class BufferedStream : Stream
		{
			public BufferedStream(string filePath, int bufferLength) : this(filePath, bufferLength, false)
			{
			}

			public BufferedStream(string filePath, int bufferLength, bool flushToDisk)
			{
				this.wrappedStream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
				this.bufferSize = bufferLength;
				this.buffer = new byte[(bufferLength != 0) ? bufferLength : 4096];
				this.flushToDisk = flushToDisk;
			}

			public int BufferSize
			{
				get
				{
					return this.bufferSize;
				}
			}

			public override long Position
			{
				get
				{
					return this.wrappedStream.Position + (long)this.currPos;
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			public override long Length
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override void Write(byte[] bufferToWrite, int index, int count)
			{
				lock (this.syncObj)
				{
					if (this.currPos + count > this.buffer.Length)
					{
						this.Flush();
					}
					if (this.currPos + count <= this.buffer.Length)
					{
						Buffer.BlockCopy(bufferToWrite, index, this.buffer, this.currPos, count);
						this.currPos += count;
					}
					else
					{
						this.Flush();
						this.InternalWrite(bufferToWrite, index, count);
						this.wrappedStream.Flush();
					}
				}
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotImplementedException();
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}

			public override void Flush()
			{
				lock (this.syncObj)
				{
					if (this.currPos != 0)
					{
						this.wrappedStream.Write(this.buffer, 0, this.currPos);
						this.wrappedStream.Flush();
						if (this.flushToDisk)
						{
							DiagnosticsNativeMethods.FlushFileBuffers(((FileStream)this.wrappedStream).SafeFileHandle);
						}
						this.currPos = 0;
					}
				}
			}

			public override void Close()
			{
				lock (this.syncObj)
				{
					this.Flush();
					this.wrappedStream.Flush();
					this.wrappedStream.Dispose();
					this.Dispose(true);
				}
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private void InternalWrite(byte[] bufferToWrite, int index, int count)
			{
				this.wrappedStream.Write(bufferToWrite, index, count);
			}

			private const int DefaultBufferSize = 4096;

			private object syncObj = new object();

			private byte[] buffer;

			private int bufferSize;

			private int currPos;

			private Stream wrappedStream;

			private bool flushToDisk;
		}

		private class LogFileId : IComparable
		{
			public LogFileId(DateTime date, string note, int instance)
			{
				this.date = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
				this.note = note;
				this.instance = instance;
			}

			public LogDirectory.LogFileId Next
			{
				get
				{
					return new LogDirectory.LogFileId(this.date, this.note, this.instance + 1);
				}
			}

			public DateTime Date
			{
				get
				{
					return this.date;
				}
			}

			public int Instance
			{
				get
				{
					return this.instance;
				}
			}

			public string Note
			{
				get
				{
					return this.note;
				}
			}

			int IComparable.CompareTo(object o)
			{
				LogDirectory.LogFileId logFileId = o as LogDirectory.LogFileId;
				if (logFileId == null)
				{
					throw new ArgumentException("object is not a LogFileId");
				}
				if (this.date < logFileId.date)
				{
					return -1;
				}
				if (logFileId.date < this.date)
				{
					return 1;
				}
				if (this.instance != logFileId.instance)
				{
					return this.instance - logFileId.instance;
				}
				return this.note.CompareTo(logFileId.note);
			}

			public bool SameLogSeries(DateTime date, LogDirectory logDirectory)
			{
				if (!this.note.Equals(logDirectory.Note, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				if (logDirectory.MaxLogFileSize != 0L)
				{
					return logDirectory.MaxLogFileSize == long.MaxValue || ((!logDirectory.EnforceAccurateAge || this.Date.Hour == date.Hour) && (this.Date.Year == date.Year && this.Date.Month == date.Month) && this.Date.Day == date.Day);
				}
				switch (logDirectory.LogFileRollOver)
				{
				case LogFileRollOver.Hourly:
					return this.Date.Year == date.Year && this.Date.Month == date.Month && this.Date.Day == date.Day && this.Date.Hour == date.Hour;
				case LogFileRollOver.Daily:
					return this.Date.Year == date.Year && this.Date.Month == date.Month && this.Date.Day == date.Day;
				case LogFileRollOver.Weekly:
					return this.Date.Year == date.Year && this.Date.Month == date.Month && LogDirectory.GetWeekNumber(this.Date) == LogDirectory.GetWeekNumber(date);
				case LogFileRollOver.Monthly:
					return this.Date.Year == date.Year && this.Date.Month == date.Month;
				default:
					throw new InvalidOperationException("The code should never be hit.");
				}
			}

			private DateTime date;

			private int instance;

			private string note;
		}
	}
}
