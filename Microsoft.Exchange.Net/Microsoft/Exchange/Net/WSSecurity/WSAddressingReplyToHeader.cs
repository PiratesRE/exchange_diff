using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.WSSecurity
{
	[XmlRoot(Namespace = "http://www.w3.org/2005/08/addressing", ElementName = "ReplyTo", IsNullable = false)]
	public sealed class WSAddressingReplyToHeader : SoapHeader
	{
		public WSAddressingReplyToHeader()
		{
		}

		[XmlElement]
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		private WSAddressingReplyToHeader(string address)
		{
			this.address = address;
		}

		private const string AnonymousReplyToLocation = "http://www.w3.org/2005/08/addressing/anonymous";

		private string address;

		public static WSAddressingReplyToHeader Anonymous = new WSAddressingReplyToHeader("http://www.w3.org/2005/08/addressing/anonymous");
	}
}
