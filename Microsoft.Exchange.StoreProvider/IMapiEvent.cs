using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiEvent
	{
		MapiEventTypeFlags EventMask { get; }

		Guid MailboxGuid { get; }

		string ObjectClass { get; }

		ObjectType ItemType { get; }

		byte[] ItemEntryId { get; }

		byte[] ParentEntryId { get; }

		MapiEventFlags EventFlags { get; }

		MapiExtendedEventFlags ExtendedEventFlags { get; }

		long ItemCount { get; }

		long UnreadItemCount { get; }

		long EventCounter { get; }
	}
}
