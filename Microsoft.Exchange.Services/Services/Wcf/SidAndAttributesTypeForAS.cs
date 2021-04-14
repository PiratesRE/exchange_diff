using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlType(TypeName = "SidAndAttributesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SidAndAttributesTypeForAS
	{
		[XmlElement(Order = 0, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string SecurityIdentifier
		{
			get
			{
				return this.securityIdentifierField;
			}
			set
			{
				this.securityIdentifierField = value;
			}
		}

		[XmlAttribute]
		public uint Attributes
		{
			get
			{
				return this.attributesField;
			}
			set
			{
				this.attributesField = value;
			}
		}

		private string securityIdentifierField;

		private uint attributesField;
	}
}
