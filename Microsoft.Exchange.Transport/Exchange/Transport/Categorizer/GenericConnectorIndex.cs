using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class GenericConnectorIndex : ConnectorIndex
	{
		public GenericConnectorIndex(string addressType, DateTime timestamp) : base(timestamp)
		{
			this.addressType = addressType;
			this.entries = new Dictionary<string, GenericConnectorIndex.IndexEntry>();
		}

		public override ConnectorMatchResult TryFindBestConnector(string address, long messageSize, out ConnectorRoutingDestination connectorDestination)
		{
			connectorDestination = null;
			int num = -1;
			bool flag = false;
			ConnectorIndex.ConnectorWithCost connectorWithCost = null;
			foreach (GenericConnectorIndex.IndexEntry indexEntry in this.entries.Values)
			{
				int num2 = indexEntry.Pattern.Match(address);
				if (num2 != -1 && num2 >= num)
				{
					ConnectorIndex.ConnectorWithCost connectorWithCost2;
					ConnectorMatchResult connectorMatchResult = indexEntry.TryGetConnectorForMessageSize(messageSize, this.Timestamp, this.addressType, address, out connectorWithCost2);
					if (connectorMatchResult == ConnectorMatchResult.Success)
					{
						if (num == num2)
						{
							if (connectorWithCost.CompareTo(connectorWithCost2) <= 0)
							{
								continue;
							}
						}
						else
						{
							num = num2;
						}
						connectorWithCost = connectorWithCost2;
						if (address.Length + 1 == num2)
						{
							break;
						}
					}
					else if (connectorMatchResult == ConnectorMatchResult.MaxMessageSizeExceeded)
					{
						flag = true;
					}
				}
			}
			if (connectorWithCost != null)
			{
				connectorDestination = connectorWithCost.ConnectorDestination;
				return ConnectorMatchResult.Success;
			}
			if (!flag)
			{
				return ConnectorMatchResult.NoAddressMatch;
			}
			return ConnectorMatchResult.MaxMessageSizeExceeded;
		}

		public override void AddConnector(AddressSpace addressSpace, ConnectorRoutingDestination connectorDestination)
		{
			WildcardPattern wildcardPattern = new WildcardPattern(addressSpace.Address);
			RoutingDiag.Tracer.TraceDebug((long)this.GetHashCode(), "[{0}] Normalized non-SMTP address space '{1}' to '{2}' (pattern type is {3}) for connector {4}.", new object[]
			{
				this.Timestamp,
				addressSpace.Address,
				wildcardPattern.Pattern,
				wildcardPattern.Type,
				connectorDestination.StringIdentity
			});
			GenericConnectorIndex.IndexEntry indexEntry;
			if (!this.entries.TryGetValue(wildcardPattern.Pattern, out indexEntry))
			{
				indexEntry = new GenericConnectorIndex.IndexEntry(wildcardPattern);
				this.entries.Add(wildcardPattern.Pattern, indexEntry);
				RoutingDiag.Tracer.TraceDebug<DateTime, WildcardPattern>((long)this.GetHashCode(), "[{0}] Added entry '{1}' into generic connector index.", this.Timestamp, wildcardPattern);
			}
			indexEntry.AddConnector(connectorDestination, addressSpace);
		}

		public override bool Match(ConnectorIndex other)
		{
			GenericConnectorIndex genericConnectorIndex = other as GenericConnectorIndex;
			if (genericConnectorIndex == null)
			{
				return false;
			}
			return RoutingUtils.MatchDictionaries<string, GenericConnectorIndex.IndexEntry>(this.entries, genericConnectorIndex.entries, (GenericConnectorIndex.IndexEntry entry1, GenericConnectorIndex.IndexEntry entry2) => entry1.Match(entry2));
		}

		private string addressType;

		private Dictionary<string, GenericConnectorIndex.IndexEntry> entries;

		private class IndexEntry
		{
			public IndexEntry(WildcardPattern pattern)
			{
				this.pattern = pattern;
				this.connectors = new List<ConnectorIndex.ConnectorWithCost>();
			}

			public WildcardPattern Pattern
			{
				get
				{
					return this.pattern;
				}
			}

			public void AddConnector(ConnectorRoutingDestination connectorDestination, AddressSpace addressSpace)
			{
				ConnectorIndex.ConnectorWithCost.InsertConnector(connectorDestination, addressSpace, this.connectors);
			}

			public ConnectorMatchResult TryGetConnectorForMessageSize(long messageSize, DateTime timestamp, string addressType, string address, out ConnectorIndex.ConnectorWithCost connectorWithCost)
			{
				connectorWithCost = null;
				int connectorForMessageSize = ConnectorIndex.ConnectorWithCost.GetConnectorForMessageSize(this.connectors, messageSize, timestamp, addressType, address);
				if (connectorForMessageSize < 0)
				{
					return ConnectorMatchResult.MaxMessageSizeExceeded;
				}
				connectorWithCost = this.connectors[connectorForMessageSize];
				return ConnectorMatchResult.Success;
			}

			public bool Match(GenericConnectorIndex.IndexEntry other)
			{
				return ConnectorIndex.ConnectorWithCost.MatchLists(this.connectors, other.connectors);
			}

			private WildcardPattern pattern;

			private List<ConnectorIndex.ConnectorWithCost> connectors;
		}
	}
}
