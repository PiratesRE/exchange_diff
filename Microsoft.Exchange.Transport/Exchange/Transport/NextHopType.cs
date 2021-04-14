using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	internal struct NextHopType
	{
		public NextHopType(DeliveryType deliveryType)
		{
			this.deliveryType = deliveryType;
			if (TransportDeliveryTypes.internalDeliveryTypes.Contains(this.deliveryType))
			{
				this.nextHopCategory = NextHopCategory.Internal;
				return;
			}
			if (TransportDeliveryTypes.externalDeliveryTypes.Contains(this.deliveryType))
			{
				this.nextHopCategory = NextHopCategory.External;
				return;
			}
			throw new InvalidOperationException(string.Format("DeliveryType '{0}' not categorized as internal or external, or missing a value for NextHopCategory", this.deliveryType));
		}

		public NextHopType(int nextHopType)
		{
			this = new NextHopType((DeliveryType)nextHopType);
		}

		public DeliveryType DeliveryType
		{
			get
			{
				return this.deliveryType;
			}
			set
			{
				this.deliveryType = value;
			}
		}

		public NextHopCategory NextHopCategory
		{
			get
			{
				return this.nextHopCategory;
			}
		}

		public bool IsConnectorDeliveryType
		{
			get
			{
				return this.IsSmtpConnectorDeliveryType || this.deliveryType == DeliveryType.NonSmtpGatewayDelivery || this.deliveryType == DeliveryType.DeliveryAgent;
			}
		}

		public bool IsSmtpConnectorDeliveryType
		{
			get
			{
				return this.deliveryType == DeliveryType.DnsConnectorDelivery || this.deliveryType == DeliveryType.SmartHostConnectorDelivery;
			}
		}

		public bool IsSmtpSmtpRelayToEdge
		{
			get
			{
				return this.deliveryType == DeliveryType.SmtpRelayWithinAdSiteToEdge;
			}
		}

		public bool IsHubRelayDeliveryType
		{
			get
			{
				return this.deliveryType == DeliveryType.SmtpRelayToDag || this.deliveryType == DeliveryType.SmtpRelayToMailboxDeliveryGroup || this.deliveryType == DeliveryType.SmtpRelayToConnectorSourceServers || this.deliveryType == DeliveryType.SmtpRelayToServers || this.deliveryType == DeliveryType.SmtpRelayToRemoteAdSite || this.deliveryType == DeliveryType.SmtpRelayWithinAdSite;
			}
		}

		public static bool IsMailboxDeliveryType(DeliveryType deliveryType)
		{
			return deliveryType == DeliveryType.MapiDelivery || deliveryType == DeliveryType.SmtpDeliveryToMailbox;
		}

		public static bool operator ==(NextHopType op1, NextHopType op2)
		{
			return op1.Equals(op2);
		}

		public static bool operator !=(NextHopType op1, NextHopType op2)
		{
			return !(op1 == op2);
		}

		public bool Equals(NextHopType obj)
		{
			return this.deliveryType == obj.deliveryType;
		}

		public int ToInt32()
		{
			return (int)this.deliveryType;
		}

		public override bool Equals(object obj)
		{
			return obj is NextHopType && this.Equals((NextHopType)obj);
		}

		public override string ToString()
		{
			return this.deliveryType.ToString();
		}

		public override int GetHashCode()
		{
			return (int)this.deliveryType;
		}

		public static readonly NextHopType Empty = new NextHopType(DeliveryType.Undefined);

		public static readonly NextHopType Unreachable = new NextHopType(DeliveryType.Unreachable);

		public static readonly NextHopType ShadowRedundancy = new NextHopType(DeliveryType.ShadowRedundancy);

		public static readonly NextHopType Heartbeat = new NextHopType(DeliveryType.Heartbeat);

		private DeliveryType deliveryType;

		private NextHopCategory nextHopCategory;
	}
}
