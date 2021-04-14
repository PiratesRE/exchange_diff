using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IActionWatermark : IComparable
	{
		string SerializeToString();
	}
}
