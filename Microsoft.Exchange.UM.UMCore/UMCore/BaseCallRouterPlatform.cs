using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;
using Microsoft.Exchange.UM.UMCore.OCS;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BaseCallRouterPlatform : DisposableBase
	{
		public BaseCallRouterPlatform(LocalizedString serviceName, LocalizedString serverName, UMADSettings config)
		{
			ValidateArgument.NotNull(serviceName, "ServiceName");
			ValidateArgument.NotNull(serverName, "ServerName");
			ValidateArgument.NotNull(config, "UMADSettings");
			this.serviceName = serviceName;
			this.serverName = serverName;
			this.config = config;
			switch (this.config.UMStartupMode)
			{
			case UMStartupMode.TCP:
				this.eventLogStringForMode = Strings.TCPOnly;
				this.eventLogStringForPorts = this.config.SipTcpListeningPort.ToString();
				break;
			case UMStartupMode.TLS:
				this.eventLogStringForMode = Strings.TLSOnly;
				this.eventLogStringForPorts = this.config.SipTlsListeningPort.ToString();
				break;
			case UMStartupMode.Dual:
				this.eventLogStringForMode = Strings.TCPnTLS;
				this.eventLogStringForPorts = Strings.Ports(this.config.SipTcpListeningPort, this.config.SipTlsListeningPort).ToString();
				break;
			default:
				throw new InvalidOperationException();
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRouterStartingMode, null, new object[]
			{
				this.eventLogStringForMode
			});
		}

		public abstract void StartListening();

		public abstract void StopListening();

		public abstract void ChangeCertificate();

		public abstract void SendPingAsync(PingInfo pingInfo, PingCompletedDelegate callBack);

		protected static void SetCallRejectionCounters(bool successRedirect)
		{
			if (!successRedirect)
			{
				Util.IncrementCounter(CallRouterAvailabilityCounters.UMCallRouterCallsRejected);
			}
			Util.SetCounter(CallRouterAvailabilityCounters.RecentUMCallRouterCallsRejected, (long)BaseCallRouterPlatform.recentPercentageRejectedCalls.Update(successRedirect));
		}

		protected void HandleMessageReceived(InfoMessage.PlatformMessageReceivedEventArgs e)
		{
			if (e.IsOptions)
			{
				this.HandleOptionsMessage(e);
				return;
			}
			this.HandleServiceRequest(e);
		}

		protected void HandleOptionsMessage(InfoMessage.PlatformMessageReceivedEventArgs e)
		{
			e.ResponseCode = 200;
		}

		protected void HandleServiceRequest(InfoMessage.PlatformMessageReceivedEventArgs e)
		{
			if (!CommonConstants.UseDataCenterCallRouting || string.IsNullOrEmpty(e.CallInfo.RemoteMatchedFQDN))
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.InvalidRequest, null, new object[0]);
			}
			using (CafeRoutingContext cafeRoutingContext = new CafeRoutingContext(e.CallInfo, this.config))
			{
				RouterCallHandler.HandleServiceRequest(cafeRoutingContext);
				ExAssert.RetailAssert(cafeRoutingContext.RedirectUri != null, "Redirection Uri has not been set");
				e.ResponseCode = cafeRoutingContext.RedirectCode;
				e.ResponseContactUri = cafeRoutingContext.RedirectUri;
			}
		}

		protected void HandleLegacyLyncNotification(PlatformCallInfo callInfo, byte[] messageBody, UserNotificationEventContext notificationContext)
		{
			if (CommonConstants.UseDataCenterCallRouting || string.IsNullOrEmpty(callInfo.RemoteMatchedFQDN))
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.InvalidRequest, null, new object[0]);
			}
			using (CafeRoutingContext cafeRoutingContext = new CafeRoutingContext(callInfo, this.config))
			{
				string text = UserNotificationEvent.ExtractEumProxyAddressFromXml(messageBody);
				notificationContext.User = text;
				EumAddress eumAddress = new EumAddress(ProxyAddress.Parse(text).AddressString);
				if (!eumAddress.IsSipExtension)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.InvalidSIPUris, "EumProxyAddress: {0}", new object[]
					{
						text
					});
				}
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(OrganizationId.ForestWideOrgId, null);
				using (UMRecipient umrecipient = UMRecipient.Factory.FromADRecipient<UMRecipient>(iadrecipientLookup.LookupByUmAddress(text)))
				{
					if (umrecipient == null || umrecipient.ADRecipient.UMRecipientDialPlanId == null)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MailboxIsNotUMEnabled, "User: {0}", new object[]
						{
							text
						});
					}
					if (umrecipient.RequiresLegacyRedirectForCallAnswering)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.NotificationNotSupportedForLegacyUser, "User: {0}", new object[]
						{
							text
						});
					}
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(umrecipient.ADRecipient);
					cafeRoutingContext.DialPlan = iadsystemConfigurationLookup.GetDialPlanFromId(umrecipient.ADRecipient.UMRecipientDialPlanId);
					if (cafeRoutingContext.DialPlan == null)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MailboxIsNotUMEnabled, "Dial Plan not found for User: {0}", new object[]
						{
							text
						});
					}
					RedirectionTarget.ResultSet forCallAnsweringCall = RedirectionTarget.Instance.GetForCallAnsweringCall(umrecipient, cafeRoutingContext);
					notificationContext.Backend = forCallAnsweringCall;
					string fromUri = string.Format(CultureInfo.InvariantCulture, "sip:{0}", new object[]
					{
						eumAddress.Extension
					});
					this.SendServiceRequest(fromUri, forCallAnsweringCall, messageBody);
				}
			}
		}

		protected abstract void SendServiceRequest(string fromUri, RedirectionTarget.ResultSet backendTarget, byte[] messageBody);

		protected LocalizedString serviceName;

		protected LocalizedString serverName;

		protected UMADSettings config;

		protected volatile bool isPlatformEnabled;

		protected string eventLogStringForPorts;

		protected LocalizedString eventLogStringForMode;

		private static PercentageBooleanSlidingCounter recentPercentageRejectedCalls = PercentageBooleanSlidingCounter.CreateFailureCounter(1000, TimeSpan.FromHours(1.0));
	}
}
