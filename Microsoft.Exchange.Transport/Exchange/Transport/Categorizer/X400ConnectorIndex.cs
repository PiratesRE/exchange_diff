using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class X400ConnectorIndex : ConnectorIndex
	{
		public X400ConnectorIndex(DateTime timestamp) : base(timestamp)
		{
			this.root = new X400ConnectorIndex.X400IndexNode(null);
		}

		public override ConnectorMatchResult TryFindBestConnector(string address, long messageSize, out ConnectorRoutingDestination connectorDestination)
		{
			RoutingX400Address address2;
			if (!RoutingX400Address.TryParse(address, out address2))
			{
				RoutingDiag.Tracer.TraceError<DateTime, string>((long)this.GetHashCode(), "[{0}] Invalid X400 recipient address '{1}'; no connector matching will be performed.", this.Timestamp, address);
				connectorDestination = null;
				return ConnectorMatchResult.InvalidX400Address;
			}
			return this.root.Search(address2, 0, messageSize, this.Timestamp, address, out connectorDestination);
		}

		public override void AddConnector(AddressSpace addressSpace, ConnectorRoutingDestination connectorDestination)
		{
			this.root.Insert(addressSpace, 0, connectorDestination);
		}

		public override bool Match(ConnectorIndex other)
		{
			X400ConnectorIndex x400ConnectorIndex = other as X400ConnectorIndex;
			return x400ConnectorIndex != null && this.root.Match(x400ConnectorIndex.root);
		}

		private X400ConnectorIndex.X400IndexNode root;

		private class X400IndexNode : IComparable<X400ConnectorIndex.X400IndexNode>
		{
			public X400IndexNode(string componentValue)
			{
				if (componentValue != null)
				{
					this.componentPattern = new WildcardPattern(componentValue);
				}
			}

			public void Insert(AddressSpace addressSpace, int componentIndex, ConnectorRoutingDestination connectorDestination)
			{
				if (componentIndex == addressSpace.X400Address.ComponentsCount)
				{
					this.AddConnector(connectorDestination, addressSpace);
					return;
				}
				X400ConnectorIndex.X400IndexNode x400IndexNode = new X400ConnectorIndex.X400IndexNode(addressSpace.X400Address[componentIndex]);
				if (this.children == null)
				{
					this.children = new List<X400ConnectorIndex.X400IndexNode>();
					this.children.Add(x400IndexNode);
				}
				else
				{
					int num = this.children.BinarySearch(x400IndexNode);
					if (num >= 0)
					{
						x400IndexNode = this.children[num];
					}
					else
					{
						num = ~num;
						this.children.Insert(num, x400IndexNode);
					}
				}
				x400IndexNode.Insert(addressSpace, componentIndex + 1, connectorDestination);
			}

			public ConnectorMatchResult Search(RoutingX400Address address, int componentIndex, long messageSize, DateTime timestamp, string strAddress, out ConnectorRoutingDestination connectorDestination)
			{
				bool flag = false;
				ConnectorMatchResult connectorMatchResult;
				if (this.children != null)
				{
					bool flag2 = componentIndex == 1 && address[componentIndex].Equals(" ", StringComparison.OrdinalIgnoreCase);
					foreach (X400ConnectorIndex.X400IndexNode x400IndexNode in this.children)
					{
						if (flag2 || x400IndexNode.componentPattern.Match(address[componentIndex], '%') >= 0)
						{
							connectorMatchResult = x400IndexNode.Search(address, componentIndex + 1, messageSize, timestamp, strAddress, out connectorDestination);
							if (connectorMatchResult == ConnectorMatchResult.Success)
							{
								return ConnectorMatchResult.Success;
							}
							if (ConnectorMatchResult.MaxMessageSizeExceeded == connectorMatchResult)
							{
								flag = true;
							}
						}
					}
				}
				connectorMatchResult = this.TryGetConnectorForMessageSize(messageSize, timestamp, strAddress, out connectorDestination);
				if (ConnectorMatchResult.NoAddressMatch == connectorMatchResult && flag)
				{
					connectorMatchResult = ConnectorMatchResult.MaxMessageSizeExceeded;
				}
				return connectorMatchResult;
			}

			public bool Match(X400ConnectorIndex.X400IndexNode other)
			{
				if (RoutingUtils.NullMatch(this.componentPattern, other.componentPattern) && (this.componentPattern == null || this.componentPattern.Equals(other.componentPattern)) && ConnectorIndex.ConnectorWithCost.MatchLists(this.connectors, other.connectors))
				{
					return RoutingUtils.MatchOrderedLists<X400ConnectorIndex.X400IndexNode>(this.children, other.children, (X400ConnectorIndex.X400IndexNode node1, X400ConnectorIndex.X400IndexNode node2) => node1.Match(node2));
				}
				return false;
			}

			int IComparable<X400ConnectorIndex.X400IndexNode>.CompareTo(X400ConnectorIndex.X400IndexNode other)
			{
				bool flag = WildcardPattern.PatternType.Wildcard == this.componentPattern.Type;
				bool flag2 = WildcardPattern.PatternType.Wildcard == other.componentPattern.Type;
				if (!flag && !flag2)
				{
					return string.Compare(this.componentPattern.Pattern, other.componentPattern.Pattern, StringComparison.OrdinalIgnoreCase);
				}
				return flag.CompareTo(flag2);
			}

			private void AddConnector(ConnectorRoutingDestination connectorDestination, AddressSpace addressSpace)
			{
				if (this.connectors == null)
				{
					this.connectors = new List<ConnectorIndex.ConnectorWithCost>();
				}
				ConnectorIndex.ConnectorWithCost.InsertConnector(connectorDestination, addressSpace, this.connectors);
			}

			private ConnectorMatchResult TryGetConnectorForMessageSize(long messageSize, DateTime timestamp, string address, out ConnectorRoutingDestination matchingConnector)
			{
				return ConnectorIndex.ConnectorWithCost.TryGetConnectorForMessageSize(this.connectors, messageSize, timestamp, "x400", address, out matchingConnector);
			}

			private WildcardPattern componentPattern;

			private List<X400ConnectorIndex.X400IndexNode> children;

			private List<ConnectorIndex.ConnectorWithCost> connectors;
		}
	}
}
