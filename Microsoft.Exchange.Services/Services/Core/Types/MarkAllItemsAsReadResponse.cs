using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("MarkAllItemsAsReadResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class MarkAllItemsAsReadResponse : BaseResponseMessage
	{
		public MarkAllItemsAsReadResponse() : base(ResponseType.MarkAllItemsAsReadResponseMessage)
		{
		}
	}
}
