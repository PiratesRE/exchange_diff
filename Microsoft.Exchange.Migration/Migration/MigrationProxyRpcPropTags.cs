using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationProxyRpcPropTags
	{
		public const uint InArgRpcHostServer = 2415984671U;

		public const uint InArgRpcProxyServer = 2416050207U;

		public const uint InArgCredentialUserName = 2416115743U;

		public const uint InArgCredentialUserDomain = 2416181279U;

		public const uint InArgCredentialUserPassword = 2416246815U;

		public const uint InArgBatchSize = 2416312323U;

		public const uint InArgStartIndex = 2416377859U;

		public const uint InArgPropTags = 2416447508U;

		public const uint InArgSmtpAddress = 2416508959U;

		public const uint InArgAutodiscoverDomain = 2416574495U;

		public const uint InArgAutodiscoverUrl = 2416640031U;

		public const uint InArgExchangeVersion = 2416705539U;

		public const uint InArgPropTagValues = 2416775199U;

		public const uint InArgRpcAuthenticationMethod = 2416836639U;

		public const uint InArgExchangeServer = 2416902175U;

		public const uint InArgLegDN = 2416967711U;

		public const uint OutArgException = 2432761887U;

		public const uint OutArgTotalSize = 2432827395U;

		public const uint OutArgNspiServer = 2432892959U;

		public const uint OutArgAutodiscoverStatus = 2432958467U;

		public const uint OutArgExchangeVersion = 2433024003U;

		public const uint OutArgMailboxDN = 2433089567U;

		public const uint OutArgExchangeServerDN = 2433155103U;

		public const uint OutArgRpcProxyServer = 2433220639U;

		public const uint OutArgExchangeServer = 2433286175U;

		public const uint OutArgAuthenticationMethod = 2433351683U;

		public const uint OutArgAutodiscoverUrl = 2433417247U;

		public const uint OutArgAutodiscoverErrorCode = 2433482755U;

		public const uint OutArgRpcErrorCode = 2433548291U;
	}
}
