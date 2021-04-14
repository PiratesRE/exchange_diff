using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal static class ExchangeComponent
	{
		public static string Name
		{
			get
			{
				return ExchangeComponent.name ?? ExchangeComponent.ReadName();
			}
		}

		private static string ReadName()
		{
			try
			{
				ExchangeComponent.name = ConfigurationManager.AppSettings["ExchangeComponent"];
			}
			catch (ConfigurationErrorsException arg)
			{
				string text = string.Format("Fail to read config value, default values are used. The error is {0}", arg);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ConfigSettingNotFound, Thread.CurrentThread.Name, new object[]
				{
					text
				});
				ExchangeComponent.name = "MessageTracing";
			}
			bool flag = string.IsNullOrWhiteSpace(ExchangeComponent.name);
			if (flag)
			{
				string text2 = string.Format("Fail to read Exchange Component name, default values is used.", new object[0]);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ConfigSettingNotFound, Thread.CurrentThread.Name, new object[]
				{
					text2
				});
				ExchangeComponent.name = "MessageTracing";
			}
			return ExchangeComponent.name;
		}

		private const string MessageTracingComponentName = "MessageTracing";

		private static string name;
	}
}
