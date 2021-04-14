using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum CowClientOperationSensitivity
	{
		Skip = 0,
		Capture = 1,
		PerformOperation = 2,
		CaptureAndPerformOperation = 3
	}
}
