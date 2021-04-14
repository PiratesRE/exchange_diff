using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class ToolbarButtons
	{
		private ToolbarButtons()
		{
		}

		public static ToolbarButton Actions
		{
			get
			{
				return ToolbarButtons.actions;
			}
		}

		public static ToolbarButton AddressBook
		{
			get
			{
				return ToolbarButtons.addressBook;
			}
		}

		public static ToolbarButton AddToContacts
		{
			get
			{
				return ToolbarButtons.addToContacts;
			}
		}

		public static ToolbarButton ApprovalApprove
		{
			get
			{
				return ToolbarButtons.approvalApprove;
			}
		}

		public static ToolbarButton ApprovalApproveMenu
		{
			get
			{
				return ToolbarButtons.approvalApproveMenu;
			}
		}

		public static ToolbarButton ApprovalReject
		{
			get
			{
				return ToolbarButtons.approvalReject;
			}
		}

		public static ToolbarButton ApprovalRejectMenu
		{
			get
			{
				return ToolbarButtons.approvalRejectMenu;
			}
		}

		public static ToolbarButton ApprovalEditResponse
		{
			get
			{
				return ToolbarButtons.approvalEditResponse;
			}
		}

		public static ToolbarButton ApprovalSendResponseNow
		{
			get
			{
				return ToolbarButtons.approvalSendResponseNow;
			}
		}

		public static ToolbarButton AttachFile
		{
			get
			{
				return ToolbarButtons.attachFile;
			}
		}

		public static ToolbarButton InsertImage
		{
			get
			{
				return ToolbarButtons.insertImage;
			}
		}

		public static ToolbarButton CalendarTitle
		{
			get
			{
				return ToolbarButtons.calendarTitle;
			}
		}

		public static ToolbarButton CheckMessages
		{
			get
			{
				return ToolbarButtons.checkMessages;
			}
		}

		public static ToolbarButton CheckNames
		{
			get
			{
				return ToolbarButtons.checkNames;
			}
		}

		public static ToolbarButton Compliance
		{
			get
			{
				return ToolbarButtons.compliance;
			}
		}

		public static ToolbarButton Edit
		{
			get
			{
				return ToolbarButtons.edit;
			}
		}

		public static ToolbarButton DayView
		{
			get
			{
				return ToolbarButtons.dayView;
			}
		}

		public static ToolbarButton Delete
		{
			get
			{
				return ToolbarButtons.delete;
			}
		}

		public static ToolbarButton DeleteTextOnly
		{
			get
			{
				return ToolbarButtons.deleteTextOnly;
			}
		}

		public static ToolbarButton DeleteCombo
		{
			get
			{
				return ToolbarButtons.deleteCombo;
			}
		}

		public static ToolbarButton DeleteInDropDown
		{
			get
			{
				return ToolbarButtons.deleteInDropDown;
			}
		}

		public static ToolbarButton DeleteWithText
		{
			get
			{
				return ToolbarButtons.deleteWithText;
			}
		}

		public static ToolbarButton IgnoreConversation
		{
			get
			{
				return ToolbarButtons.ignoreConversation;
			}
		}

		public static ToolbarButton CancelIgnoreConversationCombo
		{
			get
			{
				return ToolbarButtons.cancelIgnoreConversationCombo;
			}
		}

		public static ToolbarButton CancelIgnoreConversationInDropDown
		{
			get
			{
				return ToolbarButtons.cancelIgnoreConversationInDropDown;
			}
		}

		public static ToolbarButton DeleteInCancelIgnoreConversationDropDown
		{
			get
			{
				return ToolbarButtons.deleteInCancelIgnoreConversationDropDown;
			}
		}

		public static ToolbarButton Flag
		{
			get
			{
				return ToolbarButtons.flag;
			}
		}

		public static ToolbarButton Categories
		{
			get
			{
				return ToolbarButtons.categories;
			}
		}

		public static ToolbarButton SearchInPublicFolder
		{
			get
			{
				return ToolbarButtons.searchInPublicFolder;
			}
		}

		public static ToolbarButton ChangeSharingPermissions
		{
			get
			{
				return ToolbarButtons.changeSharingPermissions;
			}
		}

		public static ToolbarButton ShareCalendar
		{
			get
			{
				return ToolbarButtons.shareCalendar;
			}
		}

		public static ToolbarButton OpenSharedCalendar
		{
			get
			{
				return ToolbarButtons.openSharedCalendar;
			}
		}

		public static ToolbarButton ShareACalendar
		{
			get
			{
				return ToolbarButtons.shareACalendar;
			}
		}

		public static ToolbarButton RequestSharedCalendar
		{
			get
			{
				return ToolbarButtons.requestSharedCalendar;
			}
		}

		public static ToolbarButton ShareContact
		{
			get
			{
				return ToolbarButtons.shareContact;
			}
		}

		public static ToolbarButton OpenSharedContact
		{
			get
			{
				return ToolbarButtons.openSharedContact;
			}
		}

		public static ToolbarButton ShareThisContact
		{
			get
			{
				return ToolbarButtons.shareThisContact;
			}
		}

		public static ToolbarButton ShareTask
		{
			get
			{
				return ToolbarButtons.shareTask;
			}
		}

		public static ToolbarButton OpenSharedTask
		{
			get
			{
				return ToolbarButtons.openSharedTask;
			}
		}

		public static ToolbarButton ShareThisTask
		{
			get
			{
				return ToolbarButtons.shareThisTask;
			}
		}

		public static ToolbarButton Forward
		{
			get
			{
				return ToolbarButtons.forward;
			}
		}

		public static ToolbarButton ForwardAsAttachment
		{
			get
			{
				return ToolbarButtons.forwardAsAttachment;
			}
		}

		public static ToolbarButton ForwardImageOnly
		{
			get
			{
				return ToolbarButtons.forwardImageOnly;
			}
		}

		public static ToolbarButton ForwardTextOnly
		{
			get
			{
				return ToolbarButtons.forwardTextOnly;
			}
		}

		public static ToolbarButton ForwardCombo
		{
			get
			{
				return ToolbarButtons.forwardCombo;
			}
		}

		public static ToolbarButton ForwardComboImageOnly
		{
			get
			{
				return ToolbarButtons.forwardComboImageOnly;
			}
		}

		public static ToolbarButton ForwardInDropDown
		{
			get
			{
				return ToolbarButtons.forwardInDropDown;
			}
		}

		public static ToolbarButton ForwardAsAttachmentInDropDown
		{
			get
			{
				return ToolbarButtons.forwardAsAttachmentInDropDown;
			}
		}

		public static ToolbarButton ForwardSms
		{
			get
			{
				return ToolbarButtons.forwardSms;
			}
		}

		public static ToolbarButton ImportContactList
		{
			get
			{
				return ToolbarButtons.importContactList;
			}
		}

		public static ToolbarButton ImportanceHigh
		{
			get
			{
				return ToolbarButtons.importanceHigh;
			}
		}

		public static ToolbarButton ImportanceLow
		{
			get
			{
				return ToolbarButtons.importanceLow;
			}
		}

		public static ToolbarButton InsertSignature
		{
			get
			{
				return ToolbarButtons.insertSignature;
			}
		}

		public static ToolbarButton MarkComplete
		{
			get
			{
				return ToolbarButtons.markComplete;
			}
		}

		public static ToolbarButton MarkCompleteNoText
		{
			get
			{
				return ToolbarButtons.markCompleteNoText;
			}
		}

		public static ToolbarButton MeetingAccept
		{
			get
			{
				return ToolbarButtons.meetingAccept;
			}
		}

		public static ToolbarButton MeetingAcceptMenu
		{
			get
			{
				return ToolbarButtons.meetingAcceptMenu;
			}
		}

		public static ToolbarButton MeetingDecline
		{
			get
			{
				return ToolbarButtons.meetingDecline;
			}
		}

		public static ToolbarButton MeetingDeclineMenu
		{
			get
			{
				return ToolbarButtons.meetingDeclineMenu;
			}
		}

		public static ToolbarButton MeetingTentative
		{
			get
			{
				return ToolbarButtons.meetingTentative;
			}
		}

		public static ToolbarButton MeetingTentativeMenu
		{
			get
			{
				return ToolbarButtons.meetingTentativeMenu;
			}
		}

		public static ToolbarButton MeetingEditResponse
		{
			get
			{
				return ToolbarButtons.meetingEditResponse;
			}
		}

		public static ToolbarButton MeetingSendResponseNow
		{
			get
			{
				return ToolbarButtons.meetingSendResponseNow;
			}
		}

		public static ToolbarButton MeetingNoResponse
		{
			get
			{
				return ToolbarButtons.meetingNoResponse;
			}
		}

		public static ToolbarButton MeetingNoResponseRequired
		{
			get
			{
				return ToolbarButtons.meetingNoResponseRequired;
			}
		}

		public static ToolbarButton MeetingOutOfDate
		{
			get
			{
				return ToolbarButtons.meetingOutOfDate;
			}
		}

		public static ToolbarButton ParentFolder
		{
			get
			{
				return ToolbarButtons.parentFolder;
			}
		}

		public static ToolbarButton AddToFavorites
		{
			get
			{
				return ToolbarButtons.addToFavorites;
			}
		}

		public static ToolbarButton ShowCalendar
		{
			get
			{
				return ToolbarButtons.showCalendar;
			}
		}

		public static ToolbarButton MeetingCancelled
		{
			get
			{
				return ToolbarButtons.meetingCancelled;
			}
		}

		public static ToolbarButton ResponseAccepted
		{
			get
			{
				return ToolbarButtons.responseAccepted;
			}
		}

		public static ToolbarButton ResponseTentative
		{
			get
			{
				return ToolbarButtons.responseTentative;
			}
		}

		public static ToolbarButton ResponseDeclined
		{
			get
			{
				return ToolbarButtons.responseDeclined;
			}
		}

		public static ToolbarButton MessageDetails
		{
			get
			{
				return ToolbarButtons.messageDetails;
			}
		}

		public static ToolbarButton MessageOptions
		{
			get
			{
				return ToolbarButtons.messageOptions;
			}
		}

		public static ToolbarButton MailTips
		{
			get
			{
				return ToolbarButtons.mailTips;
			}
		}

		public static ToolbarButton MonthView
		{
			get
			{
				return ToolbarButtons.monthView;
			}
		}

		public static ToolbarButton MultiLine
		{
			get
			{
				return ToolbarButtons.multiLine;
			}
		}

		public static ToolbarButton SingleLine
		{
			get
			{
				return ToolbarButtons.singleLine;
			}
		}

		public static ToolbarButton NewestOnTop
		{
			get
			{
				return ToolbarButtons.newestOnTop;
			}
		}

		public static ToolbarButton OldestOnTop
		{
			get
			{
				return ToolbarButtons.oldestOnTop;
			}
		}

		public static ToolbarButton ExpandAll
		{
			get
			{
				return ToolbarButtons.expandAll;
			}
		}

		public static ToolbarButton CollapseAll
		{
			get
			{
				return ToolbarButtons.collapseAll;
			}
		}

		public static ToolbarButton ShowTree
		{
			get
			{
				return ToolbarButtons.showTree;
			}
		}

		public static ToolbarButton ShowFlatList
		{
			get
			{
				return ToolbarButtons.showFlatList;
			}
		}

		public static ToolbarButton NewMessageCombo
		{
			get
			{
				return ToolbarButtons.newMessageCombo;
			}
		}

		public static ToolbarButton NewAppointmentCombo
		{
			get
			{
				return ToolbarButtons.newAppointmentCombo;
			}
		}

		public static ToolbarButton NewContactCombo
		{
			get
			{
				return ToolbarButtons.newContactCombo;
			}
		}

		public static ToolbarButton NewTaskCombo
		{
			get
			{
				return ToolbarButtons.newTaskCombo;
			}
		}

		public static ToolbarButton NewAppointment
		{
			get
			{
				return ToolbarButtons.newAppointment;
			}
		}

		public static ToolbarButton NewMeetingRequest
		{
			get
			{
				return ToolbarButtons.newMeetingRequest;
			}
		}

		public static ToolbarButton NewContactDistributionList
		{
			get
			{
				return ToolbarButtons.newContactDistributionList;
			}
		}

		public static ToolbarButton NewContact
		{
			get
			{
				return ToolbarButtons.newContact;
			}
		}

		public static ToolbarButton NewFolder
		{
			get
			{
				return ToolbarButtons.newFolder;
			}
		}

		public static ToolbarButton NewMessage
		{
			get
			{
				return ToolbarButtons.newMessage;
			}
		}

		public static ToolbarButton NewMessageToContacts
		{
			get
			{
				return ToolbarButtons.newMessageToContacts;
			}
		}

		public static ToolbarButton NewMeetingRequestToContacts
		{
			get
			{
				return ToolbarButtons.newMeetingRequestToContacts;
			}
		}

		public static ToolbarButton NewMessageToContact
		{
			get
			{
				return ToolbarButtons.newMessageToContact;
			}
		}

		public static ToolbarButton NewMessageToDistributionList
		{
			get
			{
				return ToolbarButtons.newMessageToDistributionList;
			}
		}

		public static ToolbarButton NewMeetingRequestToContact
		{
			get
			{
				return ToolbarButtons.newMeetingRequestToContact;
			}
		}

		public static ToolbarButton NewTask
		{
			get
			{
				return ToolbarButtons.newTask;
			}
		}

		public static ToolbarButton NewWithTaskIcon
		{
			get
			{
				return ToolbarButtons.newWithTaskIcon;
			}
		}

		public static ToolbarButton NewPost
		{
			get
			{
				return ToolbarButtons.newPost;
			}
		}

		public static ToolbarButton NewWithAppointmentIcon
		{
			get
			{
				return ToolbarButtons.newWithAppointmentIcon;
			}
		}

		public static ToolbarButton NewWithPostIcon
		{
			get
			{
				return ToolbarButtons.newWithPostIcon;
			}
		}

		public static ToolbarButton Next
		{
			get
			{
				return ToolbarButtons.next;
			}
		}

		public static ToolbarButton NotJunk
		{
			get
			{
				return ToolbarButtons.notJunk;
			}
		}

		public static ToolbarButton EmptyFolder
		{
			get
			{
				return ToolbarButtons.emptyFolder;
			}
		}

		public static ToolbarButton Post
		{
			get
			{
				return ToolbarButtons.post;
			}
		}

		public static ToolbarButton Previous
		{
			get
			{
				return ToolbarButtons.previous;
			}
		}

		public static ToolbarButton Options
		{
			get
			{
				return ToolbarButtons.options;
			}
		}

		public static ToolbarButton Print
		{
			get
			{
				return ToolbarButtons.print;
			}
		}

		public static ToolbarButton PrintCalendar
		{
			get
			{
				return ToolbarButtons.printCalendar;
			}
		}

		public static ToolbarButton PrintCalendarLabel
		{
			get
			{
				return ToolbarButtons.printCalendarLabel;
			}
		}

		public static ToolbarButton PrintDailyView
		{
			get
			{
				return ToolbarButtons.printDailyView;
			}
		}

		public static ToolbarButton PrintWeeklyView
		{
			get
			{
				return ToolbarButtons.printWeeklyView;
			}
		}

		public static ToolbarButton PrintMonthlyView
		{
			get
			{
				return ToolbarButtons.printMonthlyView;
			}
		}

		public static ToolbarButton ChangeView
		{
			get
			{
				if (UserContextManager.GetUserContext().IsRtl)
				{
					return ToolbarButtons.changeViewRTL;
				}
				return ToolbarButtons.changeView;
			}
		}

		public static ToolbarButton UseConversations
		{
			get
			{
				return ToolbarButtons.useConversations;
			}
		}

		public static ToolbarButton ConversationOptions
		{
			get
			{
				return ToolbarButtons.conversationOptions;
			}
		}

		public static ToolbarButton ReadingPaneBottom
		{
			get
			{
				return ToolbarButtons.readingPaneBottom;
			}
		}

		public static ToolbarButton ReadingPaneRight
		{
			get
			{
				if (UserContextManager.GetUserContext().IsRtl)
				{
					return ToolbarButtons.readingPaneRightRTL;
				}
				return ToolbarButtons.readingPaneRight;
			}
		}

		public static ToolbarButton ReadingPaneOff
		{
			get
			{
				return ToolbarButtons.readingPaneOff;
			}
		}

		public static ToolbarButton ReadingPaneOffSwap
		{
			get
			{
				if (UserContextManager.GetUserContext().IsRtl)
				{
					return ToolbarButtons.readingPaneOffSwapRTL;
				}
				return ToolbarButtons.readingPaneOffSwap;
			}
		}

		public static ToolbarButton ReadingPaneRightSwap
		{
			get
			{
				if (UserContextManager.GetUserContext().IsRtl)
				{
					return ToolbarButtons.readingPaneRightSwapRTL;
				}
				return ToolbarButtons.readingPaneRightSwap;
			}
		}

		public static ToolbarButton Reply
		{
			get
			{
				return ToolbarButtons.reply;
			}
		}

		public static ToolbarButton ReplyImageOnly
		{
			get
			{
				return ToolbarButtons.replyImageOnly;
			}
		}

		public static ToolbarButton ReplyTextOnly
		{
			get
			{
				return ToolbarButtons.replyTextOnly;
			}
		}

		public static ToolbarButton ReplyCombo
		{
			get
			{
				return ToolbarButtons.replyCombo;
			}
		}

		public static ToolbarButton ReplyComboImageOnly
		{
			get
			{
				return ToolbarButtons.replyComboImageOnly;
			}
		}

		public static ToolbarButton ReplyInDropDown
		{
			get
			{
				return ToolbarButtons.replyInDropDown;
			}
		}

		public static ToolbarButton ReplyByChat
		{
			get
			{
				return ToolbarButtons.replyByChat;
			}
		}

		public static ToolbarButton ReplyByPhone
		{
			get
			{
				return ToolbarButtons.replyByPhone;
			}
		}

		public static ToolbarButton ReplyBySms
		{
			get
			{
				return ToolbarButtons.replyBySms;
			}
		}

		public static ToolbarButton ReplyAll
		{
			get
			{
				return ToolbarButtons.replyAll;
			}
		}

		public static ToolbarButton ReplyAllImageOnly
		{
			get
			{
				return ToolbarButtons.replyAllImageOnly;
			}
		}

		public static ToolbarButton ReplyAllTextOnly
		{
			get
			{
				return ToolbarButtons.replyAllTextOnly;
			}
		}

		public static ToolbarButton ReplyAllSms
		{
			get
			{
				return ToolbarButtons.replyAllSms;
			}
		}

		public static ToolbarButton ReplySms
		{
			get
			{
				return ToolbarButtons.replySms;
			}
		}

		public static ToolbarButton PostReply
		{
			get
			{
				return ToolbarButtons.postReply;
			}
		}

		public static ToolbarButton Reminders
		{
			get
			{
				return ToolbarButtons.reminders;
			}
		}

		public static ToolbarButton Save
		{
			get
			{
				return ToolbarButtons.save;
			}
		}

		public static ToolbarButton SaveAndClose
		{
			get
			{
				return ToolbarButtons.saveAndClose;
			}
		}

		public static ToolbarButton SaveAndCloseImageOnly
		{
			get
			{
				return ToolbarButtons.saveAndCloseImageOnly;
			}
		}

		public static ToolbarButton SaveImageOnly
		{
			get
			{
				return ToolbarButtons.saveImageOnly;
			}
		}

		public static ToolbarButton Send
		{
			get
			{
				return ToolbarButtons.send;
			}
		}

		public static ToolbarButton SendAgain
		{
			get
			{
				return ToolbarButtons.sendAgain;
			}
		}

		public static ToolbarButton SendCancelation
		{
			get
			{
				return ToolbarButtons.sendCancelation;
			}
		}

		public static ToolbarButton CancelMeeting
		{
			get
			{
				return ToolbarButtons.cancelMeeting;
			}
		}

		public static ToolbarButton SendUpdate
		{
			get
			{
				return ToolbarButtons.sendUpdate;
			}
		}

		public static ToolbarButton InviteAttendees
		{
			get
			{
				return ToolbarButtons.inviteAttendees;
			}
		}

		public static ToolbarButton CancelInvitation
		{
			get
			{
				return ToolbarButtons.cancelInvitation;
			}
		}

		public static ToolbarButton SpellCheck
		{
			get
			{
				return ToolbarButtons.spellCheck;
			}
		}

		public static ToolbarButton Today
		{
			get
			{
				return ToolbarButtons.today;
			}
		}

		public static ToolbarButton TimeZoneDropDown
		{
			get
			{
				return ToolbarButtons.timeZoneDropDown;
			}
		}

		public static ToolbarButton WeekView
		{
			get
			{
				return ToolbarButtons.weekView;
			}
		}

		public static ToolbarButton WorkWeekView
		{
			get
			{
				return ToolbarButtons.workWeekView;
			}
		}

		public static ToolbarButton Recover
		{
			get
			{
				return ToolbarButtons.recover;
			}
		}

		public static ToolbarButton Purge
		{
			get
			{
				return ToolbarButtons.purge;
			}
		}

		public static ToolbarButton Recurrence
		{
			get
			{
				return ToolbarButtons.recurrence;
			}
		}

		public static ToolbarButton RecurrenceImageOnly
		{
			get
			{
				return ToolbarButtons.recurrenceImageOnly;
			}
		}

		public static ToolbarButton CreateRule
		{
			get
			{
				return ToolbarButtons.createRule;
			}
		}

		public static ToolbarButton MessageEncryptContents
		{
			get
			{
				return ToolbarButtons.messageEncryptContents;
			}
		}

		public static ToolbarButton MessageDigitalSignature
		{
			get
			{
				return ToolbarButtons.messageDigitalSignature;
			}
		}

		public static ToolbarButton Move
		{
			get
			{
				return ToolbarButtons.move;
			}
		}

		public static ToolbarButton MoveWithText
		{
			get
			{
				return ToolbarButtons.moveWithText;
			}
		}

		public static ToolbarButton MoveTextOnly
		{
			get
			{
				return ToolbarButtons.moveTextOnly;
			}
		}

		public static ToolbarButton EditTextOnly
		{
			get
			{
				return ToolbarButtons.editTextOnly;
			}
		}

		public static ToolbarButton NewPersonalAutoAttendant
		{
			get
			{
				return ToolbarButtons.newPersonalAutoAttendant;
			}
		}

		public static ToolbarButton MoveUp
		{
			get
			{
				return ToolbarButtons.moveUp;
			}
		}

		public static ToolbarButton MoveDown
		{
			get
			{
				return ToolbarButtons.moveDown;
			}
		}

		public static ToolbarButton Chat
		{
			get
			{
				return ToolbarButtons.chat;
			}
		}

		public static ToolbarButton AddToBuddyList
		{
			get
			{
				return ToolbarButtons.addToBuddyList;
			}
		}

		public static ToolbarButton AddToBuddyListWithText
		{
			get
			{
				return ToolbarButtons.addToBuddyListWithText;
			}
		}

		public static ToolbarButton RemoveFromBuddyList
		{
			get
			{
				return ToolbarButtons.removeFromBuddyList;
			}
		}

		public static ToolbarButton RemoveFromBuddyListWithText
		{
			get
			{
				return ToolbarButtons.removeFromBuddyListWithText;
			}
		}

		public static ToolbarButton SendATextMessage
		{
			get
			{
				return ToolbarButtons.sendATextMessage;
			}
		}

		public static ToolbarButton NewSms
		{
			get
			{
				return ToolbarButtons.newSms;
			}
		}

		public static ToolbarButton SendSms
		{
			get
			{
				return ToolbarButtons.sendSms;
			}
		}

		public static ToolbarButton InviteContact
		{
			get
			{
				return ToolbarButtons.inviteContact;
			}
		}

		public static ToolbarButton FilterCombo
		{
			get
			{
				return ToolbarButtons.filterCombo;
			}
		}

		public static ToolbarButton AddThisCalendar
		{
			get
			{
				return ToolbarButtons.addThisCalendar;
			}
		}

		public static ToolbarButton SharingMyCalendar
		{
			get
			{
				return ToolbarButtons.sharingMyCalendar;
			}
		}

		public static ToolbarButton SharingDeclineMenu
		{
			get
			{
				return ToolbarButtons.sharingDeclineMenu;
			}
		}

		public static ToolbarButton Subscribe
		{
			get
			{
				return ToolbarButtons.subscribe;
			}
		}

		public static ToolbarButton SubscribeToThisCalendar
		{
			get
			{
				return ToolbarButtons.subscribeToThisCalendar;
			}
		}

		public static ToolbarButton ViewThisCalendar
		{
			get
			{
				return ToolbarButtons.viewThisCalendar;
			}
		}

		public static ToolbarButton MessageNoteInDropDown
		{
			get
			{
				return ToolbarButtons.messageNoteInDropDown;
			}
		}

		public static ToolbarButton MessageNoteInToolbar
		{
			get
			{
				return ToolbarButtons.messageNoteInToolbar;
			}
		}

		private static bool Initialize()
		{
			ToolbarButtons.importanceHigh.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.importanceLow
			});
			ToolbarButtons.importanceLow.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.importanceHigh
			});
			ToolbarButtons.multiLine.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.singleLine
			});
			ToolbarButtons.singleLine.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.multiLine
			});
			ToolbarButtons.readingPaneRightSwap.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.readingPaneOffSwap
			});
			ToolbarButtons.readingPaneOffSwap.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.readingPaneRightSwap
			});
			ToolbarButtons.readingPaneRightSwapRTL.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.readingPaneOffSwapRTL
			});
			ToolbarButtons.readingPaneOffSwapRTL.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.readingPaneRightSwapRTL
			});
			ToolbarButtons.cancelInvitation.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.inviteAttendees,
				ToolbarButtons.save,
				ToolbarButtons.send,
				ToolbarButtons.saveAndClose,
				ToolbarButtons.checkNames
			});
			ToolbarButtons.inviteAttendees.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.cancelInvitation,
				ToolbarButtons.save,
				ToolbarButtons.send,
				ToolbarButtons.saveAndClose,
				ToolbarButtons.checkNames
			});
			ToolbarButtons.newestOnTop.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.oldestOnTop
			});
			ToolbarButtons.oldestOnTop.SetSwapButtons(new ToolbarButton[]
			{
				ToolbarButtons.newestOnTop
			});
			ToolbarButtons.dayView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.weekView,
				ToolbarButtons.workWeekView,
				ToolbarButtons.monthView
			});
			ToolbarButtons.weekView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.dayView,
				ToolbarButtons.workWeekView,
				ToolbarButtons.monthView
			});
			ToolbarButtons.workWeekView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.dayView,
				ToolbarButtons.weekView,
				ToolbarButtons.monthView
			});
			ToolbarButtons.monthView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.dayView,
				ToolbarButtons.weekView,
				ToolbarButtons.workWeekView
			});
			ToolbarButtons.printDailyView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.printWeeklyView,
				ToolbarButtons.printMonthlyView
			});
			ToolbarButtons.printWeeklyView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.printDailyView,
				ToolbarButtons.printMonthlyView
			});
			ToolbarButtons.printMonthlyView.SetToggleButtons(new ToolbarButton[]
			{
				ToolbarButtons.printDailyView,
				ToolbarButtons.printWeeklyView
			});
			return true;
		}

		private static readonly ToolbarButton actions = new ToolbarButton("actions", ToolbarButtonFlags.Text | ToolbarButtonFlags.CustomMenu, -859543544, ThemeFileId.None);

		private static readonly ToolbarButton addressBook = new ToolbarButton("addressbook", ToolbarButtonFlags.Image, 1139489555, ThemeFileId.AddressBook);

		private static readonly ToolbarButton addToContacts = new ToolbarButton("addct", ToolbarButtonFlags.Image, 1775424225, ThemeFileId.AddToContacts);

		private static readonly ToolbarButton approvalApprove = new ToolbarButton("approve", ToolbarButtonFlags.ImageAndText, -236685197, ThemeFileId.Approve);

		private static readonly ToolbarButton approvalApproveMenu = new ToolbarButton("approve", ToolbarButtonFlags.Text | ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu, -236685197, ThemeFileId.Approve);

		private static readonly ToolbarButton approvalReject = new ToolbarButton("reject", ToolbarButtonFlags.ImageAndText, -2059328365, ThemeFileId.Reject);

		private static readonly ToolbarButton approvalRejectMenu = new ToolbarButton("reject", ToolbarButtonFlags.Text | ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu, -2059328365, ThemeFileId.Reject);

		private static readonly ToolbarButton approvalEditResponse = new ToolbarButton("apvedrsp", 1050381195);

		private static readonly ToolbarButton approvalSendResponseNow = new ToolbarButton("apvsndrsp", -114654491);

		private static readonly ToolbarButton attachFile = new ToolbarButton("attachfile", ToolbarButtonFlags.Image, -1532412163, ThemeFileId.Attachment1);

		private static readonly ToolbarButton insertImage = new ToolbarButton("insertimage", ToolbarButtonFlags.Image, 7329360, ThemeFileId.InsertImage);

		private static readonly ToolbarButton calendarTitle = new ToolbarButton("noAction", ToolbarButtonFlags.Text | ToolbarButtonFlags.NoAction, -1018465893, ThemeFileId.None);

		private static readonly ToolbarButton checkMessages = new ToolbarButton("checkmessages", ToolbarButtonFlags.Image, 1476440846, ThemeFileId.CheckMessages);

		private static readonly ToolbarButton checkNames = new ToolbarButton("checknames", ToolbarButtonFlags.Image, -1374765726, ThemeFileId.CheckNames);

		private static readonly ToolbarButton compliance = new ToolbarButton("compliance", ToolbarButtonFlags.Image | ToolbarButtonFlags.CustomMenu, -1246480803, ThemeFileId.ComplianceDropDown);

		private static readonly ToolbarButton edit = new ToolbarButton("edt", ToolbarButtonFlags.Text, 2119799890, ThemeFileId.None);

		private static readonly ToolbarButton dayView = new ToolbarButton("day", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -34880007, ThemeFileId.DayView, -509047980);

		private static readonly ToolbarButton delete = new ToolbarButton("delete", ToolbarButtonFlags.Image, 1381996313, ThemeFileId.Delete);

		private static readonly ToolbarButton deleteTextOnly = new ToolbarButton("delete", ToolbarButtonFlags.Text, 1381996313, ThemeFileId.None);

		private static readonly ToolbarButton deleteCombo = new ToolbarButton("delete", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, 1381996313, ThemeFileId.None);

		private static readonly ToolbarButton deleteInDropDown = new ToolbarButton("deletedrpdwn", ToolbarButtonFlags.ImageAndText, 1381996313, ThemeFileId.Delete);

		private static readonly ToolbarButton deleteWithText = new ToolbarButton("delete", ToolbarButtonFlags.Text, 1381996313, ThemeFileId.None);

		private static readonly ToolbarButton ignoreConversation = new ToolbarButton("ignoreconversation", ToolbarButtonFlags.ImageAndText, 1486263145, ThemeFileId.IgnoreConversation);

		private static readonly ToolbarButton cancelIgnoreConversationCombo = new ToolbarButton("cancelignoreconversation", ToolbarButtonFlags.Image | ToolbarButtonFlags.ComboMenu | ToolbarButtonFlags.AlwaysPressed, -476691185, ThemeFileId.IgnoreConversation);

		private static readonly ToolbarButton cancelIgnoreConversationInDropDown = new ToolbarButton("cancelignoreconversationdd", ToolbarButtonFlags.ImageAndText, -476691185, ThemeFileId.IgnoreConversation);

		private static readonly ToolbarButton deleteInCancelIgnoreConversationDropDown = new ToolbarButton("deleteigndrpdwn", ToolbarButtonFlags.ImageAndText, 1381996313, ThemeFileId.Delete);

		private static readonly ToolbarButton flag = new ToolbarButton("flag", ToolbarButtonFlags.Image | ToolbarButtonFlags.CustomMenu, -1950847676, ThemeFileId.Flag);

		private static readonly ToolbarButton categories = new ToolbarButton("cat", ToolbarButtonFlags.Image | ToolbarButtonFlags.CustomMenu, -1941714382, ThemeFileId.Categories);

		private static readonly ToolbarButton searchInPublicFolder = new ToolbarButton("searchpf", ToolbarButtonFlags.Text | ToolbarButtonFlags.Image | ToolbarButtonFlags.ImageAfterText, 656259478, ThemeFileId.Expand);

		private static readonly ToolbarButton changeSharingPermissions = new ToolbarButton("chgperm", ToolbarButtonFlags.Text | ToolbarButtonFlags.Menu, -82275026, ThemeFileId.None);

		private static readonly ToolbarButton shareCalendar = new ToolbarButton("sharecal", ToolbarButtonFlags.Text | ToolbarButtonFlags.CustomMenu, 869186573, ThemeFileId.None);

		private static readonly ToolbarButton openSharedCalendar = new ToolbarButton("opnshcal", ToolbarButtonFlags.ImageAndText, 1936872779, ThemeFileId.CalendarSharedTo);

		private static readonly ToolbarButton shareACalendar = new ToolbarButton("shcurcal", ToolbarButtonFlags.Text | ToolbarButtonFlags.Menu, 427125723, ThemeFileId.None);

		private static readonly ToolbarButton requestSharedCalendar = new ToolbarButton("requestcalendar", ToolbarButtonFlags.Text, -625603472, ThemeFileId.None);

		private static readonly ToolbarButton shareContact = new ToolbarButton("sharecnt", ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu, 869186573, ThemeFileId.ContactSharedOut);

		private static readonly ToolbarButton openSharedContact = new ToolbarButton("opnshcnt", ToolbarButtonFlags.ImageAndText, 2042364774, ThemeFileId.ContactSharedTo);

		private static readonly ToolbarButton shareThisContact = new ToolbarButton("shcurcnt", ToolbarButtonFlags.ImageAndText, -1103633587, ThemeFileId.ContactSharedOut);

		private static readonly ToolbarButton shareTask = new ToolbarButton("sharetsk", ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu, 869186573, ThemeFileId.TaskSharedOut);

		private static readonly ToolbarButton openSharedTask = new ToolbarButton("opnshtsk", ToolbarButtonFlags.ImageAndText, -1870011529, ThemeFileId.TaskSharedTo);

		private static readonly ToolbarButton shareThisTask = new ToolbarButton("shcurtsk", ToolbarButtonFlags.ImageAndText, 1583085584, ThemeFileId.TaskSharedOut);

		private static readonly ToolbarButton forward = new ToolbarButton("forward", ToolbarButtonFlags.Text, -1428116961, ThemeFileId.None);

		private static readonly ToolbarButton forwardAsAttachment = new ToolbarButton("fwia", ToolbarButtonFlags.Text, -1428116961, ThemeFileId.None);

		private static readonly ToolbarButton forwardImageOnly = new ToolbarButton("forward", ToolbarButtonFlags.Image, -1428116961, ThemeFileId.Forward);

		private static readonly ToolbarButton forwardTextOnly = new ToolbarButton("forward", ToolbarButtonFlags.Text, -1428116961, ThemeFileId.None);

		private static readonly ToolbarButton forwardCombo = new ToolbarButton("forwardcombo", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -1428116961, ThemeFileId.None);

		private static readonly ToolbarButton forwardComboImageOnly = new ToolbarButton("forwardcombo", ToolbarButtonFlags.Image | ToolbarButtonFlags.ComboMenu, -1428116961, ThemeFileId.Forward);

		private static readonly ToolbarButton forwardInDropDown = new ToolbarButton("forwarddrpdwn", ToolbarButtonFlags.ImageAndText, -1428116961, ThemeFileId.Forward);

		private static readonly ToolbarButton forwardAsAttachmentInDropDown = new ToolbarButton("fwiadrpdwn", ToolbarButtonFlags.ImageAndText, 438661106, ThemeFileId.ForwardAsAttachment);

		private static readonly ToolbarButton forwardSms = new ToolbarButton("forward", ToolbarButtonFlags.Text, -1428116961, ThemeFileId.None);

		private static readonly ToolbarButton importContactList = new ToolbarButton("impcontactlist", ToolbarButtonFlags.Text, 1660557420, ThemeFileId.None);

		private static readonly ToolbarButton importanceHigh = new ToolbarButton("imphigh", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky, 1535769152, ThemeFileId.ImportanceHigh2);

		private static readonly ToolbarButton importanceLow = new ToolbarButton("implow", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky, -1341425078, ThemeFileId.ImportanceLow2);

		private static readonly ToolbarButton insertSignature = new ToolbarButton("insertsignature", ToolbarButtonFlags.Image, -1909117233, ThemeFileId.Signature);

		private static readonly ToolbarButton markComplete = new ToolbarButton("markcomplete", ToolbarButtonFlags.Text, -32068740, ThemeFileId.None);

		private static readonly ToolbarButton markCompleteNoText = new ToolbarButton("markcomplete", ToolbarButtonFlags.Image, -32068740, ThemeFileId.MarkComplete);

		private static readonly ToolbarButton meetingAccept = new ToolbarButton("accept", ToolbarButtonFlags.Image | ToolbarButtonFlags.BigSize, -475579318, ThemeFileId.MeetingAcceptBig);

		private static readonly ToolbarButton meetingAcceptMenu = new ToolbarButton("accept", ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu | ToolbarButtonFlags.BigSize, -475579318, ThemeFileId.MeetingAcceptBig);

		private static readonly ToolbarButton meetingDecline = new ToolbarButton("decline", ToolbarButtonFlags.Image | ToolbarButtonFlags.BigSize, -2119870632, ThemeFileId.MeetingDeclineBig);

		private static readonly ToolbarButton meetingDeclineMenu = new ToolbarButton("decline", ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu | ToolbarButtonFlags.BigSize, -2119870632, ThemeFileId.MeetingDeclineBig);

		private static readonly ToolbarButton meetingTentative = new ToolbarButton("tentative", ToolbarButtonFlags.Image | ToolbarButtonFlags.BigSize, 1797669216, ThemeFileId.MeetingTentativeBig);

		private static readonly ToolbarButton meetingTentativeMenu = new ToolbarButton("tentative", ToolbarButtonFlags.Image | ToolbarButtonFlags.Menu | ToolbarButtonFlags.BigSize, 1797669216, ThemeFileId.MeetingTentativeBig);

		private static readonly ToolbarButton meetingEditResponse = new ToolbarButton("edrsp", 1050381195);

		private static readonly ToolbarButton meetingSendResponseNow = new ToolbarButton("sndrsp", -114654491);

		private static readonly ToolbarButton meetingNoResponse = new ToolbarButton("norsp", -990767046);

		private static readonly ToolbarButton meetingNoResponseRequired = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction | ToolbarButtonFlags.BigSize, -398794157, ThemeFileId.MeetingInfoBig);

		private static readonly ToolbarButton meetingOutOfDate = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction | ToolbarButtonFlags.BigSize, -1694210393, ThemeFileId.MeetingInfoBig);

		private static readonly ToolbarButton parentFolder = new ToolbarButton("up", ToolbarButtonFlags.Text, 1543969273, ThemeFileId.None);

		private static readonly ToolbarButton addToFavorites = new ToolbarButton("addFvTb", ToolbarButtonFlags.Text, -1028120515, ThemeFileId.None);

		private static readonly ToolbarButton showCalendar = new ToolbarButton("showcalendar", ToolbarButtonFlags.Image | ToolbarButtonFlags.BigSize, -373408913, ThemeFileId.MeetingOpenBig);

		private static readonly ToolbarButton meetingCancelled = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction, -1018465893, ThemeFileId.MeetingResponseDecline);

		private static readonly ToolbarButton responseAccepted = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction, -1018465893, ThemeFileId.MeetingResponseAccept);

		private static readonly ToolbarButton responseTentative = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction, -1018465893, ThemeFileId.MeetingResponseTentative);

		private static readonly ToolbarButton responseDeclined = new ToolbarButton("noAction", ToolbarButtonFlags.Image | ToolbarButtonFlags.NoAction, -1018465893, ThemeFileId.MeetingResponseDecline);

		private static readonly ToolbarButton messageDetails = new ToolbarButton("messagedetails", ToolbarButtonFlags.Image, 1624231629, ThemeFileId.MessageDetails);

		private static readonly ToolbarButton messageOptions = new ToolbarButton("messageoptions", ToolbarButtonFlags.Text, -1086592719, ThemeFileId.None);

		private static readonly ToolbarButton mailTips = new ToolbarButton("mailTips", ToolbarButtonFlags.Image | ToolbarButtonFlags.Hidden | ToolbarButtonFlags.CustomMenu, ThemeFileId.Informational);

		private static readonly ToolbarButton monthView = new ToolbarButton("month", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -1648015055, ThemeFileId.MonthView, 1011436404);

		private static readonly ToolbarButton multiLine = new ToolbarButton("ml", ToolbarButtonFlags.Text, 573748959, ThemeFileId.None);

		private static readonly ToolbarButton singleLine = new ToolbarButton("sl", ToolbarButtonFlags.Text, -2094330208, ThemeFileId.None);

		private static readonly ToolbarButton newestOnTop = new ToolbarButton("newestOnTop", ToolbarButtonFlags.Text, 1746211700, ThemeFileId.None);

		private static readonly ToolbarButton oldestOnTop = new ToolbarButton("oldestOnTop", ToolbarButtonFlags.Text, 2070168051, ThemeFileId.None);

		private static readonly ToolbarButton expandAll = new ToolbarButton("expandAll", ToolbarButtonFlags.Text, 18372887, ThemeFileId.None);

		private static readonly ToolbarButton collapseAll = new ToolbarButton("collapseAll", ToolbarButtonFlags.Text, -1678460464, ThemeFileId.None);

		private static readonly ToolbarButton showTree = new ToolbarButton("showTree", ToolbarButtonFlags.Text, 1039762825, ThemeFileId.None);

		private static readonly ToolbarButton showFlatList = new ToolbarButton("showFlatList", ToolbarButtonFlags.Text, 1376461660, ThemeFileId.None);

		private static readonly ToolbarButton newMessageCombo = new ToolbarButton("newmsgc", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newAppointmentCombo = new ToolbarButton("newapptc", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newContactCombo = new ToolbarButton("newcc", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newTaskCombo = new ToolbarButton("newtc", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newAppointment = new ToolbarButton("newappt", ToolbarButtonFlags.ImageAndText, -1797628885, ThemeFileId.Appointment);

		private static readonly ToolbarButton newMeetingRequest = new ToolbarButton("newmtng", ToolbarButtonFlags.ImageAndText, -1560657880, ThemeFileId.MeetingRequest);

		private static readonly ToolbarButton newContactDistributionList = new ToolbarButton("newcdl", ToolbarButtonFlags.ImageAndText, -1878983012, ThemeFileId.ContactDL);

		private static readonly ToolbarButton newContact = new ToolbarButton("newc", ToolbarButtonFlags.ImageAndText, 447307630, ThemeFileId.Contact);

		private static readonly ToolbarButton newFolder = new ToolbarButton("newfolder", ToolbarButtonFlags.ImageAndText, -1690271306, ThemeFileId.Folder2);

		private static readonly ToolbarButton newMessage = new ToolbarButton("newmsg", ToolbarButtonFlags.ImageAndText, 360502915, ThemeFileId.EMail3);

		private static readonly ToolbarButton newMessageToContacts = new ToolbarButton("nmsgct", ToolbarButtonFlags.Image, -747517193, ThemeFileId.EMailContact);

		private static readonly ToolbarButton newMeetingRequestToContacts = new ToolbarButton("nmrgct", ToolbarButtonFlags.Image, -1596894910, ThemeFileId.Appointment);

		private static readonly ToolbarButton newMessageToContact = new ToolbarButton("nmsgct", ToolbarButtonFlags.Image, -811703550, ThemeFileId.EMailContact);

		private static readonly ToolbarButton newMessageToDistributionList = new ToolbarButton("nmsgct", ToolbarButtonFlags.Image, -1496031392, ThemeFileId.EMailContact);

		private static readonly ToolbarButton newMeetingRequestToContact = new ToolbarButton("nmrgct", ToolbarButtonFlags.Image, -1803617069, ThemeFileId.Appointment);

		private static readonly ToolbarButton newTask = new ToolbarButton("newtsk", ToolbarButtonFlags.ImageAndText, -1516408339, ThemeFileId.Task);

		private static readonly ToolbarButton newWithTaskIcon = new ToolbarButton("newtsk", ToolbarButtonFlags.Text, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newPost = new ToolbarButton("newpst", ToolbarButtonFlags.Text, -735376682, ThemeFileId.None);

		private static readonly ToolbarButton newWithAppointmentIcon = new ToolbarButton("newappt", ToolbarButtonFlags.Text, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton newWithPostIcon = new ToolbarButton("newpst", ToolbarButtonFlags.Text, -1273337860, ThemeFileId.None);

		private static readonly ToolbarButton next = new ToolbarButton("next", ToolbarButtonFlags.Image, -1846382016, ThemeFileId.Next);

		private static readonly ToolbarButton notJunk = new ToolbarButton("ntjnk", ToolbarButtonFlags.Text, 856598503, ThemeFileId.None);

		private static readonly ToolbarButton emptyFolder = new ToolbarButton("emptyfolder", ToolbarButtonFlags.Text, 445671445, ThemeFileId.None);

		private static readonly ToolbarButton post = new ToolbarButton("post", ToolbarButtonFlags.Text, -864298084, ThemeFileId.None);

		private static readonly ToolbarButton previous = new ToolbarButton("previous", ToolbarButtonFlags.Image, -577308044, ThemeFileId.Previous);

		private static readonly ToolbarButton options = new ToolbarButton("options", ToolbarButtonFlags.Text, -1086592719, ThemeFileId.None);

		private static readonly ToolbarButton print = new ToolbarButton("print", ToolbarButtonFlags.Image, 1588554917, ThemeFileId.Print);

		private static readonly ToolbarButton printCalendar = new ToolbarButton("printcalendarview", ToolbarButtonFlags.Image, 2066252695, ThemeFileId.Print);

		private static readonly ToolbarButton printCalendarLabel = new ToolbarButton("noaction", ToolbarButtonFlags.Text | ToolbarButtonFlags.NoAction, -2048833216, ThemeFileId.None);

		private static readonly ToolbarButton printDailyView = new ToolbarButton("day", ToolbarButtonFlags.Text | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -34880007, ThemeFileId.None, -509047980);

		private static readonly ToolbarButton printWeeklyView = new ToolbarButton("week", ToolbarButtonFlags.Text | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -867675667, ThemeFileId.None, -382962026);

		private static readonly ToolbarButton printMonthlyView = new ToolbarButton("month", ToolbarButtonFlags.Text | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -1648015055, ThemeFileId.None, 1011436404);

		private static readonly ToolbarButton changeView = new ToolbarButton("rps", ToolbarButtonFlags.Text | ToolbarButtonFlags.Menu, 1582260093, ThemeFileId.None, 1898535179);

		private static readonly ToolbarButton changeViewRTL = new ToolbarButton("rps", ToolbarButtonFlags.Text | ToolbarButtonFlags.Menu, 1582260093, ThemeFileId.None, 1898535179);

		private static readonly ToolbarButton useConversations = new ToolbarButton("useConversations", ToolbarButtonFlags.Text, -1107404463, ThemeFileId.None);

		private static readonly ToolbarButton conversationOptions = new ToolbarButton("conversationOptions", ToolbarButtonFlags.Text, 1931186673, ThemeFileId.None);

		private static readonly ToolbarButton readingPaneBottom = new ToolbarButton("rpb", ToolbarButtonFlags.Text, 165760971, ThemeFileId.None);

		private static readonly ToolbarButton readingPaneRight = new ToolbarButton("rpr", ToolbarButtonFlags.Text, 2064563686, ThemeFileId.None);

		private static readonly ToolbarButton readingPaneRightRTL = new ToolbarButton("rpr", ToolbarButtonFlags.Text, 771350883, ThemeFileId.None);

		private static readonly ToolbarButton readingPaneOff = new ToolbarButton("rpo", ToolbarButtonFlags.Text, 369254891, ThemeFileId.None);

		private static readonly ToolbarButton readingPaneOffSwap = new ToolbarButton("rpo", ToolbarButtonFlags.Image, -1439936760, ThemeFileId.ReadingPaneOff);

		private static readonly ToolbarButton readingPaneOffSwapRTL = new ToolbarButton("rpo", ToolbarButtonFlags.Image, -1439936760, ThemeFileId.ReadingPaneOffRTL);

		private static readonly ToolbarButton readingPaneRightSwap = new ToolbarButton("rpr", ToolbarButtonFlags.Image, 576227563, ThemeFileId.ReadingPaneRight);

		private static readonly ToolbarButton readingPaneRightSwapRTL = new ToolbarButton("rpr", ToolbarButtonFlags.Image, 576227563, ThemeFileId.ReadingPaneRightRTL);

		private static readonly ToolbarButton reply = new ToolbarButton("reply", ToolbarButtonFlags.Text, -327372070, ThemeFileId.None);

		private static readonly ToolbarButton replyImageOnly = new ToolbarButton("reply", ToolbarButtonFlags.Image, -327372070, ThemeFileId.Reply);

		private static readonly ToolbarButton replyTextOnly = new ToolbarButton("reply", ToolbarButtonFlags.Text, -327372070, ThemeFileId.None);

		private static readonly ToolbarButton replyCombo = new ToolbarButton("replycombo", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu, -327372070, ThemeFileId.None);

		private static readonly ToolbarButton replyComboImageOnly = new ToolbarButton("replycombo", ToolbarButtonFlags.Image | ToolbarButtonFlags.ComboMenu, -327372070, ThemeFileId.Reply);

		private static readonly ToolbarButton replyInDropDown = new ToolbarButton("replydrpdwn", ToolbarButtonFlags.ImageAndText, -327372070, ThemeFileId.Reply);

		private static readonly ToolbarButton replyByChat = new ToolbarButton("replybychat", ToolbarButtonFlags.Text, 412026487, ThemeFileId.None);

		private static readonly ToolbarButton replyByPhone = new ToolbarButton("replybyphone", ToolbarButtonFlags.Text, -1314097243, ThemeFileId.None);

		private static readonly ToolbarButton replyBySms = new ToolbarButton("replybysms", ToolbarButtonFlags.Text, -838431440, ThemeFileId.None);

		private static readonly ToolbarButton replyAll = new ToolbarButton("replyall", ToolbarButtonFlags.Text, 826363927, ThemeFileId.None);

		private static readonly ToolbarButton replyAllImageOnly = new ToolbarButton("replyall", ToolbarButtonFlags.Image, 826363927, ThemeFileId.ReplyAll);

		private static readonly ToolbarButton replyAllTextOnly = new ToolbarButton("replyall", ToolbarButtonFlags.Text, 826363927, ThemeFileId.None);

		private static readonly ToolbarButton replyAllSms = new ToolbarButton("replyall", ToolbarButtonFlags.Text, 826363927, ThemeFileId.None);

		private static readonly ToolbarButton replySms = new ToolbarButton("reply", ToolbarButtonFlags.Text, -327372070, ThemeFileId.None);

		private static readonly ToolbarButton postReply = new ToolbarButton("postreply", ToolbarButtonFlags.Text, -1780771632, ThemeFileId.None);

		private static readonly ToolbarButton reminders = new ToolbarButton("reminder", ToolbarButtonFlags.Image, -1223560515, ThemeFileId.ReminderSmall);

		private static readonly ToolbarButton save = new ToolbarButton("save", ToolbarButtonFlags.Text, -1966746939, ThemeFileId.None);

		private static readonly ToolbarButton saveAndClose = new ToolbarButton("saveclose", ToolbarButtonFlags.Text, -224317800, ThemeFileId.None);

		private static readonly ToolbarButton saveAndCloseImageOnly = new ToolbarButton("saveclose", ToolbarButtonFlags.Image, -224317800, ThemeFileId.Save);

		private static readonly ToolbarButton saveImageOnly = new ToolbarButton("save", ToolbarButtonFlags.Image, -1966746939, ThemeFileId.Save);

		private static readonly ToolbarButton send = new ToolbarButton("send", ToolbarButtonFlags.Text, -158743924, ThemeFileId.None);

		private static readonly ToolbarButton sendAgain = new ToolbarButton("sendAgain", ToolbarButtonFlags.Text, -1902695064, ThemeFileId.None);

		private static readonly ToolbarButton sendCancelation = new ToolbarButton("sendC", ToolbarButtonFlags.Text, -158743924, ThemeFileId.None);

		private static readonly ToolbarButton cancelMeeting = new ToolbarButton("delete", ToolbarButtonFlags.Image, -240976491, ThemeFileId.CancelInvitation);

		private static readonly ToolbarButton sendUpdate = new ToolbarButton("send", ToolbarButtonFlags.Text, 1302559757, ThemeFileId.None);

		private static readonly ToolbarButton inviteAttendees = new ToolbarButton("invatnd", ToolbarButtonFlags.Image, -775577546, ThemeFileId.MeetingRequest);

		private static readonly ToolbarButton cancelInvitation = new ToolbarButton("caninv", ToolbarButtonFlags.Image, -1442789841, ThemeFileId.CancelInvitation);

		private static readonly ToolbarButton spellCheck = new ToolbarButton("spellcheck", ToolbarButtonFlags.Image | ToolbarButtonFlags.ComboMenu, 1570327544, ThemeFileId.Spelling);

		private static readonly ToolbarButton today = new ToolbarButton("today", ToolbarButtonFlags.Text | ToolbarButtonFlags.Tooltip, -1483924202, ThemeFileId.None, -1062953366);

		private static readonly ToolbarButton timeZoneDropDown = new TimeZoneDropDownToolbarButton();

		private static readonly ToolbarButton weekView = new ToolbarButton("week", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, -867675667, ThemeFileId.WeekView, -382962026);

		private static readonly ToolbarButton workWeekView = new ToolbarButton("workweek", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky | ToolbarButtonFlags.Radio | ToolbarButtonFlags.Tooltip, 483590582, ThemeFileId.WorkWeekView, 179982437);

		private static readonly ToolbarButton recover = new ToolbarButton("recover", ToolbarButtonFlags.Image, 1288772053, ThemeFileId.Recover);

		private static readonly ToolbarButton purge = new ToolbarButton("delete", ToolbarButtonFlags.Image, -870504544, ThemeFileId.Delete);

		private static readonly ToolbarButton recurrence = new ToolbarButton("rcr", ToolbarButtonFlags.Text, -1955658819, ThemeFileId.None);

		private static readonly ToolbarButton recurrenceImageOnly = new ToolbarButton("rcr", ToolbarButtonFlags.Image, -1955658819, ThemeFileId.Recurrence);

		private static readonly ToolbarButton createRule = new ToolbarButton("crrul", ToolbarButtonFlags.Image, 1219103799, ThemeFileId.RulesSmall);

		private static readonly ToolbarButton messageEncryptContents = new ToolbarButton("msgencypt", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky, -155608005, ThemeFileId.MessageEncrypted);

		private static readonly ToolbarButton messageDigitalSignature = new ToolbarButton("msgsgnd", ToolbarButtonFlags.Image | ToolbarButtonFlags.Sticky, -1495007479, ThemeFileId.MessageSigned);

		private static readonly ToolbarButton move = new ToolbarButton("move", ToolbarButtonFlags.Image | ToolbarButtonFlags.CustomMenu, 1182470434, ThemeFileId.Move);

		private static readonly ToolbarButton moveWithText = new ToolbarButton("move", ToolbarButtonFlags.Text | ToolbarButtonFlags.Image | ToolbarButtonFlags.CustomMenu, 1414245993, ThemeFileId.Move);

		private static readonly ToolbarButton moveTextOnly = new ToolbarButton("move", ToolbarButtonFlags.Text | ToolbarButtonFlags.CustomMenu, 1414245993, ThemeFileId.None);

		private static readonly ToolbarButton editTextOnly = new ToolbarButton("edit", ToolbarButtonFlags.Text, -309370743, ThemeFileId.None);

		private static readonly ToolbarButton newPersonalAutoAttendant = new ToolbarButton("newpaa", ToolbarButtonFlags.Text, 699355213, ThemeFileId.None);

		private static readonly ToolbarButton moveUp = new ToolbarButton("up", ToolbarButtonFlags.Image, 137938150, ThemeFileId.Previous);

		private static readonly ToolbarButton moveDown = new ToolbarButton("dwn", ToolbarButtonFlags.Image, -798660877, ThemeFileId.Next);

		private static readonly ToolbarButton chat = new ToolbarButton("chat", ToolbarButtonFlags.Text, -124986716, ThemeFileId.None);

		private static readonly ToolbarButton addToBuddyList = new ToolbarButton("ablst", ToolbarButtonFlags.Image, 1457127060, ThemeFileId.AddBuddy);

		private static readonly ToolbarButton addToBuddyListWithText = new ToolbarButton("ablst", ToolbarButtonFlags.Text, 1457127060, ThemeFileId.None);

		private static readonly ToolbarButton removeFromBuddyList = new ToolbarButton("rmblst", ToolbarButtonFlags.Image, -205408082, ThemeFileId.RemoveBuddy);

		private static readonly ToolbarButton removeFromBuddyListWithText = new ToolbarButton("rmblst", ToolbarButtonFlags.Text, -205408082, ThemeFileId.None);

		private static readonly ToolbarButton sendATextMessage = new ToolbarButton("sendsms", ToolbarButtonFlags.Image, -843330244, ThemeFileId.Sms);

		private static readonly ToolbarButton newSms = new ToolbarButton("newsms", ToolbarButtonFlags.ImageAndText, 1509309420, ThemeFileId.Sms);

		private static readonly ToolbarButton sendSms = new ToolbarButton("send", ToolbarButtonFlags.Text, -158743924, ThemeFileId.None);

		private static readonly ToolbarButton inviteContact = new ToolbarButton("invt", ToolbarButtonFlags.Text, 923791697, ThemeFileId.None);

		private static readonly ToolbarButton filterCombo = new ToolbarButton("fltrc", ToolbarButtonFlags.Text | ToolbarButtonFlags.ComboMenu | ToolbarButtonFlags.CustomMenu, -1623789156, ThemeFileId.None, -645448913);

		private static readonly ToolbarButton addThisCalendar = new ToolbarButton("addThisCalendar", ToolbarButtonFlags.Text, 2009299861, ThemeFileId.None);

		private static readonly ToolbarButton sharingMyCalendar = new ToolbarButton("shareMyCalendar", ToolbarButtonFlags.Text, -1011044205, ThemeFileId.None);

		private static readonly ToolbarButton sharingDeclineMenu = new ToolbarButton("decline", ToolbarButtonFlags.Text | ToolbarButtonFlags.Menu, -788387211, ThemeFileId.None);

		private static readonly ToolbarButton subscribe = new ToolbarButton("subscribe", ToolbarButtonFlags.Text, -419851974, ThemeFileId.None);

		private static readonly ToolbarButton subscribeToThisCalendar = new ToolbarButton("addThisCalendar", ToolbarButtonFlags.Text, -1230529569, ThemeFileId.None);

		private static readonly ToolbarButton viewThisCalendar = new ToolbarButton("viewcal", ToolbarButtonFlags.Text, -142048603, ThemeFileId.None);

		private static readonly ToolbarButton messageNoteInDropDown = new ToolbarButton("messagenotedrpdwn", ToolbarButtonFlags.ImageAndText, 1146710980, ThemeFileId.MessageAnnotation);

		private static readonly ToolbarButton messageNoteInToolbar = new ToolbarButton("messagenotetoolbar", ToolbarButtonFlags.Image, 1146710980, ThemeFileId.MessageAnnotation);

		private static bool isInitialized = ToolbarButtons.Initialize();
	}
}
