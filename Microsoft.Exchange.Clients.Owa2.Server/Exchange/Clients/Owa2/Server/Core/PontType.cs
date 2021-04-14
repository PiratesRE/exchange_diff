using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum PontType
	{
		None = 0,
		ExternalLink = 1,
		DeleteFlaggedMessage = 2,
		DeleteFlaggedContacts = 4,
		DeleteFlaggedItems = 8,
		DeleteOutlookDisabledRules = 16,
		DisabledRulesLeft = 32,
		DeleteConversation = 64,
		IgnoreConversation = 128,
		CancelIgnoreConversation = 256,
		FilteredByUnread = 512,
		DeleteAllClutter = 1024,
		All = 2147483647
	}
}
