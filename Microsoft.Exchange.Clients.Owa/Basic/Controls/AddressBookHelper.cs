using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class AddressBookHelper
	{
		public static void AddRecipientsToDraft(string[] ids, Item draft, RecipientItemType type, UserContext userContext)
		{
			if (draft == null)
			{
				throw new ArgumentNullException("draft");
			}
			MessageItem messageItem = draft as MessageItem;
			CalendarItemBase calendarItemBase = draft as CalendarItemBase;
			if (messageItem == null && calendarItemBase == null)
			{
				throw new ArgumentException("The draft should be a MessageItem or a CalendarItemBase while it is now a " + draft.GetType().Name);
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("usercontext");
			}
			if (ids == null || ids.Length == 0)
			{
				throw new ArgumentException("ids is null or empty.", "ids");
			}
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
			SortBy sortBy = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Descending);
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
			for (int i = 0; i < ids.Length; i++)
			{
				Guid guidFromBase64String = Utilities.GetGuidFromBase64String(ids[i]);
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guidFromBase64String);
				ADRecipient[] array = recipientSession.Find(null, QueryScope.SubTree, filter, sortBy, 1);
				if (array != null && array.Length != 0)
				{
					Participant participant = new Participant(array[0]);
					if (participant != null)
					{
						AddressBookHelper.AddParticipantToItem(draft, type, participant);
						int recipientFlags = 0;
						if (draft is CalendarItem && DirectoryAssistance.IsADRecipientRoom(array[0]))
						{
							recipientFlags = 2;
						}
						if (recipientCache != null && userContext.UserOptions.AddRecipientsToAutoCompleteCache)
						{
							string participantProperty = Utilities.GetParticipantProperty<string>(participant, ParticipantSchema.SmtpAddress, null);
							recipientCache.AddEntry(array[0].DisplayName, participantProperty, array[0].LegacyExchangeDN, string.Empty, "EX", AddressOrigin.Directory, recipientFlags, null, EmailAddressIndex.None);
						}
					}
				}
			}
			if (recipientCache != null && recipientCache.IsDirty)
			{
				recipientCache.Commit(true);
			}
			if (messageItem != null)
			{
				AddressBookHelper.SaveItem(draft);
				return;
			}
			if (calendarItemBase != null)
			{
				EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase);
			}
		}

		public static void AddContactsToDraft(Item draft, RecipientItemType type, UserContext userContext, params string[] ids)
		{
			if (draft == null)
			{
				throw new ArgumentNullException("draft");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("usercontext");
			}
			if (ids == null || ids.Length == 0)
			{
				throw new ArgumentException("ids is null or empty.", "ids");
			}
			Participant participant = null;
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
			int i = 0;
			while (i < ids.Length)
			{
				StoreObjectId storeObjectId = Utilities.CreateStoreObjectId(userContext.MailboxSession, ids[i]);
				if (storeObjectId.ObjectType == StoreObjectType.DistributionList)
				{
					using (DistributionList item = Utilities.GetItem<DistributionList>(userContext, storeObjectId, new PropertyDefinition[]
					{
						StoreObjectSchema.DisplayName
					}))
					{
						participant = item.GetAsParticipant();
						goto IL_113;
					}
					goto IL_9B;
				}
				goto IL_9B;
				IL_113:
				if (participant != null)
				{
					if (recipientCache != null && userContext.UserOptions.AddRecipientsToAutoCompleteCache)
					{
						string participantProperty = Utilities.GetParticipantProperty<string>(participant, ParticipantSchema.SmtpAddress, null);
						recipientCache.AddEntry(participant.DisplayName, participantProperty, participant.EmailAddress, string.Empty, participant.RoutingType, AddressOrigin.Store, 0, storeObjectId.ToBase64String(), ((StoreParticipantOrigin)participant.Origin).EmailAddressIndex);
					}
					AddressBookHelper.AddParticipantToItem(draft, type, participant);
					participant = null;
				}
				i++;
				continue;
				IL_9B:
				using (Contact item2 = Utilities.GetItem<Contact>(userContext, storeObjectId, AddressBookHelper.contactsProperties))
				{
					foreach (KeyValuePair<EmailAddressIndex, Participant> keyValuePair in item2.EmailAddresses)
					{
						if (keyValuePair.Value != null && !string.IsNullOrEmpty(keyValuePair.Value.EmailAddress))
						{
							participant = keyValuePair.Value;
							break;
						}
					}
				}
				goto IL_113;
			}
			if (recipientCache != null && recipientCache.IsDirty)
			{
				recipientCache.Commit(true);
			}
			CalendarItemBase calendarItemBase = draft as CalendarItemBase;
			if (calendarItemBase != null)
			{
				EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase);
				return;
			}
			if (draft is MessageItem)
			{
				AddressBookHelper.SaveItem(draft);
			}
		}

		public static void AddParticipantToItem(Item item, RecipientItemType type, Participant participant)
		{
			MessageItem messageItem = item as MessageItem;
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			if (messageItem != null)
			{
				messageItem.Recipients.Add(participant, type);
				return;
			}
			if (calendarItemBase != null)
			{
				CalendarUtilities.AddAttendee(calendarItemBase, participant, CalendarUtilities.RecipientTypeToAttendeeType(type));
				if (!calendarItemBase.IsMeeting)
				{
					calendarItemBase.IsMeeting = true;
				}
			}
		}

		public static void RenderRecipients(TextWriter writer, RecipientItemType type, MessageItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			bool flag = true;
			int num = 0;
			bool property = ItemUtility.GetProperty<bool>(item, MessageItemSchema.IsResend, false);
			foreach (Recipient recipient in item.Recipients)
			{
				if (recipient.RecipientItemType == type && (!property || !recipient.Submitted))
				{
					if (!flag && recipient.Participant.RoutingType != null)
					{
						writer.Write("; ");
					}
					AddressBookHelper.RenderParticipant(writer, num, recipient.Participant);
					flag = false;
				}
				num++;
			}
			if (flag)
			{
				writer.Write("&nbsp;");
			}
		}

		public static void RenderAttendees(TextWriter writer, AttendeeType type, CalendarItemBase item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			bool flag = true;
			int num = 0;
			foreach (Attendee attendee in item.AttendeeCollection)
			{
				if (attendee.AttendeeType == type)
				{
					if (!flag && attendee.Participant.RoutingType != null)
					{
						writer.Write("; ");
					}
					AddressBookHelper.RenderParticipant(writer, num, attendee.Participant);
					flag = false;
				}
				num++;
			}
			if (flag)
			{
				writer.Write("&nbsp;");
			}
		}

		private static void RenderParticipant(TextWriter writer, int recipientIndex, Participant participant)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			if (!string.IsNullOrEmpty(participant.DisplayName))
			{
				writer.Write("<span");
				if (Utilities.GetParticipantProperty<bool>(participant, ParticipantSchema.IsDistributionList, false))
				{
					writer.Write(" class=\"spndl\"");
				}
				writer.Write(">");
				Utilities.HtmlEncode(participant.DisplayName, writer);
				writer.Write("</span><span class=\"spnrm\">[<a href=\"#\" onClick=\"return onRmRcpt('");
				writer.Write(recipientIndex);
				writer.Write("');\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(341226654));
				writer.Write("</a>]</span>");
				return;
			}
			writer.Write("&nbsp;");
		}

		internal static void SaveItem(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = item.Id != null;
			ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new OwaSaveConflictException(LocalizedStrings.GetNonEncoded(-482397486), conflictResolutionResult);
			}
			item.Load();
			if (Globals.ArePerfCountersEnabled)
			{
				if (flag)
				{
					OwaSingleCounters.ItemsUpdated.Increment();
					return;
				}
				OwaSingleCounters.ItemsCreated.Increment();
			}
		}

		public static void RenderAddressBookButton(TextWriter writer, string buttonId, Strings.IDs label)
		{
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"w100\"><tr><td nowrap align=\"center\"><a href=\"#\"");
			writer.Write(" class=\"btnRW\" onClick=\"return onClkABBtn({0});\">{1}</a></td></tr></table>", buttonId, LocalizedStrings.GetHtmlEncoded(label));
		}

		public static Item GetItem(UserContext userContext, AddressBook.Mode viewMode, StoreObjectId itemId, string changeKey)
		{
			Item result = null;
			if (viewMode == AddressBook.Mode.EditMessage || viewMode == AddressBook.Mode.EditMeetingResponse)
			{
				if (!string.IsNullOrEmpty(changeKey))
				{
					result = Utilities.GetItem<MessageItem>(userContext, itemId, changeKey, new PropertyDefinition[0]);
				}
				else
				{
					result = Utilities.GetItem<MessageItem>(userContext, itemId, new PropertyDefinition[0]);
				}
			}
			else if (viewMode == AddressBook.Mode.EditCalendar)
			{
				CalendarItemBase calendarItemBase;
				EditCalendarItemHelper.GetCalendarItem(userContext, itemId, null, changeKey, true, out calendarItemBase);
				result = calendarItemBase;
			}
			return result;
		}

		public static AddressBook.Mode TryReadAddressBookMode(HttpRequest request, AddressBook.Mode defaultValue)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "ctx", false);
			if (string.IsNullOrEmpty(queryStringParameter))
			{
				return defaultValue;
			}
			int num;
			if (!int.TryParse(queryStringParameter, out num) || num < 0 || num > 4)
			{
				throw new OwaInvalidRequestException("Invalid context value in the query string parameter");
			}
			return (AddressBook.Mode)num;
		}

		public static PreFormActionResponse RedirectToEdit(UserContext userContext, Item item, AddressBook.Mode viewMode)
		{
			AddressBookViewState addressBookViewState = userContext.LastClientViewState as AddressBookViewState;
			if (addressBookViewState != null)
			{
				userContext.LastClientViewState = addressBookViewState.PreviousViewState;
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			if (item.Id != null)
			{
				preFormActionResponse.AddParameter("id", item.Id.ObjectId.ToBase64String());
			}
			preFormActionResponse.Action = "Open";
			if (viewMode == AddressBook.Mode.EditMessage || viewMode == AddressBook.Mode.EditMeetingResponse)
			{
				preFormActionResponse.State = "Draft";
			}
			preFormActionResponse.Type = item.ClassName;
			return preFormActionResponse;
		}

		private static readonly PropertyDefinition[] contactsProperties = new PropertyDefinition[]
		{
			ContactSchema.Email1DisplayName,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1AddrType,
			ContactSchema.Email2DisplayName,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email2AddrType,
			ContactSchema.Email3DisplayName,
			ContactSchema.Email3EmailAddress,
			ContactSchema.Email3AddrType
		};
	}
}
