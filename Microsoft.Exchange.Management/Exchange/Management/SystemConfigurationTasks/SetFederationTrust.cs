using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "FederationTrust", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetFederationTrust : SetSystemConfigurationObjectTask<FederationTrustIdParameter, FederationTrust>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "PublishFederationCertificate", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ApplicationUri", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override FederationTrustIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ApplicationUri")]
		public string ApplicationUri { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "PublishFederationCertificate")]
		public SwitchParameter PublishFederationCertificate { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string Thumbprint { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Uri MetadataUrl { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter RefreshMetadata { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				LocalizedString result;
				if (this.RefreshMetadata)
				{
					result = Strings.ConfirmationMessageSetFederationTrust4(this.DataObject.Name, this.MetadataUrlToUse.AbsoluteUri);
				}
				else if (null != this.MetadataUrl && !string.IsNullOrEmpty(this.MetadataUrl.AbsoluteUri))
				{
					if (this.Thumbprint != null)
					{
						result = Strings.ConfirmationMessageSetFederationTrust1(this.DataObject.Name, this.Thumbprint, this.MetadataUrl.AbsoluteUri);
					}
					else
					{
						result = Strings.ConfirmationMessageSetFederationTrust3(this.DataObject.Name, this.MetadataUrl.AbsoluteUri);
					}
				}
				else if (this.Thumbprint != null)
				{
					result = Strings.ConfirmationMessageSetFederationTrust2(this.DataObject.Name, this.Thumbprint);
				}
				else if (!string.IsNullOrEmpty(this.ApplicationUri))
				{
					result = Strings.ConfirmationMessageSetFederationTrust5(this.DataObject.Name, this.ApplicationUri);
				}
				else
				{
					result = LocalizedString.Empty;
				}
				return result;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					this.InternalValidateInternal();
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void InternalValidateInternal()
		{
			if (!string.IsNullOrEmpty(this.Thumbprint) && this.DataObject.ApplicationUri == null)
			{
				base.WriteError(new CannotUpdateCertificateWhenFederationNotProvisionedException(), ErrorCategory.InvalidArgument, null);
			}
			if (this.PublishFederationCertificate && string.IsNullOrEmpty(this.DataObject.OrgNextPrivCertificate))
			{
				base.WriteError(new NoNextCertificateException(), ErrorCategory.InvalidArgument, null);
			}
			if (this.MetadataUrl != null)
			{
				if (!this.MetadataUrl.IsAbsoluteUri)
				{
					base.WriteError(new MetadataMustBeAbsoluteUrlException(), ErrorCategory.InvalidArgument, null);
				}
				if (this.RefreshMetadata)
				{
					base.WriteError(new RefreshMetadataOptionNotAllowedException(), ErrorCategory.InvalidArgument, null);
				}
			}
			if (this.MetadataUrlChanged || this.RefreshMetadata)
			{
				this.UpdateFederationMetadata();
			}
			if (!string.IsNullOrEmpty(this.Thumbprint))
			{
				try
				{
					this.ValidateNextCertificate();
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
			}
			if (this.ApplicationUri != null && !Uri.TryCreate(this.ApplicationUri, UriKind.RelativeOrAbsolute, out this.applicationUri))
			{
				base.WriteError(new InvalidApplicationUriException(Strings.ErrorInvalidApplicationUri(this.ApplicationUri)), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.InternalProcessRecordInternal();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void InternalProcessRecordInternal()
		{
			if (this.PublishFederationCertificate)
			{
				FederationProvision federationProvision = FederationProvision.Create(this.DataObject, this);
				try
				{
					federationProvision.OnPublishFederationCertificate(this.DataObject);
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidResult, null);
				}
			}
			if (null != this.applicationUri)
			{
				this.DataObject.ApplicationUri = this.applicationUri;
			}
			if (this.Thumbprint != null)
			{
				if (!StringComparer.InvariantCultureIgnoreCase.Equals(this.DataObject.OrgNextPrivCertificate, this.Thumbprint))
				{
					this.DataObject.OrgNextCertificate = this.nextCertificate;
					this.DataObject.OrgNextPrivCertificate = this.Thumbprint;
					try
					{
						FederationCertificate.PushCertificate(new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.Thumbprint);
					}
					catch (InvalidOperationException exception2)
					{
						base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
					}
					catch (LocalizedException exception3)
					{
						base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
					}
					if (this.DataObject.NamespaceProvisioner == FederationTrust.NamespaceProvisionerType.LiveDomainServices2)
					{
						this.WriteWarning(Strings.UpdateManageDelegation2ProvisioningInDNS);
					}
				}
				else
				{
					base.WriteVerbose(Strings.IgnoringSameNextCertificate);
				}
			}
			if (this.PublishFederationCertificate)
			{
				this.DataObject.OrgPrevCertificate = this.DataObject.OrgCertificate;
				this.DataObject.OrgPrevPrivCertificate = this.DataObject.OrgPrivCertificate;
				this.DataObject.OrgCertificate = this.DataObject.OrgNextCertificate;
				this.DataObject.OrgPrivCertificate = this.DataObject.OrgNextPrivCertificate;
				this.DataObject.OrgNextCertificate = null;
				this.DataObject.OrgNextPrivCertificate = null;
				if (this.DataObject.NamespaceProvisioner == FederationTrust.NamespaceProvisionerType.LiveDomainServices2)
				{
					this.WriteWarning(Strings.PublishManageDelegation2ProvisioningInDNS);
				}
			}
			if (this.partnerFederationMetadata != null)
			{
				try
				{
					LivePartnerFederationMetadata.InitializeDataObjectFromMetadata(this.DataObject, this.partnerFederationMetadata, new WriteWarningDelegate(this.WriteWarning));
				}
				catch (FederationMetadataException exception4)
				{
					base.WriteError(exception4, ErrorCategory.MetadataError, null);
				}
			}
			base.InternalProcessRecord();
		}

		private void ValidateNextCertificate()
		{
			this.Thumbprint = FederationCertificate.UnifyThumbprintFormat(this.Thumbprint);
			this.nextCertificate = FederationCertificate.GetExchangeFederationCertByThumbprint(this.Thumbprint, new WriteVerboseDelegate(base.WriteVerbose));
			ExchangeCertificate exchangeCertificate = new ExchangeCertificate(this.nextCertificate);
			FederationCertificate.ValidateCertificate(exchangeCertificate, this.IsDatacenter);
			this.ValidateUniqueSki(exchangeCertificate, this.DataObject.OrgPrevCertificate);
			this.ValidateUniqueSki(exchangeCertificate, this.DataObject.OrgCertificate);
		}

		private void ValidateUniqueSki(ExchangeCertificate nextExchangeCertificate, X509Certificate2 otherCertificate)
		{
			if (otherCertificate != null)
			{
				ExchangeCertificate exchangeCertificate = new ExchangeCertificate(otherCertificate);
				if (StringComparer.InvariantCultureIgnoreCase.Equals(nextExchangeCertificate.SubjectKeyIdentifier, exchangeCertificate.SubjectKeyIdentifier))
				{
					throw new FederationCertificateInvalidException(Strings.ErrorCertificateSKINotUnique(nextExchangeCertificate.Thumbprint, exchangeCertificate.Thumbprint, nextExchangeCertificate.SubjectKeyIdentifier));
				}
			}
		}

		private void UpdateFederationMetadata()
		{
			try
			{
				this.partnerFederationMetadata = LivePartnerFederationMetadata.LoadFrom(this.MetadataUrlToUse, new WriteVerboseDelegate(base.WriteVerbose));
			}
			catch (FederationMetadataException exception)
			{
				base.WriteError(exception, ErrorCategory.MetadataError, null);
			}
		}

		private Uri MetadataUrlToUse
		{
			get
			{
				if (this.MetadataUrl == null)
				{
					return this.DataObject.TokenIssuerMetadataEpr;
				}
				return this.MetadataUrl;
			}
		}

		private bool MetadataUrlChanged
		{
			get
			{
				return !StringComparer.OrdinalIgnoreCase.Equals(this.MetadataUrlToUse.AbsoluteUri, this.DataObject.TokenIssuerMetadataEpr.AbsoluteUri);
			}
		}

		private bool IsDatacenter
		{
			get
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
		}

		private Uri applicationUri;

		private X509Certificate2 nextCertificate;

		private PartnerFederationMetadata partnerFederationMetadata;
	}
}
