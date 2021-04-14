using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCalendarColorCommand : ServiceCommand<CalendarActionItemIdResponse>
	{
		public SetCalendarColorCommand(CallContext callContext, Microsoft.Exchange.Services.Core.Types.ItemId itemId, CalendarColor calendarColor) : base(callContext)
		{
			this.itemId = itemId;
			this.calendarColor = calendarColor;
		}

		protected override CalendarActionItemIdResponse InternalExecute()
		{
			if (this.itemId == null || string.IsNullOrEmpty(this.itemId.Id) || string.IsNullOrEmpty(this.itemId.ChangeKey))
			{
				ExTraceGlobals.SetCalendarColorCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Invalid calendar folderid supplied. ItemId.Id: {0}, ItemId.ChangeKey: {1}", (this.itemId == null || this.itemId.Id == null) ? "is null" : this.itemId.Id, (this.itemId == null || this.itemId.ChangeKey == null) ? "is null" : this.itemId.ChangeKey);
				return new CalendarActionItemIdResponse(CalendarActionError.CalendarActionInvalidItemId);
			}
			return new SetCalendarColor(base.MailboxIdentityMailboxSession, base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.itemId).Id, this.calendarColor).Execute();
		}

		private readonly Microsoft.Exchange.Services.Core.Types.ItemId itemId;

		private readonly CalendarColor calendarColor;
	}
}
