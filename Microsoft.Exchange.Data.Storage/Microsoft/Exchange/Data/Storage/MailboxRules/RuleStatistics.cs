using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RuleStatistics
	{
		public static void LogException(Exception e)
		{
			RuleStatistics.exceptionCounter.IncrementCounter(e.ToString());
		}

		private static CounterDictionary<string> exceptionCounter = new CounterDictionary<string>(20, 2000);
	}
}
