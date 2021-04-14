using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal sealed class SharedMailboxSentItemsAgent : StoreDriverDeliveryAgent
	{
		public SharedMailboxSentItemsAgent(IPerformanceCountersFactory perfCountersFactory, ILogger logger)
		{
			ArgumentValidator.ThrowIfNull("perfCountersFactory", perfCountersFactory);
			ArgumentValidator.ThrowIfNull("logger", logger);
			base.Name = "SharedMailboxSentItemsAgent";
			this.perfCountersFactory = perfCountersFactory;
			this.logger = logger;
			base.OnPromotedMessage += this.OnPromotedMessageImplementation;
		}

		internal void ProcessSentItemWrapperMessage(EmailMessage message, string mdbGuid, MessageItem replayItem, IStoreOperations storeOperations, IAgentInfoWriter agentInfo)
		{
			if (!this.IsSharedMailboxSentItemMessage(message, agentInfo))
			{
				return;
			}
			IPerformanceCounters counterInstance = this.perfCountersFactory.GetCounterInstance(mdbGuid);
			bool flag = true;
			try
			{
				MessageItem attachedMessageItem = this.GetAttachedMessageItem(replayItem, agentInfo);
				if (attachedMessageItem == null)
				{
					this.logger.TraceDebug(new string[]
					{
						"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Message has the correct header tags but does not contain the original email attachment"
					});
				}
				else
				{
					string text = attachedMessageItem.TryGetProperty(ItemSchema.InternetMessageId) as string;
					if (text != null && storeOperations.MessageExistsInSentItems(text))
					{
						this.logger.TraceDebug(new string[]
						{
							"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: message already exits in the sent items folder. Don't have to copy."
						});
					}
					else
					{
						this.logger.TraceDebug(new string[]
						{
							"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Get the sent items folder Id."
						});
						storeOperations.CopyAttachmentToSentItemsFolder(attachedMessageItem);
						this.UpdateAverageSentItemCopyTimePerfCounter(attachedMessageItem, counterInstance);
						counterInstance.IncrementSentItemsMessages();
					}
				}
			}
			catch (StorageTransientException ex)
			{
				flag = false;
				counterInstance.IncrementErrors();
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler encountered an exception: ",
					ex.ToString()
				});
				this.logger.LogEvent(MailboxTransportEventLogConstants.Tuple_SharedMailboxSentItemsAgentException, ex);
				throw new SmtpResponseException(SharedMailboxSentItemsAgent.CopyFailedTransientError);
			}
			catch (Exception ex2)
			{
				flag = false;
				counterInstance.IncrementErrors();
				if (ex2 is OutOfMemoryException || ex2 is StackOverflowException || ex2 is ThreadAbortException)
				{
					throw;
				}
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler encountered an exception:",
					ex2.ToString()
				});
				this.logger.LogEvent(MailboxTransportEventLogConstants.Tuple_SharedMailboxSentItemsAgentException, ex2);
				throw new SmtpResponseException(SharedMailboxSentItemsAgent.CopyFailedPermanentError);
			}
			finally
			{
				if (flag)
				{
					this.logger.TraceDebug(new string[]
					{
						"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Copy operation complete. Going to raise success exception to inform delivery service to drop this message."
					});
					throw new SmtpResponseException(SharedMailboxSentItemsAgent.SharedMailboxSentItemCopySuccess, "SharedMailboxSentItemsAgent");
				}
			}
		}

		private static bool IsSharedMailboxSentItemsDeliveryAgentFeatureEnabled(MiniRecipient recipient)
		{
			return recipient != null && VariantConfiguration.GetSnapshot(recipient.GetContext(null), null, null).SharedMailbox.SharedMailboxSentItemsDeliveryAgent.Enabled;
		}

		private void OnPromotedMessageImplementation(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			this.logger.TraceDebug(new string[]
			{
				"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: entering."
			});
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = args as StoreDriverDeliveryEventArgsImpl;
			if (storeDriverDeliveryEventArgsImpl == null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: args is null or not a StoreDriverDeliveryEventArgsImpl; exiting."
				});
				return;
			}
			if (!SharedMailboxSentItemsAgent.IsSharedMailboxSentItemsDeliveryAgentFeatureEnabled(storeDriverDeliveryEventArgsImpl.MailboxOwner))
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Flight for the feature is not enabled; exiting."
				});
				return;
			}
			if (storeDriverDeliveryEventArgsImpl.MailboxSession == null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Mailbox session is null; exiting."
				});
				return;
			}
			if (args.MailItem.Message == null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Message is null; exiting."
				});
				return;
			}
			if (storeDriverDeliveryEventArgsImpl.ReplayItem == null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: ReplayItem is null; exiting."
				});
				return;
			}
			EmailMessage message = args.MailItem.Message;
			MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
			MessageItem replayItem = storeDriverDeliveryEventArgsImpl.ReplayItem;
			string mdbGuid = storeDriverDeliveryEventArgsImpl.MailboxSession.MdbGuid.ToString();
			this.ProcessSentItemWrapperMessage(message, mdbGuid, replayItem, new StoreOperations(mailboxSession, this.logger), new AgentInfoWriter(args, "SharedMailboxSentItemsAgent"));
		}

		private void UpdateAverageSentItemCopyTimePerfCounter(Item attachedMessageItem, IPerformanceCounters performanceCounter)
		{
			ExDateTime? exDateTime = attachedMessageItem.TryGetProperty(ItemSchema.SentTime) as ExDateTime?;
			if (exDateTime != null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Updating the average delivery time for for the message copy."
				});
				TimeSpan timeSpan = DateTime.UtcNow - exDateTime.Value.UniversalTime;
				performanceCounter.UpdateAverageMessageCopyTime(timeSpan);
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: Message delivery time is:" + timeSpan
				});
			}
		}

		private bool IsSharedMailboxSentItemMessage(EmailMessage message, IAgentInfoWriter agentInfo)
		{
			Header header = message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-SharedMailbox-SentItem-Message");
			if (header == null || string.IsNullOrWhiteSpace(header.Value))
			{
				this.logger.TraceDebug(new string[]
				{
					"Could not find the expected message header."
				});
				return false;
			}
			bool flag;
			if (!bool.TryParse(header.Value, out flag) || !flag)
			{
				this.logger.TraceDebug(new string[]
				{
					"Found the header but the value does not match the expected value. Expected: True, Actual ",
					header.Value
				});
				return false;
			}
			string text = "Found the message header with key X-MS-Exchange-SharedMailbox-SentItem-Message value " + header.Value + ". This message will be droped.";
			this.logger.TraceDebug(new string[]
			{
				text
			});
			agentInfo.AddAgentInfo("MessageFlagCheck", text);
			return true;
		}

		private MessageItem GetAttachedMessageItem(MessageItem message, IAgentInfoWriter agentInfo)
		{
			foreach (AttachmentHandle handle in message.AttachmentCollection.GetHandles())
			{
				using (Attachment attachment = message.AttachmentCollection.Open(handle))
				{
					ItemAttachment itemAttachment = attachment as ItemAttachment;
					if (itemAttachment != null)
					{
						return itemAttachment.GetItemAsMessage();
					}
					string text = "Warning: Message does not contain the expected attachment.";
					this.logger.TraceDebug(new string[]
					{
						text
					});
					agentInfo.AddAgentInfo("AttachmentCheck", text);
				}
			}
			return null;
		}

		private const string AgentName = "SharedMailboxSentItemsAgent";

		private static readonly SmtpResponse SharedMailboxSentItemCopySuccess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"SharedMailboxSentItemsAgent; Sent item copy to shared mailbox successfully."
		});

		private static readonly SmtpResponse CopyFailedTransientError = new SmtpResponse("432", "4.3.2", new string[]
		{
			"SharedMailboxSentItemsAgent; Sent items copy failed due to a transient error"
		});

		private static readonly SmtpResponse CopyFailedPermanentError = new SmtpResponse("550", "5.7.103", new string[]
		{
			"SharedMailboxSentItemsAgent; Sent items copy failed due to a permanent error"
		});

		private readonly IPerformanceCountersFactory perfCountersFactory;

		private readonly ILogger logger;
	}
}
