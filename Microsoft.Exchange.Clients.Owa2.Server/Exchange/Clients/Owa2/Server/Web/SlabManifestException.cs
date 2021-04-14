using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabManifestException : Exception
	{
		public SlabManifestException(string message) : base(message)
		{
		}

		public SlabManifestException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
