using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class AvailabilityMessageFaultDetail : MessageFault
	{
		internal AvailabilityMessageFaultDetail(LocalizedException exception, FaultParty faultParty)
		{
			this.serviceError = new ServiceError(exception.Message, (ResponseCodeType)exception.ErrorCode, 0, ExchangeVersion.Exchange2007);
			this.faultCode = this.GetFaultCode(faultParty);
			this.faultReason = new FaultReason(this.serviceError.MessageText);
		}

		public override FaultCode Code
		{
			get
			{
				return this.faultCode;
			}
		}

		public override bool HasDetail
		{
			get
			{
				return true;
			}
		}

		public override FaultReason Reason
		{
			get
			{
				return this.faultReason;
			}
		}

		private FaultCode GetFaultCode(FaultParty faultParty)
		{
			return (faultParty == FaultParty.Sender) ? FaultCode.CreateSenderFaultCode(this.serviceError.MessageKey.ToString(), "http://schemas.microsoft.com/exchange/services/2006/types") : FaultCode.CreateReceiverFaultCode(this.serviceError.MessageKey.ToString(), "http://schemas.microsoft.com/exchange/services/2006/types");
		}

		protected override void OnWriteDetailContents(XmlDictionaryWriter writer)
		{
			if (this.HasDetail)
			{
				writer.WriteRaw(this.GetXmlDetailString());
			}
		}

		private string GetXmlDetailString()
		{
			SafeXmlDocument xmlDocument = new SafeXmlDocument();
			XmlElement xmlElement = ServiceXml.CreateElement(xmlDocument, "DummyNode", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceXml.CreateTextElement(xmlElement, "ErrorCode", ((int)this.serviceError.MessageKey).ToString());
			return xmlElement.InnerXml;
		}

		private FaultCode faultCode;

		private ServiceError serviceError;

		private FaultReason faultReason;
	}
}
