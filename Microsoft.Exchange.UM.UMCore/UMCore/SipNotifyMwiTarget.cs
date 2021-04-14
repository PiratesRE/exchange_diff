using System;
using System.Globalization;
using System.Net.Mime;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SipNotifyMwiTarget : MwiTargetBase
	{
		public UMSipPeer Peer { get; private set; }

		internal SipNotifyMwiTarget(UMSipPeer peer, OrganizationId orgId) : base(peer.ToUMIPGateway(orgId), SipNotifyMwiTarget.instanceNameSuffix)
		{
			this.Peer = peer;
		}

		public override void SendMessageAsync(MwiMessage message)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, this.GetHashCode(), "SipNotifyMwiTarget.SendMessageAsync(message={0})", new object[]
			{
				message
			});
			ExAssert.RetailAssert(SipNotifyMwiTarget.initialized, "SipNotifyMwiTarget is not initialized!");
			base.SendMessageAsync(message);
			PlatformSipUri targetUri = this.GetTargetUri(message.UserExtension, this.Peer.Address.ToString(), this.Peer.Port);
			string text = string.Format(CultureInfo.InvariantCulture, "Messages-Waiting: {0}\r\nMessage-Account: {1}\r\nVoice-Message: {2}/{3}\r\n", new object[]
			{
				(message.UnreadVoicemailCount > 0) ? "yes" : "no",
				targetUri,
				message.UnreadVoicemailCount,
				message.TotalVoicemailCount - message.UnreadVoicemailCount
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, this.GetHashCode(), "SipNotifyMwiTarget.SendMessageAsync: sipUri={0} body={1}", new object[]
			{
				targetUri,
				text
			});
			PlatformSignalingHeader[] headers = new PlatformSignalingHeader[]
			{
				Platform.Builder.CreateSignalingHeader("Contact", string.Format(CultureInfo.InvariantCulture, "<{0}>", new object[]
				{
					targetUri.ToString()
				}))
			};
			UmServiceGlobals.VoipPlatform.SendNotifyMessageAsync(targetUri, this.Peer, SipNotifyMwiTarget.messageSummaryType, Encoding.UTF8.GetBytes(text), "message-summary", headers, message);
		}

		internal static void Initialize()
		{
			UmServiceGlobals.VoipPlatform.OnSendNotifyMessageCompleted += new VoipPlatformEventHandler<SendNotifyMessageCompletedEventArgs>(SipNotifyMwiTarget.SendNotifyMessageCompleted);
			SipNotifyMwiTarget.initialized = true;
		}

		internal static void Uninitialize()
		{
			if (SipNotifyMwiTarget.initialized)
			{
				UmServiceGlobals.VoipPlatform.OnSendNotifyMessageCompleted -= new VoipPlatformEventHandler<SendNotifyMessageCompletedEventArgs>(SipNotifyMwiTarget.SendNotifyMessageCompleted);
			}
		}

		private static void SendNotifyMessageCompleted(object sender, SendNotifyMessageCompletedEventArgs args)
		{
			MwiMessage mwiMessage = args.UserState as MwiMessage;
			if (mwiMessage != null)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(mwiMessage.EventTimeUtc);
				CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, (sender == null) ? 0 : sender.GetHashCode(), "SipNotifyMwiTarget.SendNotifyMessageCompleted(Message={0}. Latency={1}. Error={2})", new object[]
				{
					mwiMessage,
					timeSpan.TotalMilliseconds,
					args.Error
				});
				if (args.ResponseCode == 603 && Utils.IsUriValid(mwiMessage.UserExtension, UMUriType.SipName))
				{
					args.Error = null;
					CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, 0, "Ignoring 603 error for {0}.", new object[]
					{
						mwiMessage.UserExtension
					});
				}
				if (mwiMessage.CompletionCallback != null)
				{
					MwiDeliveryException error = null;
					if (args.Error != null)
					{
						error = new MwiTargetException(mwiMessage.CurrentTarget.Name, args.ResponseCode, args.ResponseReason ?? args.Error.Message, args.Error);
					}
					else
					{
						MwiDiagnostics.SetCounterValue(GeneralCounters.AverageMWILatency, SipNotifyMwiTarget.averageTotalMwiLatency.Update(timeSpan.TotalMilliseconds));
					}
					SipNotifyMwiTarget sipNotifyMwiTarget = (SipNotifyMwiTarget)mwiMessage.CurrentTarget;
					sipNotifyMwiTarget.UpdatePerformanceCounters(mwiMessage, error);
					mwiMessage.CompletionCallback(mwiMessage, error);
				}
			}
		}

		private PlatformSipUri GetTargetUri(string extension, string host, int port)
		{
			PlatformSipUri platformSipUri;
			if (Utils.IsUriValid(extension, UMUriType.SipName))
			{
				platformSipUri = Platform.Builder.CreateSipUri("sip:" + extension);
				platformSipUri.UserParameter = UserParameter.None;
			}
			else
			{
				platformSipUri = Platform.Builder.CreateSipUri(SipUriScheme.Sip, extension, host);
				platformSipUri.UserParameter = UserParameter.Phone;
			}
			platformSipUri.Port = port;
			return platformSipUri;
		}

		private const string MwiBodyFormat = "Messages-Waiting: {0}\r\nMessage-Account: {1}\r\nVoice-Message: {2}/{3}\r\n";

		private const string MwiYes = "yes";

		private const string MwiNo = "no";

		private const string MwiMessageSummaryHeader = "message-summary";

		private static System.Net.Mime.ContentType messageSummaryType = new System.Net.Mime.ContentType("application/simple-message-summary");

		private static MovingAverage averageTotalMwiLatency = new MovingAverage(50);

		private static string instanceNameSuffix = typeof(UMIPGateway).Name;

		private static bool initialized;
	}
}
