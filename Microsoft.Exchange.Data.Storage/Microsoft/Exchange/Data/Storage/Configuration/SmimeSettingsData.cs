using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class SmimeSettingsData : SerializableDataBase, IEquatable<SmimeSettingsData>, ISmimeSettingsProvider
	{
		public SmimeSettingsData(ISmimeSettingsProvider source)
		{
			this.OWAAllowUserChoiceOfSigningCertificate = source.OWAAllowUserChoiceOfSigningCertificate;
			this.OWAAlwaysEncrypt = source.OWAAlwaysEncrypt;
			this.OWAAlwaysSign = source.OWAAlwaysSign;
			this.OWABCCEncryptedEmailForking = source.OWABCCEncryptedEmailForking;
			this.OWACRLConnectionTimeout = source.OWACRLConnectionTimeout;
			this.OWACRLRetrievalTimeout = source.OWACRLRetrievalTimeout;
			this.OWACheckCRLOnSend = source.OWACheckCRLOnSend;
			this.OWAClearSign = source.OWAClearSign;
			this.OWACopyRecipientHeaders = source.OWACopyRecipientHeaders;
			this.OWADLExpansionTimeout = source.OWADLExpansionTimeout;
			this.OWADisableCRLCheck = source.OWADisableCRLCheck;
			this.OWAEncryptTemporaryBuffers = source.OWAEncryptTemporaryBuffers;
			this.OWAEncryptionAlgorithms = source.OWAEncryptionAlgorithms;
			this.OWAForceSMIMEClientUpgrade = source.OWAForceSMIMEClientUpgrade;
			this.OWAIncludeCertificateChainAndRootCertificate = source.OWAIncludeCertificateChainAndRootCertificate;
			this.OWAIncludeCertificateChainWithoutRootCertificate = source.OWAIncludeCertificateChainWithoutRootCertificate;
			this.OWAIncludeSMIMECapabilitiesInMessage = source.OWAIncludeSMIMECapabilitiesInMessage;
			this.OWAOnlyUseSmartCard = source.OWAOnlyUseSmartCard;
			this.OWASenderCertificateAttributesToDisplay = source.OWASenderCertificateAttributesToDisplay;
			this.OWASignedEmailCertificateInclusion = source.OWASignedEmailCertificateInclusion;
			this.OWASigningAlgorithms = source.OWASigningAlgorithms;
			this.OWATripleWrapSignedEncryptedMail = source.OWATripleWrapSignedEncryptedMail;
			this.OWAUseKeyIdentifier = source.OWAUseKeyIdentifier;
			this.OWAUseSecondaryProxiesWhenFindingCertificates = source.OWAUseSecondaryProxiesWhenFindingCertificates;
			this.OWASMIMECertificateIssuingCAFull = source.SMIMECertificateIssuingCAFull();
		}

		[DataMember]
		public bool OWAAllowUserChoiceOfSigningCertificate { get; set; }

		[DataMember]
		public bool OWAAlwaysEncrypt { get; set; }

		[DataMember]
		public bool OWAAlwaysSign { get; set; }

		[DataMember]
		public uint OWABCCEncryptedEmailForking { get; set; }

		[DataMember]
		public uint OWACRLConnectionTimeout { get; set; }

		[DataMember]
		public uint OWACRLRetrievalTimeout { get; set; }

		[DataMember]
		public bool OWACheckCRLOnSend { get; set; }

		[DataMember]
		public bool OWAClearSign { get; set; }

		[DataMember]
		public bool OWACopyRecipientHeaders { get; set; }

		[DataMember]
		public uint OWADLExpansionTimeout { get; set; }

		[DataMember]
		public bool OWADisableCRLCheck { get; set; }

		[DataMember]
		public bool OWAEncryptTemporaryBuffers { get; set; }

		[DataMember]
		public string OWAEncryptionAlgorithms { get; set; }

		[DataMember]
		public bool OWAForceSMIMEClientUpgrade { get; set; }

		[DataMember]
		public bool OWAIncludeCertificateChainAndRootCertificate { get; set; }

		[DataMember]
		public bool OWAIncludeCertificateChainWithoutRootCertificate { get; set; }

		[DataMember]
		public bool OWAIncludeSMIMECapabilitiesInMessage { get; set; }

		[DataMember]
		public bool OWAOnlyUseSmartCard { get; set; }

		[DataMember]
		public string OWASenderCertificateAttributesToDisplay { get; set; }

		[DataMember]
		public bool OWASignedEmailCertificateInclusion { get; set; }

		[DataMember]
		public string OWASigningAlgorithms { get; set; }

		[DataMember]
		public bool OWATripleWrapSignedEncryptedMail { get; set; }

		[DataMember]
		public bool OWAUseKeyIdentifier { get; set; }

		[DataMember]
		public bool OWAUseSecondaryProxiesWhenFindingCertificates { get; set; }

		[DataMember]
		public string OWASMIMECertificateIssuingCAFull { get; set; }

		public string SMIMECertificateIssuingCAFull()
		{
			return this.OWASMIMECertificateIssuingCAFull;
		}

		public bool Equals(SmimeSettingsData other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (this.OWAAllowUserChoiceOfSigningCertificate.Equals(other.OWAAllowUserChoiceOfSigningCertificate) && this.OWAAlwaysEncrypt.Equals(other.OWAAlwaysEncrypt) && this.OWAAlwaysSign.Equals(other.OWAAlwaysSign) && this.OWABCCEncryptedEmailForking == other.OWABCCEncryptedEmailForking && this.OWACRLConnectionTimeout == other.OWACRLConnectionTimeout && this.OWACRLRetrievalTimeout == other.OWACRLRetrievalTimeout && this.OWACheckCRLOnSend.Equals(other.OWACheckCRLOnSend) && this.OWAClearSign.Equals(other.OWAClearSign) && this.OWACopyRecipientHeaders.Equals(other.OWACopyRecipientHeaders) && this.OWADLExpansionTimeout == other.OWADLExpansionTimeout && this.OWADisableCRLCheck.Equals(other.OWADisableCRLCheck) && this.OWAEncryptTemporaryBuffers.Equals(other.OWAEncryptTemporaryBuffers) && string.Equals(this.OWAEncryptionAlgorithms, other.OWAEncryptionAlgorithms, StringComparison.Ordinal) && this.OWAForceSMIMEClientUpgrade.Equals(other.OWAForceSMIMEClientUpgrade) && this.OWAIncludeCertificateChainAndRootCertificate.Equals(other.OWAIncludeCertificateChainAndRootCertificate) && this.OWAIncludeCertificateChainWithoutRootCertificate.Equals(other.OWAIncludeCertificateChainWithoutRootCertificate) && this.OWAIncludeSMIMECapabilitiesInMessage.Equals(other.OWAIncludeSMIMECapabilitiesInMessage) && this.OWAOnlyUseSmartCard.Equals(other.OWAOnlyUseSmartCard) && string.Equals(this.OWASenderCertificateAttributesToDisplay, other.OWASenderCertificateAttributesToDisplay, StringComparison.Ordinal) && this.OWASignedEmailCertificateInclusion.Equals(other.OWASignedEmailCertificateInclusion) && string.Equals(this.OWASigningAlgorithms, other.OWASigningAlgorithms, StringComparison.Ordinal) && this.OWATripleWrapSignedEncryptedMail.Equals(other.OWATripleWrapSignedEncryptedMail) && this.OWAUseKeyIdentifier.Equals(other.OWAUseKeyIdentifier) && this.OWAUseSecondaryProxiesWhenFindingCertificates.Equals(other.OWAUseSecondaryProxiesWhenFindingCertificates) && string.Equals(this.OWASMIMECertificateIssuingCAFull, other.OWASMIMECertificateIssuingCAFull, StringComparison.Ordinal)));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as SmimeSettingsData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			num = (num * 397 ^ this.OWAAllowUserChoiceOfSigningCertificate.GetHashCode());
			num = (num * 397 ^ this.OWAAlwaysEncrypt.GetHashCode());
			num = (num * 397 ^ this.OWAAlwaysSign.GetHashCode());
			num = (num * 397 ^ (int)this.OWABCCEncryptedEmailForking);
			num = (num * 397 ^ (int)this.OWACRLConnectionTimeout);
			num = (num * 397 ^ (int)this.OWACRLRetrievalTimeout);
			num = (num * 397 ^ this.OWACheckCRLOnSend.GetHashCode());
			num = (num * 397 ^ this.OWAClearSign.GetHashCode());
			num = (num * 397 ^ this.OWACopyRecipientHeaders.GetHashCode());
			num = (num * 397 ^ (int)this.OWADLExpansionTimeout);
			num = (num * 397 ^ this.OWADisableCRLCheck.GetHashCode());
			num = (num * 397 ^ this.OWAEncryptTemporaryBuffers.GetHashCode());
			num = (num * 397 ^ ((this.OWAEncryptionAlgorithms != null) ? this.OWAEncryptionAlgorithms.GetHashCode() : 0));
			num = (num * 397 ^ this.OWAForceSMIMEClientUpgrade.GetHashCode());
			num = (num * 397 ^ this.OWAIncludeCertificateChainAndRootCertificate.GetHashCode());
			num = (num * 397 ^ this.OWAIncludeCertificateChainWithoutRootCertificate.GetHashCode());
			num = (num * 397 ^ this.OWAIncludeSMIMECapabilitiesInMessage.GetHashCode());
			num = (num * 397 ^ this.OWAOnlyUseSmartCard.GetHashCode());
			num = (num * 397 ^ ((this.OWASenderCertificateAttributesToDisplay != null) ? this.OWASenderCertificateAttributesToDisplay.GetHashCode() : 0));
			num = (num * 397 ^ this.OWASignedEmailCertificateInclusion.GetHashCode());
			num = (num * 397 ^ ((this.OWASigningAlgorithms != null) ? this.OWASigningAlgorithms.GetHashCode() : 0));
			num = (num * 397 ^ this.OWATripleWrapSignedEncryptedMail.GetHashCode());
			num = (num * 397 ^ this.OWAUseKeyIdentifier.GetHashCode());
			num = (num * 397 ^ this.OWAUseSecondaryProxiesWhenFindingCertificates.GetHashCode());
			return num * 397 ^ ((this.OWASMIMECertificateIssuingCAFull != null) ? this.OWASMIMECertificateIssuingCAFull.GetHashCode() : 0);
		}
	}
}
