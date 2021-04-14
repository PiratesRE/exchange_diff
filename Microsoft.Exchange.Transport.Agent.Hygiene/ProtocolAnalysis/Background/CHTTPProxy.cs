using System;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class CHTTPProxy : TransportConnection, IDataConnection
	{
		public CHTTPProxy(bool fPost, ProxyChain proxyChain) : base(proxyChain)
		{
			this.fPost = fPost;
			this.connectRequest = new StringBuilder();
		}

		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			this.tcpCxn = tcpCxn;
			if (this.fPost)
			{
				this.connectRequest.AppendFormat("POST http://{0}:{1}/ HTTP/1.0\\r\\nContent-Type: text/plain\\r\\nContent-Length: 6\\r\\n\\r\\nRSET\\r\\n", remoteEndpoint.Address.ToString(), remoteEndpoint.Port);
			}
			else
			{
				this.connectRequest.AppendFormat("CONNECT {0}:{1} HTTP/1.0\\r\\n\\r\\n", remoteEndpoint.Address.ToString(), remoteEndpoint.Port);
			}
			try
			{
				this.tcpCxn.SendString(this.connectRequest.ToString());
				this.state = CHTTPProxy.HTTPProxyState.RequestSent;
			}
			catch (AtsException)
			{
				base.ProxyChain.OnDisconnected();
			}
		}

		public int OnDataReceived(byte[] dataReceived, int offset, int length)
		{
			int num = 0;
			while (length - num > 0)
			{
				CHTTPProxy.HTTPProxyState httpproxyState = this.state;
				if (httpproxyState != CHTTPProxy.HTTPProxyState.RequestSent)
				{
					return num;
				}
				string @string = Encoding.ASCII.GetString(dataReceived, offset + num, length - num);
				int num2 = @string.IndexOf("\r\n\r\n", StringComparison.OrdinalIgnoreCase);
				if (num2 == -1)
				{
					return num;
				}
				bool flag = false;
				this.state = CHTTPProxy.HTTPProxyState.Finished;
				string[] array = @string.Split(CHTTPProxy.delimiter, 3);
				if (array.Length > 1)
				{
					try
					{
						int num3 = int.Parse(array[1], null);
						if (num3 >= 200 && num3 < 300)
						{
							flag = true;
						}
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
				}
				num += num2 + 4;
				if (flag)
				{
					num += base.ProxyChain.OnConnected(dataReceived, offset + num, length - num);
				}
				else
				{
					base.ProxyChain.OnDisconnected();
				}
			}
			return num;
		}

		private static char[] delimiter = new char[]
		{
			' ',
			'\t'
		};

		private StringBuilder connectRequest;

		private bool fPost;

		private TcpConnection tcpCxn;

		private CHTTPProxy.HTTPProxyState state;

		private enum HTTPProxyState
		{
			Invalid,
			RequestSent,
			Finished
		}
	}
}
