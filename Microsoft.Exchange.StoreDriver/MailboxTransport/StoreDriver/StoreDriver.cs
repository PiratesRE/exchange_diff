using System;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal sealed class StoreDriver : IStoreDriver, IDiagnosable
	{
		public static IPHostEntry LocalIP
		{
			get
			{
				return StoreDriver.localIp;
			}
		}

		public static IPAddress LocalIPAddress
		{
			get
			{
				return StoreDriver.localIp.AddressList[0];
			}
		}

		public static string ReceivedHeaderTcpInfo
		{
			get
			{
				return StoreDriver.receivedHeaderTcpInfo;
			}
		}

		public static IStoreDriver CreateStoreDriver()
		{
			StoreDriver.InitializeIPInfo();
			return null;
		}

		public static string FormatIPAddress(IPAddress address)
		{
			return "[" + address.ToString() + "]";
		}

		public void Retire()
		{
		}

		public void Start(bool initiallyPaused)
		{
		}

		public void Stop()
		{
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		public void DoLocalDelivery(NextHopConnection connection)
		{
		}

		public void ExpireOldSubmissionConnections()
		{
		}

		public string CurrentState
		{
			get
			{
				return string.Empty;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return string.Empty;
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return null;
		}

		private static void InitializeIPInfo()
		{
			lock (StoreDriver.syncObject)
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					try
					{
						StoreDriver.localIp = Dns.GetHostEntry(Dns.GetHostName());
					}
					catch (SocketException ex)
					{
						throw new TransportComponentLoadFailedException(ex.Message, ex);
					}
					StoreDriver.receivedHeaderTcpInfo = StoreDriver.FormatIPAddress(StoreDriver.localIp.AddressList[0]);
				}, 1);
			}
		}

		private static IPHostEntry localIp;

		private static string receivedHeaderTcpInfo;

		private static StoreDriver instance = new StoreDriver();

		private static object syncObject = new object();
	}
}
