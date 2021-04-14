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
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class NewMailboxOrSyncMailbox : NewMailboxBase
	{
		protected override bool runUMSteps
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = MailboxTaskHelper.GetNameOfAcceptableLengthForMultiTenantMode(value, out this.nameWarning);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return (MailboxPlanIdParameter)base.Fields[ADRecipientSchema.MailboxPlan];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxPlan] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		public override Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				base.SKUCapability = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		public override MultiValuedProperty<Capability> AddOnSKUCapability
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

		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		public override bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				base.SKUAssigned = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		public CountryInfo UsageLocation
		{
			get
			{
				return this.DataObject.UsageLocation;
			}
			set
			{
				this.DataObject.UsageLocation = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter TargetAllMDBs
		{
			get
			{
				return (SwitchParameter)(base.Fields["TargetAllMDBs"] ?? true);
			}
			set
			{
				base.Fields["TargetAllMDBs"] = value;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new NewMailboxTaskModuleFactory();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.nameWarning != LocalizedString.Empty)
			{
				this.WriteWarning(this.nameWarning);
			}
			base.InternalBeginProcessing();
			base.CheckExclusiveParameters(new object[]
			{
				ADRecipientSchema.MailboxPlan,
				"SKUCapability"
			});
			if (this.MailboxPlan != null)
			{
				this.mailboxPlanObject = base.ProvisioningCache.TryAddAndGetOrganizationDictionaryValue<ADUser, string>(CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId, base.CurrentOrganizationId, this.MailboxPlan.RawIdentity, () => (ADUser)base.GetDataObject<ADUser>(this.MailboxPlan, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.MailboxPlan.ToString())), ExchangeErrorCategory.Client));
				this.ValidateMailboxPlanRelease();
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 395, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\NewMailbox.cs");
			Organization organization = null;
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "ITopologyConfigurationSession.GetOrgContainer", LoggerHelper.CmdletPerfMonitors))
			{
				organization = topologyConfigurationSession.GetOrgContainer();
			}
			this.isMailboxForcedReplicationDisabled = organization.IsMailboxForcedReplicationDisabled;
			TaskLogger.LogExit();
		}

		protected virtual void ValidateMailboxPlanRelease()
		{
			MailboxTaskHelper.ValidateMailboxPlanRelease(this.mailboxPlanObject, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			ADUser aduser = this.mailboxPlanObject;
			if (aduser != null)
			{
				user.MailboxPlan = aduser.Id;
			}
			else if (user.MailboxPlan != null)
			{
				if (user.MailboxPlanObject != null)
				{
					aduser = user.MailboxPlanObject;
				}
				else
				{
					MailboxPlanIdParameter mailboxPlanIdParameter = new MailboxPlanIdParameter(user.MailboxPlan);
					aduser = (ADUser)base.GetDataObject<ADUser>(mailboxPlanIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(mailboxPlanIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(mailboxPlanIdParameter.ToString())), ExchangeErrorCategory.Client);
				}
				this.mailboxPlanObject = aduser;
			}
			user.MailboxPlanObject = null;
			user.propertyBag.ResetChangeTracking(ADRecipientSchema.MailboxPlanObject);
			if (aduser != null)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxOrSyncMailbox.PrepareRecipientObject", LoggerHelper.CmdletPerfMonitors))
				{
					ADUser aduser2 = new ADUser();
					aduser2.StampPersistableDefaultValues();
					aduser2.StampDefaultValues(RecipientType.UserMailbox);
					aduser2.ResetChangeTracking();
					User.FromDataObject(aduser2).ApplyCloneableProperties(User.FromDataObject(aduser));
					Mailbox.FromDataObject(aduser2).ApplyCloneableProperties(Mailbox.FromDataObject(aduser));
					CASMailbox.FromDataObject(aduser2).ApplyCloneableProperties(CASMailbox.FromDataObject(aduser));
					UMMailbox.FromDataObject(aduser2).ApplyCloneableProperties(UMMailbox.FromDataObject(aduser));
					bool litigationHoldEnabled = user.LitigationHoldEnabled;
					ElcMailboxFlags elcMailboxFlags = user.ElcMailboxFlags;
					user.CopyChangesFrom(aduser2);
					if (base.SoftDeletedObject != null)
					{
						if (litigationHoldEnabled != user.LitigationHoldEnabled)
						{
							user.LitigationHoldEnabled = litigationHoldEnabled;
						}
						if (elcMailboxFlags != user.ElcMailboxFlags)
						{
							user.ElcMailboxFlags = elcMailboxFlags;
						}
					}
				}
			}
			base.PrepareRecipientObject(user);
			MailboxTaskHelper.WriteWarningWhenMailboxIsUnlicensed(user, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			TaskLogger.LogExit();
		}

		private LocalizedString nameWarning = LocalizedString.Empty;

		protected ADUser mailboxPlanObject;
	}
}
