using System;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	public struct MDBEVENTWM
	{
		public Guid MailboxGuid;

		public Guid ConsumerGuid;

		public long EventCounter;
	}
}
