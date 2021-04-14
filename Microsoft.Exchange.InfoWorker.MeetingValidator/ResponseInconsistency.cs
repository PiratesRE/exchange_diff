using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ResponseInconsistency : Inconsistency
	{
		private ResponseInconsistency()
		{
		}

		private ResponseInconsistency(string description, ResponseType attendeeResponse, ResponseType organizerResponse, ExDateTime attendeeReplyTime, ExDateTime organizerRecordedTime, CalendarValidationContext context) : base(RoleType.Organizer, description, CalendarInconsistencyFlag.Response, context)
		{
			this.ExpectedResponse = attendeeResponse;
			this.ActualResponse = organizerResponse;
			this.AttendeeReplyTime = attendeeReplyTime;
			this.OrganizerRecordedTime = organizerRecordedTime;
		}

		internal static ResponseInconsistency CreateInstance(ResponseType attendeeResponse, ResponseType organizerResponse, ExDateTime attendeeReplyTime, ExDateTime organizerRecordedTime, CalendarValidationContext context)
		{
			return ResponseInconsistency.CreateInstance(string.Empty, attendeeResponse, organizerResponse, attendeeReplyTime, organizerRecordedTime, context);
		}

		internal static ResponseInconsistency CreateInstance(string description, ResponseType attendeeResponse, ResponseType organizerResponse, ExDateTime attendeeReplyTime, ExDateTime organizerRecordedTime, CalendarValidationContext context)
		{
			return new ResponseInconsistency(description, attendeeResponse, organizerResponse, attendeeReplyTime, organizerRecordedTime, context);
		}

		internal override RumInfo CreateRumInfo(CalendarValidationContext context, IList<Attendee> attendees)
		{
			if (this.AttendeeReplyTime.Equals(ExDateTime.MinValue))
			{
				return NullOpRumInfo.CreateInstance();
			}
			switch (this.ExpectedResponse)
			{
			default:
				return NullOpRumInfo.CreateInstance();
			case ResponseType.Tentative:
			case ResponseType.Accept:
			case ResponseType.Decline:
				return ResponseRumInfo.CreateMasterInstance();
			}
		}

		internal ResponseType ExpectedResponse { get; private set; }

		internal ResponseType ActualResponse { get; private set; }

		internal ExDateTime AttendeeReplyTime { get; private set; }

		internal ExDateTime OrganizerRecordedTime { get; private set; }
	}
}
