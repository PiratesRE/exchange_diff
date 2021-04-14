using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum EvaluationResult
	{
		Success,
		ClientErrorInvalidStoreItemId,
		ClientErrorNoContent,
		TooManyPendingRequests,
		PermanentError,
		ClientErrorItemAlreadyBeingProcessed,
		NullOrganization,
		UnexpectedPermanentError,
		ClientErrorInvalidClientScanResult,
		ClientErrorAccessDeniedStoreItemId
	}
}
