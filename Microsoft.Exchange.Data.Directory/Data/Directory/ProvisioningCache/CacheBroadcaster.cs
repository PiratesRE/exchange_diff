using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class CacheBroadcaster
	{
		public static bool IsIPv6Only()
		{
			if (CacheBroadcaster.isIpv6Only == null)
			{
				try
				{
					IPAddress[] localIPAddresses = CacheBroadcaster.GetLocalIPAddresses();
					CacheBroadcaster.isIpv6Only = new bool?(!localIPAddresses.Any((IPAddress addr) => addr.AddressFamily == AddressFamily.InterNetwork));
				}
				catch (Exception)
				{
					CacheBroadcaster.isIpv6Only = new bool?(false);
				}
			}
			return CacheBroadcaster.isIpv6Only.Value;
		}

		public static IPAddress[] GetLocalIPAddresses()
		{
			List<IPAddress> list = new List<IPAddress>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in from a in allNetworkInterfaces
			where a.OperationalStatus == OperationalStatus.Up
			select a)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				UnicastIPAddressInformationCollection unicastAddresses = ipproperties.UnicastAddresses;
				foreach (IPAddressInformation ipaddressInformation in unicastAddresses.OrderBy((UnicastIPAddressInformation ua) => ua.Address.AddressFamily))
				{
					if (!IPAddress.IsLoopback(ipaddressInformation.Address) && !ipaddressInformation.IsTransient)
					{
						if (ipaddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && !ipproperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive)
						{
							list.Add(ipaddressInformation.Address);
						}
						else if (ipaddressInformation.Address.AddressFamily == AddressFamily.InterNetworkV6 && !ipaddressInformation.Address.IsIPv6LinkLocal)
						{
							list.Add(ipaddressInformation.Address);
						}
					}
				}
			}
			return list.ToArray();
		}

		public CacheBroadcaster(uint broadcastPort)
		{
			if (!CacheBroadcaster.IsIPv6Only())
			{
				this.msgSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this.msgSendEndPoint = new IPEndPoint(IPAddress.Broadcast, (int)broadcastPort);
				this.msgSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				return;
			}
			this.msgSendSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			this.msgSendEndPoint = new IPEndPoint(IPAddress.Parse("ff02::1"), (int)broadcastPort);
			this.msgSendSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(IPAddress.Parse("ff02::1")));
		}

		public void BroadcastInvalidationMessage(OrganizationId orgId, Guid[] keys)
		{
			Guid orgId2 = Guid.Empty;
			if (orgId != null && !orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				orgId2 = orgId.ConfigurationUnit.ObjectGuid;
				if (orgId2.Equals(Guid.Empty))
				{
					return;
				}
			}
			InvalidationMessage invalidationMessage = new InvalidationMessage(orgId2, keys);
			byte[] msg = invalidationMessage.ToSendMessage();
			lock (this.sendSocketLockObj)
			{
				this.SendMessage(this.msgSendSocket, msg, this.msgSendEndPoint);
			}
		}

		private void SendMessage(Socket socket, byte[] msg, IPEndPoint endPoint)
		{
			try
			{
				socket.SendTo(msg, endPoint);
			}
			catch (SocketException ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCInvalidationMessageFailedBroadcast, endPoint.Address.ToString(), new object[]
				{
					endPoint.Port,
					ex.Message
				});
			}
			catch (ObjectDisposedException ex2)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCInvalidationMessageFailedBroadcast, endPoint.Address.ToString(), new object[]
				{
					endPoint.Port,
					ex2.Message
				});
			}
		}

		public const string Ipv6LinkLocalBroadcastAddr = "ff02::1";

		private static bool? isIpv6Only = null;

		private Socket msgSendSocket;

		private object sendSocketLockObj = new object();

		private IPEndPoint msgSendEndPoint;
	}
}
