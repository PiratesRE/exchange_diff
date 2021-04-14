using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewExchangeCertificateCommand : SyntheticCommandWithPipelineInput<X509Certificate2, X509Certificate2>
	{
		private NewExchangeCertificateCommand() : base("New-ExchangeCertificate")
		{
		}

		public NewExchangeCertificateCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewExchangeCertificateCommand SetParameters(NewExchangeCertificateCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewExchangeCertificateCommand SetParameters(NewExchangeCertificateCommand.RequestParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewExchangeCertificateCommand SetParameters(NewExchangeCertificateCommand.CertificateParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter IncludeAutoDiscover
			{
				set
				{
					base.PowerSharpParameters["IncludeAutoDiscover"] = value;
				}
			}

			public virtual string FriendlyName
			{
				set
				{
					base.PowerSharpParameters["FriendlyName"] = value;
				}
			}

			public virtual X509Certificate2 Instance
			{
				set
				{
					base.PowerSharpParameters["Instance"] = value;
				}
			}

			public virtual SwitchParameter IncludeAcceptedDomains
			{
				set
				{
					base.PowerSharpParameters["IncludeAcceptedDomains"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerFQDN
			{
				set
				{
					base.PowerSharpParameters["IncludeServerFQDN"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerNetBIOSName
			{
				set
				{
					base.PowerSharpParameters["IncludeServerNetBIOSName"] = value;
				}
			}

			public virtual X500DistinguishedName SubjectName
			{
				set
				{
					base.PowerSharpParameters["SubjectName"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomainWithSubdomains> DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual int KeySize
			{
				set
				{
					base.PowerSharpParameters["KeySize"] = value;
				}
			}

			public virtual bool PrivateKeyExportable
			{
				set
				{
					base.PowerSharpParameters["PrivateKeyExportable"] = value;
				}
			}

			public virtual string SubjectKeyIdentifier
			{
				set
				{
					base.PowerSharpParameters["SubjectKeyIdentifier"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class RequestParameters : ParametersBase
		{
			public virtual SwitchParameter GenerateRequest
			{
				set
				{
					base.PowerSharpParameters["GenerateRequest"] = value;
				}
			}

			public virtual string RequestFile
			{
				set
				{
					base.PowerSharpParameters["RequestFile"] = value;
				}
			}

			public virtual SwitchParameter BinaryEncoded
			{
				set
				{
					base.PowerSharpParameters["BinaryEncoded"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter IncludeAutoDiscover
			{
				set
				{
					base.PowerSharpParameters["IncludeAutoDiscover"] = value;
				}
			}

			public virtual string FriendlyName
			{
				set
				{
					base.PowerSharpParameters["FriendlyName"] = value;
				}
			}

			public virtual X509Certificate2 Instance
			{
				set
				{
					base.PowerSharpParameters["Instance"] = value;
				}
			}

			public virtual SwitchParameter IncludeAcceptedDomains
			{
				set
				{
					base.PowerSharpParameters["IncludeAcceptedDomains"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerFQDN
			{
				set
				{
					base.PowerSharpParameters["IncludeServerFQDN"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerNetBIOSName
			{
				set
				{
					base.PowerSharpParameters["IncludeServerNetBIOSName"] = value;
				}
			}

			public virtual X500DistinguishedName SubjectName
			{
				set
				{
					base.PowerSharpParameters["SubjectName"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomainWithSubdomains> DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual int KeySize
			{
				set
				{
					base.PowerSharpParameters["KeySize"] = value;
				}
			}

			public virtual bool PrivateKeyExportable
			{
				set
				{
					base.PowerSharpParameters["PrivateKeyExportable"] = value;
				}
			}

			public virtual string SubjectKeyIdentifier
			{
				set
				{
					base.PowerSharpParameters["SubjectKeyIdentifier"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class CertificateParameters : ParametersBase
		{
			public virtual AllowedServices Services
			{
				set
				{
					base.PowerSharpParameters["Services"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter IncludeAutoDiscover
			{
				set
				{
					base.PowerSharpParameters["IncludeAutoDiscover"] = value;
				}
			}

			public virtual string FriendlyName
			{
				set
				{
					base.PowerSharpParameters["FriendlyName"] = value;
				}
			}

			public virtual X509Certificate2 Instance
			{
				set
				{
					base.PowerSharpParameters["Instance"] = value;
				}
			}

			public virtual SwitchParameter IncludeAcceptedDomains
			{
				set
				{
					base.PowerSharpParameters["IncludeAcceptedDomains"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerFQDN
			{
				set
				{
					base.PowerSharpParameters["IncludeServerFQDN"] = value;
				}
			}

			public virtual SwitchParameter IncludeServerNetBIOSName
			{
				set
				{
					base.PowerSharpParameters["IncludeServerNetBIOSName"] = value;
				}
			}

			public virtual X500DistinguishedName SubjectName
			{
				set
				{
					base.PowerSharpParameters["SubjectName"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomainWithSubdomains> DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual int KeySize
			{
				set
				{
					base.PowerSharpParameters["KeySize"] = value;
				}
			}

			public virtual bool PrivateKeyExportable
			{
				set
				{
					base.PowerSharpParameters["PrivateKeyExportable"] = value;
				}
			}

			public virtual string SubjectKeyIdentifier
			{
				set
				{
					base.PowerSharpParameters["SubjectKeyIdentifier"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
