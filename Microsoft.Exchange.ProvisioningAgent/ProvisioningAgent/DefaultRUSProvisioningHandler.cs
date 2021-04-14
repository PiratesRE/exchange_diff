using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.DefaultProvisioningAgent.Rus;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class DefaultRUSProvisioningHandler : RUSProvisioningHandler
	{
		protected override bool UpdateRecipient(ADRecipient recipient)
		{
			string text = null;
			NetworkCredential credential = null;
			string text2;
			string text3;
			if (StringComparer.InvariantCultureIgnoreCase.Equals(base.TaskName, "Move-Mailbox"))
			{
				text2 = (string)base.UserSpecifiedParameters["GlobalCatalog"];
				text3 = (string)base.UserSpecifiedParameters["DomainController"];
				text = (string)base.UserSpecifiedParameters["GlobalCatalog"];
				PSCredential pscredential = (PSCredential)base.UserSpecifiedParameters["TargetForestCredential"];
				credential = ((pscredential != null) ? pscredential.GetNetworkCredential() : null);
			}
			else if (StringComparer.InvariantCultureIgnoreCase.Equals(base.TaskName, "Update-Recipient"))
			{
				text2 = (string)base.UserSpecifiedParameters["DomainController"];
				text3 = recipient.OriginatingServer;
				text = (string)base.UserSpecifiedParameters["DomainController"];
				PSCredential pscredential2 = (PSCredential)base.UserSpecifiedParameters["Credential"];
				credential = ((pscredential2 != null) ? pscredential2.GetNetworkCredential() : null);
			}
			else if (StringComparer.InvariantCultureIgnoreCase.Equals(base.TaskName, "Update-MovedMailbox"))
			{
				text2 = ((base.UserSpecifiedParameters["ConfigDomainController"] != null) ? base.UserSpecifiedParameters["ConfigDomainController"].ToString() : null);
				text3 = recipient.OriginatingServer;
				text = text2;
				PSCredential pscredential3 = (PSCredential)base.UserSpecifiedParameters["Credential"];
				credential = ((pscredential3 != null) ? pscredential3.GetNetworkCredential() : null);
			}
			else
			{
				text2 = ((base.UserSpecifiedParameters["DomainController"] != null) ? base.UserSpecifiedParameters["DomainController"].ToString() : null);
				text3 = recipient.OriginatingServer;
				if (string.IsNullOrEmpty(text3))
				{
					text3 = ((base.UserSpecifiedParameters["DomainController"] != null) ? base.UserSpecifiedParameters["DomainController"].ToString() : null);
				}
			}
			ExTraceGlobals.RusTracer.TraceDebug((long)this.GetHashCode(), "DefaultRUSProvisioningHandler.UpdateRecipient: recipient={0}, TaskName={1}, ConfigurationDomainController={2}, RecipientDomainController={3}, GlobalCatalog={4}, AccountPartition={5}", new object[]
			{
				recipient.Identity.ToString(),
				base.TaskName,
				text2,
				text3,
				text,
				(base.PartitionId != null) ? base.PartitionId.ForestFQDN : "<null>"
			});
			base.LogMessage(Strings.VerboseUpdateRecipientObject(recipient.Id.ToString(), text2 ?? "<null>", text3 ?? "<null>", text ?? "<null>"));
			MultiValuedProperty<ProxyAddressTemplate> specificAddressTemplates = (MultiValuedProperty<ProxyAddressTemplate>)base.UserSpecifiedParameters["SpecificAddressTemplates"];
			bool flag = new AddressBookHandler(text2, text3, text, credential, base.PartitionId, base.UserScope, base.ProvisioningCache, base.LogMessage).UpdateRecipient(recipient);
			bool flag2 = new SystemPolicyHandler(text2, text3, text, credential, base.PartitionId, base.UserScope, base.ProvisioningCache, base.LogMessage).UpdateRecipient(recipient);
			bool flag3 = new EmailAddressPolicyHandler(text2, text3, text, credential, base.PartitionId, base.UserScope, base.ProvisioningCache, false, specificAddressTemplates, base.LogMessage).UpdateRecipient(recipient);
			return flag || flag2 || flag3;
		}

		internal static readonly string[] SupportedTasks = new string[]
		{
			"enable-DistributionGroup",
			"enable-Mailbox",
			"Enable-MailContact",
			"Enable-MailUser",
			"Enable-RemoteMailbox",
			"New-DistributionGroup",
			"New-DynamicDistributionGroup",
			"New-Mailbox",
			"New-SiteMailbox",
			"New-MailboxPlan",
			"New-GroupMailbox",
			"New-MailContact",
			"New-MailUser",
			"New-RemoteMailbox",
			"New-SyncDistributionGroup",
			"New-SyncMailbox",
			"New-SyncMailContact",
			"New-SyncMailUser",
			"set-CASMailbox",
			"set-CASMailboxPlan",
			"Set-Contact",
			"Set-DistributionGroup",
			"Set-DynamicDistributionGroup",
			"Set-Group",
			"set-LinkedUser",
			"set-Mailbox",
			"Set-SiteMailbox",
			"set-MailboxPlan",
			"Set-MailContact",
			"Set-MailPublicFolder",
			"set-MailUser",
			"set-RemoteMailbox",
			"Set-SyncDistributionGroup",
			"Set-SyncMailbox",
			"Set-SyncMailContact",
			"Set-SyncMailUser",
			"Set-TextMessagingMailbox",
			"Set-UMMailbox",
			"Set-UMMailboxPlan",
			"set-User",
			"Set-UserPhoto",
			"undo-softdeletedmailbox",
			"undo-syncsoftdeletedmailbox",
			"undo-syncsoftdeletedmailuser",
			"connect-mailbox",
			"Enable-MailPublicFolder",
			"enable-SystemAttendantMailbox",
			"move-mailbox",
			"New-MicrosoftExchangeRecipient",
			"New-SyncMailPublicFolder",
			"Set-OrganizationConfig",
			"Set-ThrottlingPolicyAssociation",
			"Update-Recipient",
			"Update-MovedMailbox"
		};
	}
}
