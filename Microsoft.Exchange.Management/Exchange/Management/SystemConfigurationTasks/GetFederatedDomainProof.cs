using System;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "FederatedDomainProof")]
	public sealed class GetFederatedDomainProof : Task
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public SmtpDomain DomainName { get; set; }

		[Parameter(Mandatory = false)]
		public string Thumbprint { get; set; }

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public Fqdn DomainController { get; set; }

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (!string.IsNullOrEmpty(this.Thumbprint))
			{
				this.ProcessForCertificate(this.Thumbprint, null);
			}
			else
			{
				FederationTrust federationTrust = this.GetFederationTrust();
				var array = new <>f__AnonymousType25<string, string>[]
				{
					new
					{
						PropertyName = "OrgNextPrivCertificate",
						Thumbprint = federationTrust.OrgNextPrivCertificate
					},
					new
					{
						PropertyName = "OrgPrivCertificate",
						Thumbprint = federationTrust.OrgPrivCertificate
					},
					new
					{
						PropertyName = "OrgPrevPrivCertificate",
						Thumbprint = federationTrust.OrgPrevPrivCertificate
					}
				};
				var array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					var <>f__AnonymousType = array2[i];
					if (!string.IsNullOrEmpty(<>f__AnonymousType.Thumbprint))
					{
						this.ProcessForCertificate(<>f__AnonymousType.Thumbprint, <>f__AnonymousType.PropertyName);
					}
				}
			}
			TaskLogger.LogExit();
		}

		private void ProcessForCertificate(string thumbprint, string propertyName)
		{
			X509Certificate2 certificate = null;
			try
			{
				certificate = FederationCertificate.LoadCertificateWithPrivateKey(thumbprint, new WriteVerboseDelegate(base.WriteVerbose));
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			byte[] signature = FederatedDomainProofAlgorithm.GetSignature(certificate, this.DomainName.Domain);
			using (HashAlgorithm hashAlgorithm = new SHA512Cng())
			{
				byte[] inArray = hashAlgorithm.ComputeHash(signature);
				base.WriteObject(new FederatedDomainProof(this.DomainName, propertyName, thumbprint, Convert.ToBase64String(inArray)));
			}
		}

		private FederationTrust GetFederationTrust()
		{
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 130, "GetFederationTrust", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\GetFederatedDomainProof.cs");
			FederationTrust[] array = configurationSession.Find<FederationTrust>(null, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.NoFederationTrust), (ErrorCategory)1003, null);
				return null;
			}
			if (array.Length > 1)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.TooManyFederationTrust), (ErrorCategory)1003, null);
				return null;
			}
			return array[0];
		}
	}
}
