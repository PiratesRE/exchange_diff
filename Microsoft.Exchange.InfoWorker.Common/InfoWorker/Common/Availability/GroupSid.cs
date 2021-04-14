using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class GroupSid
	{
		public override string ToString()
		{
			return string.Format("Security Identifier: {0}, Attributes: {1:x}", this.SecurityIdentifier, this.Attributes);
		}

		[XmlElement]
		public string SecurityIdentifier;

		[XmlAttribute]
		public uint Attributes;
	}
}
