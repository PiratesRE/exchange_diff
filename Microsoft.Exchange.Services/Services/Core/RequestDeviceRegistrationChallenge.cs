using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class RequestDeviceRegistrationChallenge : SingleStepServiceCommand<DeviceRegistrationChallengeRequest, ServiceResultNone>
	{
		static RequestDeviceRegistrationChallenge()
		{
			RequestDeviceRegistrationChallenge.ConfigWatcher.OnChangeEvent += RequestDeviceRegistrationChallenge.UpdateConfigurationData;
			PushNotificationPublisherConfiguration pushNotificationPublisherConfiguration = RequestDeviceRegistrationChallenge.ConfigWatcher.Start();
			RequestDeviceRegistrationChallenge.enabledApps = RequestDeviceRegistrationChallenge.ResolveAzureMultifactorEnabledApps(pushNotificationPublisherConfiguration.AzurePublisherSettings);
		}

		public RequestDeviceRegistrationChallenge(CallContext callContext, DeviceRegistrationChallengeRequest request) : base(callContext, request)
		{
			this.ChallengeRequest = request;
		}

		private DeviceRegistrationChallengeRequest ChallengeRequest { get; set; }

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new RequestDeviceRegistrationChallengeResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			string appId = this.ChallengeRequest.AppId;
			bool? flag = RequestDeviceRegistrationChallenge.IsMultifactorRegistrationEnabledForApp(appId);
			if (flag == null || !flag.Value)
			{
				if (flag == null)
				{
					PushNotificationsCrimsonEvents.MultifactorRegistrationUnknownApp.LogPeriodic<string, string>(appId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, appId, this.ChallengeRequest.DeviceNotificationId);
				}
				else if (PushNotificationsCrimsonEvents.MultifactorRegistrationUnavailable.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.MultifactorRegistrationUnavailable.Log<string, string>(appId, this.ChallengeRequest.DeviceNotificationId);
				}
				ServiceError error = new ServiceError(CoreResources.MultifactorRegistrationUnavailable(appId), ResponseCodeType.ErrorMultifactorRegistrationUnavailable, 0, ExchangeVersion.Exchange2013);
				return new ServiceResult<ServiceResultNone>(error);
			}
			string hubName = null;
			OrganizationId organizationId = base.MailboxIdentityMailboxSession.OrganizationId;
			if (!OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				hubName = base.MailboxIdentityMailboxSession.OrganizationId.ToExternalDirectoryOrganizationId();
			}
			using (AzureChallengeRequestServiceProxy azureChallengeRequestServiceProxy = new AzureChallengeRequestServiceProxy(null))
			{
				string challenge = (!string.IsNullOrWhiteSpace(this.ChallengeRequest.ClientWatermark)) ? this.ChallengeRequest.ClientWatermark : appId;
				azureChallengeRequestServiceProxy.EndChallengeRequest(azureChallengeRequestServiceProxy.BeginChallengeRequest(new AzureChallengeRequestInfo(appId, this.ChallengeRequest.Platform, this.ChallengeRequest.DeviceNotificationId, challenge, hubName), null, null));
				if (PushNotificationsCrimsonEvents.DeviceRegistrationChallengeRequested.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.DeviceRegistrationChallengeRequested.Log<string, string>(this.ChallengeRequest.AppId, this.ChallengeRequest.DeviceNotificationId);
				}
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private static bool? IsMultifactorRegistrationEnabledForApp(string appId)
		{
			Dictionary<string, bool> dictionary = Interlocked.CompareExchange<Dictionary<string, bool>>(ref RequestDeviceRegistrationChallenge.enabledApps, RequestDeviceRegistrationChallenge.enabledApps, null);
			if (!dictionary.ContainsKey(appId))
			{
				return null;
			}
			return new bool?(dictionary[appId]);
		}

		private static void UpdateConfigurationData(object sender, ConfigurationChangedEventArgs configEventArgs)
		{
			Interlocked.Exchange<Dictionary<string, bool>>(ref RequestDeviceRegistrationChallenge.enabledApps, RequestDeviceRegistrationChallenge.ResolveAzureMultifactorEnabledApps(configEventArgs.UpdatedConfiguration.AzurePublisherSettings));
		}

		private static Dictionary<string, bool> ResolveAzureMultifactorEnabledApps(IEnumerable<AzurePublisherSettings> publisherSettings)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (AzurePublisherSettings azurePublisherSettings in publisherSettings)
			{
				dictionary[azurePublisherSettings.AppId] = azurePublisherSettings.IsMultifactorRegistrationEnabled;
			}
			if (PushNotificationsCrimsonEvents.MultifactorRegistrationApps.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (string text in dictionary.Keys)
				{
					if (dictionary[text])
					{
						stringBuilder.Append(text);
						stringBuilder.Append(",");
					}
					else
					{
						stringBuilder2.Append(text);
						stringBuilder2.Append(",");
					}
				}
				PushNotificationsCrimsonEvents.MultifactorRegistrationApps.Log<string>(string.Format("Enabled:[{0}]; Disabled:[{1}]", stringBuilder.ToString(), stringBuilder2.ToString()));
			}
			return dictionary;
		}

		private const int DefaultConfigWatcherRefreshRateInMinutes = 15;

		private const string DefaultServiceCommandAppPoolName = "msexchangeservicesapppool";

		private static readonly PublisherConfigurationWatcher ConfigWatcher = new PublisherConfigurationWatcher("msexchangeservicesapppool", 15);

		private static Dictionary<string, bool> enabledApps;
	}
}
