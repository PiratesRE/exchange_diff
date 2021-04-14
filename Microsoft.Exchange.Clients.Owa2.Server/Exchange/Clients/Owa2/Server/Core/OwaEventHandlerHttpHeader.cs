using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaEventHandlerHttpHeader
	{
		private OwaEventHandlerHttpHeader()
		{
		}

		public const string EventResult = "X-OWA-EventResult";

		public const string IsaNoCompression = "X-NoCompression";

		public const string IsaNoBuffering = "X-NoBuffering";
	}
}
