using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class IPArrayTracer : ArrayTracer<IPAddress>
	{
		public IPArrayTracer(IPAddress[] array) : base(array)
		{
		}
	}
}
