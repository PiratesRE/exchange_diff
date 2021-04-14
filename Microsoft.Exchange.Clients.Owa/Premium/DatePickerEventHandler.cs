using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("DatePicker")]
	internal sealed class DatePickerEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterEnum(typeof(DatePicker.Features));
			OwaEventRegistry.RegisterHandler(typeof(DatePickerEventHandler));
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("RenderMonth", false, true)]
		[OwaEventParameter("uF", typeof(int), false, false)]
		[OwaEventParameter("m", typeof(ExDateTime))]
		public void RenderMonth()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "DatePickerEventHandler.RenderMonth");
			int features = (int)base.GetParameter("uF");
			ExDateTime month = (ExDateTime)base.GetParameter("m");
			DatePicker datePicker = new DatePicker(string.Empty, month, features);
			datePicker.RenderMonth(this.Writer);
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId), true, true)]
		[OwaEvent("GetFreeBusy")]
		[OwaEventParameter("m", typeof(ExDateTime), false, false)]
		public void GetFreeBusy()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "DatePickerEventHandler.GetFreeBusy");
			ExDateTime month = (ExDateTime)base.GetParameter("m");
			OwaStoreObjectId[] array;
			if (!base.IsParameterSet("fId"))
			{
				array = new OwaStoreObjectId[]
				{
					base.UserContext.CalendarFolderOwaId
				};
			}
			else
			{
				array = (OwaStoreObjectId[])base.GetParameter("fId");
				if (array.Length > 5)
				{
					throw new OwaInvalidRequestException("Too many folders");
				}
				if (array.Length == 0)
				{
					throw new OwaInvalidRequestException("Must pass at least one folder id");
				}
			}
			ExDateTime exDateTime;
			ExDateTime arg;
			DatePickerBase.GetVisibleDateRange(month, out exDateTime, out arg, base.UserContext.TimeZone);
			Duration timeWindow = new Duration((DateTime)exDateTime, (DateTime)arg.IncrementDays(1));
			ExTraceGlobals.CalendarTracer.TraceDebug<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Getting free/busy data from {0} to {1}", exDateTime, arg);
			string multiCalendarFreeBusyDataForDatePicker = Utilities.GetMultiCalendarFreeBusyDataForDatePicker(timeWindow, array, base.UserContext);
			this.Writer.Write("<div id=fb _m=\"");
			this.Writer.Write(month.Month);
			this.Writer.Write("\" _y=\"");
			this.Writer.Write(month.Year);
			this.Writer.Write("\">");
			Utilities.HtmlEncode(multiCalendarFreeBusyDataForDatePicker, this.Writer);
			this.Writer.Write("</div>");
		}

		public const string EventNamespace = "DatePicker";

		public const string MethodRenderMonth = "RenderMonth";

		public const string MethodGetFreeBusy = "GetFreeBusy";

		public const string Month = "m";

		public const string Features = "uF";

		public const string FolderId = "fId";
	}
}
