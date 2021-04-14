using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal class InvalidationRecvActivity : Activity
	{
		public InvalidationRecvActivity(ProvisioningCache cache, uint recvPort) : base(cache)
		{
			if (!CacheBroadcaster.IsIPv6Only())
			{
				this.msgReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this.msgReceiveEndPoint = new IPEndPoint(IPAddress.Any, (int)recvPort);
			}
			else
			{
				this.msgReceiveSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				this.msgReceiveEndPoint = new IPEndPoint(IPAddress.IPv6Any, (int)recvPort);
				this.msgReceiveSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(IPAddress.Parse("ff02::1")));
			}
			this.msgReceiveSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
			this.msgReceiveSocket.Bind(this.msgReceiveEndPoint);
			this.recvBuffer = new byte[5000];
		}

		public override string Name
		{
			get
			{
				return "Invalidation message receiver";
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.msgReceiveSocket.Close();
			}
		}

		protected override void InternalExecute()
		{
			EndPoint endPoint = this.msgReceiveEndPoint;
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCStartingToReceiveInvalidationMessage, this.msgReceiveEndPoint.Address.ToString(), new object[]
			{
				this.msgReceiveEndPoint.Port
			});
			while (!base.GotStopSignalFromTestCode)
			{
				try
				{
					Array.Clear(this.recvBuffer, 0, this.recvBuffer.Length);
					int bufLen = this.msgReceiveSocket.ReceiveFrom(this.recvBuffer, ref endPoint);
					Exception ex = null;
					InvalidationMessage invalidationMessage = InvalidationMessage.TryFromReceivedData(this.recvBuffer, bufLen, out ex);
					if (ex != null)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCInvalidInvalidationMessageReceived, this.msgReceiveEndPoint.Address.ToString(), new object[]
						{
							this.msgReceiveEndPoint.Port,
							ex.Message
						});
					}
					else
					{
						if (invalidationMessage.IsCacheClearMessage)
						{
							base.ProvisioningCache.Reset();
						}
						else if (invalidationMessage.IsGlobal)
						{
							base.ProvisioningCache.RemoveGlobalDatas(invalidationMessage.CacheKeys);
						}
						else
						{
							base.ProvisioningCache.RemoveOrganizationDatas(invalidationMessage.OrganizationId, invalidationMessage.CacheKeys);
						}
						ProvisioningCache.IncrementReceivedInvalidationMsgNum();
					}
				}
				catch (SocketException ex2)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveInvalidationMessage, this.msgReceiveEndPoint.Address.ToString(), new object[]
					{
						this.msgReceiveEndPoint.Port,
						ex2.Message
					});
				}
				catch (ObjectDisposedException ex3)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveInvalidationMessage, this.msgReceiveEndPoint.Address.ToString(), new object[]
					{
						this.msgReceiveEndPoint.Port,
						ex3.Message
					});
					throw ex3;
				}
				catch (SecurityException ex4)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCFailedToReceiveInvalidationMessage, this.msgReceiveEndPoint.Address.ToString(), new object[]
					{
						this.msgReceiveEndPoint.Port,
						ex4.Message
					});
					throw ex4;
				}
			}
		}

		internal override void StopExecute()
		{
			base.StopExecute();
			if (base.GotStopSignalFromTestCode)
			{
				CacheBroadcaster cacheBroadcaster = new CacheBroadcaster(9050U);
				cacheBroadcaster.BroadcastInvalidationMessage(null, new Guid[]
				{
					Guid.NewGuid()
				});
				base.AsyncThread.Join();
			}
		}

		private Socket msgReceiveSocket;

		private IPEndPoint msgReceiveEndPoint;

		private byte[] recvBuffer;
	}
}
