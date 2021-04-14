using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetRecipientObjectTask<TIdentity, TPublicObject, TDataObject> : SetADTaskBase<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : ADRecipient, new()
	{
		internal LazilyInitialized<SharedTenantConfigurationState> CurrentOrgState { get; set; }

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreDefaultScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreDefaultScope"] = value;
			}
		}

		protected RecipientType DesiredRecipientType
		{
			get
			{
				return this.recipientType;
			}
			set
			{
				this.recipientType = value;
			}
		}

		protected virtual bool ShouldCheckAcceptedDomains()
		{
			return true;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 305, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\SetAdObjectTask.cs");
			if (this.IgnoreDefaultScope)
			{
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
			}
			tenantOrRootOrgRecipientSession.LinkResolutionServer = ADSession.GetCurrentConfigDC(base.SessionSettings.GetAccountOrResourceForestFqdn());
			return tenantOrRootOrgRecipientSession;
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)base.PrepareDataObject();
			if (adrecipient.IsChanged(ADRecipientSchema.PrimarySmtpAddress) && adrecipient.PrimarySmtpAddress != adrecipient.OriginalPrimarySmtpAddress && adrecipient.EmailAddressPolicyEnabled)
			{
				this.WriteWarning(Strings.WarningCannotSetPrimarySmtpAddressWhenEapEnabled);
			}
			if (RecipientTaskHelper.IsMailEnabledRecipientType(adrecipient.RecipientType) && !adrecipient.EmailAddressPolicyEnabled && adrecipient.WindowsEmailAddress != adrecipient.OriginalWindowsEmailAddress && adrecipient.PrimarySmtpAddress == adrecipient.OriginalPrimarySmtpAddress)
			{
				adrecipient.PrimarySmtpAddress = adrecipient.WindowsEmailAddress;
			}
			if (adrecipient.RecipientType == RecipientType.MailUser && (RecipientTypeDetails)adrecipient[ADRecipientSchema.RecipientTypeDetailsValue] == RecipientTypeDetails.None)
			{
				adrecipient.RecipientTypeDetails = RecipientTypeDetails.MailUser;
			}
			RecipientTaskHelper.RemoveEmptyValueFromEmailAddresses(adrecipient);
			TaskLogger.LogExit();
			return adrecipient;
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.Validate(this, this.SharedTenantConfigurationMode, this.CurrentOrgState, null);
			ADObject adobject = (ADObject)RecipientTaskHelper.ResolveDataObject<TDataObject>(base.DataSession, base.TenantGlobalCatalogSession, base.ServerSettings, this.Identity, this.RootId, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<TDataObject>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, adobject))
			{
				base.UnderscopeDataSession(adobject.OrganizationId);
				base.CurrentOrganizationId = adobject.OrganizationId;
			}
			return adobject;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.CurrentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(() => SharedConfiguration.GetSharedConfigurationState(base.CurrentOrganizationId));
			if (this.IgnoreDefaultScope && base.DomainController != null)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorIgnoreDefaultScopeAndDCSetTogether), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject
			});
			ADRecipient adrecipient = (ADRecipient)dataObject;
			this.DesiredRecipientType = adrecipient.RecipientType;
			base.StampChangesOn(dataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			SharedTenantConfigurationMode sharedTenantConfigurationMode = this.SharedTenantConfigurationMode;
			LazilyInitialized<SharedTenantConfigurationState> currentOrgState = this.CurrentOrgState;
			TIdentity identity = this.Identity;
			SharedConfigurationTaskHelper.Validate(this, sharedTenantConfigurationMode, currentOrgState, identity.ToString());
			ADObjectId adobjectId;
			if (this.IgnoreDefaultScope && !RecipientTaskHelper.IsValidDistinguishedName(this.Identity, out adobjectId))
			{
				base.WriteError(new ArgumentException(Strings.ErrorOnlyDNSupportedWithIgnoreDefaultScope), (ErrorCategory)1000, this.Identity);
			}
			base.InternalValidate();
		}

		internal sealed override void PreInternalProcessRecord()
		{
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.PreInternalProcessRecord(this, this.ConvertDataObjectToPresentationObject(this.DataObject), false);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			OrganizationId currentOrganizationId = base.CurrentOrganizationId;
			TDataObject dataObject = this.DataObject;
			if (!currentOrganizationId.Equals(dataObject.OrganizationId))
			{
				this.CurrentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(delegate()
				{
					TDataObject dataObject17 = this.DataObject;
					return SharedConfiguration.GetSharedConfigurationState(dataObject17.OrganizationId);
				});
			}
			ADRecipient adrecipient = this.DataObject;
			bool flag = adrecipient != null && adrecipient.RecipientSoftDeletedStatus > 0;
			if (RecipientTaskHelper.IsMailEnabledRecipientType(this.DesiredRecipientType) && !flag)
			{
				if (!base.IsProvisioningLayerAvailable)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), (ErrorCategory)1001, null);
				}
				TDataObject dataObject2 = this.DataObject;
				if (dataObject2.IsModified(ADRecipientSchema.EmailAddresses))
				{
					TDataObject dataObject3 = this.DataObject;
					if (dataObject3.EmailAddresses.Count > 0)
					{
						if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && this.ShouldCheckAcceptedDomains())
						{
							IDirectorySession configurationSession = this.ConfigurationSession;
							TDataObject dataObject4 = this.DataObject;
							IConfigurationSession configurationSession2 = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(configurationSession, dataObject4.OrganizationId, true);
							IConfigurationSession cfgSession = configurationSession2;
							TDataObject dataObject5 = this.DataObject;
							RecipientTaskHelper.ValidateSmtpAddress(cfgSession, dataObject5.EmailAddresses, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
						}
						ADObjectId rootOrgContainerId = base.RootOrgContainerId;
						TDataObject dataObject6 = this.DataObject;
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, dataObject6.OrganizationId, base.ExecutingUserOrganizationId, false);
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential, sessionSettings, 557, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\SetAdObjectTask.cs");
						IRecipientSession tenantCatalogSession = tenantOrRootOrgRecipientSession;
						TDataObject dataObject7 = this.DataObject;
						RecipientTaskHelper.ValidateEmailAddressErrorOut(tenantCatalogSession, dataObject7.EmailAddresses, this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
					}
				}
			}
			TDataObject dataObject8 = this.DataObject;
			if (dataObject8.IsChanged(ADObjectSchema.Id))
			{
				IDirectorySession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.OrgWideSessionSettings, ConfigScopes.TenantSubTree, 579, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\SetAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = ((IDirectorySession)base.DataSession).UseConfigNC;
				TDataObject dataObject9 = this.DataObject;
				ADObjectId parent = dataObject9.Id.Parent;
				ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(parent, new PropertyDefinition[]
				{
					ADObjectSchema.ExchangeVersion
				});
				ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)adrawEntry[ADObjectSchema.ExchangeVersion];
				TDataObject dataObject10 = this.DataObject;
				if (dataObject10.ExchangeVersion.IsOlderThan(exchangeObjectVersion))
				{
					TDataObject dataObject11 = this.DataObject;
					string name = dataObject11.Name;
					TDataObject dataObject12 = this.DataObject;
					base.WriteError(new TaskException(Strings.ErrorParentHasNewerVersion(name, dataObject12.ExchangeVersion.ToString(), exchangeObjectVersion.ToString())), (ErrorCategory)1004, null);
				}
			}
			TDataObject dataObject13 = this.DataObject;
			if (dataObject13.RecipientType != this.DesiredRecipientType && this.DesiredRecipientType != RecipientType.Invalid)
			{
				TDataObject dataObject14 = this.DataObject;
				string id = dataObject14.Identity.ToString();
				string oldType = this.DesiredRecipientType.ToString();
				TDataObject dataObject15 = this.DataObject;
				Exception exception = new InvalidOperationException(Strings.ErrorSetTaskChangeRecipientType(id, oldType, dataObject15.RecipientType.ToString()));
				ErrorCategory category = (ErrorCategory)1000;
				TDataObject dataObject16 = this.DataObject;
				base.WriteError(exception, category, dataObject16.Identity);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private RecipientType recipientType;
	}
}
