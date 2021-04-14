using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum EventTrigger
	{
		RecipientWell = 1,
		AutoSave = 2,
		Save = 4,
		AttachmentAdded = 8,
		AttachmentRemoved = 16,
		OpenDraft = 32,
		Undo = 64
	}
}
