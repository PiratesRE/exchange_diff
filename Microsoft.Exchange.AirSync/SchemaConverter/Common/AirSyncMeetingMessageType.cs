using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal enum AirSyncMeetingMessageType
	{
		Unspecified,
		NewMeetingRequest,
		FullUpdate,
		InformationalUpdate,
		Outdated,
		PrincipalWantsCopy,
		DelegatedCopy
	}
}
