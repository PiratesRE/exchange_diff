using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum MeetingMessageType
	{
		None,
		NewMeetingRequest,
		FullUpdate = 65536,
		InformationalUpdate = 131072,
		SilentUpdate = 262144,
		Outdated = 524288,
		PrincipalWantsCopy = 1048576
	}
}
