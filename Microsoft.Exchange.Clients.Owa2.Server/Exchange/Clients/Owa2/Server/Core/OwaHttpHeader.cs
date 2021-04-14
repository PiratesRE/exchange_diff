using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaHttpHeader
	{
		public const string ExplicitLogonUser = "X-OWA-ExplicitLogonUser";

		public const string ProxyUri = "X-OWA-ProxyUri";

		public const string ProxyVersion = "X-OWA-ProxyVersion";

		public const string PublishedAccessPath = "X-OWA-PublishedAccessPath";

		public const string Version = "X-OWA-Version";

		public const string OWSVersion = "X-OWA-OWSVersion";

		public const string ClientBuildVersion = "X-OWA-ClientBuildVersion";

		public const string MinimumSupportedOWSVersion = "X-OWA-MinimumSupportedOWSVersion";

		public const string OwaError = "X-OWA-Error";

		public const string RpcLatency = "X-DiagInfoRpcLatency";

		public const string LdapLatency = "X-DiagInfoLdapLatency";

		public const string IisLatency = "X-DiagInfoIisLatency";

		public const string ClientActionName = "X-OWA-ActionName";

		public const string TestExplicitLogonUserId = "X-OWA-Test-ExplicitLogonUserId";

		public const string TestRemoteNotificationEndPoint = "X-OWA-Test-RemoteNotificationEndPoint";

		public const string PassThroughProxyHeaderName = "X-OWA-Test-PassThruProxy";
	}
}
