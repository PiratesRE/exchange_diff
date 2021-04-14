using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class AutomaticLinkCriteria
	{
		internal static bool CanMergeGALLinkState(ContactInfoForLinking contactBeingSaved, ContactInfoForLinking otherContact)
		{
			return contactBeingSaved.GALLinkState == GALLinkState.NotLinked || otherContact.GALLinkState == GALLinkState.NotLinked || (contactBeingSaved.GALLinkState == GALLinkState.NotAllowed && otherContact.GALLinkState == GALLinkState.NotAllowed) || (contactBeingSaved.GALLinkState == GALLinkState.Linked && otherContact.GALLinkState == GALLinkState.Linked && contactBeingSaved.GALLinkID == otherContact.GALLinkID);
		}

		internal static ContactLinkingOperation CanLink(ContactInfoForLinking contact1, ContactInfoForLinking contact2)
		{
			Util.ThrowOnNullArgument(contact1, "contact1");
			Util.ThrowOnNullArgument(contact2, "contact2");
			if (!AutomaticLinkCriteria.CanMergeGALLinkState(contact1, contact2))
			{
				return ContactLinkingOperation.AutoLinkSkippedConflictingGALLinkState;
			}
			if (AutomaticLinkCriteria.IsPresentInLinkRejectHistory(contact1, contact2) || AutomaticLinkCriteria.IsPresentInLinkRejectHistory(contact2, contact1))
			{
				return ContactLinkingOperation.AutoLinkSkippedInLinkRejectHistory;
			}
			return AutomaticLinkRegularContactComparer.Instance.Match(contact1, contact2);
		}

		private static bool IsPresentInLinkRejectHistory(ContactInfoForLinking contact1, ContactInfoForLinking contact2)
		{
			return contact2.LinkRejectHistory != null && contact1.PersonId != null && contact2.LinkRejectHistory.Contains(contact1.PersonId);
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;
	}
}
