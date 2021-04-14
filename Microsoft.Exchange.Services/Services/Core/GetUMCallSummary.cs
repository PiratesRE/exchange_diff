using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMCallSummary : SingleStepServiceCommand<GetUMCallSummaryRequest, GetUMCallSummaryResponseMessage>
	{
		public GetUMCallSummary(CallContext callContext, GetUMCallSummaryRequest request) : base(callContext, request)
		{
			this.dialPlanGuid = request.DailPlanGuid;
			this.gatewayGuid = request.GatewayGuid;
			this.groupRecordsBy = (GroupBy)Enum.Parse(typeof(GroupBy), request.GroupRecordsBy.ToString());
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMCallSummaryResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMCallSummaryResponseMessage> Execute()
		{
			UMReportRawCounters[] umcallSummary;
			using (XSOUMCallDataRecordAccessor xsoumcallDataRecordAccessor = new XSOUMCallDataRecordAccessor(base.MailboxIdentityMailboxSession))
			{
				umcallSummary = xsoumcallDataRecordAccessor.GetUMCallSummary(this.dialPlanGuid, this.gatewayGuid, this.groupRecordsBy);
			}
			return new ServiceResult<GetUMCallSummaryResponseMessage>(new GetUMCallSummaryResponseMessage
			{
				UMReportRawCountersCollection = umcallSummary
			});
		}

		private readonly Guid dialPlanGuid;

		private readonly Guid gatewayGuid;

		private readonly GroupBy groupRecordsBy;
	}
}
