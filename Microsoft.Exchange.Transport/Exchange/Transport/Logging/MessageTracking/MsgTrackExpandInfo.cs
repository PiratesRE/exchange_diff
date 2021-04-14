using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackExpandInfo
	{
		public MsgTrackExpandInfo(RoutingAddress groupAddress, long? relatedMailItemId, string statusText) : this(null, groupAddress, relatedMailItemId, statusText)
		{
		}

		public MsgTrackExpandInfo(string sourceContext, RoutingAddress groupAddress, long? relatedMailItemId, string statusText)
		{
			this.sourceContext = sourceContext;
			this.groupAddress = groupAddress;
			this.relatedMailItemId = relatedMailItemId;
			this.statusText = statusText;
		}

		internal string SourceContext
		{
			get
			{
				return this.sourceContext;
			}
		}

		internal RoutingAddress GroupAddress
		{
			get
			{
				return this.groupAddress;
			}
		}

		internal long? RelatedMailItemId
		{
			get
			{
				return this.relatedMailItemId;
			}
		}

		internal string StatusText
		{
			get
			{
				return this.statusText;
			}
		}

		private string sourceContext;

		private RoutingAddress groupAddress;

		private long? relatedMailItemId;

		private string statusText;
	}
}
