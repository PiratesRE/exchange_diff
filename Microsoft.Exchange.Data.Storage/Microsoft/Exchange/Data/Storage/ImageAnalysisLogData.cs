using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ImageAnalysisLogData
	{
		public ImageAnalysisLogData(long timeMs)
		{
			this.operationTimeMs = timeMs;
			this.thumbnailWidth = 0;
			this.thumbnailHeight = 0;
			this.thumbnailSize = 0L;
		}

		public ImageAnalysisLogData(long timeMs, int thumbnailWidth, int thumbnailHeight, long thumbnailSize)
		{
			this.operationTimeMs = timeMs;
			this.thumbnailWidth = thumbnailWidth;
			this.thumbnailHeight = thumbnailHeight;
			this.thumbnailSize = thumbnailSize;
		}

		public long operationTimeMs;

		public int thumbnailWidth;

		public int thumbnailHeight;

		public long thumbnailSize;
	}
}
