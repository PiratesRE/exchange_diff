using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class RemoteNotificationRequester
	{
		static RemoteNotificationRequester()
		{
			CertificateValidationManager.RegisterCallback("RemoteNotification", new RemoteCertificateValidationCallback(RemoteNotificationRequester.SslCertificateValidationCallback));
		}

		public static RemoteNotificationRequester Instance
		{
			get
			{
				if (RemoteNotificationRequester.instance == null)
				{
					RemoteNotificationRequester.instance = new RemoteNotificationRequester();
				}
				return RemoteNotificationRequester.instance;
			}
		}

		public int TotalInTrasitRequests
		{
			get
			{
				return this.totalInTrasitRequests;
			}
		}

		public virtual ManualResetEventSlim UnderRequestLimitEvent
		{
			get
			{
				return this.underRequestLimitEvent;
			}
		}

		public virtual async Task SendNotificationsAsync(PusherQueue queueToProcess)
		{
			bool success = false;
			try
			{
				await Task.Delay(TimeSpan.FromMinutes((double)queueToProcess.FailureCount));
				this.IncrementInTransit();
				PusherQueuePayload[] payloads = queueToProcess.GetPayloads();
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(queueToProcess.DestinationUrl));
				webRequest.PreAuthenticate = true;
				CertificateValidationManager.SetComponentId(webRequest, "RemoteNotification");
				webRequest.ContentType = "text/plain";
				webRequest.Method = "POST";
				DateTime pushTime = DateTime.UtcNow;
				await RemoteNotificationRequester.SendRequest(webRequest, RemoteNotificationRequester.GeneratePayload(payloads), queueToProcess.DestinationUrl, payloads.Length);
				await RemoteNotificationRequester.ReceiveResponseAsync(webRequest);
				success = true;
				IEnumerable<NotificationPayloadBase> payloadsToBePushed = from p in payloads
				select p.Payload;
				NotificationStatisticsManager.Instance.NotificationPushed(queueToProcess.DestinationUrl, payloadsToBePushed, pushTime);
			}
			catch (WebException webException)
			{
				RemoteNotificationRequester.HandleWebException(webException, queueToProcess);
			}
			finally
			{
				queueToProcess.SendComplete(success);
				this.DecrementInTransit();
			}
		}

		private static string GeneratePayload(PusherQueuePayload[] payloads)
		{
			RemoteNotificationPayload[] array = (from p in payloads
			select new RemoteNotificationPayload(1, JsonConverter.ToJSON(p.Payload), p.ChannelIds.ToArray<string>())).ToArray<RemoteNotificationPayload>();
			return JsonConverter.ToJSON(array);
		}

		private void DecrementInTransit()
		{
			int num = Interlocked.Decrement(ref this.totalInTrasitRequests);
			if (num < 50)
			{
				if (!this.underRequestLimitEvent.IsSet)
				{
					OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.ConcurrentLimit)
					{
						OverLimit = false,
						InTransitCount = num
					});
				}
				this.underRequestLimitEvent.Set();
			}
		}

		private void IncrementInTransit()
		{
			int num = Interlocked.Increment(ref this.totalInTrasitRequests);
			if (num >= 50)
			{
				if (this.underRequestLimitEvent.IsSet)
				{
					OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.ConcurrentLimit)
					{
						OverLimit = true,
						InTransitCount = num
					});
				}
				this.underRequestLimitEvent.Reset();
			}
		}

		private static async Task SendRequest(HttpWebRequest webRequest, string payload, string destination, int payloadCount)
		{
			OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.Push)
			{
				Destination = destination,
				PayloadCount = payloadCount
			});
			byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
			using (Stream requestStream = await webRequest.GetRequestStreamAsync())
			{
				requestStream.Write(payloadBytes, 0, payloadBytes.Length);
				requestStream.Close();
			}
		}

		private static async Task ReceiveResponseAsync(HttpWebRequest webRequest)
		{
			StringBuilder responseBuilder;
			using (WebResponse webResponse = await webRequest.GetResponseAsync())
			{
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					responseBuilder = await BufferedStreamReader.ReadAsync(responseStream);
					responseStream.Close();
					webResponse.Close();
				}
			}
			RemoteNotificationRequester.ProcessResponse(responseBuilder.ToString());
		}

		private static void ProcessResponse(string responseString)
		{
			if (!string.IsNullOrWhiteSpace(responseString))
			{
				string[] array = responseString.Split(RemoteNotificationRequester.ChannelIdSeparators, StringSplitOptions.None);
				foreach (string channelId in array)
				{
					RemoteNotificationManager.Instance.CleanUpChannel(channelId);
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>(0L, "Pusher removed explicitly lost channels. LostChannelIds: {0}", responseString);
			}
		}

		private static void HandleWebException(WebException webException, PusherQueue queue)
		{
			OwaServerTraceLogger.AppendToLog(new PusherLogEvent(PusherEventType.PushFailed)
			{
				Destination = queue.DestinationUrl,
				HandledException = webException
			});
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
			}
		}

		private static bool SslCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public const string ChannelIdsHeaderName = "CIDs";

		public const string NotificationsCountHeaderName = "NotifCount";

		public const string ChannelIdSeparator = ",";

		private const int MaxTotalInTransitRequests = 50;

		private const string CertificateValidationComponentId = "RemoteNotification";

		private static readonly string[] ChannelIdSeparators = new string[]
		{
			","
		};

		private static RemoteNotificationRequester instance;

		private int totalInTrasitRequests;

		public ManualResetEventSlim underRequestLimitEvent = new ManualResetEventSlim(true);
	}
}
