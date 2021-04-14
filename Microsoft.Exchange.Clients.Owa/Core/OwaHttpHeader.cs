using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaHttpHeader
	{
		private OwaHttpHeader()
		{
		}

		public const string Version = "X-OWA-Version";

		public const string ProxyVersion = "X-OWA-ProxyVersion";

		public const string ProxyUri = "X-OWA-ProxyUri";

		public const string ProxySid = "X-OWA-ProxySid";

		public const string ProxyCanary = "X-OWA-ProxyCanary";

		public const string EventResult = "X-OWA-EventResult";

		public const string OwaError = "X-OWA-Error";

		public const string ExplicitLogonUser = "X-OWA-ExplicitLogonUser";

		public const string UserActivity = "X-UserActivity";

		public const string ProxyWebPart = "X-OWA-ProxyWebPart";

		public const string PerfConsoleRowId = "X-OWA-PerfConsoleRowId";

		public const string IsaNoCompression = "X-NoCompression";

		public const string IsaNoBuffering = "X-NoBuffering";

		public const string PublishedAccessPath = "X-OWA-PublishedAccessPath";

		public const string DoNotCache = "X-OWA-DoNotCache";

		public const string Mailbox = "X-DiagInfoMailbox";

		public const string DomainController = "X-DiagInfoDomainController";

		public const string RpcLatency = "X-DiagInfoRpcLatency";

		public const string LdapLatency = "X-DiagInfoLdapLatency";

		public const string IisLatency = "X-DiagInfoIisLatency";

		public const string IsFromCafe = "X-IsFromCafe";
	}
}
