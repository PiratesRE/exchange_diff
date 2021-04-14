using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSmimeConfigCommand : SyntheticCommandWithPipelineInputNoOutput<SmimeConfigurationContainer>
	{
		private SetSmimeConfigCommand() : base("Set-SmimeConfig")
		{
		}

		public SetSmimeConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSmimeConfigCommand SetParameters(SetSmimeConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetSmimeConfigCommand SetParameters(SetSmimeConfigCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool OWACheckCRLOnSend
			{
				set
				{
					base.PowerSharpParameters["OWACheckCRLOnSend"] = value;
				}
			}

			public virtual uint OWADLExpansionTimeout
			{
				set
				{
					base.PowerSharpParameters["OWADLExpansionTimeout"] = value;
				}
			}

			public virtual bool OWAUseSecondaryProxiesWhenFindingCertificates
			{
				set
				{
					base.PowerSharpParameters["OWAUseSecondaryProxiesWhenFindingCertificates"] = value;
				}
			}

			public virtual uint OWACRLConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["OWACRLConnectionTimeout"] = value;
				}
			}

			public virtual uint OWACRLRetrievalTimeout
			{
				set
				{
					base.PowerSharpParameters["OWACRLRetrievalTimeout"] = value;
				}
			}

			public virtual bool OWADisableCRLCheck
			{
				set
				{
					base.PowerSharpParameters["OWADisableCRLCheck"] = value;
				}
			}

			public virtual bool OWAAlwaysSign
			{
				set
				{
					base.PowerSharpParameters["OWAAlwaysSign"] = value;
				}
			}

			public virtual bool OWAAlwaysEncrypt
			{
				set
				{
					base.PowerSharpParameters["OWAAlwaysEncrypt"] = value;
				}
			}

			public virtual bool OWAClearSign
			{
				set
				{
					base.PowerSharpParameters["OWAClearSign"] = value;
				}
			}

			public virtual bool OWAIncludeCertificateChainWithoutRootCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeCertificateChainWithoutRootCertificate"] = value;
				}
			}

			public virtual bool OWAIncludeCertificateChainAndRootCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeCertificateChainAndRootCertificate"] = value;
				}
			}

			public virtual bool OWAEncryptTemporaryBuffers
			{
				set
				{
					base.PowerSharpParameters["OWAEncryptTemporaryBuffers"] = value;
				}
			}

			public virtual bool OWASignedEmailCertificateInclusion
			{
				set
				{
					base.PowerSharpParameters["OWASignedEmailCertificateInclusion"] = value;
				}
			}

			public virtual uint OWABCCEncryptedEmailForking
			{
				set
				{
					base.PowerSharpParameters["OWABCCEncryptedEmailForking"] = value;
				}
			}

			public virtual bool OWAIncludeSMIMECapabilitiesInMessage
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeSMIMECapabilitiesInMessage"] = value;
				}
			}

			public virtual bool OWACopyRecipientHeaders
			{
				set
				{
					base.PowerSharpParameters["OWACopyRecipientHeaders"] = value;
				}
			}

			public virtual bool OWAOnlyUseSmartCard
			{
				set
				{
					base.PowerSharpParameters["OWAOnlyUseSmartCard"] = value;
				}
			}

			public virtual bool OWATripleWrapSignedEncryptedMail
			{
				set
				{
					base.PowerSharpParameters["OWATripleWrapSignedEncryptedMail"] = value;
				}
			}

			public virtual bool OWAUseKeyIdentifier
			{
				set
				{
					base.PowerSharpParameters["OWAUseKeyIdentifier"] = value;
				}
			}

			public virtual string OWAEncryptionAlgorithms
			{
				set
				{
					base.PowerSharpParameters["OWAEncryptionAlgorithms"] = value;
				}
			}

			public virtual string OWASigningAlgorithms
			{
				set
				{
					base.PowerSharpParameters["OWASigningAlgorithms"] = value;
				}
			}

			public virtual bool OWAForceSMIMEClientUpgrade
			{
				set
				{
					base.PowerSharpParameters["OWAForceSMIMEClientUpgrade"] = value;
				}
			}

			public virtual string OWASenderCertificateAttributesToDisplay
			{
				set
				{
					base.PowerSharpParameters["OWASenderCertificateAttributesToDisplay"] = value;
				}
			}

			public virtual bool OWAAllowUserChoiceOfSigningCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAAllowUserChoiceOfSigningCertificate"] = value;
				}
			}

			public virtual byte SMIMECertificateIssuingCA
			{
				set
				{
					base.PowerSharpParameters["SMIMECertificateIssuingCA"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual bool OWACheckCRLOnSend
			{
				set
				{
					base.PowerSharpParameters["OWACheckCRLOnSend"] = value;
				}
			}

			public virtual uint OWADLExpansionTimeout
			{
				set
				{
					base.PowerSharpParameters["OWADLExpansionTimeout"] = value;
				}
			}

			public virtual bool OWAUseSecondaryProxiesWhenFindingCertificates
			{
				set
				{
					base.PowerSharpParameters["OWAUseSecondaryProxiesWhenFindingCertificates"] = value;
				}
			}

			public virtual uint OWACRLConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["OWACRLConnectionTimeout"] = value;
				}
			}

			public virtual uint OWACRLRetrievalTimeout
			{
				set
				{
					base.PowerSharpParameters["OWACRLRetrievalTimeout"] = value;
				}
			}

			public virtual bool OWADisableCRLCheck
			{
				set
				{
					base.PowerSharpParameters["OWADisableCRLCheck"] = value;
				}
			}

			public virtual bool OWAAlwaysSign
			{
				set
				{
					base.PowerSharpParameters["OWAAlwaysSign"] = value;
				}
			}

			public virtual bool OWAAlwaysEncrypt
			{
				set
				{
					base.PowerSharpParameters["OWAAlwaysEncrypt"] = value;
				}
			}

			public virtual bool OWAClearSign
			{
				set
				{
					base.PowerSharpParameters["OWAClearSign"] = value;
				}
			}

			public virtual bool OWAIncludeCertificateChainWithoutRootCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeCertificateChainWithoutRootCertificate"] = value;
				}
			}

			public virtual bool OWAIncludeCertificateChainAndRootCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeCertificateChainAndRootCertificate"] = value;
				}
			}

			public virtual bool OWAEncryptTemporaryBuffers
			{
				set
				{
					base.PowerSharpParameters["OWAEncryptTemporaryBuffers"] = value;
				}
			}

			public virtual bool OWASignedEmailCertificateInclusion
			{
				set
				{
					base.PowerSharpParameters["OWASignedEmailCertificateInclusion"] = value;
				}
			}

			public virtual uint OWABCCEncryptedEmailForking
			{
				set
				{
					base.PowerSharpParameters["OWABCCEncryptedEmailForking"] = value;
				}
			}

			public virtual bool OWAIncludeSMIMECapabilitiesInMessage
			{
				set
				{
					base.PowerSharpParameters["OWAIncludeSMIMECapabilitiesInMessage"] = value;
				}
			}

			public virtual bool OWACopyRecipientHeaders
			{
				set
				{
					base.PowerSharpParameters["OWACopyRecipientHeaders"] = value;
				}
			}

			public virtual bool OWAOnlyUseSmartCard
			{
				set
				{
					base.PowerSharpParameters["OWAOnlyUseSmartCard"] = value;
				}
			}

			public virtual bool OWATripleWrapSignedEncryptedMail
			{
				set
				{
					base.PowerSharpParameters["OWATripleWrapSignedEncryptedMail"] = value;
				}
			}

			public virtual bool OWAUseKeyIdentifier
			{
				set
				{
					base.PowerSharpParameters["OWAUseKeyIdentifier"] = value;
				}
			}

			public virtual string OWAEncryptionAlgorithms
			{
				set
				{
					base.PowerSharpParameters["OWAEncryptionAlgorithms"] = value;
				}
			}

			public virtual string OWASigningAlgorithms
			{
				set
				{
					base.PowerSharpParameters["OWASigningAlgorithms"] = value;
				}
			}

			public virtual bool OWAForceSMIMEClientUpgrade
			{
				set
				{
					base.PowerSharpParameters["OWAForceSMIMEClientUpgrade"] = value;
				}
			}

			public virtual string OWASenderCertificateAttributesToDisplay
			{
				set
				{
					base.PowerSharpParameters["OWASenderCertificateAttributesToDisplay"] = value;
				}
			}

			public virtual bool OWAAllowUserChoiceOfSigningCertificate
			{
				set
				{
					base.PowerSharpParameters["OWAAllowUserChoiceOfSigningCertificate"] = value;
				}
			}

			public virtual byte SMIMECertificateIssuingCA
			{
				set
				{
					base.PowerSharpParameters["SMIMECertificateIssuingCA"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
