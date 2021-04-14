using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMCallDataRecords : SingleStepServiceCommand<GetUMCallDataRecordsRequest, GetUMCallDataRecordsResponseMessage>
	{
		public GetUMCallDataRecords(CallContext callContext, GetUMCallDataRecordsRequest request) : base(callContext, request)
		{
			this.startTime = request.StartDateTime;
			this.endTime = request.EndDateTime;
			this.offset = request.Offset;
			this.numberOfRecords = request.NumberOfRecords;
			this.userLegacyExchangeDN = request.UserLegacyExchangeDN;
			this.filterBy = request.FilterBy;
			this.ValidateArguments();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMCallDataRecordsResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMCallDataRecordsResponseMessage> Execute()
		{
			CDRData[] callDataRecords;
			using (XSOUMCallDataRecordAccessor xsoumcallDataRecordAccessor = new XSOUMCallDataRecordAccessor(base.MailboxIdentityMailboxSession))
			{
				if (this.filterBy == UMCDRFilterByType.FilterByUser)
				{
					callDataRecords = xsoumcallDataRecordAccessor.GetUMCallDataRecordsForUser(this.userLegacyExchangeDN);
				}
				else
				{
					ExDateTime startDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.startTime.Year, this.startTime.Month, this.startTime.Day);
					ExDateTime endDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.endTime.Year, this.endTime.Month, this.endTime.Day);
					callDataRecords = xsoumcallDataRecordAccessor.GetUMCallDataRecords(startDateTime, endDateTime, this.offset, this.numberOfRecords);
				}
			}
			return new ServiceResult<GetUMCallDataRecordsResponseMessage>(new GetUMCallDataRecordsResponseMessage
			{
				CallDataRecords = callDataRecords
			});
		}

		private void ValidateArguments()
		{
			UMCDRFilterByType umcdrfilterByType = this.filterBy;
		}

		private readonly DateTime startTime;

		private readonly DateTime endTime;

		private readonly int offset;

		private readonly int numberOfRecords;

		private readonly string userLegacyExchangeDN;

		private UMCDRFilterByType filterBy;
	}
}
