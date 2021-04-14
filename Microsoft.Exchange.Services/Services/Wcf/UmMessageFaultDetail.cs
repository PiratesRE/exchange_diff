using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UmMessageFaultDetail : MessageFault
	{
		internal UmMessageFaultDetail(LocalizedException exception, FaultParty faultParty)
		{
			if (exception is ObjectNotFoundException)
			{
				this.ExitFast();
				return;
			}
			this.exceptionTypeName = exception.GetType().Name;
			this.faultCode = FaultCode.CreateSenderFaultCode(new FaultCode(this.exceptionTypeName, "http://schemas.microsoft.com/exchange/services/2006/errors"));
			ServiceError serviceError = new ServiceError(exception.Message, ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2007);
			this.faultReason = new FaultReason(serviceError.MessageText);
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

		private void ExitFast()
		{
			BailOut.SetHTTPStatusAndClose(HttpStatusCode.Unauthorized);
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
			ServiceXml.CreateTextElement(xmlElement, "ExceptionType", this.exceptionTypeName);
			return xmlElement.InnerXml;
		}

		private FaultCode faultCode;

		private FaultReason faultReason;

		private string exceptionTypeName;
	}
}
