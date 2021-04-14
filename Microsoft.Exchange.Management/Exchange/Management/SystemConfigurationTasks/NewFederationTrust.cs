using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "FederationTrust", SupportsShouldProcess = true, DefaultParameterSetName = "FederationTrustParameter")]
	public sealed class NewFederationTrust : NewSystemConfigurationObjectTask<FederationTrust>
	{
		[Parameter(Mandatory = false, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		public string ApplicationIdentifier
		{
			get
			{
				return this.DataObject.ApplicationIdentifier;
			}
			set
			{
				this.DataObject.ApplicationIdentifier = value.Trim();
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		public string AdministratorProvisioningId
		{
			get
			{
				return this.DataObject.AdministratorProvisioningId;
			}
			set
			{
				this.DataObject.AdministratorProvisioningId = value.Trim();
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		public SwitchParameter SkipNamespaceProviderProvisioning { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		public string ApplicationUri { get; set; }

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FederationTrustParameter")]
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		public string Thumbprint
		{
			get
			{
				return this.DataObject.OrgPrivCertificate;
			}
			set
			{
				this.DataObject.OrgPrivCertificate = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SkipNamespaceProviderProvisioning")]
		[Parameter(Mandatory = false, ParameterSetName = "FederationTrustParameter")]
		public Uri MetadataUrl
		{
			get
			{
				return this.DataObject.TokenIssuerMetadataEpr;
			}
			set
			{
				this.DataObject.TokenIssuerMetadataEpr = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FederationTrustParameter")]
		public SwitchParameter UseLegacyProvisioningService { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (null != this.MetadataUrl && !string.IsNullOrEmpty(this.MetadataUrl.AbsoluteUri))
				{
					return Strings.ConfirmationMessageNewFederationTrustWithMetadata(base.Name, FederationTrust.PartnerSTSType.LiveId.ToString(), this.Thumbprint, this.MetadataUrl.AbsoluteUri);
				}
				return Strings.ConfirmationMessageNewFederationTrust(base.Name, FederationTrust.PartnerSTSType.LiveId.ToString(), this.Thumbprint);
			}
		}

		internal static bool IsExchangeDataCenter()
		{
			bool result = false;
			try
			{
				result = Datacenter.IsMicrosoftHostedOnly(true);
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return result;
		}

		protected override IConfigurable PrepareDataObject()
		{
			FederationTrust federationTrust = (FederationTrust)base.PrepareDataObject();
			federationTrust.SetId(this.ConfigurationSession, base.Name);
			return federationTrust;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			IConfigurable[] array = base.DataSession.Find<FederationTrust>(null, null, true, null);
			if (array != null && array.Length > 0)
			{
				if (this.SkipNamespaceProviderProvisioning)
				{
					if (!Array.Exists<IConfigurable>(array, (IConfigurable federationTrust) => ((FederationTrust)federationTrust).NamespaceProvisioner != FederationTrust.NamespaceProvisionerType.ExternalProcess))
					{
						goto IL_76;
					}
				}
				base.WriteError(new TrustAlreadyDefinedException(), ErrorCategory.InvalidArgument, this.DataObject.Name);
			}
			IL_76:
			this.DataObject.OrgCertificate = this.GetFederatedExchangeCertificates();
			if (this.ApplicationUri != null)
			{
				Uri uri;
				if (!Uri.TryCreate(this.ApplicationUri, UriKind.RelativeOrAbsolute, out uri))
				{
					base.WriteError(new InvalidApplicationUriException(Strings.ErrorInvalidApplicationUri(this.ApplicationUri)), ErrorCategory.InvalidArgument, null);
				}
				if (null != uri)
				{
					this.DataObject.ApplicationUri = uri;
				}
			}
			if (!string.IsNullOrEmpty(this.ApplicationIdentifier))
			{
				base.WriteVerbose(Strings.NewFederationTrustSuccessAppId(FederationTrust.PartnerSTSType.LiveId.ToString(), this.ApplicationIdentifier));
			}
			TaskLogger.LogExit();
		}

		private FederationTrust.NamespaceProvisionerType NamespaceProvisionerType
		{
			get
			{
				if (this.SkipNamespaceProviderProvisioning)
				{
					return FederationTrust.NamespaceProvisionerType.ExternalProcess;
				}
				if (this.UseLegacyProvisioningService)
				{
					return FederationTrust.NamespaceProvisionerType.LiveDomainServices;
				}
				return FederationTrust.NamespaceProvisionerType.LiveDomainServices2;
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.NamespaceProvisioner = this.NamespaceProvisionerType;
			this.ProvisionSTS();
			try
			{
				FederationCertificate.PushCertificate(new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.Thumbprint);
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			base.InternalProcessRecord();
		}

		private X509Certificate2 GetFederatedExchangeCertificates()
		{
			if (!string.IsNullOrEmpty(this.Thumbprint))
			{
				this.Thumbprint = FederationCertificate.UnifyThumbprintFormat(this.Thumbprint);
				try
				{
					X509Certificate2 exchangeFederationCertByThumbprint = FederationCertificate.GetExchangeFederationCertByThumbprint(this.Thumbprint, new WriteVerboseDelegate(base.WriteVerbose));
					if (exchangeFederationCertByThumbprint == null)
					{
						throw new FederationCertificateInvalidException(Strings.ErrorCertificateNotFound(this.Thumbprint));
					}
					FederationCertificate.ValidateCertificate(new ExchangeCertificate(exchangeFederationCertByThumbprint), NewFederationTrust.IsExchangeDataCenter());
					return exchangeFederationCertByThumbprint;
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
					goto IL_7C;
				}
			}
			base.WriteError(new FederationCertificateInvalidException(Strings.ErrorFederationCertificateNotSpecified), ErrorCategory.InvalidOperation, null);
			IL_7C:
			return null;
		}

		private void ProvisionSTS()
		{
			int num = 0;
			num += 30;
			base.WriteProgress(Strings.ProgressActivityNewFederationTrust, Strings.ProgressActivityGetFederationMetadata, num);
			Uri uri = this.MetadataUrl;
			if (uri == null)
			{
				uri = LiveConfiguration.GetLiveIdFederationMetadataEpr(this.NamespaceProvisionerType);
			}
			try
			{
				PartnerFederationMetadata partnerFederationMetadata = LivePartnerFederationMetadata.LoadFrom(uri, new WriteVerboseDelegate(base.WriteVerbose));
				LivePartnerFederationMetadata.InitializeDataObjectFromMetadata(this.DataObject, partnerFederationMetadata, new WriteWarningDelegate(this.WriteWarning));
			}
			catch (FederationMetadataException exception)
			{
				base.WriteError(exception, ErrorCategory.MetadataError, null);
			}
			this.DataObject.TokenIssuerType = FederationTrust.PartnerSTSType.LiveId;
			this.DataObject.MetadataEpr = null;
			this.DataObject.MetadataPutEpr = null;
			this.DataObject.MetadataPollInterval = LiveConfiguration.DefaultFederatedMetadataTimeout;
			num += 30;
			base.WriteProgress(Strings.ProgressActivityNewFederationTrust, Strings.NewFederationTrustProvisioningService(FederationTrust.PartnerSTSType.LiveId.ToString()), num);
			base.WriteVerbose(Strings.NewFederationTrustProvisioningService(FederationTrust.PartnerSTSType.LiveId.ToString()));
			num += 30;
			base.WriteProgress(Strings.ProgressActivityNewFederationTrust, Strings.ProgressActivityCreateAppId, num);
			FederationProvision federationProvision = FederationProvision.Create(this.DataObject, this);
			try
			{
				federationProvision.OnNewFederationTrust(this.DataObject);
			}
			catch (LocalizedException ex)
			{
				base.WriteError(new ProvisioningFederatedExchangeException(ex.Message, ex), ErrorCategory.NotSpecified, null);
			}
			base.WriteProgress(Strings.ProgressActivityNewFederationTrust, Strings.ProgressStatusFinished, 100);
			switch (this.NamespaceProvisionerType)
			{
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices:
				this.WriteWarning(Strings.ManageDelegationProvisioningInDNS(this.DataObject.ApplicationIdentifier));
				return;
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices2:
				this.WriteWarning(Strings.ManageDelegation2ProvisioningInDNS);
				return;
			default:
				return;
			}
		}
	}
}
