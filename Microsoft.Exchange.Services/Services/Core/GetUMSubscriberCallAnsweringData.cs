using System;
using System.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMSubscriberCallAnsweringData : SingleStepServiceCommand<GetUMSubscriberCallAnsweringDataRequest, GetUMSubscriberCallAnsweringDataResponseMessage>
	{
		public GetUMSubscriberCallAnsweringData(CallContext callContext, GetUMSubscriberCallAnsweringDataRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMSubscriberCallAnsweringDataResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMSubscriberCallAnsweringDataResponseMessage> Execute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			ADUser user = adrecipientSession.FindADUserByObjectId(base.CallContext.AccessingADUser.ObjectId);
			TimeSpan timeout;
			try
			{
				timeout = XmlConvert.ToTimeSpan(base.Request.Timeout);
			}
			catch (FormatException innerException)
			{
				throw new InvalidValueForPropertyException((CoreResources.IDs)3078968203U, innerException);
			}
			UMSubscriberCallAnsweringData umsubscriberCallAnsweringData;
			using (XSOUMUserMailboxAccessor xsoumuserMailboxAccessor = new XSOUMUserMailboxAccessor(user, base.MailboxIdentityMailboxSession))
			{
				umsubscriberCallAnsweringData = xsoumuserMailboxAccessor.GetUMSubscriberCallAnsweringData(timeout);
			}
			GetUMSubscriberCallAnsweringDataResponseMessage getUMSubscriberCallAnsweringDataResponseMessage = new GetUMSubscriberCallAnsweringDataResponseMessage();
			if (umsubscriberCallAnsweringData != null)
			{
				getUMSubscriberCallAnsweringDataResponseMessage.IsOOF = umsubscriberCallAnsweringData.IsOOF;
				getUMSubscriberCallAnsweringDataResponseMessage.TaskTimedOut = umsubscriberCallAnsweringData.TaskTimedOut;
				getUMSubscriberCallAnsweringDataResponseMessage.IsMailboxQuotaExceeded = umsubscriberCallAnsweringData.IsMailboxQuotaExceeded;
				getUMSubscriberCallAnsweringDataResponseMessage.IsTranscriptionEnabledInMailboxConfig = umsubscriberCallAnsweringData.IsTranscriptionEnabledInMailboxConfig;
				getUMSubscriberCallAnsweringDataResponseMessage.Greeting = umsubscriberCallAnsweringData.RawGreeting;
				getUMSubscriberCallAnsweringDataResponseMessage.GreetingName = ((umsubscriberCallAnsweringData.Greeting != null) ? umsubscriberCallAnsweringData.Greeting.ExtraInfo : null);
			}
			return new ServiceResult<GetUMSubscriberCallAnsweringDataResponseMessage>(getUMSubscriberCallAnsweringDataResponseMessage);
		}
	}
}
