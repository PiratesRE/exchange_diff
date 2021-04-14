using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class RemoteNotificationHandler : HttpTaskAsyncHandler
	{
		private bool Initialize(HttpResponse response)
		{
			response.ContentType = "text/plain";
			response.Cache.SetNoServerCaching();
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetNoStore();
			return true;
		}

		public override async Task ProcessRequestAsync(HttpContext context)
		{
			Exception handledException = null;
			try
			{
				OwaServerTraceLogger.AppendToLog(new ListenerLogEvent(ListenerEventType.Listen)
				{
					OriginationServer = context.Request.UserHostName
				});
				RemoteNotificationPayload[] payloads = await this.ReadRemoteNotificationPayloadFromRequestAsync(context.Request);
				ListenerDelivery listenerDelivery = new ListenerDelivery(ListenerChannelsManager.Instance);
				HashSet<string> notFoundChannelIds = new HashSet<string>();
				foreach (RemoteNotificationPayload remoteNotificationPayload in payloads)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "Listener delivering notificatios to RemoteNotifiers. Origination Server: {0}; Channel Ids: {1}; Payload Count: {2}", context.Request.UserHostName, string.Join(",", remoteNotificationPayload.ChannelIds), remoteNotificationPayload.NotificationsCount);
					notFoundChannelIds.UnionWith(listenerDelivery.DeliverRemoteNotification(remoteNotificationPayload.ChannelIds, remoteNotificationPayload));
				}
				this.WriteNotFoundChannelsToResponse(context.Response, notFoundChannelIds);
			}
			catch (OwaInvalidRequestException ex)
			{
				context.Response.StatusCode = 400;
				handledException = ex;
			}
			catch (Exception ex2)
			{
				context.Response.StatusCode = 500;
				handledException = ex2;
			}
			if (handledException != null)
			{
				OwaServerTraceLogger.AppendToLog(new ListenerLogEvent(ListenerEventType.ListenFailed)
				{
					OriginationServer = context.Request.UserHostName,
					HandledException = handledException
				});
			}
		}

		private async Task<RemoteNotificationPayload[]> ReadRemoteNotificationPayloadFromRequestAsync(HttpRequest request)
		{
			StringBuilder requestBuilder;
			using (Stream requestStream = request.GetBufferedInputStream())
			{
				requestBuilder = await BufferedStreamReader.ReadAsync(requestStream);
				requestStream.Close();
			}
			string payloadString = requestBuilder.ToString();
			if (string.IsNullOrEmpty(payloadString))
			{
				throw new OwaInvalidRequestException("RemoteNotificationHandler: The request contains an empty payload.");
			}
			RemoteNotificationPayload[] result;
			try
			{
				RemoteNotificationPayload[] array = JsonConverter.FromJSON<RemoteNotificationPayload[]>(payloadString);
				RemoteNotificationHandler.ValidatePayload(array);
				foreach (RemoteNotificationPayload remoteNotificationPayload in array)
				{
					remoteNotificationPayload.Source = new ServerLocation(request.UserHostName ?? request.UserHostAddress);
				}
				result = array;
			}
			catch (SerializationException innerException)
			{
				throw new OwaInvalidRequestException("RemoteNotificationHandler: Failed to deserialize Json string to RemoteNotificationPayload array.", innerException);
			}
			return result;
		}

		private static void ValidatePayload(RemoteNotificationPayload[] payloads)
		{
			if (payloads == null || payloads.Length == 0)
			{
				throw new OwaInvalidRequestException("RemoteNotificationHandler: The request contains no remote payloads");
			}
			for (int i = 0; i < payloads.Length; i++)
			{
				RemoteNotificationPayload remoteNotificationPayload = payloads[i];
				if (remoteNotificationPayload == null)
				{
					throw new OwaInvalidRequestException(string.Format("RemoteNotificationHandler: Payload at position {0} is null", i));
				}
				if (remoteNotificationPayload.ChannelIds == null || remoteNotificationPayload.ChannelIds.Length == 0)
				{
					throw new OwaInvalidRequestException(string.Format("RemoteNotificationHandler: Payload at position {0} has invalid ChannelIds", i));
				}
				if (remoteNotificationPayload.NotificationsCount <= 0)
				{
					throw new OwaInvalidRequestException(string.Format("RemoteNotificationHandler: Payload at position {0} has invalid NotificationCount", i));
				}
				if (string.IsNullOrEmpty(remoteNotificationPayload.RemotePayload))
				{
					throw new OwaInvalidRequestException(string.Format("RemoteNotificationHandler: Payload at position {0} has invalid RemotePayload", i));
				}
			}
		}

		private void WriteNotFoundChannelsToResponse(HttpResponse response, ICollection<string> notFoundChannelIds)
		{
			if (this.Initialize(response) && notFoundChannelIds != null && notFoundChannelIds.Count > 0)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Listener could not deliver to following channels. LostChannelIds: {0}", string.Join(",", notFoundChannelIds));
				response.Write(string.Join(",", notFoundChannelIds));
			}
		}

		private static readonly char[] CommaSeparator = new char[]
		{
			','
		};
	}
}
