using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabManifestFormatException : SlabManifestException
	{
		public SlabManifestFormatException(string message) : base(message)
		{
		}

		public SlabManifestFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
