using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal abstract class LogDataBatch
	{
		public LogDataBatch(int batchSizeInBytes, long beginOffSet, string fullLogName, string logPrefix)
		{
			this.InitializeBatch(batchSizeInBytes, beginOffSet, fullLogName, logPrefix);
		}

		public long Size
		{
			get
			{
				return this.numOfBytesRead;
			}
		}

		public ProcessingStatus ProcessingStatus
		{
			get
			{
				return this.processingStatus;
			}
			set
			{
				this.processingStatus = value;
			}
		}

		public int RangeCount
		{
			get
			{
				return this.logRanges.Count;
			}
		}

		public long EndOffset
		{
			get
			{
				if (this.RangeCount > 0)
				{
					return this.logRanges[this.RangeCount - 1].EndOffset;
				}
				return -1L;
			}
		}

		public string LogName
		{
			get
			{
				return Path.GetFileName(this.fullLogName);
			}
		}

		public LogFileRange CurrentRange
		{
			get
			{
				return this.currentRange;
			}
		}

		public int NumberOfLinesInBatch
		{
			get
			{
				return this.numberOfLinesInBatch;
			}
			internal set
			{
				this.numberOfLinesInBatch = value;
			}
		}

		public string FullLogName
		{
			get
			{
				return this.fullLogName;
			}
		}

		public int BatchSizeInBytes
		{
			get
			{
				return this.batchSizeInBytes;
			}
		}

		public List<LogFileRange> LogRanges
		{
			get
			{
				return this.logRanges;
			}
			protected set
			{
				this.logRanges = value;
			}
		}

		public long NumOfBytesRead
		{
			get
			{
				return this.numOfBytesRead;
			}
		}

		public string LogPrefix
		{
			get
			{
				return this.logPrefix;
			}
		}

		public string Instance
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.instance))
				{
					this.instance = this.logPrefix;
				}
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		public void CreateNewRange(long beginOffSet)
		{
			this.currentRange = new LogFileRange(beginOffSet);
			this.logRanges.Add(this.currentRange);
		}

		public void RemoveLastRange()
		{
			if (this.RangeCount > 0)
			{
				this.LogRanges.RemoveAt(this.RangeCount - 1);
				if (this.RangeCount > 0)
				{
					this.currentRange = this.LogRanges[this.RangeCount - 1];
					return;
				}
				this.currentRange = null;
			}
		}

		public void RemoveLastOpenRange()
		{
			if (this.currentRange != null && this.currentRange.EndOffset == 2147483647L)
			{
				ExTraceGlobals.ReaderTracer.TraceDebug<long, long>(1000L, "RemoveLastOpenRangeFromActiveBatch remove ({0},{1}", this.currentRange.StartOffset, this.currentRange.EndOffset);
				this.RemoveLastRange();
			}
		}

		public bool LineReceived(ReadOnlyRow row)
		{
			ArgumentValidator.ThrowIfNull("row", row);
			try
			{
				ParsedReadOnlyRow parsedReadOnlyRow = new ParsedReadOnlyRow(row);
				if (this.ShouldProcessLogLine(parsedReadOnlyRow))
				{
					this.ProcessRowData(parsedReadOnlyRow);
					this.numOfBytesRead += row.EndPosition - row.Position;
					this.numberOfLinesInBatch++;
				}
			}
			catch (InvalidLogLineException exception)
			{
				this.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception, LogUploaderEventLogConstants.Tuple_LogLineParseError, (LogUploaderEventLogConstants.Message)3221230473U, "InvalidLogLineInParse");
			}
			catch (InvalidPropertyValueException exception2)
			{
				this.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception2, LogUploaderEventLogConstants.Tuple_InvalidPropertyValueInParse, (LogUploaderEventLogConstants.Message)3221230481U, "InvalidPropertyValueInParse");
			}
			catch (MissingPropertyException exception3)
			{
				this.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception3, LogUploaderEventLogConstants.Tuple_MissingPropertyInParse, (LogUploaderEventLogConstants.Message)3221230480U, "MissingPropertyInParse");
			}
			catch (InvalidCastException exception4)
			{
				this.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception4, LogUploaderEventLogConstants.Tuple_InvalidCastInParse, (LogUploaderEventLogConstants.Message)3221230482U, "InvalidCastInParse");
			}
			catch (FailedToRetrieveRegionTagException)
			{
				return false;
			}
			return true;
		}

		public virtual bool IsBatchFull(ReadOnlyRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row", "row must not be null.");
			}
			long num = row.EndPosition - row.Position;
			return this.numOfBytesRead + num > (long)this.batchSizeInBytes && this.numberOfLinesInBatch != 0;
		}

		internal void LogErrorAndUpdatePerfCounter(long rowStartOffset, long rowEndOffset, Exception exception, ExEventLog.EventTuple eventTuple, LogUploaderEventLogConstants.Message message, string component)
		{
			string text = string.Format("Failed to parse log {0} at row ({1}, {2}): \nException: {3}", new object[]
			{
				this.FullLogName,
				rowStartOffset,
				rowEndOffset,
				exception
			});
			ExTraceGlobals.ParserTracer.TraceError((long)this.GetHashCode(), text);
			EventLogger.Logger.LogEvent(eventTuple, exception.Message, new object[]
			{
				text
			});
			PerfCountersInstanceCache.GetInstance(this.Instance).TotalInvalidLogLineParseErrors.Increment();
			EventNotificationItem.Publish(ExchangeComponent.Name, component, null, text, ResultSeverityLevel.Error, false);
			ServiceLogger.LogError(ServiceLogger.Component.LogDataBatch, message, text, this.Instance, this.FullLogName);
			PerfCountersInstanceCache.GetInstance(this.Instance).TotalParseErrors.Increment();
		}

		internal void SetCurrentRangeEndOffset(long offset)
		{
			this.currentRange.EndOffset = offset;
		}

		internal void AddRangeToBufferedBatch(long start, long end)
		{
			if (this.CurrentRange != null && this.CurrentRange.EndOffset == start)
			{
				this.CurrentRange.EndOffset = end;
				return;
			}
			this.CreateNewRange(start, end, ProcessingStatus.ReadyToWriteToDatabase);
		}

		internal void StartNewRangeForActiveBatch(long startOffset)
		{
			if (this.currentRange.EndOffset == 2147483647L)
			{
				this.currentRange.StartOffset = startOffset;
				return;
			}
			this.CreateNewRange(startOffset);
		}

		internal void UpdateCurrentRangeOffsets(long startOffset, long endOffset)
		{
			if (this.currentRange.EndOffset == startOffset)
			{
				this.currentRange.EndOffset = endOffset;
				return;
			}
			if (this.currentRange.EndOffset == 2147483647L)
			{
				if (this.currentRange.StartOffset != 0L)
				{
					this.currentRange.StartOffset = startOffset;
				}
				this.currentRange.EndOffset = endOffset;
				return;
			}
			this.CreateNewRange(startOffset, endOffset, ProcessingStatus.ReadyToWriteToDatabase);
		}

		protected abstract bool ShouldProcessLogLine(ParsedReadOnlyRow parsedRow);

		protected abstract void ProcessRowData(ParsedReadOnlyRow rowData);

		protected void CheckIfArgumentNegative(string name, long value)
		{
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException(name);
			}
		}

		private void InitializeBatch(int batchSizeInBytes, long beginOffSet, string fullLogName, string logPrefix)
		{
			if (batchSizeInBytes <= 0)
			{
				throw new ArgumentOutOfRangeException("batchSizeInBytes", "The byte count limit must be positive.");
			}
			this.CheckIfArgumentNegative("beginOffSet", beginOffSet);
			if (string.IsNullOrEmpty(fullLogName))
			{
				throw new ArgumentException("fullLogName", "logName cannot be null or empty");
			}
			this.batchSizeInBytes = batchSizeInBytes;
			this.LogRanges = new List<LogFileRange>();
			this.numberOfLinesInBatch = 0;
			this.numOfBytesRead = 0L;
			this.fullLogName = fullLogName;
			this.ProcessingStatus = ProcessingStatus.InProcessing;
			this.logPrefix = logPrefix;
			this.CreateNewRange(beginOffSet);
		}

		private void CreateNewRange(long beginOffSet, long endOffset, ProcessingStatus processingStatus)
		{
			this.currentRange = new LogFileRange(beginOffSet, endOffset, processingStatus);
			this.logRanges.Add(this.currentRange);
		}

		private int batchSizeInBytes;

		private string fullLogName;

		private List<LogFileRange> logRanges;

		private LogFileRange currentRange;

		private int numberOfLinesInBatch;

		private long numOfBytesRead;

		private ProcessingStatus processingStatus;

		private string logPrefix;

		private string instance;
	}
}
