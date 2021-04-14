using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.WSSecurity
{
	[XmlRoot(Namespace = "http://www.w3.org/2005/08/addressing", ElementName = "To", IsNullable = false)]
	public sealed class WSAddressingToHeader : SoapHeader
	{
		public WSAddressingToHeader()
		{
		}

		internal WSAddressingToHeader(string value)
		{
			this.value = value;
			base.MustUnderstand = true;
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
