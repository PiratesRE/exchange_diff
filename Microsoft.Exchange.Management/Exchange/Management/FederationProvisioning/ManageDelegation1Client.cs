using System;
using System.Net.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.ManageDelegation1;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal sealed class ManageDelegation1Client : ManageDelegationClient, IDisposable
	{
		protected override CustomSoapHttpClientProtocol Client
		{
			get
			{
				return this.manageDelegation;
			}
		}

		public ManageDelegation1Client(string certificate, WriteVerboseDelegate writeVerbose) : base(LiveConfiguration.GetDomainServicesEpr().ToString(), certificate, writeVerbose)
		{
			this.manageDelegation = new ManageDelegation("ManageDelegation", new RemoteCertificateValidationCallback(ManageDelegationClient.InvalidCertificateHandler));
			this.manageDelegation.ClientCertificates.Add(base.Certificate);
		}

		public void Dispose()
		{
			if (this.manageDelegation != null)
			{
				this.manageDelegation.Dispose();
				this.manageDelegation = null;
			}
		}

		public AppIdInfo CreateAppId(string rawBase64Certificate)
		{
			AppIdInfo appIdInfo = null;
			base.ExecuteAndHandleError(string.Format("CreateAppId(certificate='{0}',properties=[])", rawBase64Certificate), delegate
			{
				appIdInfo = this.manageDelegation.CreateAppId(rawBase64Certificate, new Property[0]);
			});
			return appIdInfo;
		}

		public void UpdateAppIdCertificate(string applicationId, string adminKey, string rawBase64Certificate)
		{
			base.ExecuteAndHandleError(string.Format("UpdateAppIdCertificate(applicationId='{0}',adminKey='****', certificate='{1}')", applicationId, rawBase64Certificate), delegate
			{
				this.manageDelegation.UpdateAppIdCertificate(applicationId, adminKey, rawBase64Certificate);
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

		private const string ComponentId = "ManageDelegation";

		private ManageDelegation manageDelegation;
	}
}
