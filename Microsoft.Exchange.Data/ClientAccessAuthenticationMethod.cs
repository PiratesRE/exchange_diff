using System;

namespace Microsoft.Exchange.Data
{
	public enum ClientAccessAuthenticationMethod
	{
		[LocDescription(DataStrings.IDs.ClientAccessBasicAuthentication)]
		BasicAuthentication,
		[LocDescription(DataStrings.IDs.ClientAccessNonBasicAuthentication)]
		NonBasicAuthentication,
		[LocDescription(DataStrings.IDs.ClientAccessAdfsAuthentication)]
		AdfsAuthentication
	}
}
