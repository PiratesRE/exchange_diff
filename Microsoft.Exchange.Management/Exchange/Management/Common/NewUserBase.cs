using System;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Common
{
	public abstract class NewUserBase : NewMailEnabledRecipientObjectTask<ADUser>
	{
		protected ExPerformanceCounter NumberofCalls
		{
			get
			{
				return this.numberofCalls;
			}
			set
			{
				this.numberofCalls = value;
			}
		}

		protected ExPerformanceCounter TotalResponseTime
		{
			get
			{
				return this.totalResponseTime;
			}
			set
			{
				this.totalResponseTime = value;
			}
		}

		protected ExPerformanceCounter NumberofSuccessfulCalls
		{
			get
			{
				return this.numberofSuccessfulCalls;
			}
			set
			{
				this.numberofSuccessfulCalls = value;
			}
		}

		protected ExPerformanceCounter AverageTimeTaken
		{
			get
			{
				return this.averageTimeTaken;
			}
			set
			{
				this.averageTimeTaken = value;
			}
		}

		protected ExPerformanceCounter AverageBaseTimeTaken
		{
			get
			{
				return this.averageBaseTimeTaken;
			}
			set
			{
				this.averageBaseTimeTaken = value;
			}
		}

		protected ExPerformanceCounter AverageTimeTakenWithCache
		{
			get
			{
				return this.averageTimeTakenWithCache;
			}
			set
			{
				this.averageTimeTakenWithCache = value;
			}
		}

		protected ExPerformanceCounter AverageBaseTimeTakenWithCache
		{
			get
			{
				return this.averageBaseTimeTakenWithCache;
			}
			set
			{
				this.averageBaseTimeTakenWithCache = value;
			}
		}

		protected ExPerformanceCounter AverageTimeTakenWithoutCache
		{
			get
			{
				return this.averageTimeTakenWithoutCache;
			}
			set
			{
				this.averageTimeTakenWithoutCache = value;
			}
		}

		protected ExPerformanceCounter AverageBaseTimeTakenWithoutCache
		{
			get
			{
				return this.averageBaseTimeTakenWithoutCache;
			}
			set
			{
				this.averageBaseTimeTakenWithoutCache = value;
			}
		}

		protected ExPerformanceCounter CacheActivePercentage
		{
			get
			{
				return this.cacheActivePercentage;
			}
			set
			{
				this.cacheActivePercentage = value;
			}
		}

		protected ExPerformanceCounter CacheActiveBasePercentage
		{
			get
			{
				return this.cacheActiveBasePercentage;
			}
			set
			{
				this.cacheActiveBasePercentage = value;
			}
		}

		[Parameter]
		public string FirstName
		{
			get
			{
				return this.DataObject.FirstName;
			}
			set
			{
				this.DataObject.FirstName = value;
			}
		}

		[Parameter]
		public string Initials
		{
			get
			{
				return this.DataObject.Initials;
			}
			set
			{
				this.DataObject.Initials = value;
			}
		}

		[Parameter]
		public string LastName
		{
			get
			{
				return this.DataObject.LastName;
			}
			set
			{
				this.DataObject.LastName = value;
			}
		}

		[Parameter(Mandatory = true)]
		public virtual SecureString Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public string SamAccountName
		{
			get
			{
				return this.DataObject.SamAccountName;
			}
			set
			{
				this.DataObject.SamAccountName = value;
			}
		}

		[Parameter(Mandatory = true)]
		public virtual string UserPrincipalName
		{
			get
			{
				return this.DataObject.UserPrincipalName;
			}
			set
			{
				this.DataObject.UserPrincipalName = value;
			}
		}

		[Parameter]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)(base.Fields[ADUserSchema.PasswordLastSetRaw] ?? false);
			}
			set
			{
				base.Fields[ADUserSchema.PasswordLastSetRaw] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = true, ParameterSetName = "FederatedUser")]
		public WindowsLiveId WindowsLiveID
		{
			get
			{
				return (WindowsLiveId)base.Fields["WindowsLiveID"];
			}
			set
			{
				base.Fields["WindowsLiveID"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnableRoomMailboxAccount")]
		[Parameter(Mandatory = true, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = true, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		public WindowsLiveId MicrosoftOnlineServicesID
		{
			get
			{
				return this.WindowsLiveID;
			}
			set
			{
				this.WindowsLiveID = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveCustomDomains")]
		public SwitchParameter UseExistingLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseExistingLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseExistingLiveId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		public NetID NetID
		{
			get
			{
				return (NetID)base.Fields["NetID"];
			}
			set
			{
				base.Fields["NetID"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ImportLiveId")]
		public SwitchParameter ImportLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["ImportLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ImportLiveId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		public SwitchParameter BypassLiveId
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

		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public SwitchParameter EvictLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["EvictLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EvictLiveId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = true, ParameterSetName = "MicrosoftOnlineServicesFederatedUser")]
		public string FederatedIdentity
		{
			get
			{
				return (string)base.Fields["FederatedIdentity"];
			}
			set
			{
				base.Fields["FederatedIdentity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ImmutableId
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.ImmutableId];
			}
			set
			{
				base.Fields[ADRecipientSchema.ImmutableId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public virtual Capability SKUCapability
		{
			get
			{
				return (Capability)(base.Fields["SKUCapability"] ?? Capability.None);
			}
			set
			{
				base.VerifyValues<Capability>(CapabilityHelper.AllowedSKUCapabilities, value);
				base.Fields["SKUCapability"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public virtual MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return (MultiValuedProperty<Capability>)(base.Fields["AddOnSKUCapability"] ?? new MultiValuedProperty<Capability>());
			}
			set
			{
				if (value != null)
				{
					base.VerifyValues<Capability>(CapabilityHelper.AllowedSKUCapabilities, value.ToArray());
				}
				base.Fields["AddOnSKUCapability"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public virtual bool SKUAssigned
		{
			get
			{
				return (bool)(base.Fields[ADRecipientSchema.SKUAssigned] ?? false);
			}
			set
			{
				base.Fields[ADRecipientSchema.SKUAssigned] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return this.DataObject.Languages;
			}
			set
			{
				this.DataObject.Languages = value;
			}
		}

		protected virtual bool AllowBypassLiveIdWithoutWlid
		{
			get
			{
				return false;
			}
		}

		protected override void InternalBeginProcessing()
		{
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewUserBase.InternalBeginProcessing", LoggerHelper.CmdletPerfMonitors))
			{
				if (this.NumberofCalls != null && NewUserBase.counterCategoryExist)
				{
					this.NumberofCalls.Increment();
					if (this.NumberofCalls.RawValue >= 100000L)
					{
						this.NumberofCalls.RawValue = 1L;
						this.NumberofSuccessfulCalls.RawValue = 0L;
						this.TotalResponseTime.RawValue = 0L;
					}
				}
				if (this.WindowsLiveID != null && this.WindowsLiveID.NetId != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotProvideNetIDAndSmtpAddress), ExchangeErrorCategory.Client, null);
				}
				if (this.BypassLiveId && !this.AllowBypassLiveIdWithoutWlid)
				{
					if (this.NetID == null)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorMissingNetIDWhenBypassWLID), ExchangeErrorCategory.Client, null);
					}
					else if (this.WindowsLiveID.SmtpAddress.Equals(SmtpAddress.Empty))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorEmptyProxyAddressInWLID), ExchangeErrorCategory.Client, null);
					}
				}
				base.InternalBeginProcessing();
				if (this.NetID != null && this.WindowsLiveID.NetId != null && !this.NetID.Equals(this.WindowsLiveID.NetId))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorNetIDValuesDoNotMatch(this.NetID.ToString(), this.WindowsLiveID.NetId.ToString())), ExchangeErrorCategory.Client, null);
				}
				if (this.NetID != null && this.WindowsLiveID.NetId == null)
				{
					this.WindowsLiveID.NetId = this.NetID;
				}
				if (this.WindowsLiveID != null && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorEnableWindowsLiveIdForEnterpriseMailbox), ExchangeErrorCategory.Client, null);
				}
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.WindowsLiveID != null)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewUserBase.InternalValidate", LoggerHelper.CmdletPerfMonitors))
				{
					if (this.WindowsLiveID.NetId != null && !this.BypassLiveId)
					{
						MailboxTaskHelper.IsLiveIdExists((IRecipientSession)base.DataSession, this.WindowsLiveID.SmtpAddress, this.WindowsLiveID.NetId, new Task.ErrorLoggerDelegate(base.WriteError));
					}
					MailboxTaskHelper.CheckNameAvailability(base.TenantGlobalCatalogSession, base.Name, base.RecipientContainerId, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected virtual void StampChangesBeforeSettingPassword()
		{
		}

		protected virtual void StampChangesAfterSettingPassword()
		{
			this.DataObject.UserAccountControl = UserAccountControlFlags.NormalAccount;
		}

		protected bool IsSetRandomPassword
		{
			get
			{
				return this.isSetRandomPassword;
			}
			set
			{
				this.isSetRandomPassword = value;
			}
		}

		protected virtual void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			if (this.WindowsLiveID != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				if (base.IsDebugOn)
				{
					base.WriteDebug(Strings.DebugStartInAcceptedDomainCheck);
				}
				if (this.ShouldCheckAcceptedDomains())
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(this.ConfigurationSession, base.CurrentOrganizationId, this.WindowsLiveID.SmtpAddress.Domain, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
				}
				if (base.IsDebugOn)
				{
					base.WriteDebug(Strings.DebugEndInAcceptedDomainCheck);
				}
				this.IsSetRandomPassword = true;
				user.WindowsLiveID = this.WindowsLiveID.SmtpAddress;
				user.UserPrincipalName = this.WindowsLiveID.SmtpAddress.ToString();
			}
			else if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ValidateExternalEmailAddressInAcceptedDomain.Enabled && this.ShouldCheckAcceptedDomains())
			{
				RecipientTaskHelper.ValidateInAcceptedDomain(this.ConfigurationSession, user.OrganizationId, RecipientTaskHelper.GetDomainPartOfUserPrincalName(user.UserPrincipalName), new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			}
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			string userPrincipalName = user.UserPrincipalName;
			base.PrepareRecipientObject(user);
			bool flag = base.Fields.Contains("SoftDeletedObject");
			if (flag && userPrincipalName != user.UserPrincipalName)
			{
				user.UserPrincipalName = userPrincipalName;
			}
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewUserBase.PrepareUserObject", LoggerHelper.CmdletPerfMonitors))
			{
				this.PrepareUserObject(user);
			}
			if (!string.IsNullOrEmpty(this.ImmutableId))
			{
				this.DataObject.ImmutableId = this.ImmutableId;
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugStartUpnUniquenessCheck);
			}
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.IsUserPrincipalNameUnique", LoggerHelper.CmdletPerfMonitors))
			{
				RecipientTaskHelper.IsUserPrincipalNameUnique(base.TenantGlobalCatalogSession, user, user.UserPrincipalName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.ServerOperation, !flag);
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugEndUpnUniquenessCheck);
			}
			if (!string.IsNullOrEmpty(user.SamAccountName))
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.IsSamAccountNameUnique", LoggerHelper.CmdletPerfMonitors))
				{
					RecipientTaskHelper.IsSamAccountNameUnique(base.TenantGlobalCatalogSession, user, user.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client, !flag);
					goto IL_2C1;
				}
			}
			bool useRandomSuffix = this.WindowsLiveID != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty;
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugStartGeneratingUniqueSamAccountName);
			}
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.PrepareRecipientObject/VariantConfiguration", LoggerHelper.CmdletPerfMonitors))
			{
				IRecipientSession[] recipientSessions = new IRecipientSession[]
				{
					base.RootOrgGlobalCatalogSession
				};
				if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
				{
					recipientSessions = new IRecipientSession[]
					{
						base.RootOrgGlobalCatalogSession,
						base.PartitionOrRootOrgGlobalCatalogSession
					};
				}
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.GenerateUniqueSamAccountName", LoggerHelper.CmdletPerfMonitors))
				{
					user.SamAccountName = RecipientTaskHelper.GenerateUniqueSamAccountName(recipientSessions, user.Id.DomainId, RecipientTaskHelper.GetLocalPartOfUserPrincalName(user.UserPrincipalName), false, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), useRandomSuffix);
				}
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugEndGeneratingUniqueSamAccountName);
			}
			IL_2C1:
			if (string.IsNullOrEmpty(user.Alias))
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.GenerateUniqueAlias", LoggerHelper.CmdletPerfMonitors))
				{
					user.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, user.OrganizationId, string.IsNullOrEmpty(user.UserPrincipalName) ? user.SamAccountName : RecipientTaskHelper.GetLocalPartOfUserPrincalName(user.UserPrincipalName), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			if (base.Fields.IsModified("SKUCapability"))
			{
				user.SKUCapability = new Capability?(this.SKUCapability);
			}
			if (base.Fields.IsModified("AddOnSKUCapability"))
			{
				CapabilityHelper.SetAddOnSKUCapabilities(this.AddOnSKUCapability, user.PersistedCapabilities);
				RecipientTaskHelper.UpgradeArchiveQuotaOnArchiveAddOnSKU(user, user.PersistedCapabilities);
			}
			if (base.Fields.IsModified(ADRecipientSchema.SKUAssigned))
			{
				user.SKUAssigned = new bool?(this.SKUAssigned);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = base.Fields.Contains("SoftDeletedObject");
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(this.DataObject, base.DataSession, typeof(ADUser)));
			}
			if (this.WindowsLiveID != null && this.DataObject.NetID == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMissingWindowsLiveIdHandler), ExchangeErrorCategory.ServerOperation, null);
			}
			try
			{
				if (base.IsDebugOn)
				{
					base.WriteDebug(Strings.DebugStartSaveDataObject);
				}
				base.DataSession.Save(this.DataObject);
				if (base.IsDebugOn)
				{
					base.WriteDebug(Strings.DebugEndSaveDataObject);
				}
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			bool flag2 = false;
			try
			{
				if (!flag)
				{
					this.StampChangesBeforeSettingPassword();
					if (this.IsSetRandomPassword)
					{
						base.WriteVerbose(Strings.VerboseSettingPassword(this.DataObject.Id.ToString()));
						MailboxTaskHelper.SetMailboxPassword((IRecipientSession)base.DataSession, this.DataObject, null, new Task.ErrorLoggerDelegate(base.WriteError));
					}
					else if (this.Password != null)
					{
						base.WriteVerbose(Strings.VerboseSettingPassword(this.DataObject.Id.ToString()));
						((IRecipientSession)base.DataSession).SetPassword(this.DataObject, this.Password);
					}
					bool flag3 = base.Fields.IsModified(ADUserSchema.PasswordLastSetRaw) ? this.ResetPasswordOnNextLogon : this.DataObject.ResetPasswordOnNextLogon;
					bool bypassModerationCheck = this.DataObject.BypassModerationCheck;
					ADObjectId id = this.DataObject.Id;
					using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "DataSession.Read<ADUser>", LoggerHelper.CmdletPerfMonitors))
					{
						this.DataObject = (ADUser)base.DataSession.Read<ADUser>(this.DataObject.Identity);
					}
					if (this.DataObject == null || this.DataObject.Id == null)
					{
						string id2 = (id == null) ? string.Empty : id.ToString();
						base.WriteError(new RecipientTaskException(Strings.ErrorCreatedUserNotExist(id2)), ExchangeErrorCategory.ServerOperation, null);
					}
					this.DataObject.BypassModerationCheck = bypassModerationCheck;
					this.DataObject[ADUserSchema.PasswordLastSetRaw] = new long?(flag3 ? 0L : -1L);
					this.StampChangesAfterSettingPassword();
				}
				base.InternalProcessRecord();
				flag2 = !base.HasErrors;
			}
			finally
			{
				if (!flag2 && this.DataObject != null && this.DataObject.Id != null && !flag)
				{
					try
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetDeleteObjectVerboseString(this.DataObject.Id, base.DataSession, typeof(ADUser)));
						base.DataSession.Delete(this.DataObject);
					}
					catch (DataSourceTransientException innerException)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorRemoveUserFailed(this.DataObject.Id.ToString()), innerException), ExchangeErrorCategory.ServerTransient, null);
					}
					finally
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewUserBase.InternalEndProcessing", LoggerHelper.CmdletPerfMonitors))
			{
				base.InternalEndProcessing();
				this.timer.Stop();
				if (!base.HasErrors && this.TotalResponseTime != null && this.AverageTimeTaken != null && this.AverageBaseTimeTaken != null && this.NumberofSuccessfulCalls != null && this.AverageTimeTakenWithCache != null && this.AverageBaseTimeTakenWithCache != null && this.AverageTimeTakenWithoutCache != null && this.AverageBaseTimeTakenWithoutCache != null && this.CacheActivePercentage != null && this.CacheActiveBasePercentage != null && NewUserBase.counterCategoryExist)
				{
					this.AverageTimeTaken.IncrementBy(this.timer.ElapsedTicks);
					this.TotalResponseTime.IncrementBy(this.timer.ElapsedMilliseconds);
					this.AverageBaseTimeTaken.Increment();
					this.NumberofSuccessfulCalls.Increment();
					this.CacheActiveBasePercentage.Increment();
					if (base.ProvisioningCache.Enabled)
					{
						this.AverageTimeTakenWithCache.IncrementBy(this.timer.ElapsedTicks);
						this.AverageBaseTimeTakenWithCache.Increment();
						this.CacheActivePercentage.Increment();
					}
					else
					{
						this.AverageTimeTakenWithoutCache.IncrementBy(this.timer.ElapsedTicks);
						this.AverageBaseTimeTakenWithoutCache.Increment();
					}
				}
			}
			TaskLogger.LogExit();
		}

		private static readonly Guid EtsGroupSIDCacheKey = new Guid("A0AAE3BE-749D-401e-8711-0CE7DAC9B4D1");

		private SecureString password;

		private bool isSetRandomPassword;

		private static bool counterCategoryExist = PerformanceCounterCategory.Exists("MSExchange Provisioning");

		private Stopwatch timer = new Stopwatch();

		private ExPerformanceCounter numberofCalls;

		private ExPerformanceCounter numberofSuccessfulCalls;

		private ExPerformanceCounter averageTimeTaken;

		private ExPerformanceCounter averageBaseTimeTaken;

		private ExPerformanceCounter averageTimeTakenWithCache;

		private ExPerformanceCounter averageBaseTimeTakenWithCache;

		private ExPerformanceCounter averageTimeTakenWithoutCache;

		private ExPerformanceCounter averageBaseTimeTakenWithoutCache;

		private ExPerformanceCounter totalResponseTime;

		private ExPerformanceCounter cacheActivePercentage;

		private ExPerformanceCounter cacheActiveBasePercentage;
	}
}
