using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecurrenceInconsistency : Inconsistency
	{
		private RecurrenceInconsistency()
		{
		}

		private RecurrenceInconsistency(RoleType owner, string description, CalendarInconsistencyFlag flag, RecurrenceInconsistencyType recurrenceInconsistencyType, ExDateTime origstartDate, CalendarValidationContext context) : base(owner, description, flag, context)
		{
			this.InconsistencyType = recurrenceInconsistencyType;
			this.OriginalStartDate = origstartDate;
		}

		internal static RecurrenceInconsistency CreateInstance(RoleType owner, string description, CalendarInconsistencyFlag flag, RecurrenceInconsistencyType recurrenceInconsistencyType, ExDateTime origstartDate, CalendarValidationContext context)
		{
			return new RecurrenceInconsistency(owner, description, flag, recurrenceInconsistencyType, origstartDate, context);
		}

		internal override RumInfo CreateRumInfo(CalendarValidationContext context, IList<Attendee> attendees)
		{
			CalendarInconsistencyFlag flag = base.Flag;
			if (flag == CalendarInconsistencyFlag.ExtraOccurrenceDeletion)
			{
				return MissingAttendeeItemRumInfo.CreateOccurrenceInstance(this.OriginalStartDate, attendees, base.Flag);
			}
			return base.CreateRumInfo(context, attendees);
		}

		internal RecurrenceInconsistencyType InconsistencyType { get; set; }

		internal ExDateTime OriginalStartDate { get; set; }

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Owner", base.Owner.ToString());
			writer.WriteElementString("Description", base.Description.ToString());
			writer.WriteElementString("InconsistencyType", this.InconsistencyType.ToString());
			writer.WriteElementString("OriginalStartDate", this.OriginalStartDate.ToString());
		}
	}
}
