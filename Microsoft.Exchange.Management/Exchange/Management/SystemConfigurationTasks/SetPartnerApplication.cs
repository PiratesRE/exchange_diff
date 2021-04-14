using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "PartnerApplication", DefaultParameterSetName = "AuthMetadataUrlParameterSet", SupportsShouldProcess = true)]
	public sealed class SetPartnerApplication : SetSystemConfigurationObjectTask<PartnerApplicationIdParameter, PartnerApplication>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPartnerApplication(this.Identity.RawIdentity);
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override PartnerApplicationIdParameter Identity
		{
			get
			{
				return (PartnerApplicationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(ParameterSetName = "ACSTrustApplicationParameterSet")]
		public string ApplicationIdentifier
		{
			get
			{
				return (string)base.Fields[PartnerApplicationSchema.ApplicationIdentifier];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.ApplicationIdentifier] = value;
			}
		}

		[Parameter(ParameterSetName = "ACSTrustApplicationParameterSet")]
		public string Realm
		{
			get
			{
				return (string)base.Fields[PartnerApplicationSchema.Realm];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.Realm] = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		public string AuthMetadataUrl
		{
			get
			{
				return (string)base.Fields[PartnerApplicationSchema.AuthMetadataUrl];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.AuthMetadataUrl] = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		public SwitchParameter TrustAnySSLCertificate { get; set; }

		[Parameter(ParameterSetName = "RefreshAuthMetadataParameterSet")]
		public SwitchParameter RefreshAuthMetadata { get; set; }

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
				return (string)base.Fields[PartnerApplicationSchema.IssuerIdentifier];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.IssuerIdentifier] = value;
			}
		}

		[Parameter]
		public string[] AppOnlyPermissions
		{
			get
			{
				return (string[])base.Fields[PartnerApplicationSchema.AppOnlyPermissions];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.AppOnlyPermissions] = value;
			}
		}

		[Parameter]
		public string[] ActAsPermissions
		{
			get
			{
				return (string[])base.Fields[PartnerApplicationSchema.ActAsPermissions];
			}
			set
			{
				base.Fields[PartnerApplicationSchema.ActAsPermissions] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			PartnerApplication partnerApplication = (PartnerApplication)base.PrepareDataObject();
			if (base.Fields.IsModified(PartnerApplicationSchema.AuthMetadataUrl))
			{
				if (partnerApplication.UseAuthServer)
				{
					base.WriteError(new TaskException(Strings.ErrorPartnerApplicationUseAuthServerCannotSetUrl), ErrorCategory.InvalidArgument, null);
				}
				partnerApplication.AuthMetadataUrl = this.AuthMetadataUrl;
				OAuthTaskHelper.FetchAuthMetadata(partnerApplication, this.TrustAnySSLCertificate, false, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else if (base.Fields.IsModified(PartnerApplicationSchema.Realm) || base.Fields.IsModified(PartnerApplicationSchema.ApplicationIdentifier) || base.Fields.IsModified(PartnerApplicationSchema.IssuerIdentifier))
			{
				base.WriteError(new TaskException(Strings.ErrorChangePartnerApplicationDirectTrust), ErrorCategory.InvalidArgument, null);
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

		protected override void InternalProcessRecord()
		{
			if (this.RefreshAuthMetadata)
			{
				OAuthTaskHelper.FetchAuthMetadata(this.DataObject, this.TrustAnySSLCertificate, false, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			base.InternalProcessRecord();
		}
	}
}
