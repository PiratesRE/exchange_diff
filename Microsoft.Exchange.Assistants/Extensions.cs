using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	public static class Extensions
	{
		internal static bool Contains(this MapiEventTypeFlags eventMask, MapiEventTypeFlags eventFlag)
		{
			return (eventMask & eventFlag) != (MapiEventTypeFlags)0;
		}

		internal static bool Contains(this MapiExtendedEventFlags eventMask, MapiExtendedEventFlags eventFlag)
		{
			return (eventMask & eventFlag) != MapiExtendedEventFlags.None;
		}

		internal static bool Contains(this MailboxType mailboxFilter, MailboxType mailboxType)
		{
			return (mailboxFilter & mailboxType) != (MailboxType)0;
		}
	}
}
