using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(GroupRecipientRow))]
	public class GroupRecipientRow : RecipientRow
	{
		public GroupRecipientRow(ReducedRecipient recipient) : base(recipient)
		{
			this.DistinguishedName = recipient.DistinguishedName;
		}

		public GroupRecipientRow(MailEnabledRecipient recipient) : base(recipient)
		{
			this.DistinguishedName = recipient.DistinguishedName;
		}

		public GroupRecipientRow(WindowsGroup group) : base(group)
		{
			this.DistinguishedName = group.DistinguishedName;
		}

		[DataMember]
		public string DistinguishedName { get; protected set; }
	}
}
