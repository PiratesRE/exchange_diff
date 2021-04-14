using System;

namespace Microsoft.Exchange.OABRequestHandler
{
	public enum OABDownloadRequestMetadata
	{
		NoOfRequestsOutStanding,
		OfflineAddressBookGuid,
		Domain,
		LastTouchedTime,
		LastRequestedTime,
		FileRequested,
		FailureCode,
		IsAddressListDeleted
	}
}
