using System;
using System.Net.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.ManageDelegation2;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal sealed class ManageDelegation2Client : ManageDelegationClient, IDisposable
	{
		protected override CustomSoapHttpClientProtocol Client
		{
			get
			{
				return this.manageDelegation;
			}
		}

		public ManageDelegation2Client(string domain, string signingDomain, string certificateThumbprint, WriteVerboseDelegate writeVerbose) : base(LiveConfiguration.GetDomainServices2Epr().ToString(), certificateThumbprint, writeVerbose)
		{
			this.manageDelegation = new ManageDelegation2("ManageDelegation2", new RemoteCertificateValidationCallback(ManageDelegationClient.InvalidCertificateHandler));
			this.manageDelegation.Authenticator = SoapHttpClientAuthenticator.Create(base.Certificate);
			this.manageDelegation.DomainOwnershipProofHeaderValue = new DomainOwnershipProofHeader
			{
				Domain = domain,
				HashAlgorithm = "SHA-512",
				Signature = Convert.ToBase64String(FederatedDomainProofAlgorithm.GetSignature(base.Certificate, signingDomain))
			};
		}

		public void Dispose()
		{
			if (this.manageDelegation != null)
			{
				this.manageDelegation.Dispose();
				this.manageDelegation = null;
			}
		}

		public AppIdInfo CreateAppId(string uri)
		{
			AppIdInfo appIdInfo = null;
			base.ExecuteAndHandleError(string.Format("CreateAppId(uri='{0}',properties=[0])", uri), delegate
			{
				appIdInfo = this.manageDelegation.CreateAppId(uri, new Property[0]);
			});
			return appIdInfo;
		}

		public void UpdateAppIdCertificate(string applicationId, string rawBase64Certificate)
		{
			base.ExecuteAndHandleError(string.Format("UpdateAppIdCertificate(applicationId='{0}',certificate='{1}')", applicationId, rawBase64Certificate), delegate
			{
				this.manageDelegation.UpdateAppIdCertificate(applicationId, rawBase64Certificate);
			});
		}

		public override void AddUri(string applicationId, string uri)
		{
			base.ExecuteAndHandleError(string.Format("AddUri(applicationId='{0}',uri='{1}')", applicationId, uri), delegate
			{
				this.manageDelegation.AddUri(applicationId, uri);
			});
		}

		public override void RemoveUri(string applicationId, string uri)
		{
			base.ExecuteAndHandleError(string.Format("RemoveUri(applicationId='{0}',uri='{1}')", applicationId, uri), delegate
			{
				this.manageDelegation.RemoveUri(applicationId, uri);
			});
		}

		public override void ReserveDomain(string applicationId, string domain, string programId)
		{
			base.ExecuteAndHandleError(string.Format("ReserveDomain(applicationId='{0}',domain='{1}',programId='{2}')", applicationId, domain, programId), delegate
			{
				this.manageDelegation.ReserveDomain(applicationId, domain, programId);
			});
		}

		public override void ReleaseDomain(string applicationId, string domain)
		{
			base.ExecuteAndHandleError(string.Format("ReleaseDomain(applicationId='{0}',domain='{1}')", applicationId, domain), delegate
			{
				this.manageDelegation.ReleaseDomain(applicationId, domain);
			});
		}

		public DomainInfo GetDomainInfo(string applicationId, string domain)
		{
			DomainInfo domainInfo = null;
			base.ExecuteAndHandleError(string.Format("GetDomainInfo(applicationId='{0}',domain='{1}')", applicationId, domain), delegate
			{
				domainInfo = this.manageDelegation.GetDomainInfo(applicationId, domain);
			});
			return domainInfo;
		}

		private const string ComponentId = "ManageDelegation2";

		private ManageDelegation2 manageDelegation;
	}
}
