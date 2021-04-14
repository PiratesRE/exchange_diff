using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.AgentLog
{
	internal class AgentLogEntry
	{
		public DateTime Timestamp;

		public string SessionId;

		public IPAddress IPAddress;

		public string MessageId;

		public AgentLogEntry.RoutingAddressWrapper P1FromAddress;

		public List<AgentLogEntry.RoutingAddressWrapper> P2FromAddresses;

		public List<AgentLogEntry.RoutingAddressWrapper> Recipients;

		public string Agent;

		public string Event;

		public AgentAction Action;

		public SmtpResponse SmtpResponse;

		public string Reason;

		public string ReasonData;

		public string Diagnostics;

		public Guid NetworkMsgID;

		public Guid TenantID;

		public string Directionality;

		internal struct RoutingAddressWrapper
		{
			public RoutingAddressWrapper(RoutingAddress routingAddress)
			{
				this.routingAddress = routingAddress;
			}

			public string LocalPart
			{
				get
				{
					return this.routingAddress.LocalPart;
				}
			}

			public string DomainPart
			{
				get
				{
					return this.routingAddress.DomainPart;
				}
			}

			public bool IsValid
			{
				get
				{
					return this.routingAddress.IsValid;
				}
			}

			public string Name
			{
				get
				{
					return this.ToString();
				}
			}

			public override string ToString()
			{
				return this.routingAddress.ToString();
			}

			private readonly RoutingAddress routingAddress;
		}
	}
}
