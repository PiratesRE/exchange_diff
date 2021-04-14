using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal interface IDataConnection
	{
		int OnDataReceived(byte[] buffer, int offset, int size);
	}
}
