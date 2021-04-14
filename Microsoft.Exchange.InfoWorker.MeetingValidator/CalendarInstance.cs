using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CalendarInstance
	{
		protected CalendarInstance(ExchangePrincipal remotePrincipal)
		{
			if (remotePrincipal == null)
			{
				throw new ArgumentNullException("remotePrincipal");
			}
			this.ExchangePrincipal = remotePrincipal;
		}

		internal ExchangePrincipal ExchangePrincipal { get; private set; }

		internal bool ShouldProcessMailbox { get; set; }

		internal Inconsistency LoadInconsistency { get; set; }

		internal abstract CalendarProcessingFlags? GetCalendarConfig();

		internal abstract Inconsistency GetInconsistency(CalendarValidationContext context, string fullDescription);

		internal abstract bool WouldTryToRepairIfMissing(CalendarValidationContext context, out MeetingInquiryAction predictedAction);

		internal abstract ClientIntentFlags? GetLocationIntent(CalendarValidationContext context, GlobalObjectId globalObjectId, string organizerLocation, string attendeeLocation);
	}
}
