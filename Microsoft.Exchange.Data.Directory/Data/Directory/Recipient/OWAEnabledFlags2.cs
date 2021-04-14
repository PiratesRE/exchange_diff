using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum OWAEnabledFlags2
	{
		ForceSaveAttachmentFilteringEnabledMask = 1,
		SilverlightEnabledMask,
		AnonymousFeaturesEnabledMask = 4,
		DeprecatedMask1 = 8,
		DeprecatedMask2 = 16,
		OWAMiniEnabledMask = 32,
		PlacesEnabledMask = 64,
		AllowOfflineOnMask = 384,
		DisplayPhotosEnabledMask = 512,
		SetPhotoEnabledMask = 1024,
		LogonPageLightSelectionEnabledMask = 2048,
		LogonPagePublicPrivateSelectionEnabledMask = 4096,
		PredictedActionsEnabledMask = 8192,
		IntegratedFeaturesEnabledMask = 16384,
		UserDiagnosticEnabledMask = 32768,
		FacebookEnabledMask = 65536,
		LinkedInEnabledMask = 131072,
		WacExternalServicesEnabledMask = 262144,
		WacOMEXEnabledMask = 524288,
		WebPartsOriginTypeMask = 1048576,
		WebPartsEnableOriginMask = 2097152,
		AllowCopyContactsToDeviceAddressBookMask = 4194304,
		ReportJunkEmailEnabledMask = 8388608,
		GroupCreationEnabledMask = 16777216,
		SkipCreateUnifiedGroupCustomSharepointClassificationMask = 33554432,
		WeatherEnabledMask = 67108864
	}
}
