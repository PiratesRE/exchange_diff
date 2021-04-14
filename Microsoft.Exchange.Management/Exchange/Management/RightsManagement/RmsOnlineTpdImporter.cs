using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Online;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RmsOnlineTpdImporter : ITpdImporter
	{
		public Uri IntranetLicensingUrl { get; private set; }

		public Uri ExtranetLicensingUrl { get; private set; }

		public Uri IntranetCertificationUrl { get; private set; }

		public Uri ExtranetCertificationUrl { get; private set; }

		public RmsOnlineTpdImporter(Uri rmsOnlineKeySharingLocation, string authenticationCertificateSubjectName)
		{
			RmsUtil.ThrowIfParameterNull(rmsOnlineKeySharingLocation, "rmsOnlineKeySharingLocation");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(authenticationCertificateSubjectName, "authenticationCertificateSubjectName");
			this.rmsOnlineKeySharingLocation = rmsOnlineKeySharingLocation;
			this.authenticationCertificateSubjectName = authenticationCertificateSubjectName;
		}

		public TrustedDocDomain Import(Guid externalDirectoryOrgId)
		{
			RmsUtil.ThrowIfGuidEmpty(externalDirectoryOrgId, "externalDirectoryOrgId");
			X509Certificate2 authenticationCertificate = this.LoadAuthenticationCertificate();
			this.ThrowIfAuthenticationCertificateIsInvalid(authenticationCertificate);
			ITenantManagementService tenantManagementService = this.CreateRmsOnlineWebServiceProxy(authenticationCertificate);
			TrustedDocDomain result;
			try
			{
				TenantInfo[] tenantInfo = tenantManagementService.GetTenantInfo(new string[]
				{
					externalDirectoryOrgId.ToString()
				});
				RmsUtil.ThrowIfTenantInfoisNull(tenantInfo, externalDirectoryOrgId);
				RmsUtil.ThrowIfZeroOrMultipleTenantInfoObjectsReturned(tenantInfo, externalDirectoryOrgId);
				RmsUtil.ThrowIfErrorInfoObjectReturned(tenantInfo[0], externalDirectoryOrgId);
				RmsUtil.ThrowIfTenantInfoDoesNotIncludeActiveTPD(tenantInfo[0], externalDirectoryOrgId);
				RmsUtil.ThrowIfTpdDoesNotIncludeKeyInformation(tenantInfo[0].ActivePublishingDomain, externalDirectoryOrgId);
				RmsUtil.ThrowIfTpdDoesNotIncludeSLC(tenantInfo[0].ActivePublishingDomain, externalDirectoryOrgId);
				RmsUtil.ThrowIfTpdDoesNotIncludeTemplates(tenantInfo[0].ActivePublishingDomain, externalDirectoryOrgId);
				RmsUtil.ThrowIfTenantInfoDoesNotIncludeLicensingUrls(tenantInfo[0], externalDirectoryOrgId);
				RmsUtil.ThrowIfTenantInfoDoesNotIncludeCertificationUrls(tenantInfo[0], externalDirectoryOrgId);
				this.IntranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(tenantInfo[0].LicensingIntranetDistributionPointUrl);
				this.ExtranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(tenantInfo[0].LicensingExtranetDistributionPointUrl);
				this.IntranetCertificationUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(tenantInfo[0].CertificationIntranetDistributionPointUrl);
				this.ExtranetCertificationUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(tenantInfo[0].CertificationExtranetDistributionPointUrl);
				result = RmsUtil.ConvertFromRmsOnlineTrustedDocDomain(tenantInfo[0].ActivePublishingDomain);
			}
			catch (FaultException<ArgumentException> innerException)
			{
				throw new ImportTpdException("Caught FaultException<ArgumentException> while obtaining TPD from RMS Online", innerException);
			}
			catch (CommunicationException innerException2)
			{
				throw new ImportTpdException("Unable to communicate with RMS Online key sharing web service", innerException2);
			}
			catch (TimeoutException innerException3)
			{
				throw new ImportTpdException("The TPD import request to the RMS Online key sharing web service has timed out", innerException3);
			}
			return result;
		}

		public virtual X509Certificate2 LoadAuthenticationCertificate()
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			X509Certificate2 result;
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
				result = this.ReadAuthenticationCertificateFromStore(x509Store);
			}
			catch (CryptographicException innerException)
			{
				throw new ImportTpdException("Caught CryptographicException when attempting to load RMS Online authentication certificate", innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new ImportTpdException("Caught SecurityException when attempting to load RMS Online authentication certificate", innerException2);
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		protected virtual ITenantManagementService CreateRmsOnlineWebServiceProxy(X509Certificate2 authenticationCertificate)
		{
			TenantManagementServiceClient tenantManagementServiceClient = new TenantManagementServiceClient(new WSHttpBinding
			{
				SendTimeout = RmsOnlineConstants.SendTimeout,
				ReceiveTimeout = RmsOnlineConstants.ReceiveTimeout,
				ReaderQuotas = RmsOnlineConstants.ReaderQuotas,
				MaxReceivedMessageSize = RmsOnlineConstants.MaxReceivedMessageSize,
				Name = RmsOnlineConstants.BindingName,
				Security = RmsOnlineConstants.Security
			}, new EndpointAddress(this.rmsOnlineKeySharingLocation, new AddressHeader[0]));
			RmsUtil.ThrowIfClientCredentialsIsNull(tenantManagementServiceClient);
			if (tenantManagementServiceClient.ClientCredentials != null)
			{
				tenantManagementServiceClient.ClientCredentials.ClientCertificate.Certificate = authenticationCertificate;
			}
			return tenantManagementServiceClient;
		}

		protected virtual string AuthenticationCertificateSubjectDistinguishedName
		{
			get
			{
				return this.authenticationCertificateSubjectName;
			}
		}

		protected virtual bool AcceptValidAuthenticationCertificateOnly
		{
			get
			{
				return true;
			}
		}

		protected virtual X509Certificate2 ReadAuthenticationCertificateFromStore(X509Store store)
		{
			X509Certificate2Collection certificates = store.Certificates;
			RmsUtil.ThrowIfCertificateCollectionIsNullOrEmpty(certificates, "X509Store returned a null or empty certificate collection; unable to load the RMS Online authentication certificate");
			X509Certificate2Collection x509Certificate2Collection = certificates.Find(X509FindType.FindBySubjectDistinguishedName, this.AuthenticationCertificateSubjectDistinguishedName, this.AcceptValidAuthenticationCertificateOnly);
			RmsUtil.ThrowIfCertificateCollectionIsNullOrEmpty(x509Certificate2Collection, string.Format("X509Store was unable to find the RMS Online authentication certificate with distinguished name '{0}'", this.AuthenticationCertificateSubjectDistinguishedName));
			return x509Certificate2Collection[0];
		}

		protected virtual void ThrowIfAuthenticationCertificateIsInvalid(X509Certificate2 authenticationCertificate)
		{
			if (authenticationCertificate == null)
			{
				throw new ImportTpdException("X.509 authentication certificate is not valid for the RMS Online service", null);
			}
		}

		private readonly Uri rmsOnlineKeySharingLocation;

		private readonly string authenticationCertificateSubjectName;
	}
}
