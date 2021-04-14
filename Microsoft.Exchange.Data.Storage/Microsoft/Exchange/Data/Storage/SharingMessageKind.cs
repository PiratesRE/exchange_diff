using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public enum SharingMessageKind
	{
		Invitation,
		Request,
		AcceptOfRequest,
		DenyOfRequest
	}
}
