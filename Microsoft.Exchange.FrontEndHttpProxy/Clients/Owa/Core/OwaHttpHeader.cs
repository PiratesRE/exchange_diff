using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class OwaHttpHeader
	{
		public const string Version = "X-OWA-Version";

		public const string ProxyVersion = "X-OWA-ProxyVersion";

		public const string ProxyUri = "X-OWA-ProxyUri";

		public const string ProxySid = "X-OWA-ProxySid";

		public const string ProxyCanary = "X-OWA-ProxyCanary";

		public const string EventResult = "X-OWA-EventResult";

		public const string OwaError = "X-OWA-Error";

		public const string OwaFEError = "X-OWA-FEError";

		public const string ExplicitLogonUser = "X-OWA-ExplicitLogonUser";

		public const string ProxyWebPart = "X-OWA-ProxyWebPart";

		public const string PerfConsoleRowId = "X-OWA-PerfConsoleRowId";

		public const string IsaNoCompression = "X-NoCompression";

		public const string IsaNoBuffering = "X-NoBuffering";

		public const string PublishedAccessPath = "X-OWA-PublishedAccessPath";

		public const string DoNotCache = "X-OWA-DoNotCache";
	}
}
