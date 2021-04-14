using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface IServiceBinding
	{
		ServiceHttpContext HttpContext { get; set; }
	}
}
