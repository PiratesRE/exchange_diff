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
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailboxPlan", SupportsShouldProcess = true, DefaultParameterSetName = "MailboxPlan")]
	public sealed class NewMailboxPlan : NewMailboxOrSyncMailbox
	{
		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return base.AddressBookPolicy;
			}
			set
			{
				base.AddressBookPolicy = value;
			}
		}

		private new Guid MailboxContainerGuid
		{
			get
			{
				return base.MailboxContainerGuid;
			}
		}

		private new SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return base.ForestWideDomainControllerAffinityByExecutingUser;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxPlan(base.Name.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? false);
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		public string MailboxPlanIndex
		{
			get
			{
				return this.DataObject.MailboxPlanIndex;
			}
			set
			{
				this.DataObject.MailboxPlanIndex = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		public MailboxPlanRelease MailboxPlanRelease
		{
			get
			{
				return (MailboxPlanRelease)base.Fields["MailboxPlanRelease"];
			}
			set
			{
				base.Fields["MailboxPlanRelease"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		public bool IsPilotMailboxPlan
		{
			get
			{
				return (bool)(this.DataObject[MailboxPlanSchema.IsPilotMailboxPlan] ?? false);
			}
			set
			{
				this.DataObject[MailboxPlanSchema.IsPilotMailboxPlan] = value;
			}
		}

		private new string ExternalDirectoryObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
		}

		private new MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				base.AddOnSKUCapability = value;
			}
		}

		private new bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
		}

		private new CountryInfo UsageLocation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new SwitchParameter BypassLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BypassLiveId"] = value;
			}
		}

		private new DatabaseIdParameter Database
		{
			get
			{
				return null;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			this.isDatabaseRequired = false;
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			user.Database = ProvisioningCache.Instance.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.DatabaseContainerId, () => base.GlobalConfigSession.GetDatabasesContainerId());
			string str = ProvisioningCache.Instance.TryAddAndGetGlobalData<string>(CannedProvisioningCacheKeys.AdministrativeGroupLegDN, () => base.GlobalConfigSession.GetAdministrativeGroup().LegacyExchangeDN);
			user.MailboxRelease = MailboxRelease.None;
			user.ArchiveRelease = MailboxRelease.None;
			user.ServerLegacyDN = str + "/cn=Servers/cn=73BCAADF75B34A4781FDCDA446B76E7C";
			MailboxTaskHelper.StampMailboxRecipientTypes(user, "MailboxPlan");
			base.IsSetRandomPassword = true;
			if (this.IsDefault || ((IRecipientSession)base.DataSession).Find(null, QueryScope.SubTree, RecipientFilterHelper.MailboxPlanFilter, null, 1).Length == 0)
			{
				user.IsDefault = true;
			}
			TaskLogger.LogExit();
		}

		protected override void ValidateMailboxPlanRelease()
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADUser aduser = null;
			if (this.DataObject.IsDefault)
			{
				aduser = RecipientTaskHelper.ResetOldDefaultPlan((IRecipientSession)base.DataSession, this.DataObject.Id, this.DataObject.OrganizationalUnitRoot, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			bool flag = false;
			try
			{
				this.DataObject[MailboxPlanSchema.MailboxPlanRelease] = (base.Fields.IsModified("MailboxPlanRelease") ? this.MailboxPlanRelease : MailboxPlanRelease.AllReleases);
				base.InternalProcessRecord();
				flag = true;
			}
			finally
			{
				if (!flag && aduser != null)
				{
					aduser.IsDefault = true;
					try
					{
						base.DataSession.Save(aduser);
					}
					catch (DataSourceTransientException exception)
					{
						base.WriteError(exception, ExchangeErrorCategory.Client, null);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			ADUser aduser = (ADUser)result;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			base.WriteResult(new MailboxPlan(aduser));
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Microsoft.Exchange.Data.Directory.Management.MailboxPlan.FromDataObject((ADUser)dataObject);
		}

		private const string MailboxServerId = "73BCAADF75B34A4781FDCDA446B76E7C";
	}
}
