using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class EnableRecipientObjectTask<TIdentity, TDataObject> : RecipientObjectActionTask<TIdentity, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : ADRecipient, new()
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Alias
		{
			get
			{
				return (string)base.Fields["Alias"];
			}
			set
			{
				base.Fields["Alias"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields["DisplayName"];
			}
			set
			{
				base.Fields["DisplayName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["PrimarySmtpAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["PrimarySmtpAddress"] = value;
			}
		}

		[Parameter]
		public SwitchParameter OverrideRecipientQuotas
		{
			get
			{
				return (SwitchParameter)(base.Fields["OverrideRecipientQuotas"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["OverrideRecipientQuotas"] = value;
			}
		}

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

		public virtual CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)base.Fields[ADRecipientSchema.UsageLocation];
			}
			set
			{
				base.Fields[ADRecipientSchema.UsageLocation] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable configurable = base.ResolveDataObject();
			if (configurable != null && base.IsProvisioningLayerAvailable)
			{
				ADRecipient adrecipient = configurable as ADRecipient;
				if (adrecipient != null)
				{
					if (!this.SkipPrepareDataObject())
					{
						adrecipient.SetExchangeVersion(adrecipient.MaximumSupportedExchangeObjectVersion);
					}
					base.CurrentOrganizationId = adrecipient.OrganizationId;
				}
			}
			TaskLogger.LogExit();
			return configurable;
		}

		internal virtual bool SkipPrepareDataObject()
		{
			return false;
		}

		protected sealed override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			this.PrepareRecipientObject(ref tdataObject);
			if (!this.SkipPrepareDataObject())
			{
				this.PrepareRecipientAlias(tdataObject);
				if (this.PrimarySmtpAddress != SmtpAddress.Empty)
				{
					tdataObject.PrimarySmtpAddress = this.PrimarySmtpAddress;
					tdataObject.EmailAddressPolicyEnabled = false;
				}
				if (!string.IsNullOrEmpty(this.DisplayName))
				{
					tdataObject.DisplayName = this.DisplayName;
				}
				if (base.IsProvisioningLayerAvailable)
				{
					ProvisioningLayer.UpdateAffectedIConfigurable(this, this.ConvertDataObjectToPresentationObject(tdataObject), false);
				}
				else
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
				}
				if (tdataObject.EmailAddresses.Count > 0)
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						RecipientTaskHelper.ValidateSmtpAddress(this.ConfigurationSession, tdataObject.EmailAddresses, tdataObject, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
					}
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, tdataObject.OrganizationId, base.CurrentOrganizationId, false);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential, sessionSettings, 243, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\common\\EnableRecipientObjectTask.cs");
					RecipientTaskHelper.ValidateEmailAddressErrorOut(tenantOrRootOrgRecipientSession, tdataObject.EmailAddresses, tdataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
				}
			}
			else if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.UpdateAffectedIConfigurable(this, this.ConvertDataObjectToPresentationObject(tdataObject), false);
			}
			else
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
			return tdataObject;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		protected virtual void PrepareRecipientObject(ref TDataObject dataObject)
		{
			ADUser aduser = dataObject as ADUser;
			if (aduser != null)
			{
				if (base.Fields.IsModified("SKUCapability"))
				{
					aduser.SKUCapability = new Capability?(this.SKUCapability);
				}
				if (base.Fields.IsModified("AddOnSKUCapability"))
				{
					CapabilityHelper.SetAddOnSKUCapabilities(this.AddOnSKUCapability, aduser.PersistedCapabilities);
					RecipientTaskHelper.UpgradeArchiveQuotaOnArchiveAddOnSKU(aduser, aduser.PersistedCapabilities);
				}
				if (base.Fields.IsModified(ADRecipientSchema.SKUAssigned))
				{
					aduser.SKUAssigned = new bool?(this.SKUAssigned);
				}
				if (base.Fields.IsModified(ADRecipientSchema.UsageLocation))
				{
					aduser.UsageLocation = this.UsageLocation;
				}
			}
		}

		protected virtual void PrepareRecipientAlias(TDataObject dataObject)
		{
			if (string.IsNullOrEmpty(this.Alias))
			{
				dataObject.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, dataObject.OrganizationId, dataObject.Name, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				return;
			}
			dataObject.Alias = this.Alias;
		}
	}
}
