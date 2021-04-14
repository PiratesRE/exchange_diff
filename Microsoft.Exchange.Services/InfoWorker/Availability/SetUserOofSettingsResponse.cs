using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetUserOofSettingsResponse : IExchangeWebMethodResponse
	{
		[XmlElement(IsNullable = false)]
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessage;
			}
			set
			{
				this.responseMessage = value;
			}
		}

		public ResponseType GetResponseType()
		{
			return ResponseType.SetUserOofSettingsResponseMessage;
		}

		public ResponseCodeType GetErrorCodeToLog()
		{
			if (this.ResponseMessage == null)
			{
				return ResponseCodeType.NoError;
			}
			return this.ResponseMessage.ResponseCode;
		}

		private ResponseMessage responseMessage;
	}
}
