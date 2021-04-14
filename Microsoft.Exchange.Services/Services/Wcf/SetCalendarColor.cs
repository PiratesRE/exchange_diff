using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCalendarColor : CalendarActionBase<CalendarActionItemIdResponse>
	{
		public SetCalendarColor(MailboxSession session, StoreId calendarEntryId, CalendarColor calendarColor) : base(session)
		{
			this.calendarEntryId = calendarEntryId;
			this.calendarColor = calendarColor;
		}

		public override CalendarActionItemIdResponse Execute()
		{
			MailboxSession mailboxSession = base.MailboxSession;
			ExTraceGlobals.SetCalendarColorCallTracer.TraceDebug<StoreId, CalendarColor>((long)this.GetHashCode(), "Setting calendar color for Calendar Id: {0}. Color: {1}", this.calendarEntryId, this.calendarColor);
			CalendarActionItemIdResponse result;
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Bind(mailboxSession, this.calendarEntryId, null))
			{
				ExTraceGlobals.SetCalendarColorCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarGroupEntry bind successful - setting color. CalendarName: {0}, NodeId: {1}, CalendarId:{2}, NewColor: {3}, OldColor: {4}", new object[]
				{
					calendarGroupEntry.CalendarName,
					calendarGroupEntry.Id,
					this.calendarEntryId,
					this.calendarColor,
					calendarGroupEntry.CalendarColor
				});
				calendarGroupEntry.CalendarColor = this.calendarColor;
				ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.SetCalendarColorCallTracer.TraceDebug<string>((long)this.GetHashCode(), "CalendarGroupEntry - Unable to save new color. CalendarName: {0}", calendarGroupEntry.CalendarName);
					result = new CalendarActionItemIdResponse(CalendarActionError.CalendarActionUnableToChangeColor);
				}
				else
				{
					calendarGroupEntry.Load();
					ExTraceGlobals.SetCalendarColorCallTracer.TraceDebug<string, CalendarColor>((long)this.GetHashCode(), "SetCalendarColor command executed successfully - CalendarName: {0}, Color: {1}", calendarGroupEntry.CalendarName, this.calendarColor);
					result = new CalendarActionItemIdResponse(IdConverter.ConvertStoreItemIdToItemId(calendarGroupEntry.Id, base.MailboxSession));
				}
			}
			return result;
		}

		private readonly StoreId calendarEntryId;

		private readonly CalendarColor calendarColor;
	}
}
