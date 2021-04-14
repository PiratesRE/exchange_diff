using System;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxTypeCache : LazyLookupTimeoutCache<string, MailboxHelper.MailboxTypeType?>
	{
		private MailboxTypeCache() : base(10, 65536, false, TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(360.0))
		{
		}

		internal static MailboxTypeCache Instance
		{
			get
			{
				if (MailboxTypeCache.instance == null)
				{
					lock (MailboxTypeCache.lockObject)
					{
						if (MailboxTypeCache.instance == null)
						{
							MailboxTypeCache.instance = new MailboxTypeCache();
						}
					}
				}
				return MailboxTypeCache.instance;
			}
		}

		internal long CacheSize
		{
			get
			{
				return this.cacheSize;
			}
		}

		internal bool TryGetValue(string key, out MailboxHelper.MailboxTypeType? mailboxType)
		{
			mailboxType = base.Get(key);
			if (mailboxType == null)
			{
				int num = -1;
				if (RequestDetailsLogger.Current != null)
				{
					if (int.TryParse(RequestDetailsLogger.Current.Get(EwsMetadata.MailboxTypeCacheMissCount), out num))
					{
						RequestDetailsLogger.Current.Set(EwsMetadata.MailboxTypeCacheMissCount, num + 1);
					}
					else
					{
						RequestDetailsLogger.Current.Set(EwsMetadata.MailboxTypeCacheMissCount, 1);
					}
				}
			}
			else
			{
				int num2 = -1;
				if (RequestDetailsLogger.Current != null)
				{
					if (int.TryParse(RequestDetailsLogger.Current.Get(EwsMetadata.MailboxTypeCacheHitCount), out num2))
					{
						RequestDetailsLogger.Current.Set(EwsMetadata.MailboxTypeCacheHitCount, num2 + 1);
					}
					else
					{
						RequestDetailsLogger.Current.Set(EwsMetadata.MailboxTypeCacheHitCount, 1);
					}
				}
			}
			return mailboxType != null;
		}

		internal bool TryAdd(string key, MailboxHelper.MailboxTypeType mailboxType)
		{
			bool flag = this.TryPerformAdd(key, new MailboxHelper.MailboxTypeType?(mailboxType));
			if (!flag)
			{
				MailboxHelper.MailboxTypeType? mailboxTypeType = base.Get(key);
				if (mailboxTypeType != null && mailboxType != mailboxTypeType.Value && RequestDetailsLogger.Current != null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(CallContext.Current.ProtocolLog, "MailboxTypeCacheMismatch", string.Format("Actual:{0} Expected:{1}", mailboxTypeType.Value, mailboxType));
				}
			}
			else
			{
				Interlocked.Increment(ref this.cacheSize);
			}
			return flag;
		}

		protected override MailboxHelper.MailboxTypeType? CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			shouldAdd = false;
			return null;
		}

		internal static bool TryGetKey(string emailAddress, string orgUnitName, out string key)
		{
			key = null;
			if (string.IsNullOrEmpty(orgUnitName) || string.IsNullOrEmpty(emailAddress))
			{
				return false;
			}
			string format = "{0}:{1}";
			key = string.Format(format, orgUnitName, emailAddress);
			return true;
		}

		private static MailboxTypeCache instance;

		private static object lockObject = new object();

		private long cacheSize;
	}
}
