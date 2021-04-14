using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LogonResultFactory : StandardResultFactory
	{
		internal LogonResultFactory(byte logonId) : base(RopId.Logon)
		{
			this.logonId = logonId;
		}

		public byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		public RopResult CreateRedirectResult(string serverLegacyDn, LogonFlags logonFlags)
		{
			return new RedirectLogonResult(serverLegacyDn, logonFlags);
		}

		public RopResult CreateSuccessfulPrivateResult(IServerObject logonObject, LogonFlags logonFlags, StoreId[] folderIds, LogonExtendedResponseFlags extendedFlags, LocaleInfo? localeInfo, LogonResponseFlags logonResponseFlags, Guid mailboxInstanceGuid, ReplId databaseReplId, Guid databaseGuid, ExDateTime serverTime, ulong routingConfigurationTimestamp, StoreState storeState)
		{
			return new SuccessfulPrivateLogonResult(logonObject, logonFlags, folderIds, extendedFlags, localeInfo, logonResponseFlags, mailboxInstanceGuid, databaseReplId, databaseGuid, serverTime, routingConfigurationTimestamp, storeState);
		}

		public RopResult CreateSuccessfulPublicResult(IServerObject logonObject, LogonFlags logonFlags, StoreId[] folderIds, LogonExtendedResponseFlags extendedFlags, LocaleInfo? localeInfo, ReplId databaseReplId, Guid databaseGuid, Guid perUserReadGuid)
		{
			return new SuccessfulPublicLogonResult(logonObject, logonFlags, folderIds, extendedFlags, localeInfo, databaseReplId, databaseGuid, perUserReadGuid);
		}

		private readonly byte logonId;
	}
}
