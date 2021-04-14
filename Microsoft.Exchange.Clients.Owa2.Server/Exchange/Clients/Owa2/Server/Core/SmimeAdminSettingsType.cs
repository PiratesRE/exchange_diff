using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SmimeAdminSettingsType : UserConfigurationBaseType
	{
		public SmimeAdminSettingsType(OrganizationId organizationId) : this(SmimeAdminSettingsType.ReadSmimeSettingsFromAd(organizationId))
		{
		}

		public SmimeAdminSettingsType(ISmimeSettingsProvider settingsProvider) : base("OWA.SmimeAdminSettingsType")
		{
			this.smimeConfigurationContainer = settingsProvider;
		}

		[DataMember]
		public bool CheckCRLOnSend
		{
			get
			{
				return this.smimeConfigurationContainer.OWACheckCRLOnSend;
			}
			set
			{
			}
		}

		[DataMember]
		public uint DLExpansionTimeout
		{
			get
			{
				return this.smimeConfigurationContainer.OWADLExpansionTimeout;
			}
			set
			{
			}
		}

		[DataMember]
		public bool UseSecondaryProxiesWhenFindingCertificates
		{
			get
			{
				return this.smimeConfigurationContainer.OWAUseSecondaryProxiesWhenFindingCertificates;
			}
			set
			{
			}
		}

		[DataMember]
		public uint CRLConnectionTimeout
		{
			get
			{
				return this.smimeConfigurationContainer.OWACRLConnectionTimeout;
			}
			set
			{
			}
		}

		[DataMember]
		public uint CRLRetrievalTimeout
		{
			get
			{
				return this.smimeConfigurationContainer.OWACRLRetrievalTimeout;
			}
			set
			{
			}
		}

		[DataMember]
		public bool DisableCRLCheck
		{
			get
			{
				return this.smimeConfigurationContainer.OWADisableCRLCheck;
			}
			set
			{
			}
		}

		[DataMember]
		public bool AlwaysSign
		{
			get
			{
				return this.smimeConfigurationContainer.OWAAlwaysSign;
			}
			set
			{
			}
		}

		[DataMember]
		public bool AlwaysEncrypt
		{
			get
			{
				return this.smimeConfigurationContainer.OWAAlwaysEncrypt;
			}
			set
			{
			}
		}

		[DataMember]
		public bool ClearSign
		{
			get
			{
				return this.smimeConfigurationContainer.OWAClearSign;
			}
			set
			{
			}
		}

		[DataMember]
		public bool IncludeCertificateChainWithoutRootCertificate
		{
			get
			{
				return this.smimeConfigurationContainer.OWAIncludeCertificateChainWithoutRootCertificate;
			}
			set
			{
			}
		}

		[DataMember]
		public bool IncludeCertificateChainAndRootCertificate
		{
			get
			{
				return this.smimeConfigurationContainer.OWAIncludeCertificateChainAndRootCertificate;
			}
			set
			{
			}
		}

		[DataMember]
		public bool EncryptTemporaryBuffers
		{
			get
			{
				return this.smimeConfigurationContainer.OWAEncryptTemporaryBuffers;
			}
			set
			{
			}
		}

		[DataMember]
		public bool SignedEmailCertificateInclusion
		{
			get
			{
				return this.smimeConfigurationContainer.OWASignedEmailCertificateInclusion;
			}
			set
			{
			}
		}

		[DataMember]
		public uint BccEncryptedEmailForking
		{
			get
			{
				return this.smimeConfigurationContainer.OWABCCEncryptedEmailForking;
			}
			set
			{
			}
		}

		[DataMember]
		public bool IncludeSMIMECapabilitiesInMessage
		{
			get
			{
				return this.smimeConfigurationContainer.OWAIncludeSMIMECapabilitiesInMessage;
			}
			set
			{
			}
		}

		[DataMember]
		public bool CopyRecipientHeaders
		{
			get
			{
				return this.smimeConfigurationContainer.OWACopyRecipientHeaders;
			}
			set
			{
			}
		}

		[DataMember]
		public bool OnlyUseSmartCard
		{
			get
			{
				return this.smimeConfigurationContainer.OWAOnlyUseSmartCard;
			}
			set
			{
			}
		}

		[DataMember]
		public bool TripleWrapSignedEncryptedMail
		{
			get
			{
				return this.smimeConfigurationContainer.OWATripleWrapSignedEncryptedMail;
			}
			set
			{
			}
		}

		[DataMember]
		public bool UseKeyIdentifier
		{
			get
			{
				return this.smimeConfigurationContainer.OWAUseKeyIdentifier;
			}
			set
			{
			}
		}

		[DataMember]
		public string EncryptionAlgorithms
		{
			get
			{
				return this.smimeConfigurationContainer.OWAEncryptionAlgorithms;
			}
			set
			{
			}
		}

		[DataMember]
		public string SigningAlgorithms
		{
			get
			{
				return this.smimeConfigurationContainer.OWASigningAlgorithms;
			}
			set
			{
			}
		}

		[DataMember]
		public bool ForceSmimeClientUpgrade
		{
			get
			{
				return this.smimeConfigurationContainer.OWAForceSMIMEClientUpgrade;
			}
			set
			{
			}
		}

		[DataMember]
		public string SenderCertificateAttributesToDisplay
		{
			get
			{
				return this.smimeConfigurationContainer.OWASenderCertificateAttributesToDisplay;
			}
			set
			{
			}
		}

		[DataMember]
		public bool AllowUserChoiceOfSigningCertificate
		{
			get
			{
				return this.smimeConfigurationContainer.OWAAllowUserChoiceOfSigningCertificate;
			}
			set
			{
			}
		}

		public string SMIMECertificateIssuingCAFull
		{
			get
			{
				return this.smimeConfigurationContainer.SMIMECertificateIssuingCAFull();
			}
			set
			{
			}
		}

		internal override UserConfigurationPropertySchemaBase Schema
		{
			get
			{
				return UserOptionPropertySchema.Instance;
			}
		}

		internal static ISmimeSettingsProvider ReadSmimeSettingsFromAd(OrganizationId organizationId)
		{
			CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(organizationId, TimeSpan.FromHours(1.0), CachedOrganizationConfiguration.ConfigurationTypes.All);
			return instance.SmimeSettings.Configuration;
		}

		private const string ConfigurationName = "OWA.SmimeAdminSettingsType";

		private readonly ISmimeSettingsProvider smimeConfigurationContainer;
	}
}
