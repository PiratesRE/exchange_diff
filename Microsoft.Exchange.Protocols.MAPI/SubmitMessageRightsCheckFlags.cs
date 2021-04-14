using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum SubmitMessageRightsCheckFlags
	{
		None = 0,
		SendAsRights = 1,
		SOBORights = 2,
		SendingAsDL = 4
	}
}
