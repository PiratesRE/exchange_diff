using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.WSSecurity
{
	[XmlRoot(Namespace = "http://www.w3.org/2005/08/addressing", ElementName = "MessageID", IsNullable = false)]
	public sealed class WSAddressingMessageIDHeader : SoapHeader
	{
		internal static WSAddressingMessageIDHeader Create(string messageId)
		{
			return new WSAddressingMessageIDHeader
			{
				Value = messageId
			};
		}

		[XmlText]
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string value;
	}
}
