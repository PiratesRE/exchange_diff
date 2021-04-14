using System;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal enum MailboxSlaFilterReasonType
	{
		None,
		NoGuid,
		NotInDirectory,
		MoveDestination,
		Inaccessible,
		Archive,
		NotUser,
		PublicFolder,
		InDemandJob
	}
}
