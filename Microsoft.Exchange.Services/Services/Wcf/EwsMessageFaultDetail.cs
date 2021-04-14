using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class EwsMessageFaultDetail : MessageFault
	{
		internal EwsMessageFaultDetail(LocalizedException exception, FaultParty faultParty, ExchangeVersion currentExchangeVersion)
		{
			this.serviceError = ServiceErrors.GetServiceError(exception, currentExchangeVersion);
			this.faultCode = this.GetFaultCode(faultParty);
			string text = this.serviceError.MessageText;
			if (this.serviceError.MessageKey == ResponseCodeType.ErrorSchemaValidation)
			{
				text = CoreResources.GetLocalizedString((CoreResources.IDs)4281412187U);
				if (exception.InnerException != null)
				{
					text += exception.InnerException.Message;
				}
			}
			this.faultReason = new FaultReason(text);
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
				writer.WriteRaw(this.serviceError.GetAsXmlString());
			}
		}

		private FaultCode faultCode;

		private ServiceError serviceError;

		private FaultReason faultReason;
	}
}
