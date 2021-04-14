using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum PermissionGroups
	{
		[LocDescription(DataStrings.IDs.PermissionGroupsNone)]
		None = 0,
		[LocDescription(DataStrings.IDs.AnonymousUsers)]
		AnonymousUsers = 1,
		[LocDescription(DataStrings.IDs.ExchangeUsers)]
		ExchangeUsers = 2,
		[LocDescription(DataStrings.IDs.ExchangeServers)]
		ExchangeServers = 4,
		[LocDescription(DataStrings.IDs.ExchangeLegacyServers)]
		ExchangeLegacyServers = 8,
		[LocDescription(DataStrings.IDs.Partners)]
		Partners = 16,
		[LocDescription(DataStrings.IDs.PermissionGroupsCustom)]
		Custom = 32
	}
}
