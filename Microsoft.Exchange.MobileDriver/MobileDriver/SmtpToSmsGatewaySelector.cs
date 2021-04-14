using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.VersionedXml;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class SmtpToSmsGatewaySelector : IMobileServiceSelector
	{
		public SmtpToSmsGatewaySelector(ExchangePrincipal principal, DeliveryPoint dp)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (dp == null)
			{
				throw new ArgumentNullException("dp");
			}
			if (DeliveryPointType.SmtpToSmsGateway != dp.Type)
			{
				throw new ArgumentOutOfRangeException("dp");
			}
			this.Principal = principal;
			this.DeliveryPoint = dp;
		}

		internal SmtpToSmsGatewaySelector(ExchangePrincipal principal)
		{
			this.Principal = principal;
			this.p2pPriority = new int?(0);
			this.m2pPriority = new int?(0);
		}

		public MobileServiceType Type
		{
			get
			{
				return MobileServiceType.SmtpToSmsGateway;
			}
		}

		public int PersonToPersonMessagingPriority
		{
			get
			{
				if (this.p2pPriority == null)
				{
					this.p2pPriority = new int?(this.DeliveryPoint.P2pMessagingPriority);
				}
				return this.p2pPriority.Value;
			}
		}

		public int MachineToPersonMessagingPriority
		{
			get
			{
				if (this.m2pPriority == null)
				{
					this.m2pPriority = new int?(this.DeliveryPoint.M2pMessagingPriority);
				}
				return this.m2pPriority.Value;
			}
		}

		public ExchangePrincipal Principal { get; private set; }

		private DeliveryPoint DeliveryPoint { get; set; }

		private string Literal { get; set; }

		public override string ToString()
		{
			string result;
			if ((result = this.Literal) == null)
			{
				result = (this.Literal = string.Format("{0}:{1}", this.Type, (this.Principal == null) ? null : this.Principal.ObjectId));
			}
			return result;
		}

		private int? p2pPriority;

		private int? m2pPriority;
	}
}
