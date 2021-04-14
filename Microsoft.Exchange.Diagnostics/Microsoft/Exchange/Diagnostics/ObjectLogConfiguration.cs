using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class ObjectLogConfiguration
	{
		public virtual bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public virtual TimeSpan MaxLogAge
		{
			get
			{
				return TimeSpan.MaxValue;
			}
		}

		public virtual int BufferLength
		{
			get
			{
				return 0;
			}
		}

		public virtual string Note
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual bool FlushToDisk
		{
			get
			{
				return false;
			}
		}

		public virtual TimeSpan StreamFlushInterval
		{
			get
			{
				return TimeSpan.MaxValue;
			}
		}

		public abstract string LoggingFolder { get; }

		public abstract string LogComponentName { get; }

		public abstract string FilenamePrefix { get; }

		public abstract long MaxLogDirSize { get; }

		public abstract long MaxLogFileSize { get; }
	}
}
