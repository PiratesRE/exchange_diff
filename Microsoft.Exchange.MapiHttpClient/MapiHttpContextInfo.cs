using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MapiHttpContextInfo
	{
		internal MapiHttpContextInfo(ClientSessionContext clientSessionContext)
		{
			this.Cookies = clientSessionContext.Cookies;
			this.RequestPath = clientSessionContext.RequestPath;
			this.ExpirationPeriod = clientSessionContext.ActualExpiration;
			this.LastCall = clientSessionContext.LastCall;
			this.LastElapsedTime = clientSessionContext.LastElapsedTime;
			this.Expires = clientSessionContext.Expires;
			this.RequestGroupId = clientSessionContext.RequestGroupId;
		}

		public readonly Dictionary<string, string> Cookies;

		public readonly string RequestPath;

		public readonly TimeSpan? ExpirationPeriod;

		public readonly ExDateTime? Expires;

		public readonly ExDateTime? LastCall;

		public readonly TimeSpan? LastElapsedTime;

		public readonly string RequestGroupId;
	}
}
