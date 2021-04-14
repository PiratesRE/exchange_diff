using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	internal sealed class SharedMailboxSentItemsRoutingAgent : RoutingAgent
	{
		internal SharedMailboxSentItemsRoutingAgent(ISharedMailboxConfigurationFactory configurationFactory, ISentItemWrapperCreator wrapperCreator, ITracer tracer)
		{
			if (configurationFactory == null)
			{
				throw new ArgumentNullException("configurationFactory");
			}
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			if (wrapperCreator == null)
			{
				throw new ArgumentNullException("wrapperCreator");
			}
			this.tracer = tracer;
			this.wrapperCreator = wrapperCreator;
			this.traceId = this.GetHashCode();
			this.configurationFactory = configurationFactory;
			base.OnSubmittedMessage += this.OnSubmittedMessageHandler;
		}

		internal void OnSubmittedMessageHandler(SubmittedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).SharedMailbox.SharedMailboxSentItemsRoutingAgent.Enabled)
			{
				this.tracer.TraceDebug((long)this.traceId, "SharedMailboxSentItemsRoutingAgent flight is not enabled. Exiting.");
				return;
			}
			this.HandleSubmittedMessage(new EventSource(source), args.MailItem);
		}

		internal void HandleSubmittedMessage(IEventSource source, MailItem mailItem)
		{
			if (mailItem == null || mailItem.Message == null || mailItem.Message.MapiMessageClass == null)
			{
				this.tracer.TraceDebug((long)this.traceId, "Cannot find the message class for the current message. Exiting.");
				return;
			}
			if (!ObjectClass.IsOfClass(mailItem.Message.MapiMessageClass, "IPM.Note"))
			{
				this.tracer.TraceDebug((long)this.traceId, "Current message is not Ipmnote. Exiting.");
				return;
			}
			if (mailItem.Message != null && mailItem.Message.RootPart != null && mailItem.Message.RootPart.Headers != null)
			{
				Header header = mailItem.Message.RootPart.Headers.FindFirst("X-MS-Exchange-SharedMailbox-RoutingAgent-Processed");
				if (header != null && string.Equals("True", header.Value, StringComparison.OrdinalIgnoreCase))
				{
					this.tracer.TraceDebug((long)this.traceId, "Message has been already been processed by the agent. Exiting.");
					return;
				}
			}
			int messageRetryCount = SharedMailboxSentItemsRoutingAgent.GetMessageRetryCount(mailItem.Properties);
			try
			{
				if (!this.ShouldCopyMessageToSentItemsOfSharedMailbox(mailItem))
				{
					this.tracer.TraceDebug((long)this.traceId, "Configuration indicates that the message need not be copied to the shared mailbox. Exiting.");
				}
				else
				{
					Exception ex = this.wrapperCreator.CreateAndSubmit(mailItem, this.traceId);
					if (ex != null)
					{
						this.tracer.TraceError((long)this.traceId, "error occured in sending the wrapper message:" + ex);
						SharedMailboxSentItemsRoutingAgent.DeferMessageForRetry(source, mailItem, messageRetryCount, SharedMailboxSentItemsRoutingAgent.FailedSendMessage);
					}
					else
					{
						mailItem.Message.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-SharedMailbox-RoutingAgent-Processed", "True"));
					}
				}
			}
			catch (Exception ex2)
			{
				if (ex2 is OutOfMemoryException || ex2 is StackOverflowException || ex2 is ThreadAbortException)
				{
					throw;
				}
				this.tracer.TraceError((long)this.traceId, "Unhandled exception occured :" + ex2);
				SharedMailboxSentItemsRoutingAgent.DeferMessageForRetry(source, mailItem, messageRetryCount, SharedMailboxSentItemsRoutingAgent.ErrorOccuredInProcessing);
			}
		}

		private static int GetMessageRetryCount(IDictionary<string, object> properties)
		{
			object obj;
			if (properties.TryGetValue("SharedMailboxSentItemsRoutingAgent.RetryCount", out obj) && obj is int)
			{
				return (int)obj;
			}
			return 0;
		}

		private static void DeferMessageForRetry(IEventSource source, MailItem mailItem, int currentMessageRetryCount, SmtpResponse reason)
		{
			if (currentMessageRetryCount < 12)
			{
				mailItem.Properties["SharedMailboxSentItemsRoutingAgent.RetryCount"] = currentMessageRetryCount + 1;
				source.Defer(SharedMailboxSentItemsRoutingAgent.DeferTimeout, reason);
			}
		}

		private MessageSentRepresentingFlags GetMessageSentRepresentingType(EmailMessage emailMessage)
		{
			if (emailMessage == null || emailMessage.RootPart == null || emailMessage.RootPart.Headers == null)
			{
				return MessageSentRepresentingFlags.None;
			}
			Header header = emailMessage.RootPart.Headers.FindFirst("X-MS-Exchange-MessageSentRepresentingType");
			if (header == null || string.IsNullOrEmpty(header.Value))
			{
				return MessageSentRepresentingFlags.None;
			}
			int num;
			if (!int.TryParse(header.Value, out num))
			{
				return MessageSentRepresentingFlags.None;
			}
			MessageSentRepresentingFlags messageSentRepresentingFlags = (MessageSentRepresentingFlags)num;
			if (!Enum.IsDefined(typeof(MessageSentRepresentingFlags), messageSentRepresentingFlags))
			{
				return MessageSentRepresentingFlags.None;
			}
			return messageSentRepresentingFlags;
		}

		private bool ShouldCopyMessageToSentItemsOfSharedMailbox(MailItem transportMailItem)
		{
			MessageSentRepresentingFlags messageSentRepresentingType = this.GetMessageSentRepresentingType(transportMailItem.Message);
			if (messageSentRepresentingType == MessageSentRepresentingFlags.None)
			{
				this.tracer.TraceDebug((long)this.traceId, "Message is not sent as or on-behalf of another user.");
				return false;
			}
			this.tracer.TraceDebug((long)this.traceId, "Message is sent as another user. SentRepresentingFlags value : " + messageSentRepresentingType.ToString());
			SharedMailboxConfiguration sharedMailboxConfiguration = this.configurationFactory.GetSharedMailboxConfiguration(transportMailItem, transportMailItem.FromAddress.ToString());
			if (!sharedMailboxConfiguration.IsSharedMailbox)
			{
				this.tracer.TraceDebug((long)this.traceId, "Message sender is not a shared mailbox.");
				return false;
			}
			this.tracer.TraceDebug((long)this.traceId, string.Concat(new object[]
			{
				"Sharedmailbox sent item configuration. SentAsBehavior:",
				sharedMailboxConfiguration.SentAsBehavior,
				" SentOnBehalfOfBehavior:",
				sharedMailboxConfiguration.SentOnBehalfOfBehavior
			}));
			return (messageSentRepresentingType != MessageSentRepresentingFlags.SendAs || sharedMailboxConfiguration.SentAsBehavior == SharedMailboxSentItemBehavior.CopyToSharedMailbox) && (messageSentRepresentingType != MessageSentRepresentingFlags.SendOnBehalfOf || sharedMailboxConfiguration.SentOnBehalfOfBehavior == SharedMailboxSentItemBehavior.CopyToSharedMailbox);
		}

		internal const string AgentName = "SharedMailboxSentItemsRoutingAgent";

		internal const string RetryCountKey = "SharedMailboxSentItemsRoutingAgent.RetryCount";

		internal const string AgentProcessedHeaderValue = "True";

		private const int MaxRetryCount = 12;

		private static readonly TimeSpan DeferTimeout = TimeSpan.FromMinutes(5.0);

		private static readonly SmtpResponse FailedSendMessage = new SmtpResponse("452", "4.3.2", new string[]
		{
			"Failed to send the message"
		});

		private static readonly SmtpResponse ErrorOccuredInProcessing = new SmtpResponse("452", "4.3.2", new string[]
		{
			"Failed to process the message. Should retry."
		});

		private readonly ITracer tracer;

		private readonly ISharedMailboxConfigurationFactory configurationFactory;

		private readonly ISentItemWrapperCreator wrapperCreator;

		private readonly int traceId;
	}
}
