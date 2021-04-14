using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum Feature : ulong
	{
		GlobalAddressList = 1UL,
		Calendar = 2UL,
		Contacts = 4UL,
		Tasks = 8UL,
		Journal = 16UL,
		StickyNotes = 32UL,
		PublicFolders = 64UL,
		Organization = 128UL,
		Notifications = 256UL,
		RichClient = 512UL,
		SpellChecker = 1024UL,
		SMime = 2048UL,
		SearchFolders = 4096UL,
		Signature = 8192UL,
		Rules = 16384UL,
		Themes = 32768UL,
		JunkEMail = 65536UL,
		UMIntegration = 131072UL,
		WssIntegrationFromPublicComputer = 262144UL,
		WssIntegrationFromPrivateComputer = 524288UL,
		UncIntegrationFromPublicComputer = 1048576UL,
		UncIntegrationFromPrivateComputer = 2097152UL,
		EasMobileOptions = 4194304UL,
		ExplicitLogon = 8388608UL,
		AddressLists = 16777216UL,
		Dumpster = 33554432UL,
		ChangePassword = 67108864UL,
		InstantMessage = 134217728UL,
		TextMessage = 268435456UL,
		OWALight = 536870912UL,
		DelegateAccess = 1073741824UL,
		Irm = 2147483648UL,
		ForceSaveAttachmentFiltering = 4294967296UL,
		Silverlight = 8589934592UL,
		DisplayPhotos = 2199023255552UL,
		SetPhoto = 4398046511104UL,
		All = 18446744073709551615UL
	}
}
