using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FileSystemPerformanceTracker : IDisposable
	{
		public FileSystemPerformanceTracker(string marker, IOCostStream ioCostStream, IPerformanceDataLogger logger)
		{
			if (string.IsNullOrEmpty(marker))
			{
				throw new ArgumentNullException("marker");
			}
			if (ioCostStream == null)
			{
				throw new ArgumentNullException("ioCostStream");
			}
			this.marker = marker;
			this.ioCostStream = ioCostStream;
			this.logger = logger;
		}

		public void Dispose()
		{
			if (this.logger != null)
			{
				this.logger.Log(this.marker, "FS.BytesRead", (uint)this.ioCostStream.BytesRead);
				this.logger.Log(this.marker, "FS.BytesWritten", (uint)this.ioCostStream.BytesWritten);
				this.logger.Log(this.marker, "FS.Reading.ElapsedTime", this.ioCostStream.Reading);
				this.logger.Log(this.marker, "FS.Writing.ElapsedTime", this.ioCostStream.Writing);
			}
		}

		private const string FSRead = "FS.BytesRead";

		private const string FSWritten = "FS.BytesWritten";

		private const string FSReadingTime = "FS.Reading.ElapsedTime";

		private const string FSWritingTime = "FS.Writing.ElapsedTime";

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private readonly IOCostStream ioCostStream;
	}
}
