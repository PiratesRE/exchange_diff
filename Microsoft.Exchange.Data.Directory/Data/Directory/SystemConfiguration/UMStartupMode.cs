using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UMStartupMode
	{
		[LocDescription(DirectoryStrings.IDs.TCP)]
		TCP,
		[LocDescription(DirectoryStrings.IDs.TLS)]
		TLS,
		[LocDescription(DirectoryStrings.IDs.Dual)]
		Dual
	}
}
