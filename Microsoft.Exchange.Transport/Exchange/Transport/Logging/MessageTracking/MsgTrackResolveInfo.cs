using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackResolveInfo
	{
		public MsgTrackResolveInfo(string sourceContext, RoutingAddress origAddress, RoutingAddress newAddress)
		{
			this.sourceContext = sourceContext;
			this.originalAddress = origAddress;
			this.resolvedAddress = newAddress;
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

		internal RoutingAddress ResolvedAddress
		{
			get
			{
				return this.resolvedAddress;
			}
		}

		private string sourceContext;

		private RoutingAddress originalAddress;

		private RoutingAddress resolvedAddress;
	}
}
