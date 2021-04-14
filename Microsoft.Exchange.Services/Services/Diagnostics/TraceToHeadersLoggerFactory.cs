using System;
using System.Collections.Specialized;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TraceToHeadersLoggerFactory
	{
		public TraceToHeadersLoggerFactory(bool enabled)
		{
			this.enabled = enabled;
		}

		public ITraceLogger Create(NameValueCollection headers)
		{
			if (!this.enabled)
			{
				return NullTraceLogger.Instance;
			}
			return new TraceToHeadersLogger(headers);
		}

		private readonly bool enabled;
	}
}
