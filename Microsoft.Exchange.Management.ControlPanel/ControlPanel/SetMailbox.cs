using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetMailbox : SetObjectProperties
	{
		public IEnumerable<string> EmailAddresses { get; set; }

		[DataMember]
		public string MailTip
		{
			get
			{
				return (string)base["MailTip"];
			}
			set
			{
				base["MailTip"] = value;
			}
		}

		[DataMember]
		public string MailboxPlan
		{
			get
			{
				return (string)base[ADRecipientSchema.MailboxPlan];
			}
			set
			{
				base[ADRecipientSchema.MailboxPlan] = value;
			}
		}

		[DataMember]
		public string RoleAssignmentPolicy
		{
			get
			{
				return (string)base[MailboxSchema.RoleAssignmentPolicy];
			}
			set
			{
				base[MailboxSchema.RoleAssignmentPolicy] = value;
			}
		}

		[DataMember]
		public string RetentionPolicy
		{
			get
			{
				return (string)base[MailboxSchema.RetentionPolicy];
			}
			set
			{
				base[MailboxSchema.RetentionPolicy] = value;
			}
		}

		[DataMember]
		public string ResourceCapacity
		{
			get
			{
				return (string)base[MailboxSchema.ResourceCapacity];
			}
			set
			{
				base[MailboxSchema.ResourceCapacity] = value;
			}
		}

		[DataMember]
		public bool? LitigationHoldEnabled
		{
			get
			{
				if (base[MailboxSchema.LitigationHoldEnabled] != null)
				{
					return new bool?((bool)base[MailboxSchema.LitigationHoldEnabled]);
				}
				return null;
			}
			set
			{
				base[MailboxSchema.LitigationHoldEnabled] = value;
			}
		}

		[DataMember]
		public string RetentionComment
		{
			get
			{
				return (string)base[MailboxSchema.RetentionComment];
			}
			set
			{
				base[MailboxSchema.RetentionComment] = value;
			}
		}

		[DataMember]
		public string RetentionUrl
		{
			get
			{
				return (string)base[MailboxSchema.RetentionUrl];
			}
			set
			{
				base[MailboxSchema.RetentionUrl] = value;
			}
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

		public void UpdateEmailAddresses(Mailbox mailbox)
		{
			ProxyAddressCollection emailAddresses = mailbox.EmailAddresses;
			for (int i = emailAddresses.Count - 1; i >= 0; i--)
			{
				if (emailAddresses[i] is SmtpProxyAddress && !((SmtpProxyAddress)emailAddresses[i]).IsPrimaryAddress)
				{
					emailAddresses.RemoveAt(i);
				}
			}
			if (this.EmailAddresses != null)
			{
				foreach (string text in this.EmailAddresses)
				{
					ProxyAddress proxyAddress = ProxyAddress.Parse(text);
					if (proxyAddress is InvalidProxyAddress)
					{
						InvalidProxyAddress invalidProxyAddress = proxyAddress as InvalidProxyAddress;
						throw new FaultException(invalidProxyAddress.ParseException.Message);
					}
					if (emailAddresses.Contains(proxyAddress))
					{
						throw new FaultException(string.Format(OwaOptionStrings.DuplicateProxyAddressError, text));
					}
					emailAddresses.Add(proxyAddress);
				}
			}
			base[MailEnabledRecipientSchema.EmailAddresses] = emailAddresses;
		}
	}
}
