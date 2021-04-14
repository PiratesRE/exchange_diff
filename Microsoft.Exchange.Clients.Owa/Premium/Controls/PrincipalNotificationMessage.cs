using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PrincipalNotificationMessage
	{
		internal PrincipalNotificationMessage(string itemId, OwaStoreObjectId folderId, UserContext userContext, HttpContext httpContext, PrincipalNotificationMessage.ActionType actionType, bool isNewItem, bool isMeeting)
		{
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			this.itemId = itemId;
			this.folderId = folderId;
			this.userContext = userContext;
			this.httpContext = httpContext;
			this.actionType = actionType;
			this.isNewItem = isNewItem;
			this.isMeeting = isMeeting;
			this.GetMessageAndTitleTypes();
		}

		internal void SendNotificationMessage()
		{
			string text = this.CapturePrintMeetingMarkup();
			text = string.Format("<div style=\"font-size: 70%; font-family:'{0}'\">{1}</div><br>{2}", Utilities.GetDefaultFontName(), LocalizedStrings.GetHtmlEncoded(PrincipalNotificationMessage.NotificationMessages[(int)this.messageType]), text);
			using (MessageItem messageItem = MessageItem.Create(this.userContext.MailboxSession, this.userContext.DraftsFolderId))
			{
				messageItem.Subject = LocalizedStrings.GetNonEncoded(PrincipalNotificationMessage.NotificationMessageTitles[(int)this.titleType]);
				BodyConversionUtilities.SetBody(messageItem, text, Markup.Html, StoreObjectType.Message, this.userContext);
				messageItem[ItemSchema.ConversationIndexTracking] = true;
				IExchangePrincipal folderOwnerExchangePrincipal = Utilities.GetFolderOwnerExchangePrincipal(this.folderId, this.userContext);
				Participant participant = new Participant(folderOwnerExchangePrincipal);
				messageItem.Recipients.Add(participant, RecipientItemType.To);
				try
				{
					messageItem.SendWithoutSavingMessage();
				}
				catch
				{
					ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Error sending principal notification message.");
					throw;
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsCreated.Increment();
				OwaSingleCounters.MessagesSent.Increment();
			}
		}

		private void GetMessageAndTitleTypes()
		{
			switch (this.actionType)
			{
			case PrincipalNotificationMessage.ActionType.Send:
				if (this.isNewItem)
				{
					this.titleType = PrincipalNotificationMessage.TitleType.MeetingCreatedNotification;
					this.messageType = PrincipalNotificationMessage.MessageType.SendNewMeetingRequest;
					return;
				}
				this.titleType = PrincipalNotificationMessage.TitleType.MeetingChangedNotification;
				this.messageType = PrincipalNotificationMessage.MessageType.SendUpdateForExistingMeetingRequest;
				return;
			case PrincipalNotificationMessage.ActionType.Save:
				if (this.isNewItem)
				{
					if (this.isMeeting)
					{
						this.titleType = PrincipalNotificationMessage.TitleType.MeetingCreatedNotification;
						this.messageType = PrincipalNotificationMessage.MessageType.SaveNewMeetingRequest;
						return;
					}
					this.titleType = PrincipalNotificationMessage.TitleType.AppointmentCreatedNotification;
					this.messageType = PrincipalNotificationMessage.MessageType.SaveNewAppointment;
					return;
				}
				else
				{
					if (this.isMeeting)
					{
						this.titleType = PrincipalNotificationMessage.TitleType.MeetingChangedNotification;
						this.messageType = PrincipalNotificationMessage.MessageType.SaveExistingMeetingRequest;
						return;
					}
					this.titleType = PrincipalNotificationMessage.TitleType.AppointmentChangedNotification;
					this.messageType = PrincipalNotificationMessage.MessageType.SaveExistingAppointment;
					return;
				}
				break;
			case PrincipalNotificationMessage.ActionType.Delete:
				if (!this.isMeeting)
				{
					this.titleType = PrincipalNotificationMessage.TitleType.AppointmentCancellationNotification;
					this.messageType = PrincipalNotificationMessage.MessageType.DeleteAppointment;
					return;
				}
				break;
			case PrincipalNotificationMessage.ActionType.Cancel:
				this.titleType = PrincipalNotificationMessage.TitleType.MeetingCancellationNotification;
				this.messageType = PrincipalNotificationMessage.MessageType.CancelMeetingRequest;
				return;
			case PrincipalNotificationMessage.ActionType.Move:
				if (this.isMeeting)
				{
					this.titleType = PrincipalNotificationMessage.TitleType.MeetingChangedNotification;
					this.messageType = PrincipalNotificationMessage.MessageType.MoveMeetingRequest;
					return;
				}
				this.titleType = PrincipalNotificationMessage.TitleType.AppointmentChangedNotification;
				this.messageType = PrincipalNotificationMessage.MessageType.MoveAppointment;
				break;
			default:
				return;
			}
		}

		private string CapturePrintMeetingMarkup()
		{
			string path = string.Format(CultureInfo.InvariantCulture, "forms/premium/PrintMeetingPage.aspx?ae=Item&t=IPM.Appointment&id={0}", new object[]
			{
				Utilities.UrlEncode(this.itemId)
			});
			StringBuilder sb = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				this.httpContext.Server.Execute(path, stringWriter);
				result = stringWriter.ToString();
			}
			return result;
		}

		private static readonly Strings.IDs[] NotificationMessageTitles = new Strings.IDs[]
		{
			-103948802,
			1718903530,
			1474482155,
			904403258,
			1104105618,
			900208633
		};

		private static readonly Strings.IDs[] NotificationMessages = new Strings.IDs[]
		{
			1759358345,
			-737562370,
			887581567,
			-1622812136,
			1378908170,
			-1597738953,
			-516697337,
			-653709769,
			987522774,
			-1700473406
		};

		private string itemId;

		private OwaStoreObjectId folderId;

		private UserContext userContext;

		private HttpContext httpContext;

		private PrincipalNotificationMessage.ActionType actionType;

		private bool isNewItem;

		private bool isMeeting;

		private PrincipalNotificationMessage.TitleType titleType;

		private PrincipalNotificationMessage.MessageType messageType;

		public enum TitleType
		{
			MeetingChangedNotification,
			MeetingCreatedNotification,
			MeetingCancellationNotification,
			AppointmentChangedNotification,
			AppointmentCreatedNotification,
			AppointmentCancellationNotification
		}

		public enum MessageType
		{
			SendNewMeetingRequest,
			SendUpdateForExistingMeetingRequest,
			SaveNewAppointment,
			SaveNewMeetingRequest,
			SaveExistingAppointment,
			SaveExistingMeetingRequest,
			DeleteAppointment,
			CancelMeetingRequest,
			MoveMeetingRequest,
			MoveAppointment
		}

		public enum ActionType
		{
			Send,
			Save,
			Delete,
			Cancel,
			Move
		}
	}
}
