using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingDeferAction : IErrorHandlingAction
	{
		public AgentErrorHandlingDeferAction(TimeSpan waitTime, bool isProgressive)
		{
			this.WaitTime = waitTime;
			this.IsWaitTimeProgressive = isProgressive;
		}

		public ErrorHandlingActionType ActionType
		{
			get
			{
				return ErrorHandlingActionType.Defer;
			}
		}

		public TimeSpan WaitTime { get; private set; }

		public bool IsWaitTimeProgressive { get; private set; }

		public SmtpResponse? SmtpResponse { get; set; }

		public void TakeAction(QueuedMessageEventSource source, MailItem mailItem)
		{
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper == null)
			{
				throw new ArgumentException("mailItem");
			}
			TransportMailItem transportMailItem = transportMailItemWrapper.TransportMailItem;
			int num = transportMailItem.ExtendedProperties.GetValue<int>("Microsoft.Exchange.Transport.AgentErrorDeferCount", 0) + 1;
			transportMailItem.ExtendedProperties.SetValue<int>("Microsoft.Exchange.Transport.AgentErrorDeferCount", num);
			SmtpResponse value;
			if (this.SmtpResponse != null)
			{
				value = this.SmtpResponse.Value;
			}
			else
			{
				value = new SmtpResponse("421", "4.7.11", "Message deferred by Agent error handling action", true, new string[]
				{
					string.Empty
				});
			}
			source.Defer(AgentErrorHandlingDeferAction.GetDeferInterval(this.WaitTime, num, this.IsWaitTimeProgressive), value);
		}

		internal static TimeSpan GetDeferInterval(TimeSpan waitTime, int deferCount, bool isWaitTimeProgressive)
		{
			if (isWaitTimeProgressive)
			{
				int num = (int)waitTime.TotalSeconds * deferCount;
				num += Convert.ToInt32(AgentErrorHandlingDeferAction.RandomGenerator.NextDouble() * 180.0);
				return TimeSpan.FromSeconds(Math.Min((double)num, AgentErrorHandlingDeferAction.MaxDeferralInterval.TotalSeconds));
			}
			return waitTime;
		}

		private const int DeferralRandomizationWindowInSeconds = 180;

		private static readonly TimeSpan MaxDeferralInterval = TimeSpan.FromDays(1.0);

		private static readonly Random RandomGenerator = new Random((int)DateTime.UtcNow.Ticks);
	}
}
