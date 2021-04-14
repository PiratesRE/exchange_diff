using System;
using System.IO;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogFileInfo : IComparable<LogFileInfo>, ILogFileInfo
	{
		public LogFileInfo(string fileName, bool isActive, string instanceName, IWatermarkFileHelper wmkFileHelper)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("fileName", fileName);
			ArgumentValidator.ThrowIfNull("wmkFileMgr", wmkFileHelper);
			ArgumentValidator.ThrowIfNullOrEmpty("LogFileDirectory", wmkFileHelper.LogFileDirectory);
			this.fileName = Path.GetFileName(fileName);
			ArgumentValidator.ThrowIfNullOrEmpty("this.fileName", this.fileName);
			this.fullFileName = Path.Combine(wmkFileHelper.LogFileDirectory, this.fileName);
			this.status = ProcessingStatus.NeedProcessing;
			this.isActive = isActive;
			this.syncObject = new object();
			this.instance = instanceName;
			if (!File.Exists(this.fullFileName))
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_FailedToInstantiateLogFileInfoFileNotExist, this.fullFileName, new object[]
				{
					this.fullFileName
				});
				ServiceLogger.LogError(ServiceLogger.Component.LogFileInfo, (LogUploaderEventLogConstants.Message)3221226477U, string.Empty, this.instance, this.fullFileName);
				throw new FailedToInstantiateLogFileInfoException(Strings.FailedToInstantiateLogFileInfoFileNotExist(this.fullFileName));
			}
			this.fileInfo = new FileInfo(this.fullFileName);
			this.fileInfo.Refresh();
			this.creationTimeUtc = this.fileInfo.CreationTimeUtc;
			this.waterMarkFile = wmkFileHelper.CreateWaterMarkFileObj(this.fileName, instanceName);
			Tools.DebugAssert(this.waterMarkFile != null, "this.waterMarkFile != null");
		}

		public IWatermarkFile WatermarkFileObj
		{
			get
			{
				return this.waterMarkFile;
			}
		}

		public DateTime LastWriteTimeUtc
		{
			get
			{
				if (File.Exists(this.fullFileName))
				{
					this.fileInfo.Refresh();
					return this.fileInfo.LastWriteTimeUtc;
				}
				throw new MessageTracingException(Strings.GetLogTimeStampFailed(this.fullFileName));
			}
		}

		public DateTime CreationTimeUtc
		{
			get
			{
				return this.creationTimeUtc;
			}
		}

		public ProcessingStatus Status
		{
			get
			{
				ProcessingStatus result;
				lock (this.syncObject)
				{
					result = this.status;
				}
				return result;
			}
			set
			{
				lock (this.syncObject)
				{
					this.status = value;
				}
				if (this.status == ProcessingStatus.CompletedProcessing && this.HasEverBeenInactive && !this.inactiveTimeCounted)
				{
					long ticks = DateTime.UtcNow.Ticks;
					long incrementValue = ticks - this.startOfInactivityInTicks;
					PerfCountersInstanceCache.GetInstance(this.instance).AverageInactiveParseLatencyBase.Increment();
					PerfCountersInstanceCache.GetInstance(this.instance).AverageInactiveParseLatency.IncrementBy(incrementValue);
					this.inactiveTimeCounted = true;
				}
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string FullFileName
		{
			get
			{
				return this.fullFileName;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
			set
			{
				if (!this.isActive && value)
				{
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)2147486660U, string.Empty, this.instance, this.fullFileName);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InactiveFileTurnsToActiveException, this.fullFileName, new object[]
					{
						string.Empty
					});
				}
				this.isActive = value;
				if (!this.isActive && !this.HasEverBeenInactive)
				{
					this.startOfInactivityInTicks = DateTime.UtcNow.Ticks;
				}
			}
		}

		public long Size
		{
			get
			{
				long result = 0L;
				try
				{
					this.fileInfo.Refresh();
					result = this.fileInfo.Length;
				}
				catch (FileNotFoundException)
				{
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221231489U, string.Empty, this.instance, this.fullFileName);
					result = 0L;
				}
				return result;
			}
		}

		public long FileSizeAtLastDirectoryCheck
		{
			get
			{
				return this.fileSizeAtLastDirectoryCheck;
			}
		}

		public DateTime LastProcessedTime
		{
			get
			{
				return this.lastProcessedTime;
			}
			internal set
			{
				this.lastProcessedTime = value;
			}
		}

		public bool FileExists
		{
			get
			{
				return File.Exists(this.fullFileName);
			}
		}

		private bool HasEverBeenInactive
		{
			get
			{
				return this.startOfInactivityInTicks != 0L;
			}
		}

		public int CompareTo(LogFileInfo other)
		{
			return this.CreationTimeUtc.CompareTo(other.CreationTimeUtc);
		}

		public long AddedLogSize()
		{
			long size = this.Size;
			long num = this.fileSizeAtLastDirectoryCheck;
			this.fileSizeAtLastDirectoryCheck = size;
			return size - num;
		}

		private readonly string fileName;

		private readonly string fullFileName;

		private readonly string instance;

		private readonly DateTime creationTimeUtc;

		private ProcessingStatus status;

		private bool isActive;

		private long startOfInactivityInTicks;

		private bool inactiveTimeCounted;

		private object syncObject;

		private FileInfo fileInfo;

		private DateTime lastProcessedTime;

		private long fileSizeAtLastDirectoryCheck;

		private IWatermarkFile waterMarkFile;
	}
}
