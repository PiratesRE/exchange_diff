using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("New", "SyncMailPublicFolder", DefaultParameterSetName = "SyncMailPublicFolder", SupportsShouldProcess = true)]
	public sealed class NewSyncMailPublicFolder : NewRecipientObjectTaskBase<ADPublicFolder>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "SyncMailPublicFolder")]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "SyncMailPublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SyncMailPublicFolder")]
		public SwitchParameter HiddenFromAddressListsEnabled
		{
			get
			{
				return (SwitchParameter)(base.Fields["HiddenFromAddressListsEnabled"] ?? false);
			}
			set
			{
				base.Fields["HiddenFromAddressListsEnabled"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SyncMailPublicFolder")]
		[ValidateNotNullOrEmpty]
		public string EntryId
		{
			get
			{
				return (string)base.Fields["EntryId"];
			}
			set
			{
				base.Fields["EntryId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SyncMailPublicFolder")]
		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["WindowsEmailAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["WindowsEmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SyncMailPublicFolder")]
		public SmtpAddress ExternalEmailAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["ExternalEmailAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["ExternalEmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SyncMailPublicFolder")]
		public ProxyAddress[] EmailAddresses
		{
			get
			{
				return (ProxyAddress[])base.Fields["EmailAddresses"];
			}
			set
			{
				base.Fields["EmailAddresses"] = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableMailPublicFolder(this.Name);
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
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 202, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\PublicFolder\\NewSyncMailPublicFolder.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
					this.recipientSession = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSession;
			}
		}

		protected override void PrepareRecipientObject(ADPublicFolder publicFolder)
		{
			this.DataObject = publicFolder;
			if (MapiTaskHelper.IsDatacenter)
			{
				publicFolder.OrganizationId = base.CurrentOrganizationId;
			}
			publicFolder.StampPersistableDefaultValues();
			if (this.Name.Contains("\n"))
			{
				this.Name = this.Name.Replace("\n", "_");
			}
			if (this.Name.Length > 64)
			{
				publicFolder.Name = this.Name.Substring(0, 64);
			}
			else
			{
				publicFolder.Name = this.Name;
			}
			publicFolder.DisplayName = publicFolder.Name;
			publicFolder.Alias = RecipientTaskHelper.GenerateUniqueAlias(this.RecipientSession, OrganizationId.ForestWideOrgId, this.Alias, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			if (this.WindowsEmailAddress != SmtpAddress.Empty)
			{
				publicFolder.WindowsEmailAddress = this.WindowsEmailAddress;
			}
			else
			{
				publicFolder.WindowsEmailAddress = publicFolder.PrimarySmtpAddress;
			}
			publicFolder.HiddenFromAddressListsEnabled = this.HiddenFromAddressListsEnabled;
			publicFolder.SendModerationNotifications = TransportModerationNotificationFlags.Never;
			publicFolder.RecipientTypeDetails = RecipientTypeDetails.PublicFolder;
			publicFolder.ObjectCategory = this.ConfigurationSession.SchemaNamingContext.GetChildId(publicFolder.ObjectCategoryCN);
			publicFolder.LegacyExchangeDN = PublicFolderSession.ConvertToLegacyDN("e71f13d1-0178-42a7-8c47-24206de84a77", this.EntryId);
			ADObjectId adobjectId;
			if (base.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
			{
				adobjectId = base.CurrentOrgContainerId.DomainId.GetChildId("Microsoft Exchange System Objects");
			}
			else
			{
				adobjectId = base.CurrentOrganizationId.OrganizationalUnit;
			}
			ADObjectId childId = adobjectId.GetChildId(publicFolder.Name);
			ADRecipient adrecipient = this.RecipientSession.Read(childId);
			if (adrecipient != null)
			{
				Random random = new Random();
				childId = adobjectId.GetChildId(string.Format("{0} {1}", publicFolder.Name, random.Next(100000000).ToString("00000000")));
			}
			publicFolder.SetId(childId);
			if (base.IsProvisioningLayerAvailable)
			{
				base.WriteVerbose(Strings.VerboseInvokingRUS(publicFolder.Identity.ToString(), publicFolder.GetType().Name));
				ADPublicFolder adpublicFolder = new ADPublicFolder();
				adpublicFolder.CopyChangesFrom(publicFolder);
				ProvisioningLayer.UpdateAffectedIConfigurable(this, RecipientTaskHelper.ConvertRecipientToPresentationObject(adpublicFolder), false);
				publicFolder.CopyChangesFrom(adpublicFolder);
			}
			else
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			}
			if (this.ExternalEmailAddress != SmtpAddress.Empty)
			{
				publicFolder.ExternalEmailAddress = ProxyAddress.Parse(this.ExternalEmailAddress.ToString());
			}
			else
			{
				publicFolder.ExternalEmailAddress = ProxyAddress.Parse(publicFolder.WindowsEmailAddress.ToString());
			}
			MailUserTaskHelper.ValidateExternalEmailAddress(publicFolder, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			if (this.EmailAddresses != null)
			{
				foreach (ProxyAddress proxyAddress in this.EmailAddresses)
				{
					if (proxyAddress != null && !publicFolder.EmailAddresses.Contains(proxyAddress))
					{
						publicFolder.EmailAddresses.Add(proxyAddress);
					}
				}
			}
			adrecipient = this.RecipientSession.FindByProxyAddress(ProxyAddress.Parse("X500:" + publicFolder.LegacyExchangeDN));
			if (adrecipient != null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorObjectAlreadyExists("ADPublicFolder object : ", this.Name)), ErrorCategory.InvalidData, null);
			}
			publicFolder.EmailAddresses.Add(ProxyAddress.Parse("X500:" + publicFolder.LegacyExchangeDN));
			RecipientTaskHelper.ValidateEmailAddressErrorOut(this.RecipientSession, publicFolder.EmailAddresses, publicFolder, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerReThrowDelegate(this.WriteError));
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		private const string TaskNoun = "SyncMailPublicFolder";

		private const string ParameterSetSyncMailPublicFolder = "SyncMailPublicFolder";

		private const string ParameterName = "Name";

		private const string ParameterAlias = "Alias";

		private const string ParameterHiddenFromAddressListsEnabled = "HiddenFromAddressListsEnabled";

		private const string ParameterEntryId = "EntryId";

		private const string ParameterWindowsEmailAddress = "WindowsEmailAddress";

		private const string ParameterExternalEmailAddress = "ExternalEmailAddress";

		private const string ParameterEmailAddresses = "EmailAddresses";

		private IRecipientSession recipientSession;
	}
}
