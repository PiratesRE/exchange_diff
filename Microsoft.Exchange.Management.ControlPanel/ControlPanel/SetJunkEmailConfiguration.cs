using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetJunkEmailConfiguration : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxJunkEmailConfiguration";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string Enabled
		{
			get
			{
				return ((bool)(base[MailboxJunkEmailConfigurationSchema.Enabled] ?? true)).ToJsonString(null);
			}
			set
			{
				bool flag;
				if (value != null && bool.TryParse(value, out flag))
				{
					base[MailboxJunkEmailConfigurationSchema.Enabled] = flag;
					return;
				}
				base[MailboxJunkEmailConfigurationSchema.Enabled] = false;
			}
		}

		[DataMember]
		public bool ContactsTrusted
		{
			get
			{
				return (bool)(base[MailboxJunkEmailConfigurationSchema.ContactsTrusted] ?? false);
			}
			set
			{
				base[MailboxJunkEmailConfigurationSchema.ContactsTrusted] = value;
			}
		}

		[DataMember]
		public bool TrustedListsOnly
		{
			get
			{
				return (bool)(base[MailboxJunkEmailConfigurationSchema.TrustedListsOnly] ?? false);
			}
			set
			{
				base[MailboxJunkEmailConfigurationSchema.TrustedListsOnly] = value;
			}
		}

		[DataMember]
		public string[] TrustedSendersAndDomains
		{
			get
			{
				return (string[])(base[MailboxJunkEmailConfigurationSchema.TrustedSendersAndDomains] ?? null);
			}
			set
			{
				base[MailboxJunkEmailConfigurationSchema.TrustedSendersAndDomains] = value;
			}
		}

		[DataMember]
		public string[] BlockedSendersAndDomains
		{
			get
			{
				return (string[])(base[MailboxJunkEmailConfigurationSchema.BlockedSendersAndDomains] ?? null);
			}
			set
			{
				base[MailboxJunkEmailConfigurationSchema.BlockedSendersAndDomains] = value;
			}
		}
	}
}
