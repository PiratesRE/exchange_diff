using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DialScopeEnum
	{
		[LocDescription(DirectoryStrings.IDs.DialPlan)]
		DialPlan,
		[LocDescription(DirectoryStrings.IDs.GlobalAddressList)]
		GlobalAddressList,
		[LocDescription(DirectoryStrings.IDs.AddressList)]
		AddressList
	}
}
