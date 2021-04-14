using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class AddressBookPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			UserContext userContext = owaContext.UserContext;
			HttpRequest request = owaContext.HttpContext.Request;
			string[] array = null;
			applicationElement = ApplicationElement.Item;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			string formParameter = Utilities.GetFormParameter(request, "hidAB");
			int num = 0;
			StoreObjectId storeObjectId = null;
			string changeKey = null;
			string[] array2 = formParameter.Split(new char[]
			{
				';'
			});
			if (array2 != null && array2.Length > 0)
			{
				if (string.CompareOrdinal(array2[0], "Ad") == 0)
				{
					num = 1;
				}
				else
				{
					if (string.CompareOrdinal(array2[0], "Con") != 0)
					{
						throw new OwaInvalidRequestException("Invalid search location for addressbook");
					}
					num = 2;
				}
			}
			string action2 = owaContext.FormsRegistryContext.Action;
			if (action2 == null)
			{
				throw new OwaInvalidRequestException("Action query string parameter is missing");
			}
			object obj = AddressBookPreFormAction.actionParser.Parse(action2);
			AddressBookPreFormAction.Action action3 = (AddressBookPreFormAction.Action)obj;
			string text = request.Form["chkRcpt"];
			if (!string.IsNullOrEmpty(text))
			{
				array = text.Split(new char[]
				{
					','
				});
			}
			AddressBook.Mode mode = AddressBookHelper.TryReadAddressBookMode(request, AddressBook.Mode.None);
			if (AddressBook.IsEditingMode(mode))
			{
				string formParameter2 = Utilities.GetFormParameter(request, "hidid", false);
				changeKey = Utilities.GetFormParameter(request, "hidchk", false);
				if (!string.IsNullOrEmpty(formParameter2))
				{
					storeObjectId = Utilities.CreateStoreObjectId(userContext.MailboxSession, formParameter2);
					if (storeObjectId == null)
					{
						throw new OwaInvalidRequestException("ItemId cannot be null");
					}
				}
			}
			switch (action3)
			{
			case AddressBookPreFormAction.Action.Done:
				if (AddressBook.IsEditingMode(mode))
				{
					using (Item item = AddressBookHelper.GetItem(userContext, mode, storeObjectId, changeKey))
					{
						if (array != null && array.Length > 0)
						{
							RecipientItemType type2 = RecipientItemType.To;
							string formParameter3 = Utilities.GetFormParameter(request, "hidrw");
							if (!string.IsNullOrEmpty(formParameter3))
							{
								int num2;
								if (!int.TryParse(formParameter3, out num2) || num2 < 1 || num2 > 3)
								{
									type2 = RecipientItemType.To;
								}
								else
								{
									type2 = (RecipientItemType)num2;
								}
							}
							if (num == 1)
							{
								AddressBookHelper.AddRecipientsToDraft(array, item, type2, userContext);
							}
							else if (num == 2)
							{
								AddressBookHelper.AddContactsToDraft(item, type2, userContext, array);
							}
						}
						preFormActionResponse = AddressBookHelper.RedirectToEdit(userContext, item, mode);
						break;
					}
				}
				throw new OwaInvalidRequestException("This action must be in editing mode");
			case AddressBookPreFormAction.Action.Mail:
				if (array != null && array.Length > 0)
				{
					using (Item item2 = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId))
					{
						item2[ItemSchema.ConversationIndexTracking] = true;
						if (num == 1)
						{
							AddressBookHelper.AddRecipientsToDraft(array, item2, RecipientItemType.To, userContext);
						}
						else if (num == 2)
						{
							AddressBookHelper.AddContactsToDraft(item2, RecipientItemType.To, userContext, array);
						}
						preFormActionResponse.ApplicationElement = ApplicationElement.Item;
						preFormActionResponse.Type = "IPM.Note";
						preFormActionResponse.Action = "Open";
						preFormActionResponse.State = "Draft";
						preFormActionResponse.AddParameter("id", item2.Id.ObjectId.ToBase64String());
						break;
					}
				}
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = "IPM.Note";
				preFormActionResponse.Action = "New";
				break;
			case AddressBookPreFormAction.Action.MeetingRequest:
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = "IPM.Appointment";
				if (array != null && array.Length > 0)
				{
					using (CalendarItemBase calendarItemBase = EditCalendarItemHelper.CreateDraft(userContext, userContext.CalendarFolderId))
					{
						calendarItemBase.IsMeeting = true;
						if (num == 1)
						{
							AddressBookHelper.AddRecipientsToDraft(array, calendarItemBase, RecipientItemType.To, userContext);
						}
						else if (num == 2)
						{
							AddressBookHelper.AddContactsToDraft(calendarItemBase, RecipientItemType.To, userContext, array);
						}
						preFormActionResponse.Action = "Open";
						EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase);
						break;
					}
				}
				preFormActionResponse.AddParameter("mr", "1");
				preFormActionResponse.Action = "New";
				break;
			case AddressBookPreFormAction.Action.Close:
				if (AddressBook.IsEditingMode(mode))
				{
					using (Item item3 = AddressBookHelper.GetItem(userContext, mode, storeObjectId, changeKey))
					{
						preFormActionResponse = AddressBookHelper.RedirectToEdit(userContext, item3, mode);
						break;
					}
				}
				throw new OwaInvalidRequestException("This action must be in editing mode");
			case AddressBookPreFormAction.Action.AddToContacts:
			{
				string type3 = "IPM.Contact";
				string text2 = null;
				if (array == null || array.Length != 1)
				{
					throw new OwaInvalidRequestException("User must select some recipient to add and can only add one recipient to contacts at one time");
				}
				ADObjectId adobjectId = DirectoryAssistance.ParseADObjectId(array[0]);
				if (adobjectId == null)
				{
					throw new OwaADObjectNotFoundException();
				}
				IRecipientSession recipientSession = Utilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.FullyConsistent, true, userContext);
				ADRecipient adrecipient = recipientSession.Read(adobjectId);
				if (adrecipient == null)
				{
					throw new OwaADObjectNotFoundException();
				}
				using (ContactBase contactBase = ContactUtilities.AddADRecipientToContacts(userContext, adrecipient))
				{
					if (contactBase != null)
					{
						contactBase.Load();
						text2 = contactBase.Id.ObjectId.ToBase64String();
						type3 = contactBase.ClassName;
					}
				}
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = type3;
				if (text2 != null)
				{
					preFormActionResponse.Action = "Open";
					preFormActionResponse.State = "Draft";
					preFormActionResponse.AddParameter("id", text2);
				}
				else
				{
					preFormActionResponse.Action = "New";
				}
				break;
			}
			default:
				throw new OwaInvalidRequestException("Invalid request for addressbook preformaction");
			}
			return preFormActionResponse;
		}

		private const string RecipientCheckBox = "chkRcpt";

		private const string FormDraftId = "hidid";

		private const string ChangeKeyString = "hidchk";

		private const string AdrressBook = "hidAB";

		private const string RecipientWellString = "hidrw";

		private static FastEnumParser actionParser = new FastEnumParser(typeof(AddressBookPreFormAction.Action));

		private enum Action
		{
			Done,
			Mail,
			MeetingRequest,
			Close,
			AddToContacts
		}
	}
}
