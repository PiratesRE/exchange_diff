using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Entity;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class SmartForwardCommand : SendMailBase
	{
		public SmartForwardCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSmartForwards;
		}

		internal ICalendaringContainer CalendaringContainer { get; set; }

		internal IStoreSession StoreSession { get; set; }

		protected override string RootNodeName
		{
			get
			{
				return "SmartForward";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		internal override void ParseXmlRequest()
		{
			base.ParseXmlRequest();
			if (base.Version >= 160)
			{
				if (base.XmlRequest["Mime"] != null)
				{
					if (base.XmlRequest["Importance"] != null || base.XmlRequest["Body", "AirSyncBase:"] != null || base.XmlRequest["Forwardees"] != null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidMimeTags");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidMimeBodyCombination, null, false);
					}
				}
				else
				{
					if (base.XmlRequest["SaveInSentItems"] != null || base.XmlRequest["ReplaceMime"] != null || base.XmlRequest["TemplateID", "RightsManagement:"] != null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidBodyTags");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidMimeBodyCombination, null, false);
					}
					try
					{
						this.forwardEventParameters = EventParametersParser.ParseForward(base.Request.CommandXml);
					}
					catch (RequestParsingException ex)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, ex.LogMessage);
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidSmartForwardParameters, null, false);
					}
				}
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			base.ValidateBody();
			StoreObjectId smartItemId = base.GetSmartItemId();
			if (base.Version >= 160 && (smartItemId.ObjectType == StoreObjectType.CalendarItem || smartItemId.ObjectType == StoreObjectType.CalendarItemOccurrence || smartItemId.ObjectType == StoreObjectType.CalendarItemSeries))
			{
				return this.ForwardUsingEntities(smartItemId);
			}
			return this.ForwardUsingXso(smartItemId);
		}

		private Command.ExecutionState ForwardUsingEntities(StoreObjectId smartId)
		{
			if (base.Occurrence != ExDateTime.MinValue)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoOccurrenceSupport");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.ServerError, null, false);
			}
			if (this.CalendaringContainer == null)
			{
				this.CalendaringContainer = new CalendaringContainer(base.MailboxSession, null);
			}
			if (this.StoreSession == null)
			{
				this.StoreSession = base.MailboxSession;
			}
			string key = EntitySyncItem.GetKey(this.StoreSession.MailboxGuid, smartId);
			IEvents events = EntitySyncItem.GetEvents(this.CalendaringContainer, this.StoreSession, smartId);
			events.Forward(key, this.forwardEventParameters, null);
			return Command.ExecutionState.Complete;
		}

		private Command.ExecutionState ForwardUsingXso(StoreObjectId smartId)
		{
			Item smartItem = base.GetSmartItem(smartId);
			MessageItem messageItem = null;
			VersionedId versionedId = null;
			MessageItem messageItem2 = null;
			CalendarItemBase calendarItemBase = null;
			try
			{
				StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				messageItem = MessageItem.Create(base.MailboxSession, defaultFolderId);
				base.ParseMimeToMessage(messageItem);
				messageItem.Save(SaveMode.NoConflictResolution);
				messageItem.Load();
				versionedId = messageItem.Id;
				messageItem.Dispose();
				messageItem = MessageItem.Bind(base.MailboxSession, versionedId);
				RmsTemplate rmsTemplate = null;
				SendMailBase.IrmAction irmAction = base.GetIrmAction(delegate(RightsManagedMessageItem originalRightsManagedItem)
				{
					if (originalRightsManagedItem == null)
					{
						throw new ArgumentNullException("originalRightsManagedItem");
					}
					if (!originalRightsManagedItem.UsageRights.IsUsageRightGranted(ContentRight.Forward))
					{
						throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, false)
						{
							ErrorStringForProtocolLogger = "sfcEOperationNotPermitted"
						};
					}
				}, ref smartItem, out rmsTemplate);
				Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat = messageItem.Body.Format;
				MeetingMessage meetingMessage = smartItem as MeetingMessage;
				string text;
				if ((base.ReplaceMime || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage) && meetingMessage != null && !meetingMessage.IsDelegated() && (meetingMessage is MeetingRequest || meetingMessage is MeetingCancellation))
				{
					text = string.Empty;
				}
				else
				{
					using (TextReader textReader = messageItem.Body.OpenTextReader(bodyFormat))
					{
						text = textReader.ReadToEnd();
					}
					Body body = (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.ReusePublishingLicenseInlineOriginalBody) ? ((RightsManagedMessageItem)smartItem).ProtectedBody : smartItem.Body;
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
				if (base.Version >= 120)
				{
					if (smartItem is MessageItem)
					{
						messageItem2 = ((MessageItem)smartItem).CreateForward(defaultFolderId, replyForwardConfiguration);
						if (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicense || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseInlineOriginalBody || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
						{
							messageItem2 = base.GetRightsManagedReplyForward(messageItem2, irmAction, rmsTemplate);
						}
					}
					else if (smartItem is CalendarItem)
					{
						CalendarItem calendarItem = (CalendarItem)smartItem;
						calendarItemBase = base.GetCalendarItemBaseToReplyOrForward(calendarItem);
						messageItem2 = calendarItemBase.CreateForward(defaultFolderId, replyForwardConfiguration);
						if (!calendarItem.IsMeeting)
						{
							BodyConversionUtilities.CopyBody(messageItem, messageItem2);
						}
					}
					if (messageItem2 == null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ForwardFailed");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed, null, false);
					}
					if (base.ReplaceMime || irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage)
					{
						RightsManagedMessageItem rightsManagedMessageItem = messageItem2 as RightsManagedMessageItem;
						if (rightsManagedMessageItem != null)
						{
							rightsManagedMessageItem.ProtectedAttachmentCollection.RemoveAll();
						}
						else
						{
							messageItem2.AttachmentCollection.RemoveAll();
						}
					}
					base.CopyMessageContents(messageItem, messageItem2, false, (irmAction == SendMailBase.IrmAction.CreateNewPublishingLicenseAttachOriginalMessage) ? smartItem : null);
					base.SendMessage(messageItem2);
				}
				else if (smartItem is MessageItem)
				{
					using (ItemAttachment itemAttachment = messageItem.AttachmentCollection.AddExistingItem(smartItem))
					{
						MessageItem messageItem3 = (MessageItem)smartItem;
						itemAttachment.FileName = messageItem3.Subject + itemAttachment.FileExtension;
						itemAttachment.Save();
					}
					base.SendMessage(messageItem);
				}
				else if (smartItem is CalendarItem)
				{
					CalendarItem calendarItem2 = (CalendarItem)smartItem;
					messageItem2 = calendarItem2.CreateForward(defaultFolderId, replyForwardConfiguration);
					if (messageItem2 == null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ForwardFailed2");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MailSubmissionFailed, null, false);
					}
					if (!calendarItem2.IsMeeting)
					{
						BodyConversionUtilities.CopyBody(messageItem, messageItem2);
					}
					base.CopyMessageContents(messageItem, messageItem2, false, null);
					base.SendMessage(messageItem2);
				}
			}
			finally
			{
				if (messageItem != null)
				{
					if (versionedId != null)
					{
						base.MailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							versionedId
						});
					}
					messageItem.Dispose();
				}
				if (smartItem != null)
				{
					smartItem.Dispose();
				}
				if (messageItem2 != null)
				{
					messageItem2.Dispose();
				}
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
				}
			}
			return Command.ExecutionState.Complete;
		}

		private ForwardEventParameters forwardEventParameters;
	}
}
