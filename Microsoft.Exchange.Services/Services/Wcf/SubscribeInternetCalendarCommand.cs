using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SubscribeInternetCalendarCommand : ServiceCommand<CalendarActionFolderIdResponse>
	{
		public SubscribeInternetCalendarCommand(CallContext callContext, SubscribeInternetCalendarRequest request) : base(callContext)
		{
			this.request = request;
			this.request.ValidateRequest();
		}

		protected override CalendarActionFolderIdResponse InternalExecute()
		{
			CalendarActionFolderIdResponse result;
			try
			{
				result = new SubscribeInternetCalendar(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.request.ICalUrl, this.request.GroupId, this.request.CalendarName).Execute();
			}
			catch (InvalidSharingDataException ex)
			{
				SubscribeInternetCalendarCommand.TraceError(this, ex, "Bad ICal Url format");
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidUrlFormat);
			}
			catch (NotSupportedWithMailboxVersionException ex2)
			{
				SubscribeInternetCalendarCommand.TraceError(this, ex2, "Your account isn't set up to allow calendars to be added from the Internet.");
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToSubscribeToCalendar);
			}
			return result;
		}

		public static void TraceError(object source, string message)
		{
			SubscribeInternetCalendarCommand.TraceError(source, null, message);
		}

		public static void TraceError(object source, Exception ex, string message)
		{
			if (ex == null)
			{
				ExTraceGlobals.SubscribeInternetCalendarCallTracer.TraceError((long)source.GetHashCode(), message);
				return;
			}
			ExTraceGlobals.SubscribeInternetCalendarCallTracer.TraceError<string, string>((long)source.GetHashCode(), message, ex.Message, ex.StackTrace);
		}

		public static void TraceDebug(object source, string message)
		{
			ExTraceGlobals.SubscribeInternetCalendarCallTracer.TraceDebug((long)source.GetHashCode(), message);
		}

		private readonly SubscribeInternetCalendarRequest request;
	}
}
