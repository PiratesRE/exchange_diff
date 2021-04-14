using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ClientIPTable
	{
		public ClientData Add(IPAddress address, out int totalConnections)
		{
			ArgumentValidator.ThrowIfNull("address", address);
			totalConnections = Interlocked.Increment(ref this.connectionCount);
			ClientData result;
			lock (((ICollection)this.table).SyncRoot)
			{
				ClientData clientData;
				if (this.table.TryGetValue(address, out clientData))
				{
					clientData.Count++;
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IPAddress, int, bool>((long)address.GetHashCode(), "Client IP entry for {0} found: count={1} discredited={2}", address, clientData.Count, clientData.Discredited);
					result = clientData;
				}
				else
				{
					clientData = new ClientData();
					clientData.Count = 1;
					this.table.Add(address, clientData);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IPAddress>((long)address.GetHashCode(), "Client IP entry for {0} not found: created", address);
					result = clientData;
				}
			}
			return result;
		}

		public ClientData Add(IPAddress address, ulong significantAddressBytes, out int totalConnections)
		{
			ArgumentValidator.ThrowIfNull("address", address);
			totalConnections = Interlocked.Increment(ref this.connectionCount);
			ClientData result;
			lock (((ICollection)this.table).SyncRoot)
			{
				ClientData clientData;
				if (this.ipAddressSignificantBytesTable.TryGetValue(significantAddressBytes, out clientData))
				{
					clientData.Count++;
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)address.GetHashCode(), "Client IP entry for {0} found using its required significant 64 bits {1}: count={2} discredited={3}", new object[]
					{
						address,
						significantAddressBytes,
						clientData.Count,
						clientData.Discredited
					});
					result = clientData;
				}
				else
				{
					clientData = new ClientData();
					clientData.Count = 1;
					this.ipAddressSignificantBytesTable.Add(significantAddressBytes, clientData);
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IPAddress, ulong>((long)address.GetHashCode(), "Client IP entry for {0} not found using its required significant 64 bits {1}: created", address, significantAddressBytes);
					result = clientData;
				}
			}
			return result;
		}

		public void Remove(IPAddress address)
		{
			ArgumentValidator.ThrowIfNull("address", address);
			Interlocked.Decrement(ref this.connectionCount);
			lock (((ICollection)this.table).SyncRoot)
			{
				ClientData clientData;
				if (this.table.TryGetValue(address, out clientData))
				{
					clientData.Count--;
					if (clientData.Count == 0)
					{
						this.table.Remove(address);
					}
				}
			}
		}

		public void Remove(ulong significantAddressBytes)
		{
			Interlocked.Decrement(ref this.connectionCount);
			lock (((ICollection)this.table).SyncRoot)
			{
				ClientData clientData;
				if (this.ipAddressSignificantBytesTable.TryGetValue(significantAddressBytes, out clientData))
				{
					clientData.Count--;
					if (clientData.Count == 0)
					{
						this.ipAddressSignificantBytesTable.Remove(significantAddressBytes);
					}
				}
			}
		}

		private const int InitialTableSize = 500;

		private int connectionCount;

		private Dictionary<IPAddress, ClientData> table = new Dictionary<IPAddress, ClientData>(500);

		private Dictionary<ulong, ClientData> ipAddressSignificantBytesTable = new Dictionary<ulong, ClientData>();
	}
}
