using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackRedirectInfo
	{
		public MsgTrackRedirectInfo(string sourceContext, RoutingAddress originalAddress, RoutingDomain redirectedConnectorDomain, string redirectedDeliveryDestination, long? relatedMailItemId) : this(sourceContext, originalAddress, RoutingAddress.Empty, relatedMailItemId, null)
		{
			this.redirectedConnectorDomain = redirectedConnectorDomain;
			this.redirectedDeliveryDestination = redirectedDeliveryDestination;
		}

		public MsgTrackRedirectInfo(RoutingAddress originalAddress, RoutingAddress redirectedAddress, long? relatedMailItemId) : this(null, originalAddress, redirectedAddress, relatedMailItemId, null)
		{
		}

		public MsgTrackRedirectInfo(RoutingAddress originalAddress, RoutingAddress redirectedAddress, long? relatedMailItemId, SmtpResponse? response) : this(null, originalAddress, redirectedAddress, relatedMailItemId, response)
		{
		}

		public MsgTrackRedirectInfo(string sourceContext, RoutingAddress originalAddress, RoutingAddress redirectedAddress, long? relatedMailItemId) : this(sourceContext, originalAddress, redirectedAddress, relatedMailItemId, null)
		{
		}

		public MsgTrackRedirectInfo(string sourceContext, RoutingAddress originalAddress, RoutingAddress redirectedAddress, long? relatedMailItemId, SmtpResponse? response)
		{
			this.sourceContext = sourceContext;
			this.originalAddress = originalAddress;
			this.redirectedAddress = redirectedAddress;
			this.relatedMailItemId = relatedMailItemId;
			this.response = response;
		}

		internal string SourceContext
		{
			get
			{
				return this.sourceContext;
			}
		}

		internal RoutingAddress OriginalAddress
		{
			get
			{
				return this.originalAddress;
			}
		}

		internal RoutingAddress RedirectedAddress
		{
			get
			{
				return this.redirectedAddress;
			}
		}

		internal RoutingDomain RedirectedConnectorDomain
		{
			get
			{
				return this.redirectedConnectorDomain;
			}
		}

		internal string RedirectedDeliveryDestination
		{
			get
			{
				return this.redirectedDeliveryDestination;
			}
		}

		internal long? RelatedMailItemId
		{
			get
			{
				return this.relatedMailItemId;
			}
		}

		internal SmtpResponse? Response
		{
			get
			{
				return this.response;
			}
		}

		private readonly string sourceContext;

		private readonly RoutingAddress originalAddress;

		private readonly RoutingAddress redirectedAddress;

		private readonly RoutingDomain redirectedConnectorDomain;

		private readonly string redirectedDeliveryDestination;

		private readonly long? relatedMailItemId;

		private readonly SmtpResponse? response;
	}
}
