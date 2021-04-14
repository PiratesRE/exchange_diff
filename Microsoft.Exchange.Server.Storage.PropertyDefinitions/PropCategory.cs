using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public enum PropCategory
	{
		NoGetProp,
		NoGetPropList,
		NoGetPropListForFastTransfer,
		SetPropRestricted,
		SetPropAllowedForMailboxMove,
		SetPropAllowedForAdmin,
		SetPropAllowedForTransport,
		SetPropAllowedOnEmbeddedMessage,
		FacebookProtectedProperties,
		NoCopy,
		Computed,
		IgnoreSetError,
		MessageBody,
		CAI,
		ServerOnlySyncGroupProperty,
		Sensitive,
		DoNotBumpChangeNumber,
		DoNotDeleteAtFXCopyToDestination,
		Test,
		Count
	}
}
