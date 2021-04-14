using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "PartnerApplication", DefaultParameterSetName = "AuthMetadataUrlParameterSet", SupportsShouldProcess = true)]
	public sealed class NewPartnerApplication : NewMultitenancySystemConfigurationObjectTask<PartnerApplication>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPartnerApplication(base.Name);
			}
		}

		[Parameter(ParameterSetName = "ACSTrustApplicationParameterSet", Mandatory = true)]
		public string ApplicationIdentifier
		{
			get
			{
				return this.DataObject.ApplicationIdentifier;
			}
			set
			{
				this.DataObject.ApplicationIdentifier = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter]
		public bool AcceptSecurityIdentifierInformation
		{
			get
			{
				return this.DataObject.AcceptSecurityIdentifierInformation;
			}
			set
			{
				this.DataObject.AcceptSecurityIdentifierInformation = value;
			}
		}

		[Parameter(ParameterSetName = "ACSTrustApplicationParameterSet")]
		public string Realm
		{
			get
			{
				return this.DataObject.Realm;
			}
			set
			{
				this.DataObject.Realm = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet", Mandatory = true)]
		public string AuthMetadataUrl
		{
			get
			{
				return this.DataObject.AuthMetadataUrl;
			}
			set
			{
				this.DataObject.AuthMetadataUrl = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		public SwitchParameter TrustAnySSLCertificate { get; set; }

		[Parameter]
		public UserIdParameter LinkedAccount
		{
			get
			{
				return (UserIdParameter)base.Fields[PartnerApplicationSchema.LinkedAccount];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.LinkedAccount] = value;
			}
		}

		[Parameter]
		public string IssuerIdentifier
		{
			get
			{
				return this.DataObject.IssuerIdentifier;
			}
			set
			{
				this.DataObject.IssuerIdentifier = value;
			}
		}

		[Parameter]
		public string[] AppOnlyPermissions
		{
			get
			{
				return this.DataObject.AppOnlyPermissions;
			}
			set
			{
				this.DataObject.AppOnlyPermissions = value;
			}
		}

		[Parameter]
		public string[] ActAsPermissions
		{
			get
			{
				return this.DataObject.ActAsPermissions;
			}
			set
			{
				this.DataObject.ActAsPermissions = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.CreatePartnerApplicationsContainer();
			PartnerApplication partnerApplication = (PartnerApplication)base.PrepareDataObject();
			ADObjectId containerId = PartnerApplication.GetContainerId(this.ConfigurationSession);
			partnerApplication.SetId(containerId.GetChildId(partnerApplication.Name));
			partnerApplication.UseAuthServer = true;
			if (partnerApplication.IsModified(PartnerApplicationSchema.AuthMetadataUrl))
			{
				partnerApplication.UseAuthServer = false;
				OAuthTaskHelper.FetchAuthMetadata(partnerApplication, this.TrustAnySSLCertificate, true, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(PartnerApplicationSchema.LinkedAccount))
			{
				if (this.LinkedAccount == null)
				{
					partnerApplication.LinkedAccount = null;
				}
				else
				{
					ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.LinkedAccount, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.LinkedAccount.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.LinkedAccount.ToString())));
					partnerApplication.LinkedAccount = adrecipient.Id;
				}
			}
			if (base.Fields.IsModified(PartnerApplicationSchema.AppOnlyPermissions))
			{
				partnerApplication.AppOnlyPermissions = this.AppOnlyPermissions;
			}
			if (base.Fields.IsModified(PartnerApplicationSchema.ActAsPermissions))
			{
				partnerApplication.ActAsPermissions = this.ActAsPermissions;
			}
			OAuthTaskHelper.ValidateApplicationRealmAndUniqueness(partnerApplication, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			return partnerApplication;
		}

		private void CreatePartnerApplicationsContainer()
		{
			ADObjectId containerId = PartnerApplication.GetContainerId(this.ConfigurationSession);
			if (this.ConfigurationSession.Read<Container>(containerId) == null)
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				OrganizationId currentOrganizationId = this.ConfigurationSession.SessionSettings.CurrentOrganizationId;
				if (!currentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					ADObjectId containerId2 = AuthConfig.GetContainerId(this.ConfigurationSession);
					if (this.ConfigurationSession.Read<AuthConfig>(containerId2) == null)
					{
						AuthConfig authConfig = new AuthConfig();
						authConfig.OrganizationId = currentOrganizationId;
						authConfig.SetId(containerId2);
						configurationSession.Save(authConfig);
					}
				}
				Container container = new Container();
				container.OrganizationId = currentOrganizationId;
				container.SetId(containerId);
				configurationSession.Save(container);
			}
		}
	}
}
