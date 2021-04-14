using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ReadContactPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			HttpContext httpContext = owaContext.HttpContext;
			UserContext userContext = owaContext.UserContext;
			HttpRequest request = httpContext.Request;
			applicationElement = ApplicationElement.Item;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			if (owaContext.FormsRegistryContext.Action == null)
			{
				throw new OwaInvalidRequestException("Action is missing");
			}
			string formParameter = Utilities.GetFormParameter(request, "hidid", true);
			StoreObjectId storeObjectId = Utilities.CreateStoreObjectId(userContext.MailboxSession, formParameter);
			string action2;
			if ((action2 = owaContext.FormsRegistryContext.Action) != null)
			{
				if (!(action2 == "Mail"))
				{
					if (!(action2 == "MeetingRequest"))
					{
						if (action2 == "Delete")
						{
							goto IL_1E9;
						}
						if (!(action2 == "AddRcpt"))
						{
							goto IL_33E;
						}
						AddressBook.Mode mode = AddressBookHelper.TryReadAddressBookMode(request, AddressBook.Mode.None);
						if (mode != AddressBook.Mode.EditMessage && mode != AddressBook.Mode.EditMeetingResponse && mode != AddressBook.Mode.EditCalendar)
						{
							throw new OwaInvalidRequestException("Not in edit mode");
						}
						string queryStringParameter = Utilities.GetQueryStringParameter(request, "id", false);
						StoreObjectId itemId = null;
						if (!string.IsNullOrEmpty(queryStringParameter))
						{
							itemId = Utilities.CreateStoreObjectId(userContext.MailboxSession, queryStringParameter);
						}
						string queryStringParameter2 = Utilities.GetQueryStringParameter(request, "chk", false);
						RecipientItemType type2 = (RecipientItemType)RequestParser.TryGetIntValueFromQueryString(request, "rw", 1);
						using (Item item = AddressBookHelper.GetItem(userContext, mode, itemId, queryStringParameter2))
						{
							Participant participant = Utilities.CreateParticipantFromQueryString(userContext, request);
							if (participant != null)
							{
								AddressBookHelper.AddParticipantToItem(item, type2, participant);
								CalendarItemBase calendarItemBase = item as CalendarItemBase;
								if (calendarItemBase != null)
								{
									EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase);
								}
								else if (item is MessageItem)
								{
									AddressBookHelper.SaveItem(item);
								}
							}
							return AddressBookHelper.RedirectToEdit(userContext, item, mode);
						}
						goto IL_33E;
					}
				}
				else
				{
					using (MessageItem messageItem = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId))
					{
						messageItem[ItemSchema.ConversationIndexTracking] = true;
						AddressBookHelper.AddContactsToDraft(messageItem, RecipientItemType.To, userContext, new string[]
						{
							storeObjectId.ToBase64String()
						});
						preFormActionResponse.AddParameter("id", messageItem.Id.ObjectId.ToBase64String());
						preFormActionResponse.ApplicationElement = ApplicationElement.Item;
						preFormActionResponse.Type = "IPM.Note";
						preFormActionResponse.Action = "Open";
						preFormActionResponse.State = "Draft";
						return preFormActionResponse;
					}
				}
				using (CalendarItemBase calendarItemBase2 = EditCalendarItemHelper.CreateDraft(userContext, userContext.CalendarFolderId))
				{
					calendarItemBase2.IsMeeting = true;
					AddressBookHelper.AddContactsToDraft(calendarItemBase2, RecipientItemType.To, userContext, new string[]
					{
						storeObjectId.ToBase64String()
					});
					preFormActionResponse.Action = "Open";
					preFormActionResponse.ApplicationElement = ApplicationElement.Item;
					preFormActionResponse.Type = calendarItemBase2.ClassName;
					EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase2);
					return preFormActionResponse;
				}
				IL_1E9:
				string formParameter2 = Utilities.GetFormParameter(request, "hidcmdpst", true);
				if (formParameter2 == "d")
				{
					Utilities.DeleteItems(userContext, DeleteItemFlags.SoftDelete, new StoreId[]
					{
						storeObjectId
					});
				}
				else
				{
					if (!(formParameter2 == "m"))
					{
						throw new OwaInvalidRequestException("Unknown delete command");
					}
					Utilities.DeleteItems(userContext, DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						storeObjectId
					});
				}
				userContext.ForceNewSearch = true;
				preFormActionResponse = userContext.LastClientViewState.ToPreFormActionResponse();
				return preFormActionResponse;
			}
			IL_33E:
			throw new OwaInvalidRequestException("Invalid action for readcontact preformaction");
		}

		private const string FormContactId = "hidid";

		private const string FormCommand = "hidcmdpst";

		private const string MoveToDeletedItemsCommand = "m";

		private const string SoftDeleteCommand = "d";

		private const string ChangeKeyQueryStringParameter = "chk";

		private const string RecipientWellQueryStringParameter = "rw";
	}
}
