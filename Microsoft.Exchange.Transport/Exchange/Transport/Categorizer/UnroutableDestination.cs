using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class UnroutableDestination : RoutingDestination
	{
		public UnroutableDestination(MailRecipientType destinationType, string identity, RoutingNextHop nextHop) : base(RouteInfo.CreateForUnroutableDestination(identity, nextHop))
		{
			this.destinationType = destinationType;
		}

		public override MailRecipientType DestinationType
		{
			get
			{
				return this.destinationType;
			}
		}

		public override string StringIdentity
		{
			get
			{
				return base.RouteInfo.DestinationName;
			}
		}

		private MailRecipientType destinationType;
	}
}
