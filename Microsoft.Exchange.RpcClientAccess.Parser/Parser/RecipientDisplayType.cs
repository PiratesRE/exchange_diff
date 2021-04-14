using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum RecipientDisplayType : byte
	{
		MailUser,
		DistributionList,
		Forum,
		Agent,
		Organization,
		PrivateDistributionList,
		RemoteMailUser
	}
}
