using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	internal interface IWcfClient
	{
		bool IsValid { get; }

		void Disconnect();
	}
}
