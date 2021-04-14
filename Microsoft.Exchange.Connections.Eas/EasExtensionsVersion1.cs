using System;

namespace Microsoft.Exchange.Connections.Eas
{
	[Flags]
	internal enum EasExtensionsVersion1
	{
		FolderTypes = 1,
		SystemCategories = 2,
		DefaultFromAddress = 4,
		Archive = 8,
		Unsubscribe = 16,
		MessageUpload = 32,
		AdvanedSearch = 64,
		PicwData = 128,
		TrueMessageRead = 256,
		Rules = 512,
		ExtendedDateFilters = 1024,
		SmsExtensions = 2048,
		ActionableSearch = 4096,
		FolderPermission = 8192,
		FolderExtensionType = 16384,
		VoiceMailExtension = 32768
	}
}
