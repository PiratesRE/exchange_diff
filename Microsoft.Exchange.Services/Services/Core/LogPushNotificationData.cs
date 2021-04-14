using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class LogPushNotificationData : SingleStepServiceCommand<LogPushNotificationDataRequest, ServiceResultNone>
	{
		internal LogPushNotificationData(CallContext callContext, LogPushNotificationDataRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			this.ValidateRequest();
			Dictionary<string, string> dictionary = this.CreateDictionary();
			string owaDeviceId = base.CallContext.OwaDeviceId;
			string text = base.MailboxIdentityMailboxSession.MailboxGuid.ToString();
			if (string.Compare(base.Request.DataType, "BackgroundPushNotificationEvent", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.Compare(dictionary["BackgroundPushNotificationResult"], "Succeed", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dictionary["BackgroundPushNotificationResult"], "SyncSucceededWithNoItem", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dictionary["BackgroundPushNotificationResult"], "RichMailNotificationIsNotEnabled", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(dictionary["BackgroundPushNotificationResult"], "ForegroundNotification", StringComparison.OrdinalIgnoreCase) == 0)
				{
					PushNotificationsCrimsonEvents.BackgroundPushNotificationSuccess.Log<string, string, string, string, string, string, string, string, string, string, string>(base.Request.AppId, dictionary["ClientId"], dictionary["ServerId"], owaDeviceId, dictionary["StartTime"], text, dictionary["DeviceModel"], dictionary["PalVersion"], dictionary["OwaVersion"], dictionary["BackgroundPushNotificationResult"], dictionary["BackgroundSyncResult"]);
				}
				else
				{
					PushNotificationsCrimsonEvents.BackgroundPushNotificationWarning.LogPeriodic<string, string, string, string, string, string, string, string, string, string, string>(owaDeviceId + dictionary["BackgroundPushNotificationResult"], TimeSpan.FromMinutes(4.0), base.Request.AppId, dictionary["ClientId"], dictionary["ServerId"], owaDeviceId, dictionary["StartTime"], text, dictionary["DeviceModel"], dictionary["PalVersion"], dictionary["OwaVersion"], dictionary["BackgroundPushNotificationResult"], dictionary["BackgroundSyncResult"]);
				}
			}
			else if (string.Compare(base.Request.DataType, PushNotificationsCrimsonEvents.ContinuousRegistrationError.GetType().Name, StringComparison.OrdinalIgnoreCase) == 0)
			{
				PushNotificationsCrimsonEvents.ContinuousRegistrationError.LogPeriodic<string, string, string, string>(owaDeviceId, TimeSpan.FromMinutes(1440.0), base.Request.AppId, owaDeviceId, text, dictionary["Error"]);
			}
			else
			{
				if (string.Compare(base.Request.DataType, PushNotificationsCrimsonEvents.RegistrationError.GetType().Name, StringComparison.OrdinalIgnoreCase) != 0)
				{
					throw new InvalidRequestException();
				}
				PushNotificationsCrimsonEvents.RegistrationError.LogPeriodic<string, string, string, string>(owaDeviceId, TimeSpan.FromMinutes(1440.0), base.Request.AppId, owaDeviceId, text, dictionary["Error"]);
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			LogPushNotificationDataResponse logPushNotificationDataResponse = new LogPushNotificationDataResponse();
			logPushNotificationDataResponse.ProcessServiceResult(base.Result);
			return logPushNotificationDataResponse;
		}

		private void ValidateRequest()
		{
			if (string.IsNullOrEmpty(base.Request.AppId) || string.IsNullOrEmpty(base.Request.DataType))
			{
				throw new InvalidRequestException();
			}
			if (base.Request.KeyValuePairs == null || base.Request.KeyValuePairs.Length % 2 == 1)
			{
				throw new InvalidRequestException();
			}
		}

		private Dictionary<string, string> CreateDictionary()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < base.Request.KeyValuePairs.Length; i += 2)
			{
				dictionary.Add(base.Request.KeyValuePairs[i], base.Request.KeyValuePairs[i + 1]);
			}
			return dictionary;
		}

		private const int LogPeriodicSuppressionInMinutes = 1440;

		private const string ErrorKey = "Error";

		private const string ServerIdName = "ServerId";

		private const string BackgroundPushNotificationResultName = "BackgroundPushNotificationResult";

		private const string BackgroundSyncResultName = "BackgroundSyncResult";

		private const string StartTimeName = "StartTime";

		private const string DeviceModelName = "DeviceModel";

		private const string PalVersionName = "PalVersion";

		private const string OwaVersionName = "OwaVersion";

		private const string ClientIdName = "ClientId";

		private const string BackgroundPushNotificationEventName = "BackgroundPushNotificationEvent";
	}
}
