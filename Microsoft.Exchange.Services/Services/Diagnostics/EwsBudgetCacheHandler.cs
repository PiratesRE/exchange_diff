using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal sealed class EwsBudgetCacheHandler : ExchangeDiagnosableWrapper<BudgetCacheHandlerMetadata>
	{
		private EwsBudgetCacheHandler()
		{
		}

		protected override string UsageText
		{
			get
			{
				return "This handler returns metadata about the EWS budget cache in a given process.";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Return all budget entries in cache:\\r\\nGet-ExchangeDiagnosticInfo -Process MSExchangeServicesAppPool -Component EwsBudgetCache\r\n\r\nExample 2: Return all budget entries that are locked out:\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeServicesAppPool -Component EwsBudgetCache -Argument \"IsLocked -eq 'true'\"\r\n\r\nExample 3: Return all budget entries johndoe (alias):\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeServicesAppPool -Component EwsBudgetCache -Argument \"Name -like '*johndoe*'\"\r\n\r\nThe argument is a OPath query.  Supported fields:\r\nInCutoff (bool), InMicroDelay (bool), NotThrottled (bool), Connections (int), Balance (float), OutstandingActionCount (int)\r\nIsServiceAccount (bool), ThrottlingPolicy (string), LiveTime (TimeSpan), Name (string), IsGlobalPolicy (bool), IsOrgPolicy (bool)\r\nIsRegularPolicy (bool)\r\n\r\nNOTE: Name is the budget KEY value, not the smtp address.  This will typically map to either the NTAccount or the sid of the user.  You must \r\nuse the -like filter if you wish to be happy.";
			}
		}

		protected override string ComponentName
		{
			get
			{
				return "EwsBudgetCache";
			}
		}

		public static EwsBudgetCacheHandler GetInstance()
		{
			if (EwsBudgetCacheHandler.instance == null)
			{
				lock (EwsBudgetCacheHandler.lockObject)
				{
					if (EwsBudgetCacheHandler.instance == null)
					{
						EwsBudgetCacheHandler.instance = new EwsBudgetCacheHandler();
					}
				}
			}
			return EwsBudgetCacheHandler.instance;
		}

		internal override BudgetCacheHandlerMetadata GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			return EwsBudgetCache.Singleton.GetMetadata((arguments.Argument == null) ? null : arguments.Argument);
		}

		private static EwsBudgetCacheHandler instance;

		private static object lockObject = new object();
	}
}
