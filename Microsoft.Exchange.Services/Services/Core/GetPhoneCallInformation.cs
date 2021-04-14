using System;
using System.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetPhoneCallInformation : SingleStepServiceCommand<GetPhoneCallInformationRequest, GetPhoneCallInformationResponseMessage>
	{
		public GetPhoneCallInformation(CallContext callContext, GetPhoneCallInformationRequest request) : base(callContext, request)
		{
			this.callId = request.CallId;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetPhoneCallInformationResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetPhoneCallInformationResponseMessage> Execute()
		{
			UMCallInfoEx callInfo = null;
			using (UMClientCommon umclientCommon = new UMClientCommon(base.CallContext.AccessingPrincipal))
			{
				callInfo = umclientCommon.GetCallInfo(this.callId.Id);
			}
			return new ServiceResult<GetPhoneCallInformationResponseMessage>(new GetPhoneCallInformationResponseMessage
			{
				PhoneCallInformation = this.BuildCallInformationXml(callInfo)
			});
		}

		private XmlElement BuildCallInformationXml(UMCallInfoEx callInfo)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "PhoneCallInformation", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceXml.CreateTextElement(xmlElement, "PhoneCallState", callInfo.CallState.ToString(), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "ConnectionFailureCause", callInfo.EventCause.ToString(), "http://schemas.microsoft.com/exchange/services/2006/types");
			if (callInfo.EventCause != UMEventCause.None)
			{
				ServiceXml.CreateTextElement(xmlElement, "SIPResponseText", callInfo.ResponseText, "http://schemas.microsoft.com/exchange/services/2006/types");
				ServiceXml.CreateTextElement(xmlElement, "SIPResponseCode", callInfo.ResponseCode.ToString(), "http://schemas.microsoft.com/exchange/services/2006/types");
			}
			return xmlElement;
		}

		private PhoneCallId callId;
	}
}
