using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface ILogTransactionInformation
	{
		byte Type();

		int Serialize(byte[] buffer, int offset);

		void Parse(byte[] buffer, ref int offset);
	}
}
