using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UnsubscribeResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UnsubscribeResponse : BaseResponseMessage
	{
		public UnsubscribeResponse() : base(ResponseType.UnsubscribeResponseMessage)
		{
		}
	}
}
