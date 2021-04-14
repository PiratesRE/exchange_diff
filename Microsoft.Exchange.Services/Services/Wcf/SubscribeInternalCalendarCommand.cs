using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SubscribeInternalCalendarCommand : ServiceCommand<CalendarActionFolderIdResponse>
	{
		public SubscribeInternalCalendarCommand(CallContext callContext, SubscribeInternalCalendarRequest request) : base(callContext)
		{
			this.request = request;
			this.request.ValidateRequest();
		}

		protected override CalendarActionFolderIdResponse InternalExecute()
		{
			return new SubscribeInternalCalendar(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.request.Recipient, this.request.GroupId).Execute();
		}

		public static void TraceError(object source, string message)
		{
			SubscribeInternalCalendarCommand.TraceError(source, null, message);
		}

		public static void TraceError(object source, Exception ex, string message)
		{
			if (ex == null)
			{
				ExTraceGlobals.SubscribeInternalCalendarCallTracer.TraceError((long)source.GetHashCode(), message);
				return;
			}
			ExTraceGlobals.SubscribeInternalCalendarCallTracer.TraceError<string, string>((long)source.GetHashCode(), message, ex.Message, ex.StackTrace);
		}

		public static void TraceDebug(object source, string message)
		{
			ExTraceGlobals.SubscribeInternalCalendarCallTracer.TraceDebug((long)source.GetHashCode(), message);
		}

		private readonly SubscribeInternalCalendarRequest request;
	}
}
