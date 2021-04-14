using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DatabaseSeedStatus
	{
		public int ProgressPercentage { get; private set; }

		public ByteQuantifiedSize BytesRead { get; private set; }

		public ByteQuantifiedSize BytesWritten { get; private set; }

		public ByteQuantifiedSize BytesReadPerSec { get; private set; }

		public ByteQuantifiedSize BytesWrittenPerSec { get; private set; }

		internal DatabaseSeedStatus(int percent, long kbytesRead, long kbytesWritten, float kbytesReadPerSec, float kbytesWrittenPerSec)
		{
			this.ProgressPercentage = percent;
			this.BytesRead = ByteQuantifiedSize.FromKB((ulong)kbytesRead);
			this.BytesWritten = ByteQuantifiedSize.FromKB((ulong)kbytesWritten);
			this.BytesReadPerSec = ByteQuantifiedSize.FromKB((ulong)kbytesReadPerSec);
			this.BytesWrittenPerSec = ByteQuantifiedSize.FromKB((ulong)kbytesWrittenPerSec);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}; {2}:{3}; {4}:{5}; {6}:{7}; {8}:{9}", new object[]
			{
				Strings.DatabaseSeedStatusLabelPercentage,
				this.ProgressPercentage,
				Strings.DatabaseSeedStatusLabelRead,
				this.BytesRead.ToString("a"),
				Strings.DatabaseSeedStatusLabelWritten,
				this.BytesWritten.ToString("a"),
				Strings.DatabaseSeedStatusLabelReadPerSec,
				this.BytesReadPerSec.ToString("a"),
				Strings.DatabaseSeedStatusLabelWrittenPerSec,
				this.BytesWrittenPerSec.ToString("a")
			});
		}

		private const string ToStringFormatStr = "{0}:{1}; {2}:{3}; {4}:{5}; {6}:{7}; {8}:{9}";
	}
}
