using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateUMCallDataRecordResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateUMCallDataRecordResponseMessage : ResponseMessage
	{
		public CreateUMCallDataRecordResponseMessage()
		{
		}

		internal CreateUMCallDataRecordResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.CreateUMCallDataRecordResponseMessage;
		}
	}
}
