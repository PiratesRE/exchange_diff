using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum UpdateMailboxAssociationMetadata
	{
		[DisplayName("UMA", "Guid")]
		ExchangeGuid,
		[DisplayName("UMA", "ExtId")]
		ExternalDirectoryObjectId,
		[DisplayName("UMA", "ADUserCached")]
		IsPopulateADUserInCacheSuccessful,
		[DisplayName("UMA", "MiniRecipCached")]
		IsPopulateMiniRecipientInCacheSuccessful
	}
}
