using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum OWAFileAccessFlags
	{
		DirectFileAccessEnabledMask = 1,
		WebReadyDocumentViewingEnabledMask,
		ForceWebReadyDocumentViewingFirstMask = 4,
		WacViewingEnabledMask = 8,
		ForceWacViewingFirstMask = 16
	}
}
