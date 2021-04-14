using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDataImport : IDisposable
	{
		IDataMessage SendMessageAndWaitForReply(IDataMessage message);

		void SendMessage(IDataMessage message);
	}
}
