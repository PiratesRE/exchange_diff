using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateUMCallDataRecord : SingleStepServiceCommand<CreateUMCallDataRecordRequest, CreateUMCallDataRecordResponseMessage>
	{
		public CreateUMCallDataRecord(CallContext callContext, CreateUMCallDataRecordRequest request) : base(callContext, request)
		{
			this.cdrData = request.CDRData;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new CreateUMCallDataRecordResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<CreateUMCallDataRecordResponseMessage> Execute()
		{
			using (XSOUMCallDataRecordAccessor xsoumcallDataRecordAccessor = new XSOUMCallDataRecordAccessor(base.MailboxIdentityMailboxSession))
			{
				xsoumcallDataRecordAccessor.CreateUMCallDataRecord(this.cdrData);
			}
			return new ServiceResult<CreateUMCallDataRecordResponseMessage>(new CreateUMCallDataRecordResponseMessage());
		}

		private readonly CDRData cdrData;
	}
}
