using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ApprovalApplicationContainerSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition RetentionPolicy = IADMailStorageSchema.RetentionPolicy;
	}
}
