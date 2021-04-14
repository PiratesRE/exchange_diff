using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Net
{
	internal interface INetworkConnection : IDisposable
	{
		long ConnectionId { get; }

		int ReceiveTimeout { get; set; }

		int SendTimeout { get; set; }

		int Timeout { set; }

		IPEndPoint LocalEndPoint { get; }

		IPEndPoint RemoteEndPoint { get; }

		int MaxLineLength { get; set; }

		long BytesReceived { get; }

		long BytesSent { get; }

		bool IsLineAvailable { get; }

		bool IsTls { get; }

		byte[] TlsEapKey { get; }

		int TlsCipherKeySize { get; }

		ConnectionInfo TlsConnectionInfo { get; }

		IX509Certificate2 RemoteCertificate { get; }

		IX509Certificate2 LocalCertificate { get; }

		ChannelBindingToken ChannelBindingToken { get; }

		SchannelProtocols ServerTlsProtocols { get; set; }

		SchannelProtocols ClientTlsProtocols { get; set; }

		void Shutdown();

		void Shutdown(int waitSeconds);

		IAsyncResult BeginRead(AsyncCallback callback, object state);

		void EndRead(IAsyncResult asyncResult, out byte[] buffer, out int offset, out int size, out object errorCode);

		Task<NetworkConnection.LazyAsyncResultWithTimeout> ReadAsync();

		void PutBackReceivedBytes(int bytesUnconsumed);

		IAsyncResult BeginReadLine(AsyncCallback callback, object state);

		void EndReadLine(IAsyncResult asyncResult, out byte[] buffer, out int offset, out int size, out object errorCode);

		Task<NetworkConnection.LazyAsyncResultWithTimeout> ReadLineAsync();

		IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

		IAsyncResult BeginWrite(Stream stream, AsyncCallback callback, object state);

		void EndWrite(IAsyncResult asyncResult, out object errorCode);

		Task<object> WriteAsync(byte[] buffer, int offset, int size);

		IAsyncResult BeginNegotiateTlsAsClient(X509Certificate certificate, string targetName, AsyncCallback callback, object state);

		void EndNegotiateTlsAsClient(IAsyncResult asyncResult, out object errorCode);

		Task<object> NegotiateTlsAsClientAsync(IX509Certificate2 certificate, string targetName);

		IAsyncResult BeginNegotiateTlsAsServer(X509Certificate2 cert, bool requestClientCertificate, AsyncCallback callback, object state);

		void EndNegotiateTlsAsServer(IAsyncResult asyncResult, out object errorCode);

		Task<object> NegotiateTlsAsServerAsync(IX509Certificate2 certificate, bool requestClientCertificate);
	}
}
