using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Microsoft.BDM.Pets.DNSManagement;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "WebDnsRecord", DefaultParameterSetName = "Identity")]
	public sealed class GetWebDnsRecord : ObjectActionTenantADTask<AcceptedDomainIdParameter, AcceptedDomain>
	{
		[Parameter(Mandatory = false)]
		public Uri DnsEndPoint
		{
			get
			{
				return (Uri)base.Fields["DnsEndPoint"];
			}
			set
			{
				base.Fields["DnsEndPoint"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Thumbprint
		{
			get
			{
				return (string)base.Fields["thumbprint"];
			}
			set
			{
				base.Fields["thumbprint"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(this.Identity.ToString()), 97, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\WebDns\\GetWebDnsRecord.cs");
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					if (!this.GetDnsServiceData())
					{
						base.WriteError(new LocalizedException(Strings.ErrorMissingWebDnsInformation), (ErrorCategory)1000, this);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			WebSvcDns webSvcDns = new WebSvcDns(null, "");
			DNSWebSvcClient dnswebSvcClient = null;
			AcceptedDomain dataObject = this.DataObject;
			string domain = dataObject.DomainName.Domain;
			try
			{
				dnswebSvcClient = new DNSWebSvcClient(webSvcDns.Wsb, new EndpointAddress(this.dnsEndpoint, new AddressHeader[0]));
				dnswebSvcClient.ClientCredentials.ClientCertificate.Certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(this.certificateSubject);
				dnswebSvcClient.Open();
				if (dnswebSvcClient.IsDomainAvailable(domain))
				{
					return;
				}
				ResourceRecord[] allResourceRecordsByDomainName = dnswebSvcClient.GetAllResourceRecordsByDomainName(domain);
				foreach (ResourceRecord record in allResourceRecordsByDomainName)
				{
					base.WriteObject(new WebDnsRecord(record));
				}
			}
			catch (TimeoutException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, dataObject);
			}
			catch (SecurityAccessDeniedException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, dataObject);
			}
			catch (CommunicationException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, dataObject);
			}
			finally
			{
				if (dnswebSvcClient != null)
				{
					dnswebSvcClient.Close();
				}
			}
			TaskLogger.LogExit();
		}

		private bool GetDnsServiceData()
		{
			bool result = true;
			AcceptedDomainUtility.ReloadProvisioningParameters();
			if (base.Fields.IsModified("DnsEndPoint"))
			{
				this.dnsEndpoint = this.DnsEndPoint;
			}
			else
			{
				this.dnsEndpoint = AcceptedDomainUtility.DnsRegistrationEndpoint;
			}
			if (base.Fields.IsModified("thumbprint"))
			{
				X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite);
				try
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, this.Thumbprint, false);
					if (x509Certificate2Collection.Count > 0)
					{
						X509Certificate2 x509Certificate = x509Certificate2Collection[0];
						this.certificateSubject = x509Certificate.SubjectName.ToString();
					}
					goto IL_9D;
				}
				finally
				{
					x509Store.Close();
				}
			}
			this.certificateSubject = AcceptedDomainUtility.DnsRegistrationCertificateSubject;
			IL_9D:
			if (string.IsNullOrEmpty(this.certificateSubject) || this.dnsEndpoint == null || string.IsNullOrEmpty(this.dnsEndpoint.ToString()))
			{
				result = false;
			}
			return result;
		}

		private Uri dnsEndpoint;

		private string certificateSubject;
	}
}
