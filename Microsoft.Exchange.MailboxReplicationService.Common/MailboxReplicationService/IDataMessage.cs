using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDataMessage
	{
		int GetSize();

		void Serialize(bool useCompression, out DataMessageOpcode opcode, out byte[] data);
	}
}
