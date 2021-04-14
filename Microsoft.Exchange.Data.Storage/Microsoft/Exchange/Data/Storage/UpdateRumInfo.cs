using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpdateRumInfo : OrganizerRumInfo
	{
		private UpdateRumInfo() : this(null, null, CalendarInconsistencyFlag.None)
		{
		}

		protected UpdateRumInfo(ExDateTime? originalStartTime, IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag) : base(RumType.Update, originalStartTime, attendees)
		{
			EnumValidator<CalendarInconsistencyFlag>.ThrowIfInvalid(inconsistencyFlag, "inconsistencyFlag");
			this.InconsistencyFlagList = new List<CalendarInconsistencyFlag>(1)
			{
				inconsistencyFlag
			};
		}

		public static UpdateRumInfo CreateMasterInstance(IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag)
		{
			return new UpdateRumInfo(null, attendees, inconsistencyFlag);
		}

		public static UpdateRumInfo CreateOccurrenceInstance(ExDateTime originalStartTime, IList<Attendee> attendees, CalendarInconsistencyFlag inconsistencyFlag)
		{
			return new UpdateRumInfo(new ExDateTime?(originalStartTime), attendees, inconsistencyFlag);
		}

		public IEnumerable<CalendarInconsistencyFlag> InconsistencyFlagList { get; private set; }

		protected override void Merge(RumInfo infoToMerge)
		{
			if (infoToMerge is UpdateRumInfo)
			{
				base.Merge(infoToMerge);
				UpdateRumInfo updateRumInfo = (UpdateRumInfo)infoToMerge;
				this.InconsistencyFlagList = this.InconsistencyFlagList.Union(updateRumInfo.InconsistencyFlagList);
				return;
			}
			string message = string.Format("An update RUM can be merged only with another update RUM. RumInfo type is {0}", infoToMerge.GetType());
			throw new ArgumentException(message, "infoToMerge");
		}
	}
}
