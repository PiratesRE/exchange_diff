using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public interface ISmimeSettingsProvider
	{
		bool OWACheckCRLOnSend { get; }

		uint OWADLExpansionTimeout { get; }

		bool OWAUseSecondaryProxiesWhenFindingCertificates { get; }

		uint OWACRLConnectionTimeout { get; }

		uint OWACRLRetrievalTimeout { get; }

		bool OWADisableCRLCheck { get; }

		bool OWAAlwaysSign { get; }

		bool OWAAlwaysEncrypt { get; }

		bool OWAClearSign { get; }

		bool OWAIncludeCertificateChainWithoutRootCertificate { get; }

		bool OWAIncludeCertificateChainAndRootCertificate { get; }

		bool OWAEncryptTemporaryBuffers { get; }

		bool OWASignedEmailCertificateInclusion { get; }

		uint OWABCCEncryptedEmailForking { get; }

		bool OWAIncludeSMIMECapabilitiesInMessage { get; }

		bool OWACopyRecipientHeaders { get; }

		bool OWAOnlyUseSmartCard { get; }

		bool OWATripleWrapSignedEncryptedMail { get; }

		bool OWAUseKeyIdentifier { get; }

		string OWAEncryptionAlgorithms { get; }

		string OWASigningAlgorithms { get; }

		bool OWAForceSMIMEClientUpgrade { get; }

		string OWASenderCertificateAttributesToDisplay { get; }

		bool OWAAllowUserChoiceOfSigningCertificate { get; }

		string SMIMECertificateIssuingCAFull();
	}
}
