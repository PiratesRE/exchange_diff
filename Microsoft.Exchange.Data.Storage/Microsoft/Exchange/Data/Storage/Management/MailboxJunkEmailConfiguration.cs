using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class MailboxJunkEmailConfiguration : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxJunkEmailConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[MailboxJunkEmailConfigurationSchema.Enabled];
			}
			set
			{
				this[MailboxJunkEmailConfigurationSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TrustedListsOnly
		{
			get
			{
				return (bool)this[MailboxJunkEmailConfigurationSchema.TrustedListsOnly];
			}
			set
			{
				this[MailboxJunkEmailConfigurationSchema.TrustedListsOnly] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ContactsTrusted
		{
			get
			{
				return (bool)this[MailboxJunkEmailConfigurationSchema.ContactsTrusted];
			}
			set
			{
				this[MailboxJunkEmailConfigurationSchema.ContactsTrusted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> TrustedSendersAndDomains
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxJunkEmailConfigurationSchema.TrustedSendersAndDomains];
			}
			set
			{
				this[MailboxJunkEmailConfigurationSchema.TrustedSendersAndDomains] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> BlockedSendersAndDomains
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxJunkEmailConfigurationSchema.BlockedSendersAndDomains];
			}
			set
			{
				this[MailboxJunkEmailConfigurationSchema.BlockedSendersAndDomains] = value;
			}
		}

		private static MailboxJunkEmailConfigurationSchema schema = ObjectSchema.GetInstance<MailboxJunkEmailConfigurationSchema>();
	}
}
