using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class EditMeetingResponsePreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.Item;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			HttpContext httpContext = owaContext.HttpContext;
			HttpRequest request = httpContext.Request;
			UserContext userContext = owaContext.UserContext;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			string formParameter = Utilities.GetFormParameter(request, "hidid", true);
			string formParameter2 = Utilities.GetFormParameter(request, "hidchk", true);
			string formParameter3 = Utilities.GetFormParameter(request, "hidcmdpst", true);
			MessageItem messageItem = null;
			try
			{
				if (string.CompareOrdinal(formParameter3, "cls") != 0 && Utilities.IsPostRequest(request))
				{
					if (ObjectClass.IsMeetingRequest(owaContext.FormsRegistryContext.Type))
					{
						messageItem = Utilities.GetItem<MeetingRequest>(userContext, formParameter, formParameter2, new PropertyDefinition[0]);
					}
					else if (ObjectClass.IsMeetingCancellation(owaContext.FormsRegistryContext.Type))
					{
						messageItem = Utilities.GetItem<MeetingCancellation>(userContext, formParameter, formParameter2, new PropertyDefinition[0]);
					}
					else
					{
						if (!ObjectClass.IsMeetingResponse(owaContext.FormsRegistryContext.Type))
						{
							throw new OwaInvalidRequestException("Invalid Type");
						}
						messageItem = Utilities.GetItem<MeetingResponse>(userContext, formParameter, formParameter2, new PropertyDefinition[0]);
					}
					this.base64ItemId = messageItem.Id.ObjectId.ToBase64String();
				}
				string text = null;
				string a;
				if ((a = formParameter3) != null)
				{
					if (!(a == "snd"))
					{
						if (a == "cls")
						{
							return userContext.LastClientViewState.ToPreFormActionResponse();
						}
						if (!(a == "attch"))
						{
							if (!(a == "addrBook"))
							{
								if (a == "viewRcptWhenEdt")
								{
									EditMessageHelper.UpdateMessage(messageItem, userContext, request, out text);
									if (!string.IsNullOrEmpty(text))
									{
										return this.RedirectToCompose(owaContext, text);
									}
									return EditMessageHelper.RedirectToRecipient(owaContext, messageItem, AddressBook.Mode.EditMeetingResponse);
								}
							}
							else
							{
								EditMessageHelper.UpdateMessage(messageItem, userContext, request, out text);
								if (!string.IsNullOrEmpty(text))
								{
									return this.RedirectToCompose(owaContext, text);
								}
								return EditMessageHelper.RedirectToPeoplePicker(owaContext, messageItem, AddressBook.Mode.EditMeetingResponse);
							}
						}
						else
						{
							EditMessageHelper.UpdateMessage(messageItem, userContext, request, out text);
							if (!string.IsNullOrEmpty(text))
							{
								return this.RedirectToCompose(owaContext, text);
							}
							return this.RedirectToAttachmentManager(owaContext);
						}
					}
					else
					{
						StoreObjectId storeObjectId = null;
						MeetingResponse meetingResponse = messageItem as MeetingResponse;
						if (meetingResponse != null)
						{
							storeObjectId = meetingResponse.AssociatedMeetingRequestId;
						}
						bool flag = EditMessageHelper.UpdateMessage(messageItem, userContext, request, out text);
						if (!string.IsNullOrEmpty(text))
						{
							return this.RedirectToCompose(owaContext, text);
						}
						if (!flag && Utilities.GetFormParameter(request, "hidunrslrcp", false) == "1")
						{
							flag = true;
						}
						if (flag)
						{
							return this.RedirectToCompose(owaContext, LocalizedStrings.GetNonEncoded(-2019438132));
						}
						text = EditMessageHelper.SendMessage(userContext, messageItem);
						if (text != null)
						{
							return this.RedirectToCompose(owaContext, text);
						}
						if (storeObjectId != null)
						{
							StoreObjectId storeObjectId2 = null;
							using (MeetingRequest item = Utilities.GetItem<MeetingRequest>(userContext, storeObjectId, new PropertyDefinition[0]))
							{
								storeObjectId2 = item.ParentId;
							}
							if (storeObjectId2 != null)
							{
								if (Utilities.IsDefaultFolderId(userContext.MailboxSession, storeObjectId2, DefaultFolderType.DeletedItems))
								{
									Utilities.DeleteItems(userContext, DeleteItemFlags.SoftDelete, new StoreId[]
									{
										storeObjectId
									});
								}
								else
								{
									Utilities.DeleteItems(userContext, DeleteItemFlags.MoveToDeletedItems, new StoreId[]
									{
										storeObjectId
									});
								}
							}
						}
						userContext.ForceNewSearch = true;
						return userContext.LastClientViewState.ToPreFormActionResponse();
					}
				}
				throw new OwaInvalidRequestException("Invalid command form parameter");
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
				}
			}
			PreFormActionResponse result;
			return result;
		}

		private PreFormActionResponse RedirectToCompose(OwaContext owaContext, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage))
			{
				owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateText(errorMessage, InfobarMessageType.Error);
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.Type = owaContext.FormsRegistryContext.Type;
			preFormActionResponse.Action = "Open";
			preFormActionResponse.State = "Draft";
			preFormActionResponse.AddParameter("id", this.base64ItemId);
			return preFormActionResponse;
		}

		private PreFormActionResponse RedirectToAttachmentManager(OwaContext owaContext)
		{
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Dialog;
			preFormActionResponse.Type = "Attach";
			preFormActionResponse.AddParameter("id", this.base64ItemId);
			return preFormActionResponse;
		}

		private const string CommandFormParameter = "hidcmdpst";

		private const string MessageIdFormParameter = "hidid";

		private const string ChangeKeyFormParameter = "hidchk";

		private const string UnresolvedRecipientsFormParameter = "hidunrslrcp";

		private string base64ItemId;
	}
}
