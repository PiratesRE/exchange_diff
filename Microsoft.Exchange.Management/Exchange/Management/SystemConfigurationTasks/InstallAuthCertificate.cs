using System;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "AuthCertificate")]
	public class InstallAuthCertificate : Task
	{
		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		private IConfigurationSession ConfigSession { get; set; }

		protected override void InternalBeginProcessing()
		{
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 75, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OAuth\\InstallAuthCertificate.cs");
		}

		protected override void InternalProcessRecord()
		{
			if (this.IsAuthCertConfigured())
			{
				return;
			}
			X509Certificate2 certificate = this.GenerateCertificate();
			if (this.IsAuthCertConfigured())
			{
				this.DeleteCertificate(certificate);
				return;
			}
			this.StampAuthCertificate(certificate);
		}

		private bool IsAuthCertConfigured()
		{
			AuthConfig authConfig = AuthConfig.Read(this.ConfigSession);
			return authConfig == null || !string.IsNullOrEmpty(authConfig.CurrentCertificateThumbprint);
		}

		private void StampAuthCertificate(X509Certificate2 certificate)
		{
			AuthConfig authConfig = AuthConfig.Read(this.ConfigSession);
			authConfig.CurrentCertificateThumbprint = certificate.Thumbprint;
			this.ConfigSession.Save(authConfig);
		}

		private X509Certificate2 GenerateCertificate()
		{
			return TlsCertificateInfo.CreateSelfSignCertificate(new X500DistinguishedName("CN=" + InstallAuthCertificate.CertificateSubject), null, InstallAuthCertificate.CertificateLifeTime, CertificateCreationOption.Exportable, 2048, InstallAuthCertificate.CertificateSubject);
		}

		private void DeleteCertificate(X509Certificate2 certificate)
		{
			X509Store x509Store = null;
			try
			{
				x509Store = new X509Store(StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
				x509Store.Remove(certificate);
			}
			catch (CryptographicException)
			{
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is CryptographicException || base.IsKnownException(e);
		}

		private const int KeySize = 2048;

		internal static readonly string CertificateSubject = "Microsoft Exchange Server Auth Certificate";

		internal static readonly TimeSpan CertificateLifeTime = TimeSpan.FromDays(1800.0);
	}
}
