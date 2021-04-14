using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Approval;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Approval.Applications
{
	internal class ModeratedDLApplication : ApprovalApplication
	{
		internal ModeratedDLApplication()
		{
		}

		internal override void OnStart()
		{
			lock (ModeratedDLApplication.SyncRoot)
			{
				if (ModeratedDLApplication.refCount == 0)
				{
					MessageTrackingLog.Start("MSGTRKMA");
					Server server = null;
					ADNotificationAdapter.TryRunADOperation(delegate()
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 144, "OnStart", "f:\\15.00.1497\\sources\\dev\\Approval\\src\\Applications\\ModeratedDLApplication\\ModeratedDLApplication.cs");
						this.serverConfigNotificationCookie = ADNotificationAdapter.RegisterChangeNotification<Server>(topologyConfigurationSession.GetOrgContainerId(), new ADNotificationCallback(ModeratedDLApplication.ServerChangeCallback));
						server = topologyConfigurationSession.ReadLocalServer();
						Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain acceptedDomain = topologyConfigurationSession.GetDefaultAcceptedDomain();
						if (acceptedDomain != null && acceptedDomain.DomainName.SmtpDomain != null)
						{
							this.defaultAcceptedDomain = acceptedDomain.DomainName.SmtpDomain.Domain;
						}
					});
					if (server == null)
					{
						ModeratedDLApplication.diag.TraceError((long)this.GetHashCode(), "Cannot read local server for message tracking");
					}
					MessageTrackingLog.Configure(server);
				}
				ModeratedDLApplication.refCount++;
			}
			base.OnStart();
		}

		internal override void OnStop()
		{
			lock (ModeratedDLApplication.SyncRoot)
			{
				ModeratedDLApplication.refCount--;
				if (ModeratedDLApplication.refCount == 0)
				{
					if (this.serverConfigNotificationCookie != null)
					{
						ADNotificationAdapter.UnregisterChangeNotification(this.serverConfigNotificationCookie);
						this.serverConfigNotificationCookie = null;
					}
					MessageTrackingLog.Stop();
				}
			}
			base.OnStop();
		}

		internal override bool OnApprove(MessageItem message)
		{
			using (MessageItem messageItem = MessageItem.CloneMessage(message.Session, message.ParentId, message, null))
			{
				this.TrackDecision(message, true);
				messageItem.Recipients.Clear();
				messageItem.Recipients.Add(message.Sender);
				messageItem.SendWithoutSavingMessage();
			}
			return true;
		}

		internal override bool OnReject(MessageItem message)
		{
			this.TrackDecision(message, false);
			this.SendRejectNotification(message);
			return true;
		}

		internal override void OnAllDecisionMakersOof(MessageItem messageItem)
		{
			this.SendExpiryNdrOofNotification(messageItem, ApprovalInformation.ApprovalNotificationType.ModeratorsOofNotification);
		}

		internal override void OnAllDecisionMakersNdred(MessageItem messageItem)
		{
			string valueOrDefault = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
			MessageTrackingLog.TrackModeratorsAllNdr(MessageTrackingSource.APPROVAL, messageItem.InternetMessageId, valueOrDefault, valueOrDefault2, this.GetModeratedRecipients(messageItem, true), ModeratedDLApplication.GetOrganizationIdFromMessage(messageItem));
			this.SendExpiryNdrOofNotification(messageItem, ApprovalInformation.ApprovalNotificationType.ModeratorsNdrNotification);
		}

		internal override bool OnExpire(MessageItem messageItem, out bool sendUpdate)
		{
			sendUpdate = false;
			string valueOrDefault = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
			if ((!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && !Datacenter.IsPartnerHostedOnly(true)) || ModeratedDLApplication.ExpiryNotificationCounterWrapper.Instance.IncrementCountAndCheckLimit())
			{
				sendUpdate = true;
				this.SendExpiryNdrOofNotification(messageItem, ApprovalInformation.ApprovalNotificationType.ExpiryNotification);
				this.SendModeratorExpiryNotification(messageItem, ApprovalInformation.ApprovalNotificationType.ModeratorExpiryNotification);
			}
			MessageTrackingLog.TrackModeratorExpired(MessageTrackingSource.APPROVAL, messageItem.InternetMessageId, valueOrDefault, valueOrDefault2, this.GetModeratedRecipients(messageItem, true), ModeratedDLApplication.GetOrganizationIdFromMessage(messageItem), sendUpdate);
			return true;
		}

		private static void ServerChangeCallback(ADNotificationEventArgs args)
		{
			Server server = null;
			ADNotificationAdapter.TryReadConfiguration<Server>(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 362, "ServerChangeCallback", "f:\\15.00.1497\\sources\\dev\\Approval\\src\\Applications\\ModeratedDLApplication\\ModeratedDLApplication.cs");
				return topologyConfigurationSession.ReadLocalServer();
			}, out server);
			if (server == null)
			{
				ModeratedDLApplication.diag.TraceError(0L, "Cannot read local server for updating message tracking config. Continue with previous config.");
				return;
			}
			MessageTrackingLog.Configure(server);
		}

		private static bool IsEncodingMatch(IEnumerable<int> codepages, int codepageToFind)
		{
			foreach (int num in codepages)
			{
				if (codepageToFind == num)
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryGetCommentAttachment(MessageItem messageItem, out StreamAttachment targetAttachment)
		{
			targetAttachment = null;
			AttachmentCollection attachmentCollection = messageItem.AttachmentCollection;
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				Attachment attachment = null;
				try
				{
					attachment = attachmentCollection.Open(handle);
					if ("DecisionComments.txt".Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase))
					{
						targetAttachment = (attachment as StreamAttachment);
						if (targetAttachment != null)
						{
							attachment = null;
							return true;
						}
						ModeratedDLApplication.diag.TraceError<Attachment>(0L, "Found attachment with the correct name, but it's not a stream: {0}", attachment);
					}
				}
				finally
				{
					if (attachment != null)
					{
						attachment.Dispose();
						attachment = null;
					}
				}
			}
			return false;
		}

		private static OrganizationId GetOrganizationIdFromMessage(MessageItem messageItem)
		{
			MailboxSession mailboxSession = messageItem.Session as MailboxSession;
			if (mailboxSession != null && mailboxSession.MailboxOwner != null)
			{
				return mailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
			}
			ModeratedDLApplication.diag.TraceError(0L, "Cannot get organization ID, no session or no mailbox owner.");
			return null;
		}

		private void TrackDecision(MessageItem messageItem, bool isApproved)
		{
			string valueOrDefault = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
			MessageTrackingLog.TrackModeratorDecision(MessageTrackingSource.APPROVAL, messageItem.InternetMessageId, valueOrDefault, valueOrDefault2, this.GetModeratedRecipients(messageItem, true), isApproved, ModeratedDLApplication.GetOrganizationIdFromMessage(messageItem));
		}

		private void SendExpiryNdrOofNotification(MessageItem messageItem, ApprovalInformation.ApprovalNotificationType notificationType)
		{
			ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Entering SendExpiryNdrOofNotification");
			if (!this.ShouldSendNotification(messageItem))
			{
				return;
			}
			RoutingAddress routingAddress;
			if (!this.TryGetOriginalSender(messageItem, out routingAddress))
			{
				return;
			}
			MailboxSession mailboxSession = (MailboxSession)messageItem.Session;
			messageItem.Load(ModeratedDLApplication.NotificationPropertiesFromInitMessage);
			string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.AcceptLanguage, string.Empty);
			ICollection<string> moderatedRecipients = this.GetModeratedRecipients(messageItem, false);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			DsnHumanReadableWriter defaultDsnHumanReadableWriter = DsnHumanReadableWriter.DefaultDsnHumanReadableWriter;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox);
			using (MessageItem messageItem2 = MessageItem.Create(mailboxSession, defaultFolderId))
			{
				ApprovalInformation approvalInformation = null;
				approvalInformation = defaultDsnHumanReadableWriter.GetMessageInModerationExpiredNdrOofInformation(notificationType, messageItem.Subject, moderatedRecipients, valueOrDefault2, valueOrDefault);
				messageItem2.Subject = approvalInformation.Subject;
				BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, approvalInformation.MessageCharset.Name);
				using (Stream stream = messageItem2.Body.OpenWriteStream(configuration))
				{
					defaultDsnHumanReadableWriter.WriteHtmlModerationBody(stream, approvalInformation);
				}
				this.StampCommonNotificationProperties(messageItem2, messageItem, new RoutingAddress[]
				{
					routingAddress
				}, valueOrDefault2, approvalInformation.Culture);
				string text;
				this.AttachOriginalMessageToNotification(messageItem, messageItem2, out text);
				messageItem2.ClassName = "IPM.Note.Microsoft.Approval.Reply";
				messageItem2.SendWithoutSavingMessage();
				ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Notification sent.");
			}
		}

		private void SendModeratorExpiryNotification(MessageItem messageItem, ApprovalInformation.ApprovalNotificationType notificationType)
		{
			ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Entering SendModeratorExpiryNotification");
			if (!this.ShouldSendNotification(messageItem))
			{
				return;
			}
			string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalAllowedDecisionMakers);
			RoutingAddress[] collection;
			if (!ApprovalUtils.TryGetDecisionMakers(valueOrDefault, out collection))
			{
				return;
			}
			MailboxSession mailboxSession = (MailboxSession)messageItem.Session;
			messageItem.Load(ModeratedDLApplication.NotificationPropertiesFromInitMessage);
			ICollection<string> moderatedRecipients = this.GetModeratedRecipients(messageItem, false);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			int value = messageItem.GetValueAsNullable<int>(StoreObjectSchema.RetentionPeriod) ?? 2;
			Dictionary<CultureInfo, List<RoutingAddress>> dictionary = null;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, mailboxSession.GetADSessionSettings(), 587, "SendModeratorExpiryNotification", "f:\\15.00.1497\\sources\\dev\\Approval\\src\\Applications\\ModeratedDLApplication\\ModeratedDLApplication.cs");
			DsnHumanReadableWriter defaultDsnHumanReadableWriter = DsnHumanReadableWriter.DefaultDsnHumanReadableWriter;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox);
			if (ClientCultures.IsCultureSupportedForDsn(CultureInfo.CurrentCulture))
			{
				this.defaultFallBackCulture = CultureInfo.CurrentCulture;
			}
			else
			{
				this.defaultFallBackCulture = CultureInfo.GetCultureInfo("en-US");
			}
			if (ApprovalProcessor.TryGetCulturesForDecisionMakers(new List<RoutingAddress>(collection), tenantOrRootOrgRecipientSession, this.defaultFallBackCulture, out dictionary))
			{
				foreach (CultureInfo cultureInfo in dictionary.Keys)
				{
					IList<RoutingAddress> list = dictionary[cultureInfo];
					using (MessageItem messageItem2 = MessageItem.Create(mailboxSession, defaultFolderId))
					{
						ApprovalInformation approvalInformation = null;
						string text;
						this.AttachOriginalMessageToNotification(messageItem, messageItem2, out text);
						if (string.IsNullOrEmpty(text))
						{
							RoutingAddress routingAddress;
							if (!this.TryGetOriginalSender(messageItem, out routingAddress))
							{
								break;
							}
							text = routingAddress.ToString();
						}
						approvalInformation = defaultDsnHumanReadableWriter.GetMessageInModerationModeratorExpiredInformation(notificationType, messageItem.Subject, moderatedRecipients, text, new int?(value), cultureInfo.Name, this.defaultFallBackCulture);
						messageItem2.Subject = approvalInformation.Subject;
						BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, approvalInformation.MessageCharset.Name);
						using (Stream stream = messageItem2.Body.OpenWriteStream(configuration))
						{
							defaultDsnHumanReadableWriter.WriteHtmlModerationBody(stream, approvalInformation);
						}
						this.StampCommonNotificationProperties(messageItem2, messageItem, list, valueOrDefault2, approvalInformation.Culture);
						messageItem2.ClassName = "IPM.Note.Microsoft.Approval.Reply.Reject";
						messageItem2.SendWithoutSavingMessage();
						ModeratedDLApplication.diag.TraceDebug<int, string>((long)this.GetHashCode(), "Expiry Notification sent for {0} decision makers, original message id '{1}'", list.Count, valueOrDefault2);
					}
				}
			}
		}

		private void SendRejectNotification(MessageItem messageItem)
		{
			ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Entering SendRejectNotification");
			if (!this.ShouldSendNotification(messageItem))
			{
				return;
			}
			RoutingAddress routingAddress;
			if (!this.TryGetOriginalSender(messageItem, out routingAddress))
			{
				return;
			}
			MailboxSession mailboxSession = (MailboxSession)messageItem.Session;
			messageItem.Load(ModeratedDLApplication.NotificationPropertiesFromInitMessage);
			string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.AcceptLanguage, string.Empty);
			DsnHumanReadableWriter defaultDsnHumanReadableWriter = DsnHumanReadableWriter.DefaultDsnHumanReadableWriter;
			ICollection<string> moderatedRecipients = this.GetModeratedRecipients(messageItem, false);
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences, string.Empty);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox);
			using (MessageItem messageItem2 = MessageItem.Create(mailboxSession, defaultFolderId))
			{
				StreamAttachment streamAttachment = null;
				try
				{
					bool flag = ModeratedDLApplication.TryGetCommentAttachment(messageItem, out streamAttachment) && streamAttachment.Size > 0L;
					messageItem2.ClassName = "IPM.Note.Microsoft.Approval.Reply.Reject";
					bool flag2 = true;
					ApprovalInformation approvalInformation = null;
					if (flag)
					{
						approvalInformation = defaultDsnHumanReadableWriter.GetModerateRejectInformation(messageItem.Subject, moderatedRecipients, flag, valueOrDefault2, valueOrDefault);
						messageItem2.Subject = approvalInformation.Subject;
						flag2 = this.TryWriteNotificationWithAppendedComments(defaultDsnHumanReadableWriter, messageItem2, streamAttachment, approvalInformation);
					}
					if (!flag || !flag2)
					{
						approvalInformation = defaultDsnHumanReadableWriter.GetModerateRejectInformation(messageItem.Subject, moderatedRecipients, false, valueOrDefault2, valueOrDefault);
						messageItem2.Subject = approvalInformation.Subject;
						BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, approvalInformation.MessageCharset.Name);
						using (Stream stream = messageItem2.Body.OpenWriteStream(configuration))
						{
							defaultDsnHumanReadableWriter.WriteHtmlModerationBody(stream, approvalInformation);
							stream.Flush();
						}
					}
					if (flag && !flag2)
					{
						using (StreamAttachment streamAttachment2 = messageItem2.AttachmentCollection.Create(AttachmentType.Stream) as StreamAttachment)
						{
							streamAttachment2.FileName = defaultDsnHumanReadableWriter.GetModeratorsCommentFileName(approvalInformation.Culture);
							using (Stream contentStream = streamAttachment2.GetContentStream())
							{
								using (Stream contentStream2 = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
								{
									byte[] buffer = new byte[8192];
									ApprovalProcessor.CopyStream(contentStream2, contentStream, buffer);
								}
								contentStream.Flush();
							}
							streamAttachment2.Save();
						}
					}
					this.StampCommonNotificationProperties(messageItem2, messageItem, new RoutingAddress[]
					{
						routingAddress
					}, valueOrDefault2, approvalInformation.Culture);
					messageItem2.SendWithoutSavingMessage();
				}
				finally
				{
					if (streamAttachment != null)
					{
						streamAttachment.Dispose();
					}
				}
			}
		}

		private bool ShouldSendNotification(MessageItem messageItem)
		{
			string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalApplicationData, string.Empty);
			if (!string.IsNullOrEmpty(valueOrDefault) && valueOrDefault.Length >= 1 && valueOrDefault[0] == '0')
			{
				ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Notification suppressed. Skipping.");
				return false;
			}
			return true;
		}

		private bool TryGetOriginalSender(MessageItem messageItem, out RoutingAddress originalSender)
		{
			RoutingAddress routingAddress = (RoutingAddress)messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
			if (!routingAddress.IsValid || routingAddress == RoutingAddress.NullReversePath)
			{
				ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Approval requestor not present or it is invalid");
				return false;
			}
			originalSender = routingAddress;
			return true;
		}

		private void StampCommonNotificationProperties(MessageItem notificationItem, MessageItem initMessageItem, ICollection<RoutingAddress> notificationReceivers, string originalMessageId, CultureInfo cultureInfo)
		{
			foreach (RoutingAddress routingAddress in notificationReceivers)
			{
				Participant participant = new Participant(string.Empty, routingAddress.ToString(), ProxyAddressPrefix.Smtp.ToString());
				notificationItem.Recipients.Add(participant);
			}
			notificationItem.Sender = initMessageItem.Sender;
			notificationItem[MessageItemSchema.IsNonDeliveryReceiptRequested] = false;
			notificationItem[MessageItemSchema.IsDeliveryReceiptRequested] = false;
			notificationItem[MessageItemSchema.MessageLocaleId] = cultureInfo.LCID;
			byte[] conversationIndex = initMessageItem.ConversationIndex;
			notificationItem.ConversationIndex = ConversationIndex.CreateFromParent(conversationIndex).ToByteArray();
			notificationItem[ItemSchema.NormalizedSubject] = initMessageItem.ConversationTopic;
			if (!string.IsNullOrEmpty(originalMessageId))
			{
				notificationItem[ItemSchema.InReplyTo] = originalMessageId;
			}
		}

		private ICollection<string> GetModeratedRecipients(MessageItem messageItem, bool validateAndUseAddressOnly)
		{
			if (messageItem.Recipients.Count == 0)
			{
				ModeratedDLApplication.diag.TraceError((long)this.GetHashCode(), "Init message has no recipients at all.");
				return ModeratedDLApplication.EmptyRecipientList;
			}
			List<string> list = new List<string>(messageItem.Recipients.Count - 1);
			foreach (Recipient recipient in messageItem.Recipients)
			{
				if (recipient.RecipientItemType == RecipientItemType.Cc)
				{
					string text = null;
					if (!validateAndUseAddressOnly)
					{
						text = (recipient.TryGetProperty(RecipientSchema.EmailDisplayName) as string);
					}
					if (string.IsNullOrEmpty(text))
					{
						text = (recipient.TryGetProperty(RecipientSchema.SmtpAddress) as string);
						if (string.IsNullOrEmpty(text))
						{
							ModeratedDLApplication.diag.TraceError((long)this.GetHashCode(), "skipping moderated recipient without address or display name");
							continue;
						}
						if (validateAndUseAddressOnly && !SmtpAddress.IsValidSmtpAddress(text))
						{
							ModeratedDLApplication.diag.TraceError((long)this.GetHashCode(), "skipping moderated recipient without a valid address");
							continue;
						}
					}
					list.Add(text);
				}
			}
			return list;
		}

		private void AttachOriginalMessageToNotification(MessageItem initiationMessage, MessageItem notificationMessage, out string originalSenderDisplayname)
		{
			originalSenderDisplayname = string.Empty;
			if (string.IsNullOrEmpty(this.defaultAcceptedDomain))
			{
				ModeratedDLApplication.diag.TraceDebug((long)this.GetHashCode(), "Cannot attach original message to notification without domain for content conversion.");
				return;
			}
			AttachmentCollection attachmentCollection = initiationMessage.AttachmentCollection;
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					if ("OriginalMessage".Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase))
					{
						StreamAttachment streamAttachment = attachment as StreamAttachment;
						if (streamAttachment != null)
						{
							using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
							{
								using (ItemAttachment itemAttachment = (ItemAttachment)notificationMessage.AttachmentCollection.Create(AttachmentType.EmbeddedMessage))
								{
									using (Item item = itemAttachment.GetItem())
									{
										InboundConversionOptions options = new InboundConversionOptions(this.defaultAcceptedDomain);
										ItemConversion.ConvertAnyMimeToItem(item, contentStream, options);
										item[MessageItemSchema.Flags] = MessageFlags.None;
										originalSenderDisplayname = (item.TryGetProperty(MessageItemSchema.SenderDisplayName) as string);
										item.Save(SaveMode.NoConflictResolution);
										itemAttachment[AttachmentSchema.DisplayName] = initiationMessage.Subject;
										itemAttachment.Save();
									}
								}
							}
						}
						break;
					}
				}
			}
		}

		private bool TryWriteNotificationWithAppendedComments(DsnHumanReadableWriter notificationWriter, MessageItem rejectItem, StreamAttachment commentAttachment, ApprovalInformation info)
		{
			bool result = true;
			string htmlModerationBody = notificationWriter.GetHtmlModerationBody(info);
			Charset textCharset = commentAttachment.TextCharset;
			Encoding inputEncoding = null;
			if (textCharset == null || !textCharset.TryGetEncoding(out inputEncoding))
			{
				return false;
			}
			Charset charset = textCharset;
			if (!ModeratedDLApplication.IsEncodingMatch(info.Codepages, textCharset.CodePage))
			{
				charset = Charset.UTF8;
			}
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, charset.Name);
			using (Stream stream = rejectItem.Body.OpenWriteStream(configuration))
			{
				HtmlToHtml htmlToHtml = new HtmlToHtml();
				htmlToHtml.Header = htmlModerationBody;
				htmlToHtml.HeaderFooterFormat = HeaderFooterFormat.Html;
				htmlToHtml.InputEncoding = inputEncoding;
				htmlToHtml.OutputEncoding = charset.GetEncoding();
				try
				{
					using (Stream contentStream = commentAttachment.GetContentStream(PropertyOpenMode.ReadOnly))
					{
						htmlToHtml.Convert(contentStream, stream);
						stream.Flush();
					}
				}
				catch (ExchangeDataException arg)
				{
					ModeratedDLApplication.diag.TraceDebug<ExchangeDataException>(0L, "Attaching comments failed with {0}", arg);
					result = false;
				}
			}
			return result;
		}

		private static readonly Trace diag = ExTraceGlobals.ModeratedTransportTracer;

		private static readonly PropertyDefinition[] NotificationPropertiesFromInitMessage = new PropertyDefinition[]
		{
			MessageItemSchema.AcceptLanguage
		};

		private static readonly string[] EmptyRecipientList = new string[0];

		private static readonly object SyncRoot = new object();

		private static int refCount;

		private ADNotificationRequestCookie serverConfigNotificationCookie;

		private string defaultAcceptedDomain;

		private CultureInfo defaultFallBackCulture;

		private enum ApplicationDataField
		{
			SuppressNotification
		}

		private enum NotificationType
		{
			Expiry,
			Ndr,
			Oof
		}

		private class ExpiryNotificationCounterWrapper
		{
			private ExpiryNotificationCounterWrapper()
			{
				this.expiryNotificationinLastHour = new SlidingTotalCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			}

			public static ModeratedDLApplication.ExpiryNotificationCounterWrapper Instance
			{
				get
				{
					return ModeratedDLApplication.ExpiryNotificationCounterWrapper.instance;
				}
			}

			public bool IncrementCountAndCheckLimit()
			{
				if (this.expiryNotificationinLastHour.Sum >= 300L)
				{
					return false;
				}
				this.expiryNotificationinLastHour.AddValue(1L);
				return true;
			}

			private const int MaxNotificationNumberPerHour = 300;

			private static ModeratedDLApplication.ExpiryNotificationCounterWrapper instance = new ModeratedDLApplication.ExpiryNotificationCounterWrapper();

			private SlidingTotalCounter expiryNotificationinLastHour;
		}
	}
}
