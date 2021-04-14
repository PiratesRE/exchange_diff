using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetTimeZoneOffsets : SingleStepServiceCommand<GetTimeZoneOffsetsRequest, GetTimeZoneOffsetsResponseMessage>
	{
		public GetTimeZoneOffsets(CallContext callContext, GetTimeZoneOffsetsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.responseMessage.Initialize(base.Result.Code, base.Result.Error);
			return this.responseMessage;
		}

		internal override ServiceResult<GetTimeZoneOffsetsResponseMessage> Execute()
		{
			ExTraceGlobals.GetTimeZoneOffsetsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "GetTimeZoneOffsets.Execute: User '{0}'", base.CallContext.EffectiveCaller.PrimarySmtpAddress);
			ExDateTime startTime = ExDateTime.ParseISO(base.Request.StartTime);
			ExDateTime endTime = ExDateTime.ParseISO(base.Request.EndTime);
			this.responseMessage.TimeZones = GetTimeZoneOffsetsCore.GetTheTimeZoneOffsets(startTime, endTime, base.Request.TimeZoneId);
			return new ServiceResult<GetTimeZoneOffsetsResponseMessage>(this.responseMessage);
		}

		private GetTimeZoneOffsetsResponseMessage responseMessage = new GetTimeZoneOffsetsResponseMessage();
	}
}
