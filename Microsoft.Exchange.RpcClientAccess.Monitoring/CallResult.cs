using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CallResult
	{
		public abstract bool IsSuccessful { get; }

		public TimeSpan Latency { get; internal set; }

		public virtual void Validate()
		{
		}
	}
}
