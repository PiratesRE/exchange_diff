using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory
{
	public sealed class OrderedMailboxProvisioningConstraint : MailboxProvisioningConstraint
	{
		public OrderedMailboxProvisioningConstraint()
		{
		}

		public OrderedMailboxProvisioningConstraint(int index, string value) : base(value)
		{
			this.Index = index;
		}

		[XmlAttribute("P")]
		public int Index { get; set; }
	}
}
