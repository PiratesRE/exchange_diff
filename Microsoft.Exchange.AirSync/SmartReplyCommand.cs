using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class SmartReplyCommand : SendMailBase
	{
		public SmartReplyCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSmartReplys;
		}

		protected override string RootNodeName
		{
			get
			{
				return "SmartReply";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			MessageItem clientMessage = null;
			Item item = null;
			MessageItem messageItem = null;
			VersionedId versionedId = null;
			CalendarItemBase calendarItemBase = null;
			try
			{
				base.ValidateBody();
				StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				clientMessage = MessageItem.Create(base.MailboxSession, defaultFolderId);
				base.ParseMimeToMessage(clientMessage);
				clientMessage.Save(SaveMode.NoConflictResolution);
				clientMessage.Load();
				versionedId = clientMessage.Id;
				clientMessage.Dispose();
				clientMessage = MessageItem.Bind(base.MailboxSession, versionedId);
				item = base.GetSmartItem();
				RmsTemplate rmsTemplate = null;
				bool isReplyAll = false;
				SendMailBase.IrmAction irmAction = base.GetIrmAction(delegate(RightsManagedMessageItem originalRightsManagedItem)
				{
					if (originalRightsManagedItem == null)
					{
						throw new ArgumentNullException("originalRightsManagedItem");
					}
					isReplyAll = this.IsIrmReplyAll(originalRightsManagedItem, clientMessage);
				}, ref item, out rmsTemplate);
				Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat = clientMessage.Body.Format;
				MeetingMessage meetingMessage = item as MeetingMessage;
				string text;
				if ((base.ReplaceMime || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage) && meetingMessage != null && !meetingMessage.IsDelegated() && (meetingMessage is MeetingCancellation || meetingMessage is MeetingRequest))
				{
					text = string.Empty;
				}
				else
				{
					using (TextReader textReader = clientMessage.Body.OpenTextReader(bodyFormat))
					{
						text = textReader.ReadToEnd();
					}
					Body body = (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.ReusePublishingLicenseInlineOriginalBody) ? ((RightsManagedMessageItem)item).ProtectedBody : item.Body;
					if (body.Format == Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml)
					{
						if (bodyFormat == Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain)
						{
							XmlDocument xmlDocument = new SafeXmlDocument();
							XmlNode xmlNode = xmlDocument.CreateElement("PRE");
							XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("STYLE");
							xmlAttribute.Value = "word-wrap:break-word; font-size:10.0pt; font-family:Tahoma; color:black";
							xmlNode.Attributes.Append(xmlAttribute);
							xmlNode.InnerText = text;
							text = xmlNode.OuterXml;
						}
						bodyFormat = Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml;
					}
				}
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(bodyFormat);
				replyForwardConfiguration.ConversionOptionsForSmime = AirSyncUtility.GetInboundConversionOptions();
				replyForwardConfiguration.AddBodyPrefix(text);
				if (item is MessageItem)
				{
					MessageItem messageItem2 = (MessageItem)item;
					if (!messageItem2.IsReplyAllowed)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ReplyNotAllowed");
						AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MessageReplyNotAllowed, null, false);
						throw ex;
					}
					if (isReplyAll)
					{
						messageItem = messageItem2.CreateReplyAll(defaultFolderId, replyForwardConfiguration);
					}
					else
					{
						messageItem = messageItem2.CreateReply(defaultFolderId, replyForwardConfiguration);
					}
					if (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicense || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
					{
						messageItem = base.GetRightsManagedReplyForward(messageItem, irmAction, rmsTemplate);
					}
				}
				else if (item is CalendarItem)
				{
					CalendarItem item2 = (CalendarItem)item;
					calendarItemBase = base.GetCalendarItemBaseToReplyOrForward(item2);
					messageItem = calendarItemBase.CreateReply(defaultFolderId, replyForwardConfiguration);
				}
				if (messageItem == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ReplyFailed");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed, null, false);
				}
				base.CopyMessageContents(clientMessage, messageItem, true, (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage) ? item : null);
				base.SendMessage(messageItem);
			}
			finally
			{
				if (clientMessage != null)
				{
					if (versionedId != null)
					{
						base.MailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							versionedId
						});
					}
					clientMessage.Dispose();
				}
				if (item != null)
				{
					item.Dispose();
				}
				if (messageItem != null)
				{
					messageItem.Dispose();
				}
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
				}
			}
			return Command.ExecutionState.Complete;
		}

		private static bool IsPresent(Participant recipient, IList<Participant> participants)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (participants == null || participants.Count == 0)
			{
				throw new ArgumentNullException("participants");
			}
			foreach (Participant participant in participants)
			{
				if (Participant.HasSameEmail(recipient, participant, true))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsIrmReplyAll(RightsManagedMessageItem originalMessage, MessageItem newMessage)
		{
			if (originalMessage == null)
			{
				throw new ArgumentNullException("originalMessage");
			}
			if (newMessage == null)
			{
				throw new ArgumentNullException("newMessage");
			}
			bool flag = true;
			Participant participant = new Participant(base.User.ExchangePrincipal);
			IList<Participant> list = new List<Participant>(originalMessage.ReplyTo);
			list.Add(originalMessage.Sender);
			list.Add(originalMessage.From);
			foreach (Recipient recipient in newMessage.Recipients)
			{
				if (!Participant.HasSameEmail(recipient.Participant, participant, true) && !SmartReplyCommand.IsPresent(recipient.Participant, list))
				{
					flag = false;
					break;
				}
			}
			bool flag2 = !flag;
			if (flag2)
			{
				foreach (Recipient recipient2 in originalMessage.Recipients)
				{
					if (!Participant.HasSameEmail(recipient2.Participant, participant, true) && !SmartReplyCommand.IsPresent(recipient2.Participant, list) && !newMessage.Recipients.Contains(recipient2.Participant, true))
					{
						flag2 = false;
						break;
					}
				}
			}
			if (flag2)
			{
				foreach (Recipient recipient3 in newMessage.Recipients)
				{
					if (!Participant.HasSameEmail(recipient3.Participant, participant, true) && !SmartReplyCommand.IsPresent(recipient3.Participant, list) && !originalMessage.Recipients.Contains(recipient3.Participant, true))
					{
						flag2 = false;
						break;
					}
				}
			}
			if (flag2 && !SmartReplyCommand.IsPresent(participant, list))
			{
				bool flag3 = false;
				foreach (Participant participant2 in list)
				{
					if (newMessage.Recipients.Contains(participant2, true))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					flag2 = false;
				}
			}
			bool flag4 = originalMessage.UsageRights.IsUsageRightGranted(ContentRight.Reply);
			bool flag5 = originalMessage.UsageRights.IsUsageRightGranted(ContentRight.ReplyAll);
			bool flag6 = originalMessage.UsageRights.IsUsageRightGranted(ContentRight.Forward);
			if (flag6 || (flag4 && flag5 && (flag || flag2)) || (flag4 && !flag5 && flag && !flag2) || (!flag4 && flag5 && !flag && flag2))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "IsIrmReplyAll: isReplyAllowed={0}; isReplyAllAllowed={1}; isForwardAllowed={2}; isReplyAll={3}", new object[]
				{
					flag4,
					flag5,
					flag6,
					flag2
				});
				if ((flag2 && flag5) || (!flag2 && flag4))
				{
					return flag2;
				}
			}
			throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, false)
			{
				ErrorStringForProtocolLogger = "srcIiraOperationNotPermitted"
			};
		}
	}
}
