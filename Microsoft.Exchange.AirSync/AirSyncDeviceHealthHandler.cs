using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AirSyncDeviceHealthHandler : ExchangeDiagnosableWrapper<List<AirSyncDeviceHealth>>
	{
		protected override string UsageText
		{
			get
			{
				return "This diagnostics handler returns health of an EAS device or an user. The handler supports \"EmailAddress\" & \"DeviceID\" arguments. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return " Example 1: Returns health for all devices in cache\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncDeviceHealth\r\n\r\n                        Example 2: Returns health information about all devices/requests for specified user.\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncDeviceHealth -Argument \"EmailAddress=jondoe@contoso.com\";\r\n\r\n                        Example 2: Returns health information for specified device.\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncDeviceHealth -Argument \"DeviceID=WP986912973799292012\"";
			}
		}

		public static AirSyncDeviceHealthHandler GetInstance()
		{
			if (AirSyncDeviceHealthHandler.instance == null)
			{
				lock (AirSyncDeviceHealthHandler.lockObject)
				{
					if (AirSyncDeviceHealthHandler.instance == null)
					{
						AirSyncDeviceHealthHandler.instance = new AirSyncDeviceHealthHandler();
					}
				}
			}
			return AirSyncDeviceHealthHandler.instance;
		}

		private AirSyncDeviceHealthHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "AirSyncDeviceHealth";
			}
		}

		internal override List<AirSyncDeviceHealth> GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			List<AirSyncDeviceHealth> list = new List<AirSyncDeviceHealth>();
			string parsedArgument;
			CallType callType = DiagnosticsHelper.GetCallType(arguments.Argument, out parsedArgument);
			List<ActiveSyncRequestData> list2;
			if (string.IsNullOrEmpty(arguments.Argument))
			{
				list2 = (from request in ActiveSyncRequestCache.Instance.Values
				where request.UserEmail != null && request.DeviceID != null
				select request).ToList<ActiveSyncRequestData>();
			}
			else if (callType == CallType.EmailAddress)
			{
				list2 = (from request in ActiveSyncRequestCache.Instance.Values
				where string.Equals(request.UserEmail, parsedArgument, StringComparison.InvariantCultureIgnoreCase)
				select request).ToList<ActiveSyncRequestData>();
			}
			else
			{
				if (callType != CallType.DeviceId)
				{
					throw new ArgumentException("Invalid value found in 'Argument' parameter. please use ? as argument to see proper usage.");
				}
				list2 = (from request in ActiveSyncRequestCache.Instance.Values
				where string.Equals(request.DeviceID, parsedArgument, StringComparison.InvariantCultureIgnoreCase)
				select request).ToList<ActiveSyncRequestData>();
			}
			foreach (ActiveSyncRequestData data in list2)
			{
				list.Add(new AirSyncDeviceHealth(data));
			}
			return list;
		}

		private static AirSyncDeviceHealthHandler instance;

		private static object lockObject = new object();
	}
}
