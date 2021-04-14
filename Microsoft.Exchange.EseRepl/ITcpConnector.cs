using System;

namespace Microsoft.Exchange.EseRepl
{
	internal interface ITcpConnector
	{
		TcpClientChannel TryConnect(NetworkPath netPath, out NetworkTransportException failureEx);

		TcpClientChannel TryConnect(NetworkPath netPath, int timeoutInMsec, out NetworkTransportException failureEx);

		TcpClientChannel OpenChannel(string targetServerName, ISimpleBufferPool socketStreamBufferPool, IPool<SocketStreamAsyncArgs> socketStreamAsyncArgPool, SocketStream.ISocketStreamPerfCounters perfCtrs, out NetworkPath netPath);

		NetworkPath BuildDnsNetworkPath(string targetServer, int replicationPort);

		NetworkPath ChooseDagNetworkPath(string targetName, string networkName, NetworkPath.ConnectionPurpose purpose);

		bool ForceSocketStream { get; set; }
	}
}
