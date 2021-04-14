using System;

namespace Microsoft.Exchange.LogUploader
{
	internal interface ILogFileInfo
	{
		DateTime LastWriteTimeUtc { get; }

		DateTime CreationTimeUtc { get; }

		ProcessingStatus Status { get; set; }

		string FileName { get; }

		string FullFileName { get; }

		bool IsActive { get; set; }

		long Size { get; }

		IWatermarkFile WatermarkFileObj { get; }
	}
}
