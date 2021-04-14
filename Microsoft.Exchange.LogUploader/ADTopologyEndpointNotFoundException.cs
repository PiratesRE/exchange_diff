using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class ADTopologyEndpointNotFoundException : MessageTracingException
	{
		public ADTopologyEndpointNotFoundException(string message) : base(message)
		{
		}
	}
}
