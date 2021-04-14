using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class EasMailboxSessionCacheHandler : ExchangeDiagnosableWrapper<EasMailboxSessionCacheResult>
	{
		protected override string UsageText
		{
			get
			{
				return "The mailbox session cache handler is a diagnostics handler that returns information about the current state of the mailbox session cache. \r\n                        The handler supports dumpcache argument. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Metadata only.\r\n                        Get-ExchangeDiagnosticInfo Process MSExchangeSync Component EASMailboxSessionCache\r\n\r\n                        Example 2: Dump Cache.\r\n                        Get-ExchangeDiagnosticInfo Process MSExchangeSync Component EASMailboxSessionCache -Argument dumpcache";
			}
		}

		public static EasMailboxSessionCacheHandler GetInstance()
		{
			if (EasMailboxSessionCacheHandler.instance == null)
			{
				lock (EasMailboxSessionCacheHandler.lockObject)
				{
					if (EasMailboxSessionCacheHandler.instance == null)
					{
						EasMailboxSessionCacheHandler.instance = new EasMailboxSessionCacheHandler();
					}
				}
			}
			return EasMailboxSessionCacheHandler.instance;
		}

		private EasMailboxSessionCacheHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "EasMailboxSessionCache";
			}
		}

		internal override EasMailboxSessionCacheResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			if (argument.Argument.ToLower() == "dumpcache")
			{
				return new EasMailboxSessionCacheResult(MailboxSessionCache.GetCacheEntries());
			}
			return new EasMailboxSessionCacheResult(MailboxSessionCache.Count);
		}

		private const string DumpCacheArgument = "dumpcache";

		private static EasMailboxSessionCacheHandler instance;

		private static object lockObject = new object();
	}
}
