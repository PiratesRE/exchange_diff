using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewRecipientObjectTask<TDataObject> : NewGeneralRecipientObjectTask<TDataObject> where TDataObject : ADRecipient, new()
	{
		[Parameter]
		[ValidateNotNullOrEmpty]
		public string Alias
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.Alias;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.Alias = value;
			}
		}

		[Parameter]
		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.PrimarySmtpAddress;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.PrimarySmtpAddress = value;
			}
		}

		internal sealed override void PreInternalProcessRecord()
		{
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.PreInternalProcessRecord(this, this.ConvertDataObjectToPresentationObject(this.DataObject), false);
			}
		}

		protected virtual bool ShouldCheckAcceptedDomains()
		{
			return true;
		}

		protected virtual bool GetEmailAddressPolicyEnabledDefaultValue(IConfigurable dataObject)
		{
			TDataObject tdataObject = (TDataObject)((object)dataObject);
			return tdataObject.PrimarySmtpAddress == SmtpAddress.Empty;
		}

		protected sealed override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			if (string.IsNullOrEmpty(tdataObject.Alias))
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "RecipientTaskHelper.GenerateUniqueAlias", LoggerHelper.CmdletPerfMonitors))
				{
					tdataObject.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, base.CurrentOrganizationId, base.Name, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			if (!this.GetEmailAddressPolicyEnabledDefaultValue(tdataObject))
			{
				tdataObject.EmailAddressPolicyEnabled = false;
			}
			if (string.IsNullOrEmpty(tdataObject.DisplayName))
			{
				tdataObject.DisplayName = tdataObject.Name;
			}
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.UpdateAffectedIConfigurable(this, this.ConvertDataObjectToPresentationObject(tdataObject), false);
			}
			else
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), (ErrorCategory)1001, null);
			}
			if (tdataObject.EmailAddresses.Count > 0)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewRecipientObjectTask<TDataObject>.VerifyProxyAddress", LoggerHelper.CmdletPerfMonitors))
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, tdataObject.OrganizationId, base.ExecutingUserOrganizationId, false);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential, sessionSettings, 867, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
					bool flag = base.Fields["SoftDeletedObject"] != null;
					if (flag)
					{
						RecipientTaskHelper.StripInvalidSMTPAddress(this.ConfigurationSession, tdataObject, base.ProvisioningCache, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
						RecipientTaskHelper.StripConflictEmailAddress(tenantOrRootOrgRecipientSession, tdataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
					}
					else
					{
						if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && this.ShouldCheckAcceptedDomains())
						{
							RecipientTaskHelper.ValidateSmtpAddress(this.ConfigurationSession, tdataObject.EmailAddresses, tdataObject, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
						}
						RecipientTaskHelper.ValidateEmailAddressErrorOut(tenantOrRootOrgRecipientSession, tdataObject.EmailAddresses, tdataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
					}
				}
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				ADRecipient adrecipient = tdataObject;
				if ((RecipientTaskHelper.GetAcceptedRecipientTypes() & adrecipient.RecipientTypeDetails) != RecipientTypeDetails.None && string.IsNullOrEmpty(adrecipient.ExternalDirectoryObjectId))
				{
					adrecipient.ExternalDirectoryObjectId = Guid.NewGuid().ToString("D");
				}
			}
			TaskLogger.LogExit();
			return tdataObject;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}
	}
}
