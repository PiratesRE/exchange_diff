using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class FreeBusyQuery : BaseQuery
	{
		public AttendeeKind AttendeeKind
		{
			get
			{
				return this.attendeeKind;
			}
		}

		public new FreeBusyQueryResult Result
		{
			get
			{
				return (FreeBusyQueryResult)base.Result;
			}
		}

		internal FreeBusyQuery[] GroupMembersForFreeBusy
		{
			get
			{
				return this.groupMembersForFreeBusy;
			}
		}

		internal FreeBusyQuery[] GroupMembersForSuggestions
		{
			get
			{
				return this.groupMembersForSuggestions;
			}
		}

		public static FreeBusyQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return new FreeBusyQuery(recipientData, AttendeeKind.Unknown, new FreeBusyQueryResult(exception));
		}

		public static FreeBusyQuery CreateFromIndividual(RecipientData recipientData)
		{
			return new FreeBusyQuery(recipientData, AttendeeKind.Individual, null);
		}

		public static FreeBusyQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return new FreeBusyQuery(recipientData, AttendeeKind.Individual, new FreeBusyQueryResult(exception));
		}

		public static FreeBusyQuery CreateFromGroup(RecipientData recipientData, LocalizedException exception)
		{
			return new FreeBusyQuery(recipientData, AttendeeKind.Group, new FreeBusyQueryResult(exception));
		}

		public static FreeBusyQuery CreateFromGroup(RecipientData recipientData, FreeBusyQuery[] groupMembersForFreeBusy, FreeBusyQuery[] groupMembersForSuggestions)
		{
			return new FreeBusyQuery(recipientData, AttendeeKind.Group, groupMembersForFreeBusy, groupMembersForSuggestions);
		}

		private FreeBusyQuery(RecipientData recipientData, AttendeeKind attendeeKind, FreeBusyQueryResult result) : base(recipientData, result)
		{
			this.attendeeKind = attendeeKind;
		}

		private FreeBusyQuery(RecipientData recipientData, AttendeeKind attendeeKind, FreeBusyQuery[] groupMembersForFreeBusy, FreeBusyQuery[] groupMembersForSuggestions) : base(recipientData, null)
		{
			this.attendeeKind = attendeeKind;
			this.groupMembersForFreeBusy = groupMembersForFreeBusy;
			this.groupMembersForSuggestions = groupMembersForSuggestions;
		}

		private AttendeeKind attendeeKind;

		private FreeBusyQuery[] groupMembersForFreeBusy;

		private FreeBusyQuery[] groupMembersForSuggestions;
	}
}
