using System;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Agent.Hygiene;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal sealed class ProxyChain : IDataConnection
	{
		public ProxyChain(ProxyEndPoint[] remote, ProxyTest proxyTest, string greetingBanner)
		{
			this.proxyTest = proxyTest;
			this.proxyEndpoints = remote;
			this.currentPos = 0;
			this.matchingBanner = greetingBanner;
			this.detectionTimeout = new Timer(new TimerCallback(ProxyChain.OpenProxyDetectionTimeout), this, -1, -1);
		}

		public IPEndPoint RemoteEndpoint
		{
			get
			{
				return this.proxyEndpoints[this.proxyEndpoints.Length - 2].Endpoint;
			}
		}

		public int OnDataReceived(byte[] buffer, int offset, int size)
		{
			if (size < this.matchingBanner.Length)
			{
				return 0;
			}
			OPDetectionResult result = OPDetectionResult.NotOpenProxy;
			string @string = Encoding.ASCII.GetString(buffer, offset, size);
			if (@string.StartsWith(this.matchingBanner, StringComparison.OrdinalIgnoreCase))
			{
				result = OPDetectionResult.IsOpenProxy;
			}
			this.DetectionComplete(result);
			return size;
		}

		public int OnConnected(byte[] dataReceived, int offset, int length)
		{
			if (this.currentPos < this.proxyEndpoints.Length)
			{
				int num = this.currentPos;
				TcpConnection tcpConnection = null;
				lock (this)
				{
					this.currentPos++;
					this.dataCxn = this.CreateDataCxn(this.proxyEndpoints[num].Type);
					if (this.tcpCxn != null)
					{
						this.tcpCxn.DataCxn = this.dataCxn;
					}
					tcpConnection = this.tcpCxn;
				}
				if (tcpConnection != null)
				{
					((TransportConnection)this.dataCxn).AsyncConnect(this.proxyEndpoints[num].Endpoint, this.tcpCxn, this.proxyEndpoints[num].AuthInfo);
				}
			}
			else
			{
				lock (this)
				{
					if (this.tcpCxn != null)
					{
						this.tcpCxn.DataCxn = this;
					}
				}
				if (dataReceived != null)
				{
					return this.OnDataReceived(dataReceived, offset, length);
				}
			}
			return 0;
		}

		public void OnDisconnected()
		{
			this.DetectionComplete(OPDetectionResult.NotOpenProxy);
		}

		public void DetectOpenProxy(int timeoutPeriod)
		{
			if (this.proxyEndpoints[0].Type != ProxyType.None)
			{
				throw new LocalizedException(AgentStrings.InvalidProxyChain);
			}
			this.detectionTimeout.Change(timeoutPeriod, -1);
			this.tcpCxn = new TcpConnection(this);
			this.currentPos++;
			this.tcpCxn.DataCxn = this;
			this.tcpCxn.AsyncConnect(this.proxyEndpoints[0].Endpoint, null, this.proxyEndpoints[0].AuthInfo);
		}

		private static void OpenProxyDetectionTimeout(object state)
		{
			ProxyChain proxyChain = (ProxyChain)state;
			OPDetectionResult result = OPDetectionResult.Unknown;
			lock (proxyChain)
			{
				if (proxyChain.tcpCxn != null && proxyChain.tcpCxn.RemoteReachable)
				{
					result = OPDetectionResult.NotOpenProxy;
				}
			}
			proxyChain.DetectionComplete(result);
		}

		private IDataConnection CreateDataCxn(ProxyType type)
		{
			switch (type)
			{
			case ProxyType.Socks4:
				return new CSocks4Proxy(this);
			case ProxyType.Socks5:
				return new CSocks5Proxy(this);
			case ProxyType.HttpConnect:
				return new CHTTPProxy(false, this);
			case ProxyType.HttpPost:
				return new CHTTPProxy(true, this);
			case ProxyType.Telnet:
				return new CTelnetProxy(false, this);
			case ProxyType.Cisco:
				return new CTelnetProxy(true, this);
			case ProxyType.Wingate:
				return new CWinGateProxy(this);
			default:
				throw new LocalizedException(AgentStrings.InvalidOpenProxyType);
			}
		}

		private void DetectionComplete(OPDetectionResult result)
		{
			bool flag = false;
			lock (this)
			{
				if (this.detectionTimeout != null)
				{
					try
					{
						this.detectionTimeout.Change(-1, -1);
						this.tcpCxn.Shutdown(false);
					}
					finally
					{
						this.detectionTimeout.Dispose();
						this.detectionTimeout = null;
						this.tcpCxn.Dispose();
						this.tcpCxn = null;
					}
					flag = true;
				}
			}
			if (flag)
			{
				this.proxyTest.DetectionChainResult(result, this.proxyEndpoints[this.proxyEndpoints.Length - 1].Type, this.proxyEndpoints[this.proxyEndpoints.Length - 2].Endpoint.Port);
			}
		}

		private TcpConnection tcpCxn;

		private IDataConnection dataCxn;

		private ProxyTest proxyTest;

		private string matchingBanner;

		private Timer detectionTimeout;

		private ProxyEndPoint[] proxyEndpoints;

		private int currentPos;
	}
}
