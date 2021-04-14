using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum IRMFlags
	{
		Empty = 0,
		EncryptionEnabled = 1,
		PrelicensingEnabled = 2,
		JournalReportDecryptionEnabled = 4,
		UseSharedRMS = 8,
		ExternalLicensingEnabled = 16,
		SearchEnabled = 32,
		ClientAccessServerEnabled = 64,
		TransportDecryptionOptional = 128,
		TransportDecryptionMandatory = 256,
		InternalLicensingEnabled = 512,
		InternetConfidentialEnabled = 1024,
		EDiscoverySuperUserDisabled = 2048,
		All = 1012,
		LastFlag = 8388608
	}
}
