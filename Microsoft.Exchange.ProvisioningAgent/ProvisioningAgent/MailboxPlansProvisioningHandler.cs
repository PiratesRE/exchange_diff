using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class MailboxPlansProvisioningHandler : ProvisioningHandlerBase
	{
		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			if (base.TaskName != "New-Mailbox" && base.TaskName != "New-SyncMailbox" && base.TaskName != "Enable-Mailbox" && base.TaskName != "Undo-SoftDeletedMailbox" && base.TaskName != "Undo-SyncSoftDeletedMailbox")
			{
				return null;
			}
			string[] array = new string[]
			{
				"Arbitration",
				"Discovery",
				"AuditLog"
			};
			foreach (string key in array)
			{
				object obj = base.UserSpecifiedParameters[key];
				if (obj != null && (SwitchParameter)obj == true)
				{
					return null;
				}
			}
			Mailbox mailbox = readOnlyIConfigurable as Mailbox;
			ADUser aduser = null;
			if (base.UserSpecifiedParameters["MailboxPlan"] == null && (mailbox == null || mailbox.MailboxPlan == null))
			{
				aduser = this.FindMailboxPlan((mailbox == null) ? null : ((ADUser)mailbox.DataObject), null);
			}
			if (aduser == null)
			{
				return null;
			}
			ADUser aduser2 = new ADUser();
			if (aduser != null)
			{
				aduser2.MailboxPlan = aduser.Id;
				aduser2.MailboxPlanObject = aduser;
			}
			if (base.TaskName == "New-SyncMailbox")
			{
				return new SyncMailbox(aduser2);
			}
			return new Mailbox(aduser2);
		}

		public override bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable)
		{
			if (base.TaskName != "New-MoveRequest" && (base.TaskName != "Update-MovedMailbox" || base.UserSpecifiedParameters["MorphToMailUser"] != null || base.UserSpecifiedParameters["Credential"] != null))
			{
				return false;
			}
			ADPresentationObject adpresentationObject = writeableIConfigurable as ADPresentationObject;
			ADUser aduser;
			if (adpresentationObject != null)
			{
				aduser = (adpresentationObject.DataObject as ADUser);
			}
			else
			{
				aduser = (writeableIConfigurable as ADUser);
			}
			if (base.UserSpecifiedParameters["MailboxPlan"] == null && aduser != null && aduser.MailboxPlan == null)
			{
				if (aduser.RecipientTypeDetails != RecipientTypeDetails.UserMailbox)
				{
					return false;
				}
				ADUser aduser2 = this.FindMailboxPlan(aduser, aduser.OrganizationId);
				if (aduser2 != null && aduser.MailboxPlan != aduser2.Id)
				{
					aduser.MailboxPlan = aduser2.Id;
					aduser.SKUCapability = aduser2.SKUCapability;
					return true;
				}
			}
			return false;
		}

		private ADUser FindMailboxPlan(ADUser user, OrganizationId userOrgId)
		{
			OrganizationId organizationId = userOrgId ?? base.UserScope.CurrentOrganizationId;
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				base.LogMessage(Strings.WarningNoDefaultMailboxPlan);
				return null;
			}
			IRecipientSession session = this.GetRecipientSession(organizationId);
			Capability? skucapability = this.GetSKUCapability(user);
			ADUser result;
			if (skucapability != null)
			{
				result = this.FindMailboxPlanWithSKUCapability(skucapability.Value, session, organizationId);
			}
			else
			{
				result = base.ProvisioningCache.TryAddAndGetOrganizationData<ADUser>(CannedProvisioningCacheKeys.DefaultMailboxPlan, organizationId, () => this.FindDefaultMailboxPlan(session));
			}
			return result;
		}

		private Capability? GetSKUCapability(ADUser user)
		{
			Capability? result = null;
			if (base.UserSpecifiedParameters["SKUCapability"] != null)
			{
				result = new Capability?((Capability)base.UserSpecifiedParameters["SKUCapability"]);
			}
			else if (user != null)
			{
				result = user.SKUCapability;
			}
			return result;
		}

		private ADUser FindMailboxPlanWithSKUCapability(Capability skuCapability, IRecipientSession recipientSession, OrganizationId organizationId)
		{
			bool checkCurrentReleasePlanFirst = RecipientTaskHelper.IsOrganizationInPilot(DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, recipientSession.SessionSettings, 266, "FindMailboxPlanWithSKUCapability", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\MailboxPlansProvisioningHandler.cs"), organizationId);
			LocalizedString message;
			ADUser aduser = MailboxTaskHelper.FindMailboxPlanWithSKUCapability(skuCapability, recipientSession, out message, checkCurrentReleasePlanFirst);
			if (aduser == null)
			{
				throw new ProvisioningException(message);
			}
			return aduser;
		}

		private ADUser FindDefaultMailboxPlan(IRecipientSession session)
		{
			ADUser[] array = session.FindADUser(null, QueryScope.SubTree, MailboxTaskHelper.defaultMailboxPlanFilter, null, 2);
			if (array.Length == 1)
			{
				return array[0];
			}
			if (array.Length > 1)
			{
				throw new ProvisioningException(Strings.ErrorTooManyDefaultMailboxPlans);
			}
			array = session.FindADUser(null, QueryScope.SubTree, MailboxTaskHelper.mailboxPlanFilter, null, 2);
			if (array.Length == 0)
			{
				throw new ProvisioningException(Strings.ErrorNoMailboxPlan);
			}
			if (array.Length > 1)
			{
				throw new ProvisioningException(Strings.ErrorNoDefaultMailboxPlan);
			}
			base.LogMessage(Strings.WarningNoDefaultMailboxPlanUsingNonDefault);
			return array[0];
		}

		private IRecipientSession GetRecipientSession(OrganizationId userOrgId)
		{
			string domainController = (base.UserSpecifiedParameters["DomainController"] != null) ? base.UserSpecifiedParameters["DomainController"].ToString() : null;
			OrganizationId currentOrganizationId = userOrgId ?? base.UserScope.CurrentOrganizationId;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), currentOrganizationId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 329, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\MailboxPlansProvisioningHandler.cs");
		}

		private const string MailboxParameterSetArbitration = "Arbitration";

		private const string MailboxParameterSetDiscovery = "Discovery";

		private const string MailboxParameterSetAuditLog = "AuditLog";
	}
}
