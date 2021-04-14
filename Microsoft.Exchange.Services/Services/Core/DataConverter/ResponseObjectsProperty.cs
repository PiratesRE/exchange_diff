using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ResponseObjectsProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		public ResponseObjectsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ResponseObjectsProperty CreateCommand(CommandContext commandContext)
		{
			return new ResponseObjectsProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ResponseObjectsProperty.ToXml should not be called.");
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			this.serviceObject = commandSettings.ServiceObject;
			this.responseObjects = new List<ResponseObjectType>();
			CalendarItemBase calendarItemBase = null;
			MeetingMessage meetingMessage = null;
			MessageItem messageItem = null;
			PostItem postItem = null;
			if (XsoDataConverter.TryGetStoreObject<CalendarItemBase>(storeObject, out calendarItemBase))
			{
				this.CreateCalendarItemBaseResponseObjects(calendarItemBase);
			}
			else if (XsoDataConverter.TryGetStoreObject<MeetingMessage>(storeObject, out meetingMessage))
			{
				this.CreateMeetingMessageResponseObjects(meetingMessage);
			}
			else if (XsoDataConverter.TryGetStoreObject<MessageItem>(storeObject, out messageItem))
			{
				this.CreateMessageResponseObjects(messageItem);
			}
			else if (XsoDataConverter.TryGetStoreObject<PostItem>(storeObject, out postItem) && ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
			{
				this.CreatePostItemResponseObjects();
			}
			if (this.responseObjects.Count > 0)
			{
				this.serviceObject[this.commandContext.PropertyInformation] = this.responseObjects.ToArray();
			}
		}

		private static bool IsMessageInJunkEmailFolder(MessageItem messageItem)
		{
			StoreObjectId parentId;
			try
			{
				parentId = messageItem.ParentId;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			MailboxSession mailboxSession = messageItem.Session as MailboxSession;
			return mailboxSession != null && mailboxSession.IsDefaultFolderType(parentId) == DefaultFolderType.JunkEmail;
		}

		private void CreateMessageResponseObjects(MessageItem messageItem)
		{
			if ((!Shape.IsGenericMessageOnly(messageItem) || ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1)) && !messageItem.ClassName.StartsWith("rpmsg.message", StringComparison.OrdinalIgnoreCase))
			{
				this.CreateForwardReplyResponseObjects(messageItem);
				this.CreateSuppressReadReceiptResponseObject(messageItem);
			}
		}

		private void CreateForwardReplyResponseObjects(MessageItem messageItem)
		{
			if (!messageItem.IsDraft && messageItem.IsReplyAllowed)
			{
				this.responseObjects.Add(new ReplyToItemType());
				this.responseObjects.Add(new ReplyAllToItemType());
			}
			this.responseObjects.Add(new ForwardItemType());
		}

		private void CreateSuppressReadReceiptResponseObject(MessageItem messageItem)
		{
			if ((bool)messageItem[this.propertyDefinitions[0]] && messageItem.Session is MailboxSession && !ResponseObjectsProperty.IsMessageInJunkEmailFolder(messageItem))
			{
				this.responseObjects.Add(new SuppressReadReceiptType());
			}
		}

		private void CreateCalendarItemBaseResponseObjects(CalendarItemBase calendarItemBase)
		{
			if (calendarItemBase.IsOrganizer())
			{
				if (calendarItemBase.IsMeeting && calendarItemBase.Session is MailboxSession)
				{
					this.responseObjects.Add(new CancelCalendarItemType());
				}
			}
			else
			{
				MailboxSession mailboxSession = calendarItemBase.Session as MailboxSession;
				bool flag = mailboxSession != null && mailboxSession.IsGroupMailbox();
				if (flag)
				{
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
					{
						this.CreateGroupMailboxResponseObjects(calendarItemBase.IsCancelled);
					}
				}
				else
				{
					if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) || !calendarItemBase.IsCancelled)
					{
						this.CreateAcceptTentativeDeclineResponseObjects(mailboxSession);
					}
					this.CreateProposeNewTimeResponseObject(calendarItemBase);
				}
			}
			if ((ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) && calendarItemBase.IsMeeting) || !calendarItemBase.IsOrganizer())
			{
				this.responseObjects.Add(new ReplyToItemType());
				this.responseObjects.Add(new ReplyAllToItemType());
			}
			this.responseObjects.Add(new ForwardItemType());
		}

		private void CreateMeetingMessageResponseObjects(MeetingMessage meetingMessage)
		{
			MailboxSession mailboxSession = meetingMessage.Session as MailboxSession;
			bool flag = mailboxSession != null && mailboxSession.IsGroupMailbox();
			if (meetingMessage is MeetingCancellation && meetingMessage.Session is MailboxSession)
			{
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) || !meetingMessage.IsOrganizer())
				{
					this.responseObjects.Add(new RemoveItemType());
				}
			}
			else if (meetingMessage is MeetingRequest)
			{
				if (flag)
				{
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
					{
						this.CreateGroupMailboxResponseObjects(meetingMessage.IsOutOfDate());
					}
				}
				else
				{
					if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) || (!meetingMessage.IsOrganizer() && !meetingMessage.IsOutOfDate()))
					{
						this.CreateAcceptTentativeDeclineResponseObjects(mailboxSession);
					}
					this.CreateProposeNewTimeResponseObject((MeetingRequest)meetingMessage);
				}
			}
			this.CreateForwardReplyResponseObjects(meetingMessage);
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) || !flag)
			{
				this.CreateSuppressReadReceiptResponseObject(meetingMessage);
			}
		}

		private void CreateGroupMailboxResponseObjects(bool isMeetingOutOfDateOrCancelled)
		{
			if (!isMeetingOutOfDateOrCancelled)
			{
				this.responseObjects.Add(new AddItemToMyCalendarType());
			}
		}

		private void CreateAcceptTentativeDeclineResponseObjects(MailboxSession mailboxSession)
		{
			if (mailboxSession != null)
			{
				this.responseObjects.Add(new AcceptItemType());
				this.responseObjects.Add(new TentativelyAcceptItemType());
				this.responseObjects.Add(new DeclineItemType());
			}
		}

		private void CreateProposeNewTimeResponseObject(MeetingRequest meetingRequest)
		{
			if (meetingRequest != null && meetingRequest.Session is MailboxSession && ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2013_SP1) && !meetingRequest.IsOrganizer() && !meetingRequest.IsOutOfDate() && meetingRequest.AllowNewTimeProposal && !meetingRequest.IsRecurringMaster)
			{
				this.responseObjects.Add(new ProposeNewTimeType());
			}
		}

		private void CreateProposeNewTimeResponseObject(CalendarItemBase calendarItem)
		{
			if (calendarItem != null && calendarItem.Session is MailboxSession && ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2013_SP1) && !calendarItem.IsOrganizer() && !calendarItem.IsCancelled && calendarItem.AllowNewTimeProposal && calendarItem.CalendarItemType != CalendarItemType.RecurringMaster)
			{
				this.responseObjects.Add(new ProposeNewTimeType());
			}
		}

		private void CreatePostItemResponseObjects()
		{
			this.responseObjects.Add(new PostReplyItemType());
			this.responseObjects.Add(new ReplyToItemType());
			this.responseObjects.Add(new ReplyAllToItemType());
			this.responseObjects.Add(new ForwardItemType());
		}

		private ServiceObject serviceObject;

		private List<ResponseObjectType> responseObjects;
	}
}
