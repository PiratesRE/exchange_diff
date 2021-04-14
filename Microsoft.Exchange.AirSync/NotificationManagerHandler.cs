using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class NotificationManagerHandler : ExchangeDiagnosableWrapper<NotificationManagerResult>
	{
		protected override string UsageText
		{
			get
			{
				return "The Notification Manager handler is a static diagnostics handler that dumps information about the notification manager and cache for EAS running processes.  \r\n                        It is implemented by the NotificationManagerHandler class, responds to the NotificationManager component (method) in Get-ExchangeDiagnosticInfo and returns NotificationManagerResults. \r\n                        Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Metadata only call\r\n                            Get-ExchangeDiagnosticInfo –Process MSExchangeSyncAppPool –Component NotificationManager\r\n                                            \r\n                            Example 2: Dump cache call\r\n                            Get-ExchangeDiagnosticInfo –Process MSExchangeSyncAppPool –Component NotificationManager –Argument “dumpcache” ";
			}
		}

		public static NotificationManagerHandler GetInstance()
		{
			if (NotificationManagerHandler.instance == null)
			{
				lock (NotificationManagerHandler.lockObject)
				{
					if (NotificationManagerHandler.instance == null)
					{
						NotificationManagerHandler.instance = new NotificationManagerHandler();
					}
				}
			}
			return NotificationManagerHandler.instance;
		}

		private NotificationManagerHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "NotificationManager";
			}
		}

		internal override NotificationManagerResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			string argument2;
			CallType callType = DiagnosticsHelper.GetCallType(argument.Argument, out argument2);
			return NotificationManager.GetDiagnosticInfo(callType, argument2);
		}

		private static NotificationManagerHandler instance;

		private static object lockObject = new object();
	}
}
