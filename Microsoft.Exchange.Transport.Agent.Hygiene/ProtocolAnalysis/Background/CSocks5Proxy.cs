using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class CSocks5Proxy : TransportConnection, IDataConnection
	{
		private static CSocks5Proxy.Socks5ResponseMethod ConvertSocks5ResponseCodeToMethod(byte response)
		{
			CSocks5Proxy.Socks5ResponseMethod result = CSocks5Proxy.Socks5ResponseMethod.NoAcceptableMethod;
			switch (response)
			{
			case 0:
				result = CSocks5Proxy.Socks5ResponseMethod.NoAuthenticationRequired;
				break;
			case 1:
				result = CSocks5Proxy.Socks5ResponseMethod.GSSAPIAuthentication;
				break;
			case 2:
				result = CSocks5Proxy.Socks5ResponseMethod.UserNamePasswordAuth;
				break;
			case 3:
				result = CSocks5Proxy.Socks5ResponseMethod.IANAAssigned;
				break;
			case 4:
				result = CSocks5Proxy.Socks5ResponseMethod.PrivateMethod;
				break;
			}
			return result;
		}

		public CSocks5Proxy(ProxyChain proxyChain) : base(proxyChain)
		{
		}

		private bool UpdateUsernamePassword(string username, string password)
		{
			int num = 0;
			if (username.Length >= 255)
			{
				return false;
			}
			if (password.Length >= 255)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				this.usernamePasswordSize = 3 + username.Length + password.Length;
				this.usernamePasswordRequest = new byte[this.usernamePasswordSize];
				this.usernamePasswordRequest[num++] = 1;
				this.usernamePasswordRequest[num++] = (byte)(username.Length & 255);
				for (int i = 0; i < username.Length; i++)
				{
					this.usernamePasswordRequest[num++] = (byte)username[i];
				}
				this.usernamePasswordRequest[num++] = (byte)(password.Length & 255);
				for (int i = 0; i < password.Length; i++)
				{
					this.usernamePasswordRequest[num++] = (byte)password[i];
				}
			}
			else
			{
				this.usernamePasswordSize = 0;
				this.usernamePasswordRequest = null;
			}
			return true;
		}

		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			if (!this.UpdateUsernamePassword(authInfo.UserName, authInfo.Password))
			{
				base.ProxyChain.OnDisconnected();
				return;
			}
			this.tcpCxn = tcpCxn;
			this.state = CSocks5Proxy.Socks5ProxyState.NegotiationSent;
			try
			{
				if (this.usernamePasswordRequest == null)
				{
					this.versionMethodRequestSize--;
				}
				this.versionMethodRequest = new byte[this.versionMethodRequestSize];
				this.versionMethodRequest[0] = 5;
				if (this.usernamePasswordRequest == null)
				{
					this.versionMethodRequest[1] = 1;
					this.versionMethodRequest[2] = 0;
				}
				else
				{
					this.versionMethodRequest[1] = 2;
					this.versionMethodRequest[2] = 0;
					this.versionMethodRequest[3] = 3;
				}
				this.connectRequest = new byte[24];
				this.connectRequest[0] = 5;
				this.connectRequest[1] = 1;
				this.connectRequest[2] = 0;
				this.connectRequest[3] = 1;
				int num = (remoteEndpoint.Address.AddressFamily == AddressFamily.InterNetworkV6) ? 16 : 4;
				byte[] addressBytes = remoteEndpoint.Address.GetAddressBytes();
				Array.Copy(addressBytes, 0, this.connectRequest, 4, num);
				this.connectRequest[4 + num] = (byte)(remoteEndpoint.Port >> 8 & 255);
				this.connectRequest[5 + num] = (byte)(remoteEndpoint.Port & 255);
				this.tcpCxn.SendMessage(this.versionMethodRequest, 0, this.versionMethodRequestSize);
			}
			catch (AtsException)
			{
				this.state = CSocks5Proxy.Socks5ProxyState.Finished;
				base.ProxyChain.OnDisconnected();
			}
		}

		public int OnDataReceived(byte[] dataReceived, int offset, int length)
		{
			int num = 0;
			try
			{
				while (length - num > 0)
				{
					switch (this.state)
					{
					case CSocks5Proxy.Socks5ProxyState.NegotiationSent:
						if (length - num < 2)
						{
							return num;
						}
						switch (CSocks5Proxy.ConvertSocks5ResponseCodeToMethod(dataReceived[offset + num + 1]))
						{
						case CSocks5Proxy.Socks5ResponseMethod.NoAuthenticationRequired:
							this.state = CSocks5Proxy.Socks5ProxyState.ConnectSent;
							this.tcpCxn.SendMessage(this.connectRequest, 0, 24);
							break;
						case CSocks5Proxy.Socks5ResponseMethod.GSSAPIAuthentication:
							goto IL_A2;
						case CSocks5Proxy.Socks5ResponseMethod.UserNamePasswordAuth:
							if (this.usernamePasswordRequest == null)
							{
								goto IL_A2;
							}
							this.state = CSocks5Proxy.Socks5ProxyState.UserPwdSent;
							this.tcpCxn.SendMessage(this.usernamePasswordRequest, 0, this.usernamePasswordSize);
							break;
						default:
							goto IL_A2;
						}
						IL_B4:
						num += 2;
						break;
						IL_A2:
						this.state = CSocks5Proxy.Socks5ProxyState.Finished;
						base.ProxyChain.OnDisconnected();
						goto IL_B4;
					case CSocks5Proxy.Socks5ProxyState.UserPwdSent:
						if (length - num < 2)
						{
							return num;
						}
						num += 2;
						if (dataReceived[offset + num + 1] == 0)
						{
							this.state = CSocks5Proxy.Socks5ProxyState.ConnectSent;
							this.tcpCxn.SendMessage(this.connectRequest, 0, 24);
						}
						else
						{
							this.state = CSocks5Proxy.Socks5ProxyState.Finished;
							base.ProxyChain.OnDisconnected();
						}
						break;
					case CSocks5Proxy.Socks5ProxyState.ConnectSent:
					{
						if (length - num < 6)
						{
							return num;
						}
						byte b = dataReceived[offset + num + 1];
						if (b != 0)
						{
							base.ProxyChain.OnDisconnected();
							return num;
						}
						switch (dataReceived[offset + num + 3])
						{
						case 1:
							this.connectResponseSize = 10;
							break;
						case 2:
							goto IL_134;
						case 3:
							this.connectResponseSize = (int)(7 + dataReceived[offset + num + 4]);
							break;
						case 4:
							this.connectResponseSize = 22;
							break;
						default:
							goto IL_134;
						}
						if (length - num < this.connectResponseSize)
						{
							return num;
						}
						num += this.connectResponseSize;
						this.state = CSocks5Proxy.Socks5ProxyState.Finished;
						num += base.ProxyChain.OnConnected(dataReceived, offset + num, length - num);
						break;
						IL_134:
						base.ProxyChain.OnDisconnected();
						return num;
					}
					default:
						return num;
					}
				}
			}
			catch (AtsException)
			{
				this.state = CSocks5Proxy.Socks5ProxyState.Finished;
				base.ProxyChain.OnDisconnected();
			}
			return num;
		}

		private const int MaxUsernameLength = 255;

		private const int MaxPasswordLength = 255;

		private const int IPv4AddressOctets = 4;

		private const int IPv6AddressOctets = 16;

		private const byte Socks5RequestGrantCode = 0;

		private const byte Socks5UserPwdAuthSuccessCode = 0;

		private const byte IPv4AddressType = 1;

		private const byte HostnameAddressType = 3;

		private const byte IPv6AddressType = 4;

		private const int VersionMethodResponseSize = 2;

		private const int UsernamePasswordResponseSize = 2;

		private const int ConnectRequestSize = 24;

		private const int MinconnectResponseSize = 6;

		private int versionMethodRequestSize = 4;

		private int connectResponseSize;

		private byte[] versionMethodRequest;

		private byte[] connectRequest;

		private TcpConnection tcpCxn;

		private CSocks5Proxy.Socks5ProxyState state;

		private byte[] usernamePasswordRequest;

		private int usernamePasswordSize;

		private enum Socks5ProxyState
		{
			Invalid,
			NegotiationSent,
			UserPwdSent,
			ConnectSent,
			Finished
		}

		private enum Socks5ResponseMethod : byte
		{
			NoAuthenticationRequired,
			GSSAPIAuthentication,
			UserNamePasswordAuth,
			IANAAssigned,
			PrivateMethod,
			NoAcceptableMethod
		}
	}
}
