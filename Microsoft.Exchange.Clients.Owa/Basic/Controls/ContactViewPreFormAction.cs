using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ContactViewPreFormAction : IPreFormAction
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
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "a", true);
			object obj = ContactViewPreFormAction.actionParser.Parse(queryStringParameter);
			ContactViewPreFormAction.Action action2 = (ContactViewPreFormAction.Action)obj;
			string text = request.Form["chkRcpt"];
			if (!string.IsNullOrEmpty(text))
			{
				array = text.Split(new char[]
				{
					','
				});
			}
			switch (action2)
			{
			case ContactViewPreFormAction.Action.Mail:
				if (array != null && array.Length > 0)
				{
					using (MessageItem messageItem = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId))
					{
						messageItem[ItemSchema.ConversationIndexTracking] = true;
						AddressBookHelper.AddContactsToDraft(messageItem, RecipientItemType.To, userContext, array);
						preFormActionResponse.ApplicationElement = ApplicationElement.Item;
						preFormActionResponse.Type = "IPM.Note";
						preFormActionResponse.Action = "Open";
						preFormActionResponse.State = "Draft";
						preFormActionResponse.AddParameter("id", messageItem.Id.ObjectId.ToBase64String());
						break;
					}
				}
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = "IPM.Note";
				preFormActionResponse.Action = "New";
				break;
			case ContactViewPreFormAction.Action.MeetingRequest:
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = "IPM.Appointment";
				if (array != null && array.Length > 0)
				{
					using (CalendarItemBase calendarItemBase = EditCalendarItemHelper.CreateDraft(userContext, userContext.CalendarFolderId))
					{
						calendarItemBase.IsMeeting = true;
						AddressBookHelper.AddContactsToDraft(calendarItemBase, RecipientItemType.To, userContext, array);
						preFormActionResponse.Action = "Open";
						EditCalendarItemHelper.CreateUserContextData(userContext, calendarItemBase);
						break;
					}
				}
				preFormActionResponse.AddParameter("mr", "1");
				preFormActionResponse.Action = "New";
				break;
			default:
				throw new OwaInvalidRequestException("Invalid request for addressbook preformaction");
			}
			return preFormActionResponse;
		}

		private const string RecipientCheckBox = "chkRcpt";

		private const string ToolbarAction = "a";

		private static FastEnumParser actionParser = new FastEnumParser(typeof(ContactViewPreFormAction.Action));

		private enum Action
		{
			Mail,
			MeetingRequest
		}
	}
}
