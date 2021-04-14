using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Enable", "MailPublicFolder", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class EnableMailPublicFolder : ObjectActionTenantADTask<PublicFolderIdParameter, ADPublicFolder>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableMailPublicFolder(this.Identity.ToString());
			}
		}

		private OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)(base.Fields["HiddenFromAddressListsEnabled"] ?? false);
			}
			set
			{
				base.Fields["HiddenFromAddressListsEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		private IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					ADSessionSettings sessionSettings;
					if (MapiTaskHelper.IsDatacenter)
					{
						sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
					}
					else
					{
						sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					}
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 112, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\PublicFolder\\EnableMailPublicFolder.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).AD.UseGlobalCatalogIsSetToFalse.Enabled;
					this.recipientSession = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSession;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.currentPublicFolder.FolderPath.IsNonIpmPath || this.currentPublicFolder.FolderPath.IsSubtreeRoot)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorEnableMailPublicFolderNotAllowed(this.currentPublicFolder.FolderPath.ToString())), ErrorCategory.InvalidData, this.Identity);
			}
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return false;
		}

		protected override void InternalProcessRecord()
		{
			bool flag = false;
			try
			{
				bool flag2 = true;
				if (this.currentPublicFolder.MailEnabled)
				{
					byte[] proxyGuid = this.currentPublicFolder.ProxyGuid;
					if (proxyGuid != null && proxyGuid.Length == Marshal.SizeOf(typeof(Guid)))
					{
						ADPublicFolder adpublicFolder = this.RecipientSession.Read(new ADObjectId(proxyGuid)) as ADPublicFolder;
						if (adpublicFolder != null)
						{
							ADRecipient adrecipient = this.RecipientSession.FindByExchangeGuid(this.currentPublicFolder.ContentMailboxGuid);
							if (adrecipient == null || adrecipient.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
							{
								base.WriteError(new InvalidOperationException(Strings.ErrorUnableToDetermineContentMailbox(this.Identity.ToString(), this.currentPublicFolder.ContentMailboxGuid)), ErrorCategory.InvalidData, this.Identity);
							}
							adpublicFolder.ContentMailbox = adrecipient.Id;
							adpublicFolder.EntryId = this.currentPublicFolder.EntryId;
							this.RecipientSession.Save(adpublicFolder);
							this.RecipientSession.DomainController = this.RecipientSession.LastUsedDc;
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					base.InternalProcessRecord();
					this.currentPublicFolder.MailEnabled = true;
					if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).AD.UseGlobalCatalogIsSetToFalse.Enabled)
					{
						bool useGlobalCatalog = this.RecipientSession.UseGlobalCatalog;
						this.RecipientSession.UseGlobalCatalog = false;
						this.currentPublicFolder.ProxyGuid = this.RecipientSession.Read(this.DataObject.Id).Guid.ToByteArray();
						this.RecipientSession.UseGlobalCatalog = useGlobalCatalog;
					}
					else
					{
						this.currentPublicFolder.ProxyGuid = this.RecipientSession.Read(this.DataObject.Id).Guid.ToByteArray();
					}
					this.publicFolderDataProvider.Save(this.currentPublicFolder);
				}
				flag = true;
			}
			finally
			{
				if (!flag && this.DataObject != null && this.DataObject.ObjectState != ObjectState.New)
				{
					try
					{
						base.WriteVerbose(Strings.VerboseDeletePFProxy(this.DataObject.Id.ToString()));
						this.RecipientSession.Delete(this.DataObject);
					}
					catch (DataSourceTransientException ex)
					{
						this.WriteWarning(Strings.FailedToDeletePublicFolderProxy(this.DataObject.Identity.ToString(), ex.Message));
						TaskLogger.Trace("Failed to delete Public Folder Proxy {0} when rolling back. {1}", new object[]
						{
							this.DataObject.Identity,
							ex.ToString()
						});
					}
					catch (DataSourceOperationException ex2)
					{
						this.WriteWarning(Strings.FailedToDeletePublicFolderProxy(this.DataObject.Identity.ToString(), ex2.Message));
						TaskLogger.Trace("Failed to rollback created database object '{0}'. {1}", new object[]
						{
							this.DataObject.Identity,
							ex2.ToString()
						});
					}
				}
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (MapiTaskHelper.IsDatacenter)
			{
				this.Organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(this.Organization, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, this.Organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			if (this.publicFolderDataProvider == null || base.CurrentOrganizationId != this.publicFolderDataProvider.CurrentOrganizationId)
			{
				if (this.publicFolderDataProvider != null)
				{
					this.publicFolderDataProvider.Dispose();
					this.publicFolderDataProvider = null;
				}
				this.recipientSession = null;
				try
				{
					this.publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "Enable-MailPublicFolder", Guid.Empty);
				}
				catch (AccessDeniedException exception)
				{
					base.WriteError(exception, ErrorCategory.PermissionDenied, this.Identity);
				}
			}
			return this.RecipientSession;
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (this.currentPublicFolder == null)
			{
				this.currentPublicFolder = (PublicFolder)base.GetDataObject<PublicFolder>(this.Identity, this.publicFolderDataProvider, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.Identity.ToString())));
			}
			return this.CreatePublicFolderProxy();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.publicFolderDataProvider != null)
			{
				this.publicFolderDataProvider.Dispose();
				this.publicFolderDataProvider = null;
			}
		}

		protected override void InternalStateReset()
		{
			this.currentPublicFolder = null;
			base.InternalStateReset();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		private ADPublicFolder CreatePublicFolderProxy()
		{
			ADPublicFolder adpublicFolder = new ADPublicFolder();
			if (MapiTaskHelper.IsDatacenter)
			{
				adpublicFolder.OrganizationId = base.CurrentOrganizationId;
			}
			adpublicFolder.StampPersistableDefaultValues();
			string text = this.currentPublicFolder.Name.Trim();
			if (text.Length > 256)
			{
				adpublicFolder.DisplayName = text.Substring(0, 256);
			}
			else
			{
				adpublicFolder.DisplayName = text.Trim();
			}
			if (text.Length > 64)
			{
				adpublicFolder.Name = text.Substring(0, 64);
			}
			else
			{
				adpublicFolder.Name = text;
			}
			adpublicFolder.Alias = RecipientTaskHelper.GenerateUniqueAlias(this.RecipientSession, OrganizationId.ForestWideOrgId, adpublicFolder.Name, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			adpublicFolder.WindowsEmailAddress = adpublicFolder.PrimarySmtpAddress;
			adpublicFolder.SendModerationNotifications = TransportModerationNotificationFlags.Never;
			adpublicFolder.RecipientTypeDetails = RecipientTypeDetails.PublicFolder;
			adpublicFolder.ObjectCategory = this.ConfigurationSession.SchemaNamingContext.GetChildId(adpublicFolder.ObjectCategoryCN);
			ADRecipient adrecipient = this.RecipientSession.FindByExchangeGuid(this.currentPublicFolder.ContentMailboxGuid);
			if (adrecipient == null || adrecipient.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorUnableToDetermineContentMailbox(this.Identity.ToString(), this.currentPublicFolder.ContentMailboxGuid)), ErrorCategory.InvalidData, this.Identity);
			}
			adpublicFolder.ContentMailbox = adrecipient.Id;
			adpublicFolder.EntryId = this.currentPublicFolder.EntryId;
			adpublicFolder.LegacyExchangeDN = PublicFolderSession.ConvertToLegacyDN(this.currentPublicFolder.ContentMailboxGuid.ToString(), this.currentPublicFolder.EntryId);
			ADObjectId adobjectId;
			if (!MapiTaskHelper.IsDatacenter)
			{
				adobjectId = base.CurrentOrgContainerId.DomainId.GetChildId("Microsoft Exchange System Objects");
			}
			else
			{
				adobjectId = base.CurrentOrganizationId.OrganizationalUnit;
			}
			ADObjectId childId = adobjectId.GetChildId(adpublicFolder.Name);
			ADRecipient adrecipient2 = this.RecipientSession.Read(childId);
			if (adrecipient2 != null)
			{
				string text2 = adpublicFolder.Name;
				if (text2.Length > 55)
				{
					text2 = text2.Substring(0, 55);
				}
				Random random = new Random();
				childId = adobjectId.GetChildId(string.Format("{0} {1}", text2, random.Next(100000000).ToString("00000000")));
			}
			adpublicFolder.SetId(childId);
			if (base.IsProvisioningLayerAvailable)
			{
				base.WriteVerbose(Strings.VerboseInvokingRUS(adpublicFolder.Identity.ToString(), adpublicFolder.GetType().Name));
				ADPublicFolder adpublicFolder2 = new ADPublicFolder();
				adpublicFolder2.CopyChangesFrom(adpublicFolder);
				ProvisioningLayer.UpdateAffectedIConfigurable(this, RecipientTaskHelper.ConvertRecipientToPresentationObject(adpublicFolder2), false);
				adpublicFolder.CopyChangesFrom(adpublicFolder2);
			}
			else
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			}
			return adpublicFolder;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		private const string ParameterHiddenFromAddressListsEnabled = "HiddenFromAddressListsEnabled";

		private IRecipientSession recipientSession;

		private PublicFolderDataProvider publicFolderDataProvider;

		private PublicFolder currentPublicFolder;
	}
}
