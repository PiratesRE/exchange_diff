using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PropertyBagSaveFlags
	{
		Default = 0,
		IgnoreMapiComputedErrors = 1,
		IgnoreUnresolvedHeaders = 2,
		SaveFolderPropertyBagConditional = 4,
		IgnoreAccessDeniedErrors = 8,
		DisableNewXHeaderMapping = 16,
		NoChangeTracking = 32,
		ForceNotificationPublish = 64,
		ResolveSenderProperties = 128
	}
}
