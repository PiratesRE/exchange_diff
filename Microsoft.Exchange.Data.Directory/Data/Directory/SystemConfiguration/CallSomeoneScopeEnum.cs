using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum CallSomeoneScopeEnum
	{
		[LocDescription(DirectoryStrings.IDs.DialPlan)]
		DialPlan,
		[LocDescription(DirectoryStrings.IDs.GlobalAddressList)]
		GlobalAddressList,
		[LocDescription(DirectoryStrings.IDs.Extension)]
		Extension,
		[LocDescription(DirectoryStrings.IDs.AutoAttendantLink)]
		AutoAttendantLink,
		[LocDescription(DirectoryStrings.IDs.AddressList)]
		AddressList
	}
}
