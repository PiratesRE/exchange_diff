using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationsOutboundTracer : IOutboundTracer
	{
		public PushNotificationsOutboundTracer(string tracerId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("tracerId", tracerId);
			this.TracerId = tracerId;
		}

		private string TracerId { get; set; }

		public void LogError(int hashCode, string formatString, params object[] args)
		{
			PushNotificationsCrimsonEvents.OutboundTracerError.Log<string, string>(this.TracerId, this.GenerateMessage(formatString, args));
		}

		public void LogInformation(int hashCode, string formatString, params object[] args)
		{
			if (PushNotificationsCrimsonEvents.OutboundTracerInformation.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.OutboundTracerInformation.Log<string, string>(this.TracerId, this.GenerateMessage(hashCode, formatString, args));
			}
		}

		public void LogToken(int hashCode, string tokenString)
		{
			if (PushNotificationsCrimsonEvents.OutboundTracerInformation.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.OutboundTracerInformation.Log<string, string>(this.TracerId, string.Format("Token:{0}", tokenString));
			}
		}

		public void LogWarning(int hashCode, string formatString, params object[] args)
		{
			PushNotificationsCrimsonEvents.OutboundTracerWarning.Log<string, string>(this.TracerId, this.GenerateMessage(formatString, args));
		}

		private string GenerateMessage(int hashCode, string formatString, params object[] args)
		{
			return string.Format("[{0}] {1}", hashCode, this.GenerateMessage(formatString, args));
		}

		private string GenerateMessage(string formatString, params object[] args)
		{
			string result;
			try
			{
				if (args == null || args.Length == 0)
				{
					result = formatString;
				}
				else
				{
					result = string.Format(formatString, args);
				}
			}
			catch (FormatException)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object arg in args)
				{
					stringBuilder.AppendFormat("{0};", arg);
				}
				result = string.Format("{0}, args:[{1}]", formatString, stringBuilder.ToString());
			}
			return result;
		}
	}
}
