using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Messages;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class CheckExchangeRpcServiceResponsive : BaseObject
	{
		public CheckExchangeRpcServiceResponsive(ExEventLog eventLog)
		{
			this.eventLog = eventLog;
			this.wasExchangeRpcServiceResponsive = true;
			this.localIpAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			this.ipAddressFamily = this.GetAddressFamily();
			this.periodicCheckIfExchangeRpcServiceIsResponsiveTimer = new MaintenanceJobTimer(new Action(this.CheckIfExchangeRpcServiceIsResponsive), () => Configuration.ServiceConfiguration.WaitBetweenTcpConnectToFindIfRpcServiceResponsive != TimeSpan.Zero, Configuration.ServiceConfiguration.WaitBetweenTcpConnectToFindIfRpcServiceResponsive, TimeSpan.Zero);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CheckExchangeRpcServiceResponsive>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.periodicCheckIfExchangeRpcServiceIsResponsiveTimer);
			base.InternalDispose();
		}

		private void CheckIfExchangeRpcServiceIsResponsive()
		{
			bool flag = this.TcpConnect(this.localIpAddresses, 6001, this.ipAddressFamily);
			if (!flag && !this.wasExchangeRpcServiceResponsive)
			{
				ExTraceGlobals.ConnectRpcTracer.TraceDebug(Activity.TraceId, "Restart ExchangeRpcService process because a possible deadlock is detected");
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_RpcClientAccessServiceDeadlocked, string.Empty, new object[]
				{
					string.Empty
				});
				Environment.Exit(-559034354);
			}
			this.wasExchangeRpcServiceResponsive = flag;
		}

		private bool TcpConnect(IPAddress[] localIpAddresses, int port, AddressFamily addressFamily)
		{
			TcpClient tcpClient = new TcpClient(addressFamily);
			bool result;
			try
			{
				tcpClient.Connect(localIpAddresses, port);
				result = true;
			}
			catch (SocketException)
			{
				result = false;
			}
			catch (ObjectDisposedException)
			{
				result = false;
			}
			finally
			{
				tcpClient.Close();
			}
			return result;
		}

		private AddressFamily GetAddressFamily()
		{
			bool flag = false;
			foreach (IPAddress ipaddress in this.localIpAddresses)
			{
				if (AddressFamily.InterNetworkV6 == ipaddress.AddressFamily)
				{
					flag = true;
					break;
				}
				AddressFamily addressFamily = ipaddress.AddressFamily;
			}
			if (!flag)
			{
				return AddressFamily.InterNetwork;
			}
			return AddressFamily.InterNetworkV6;
		}

		private readonly ExEventLog eventLog;

		private readonly MaintenanceJobTimer periodicCheckIfExchangeRpcServiceIsResponsiveTimer;

		private readonly IPAddress[] localIpAddresses;

		private readonly AddressFamily ipAddressFamily;

		private bool wasExchangeRpcServiceResponsive;
	}
}
