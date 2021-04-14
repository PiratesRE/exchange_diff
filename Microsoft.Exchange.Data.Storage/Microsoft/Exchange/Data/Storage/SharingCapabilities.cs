using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SharingCapabilities
	{
		None = 0,
		UrlConfiguration = 1,
		FileAssociation = 2,
		AssociatedStore = 4,
		RestrictedStorage = 8,
		ReadSharing = 16,
		WriteSharing = 32,
		PublishSharing = 64,
		ItemPrivacy = 128,
		ScopeItems = 256,
		ScopeSingleFolder = 512,
		ScopeMultipleFolder = 1024,
		ScopeHierarchy = 2048,
		NoRoamBinding = 32768,
		FreeBinding = 131072,
		AccessControl = 262144,
		SubfolderBinding = 1048576
	}
}
