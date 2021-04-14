using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "ExchangeCertificate", SupportsShouldProcess = true)]
	public class InstallExchangeCertificate : DataAccessTask<Server>
	{
		[Parameter(Mandatory = true)]
		public AllowedServices Services
		{
			get
			{
				return this.services;
			}
			set
			{
				this.services = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DoNotRequireSsl
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotRequireSsl"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DoNotRequireSsl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public virtual string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
			set
			{
				this.thumbprint = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebSiteName
		{
			get
			{
				return this.webSiteName;
			}
			set
			{
				this.webSiteName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InstallInTrustedRootCAIfSelfSigned
		{
			get
			{
				return this.installInTrustedRootCAIfSelfSigned;
			}
			set
			{
				this.installInTrustedRootCAIfSelfSigned = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter NetworkServiceAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["NetworkServiceAllowed"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["NetworkServiceAllowed"] = value;
			}
		}

		internal override IConfigurationSession ConfigurationSession
		{
			get
			{
				return (IConfigurationSession)base.DataSession;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 182, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\InstallExchangeCertificate.cs");
		}

		protected override void InternalValidate()
		{
			bool flag = false;
			try
			{
				this.localServer = ManageExchangeCertificate.FindLocalServer((ITopologyConfigurationSession)this.ConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(this.localServer))
			{
				base.WriteError(new RoleDoesNotSupportExchangeCertificateTasksException(), ErrorCategory.InvalidOperation, null);
			}
			ManageExchangeCertificate.AddUniqueDomainIfValid(this.rawDomains, ComputerInformation.DnsHostName);
			ManageExchangeCertificate.AddUniqueDomainIfValid(this.rawDomains, ComputerInformation.DnsPhysicalHostName);
			ManageExchangeCertificate.AddUniqueDomainIfValid(this.rawDomains, ComputerInformation.DnsFullyQualifiedDomainName);
			ManageExchangeCertificate.AddUniqueDomainIfValid(this.rawDomains, ComputerInformation.DnsPhysicalFullyQualifiedDomainName);
			this.subjectName = TlsCertificateInfo.GetDefaultSubjectName(this.rawDomains);
		}

		protected X509Certificate2 GenerateSelfSignedCertificate()
		{
			TimeSpan validFor = DateTime.UtcNow.AddMonths(60) - DateTime.UtcNow;
			return TlsCertificateInfo.CreateSelfSignCertificate(this.subjectName, this.rawDomains, validFor, CertificateCreationOption.None, 2048, null);
		}

		protected override void InternalProcessRecord()
		{
			X509Certificate2 x509Certificate = null;
			if (!string.IsNullOrEmpty(this.thumbprint))
			{
				this.thumbprint = ManageExchangeCertificate.UnifyThumbprintFormat(this.thumbprint);
				x509Certificate = this.FindCertificate(this.thumbprint);
				if (x509Certificate == null)
				{
					base.WriteError(new ArgumentException(Strings.CertificateNotFound(this.thumbprint), "Thumbprint"), ErrorCategory.InvalidArgument, this.thumbprint);
				}
			}
			else
			{
				AllowedServices allowedServices = this.Services;
				if (allowedServices != AllowedServices.IIS && allowedServices != (AllowedServices.IMAP | AllowedServices.POP | AllowedServices.IIS))
				{
					if (allowedServices != AllowedServices.SMTP)
					{
						return;
					}
				}
				else
				{
					x509Certificate = this.FindIisCertificate();
				}
				if (x509Certificate == null && this.Services != AllowedServices.SMTP)
				{
					try
					{
						x509Certificate = InstallExchangeCertificate.GetDefaultCertificate();
					}
					catch (ArgumentException exception)
					{
						base.WriteError(exception, ErrorCategory.InvalidData, null);
						return;
					}
				}
				if (x509Certificate == null)
				{
					if (!this.rawDomains.Any<string>())
					{
						base.WriteError(new UnableToResolveValidDomainExchangeCertificateTasksException(ComputerInformation.DnsHostName, ComputerInformation.DnsPhysicalHostName, ComputerInformation.DnsFullyQualifiedDomainName, ComputerInformation.DnsPhysicalFullyQualifiedDomainName), ErrorCategory.InvalidOperation, null);
					}
					try
					{
						x509Certificate = this.GenerateSelfSignedCertificate();
					}
					catch (CryptographicException exception2)
					{
						base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
					}
				}
				if (x509Certificate != null && this.InstallInTrustedRootCAIfSelfSigned && TlsCertificateInfo.IsSelfSignedCertificate(x509Certificate))
				{
					TlsCertificateInfo.TryInstallCertificateInTrustedRootCA(x509Certificate);
				}
			}
			base.WriteVerbose(Strings.CertificateInformation(x509Certificate.Issuer, x509Certificate.NotBefore, x509Certificate.NotAfter, x509Certificate.Subject));
			if ((DateTime)ExDateTime.Now < x509Certificate.NotBefore || (DateTime)ExDateTime.Now > x509Certificate.NotAfter)
			{
				base.WriteError(new CryptographicException(Strings.CertificateStatusDateInvalid), ErrorCategory.InvalidData, null);
			}
			try
			{
				this.EnableForServices(x509Certificate, this.Services);
			}
			catch (IISNotInstalledException)
			{
				base.WriteError(new ArgumentException(Strings.IISNotInstalled, "Services"), ErrorCategory.InvalidArgument, null);
			}
			catch (InvalidOperationException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ObjectNotFound, null);
			}
		}

		protected void EnableForServices(X509Certificate2 cert, AllowedServices services)
		{
			try
			{
				ManageExchangeCertificate.EnableForServices(cert, services, this.webSiteName, !this.DoNotRequireSsl, (ITopologyConfigurationSession)base.DataSession, this.localServer, null, false, this.NetworkServiceAllowed);
			}
			catch (IISNotInstalledException)
			{
				base.WriteError(new ArgumentException(Strings.IISNotInstalled, "Services"), ErrorCategory.InvalidArgument, null);
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ErrorCategory.NotSpecified, null);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(AddAccessRuleCryptographicException).IsInstanceOfType(exception) || typeof(AddAccessRuleArgumentException).IsInstanceOfType(exception) || typeof(AddAccessRuleUnauthorizedAccessException).IsInstanceOfType(exception) || typeof(AddAccessRuleCOMException).IsInstanceOfType(exception);
		}

		private static X509Certificate2 GetDefaultCertificate()
		{
			string[] names = new string[]
			{
				ComputerInformation.DnsFullyQualifiedDomainName,
				ComputerInformation.DnsHostName,
				ComputerInformation.DnsPhysicalFullyQualifiedDomainName,
				ComputerInformation.DnsPhysicalHostName
			};
			return TlsCertificateInfo.FindCertificate(names, CertificateSelectionOption.PreferedNonSelfSigned);
		}

		private X509Certificate2 FindIisCertificate()
		{
			string webSiteSslCertificate = IisUtility.GetWebSiteSslCertificate("IIS://localhost/W3SVC/1");
			if (string.IsNullOrEmpty(webSiteSslCertificate))
			{
				return null;
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
			X509Certificate2Collection x509Certificate2Collection;
			try
			{
				x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, webSiteSslCertificate, false);
			}
			finally
			{
				x509Store.Close();
			}
			if (x509Certificate2Collection.Count > 0)
			{
				return x509Certificate2Collection[0];
			}
			return null;
		}

		private X509Certificate2 FindCertificate(string thumbprint)
		{
			if (string.IsNullOrEmpty(thumbprint))
			{
				return null;
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.OpenExistingOnly);
			X509Certificate2Collection x509Certificate2Collection;
			try
			{
				x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
			}
			finally
			{
				x509Store.Close();
			}
			if (x509Certificate2Collection.Count > 0)
			{
				return x509Certificate2Collection[0];
			}
			return null;
		}

		private const int KeySize = 2048;

		private Server localServer;

		private X500DistinguishedName subjectName;

		private List<string> rawDomains = new List<string>();

		private string domainController;

		private AllowedServices services = AllowedServices.SMTP;

		private string thumbprint;

		private string webSiteName;

		private bool installInTrustedRootCAIfSelfSigned;
	}
}
