using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal sealed class RemoveConditionHandler : ExchangeDiagnosableWrapper<RemoveConditionResult>
	{
		protected override string UsageText
		{
			get
			{
				return "The RemoveConditional handler does what its name suggests – it “unregisters” or removes active conditionals.  The component or “method name” to use here is “RemoveCondition”.. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: To remove a single conditional, the argument must be in the format: Cookie=<GUID>\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component RemoveCondition –Argument \"cookie=0d341cfc-8577-4677-aa73-187b9ba6cc5c\"\r\n\r\n                                    Example 2: To remove ALL conditionals, pass in “removeall” as the argument.  The response will include all of the conditionals that were removed.\r\n                                    Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component RemoveCondition –Argument removeall";
			}
		}

		public static RemoveConditionHandler GetInstance()
		{
			if (RemoveConditionHandler.instance == null)
			{
				lock (RemoveConditionHandler.lockObject)
				{
					if (RemoveConditionHandler.instance == null)
					{
						RemoveConditionHandler.instance = new RemoveConditionHandler();
					}
				}
			}
			return RemoveConditionHandler.instance;
		}

		private RemoveConditionHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "RemoveCondition";
			}
		}

		internal override RemoveConditionResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			if (string.IsNullOrEmpty(argument.Argument))
			{
				throw new ArgumentException("RemoveCondition requires an argument with either 'cookie=[guid]' or 'removeall'.");
			}
			if (argument.Argument.Trim().ToLower().StartsWith("cookie="))
			{
				return this.GetRemove(argument.Argument);
			}
			if (argument.Argument.Trim().ToLower().StartsWith("removeall"))
			{
				return this.GetAllRemove();
			}
			throw new ArgumentException("RemoveCondition requires an argument with either 'cookie=[guid]' or 'removeall'.  Encountered: " + argument.Argument);
		}

		internal RemoveConditionResult GetRemove(string argument)
		{
			int num = argument.IndexOf('=');
			string cookie = argument.Substring(num + 1);
			ConditionalRegistration conditionalRegistration = ConditionalRegistrationCache.Singleton.GetRegistration(cookie) as ConditionalRegistration;
			bool removed = false;
			if (conditionalRegistration != null)
			{
				removed = ConditionalRegistrationCache.Singleton.Remove(cookie);
			}
			return new RemoveConditionResult(cookie, removed);
		}

		internal RemoveConditionResult GetAllRemove()
		{
			List<string> allKeys = ConditionalRegistrationCache.Singleton.GetAllKeys();
			for (int i = allKeys.Count - 1; i >= 0; i--)
			{
				string cookie = allKeys[i];
				ConditionalRegistration conditionalRegistration = ConditionalRegistrationCache.Singleton.GetRegistration(cookie) as ConditionalRegistration;
				if (conditionalRegistration != null)
				{
					ConditionalRegistrationCache.Singleton.Remove(cookie);
				}
				else
				{
					allKeys.RemoveAt(i);
				}
			}
			return new RemoveConditionResult(allKeys);
		}

		private const string CookieArg = "cookie=";

		private const string RemoveAllArg = "removeall";

		private static RemoveConditionHandler instance;

		private static object lockObject = new object();
	}
}
