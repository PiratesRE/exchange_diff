using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogFileRange
	{
		public LogFileRange(long startOffset)
		{
			if (startOffset < 0L)
			{
				throw new ArgumentOutOfRangeException("startOffset", "The start offset must be non-negative.");
			}
			this.startOffset = startOffset;
			this.endOffset = 2147483647L;
			this.processingStatus = ProcessingStatus.InProcessing;
		}

		public LogFileRange(long startOffset, long endOffset, ProcessingStatus processingStatus) : this(startOffset)
		{
			if (endOffset < startOffset)
			{
				throw new ArgumentException(string.Format("The end offset must be at least as large as the start offset. startOffset={0}, endOffset={1}", startOffset, endOffset));
			}
			this.endOffset = endOffset;
			this.processingStatus = processingStatus;
		}

		public long StartOffset
		{
			get
			{
				return this.startOffset;
			}
			internal set
			{
				if (value < 0L)
				{
					throw new InvalidLogFileRangeException(Strings.LogFileRangeNegativeStartOffset);
				}
				if (this.endOffset < value)
				{
					throw new InvalidLogFileRangeException(Strings.LogFileRangeWrongOffsets(value, this.endOffset));
				}
				this.startOffset = value;
			}
		}

		public long EndOffset
		{
			get
			{
				return this.endOffset;
			}
			set
			{
				if (value < 0L)
				{
					throw new InvalidLogFileRangeException(Strings.LogFileRangeNegativeEndOffset);
				}
				if (this.startOffset > value)
				{
					throw new InvalidLogFileRangeException(Strings.LogFileRangeWrongOffsets(this.startOffset, value));
				}
				this.endOffset = value;
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

		public int Size
		{
			get
			{
				return (int)(this.EndOffset - this.startOffset);
			}
		}

		public static LogFileRange Parse(string line)
		{
			if (line == null)
			{
				throw new ArgumentNullException("line");
			}
			string[] array = line.Split(new char[]
			{
				','
			});
			if (array.Length != 2)
			{
				throw new MalformedLogRangeLineException(Strings.MalformedLogRangeLine(line));
			}
			LogFileRange result;
			try
			{
				long num = long.Parse(array[0]);
				long num2 = long.Parse(array[1]);
				if (num < 0L || num2 < 0L || num > num2)
				{
					throw new MalformedLogRangeLineException(Strings.MalformedLogRangeLine(line));
				}
				result = new LogFileRange(num, num2, ProcessingStatus.CompletedProcessing);
			}
			catch (FormatException)
			{
				throw new MalformedLogRangeLineException(Strings.MalformedLogRangeLine(line));
			}
			catch (OverflowException)
			{
				throw new MalformedLogRangeLineException(Strings.MalformedLogRangeLine(line));
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", this.startOffset, this.EndOffset);
		}

		public bool IsAdjacentTo(LogFileRange other)
		{
			return this.EndOffset == other.startOffset || other.EndOffset == this.startOffset;
		}

		public bool Overlaps(LogFileRange other)
		{
			return (this.startOffset <= other.StartOffset && other.StartOffset < this.EndOffset) || (other.StartOffset <= this.startOffset && this.startOffset < other.EndOffset);
		}

		public bool Overlaps(IEnumerable<LogFileRange> others)
		{
			foreach (LogFileRange other in others)
			{
				if (this.Overlaps(other))
				{
					return true;
				}
			}
			return false;
		}

		public void Merge(LogFileRange other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other", "The other log file range must not be null.");
			}
			if (this.ProcessingStatus != other.ProcessingStatus)
			{
				throw new ArgumentException("The two log file ranges must have the same processing status.", "other");
			}
			string message = Strings.MergeLogRangesFailed(other.StartOffset, other.EndOffset, this.startOffset, this.EndOffset);
			if (this.Overlaps(other))
			{
				throw new IllegalRangeMergeException(message);
			}
			if (!this.IsAdjacentTo(other))
			{
				throw new IllegalRangeMergeException(message);
			}
			if (this.EndOffset == other.StartOffset)
			{
				this.EndOffset = other.EndOffset;
				return;
			}
			this.startOffset = other.StartOffset;
		}

		public LogFileRange Split(long splitOffset)
		{
			if (this.startOffset >= splitOffset || splitOffset >= this.EndOffset)
			{
				string paramName = string.Format(CultureInfo.InvariantCulture, "Argument ({0}) must be greater than startOffset ({1}) and less than endOffset ({2})", new object[]
				{
					splitOffset,
					this.startOffset,
					this.EndOffset
				});
				throw new ArgumentOutOfRangeException(paramName, "other");
			}
			LogFileRange result = new LogFileRange(splitOffset, this.EndOffset, ProcessingStatus.NeedProcessing);
			this.EndOffset = splitOffset;
			return result;
		}

		public bool Equals(LogFileRange other)
		{
			return this.startOffset == other.startOffset && this.endOffset == other.endOffset && this.processingStatus == other.processingStatus;
		}

		private long startOffset;

		private long endOffset;

		private ProcessingStatus processingStatus;
	}
}
