using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class TargetHostArrayTracer : ArrayTracerWithWrapper<TargetHost, TargetHostTraceWrapper>
	{
		public TargetHostArrayTracer(TargetHost[] array) : base(array)
		{
		}
	}
}
