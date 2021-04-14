using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Attendee : RecipientBase
	{
		internal Attendee(CoreRecipient coreRecipient) : base(coreRecipient)
		{
		}

		public bool IsSendable()
		{
			bool? flag = base.Participant.IsRoutable(null);
			return flag != null && flag.Value && !this.IsOrganizer;
		}

		public bool IsOrganizer
		{
			get
			{
				return (base.RecipientFlags & RecipientFlags.Organizer) == RecipientFlags.Organizer;
			}
		}

		public ResponseType ResponseType
		{
			get
			{
				return base.GetValueOrDefault<ResponseType>(InternalSchema.RecipientTrackStatus);
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ResponseType>(value, "value");
				base[InternalSchema.RecipientTrackStatus] = (int)value;
			}
		}

		public AttendeeType AttendeeType
		{
			get
			{
				return Attendee.RecipientItemTypeToAttendeeType(base.RecipientItemType);
			}
			set
			{
				EnumValidator.ThrowIfInvalid<AttendeeType>(value, "value");
				base.RecipientItemType = Attendee.AttendeeTypeToRecipientItemType(value);
			}
		}

		public ExDateTime ReplyTime
		{
			get
			{
				ExDateTime valueOrDefault = base.GetValueOrDefault<ExDateTime>(InternalSchema.RecipientTrackStatusTime, ExDateTime.MinValue);
				if (!ExDateTime.Equals(valueOrDefault, CalendarItemBase.OutlookRtmNone))
				{
					return valueOrDefault;
				}
				return ExDateTime.MinValue;
			}
			set
			{
				base[InternalSchema.RecipientTrackStatusTime] = value;
			}
		}

		public ExDateTime ProposedStart
		{
			get
			{
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.RecipientProposedStartTime, CalendarItemBase.OutlookRtmNone);
			}
		}

		public ExDateTime ProposedEnd
		{
			get
			{
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.RecipientProposedEndTime, CalendarItemBase.OutlookRtmNone);
			}
		}

		public bool HasTimeProposal
		{
			get
			{
				return !this.ProposedStart.Equals(CalendarItemBase.OutlookRtmNone) && !this.ProposedEnd.Equals(CalendarItemBase.OutlookRtmNone);
			}
		}

		internal static AttendeeType RecipientItemTypeToAttendeeType(RecipientItemType type)
		{
			switch (type)
			{
			case RecipientItemType.To:
				return AttendeeType.Required;
			case RecipientItemType.Cc:
				return AttendeeType.Optional;
			case RecipientItemType.Bcc:
				return AttendeeType.Resource;
			default:
				ExTraceGlobals.StorageTracer.TraceDebug<RecipientItemType>(0L, "AttendeeType::MapiRecipientTypeToXsoAttendeeType. Wrong Recipienttype: {0}", type);
				return AttendeeType.Required;
			}
		}

		internal static RecipientItemType AttendeeTypeToRecipientItemType(AttendeeType attendeeType)
		{
			switch (attendeeType)
			{
			case AttendeeType.Required:
				return RecipientItemType.To;
			case AttendeeType.Optional:
				return RecipientItemType.Cc;
			case AttendeeType.Resource:
				return RecipientItemType.Bcc;
			default:
				ExTraceGlobals.StorageTracer.TraceDebug<AttendeeType>(0L, "AttendeeType::AttendeeTypeToRecipientType. Wrong Attendee.Type: {0}", attendeeType);
				throw new ArgumentException(ServerStrings.ExInvalidAttendeeType(attendeeType));
			}
		}

		internal static void SetDefaultAttendeeProperties(CoreRecipient coreRecipient)
		{
			RecipientBase.SetDefaultRecipientBaseProperties(coreRecipient);
			coreRecipient.RecipientItemType = Attendee.AttendeeTypeToRecipientItemType(AttendeeType.Required);
			coreRecipient.PropertyBag[InternalSchema.RecipientTrackStatus] = 0;
		}

		internal string GetAttendeeKey()
		{
			return (base.Participant.RoutingType + ":" + base.Participant.EmailAddress).ToUpper();
		}
	}
}
