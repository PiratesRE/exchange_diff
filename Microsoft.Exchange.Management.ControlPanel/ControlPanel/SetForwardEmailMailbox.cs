using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetForwardEmailMailbox : SetObjectProperties
	{
		public SetForwardEmailMailbox()
		{
			base.IgnoreNullOrEmpty = false;
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-Mailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self|Organization";
			}
		}

		[DataMember]
		public string ForwardingSmtpAddress
		{
			get
			{
				return (string)base[MailboxSchema.ForwardingSmtpAddress];
			}
			set
			{
				base[MailboxSchema.ForwardingSmtpAddress] = value;
			}
		}

		[DataMember]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)(base[MailboxSchema.DeliverToMailboxAndForward] ?? false);
			}
			set
			{
				base[MailboxSchema.DeliverToMailboxAndForward] = value;
			}
		}
	}
}
