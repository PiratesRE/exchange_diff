using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetUserAvailabilityInternalCommand : ServiceCommand<GetUserAvailabilityInternalJsonResponse>
	{
		public GetUserAvailabilityInternalCommand(CallContext callContext, GetUserAvailabilityRequest request) : base(callContext)
		{
			this.request = request;
		}

		protected override GetUserAvailabilityInternalJsonResponse InternalExecute()
		{
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug((long)this.GetHashCode(), "Executing call to GetUserAvailability.");
			GetUserAvailability getUserAvailability = new GetUserAvailability(base.CallContext, this.request);
			GetUserAvailabilityResponse value = getUserAvailability.Execute().Value;
			ExTraceGlobals.GetUserAvailabilityInternalCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Receiving response - FreeBusyResponseArrayLength: {0}", value.FreeBusyResponseArray.Length);
			return new GetUserAvailabilityInternal(base.MailboxIdentityMailboxSession, this.request, value).Execute();
		}

		private readonly GetUserAvailabilityRequest request;
	}
}
