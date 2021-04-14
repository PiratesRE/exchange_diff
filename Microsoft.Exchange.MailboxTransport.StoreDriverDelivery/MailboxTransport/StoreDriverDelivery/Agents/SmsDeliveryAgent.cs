using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Agents;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class SmsDeliveryAgent : StoreDriverDeliveryAgent
	{
		public SmsDeliveryAgent()
		{
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		private void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = args as StoreDriverDeliveryEventArgsImpl;
			if (storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient || !ObjectClass.IsSmsMessage(storeDriverDeliveryEventArgsImpl.MessageClass))
			{
				return;
			}
			SmsDeliveryAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Processing incoming message");
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				MimePart rootPart = storeDriverDeliveryEventArgsImpl.MailItem.Message.RootPart;
				Header header = rootPart.Headers.FindFirst("X-MS-Reply-To-Mobile");
				if (header == null || string.IsNullOrEmpty(header.Value))
				{
					SmsDeliveryAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "There's no {0} header or the header value is empty in MimePart of the inbound SMS message", "X-MS-Reply-To-Mobile");
				}
				else
				{
					string value = header.Value;
					string displayName = value;
					using (SmsRecipientInfoCache smsRecipientInfoCache = new SmsRecipientInfoCache(storeDriverDeliveryEventArgsImpl.MailboxSession, SmsDeliveryAgent.Tracer))
					{
						RecipientInfoCacheEntry recipientInfoCacheEntry = smsRecipientInfoCache.LookUp(value);
						if (recipientInfoCacheEntry != null)
						{
							displayName = recipientInfoCacheEntry.DisplayName;
						}
					}
					storeDriverDeliveryEventArgsImpl.ReplayItem.From = new Participant(displayName, value, "MOBILE");
					SmsDeliveryAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Found entry from SMS recipient which matches");
					string text = storeDriverDeliveryEventArgsImpl.ReplayItem.Body.GetPartialTextBody(160);
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Replace("\r\n", " ").Trim();
					}
					storeDriverDeliveryEventArgsImpl.ReplayItem.Subject = (text ?? string.Empty);
				}
			}
			finally
			{
				stopwatch.Stop();
				MSExchangeInboundSmsDelivery.MessageReceived.Increment();
				if (MSExchangeInboundSmsDelivery.MaximumMessageProcessingTime.RawValue < stopwatch.ElapsedMilliseconds)
				{
					MSExchangeInboundSmsDelivery.MaximumMessageProcessingTime.RawValue = stopwatch.ElapsedMilliseconds;
				}
				SmsDeliveryAgent.averageMessageProcessingTime.Update(stopwatch.ElapsedMilliseconds);
				SmsDeliveryAgent.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Exiting ConversationsProcessing.ProcessMessage.  Total execution time = {0} ms.", stopwatch.ElapsedMilliseconds);
			}
		}

		private const string MobileRoutingType = "MOBILE";

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.SmsDeliveryAgentTracer;

		private static AveragePerformanceCounterWrapper averageMessageProcessingTime = new AveragePerformanceCounterWrapper(MSExchangeInboundSmsDelivery.AverageMessageProcessingTime);
	}
}
