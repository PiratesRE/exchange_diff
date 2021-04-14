using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SendItemResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SendItemResponse : BaseResponseMessage
	{
		public SendItemResponse() : base(ResponseType.SendItemResponseMessage)
		{
		}
	}
}
