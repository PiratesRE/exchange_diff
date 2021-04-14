using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class SmtpConnectorIndex : ConnectorIndex
	{
		public SmtpConnectorIndex(DateTime timestamp) : base(timestamp)
		{
			this.table = new Dictionary<DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString, SmtpConnectorIndex.SmtpIndexNode>();
		}

		public override ConnectorMatchResult TryFindBestConnector(string address, long messageSize, out ConnectorRoutingDestination connectorDestination)
		{
			connectorDestination = null;
			RoutingUtils.ThrowIfNullOrEmpty(address, "address");
			SmtpDomain smtpDomain = null;
			if (!SmtpDomain.TryParse(address, out smtpDomain))
			{
				return ConnectorMatchResult.InvalidSmtpAddress;
			}
			ConnectorMatchResult connectorMatchResult = ConnectorMatchResult.NoAddressMatch;
			DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString subString = new DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString(address, 0);
			SmtpConnectorIndex.SmtpIndexNode smtpIndexNode;
			if (this.table.TryGetValue(subString, out smtpIndexNode))
			{
				connectorMatchResult = smtpIndexNode.TryGetConnector(messageSize, this.Timestamp, address, out connectorDestination);
			}
			if (connectorMatchResult != ConnectorMatchResult.Success)
			{
				int[] array = DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.FindAllDots(address);
				int num = (array.Length > this.maxDots) ? (array.Length - this.maxDots) : 0;
				while (num < array.Length && connectorMatchResult != ConnectorMatchResult.Success)
				{
					subString.SetIndex(array[num] + 1);
					if (this.table.TryGetValue(subString, out smtpIndexNode))
					{
						connectorMatchResult = smtpIndexNode.TryGetConnector(messageSize, this.Timestamp, address, out connectorDestination);
					}
					num++;
				}
			}
			if (connectorMatchResult != ConnectorMatchResult.Success && this.star != null)
			{
				connectorMatchResult = this.star.TryGetConnector(messageSize, this.Timestamp, address, out connectorDestination);
			}
			return connectorMatchResult;
		}

		public override void AddConnector(AddressSpace addressSpace, ConnectorRoutingDestination connectorDestination)
		{
			if (addressSpace.DomainWithSubdomains.SmtpDomain == null)
			{
				if (this.star == null)
				{
					this.star = new SmtpConnectorIndex.SmtpIndexNode(addressSpace);
				}
				this.star.AddConnector(connectorDestination, addressSpace);
				return;
			}
			SmtpConnectorIndex.SmtpIndexNode smtpIndexNode = null;
			DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString key = new DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString(addressSpace.Domain, 0);
			if (!this.table.TryGetValue(key, out smtpIndexNode))
			{
				smtpIndexNode = new SmtpConnectorIndex.SmtpIndexNode(addressSpace);
				this.table[key] = smtpIndexNode;
			}
			smtpIndexNode.AddConnector(connectorDestination, addressSpace);
			int num = DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.CountDots(addressSpace.Domain);
			if (num >= this.maxDots)
			{
				this.maxDots = num + 1;
			}
		}

		public override bool Match(ConnectorIndex other)
		{
			SmtpConnectorIndex smtpConnectorIndex = other as SmtpConnectorIndex;
			if (smtpConnectorIndex == null)
			{
				return false;
			}
			if (!RoutingUtils.NullMatch(this.star, smtpConnectorIndex.star) || (this.star != null && !this.star.Match(smtpConnectorIndex.star)))
			{
				return false;
			}
			return RoutingUtils.MatchDictionaries<DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString, SmtpConnectorIndex.SmtpIndexNode>(this.table, smtpConnectorIndex.table, (SmtpConnectorIndex.SmtpIndexNode node1, SmtpConnectorIndex.SmtpIndexNode node2) => node1.Match(node2));
		}

		private readonly Dictionary<DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.SubString, SmtpConnectorIndex.SmtpIndexNode> table;

		private SmtpConnectorIndex.SmtpIndexNode star;

		private int maxDots;

		private class SmtpIndexNode : DomainMatchMap<SmtpConnectorIndex.SmtpIndexNode>.IDomainEntry
		{
			public SmtpIndexNode(AddressSpace addressSpace)
			{
				this.domainWithSubdomains = addressSpace.DomainWithSubdomains;
				this.connectors = new List<ConnectorIndex.ConnectorWithCost>();
			}

			public List<ConnectorIndex.ConnectorWithCost> Connectors
			{
				get
				{
					return this.connectors;
				}
			}

			public SmtpConnectorIndex.SmtpIndexNode Sibling
			{
				get
				{
					return this.sibling;
				}
			}

			public SmtpDomainWithSubdomains DomainName
			{
				get
				{
					return this.domainWithSubdomains;
				}
			}

			public void AddConnector(ConnectorRoutingDestination connectorDestination, AddressSpace addressSpace)
			{
				SmtpConnectorIndex.SmtpIndexNode smtpIndexNode = this;
				if (addressSpace.DomainWithSubdomains.IncludeSubDomains != this.domainWithSubdomains.IncludeSubDomains)
				{
					if (this.sibling == null)
					{
						this.sibling = new SmtpConnectorIndex.SmtpIndexNode(addressSpace);
					}
					smtpIndexNode = this.sibling;
				}
				ConnectorIndex.ConnectorWithCost.InsertConnector(connectorDestination, addressSpace, smtpIndexNode.connectors);
			}

			public ConnectorMatchResult TryGetConnector(long messageSize, DateTime timestamp, string address, out ConnectorRoutingDestination connectorDestination)
			{
				connectorDestination = null;
				SmtpConnectorIndex.SmtpIndexNode smtpIndexNode = this;
				if (this.DomainName.SmtpDomain != null)
				{
					smtpIndexNode = this.GetCorrectNode(address);
					if (smtpIndexNode == null)
					{
						return ConnectorMatchResult.NoAddressMatch;
					}
				}
				return ConnectorIndex.ConnectorWithCost.TryGetConnectorForMessageSize(smtpIndexNode.connectors, messageSize, timestamp, "smtp", address, out connectorDestination);
			}

			public bool Match(SmtpConnectorIndex.SmtpIndexNode other)
			{
				if (!RoutingUtils.NullMatch(this.sibling, other.sibling))
				{
					return false;
				}
				List<ConnectorIndex.ConnectorWithCost> l;
				List<ConnectorIndex.ConnectorWithCost> l2;
				if (this.domainWithSubdomains.IncludeSubDomains)
				{
					l = this.connectors;
					l2 = ((this.sibling == null) ? null : this.sibling.connectors);
				}
				else
				{
					l2 = this.connectors;
					l = ((this.sibling == null) ? null : this.sibling.connectors);
				}
				List<ConnectorIndex.ConnectorWithCost> l3;
				List<ConnectorIndex.ConnectorWithCost> l4;
				if (other.domainWithSubdomains.IncludeSubDomains)
				{
					l3 = other.connectors;
					l4 = ((other.sibling == null) ? null : other.sibling.connectors);
				}
				else
				{
					l4 = other.connectors;
					l3 = ((other.sibling == null) ? null : other.sibling.connectors);
				}
				return ConnectorIndex.ConnectorWithCost.MatchLists(l2, l4) && ConnectorIndex.ConnectorWithCost.MatchLists(l, l3);
			}

			private SmtpConnectorIndex.SmtpIndexNode GetCorrectNode(string address)
			{
				SmtpConnectorIndex.SmtpIndexNode result = this;
				if (address.Equals(this.DomainName.Domain.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					if (this.DomainName.IncludeSubDomains && this.sibling != null)
					{
						result = this.sibling;
					}
				}
				else if (!this.DomainName.IncludeSubDomains)
				{
					result = this.sibling;
				}
				return result;
			}

			private readonly SmtpDomainWithSubdomains domainWithSubdomains;

			private List<ConnectorIndex.ConnectorWithCost> connectors;

			private SmtpConnectorIndex.SmtpIndexNode sibling;
		}
	}
}
