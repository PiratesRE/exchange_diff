using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class ThrottleSession
	{
		public ThrottleSession(long sessionId)
		{
			this.sessionId = sessionId;
			this.recipients = new Dictionary<RoutingAddress, int>();
		}

		public long SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public Guid? Mdb { get; set; }

		public long MessageSize { get; set; }

		public Dictionary<RoutingAddress, int> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		private readonly long sessionId;

		private Dictionary<RoutingAddress, int> recipients;
	}
}
