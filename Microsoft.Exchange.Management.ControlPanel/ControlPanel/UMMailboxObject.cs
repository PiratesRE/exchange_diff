using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxObject : BaseRow
	{
		public UMMailboxObject(UMMailbox mailbox) : base(mailbox)
		{
			this.Mailbox = mailbox;
		}

		public UMMailbox Mailbox { get; private set; }

		[DataMember]
		public virtual string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Mailbox.DisplayName))
				{
					return this.Mailbox.DisplayName;
				}
				return this.Mailbox.Name;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string[] CallAnsweringRulesExtensions
		{
			get
			{
				return this.Mailbox.CallAnsweringRulesExtensions.ToArray();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}
	}
}
