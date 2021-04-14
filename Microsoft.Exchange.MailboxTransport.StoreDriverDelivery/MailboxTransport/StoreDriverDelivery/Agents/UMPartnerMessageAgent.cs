using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.TextProcessing.Boomerang;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class UMPartnerMessageAgent : StoreDriverDeliveryAgent
	{
		public UMPartnerMessageAgent(SmtpServer server)
		{
			this.server = (server as StoreDriverServer);
			if (this.server == null)
			{
				throw new ArgumentException("The instance of the SmtpServer is not of the expected type.", "server");
			}
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)e;
			if (!storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient && !storeDriverDeliveryEventArgsImpl.IsJournalReport && UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsMessageInteresting(storeDriverDeliveryEventArgsImpl))
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.HandleMessagePromotedEvent(this.server, storeDriverDeliveryEventArgsImpl);
			}
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)e;
			if (!storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient && !storeDriverDeliveryEventArgsImpl.IsJournalReport && UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsMessageInteresting(storeDriverDeliveryEventArgsImpl))
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.HandleMessageCreatedEvent(this.server, storeDriverDeliveryEventArgsImpl);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.UMPartnerMessageTracer;

		private StoreDriverServer server;

		internal abstract class UMPartnerMessageAgentProcessor
		{
			private protected StoreDriverServer Server { protected get; private set; }

			private protected StoreDriverDeliveryEventArgsImpl DeliveryArgs { protected get; private set; }

			internal static bool IsMessageInteresting(StoreDriverDeliveryEventArgsImpl deliveryArgs)
			{
				return ObjectClass.IsUMTranscriptionRequest(deliveryArgs.ReplayItem.ClassName) || UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsUMPartnerResponseMessage(deliveryArgs.ReplayItem);
			}

			internal static void HandleMessagePromotedEvent(StoreDriverServer server, StoreDriverDeliveryEventArgsImpl deliveryArgs)
			{
				try
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor umpartnerMessageAgentProcessor = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.Create(server, deliveryArgs);
					umpartnerMessageAgentProcessor.ProcessMessage();
				}
				catch (Exception ex)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPMAP.HandleMessagePromotedEvent: Message:{0} ClassName:{1} {2}", new object[]
					{
						deliveryArgs.ReplayItem,
						deliveryArgs.ReplayItem.ClassName,
						ex
					});
					throw;
				}
			}

			internal static void HandleMessageCreatedEvent(StoreDriverServer server, StoreDriverDeliveryEventArgsImpl deliveryArgs)
			{
				try
				{
					if (UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsUMPartnerResponseMessage(deliveryArgs.MessageItem))
					{
						using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(deliveryArgs.MailboxSession))
						{
							StoreObjectId storeObjectId = StoreId.GetStoreObjectId(deliveryArgs.DeliverToFolder);
							StoreObjectId storeObjectId2 = StoreId.GetStoreObjectId(folder.Id);
							if (storeObjectId == null || !storeObjectId.Equals(storeObjectId2))
							{
								UMPartnerMessageAgent.Tracer.TraceDebug(0L, "DeliverToFolder is not UM's staging folder -> Setting ClassName:IPM.Note (SCG case)");
								deliveryArgs.MessageItem.ClassName = "IPM.Note";
							}
						}
					}
				}
				catch (Exception ex)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPMAP.HandleMessageCreatedEvent: Message:{0} ClassName:{1} {2}", new object[]
					{
						deliveryArgs.ReplayItem,
						deliveryArgs.ReplayItem.ClassName,
						ex
					});
					throw;
				}
			}

			protected static UMPartnerMessageAgent.UMPartnerMessageAgentProcessor Create(StoreDriverServer server, StoreDriverDeliveryEventArgsImpl args)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor umpartnerMessageAgentProcessor = null;
				if (ObjectClass.IsUMTranscriptionRequest(args.ReplayItem.ClassName))
				{
					umpartnerMessageAgentProcessor = new UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor();
				}
				else if (UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsUMPartnerResponseMessage(args.ReplayItem))
				{
					string a = args.ReplayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContent) as string;
					if (string.Equals(a, "fax", StringComparison.OrdinalIgnoreCase))
					{
						umpartnerMessageAgentProcessor = new UMPartnerMessageAgent.UMPartnerFaxResponseProcessor();
					}
					else if (string.Equals(a, "voice+transcript", StringComparison.OrdinalIgnoreCase))
					{
						umpartnerMessageAgentProcessor = new UMPartnerMessageAgent.UMPartnerTranscriptionResponseProcessor();
					}
					else if (string.Equals(a, "voice", StringComparison.OrdinalIgnoreCase))
					{
						umpartnerMessageAgentProcessor = new UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.UMPartnerDeliverAsMessageProcessor();
					}
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("Created processor {0}", new object[]
				{
					(umpartnerMessageAgentProcessor != null) ? umpartnerMessageAgentProcessor.GetType().Name : null
				});
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(umpartnerMessageAgentProcessor, AckReason.UMPartnerMessageInvalidHeaders);
				umpartnerMessageAgentProcessor.Server = server;
				umpartnerMessageAgentProcessor.DeliveryArgs = args;
				return umpartnerMessageAgentProcessor;
			}

			protected static AttachmentHandle FindXsoAttachmentByName(MessageItem message, string name)
			{
				int num = 0;
				AttachmentHandle attachmentHandle = null;
				foreach (AttachmentHandle attachmentHandle2 in message.AttachmentCollection)
				{
					using (Attachment attachment = message.AttachmentCollection.Open(attachmentHandle2))
					{
						if (string.Equals(attachment.FileName, name, StringComparison.OrdinalIgnoreCase))
						{
							attachmentHandle = attachmentHandle2;
							break;
						}
					}
					if (++num == UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.MaxAttachmentCount)
					{
						break;
					}
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("FindXsoAttachmentByName: Msg:{0} Name={1} Result:{2}", new object[]
				{
					message,
					name,
					attachmentHandle
				});
				return attachmentHandle;
			}

			protected static StreamAttachment CreateXsoStreamAttachment(MessageItem message)
			{
				return (StreamAttachment)message.AttachmentCollection.Create(Microsoft.Exchange.Data.Storage.AttachmentType.Stream);
			}

			protected static void CopyStreamAttachment(Attachment source, Attachment destination)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("Begin copy attachment - src.Name:{0} dst.Name={1}", new object[]
				{
					source.Id,
					destination.Id
				});
				StreamAttachment streamAttachment = source as StreamAttachment;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(streamAttachment, AckReason.UMPartnerMessageInvalidAttachments);
				StreamAttachment streamAttachment2 = destination as StreamAttachment;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(streamAttachment2, AckReason.UMPartnerMessageInvalidAttachments);
				streamAttachment2.FileName = streamAttachment.FileName;
				streamAttachment2.ContentType = streamAttachment.ContentType;
				streamAttachment2[AttachmentSchema.DisplayName] = streamAttachment.DisplayName;
				using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
				{
					using (Stream contentStream2 = streamAttachment2.GetContentStream())
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStream(contentStream, contentStream2);
					}
				}
				destination.Save();
				destination.Load();
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("End copy attachment - src.Name:{0} dst.Name={1}", new object[]
				{
					source.FileName,
					destination.FileName
				});
			}

			protected static void CopyStream(Stream sourceStream, Stream destinationStream)
			{
				byte[] array = null;
				Utility.CopyStream(sourceStream, destinationStream, ref array);
			}

			protected static void ThrowAndSendNdrIfTrue(bool condition, SmtpResponse response)
			{
				if (condition)
				{
					throw new SmtpResponseException(response);
				}
			}

			protected static void ThrowAndSendNdrIfNull(object obj, SmtpResponse response)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(obj == null, response);
			}

			protected static bool IsUMPartnerResponseMessage(MessageItem message)
			{
				return ObjectClass.IsUMPartnerMessage(message.ClassName) || UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsAnonymousUMPartnerResponseMessage(message);
			}

			protected static bool IsAnonymousUMPartnerResponseMessage(MessageItem message)
			{
				if (!ObjectClass.IsMessage(message.ClassName, false))
				{
					return false;
				}
				string text = message.TryGetProperty(StoreObjectSchema.ContentClass) as string;
				return !string.IsNullOrEmpty(text) && string.Equals(text, "unauthenticated,MS-Exchange-UM-Partner", StringComparison.OrdinalIgnoreCase);
			}

			protected static void TraceDebug(string format, params object[] args)
			{
				UMPartnerMessageAgent.Tracer.TraceDebug(0L, format, args);
			}

			protected void PrepareMessageForDuplicateDetection(MessageItem message, ExDateTime clientSubmitTime, string messageId)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("PrepareMessageForDuplicateDetection: Message:{0} SubmitTime:{1} MessageId:{2}", new object[]
				{
					message,
					clientSubmitTime,
					messageId
				});
				message[ItemSchema.SentTime] = clientSubmitTime;
				message[ItemSchema.InternetMessageId] = BoomerangProvider.Instance.FormatInternetMessageId(messageId, this.Server.Name);
			}

			protected UMMailbox GetUMMailbox()
			{
				ADUser dataObject = null;
				if (!UMAgentUtil.TryGetADUser(UMPartnerMessageAgent.Tracer, this.DeliveryArgs.ADRecipientCache, this.DeliveryArgs.MailRecipient, out dataObject))
				{
					throw new SmtpResponseException(AckReason.UMPartnerMessageMessageCannotReadRecipient);
				}
				return new UMMailbox(dataObject);
			}

			protected abstract void ProcessMessage();

			private static readonly int MaxAttachmentCount = 3;

			private class UMPartnerDeliverAsMessageProcessor : UMPartnerMessageAgent.UMPartnerMessageAgentProcessor
			{
				protected override void ProcessMessage()
				{
					base.DeliveryArgs.ReplayItem.ClassName = "IPM.Note";
				}
			}
		}

		internal abstract class UMPartnerResponseProcessorBase : UMPartnerMessageAgent.UMPartnerMessageAgentProcessor
		{
			protected virtual bool SenderIdCheckRequired
			{
				get
				{
					return true;
				}
			}

			protected override void ProcessMessage()
			{
				UMMailbox ummailbox = base.GetUMMailbox();
				bool flag = true;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPartnerResponseProcessorBase:ProcessMessage -> Mailbox:{0} UMEnabled:{1} UMMailboxPolicy:{2}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner,
					ummailbox.UMEnabled,
					ummailbox.UMMailboxPolicy
				});
				if (ummailbox.UMEnabled && ummailbox.UMMailboxPolicy != null)
				{
					if (UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.IsAnonymousUMPartnerResponseMessage(base.DeliveryArgs.ReplayItem))
					{
						flag = this.ProcessAnonymousMessage(base.DeliveryArgs.ReplayItem, ummailbox);
					}
					this.InternalProcessMessage();
					if (flag)
					{
						this.DeliverToUMStagingFolder();
					}
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPartnerResponseProcessorBase:ProcessMessage DONE -> Mailbox:{0} ClassName:{1}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner,
					base.DeliveryArgs.ReplayItem.ClassName
				});
			}

			protected bool ProcessAnonymousMessage(MessageItem message, UMMailbox recipient)
			{
				IExchangePrincipal exchangePrincipal = (base.DeliveryArgs != null) ? base.DeliveryArgs.MailboxSession.MailboxOwner : null;
				bool flag = false;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPartnerResponseProcessorBase:ProcessAnonymousMessage -> Mailbox:{0}", new object[]
				{
					exchangePrincipal
				});
				if (this.SenderIdCheckRequired)
				{
					string text = null;
					bool flag2 = false;
					if (VariantConfiguration.InvariantNoFlightingSnapshot.MailboxTransport.UseFopeReceivedSpfHeader.Enabled)
					{
						if (this.GetSPFCheckResult(exchangePrincipal, message, out text))
						{
							flag2 = true;
						}
					}
					else
					{
						int? valueAsNullable = message.GetValueAsNullable<int>(MessageItemSchema.SenderIdStatus);
						if (valueAsNullable != null && valueAsNullable.Value == 2)
						{
							flag2 = true;
							text = (message.TryGetProperty(ItemSchema.PurportedSenderDomain) as string);
							if (string.IsNullOrEmpty(text))
							{
								UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ProcessAnonymousMessage: PurportedSenderDomain missing.  Sending as regular email", new object[0]);
							}
							UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ProcessAnonymousMessage: SenderIdStatus:{0} SenderDomain:{1} Mailbox:{2}", new object[]
							{
								valueAsNullable.Value,
								text,
								exchangePrincipal
							});
						}
						else if (this.GetSPFCheckResult(exchangePrincipal, message, out text))
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						UMMailboxPolicy policy;
						if (!this.TryGetUMMailboxPolicy(recipient, out policy))
						{
							UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ProcessAnonymousMessage: Could not get UMMailboxPolicy for mailbox {0}", new object[]
							{
								exchangePrincipal
							});
							throw new SmtpResponseException(AckReason.UMPartnerMessageMessageCannotReadPolicy);
						}
						if (string.IsNullOrEmpty(text) || !this.IsValidPartnerDomain(text, policy))
						{
							UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ProcessAnonymousMessage: Partner domain {0} is not valid for mailbox {1}.  Sending as regular email.", new object[]
							{
								text,
								exchangePrincipal
							});
						}
						else
						{
							flag = true;
						}
					}
				}
				else
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ProcessAnonymousMessage: SenderIdCheck is not required, promoting message", new object[0]);
				}
				if (!this.SenderIdCheckRequired || flag)
				{
					message.ClassName = "IPM.Note.Microsoft.Partner.UM";
					return true;
				}
				message.ClassName = "IPM.Note";
				return false;
			}

			protected virtual bool TryGetUMMailboxPolicy(UMMailbox recipient, out UMMailboxPolicy policy)
			{
				return UMAgentUtil.TryGetUMMailboxPolicy(UMPartnerMessageAgent.Tracer, recipient, out policy);
			}

			protected void DeliverToUMStagingFolder()
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("Delivering message to UM Staging folder. Mailbox {0}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner
				});
				using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(base.DeliveryArgs.MailboxSession))
				{
					base.DeliveryArgs.DeliverToFolder = folder.Id;
					base.DeliveryArgs.DeliverToFolderName = folder.DisplayName;
					base.DeliveryArgs.ShouldSkipMoveRule = true;
				}
				PolicyTagHelper.SetRetentionProperties(base.DeliveryArgs.ReplayItem, ExDateTime.UtcNow.AddDays(7.0), 7);
			}

			protected abstract void InternalProcessMessage();

			protected abstract bool IsValidPartnerDomain(string senderDomain, UMMailboxPolicy policy);

			private bool GetSPFCheckResult(IExchangePrincipal mailboxOwner, MessageItem message, out string domain)
			{
				domain = null;
				try
				{
					string text = message.TryGetProperty(MessageItemSchema.ReceivedSPF) as string;
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("GetSPFCheckResult: Processing {0} for Mailbox {1}", new object[]
					{
						text,
						mailboxOwner
					});
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Trim();
						if (text.StartsWith("pass", StringComparison.InvariantCultureIgnoreCase))
						{
							int num = text.IndexOf("envelope-from=<", StringComparison.InvariantCultureIgnoreCase);
							if (num >= 0)
							{
								num += "envelope-from=<".Length;
								int num2 = text.IndexOf(">", num, StringComparison.InvariantCultureIgnoreCase);
								if (num2 - num > 0)
								{
									string address = text.Substring(num, num2 - num);
									domain = SmtpAddress.Parse(address).Domain;
								}
							}
						}
					}
				}
				catch (ArgumentException ex)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("GetSPFCheckResult: Invalid input: {0}. Mailbox: {1}", new object[]
					{
						ex,
						mailboxOwner
					});
				}
				catch (FormatException ex2)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("GetSPFCheckResult: Invalid input: {0}. Mailbox: {1}", new object[]
					{
						ex2,
						mailboxOwner
					});
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("GetSPFCheckResult-> Returning domain: {0}. Mailbox: {1}", new object[]
				{
					domain,
					mailboxOwner
				});
				return !string.IsNullOrEmpty(domain);
			}

			private const string SPFResultPass = "pass";

			private const string SPFEnvelopeFromPrefix = "envelope-from=<";

			private const string SPFEnvelopeFromSuffix = ">";
		}

		private class UMPartnerFaxResponseProcessor : UMPartnerMessageAgent.UMPartnerResponseProcessorBase
		{
			protected override void InternalProcessMessage()
			{
				UMPartnerMessageAgent.UMPartnerFaxResponseProcessor.ValidateFaxMessage(base.DeliveryArgs.ReplayItem);
			}

			protected override bool IsValidPartnerDomain(string senderDomain, UMMailboxPolicy policy)
			{
				bool result = false;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPartnerFaxResponseProcessor::IsValidPartnerDomain ->senderDomain:{0} policy.AllowFax:{1} policy.FaxServerUri:{2}", new object[]
				{
					senderDomain,
					policy.AllowFax,
					policy.FaxServerURI
				});
				if (policy.AllowFax && !string.IsNullOrEmpty(policy.FaxServerURI))
				{
					result = (policy.FaxServerURI.IndexOf(senderDomain + ":", 0, policy.FaxServerURI.Length, StringComparison.OrdinalIgnoreCase) >= 0);
				}
				return result;
			}

			private static void ValidateFaxMessage(MessageItem message)
			{
				string status = message.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerStatus) as string;
				UMPartnerFaxStatus umpartnerFaxStatus;
				if (!UMPartnerFaxStatus.TryParse(status, out umpartnerFaxStatus))
				{
					throw new SmtpResponseException(AckReason.UMPartnerMessageInvalidHeaders);
				}
				object obj = message.TryGetProperty(MessageItemSchema.FaxNumberOfPages);
				if (obj == null || obj is PropertyError)
				{
					throw new SmtpResponseException(AckReason.UMPartnerMessageInvalidHeaders);
				}
				int num = (int)obj;
				if (!umpartnerFaxStatus.MissedCall)
				{
					UMPartnerMessageAgent.UMPartnerFaxResponseProcessor.CheckTiffAttachment(message);
					if (num <= 0)
					{
						throw new SmtpResponseException(AckReason.UMPartnerMessageInvalidHeaders);
					}
				}
				string base64Data = message.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContext) as string;
				UMPartnerFaxContext umpartnerFaxContext;
				if (!UMPartnerContext.TryParse<UMPartnerFaxContext>(base64Data, out umpartnerFaxContext) || string.IsNullOrEmpty(umpartnerFaxContext.CallId))
				{
					throw new SmtpResponseException(AckReason.InboundInvalidContent);
				}
			}

			private static void CheckTiffAttachment(MessageItem message)
			{
				if (message.AttachmentCollection.Count != 1)
				{
					throw new SmtpResponseException(AckReason.UMFaxPartnerMessageInvalidAttachments);
				}
				foreach (AttachmentHandle handle in message.AttachmentCollection)
				{
					using (Attachment attachment = message.AttachmentCollection.Open(handle))
					{
						if (!string.Equals(attachment.ContentType, "image/tiff", StringComparison.OrdinalIgnoreCase))
						{
							throw new SmtpResponseException(AckReason.UMFaxPartnerMessageInvalidAttachments);
						}
					}
				}
			}
		}

		private class UMPartnerTranscriptionRequestProcessor : UMPartnerMessageAgent.UMPartnerMessageAgentProcessor
		{
			protected override void ProcessMessage()
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("UMPartnerTranscriptionRequestProcessor:ProcessMessage -> Mailbox:{0}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner
				});
				string base64Data = base.DeliveryArgs.ReplayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContext) as string;
				UMPartnerTranscriptionContext context;
				if (!UMPartnerContext.TryParse<UMPartnerTranscriptionContext>(base64Data, out context))
				{
					throw new SmtpResponseException(AckReason.UMPartnerMessageInvalidHeaders);
				}
				StoreObjectId deferredMessageId = this.SendTimeoutMessageWithDeferredSubmission(context);
				this.SendTranscriptionRequestToPartner(context, deferredMessageId);
				base.DeliveryArgs.MailRecipient.DsnRequested = DsnRequestedFlags.Never;
				throw new SmtpResponseException(AckReason.UMTranscriptionRequestSuccess, "UMPartnerMessageAgent");
			}

			private static void SetHeaderValue(EmailMessage message, HeaderId headerId, object headerValue)
			{
				Header header = message.RootPart.Headers.FindFirst(headerId);
				if (header == null)
				{
					header = Header.Create(headerId);
					message.RootPart.Headers.AppendChild(header);
				}
				header.Value = ((headerValue != null) ? headerValue.ToString() : string.Empty);
			}

			private static void SetHeaderValue(EmailMessage message, string headerName, object headerValue)
			{
				Header header = message.RootPart.Headers.FindFirst(headerName);
				if (header == null)
				{
					header = Header.Create(headerName);
					message.RootPart.Headers.AppendChild(header);
				}
				header.Value = ((headerValue != null) ? headerValue.ToString() : string.Empty);
			}

			private void SendTranscriptionRequestToPartner(UMPartnerTranscriptionContext context, StoreObjectId deferredMessageId)
			{
				MailboxSession mailboxSession = base.DeliveryArgs.MailboxSession;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("SendTranscriptionRequestToPartner START -> Mailbox:{0}", new object[]
				{
					mailboxSession.MailboxOwner
				});
				string headerValue = base.DeliveryArgs.ReplayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerAssignedID) as string;
				EmailMessage emailMessage = EmailMessage.Create();
				emailMessage.To.Add(new EmailRecipient(string.Empty, (string)context.PartnerAddress));
				string displayName = base.GetUMMailbox().DisplayName ?? string.Empty;
				emailMessage.From = new EmailRecipient(displayName, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				emailMessage.Subject = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					"MS-Exchange-UM-Partner",
					"voice"
				});
				UMPartnerMinimalTranscriptionContext umpartnerMinimalTranscriptionContext = UMPartnerContext.Create<UMPartnerMinimalTranscriptionContext>();
				umpartnerMinimalTranscriptionContext.TimeoutMessageId = deferredMessageId.ToBase64String();
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, HeaderId.ContentClass, "MS-Exchange-UM-Partner");
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-MS-Exchange-UM-PartnerContent", "voice");
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-MS-Exchange-UM-PartnerContext", umpartnerMinimalTranscriptionContext);
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-MS-Exchange-UM-PartnerAssignedID", headerValue);
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-VoiceMessageDuration", context.Duration);
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-VoiceMessageSenderName", context.CallerName);
				UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-CallingTelephoneNumber", context.CallingParty);
				if (!string.IsNullOrEmpty(context.UMDialPlanLanguage))
				{
					UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-MS-Exchange-UM-DialPlanLanguage", context.UMDialPlanLanguage);
				}
				if (!string.IsNullOrEmpty(context.CallerInformedOfAnalysis))
				{
					UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.SetHeaderValue(emailMessage, "X-MS-Exchange-UM-CallerInformedOfAnalysis", context.CallerInformedOfAnalysis);
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("SendTranscriptionRequestToPartner:  To:{0} From:{1} VoiceMessageDuration:{2} VoiceMessageSenderName:{3} CallingTelephoneNumber:{4} PartnerAudioAttachmentName:{5} Mailbox:{6} PartnerContext:{7} UMDialPlanLanguage:{8} CallerInformedOfAnalysis:{9}", new object[]
				{
					context.PartnerAddress,
					mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
					context.Duration,
					context.CallerGuid,
					context.CallingParty,
					context.PartnerAudioAttachmentName,
					mailboxSession.MailboxOwner,
					umpartnerMinimalTranscriptionContext,
					context.UMDialPlanLanguage,
					context.CallerInformedOfAnalysis
				});
				AttachmentHandle attachmentHandle = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(base.DeliveryArgs.ReplayItem, context.PartnerAudioAttachmentName);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle, AckReason.UMPartnerMessageInvalidAttachments);
				Attachment attachment = emailMessage.Attachments.Add();
				using (Attachment attachment2 = base.DeliveryArgs.ReplayItem.AttachmentCollection.Open(attachmentHandle))
				{
					StreamAttachment streamAttachment = attachment2 as StreamAttachment;
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(streamAttachment, AckReason.UMPartnerMessageInvalidAttachments);
					attachment.FileName = streamAttachment.FileName;
					attachment.ContentType = streamAttachment.ContentType;
					using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
					{
						using (Stream contentWriteStream = attachment.GetContentWriteStream())
						{
							UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStream(contentStream, contentWriteStream);
						}
					}
				}
				base.Server.SubmitMessage(base.DeliveryArgs.MailItemDeliver.MbxTransportMailItem, emailMessage, base.DeliveryArgs.MailItemDeliver.MbxTransportMailItem.OrganizationId, base.DeliveryArgs.MailItemDeliver.MbxTransportMailItem.ExternalOrganizationId, true);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("SendTranscriptionRequestToPartner DONE -> Mailbox:{0}", new object[]
				{
					mailboxSession.MailboxOwner
				});
			}

			private StoreObjectId SendTimeoutMessageWithDeferredSubmission(UMPartnerTranscriptionContext context)
			{
				StoreObjectId storeObjectId = null;
				MailboxSession mailboxSession = base.DeliveryArgs.MailboxSession;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("SendTimeoutMessageWithDeferredSubmission START -> Mailbox:{0}", new object[]
				{
					mailboxSession.MailboxOwner
				});
				ADSessionSettings adSettings = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings();
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromLegacyDN(adSettings, mailboxSession.MailboxOwnerLegacyDN);
				using (MailboxSession mailboxSession2 = MailboxSession.OpenAsAdmin(mailboxOwner, mailboxSession.Culture, "Client=UM;Action=UMPartnerMessageAgent"))
				{
					using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(mailboxSession2))
					{
						using (MessageItem messageItem = MessageItem.Create(mailboxSession2, folder.Id))
						{
							messageItem.Recipients.Add(new Participant(mailboxSession2.MailboxOwner), RecipientItemType.To);
							messageItem.Subject = "504 Timeout";
							messageItem.ClassName = "IPM.Note.Microsoft.Partner.UM";
							messageItem.Save(SaveMode.ResolveConflicts);
							messageItem.Load();
							AttachmentHandle attachmentHandle = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(base.DeliveryArgs.ReplayItem, context.PcmAudioAttachmentName);
							UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle, AckReason.UMPartnerMessageInvalidAttachments);
							using (Attachment attachment = base.DeliveryArgs.ReplayItem.AttachmentCollection.Open(attachmentHandle))
							{
								using (StreamAttachment streamAttachment = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CreateXsoStreamAttachment(messageItem))
								{
									UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStreamAttachment(attachment, streamAttachment);
								}
							}
							if (!string.IsNullOrEmpty(context.IpmAttachmentName))
							{
								attachmentHandle = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(base.DeliveryArgs.ReplayItem, context.IpmAttachmentName);
								UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle, AckReason.UMPartnerMessageInvalidAttachments);
								using (Attachment attachment2 = base.DeliveryArgs.ReplayItem.AttachmentCollection.Open(attachmentHandle))
								{
									using (StreamAttachment streamAttachment2 = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CreateXsoStreamAttachment(messageItem))
									{
										UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStreamAttachment(attachment2, streamAttachment2);
									}
								}
							}
							using (StreamAttachment streamAttachment3 = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CreateXsoStreamAttachment(messageItem))
							{
								streamAttachment3.FileName = Guid.NewGuid().ToString("N") + ".xml";
								streamAttachment3.ContentType = "text/xml";
								context.PartnerTranscriptionAttachmentName = streamAttachment3.FileName;
								using (Stream contentStream = streamAttachment3.GetContentStream())
								{
									using (StreamWriter streamWriter = new StreamWriter(contentStream, Encoding.UTF8))
									{
										streamWriter.Write(VoiceMailPreviewSchema.InternalXml.TimeoutTranscription);
									}
								}
								streamAttachment3.Save();
							}
							base.PrepareMessageForDuplicateDetection(messageItem, context.CreationTime, context.SessionId);
							messageItem[UMPartnerMessageAgent.UMPartnerTranscriptionRequestProcessor.deferredSendTimeDefinition] = ExDateTime.UtcNow.AddSeconds((double)context.PartnerMaxDeliveryDelay);
							messageItem[MessageItemSchema.XMsExchangeUMPartnerStatus] = "504 Timeout";
							messageItem[MessageItemSchema.XMsExchangeUMPartnerContent] = "voice+transcript";
							messageItem[MessageItemSchema.XMsExchangeUMPartnerContext] = context.ToString();
							messageItem.SendWithoutSavingMessage();
							messageItem.Load(null);
							storeObjectId = messageItem.Id.ObjectId;
						}
					}
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("SendTimeoutMessageWithDeferredSubmission DONE -> TimeoutMessageId:{0} Mailbox:{1}", new object[]
				{
					storeObjectId,
					mailboxSession.MailboxOwner
				});
				return storeObjectId;
			}

			private static PropertyTagPropertyDefinition deferredSendTimeDefinition = PropertyTagPropertyDefinition.CreateCustom("DeferredSendTime", 1072627776U);
		}

		private class UMPartnerTranscriptionResponseProcessor : UMPartnerMessageAgent.UMPartnerResponseProcessorBase
		{
			protected override bool SenderIdCheckRequired
			{
				get
				{
					return false;
				}
			}

			protected override void InternalProcessMessage()
			{
				MessageItem replayItem = base.DeliveryArgs.ReplayItem;
				string text = replayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerStatus) as string;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: status={0} ", new object[]
				{
					text
				});
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(string.IsNullOrEmpty(text), AckReason.UMPartnerMessageInvalidHeaders);
				string text2 = replayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContent) as string;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: content={0} ", new object[]
				{
					text2
				});
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(string.IsNullOrEmpty(text2), AckReason.UMPartnerMessageInvalidHeaders);
				string text3 = replayItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContext) as string;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: base64Context={0} ", new object[]
				{
					text3
				});
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(string.IsNullOrEmpty(text3), AckReason.UMPartnerMessageInvalidHeaders);
				string text4 = string.Format("{0}/{1}/{2}", text2, text, replayItem.InternetMessageId);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage -> Processing message:{0} for {1}", new object[]
				{
					text4,
					base.DeliveryArgs.MailboxSession.MailboxOwner
				});
				try
				{
					UMPartnerTranscriptionContext umpartnerTranscriptionContext = null;
					string b = "504 Timeout";
					if (!string.Equals(text, b, StringComparison.OrdinalIgnoreCase))
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: Mailbox:{0} Processing partner message...", new object[]
						{
							base.DeliveryArgs.MailboxSession.MailboxOwner
						});
						UMPartnerMinimalTranscriptionContext umpartnerMinimalTranscriptionContext = UMPartnerContext.Parse<UMPartnerMinimalTranscriptionContext>(text3);
						StoreObjectId deferredMessageId = StoreObjectId.Deserialize(umpartnerMinimalTranscriptionContext.TimeoutMessageId);
						this.OpenDeferredMessageAndAddMissingData(deferredMessageId, replayItem, out umpartnerTranscriptionContext);
					}
					else
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: Mailbox:{0} Processing message with deferred delivery (timeout)...", new object[]
						{
							base.DeliveryArgs.MailboxSession.MailboxOwner
						});
						umpartnerTranscriptionContext = UMPartnerContext.Parse<UMPartnerTranscriptionContext>(text3);
					}
					base.PrepareMessageForDuplicateDetection(replayItem, umpartnerTranscriptionContext.CreationTime, umpartnerTranscriptionContext.SessionId);
				}
				catch (ArgumentException ex)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: Mailbox:{0} Error:{1} ", new object[]
					{
						base.DeliveryArgs.MailboxSession.MailboxOwner,
						ex
					});
					throw new SmtpResponseException(AckReason.UMPartnerMessageInvalidHeaders);
				}
				catch (ObjectNotFoundException)
				{
					UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("InternalProcessMessage: Mailbox:{0} Couldn't find timeout msg. Partner response arrived too late?", new object[]
					{
						base.DeliveryArgs.MailboxSession.MailboxOwner
					});
					this.LogPartnerMessageArrivedTooLateWarningEvent(text4);
					throw new SmtpResponseException(AckReason.UMPartnerMessageMessageArrivedTooLate);
				}
			}

			protected override bool IsValidPartnerDomain(string domain, UMMailboxPolicy policy)
			{
				SmtpAddress? voiceMailPreviewPartnerAddress = policy.VoiceMailPreviewPartnerAddress;
				return voiceMailPreviewPartnerAddress != null && policy.AllowVoiceMailPreview && string.Equals(voiceMailPreviewPartnerAddress.Value.Domain, domain, StringComparison.OrdinalIgnoreCase);
			}

			private void LogPartnerMessageArrivedTooLateWarningEvent(string messageDescription)
			{
				MessageItem replayItem = base.DeliveryArgs.ReplayItem;
				string text = (replayItem.From != null) ? replayItem.From.EmailAddress : string.Empty;
				string text2 = base.DeliveryArgs.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_UMPartnerMessageArrivedTooLate, messageDescription, new object[]
				{
					messageDescription,
					text2,
					text
				});
			}

			private void ReplacePartnerAudioWithOriginalPCM(MessageItem source, MessageItem destination, UMPartnerTranscriptionContext context)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ReplacePartnerAudioWithOriginalPCM: Mailbox:{0} Source:{1} Destination:{2}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner,
					source.InternetMessageId,
					destination.InternetMessageId
				});
				AttachmentHandle attachmentHandle = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(source, context.PcmAudioAttachmentName);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle, AckReason.UMPartnerMessageInvalidAttachments);
				AttachmentHandle attachmentHandle2 = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(destination, context.PartnerAudioAttachmentName);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle2, AckReason.UMPartnerMessageInvalidAttachments);
				destination.AttachmentCollection.Remove(attachmentHandle2);
				using (Attachment attachment = source.AttachmentCollection.Open(attachmentHandle, Microsoft.Exchange.Data.Storage.AttachmentType.Stream))
				{
					using (Attachment attachment2 = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CreateXsoStreamAttachment(destination))
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStreamAttachment(attachment, attachment2);
					}
				}
			}

			private void AddInterpersonalMessageIfPresent(MessageItem source, MessageItem destination, UMPartnerTranscriptionContext context)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("AddInterpersonalMessageIfPresent: Mailbox:{0} Source:{1} Destination:{2} IpmAttachmentName:{3}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner,
					source.InternetMessageId,
					destination.InternetMessageId,
					context.IpmAttachmentName
				});
				if (string.IsNullOrEmpty(context.IpmAttachmentName))
				{
					return;
				}
				AttachmentHandle attachmentHandle = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.FindXsoAttachmentByName(source, context.IpmAttachmentName);
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(attachmentHandle, AckReason.UMPartnerMessageInvalidAttachments);
				using (Attachment attachment = source.AttachmentCollection.Open(attachmentHandle))
				{
					using (StreamAttachment streamAttachment = UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CreateXsoStreamAttachment(destination))
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.CopyStreamAttachment(attachment, streamAttachment);
					}
				}
			}

			private void ValidateTranscriptionMessage(MessageItem message, UMPartnerTranscriptionContext context)
			{
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ValidateTranscriptionMessage: Mailbox:{0} Message:{1}", new object[]
				{
					base.DeliveryArgs.MailboxSession.MailboxOwner,
					message.InternetMessageId
				});
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(message.AttachmentCollection.Count != 2, AckReason.UMPartnerMessageInvalidAttachments);
				context.PartnerAudioAttachmentName = null;
				context.PartnerTranscriptionAttachmentName = null;
				foreach (AttachmentHandle handle in message.AttachmentCollection)
				{
					using (Attachment attachment = message.AttachmentCollection.Open(handle))
					{
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("ValidateTranscriptionMessage: Processing {0} - {1}", new object[]
						{
							attachment.FileName,
							attachment.ContentType
						});
						if (attachment.AttachmentType == Microsoft.Exchange.Data.Storage.AttachmentType.Stream && !string.IsNullOrEmpty(attachment.ContentType))
						{
							if (attachment.ContentType.StartsWith("audio/wav", StringComparison.OrdinalIgnoreCase))
							{
								context.PartnerAudioAttachmentName = attachment.FileName;
							}
							else if (attachment.ContentType.StartsWith("text/xml", StringComparison.OrdinalIgnoreCase) || attachment.ContentType.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase))
							{
								context.PartnerTranscriptionAttachmentName = attachment.FileName;
								StreamAttachment streamAttachment = attachment as StreamAttachment;
								UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfNull(streamAttachment, AckReason.UMPartnerMessageInvalidAttachments);
								using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
								{
									bool flag = VoiceMailPreviewSchema.IsValidTranscription(contentStream);
									UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(!flag, AckReason.UMPartnerMessageInvalidTranscriptionDocument);
								}
							}
						}
					}
				}
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(string.IsNullOrEmpty(context.PartnerAudioAttachmentName) || string.IsNullOrEmpty(context.PartnerTranscriptionAttachmentName), AckReason.UMPartnerMessageInvalidAttachments);
			}

			private void OpenDeferredMessageAndAddMissingData(StoreObjectId deferredMessageId, MessageItem partnerMessage, out UMPartnerTranscriptionContext context)
			{
				MailboxSession mailboxSession = base.DeliveryArgs.MailboxSession;
				UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.TraceDebug("OpenDeferredMessageAndAddMissingData: Mailbox:{0} Message:{1}", new object[]
				{
					mailboxSession.MailboxOwner,
					partnerMessage.InternetMessageId
				});
				using (MailboxSession mailboxSession2 = MailboxSession.OpenAsAdmin(mailboxSession.MailboxOwner, mailboxSession.Culture, "Client=UM;Action=UMPartnerMessageAgent"))
				{
					using (MessageItem messageItem = MessageItem.Bind(mailboxSession2, deferredMessageId))
					{
						messageItem.Load(new PropertyDefinition[]
						{
							MessageItemSchema.XMsExchangeUMPartnerContext
						});
						string text = messageItem.TryGetProperty(MessageItemSchema.XMsExchangeUMPartnerContext) as string;
						UMPartnerMessageAgent.UMPartnerMessageAgentProcessor.ThrowAndSendNdrIfTrue(string.IsNullOrEmpty(text), AckReason.UMPartnerMessageInvalidHeaders);
						context = UMPartnerContext.Parse<UMPartnerTranscriptionContext>(text);
						this.ValidateTranscriptionMessage(partnerMessage, context);
						this.ReplacePartnerAudioWithOriginalPCM(messageItem, partnerMessage, context);
						this.AddInterpersonalMessageIfPresent(messageItem, partnerMessage, context);
						partnerMessage[MessageItemSchema.XMsExchangeUMPartnerContext] = context.ToString();
					}
				}
			}
		}
	}
}
