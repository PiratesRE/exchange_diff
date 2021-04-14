using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class MeetingPagePreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			state = string.Empty;
			action = string.Empty;
			HttpContext httpContext = owaContext.HttpContext;
			this.userContext = owaContext.UserContext;
			HttpRequest request = httpContext.Request;
			if (!Utilities.IsPostRequest(request) && owaContext.FormsRegistryContext.Action != "Prev" && owaContext.FormsRegistryContext.Action != "Next")
			{
				return this.userContext.LastClientViewState.ToPreFormActionResponse();
			}
			this.context = owaContext;
			StoreId storeId = null;
			this.itemType = this.context.FormsRegistryContext.Type;
			string text;
			string storeObjectId;
			if (Utilities.IsPostRequest(request))
			{
				text = Utilities.GetFormParameter(request, "hidid", false);
				if (text == null)
				{
					throw new OwaInvalidRequestException("MessageId is not set in the form");
				}
				string formParameter = Utilities.GetFormParameter(request, "hidchk", false);
				if (formParameter != null)
				{
					storeId = Utilities.CreateItemId(this.userContext.MailboxSession, text, formParameter);
				}
				storeObjectId = Utilities.GetFormParameter(request, "hidfldid", true);
				string formParameter2 = Utilities.GetFormParameter(request, "rdoRsp", false);
				int num;
				if (!string.IsNullOrEmpty(formParameter2) && int.TryParse(formParameter2, out num))
				{
					this.responseAction = (MeetingPagePreFormAction.ResponseAction)num;
				}
			}
			else
			{
				text = Utilities.GetQueryStringParameter(request, "id", false);
				storeObjectId = Utilities.GetQueryStringParameter(request, "fId", true);
			}
			ItemOperations.Result result = null;
			StoreObjectId storeObjectId2 = Utilities.CreateStoreObjectId(this.userContext.MailboxSession, text);
			StoreObjectId folderId = Utilities.CreateStoreObjectId(this.userContext.MailboxSession, storeObjectId);
			string action2;
			if ((action2 = owaContext.FormsRegistryContext.Action) != null)
			{
				if (<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x60002a7-1 == null)
				{
					<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x60002a7-1 = new Dictionary<string, int>(10)
					{
						{
							"Prev",
							0
						},
						{
							"Next",
							1
						},
						{
							"Del",
							2
						},
						{
							"RemoveFromCal",
							3
						},
						{
							"Junk",
							4
						},
						{
							"Close",
							5
						},
						{
							"Accept",
							6
						},
						{
							"Tentative",
							7
						},
						{
							"Decline",
							8
						},
						{
							"DeclineDelete",
							9
						}
					};
				}
				int num2;
				if (<PrivateImplementationDetails>{912F2AED-BF68-4DDC-9379-4CB89AA1AA01}.$$method0x60002a7-1.TryGetValue(action2, out num2))
				{
					switch (num2)
					{
					case 0:
						result = ItemOperations.GetNextViewItem(this.userContext, ItemOperations.Action.Prev, storeObjectId2, folderId);
						break;
					case 1:
						result = ItemOperations.GetNextViewItem(this.userContext, ItemOperations.Action.Next, storeObjectId2, folderId);
						break;
					case 2:
						result = this.DeleteItem(storeObjectId2, folderId);
						this.userContext.ForceNewSearch = true;
						break;
					case 3:
						result = this.RemoveFromCalendar(storeObjectId2, folderId);
						this.userContext.ForceNewSearch = true;
						break;
					case 4:
						if (!this.userContext.IsJunkEmailEnabled)
						{
							throw new OwaInvalidRequestException(Strings.JunkMailDisabled);
						}
						owaContext[OwaContextProperty.InfobarMessage] = JunkEmailHelper.MarkAsJunk(this.userContext, new StoreObjectId[]
						{
							storeObjectId2
						});
						this.userContext.ForceNewSearch = true;
						break;
					case 5:
						break;
					case 6:
						this.responseType = ResponseType.Accept;
						this.userContext.ForceNewSearch = true;
						break;
					case 7:
						this.responseType = ResponseType.Tentative;
						this.userContext.ForceNewSearch = true;
						break;
					case 8:
						this.responseType = ResponseType.Decline;
						this.userContext.ForceNewSearch = true;
						break;
					case 9:
						this.responseType = ResponseType.Decline;
						this.responseAction = MeetingPagePreFormAction.ResponseAction.SendResponseAndDelete;
						this.userContext.ForceNewSearch = true;
						break;
					default:
						goto IL_370;
					}
					if (this.responseType != ResponseType.None)
					{
						return this.ProcessResponse(storeObjectId2, storeId, folderId);
					}
					return ItemOperations.GetPreFormActionResponse(this.userContext, result);
				}
			}
			IL_370:
			throw new OwaInvalidRequestException("Unknown action '" + owaContext.FormsRegistryContext.Action + "' in MeetingPage PreFormAction.");
		}

		private static void UpdateItem(Item item, ResponseType responseType)
		{
			item[CalendarItemBaseSchema.ResponseType] = responseType;
			if (item is MeetingRequest)
			{
				((MessageItem)item).IsRead = true;
			}
		}

		private PreFormActionResponse ProcessResponse(StoreObjectId itemId, StoreId storeId, StoreObjectId folderId)
		{
			PreFormActionResponse result = null;
			switch (this.responseAction)
			{
			case MeetingPagePreFormAction.ResponseAction.SendResponse:
				result = this.ProcessNonEditResponse(MeetingPagePreFormAction.ResponseAction.SendResponse, storeId, itemId, folderId);
				break;
			case MeetingPagePreFormAction.ResponseAction.EditResponse:
			case MeetingPagePreFormAction.ResponseAction.SendResponseAndDelete:
				result = this.EditResponse(storeId);
				break;
			case MeetingPagePreFormAction.ResponseAction.NoResponse:
				result = this.ProcessNonEditResponse(MeetingPagePreFormAction.ResponseAction.NoResponse, storeId, itemId, folderId);
				break;
			}
			return result;
		}

		private PreFormActionResponse EditResponse(StoreId storeId)
		{
			HttpRequest request = this.context.HttpContext.Request;
			MeetingRequest meetingRequest = null;
			CalendarItemBase calendarItemBase = null;
			this.properties = new PropertyDefinition[]
			{
				MeetingMessageSchema.CalendarProcessed,
				StoreObjectSchema.ParentItemId
			};
			PreFormActionResponse result;
			try
			{
				if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(this.itemType))
				{
					calendarItemBase = Utilities.GetItem<CalendarItemBase>(this.userContext, storeId, false, this.properties);
				}
				else if (ObjectClass.IsMeetingRequest(this.itemType))
				{
					meetingRequest = Utilities.GetItem<MeetingRequest>(this.userContext, storeId, false, this.properties);
					calendarItemBase = MeetingUtilities.UpdateCalendarItem(meetingRequest);
				}
				Utilities.ValidateCalendarItemBaseStoreObject(calendarItemBase);
				result = this.EditResponseInternal(this.responseType, calendarItemBase);
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
				if (meetingRequest != null)
				{
					meetingRequest.Dispose();
					meetingRequest = null;
				}
			}
			return result;
		}

		private PreFormActionResponse EditResponseInternal(ResponseType responseType, CalendarItemBase calendarItemBase)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "MeetingPagePreFormAction.EditResponseInternal");
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			using (MeetingResponse meetingResponse = MeetingUtilities.EditResponse(responseType, calendarItemBase))
			{
				meetingResponse.Load();
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = meetingResponse.ClassName;
				preFormActionResponse.Action = "New";
				preFormActionResponse.AddParameter("id", meetingResponse.Id.ObjectId.ToBase64String());
			}
			return preFormActionResponse;
		}

		private PreFormActionResponse ProcessNonEditResponse(MeetingPagePreFormAction.ResponseAction responseAction, StoreId storeId, StoreObjectId itemId, StoreObjectId folderId)
		{
			ItemOperations.Result result = null;
			HttpRequest request = this.context.HttpContext.Request;
			StoreObjectType storeObjectType = StoreObjectType.Unknown;
			if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(this.itemType))
			{
				storeObjectType = StoreObjectType.CalendarItem;
			}
			else if (ObjectClass.IsMeetingRequest(this.itemType))
			{
				storeObjectType = StoreObjectType.MeetingRequest;
			}
			MeetingRequest meetingRequest = null;
			CalendarItemBase calendarItemBase = null;
			try
			{
				StoreObjectType storeObjectType2 = storeObjectType;
				if (storeObjectType2 != StoreObjectType.MeetingRequest)
				{
					if (storeObjectType2 == StoreObjectType.CalendarItem)
					{
						calendarItemBase = Utilities.GetItem<CalendarItemBase>(this.userContext, storeId, false, this.properties);
						MeetingUtilities.ThrowIfMeetingResponseInvalid(calendarItemBase);
					}
				}
				else
				{
					this.properties = new PropertyDefinition[]
					{
						MeetingMessageSchema.CalendarProcessed,
						StoreObjectSchema.ParentItemId
					};
					meetingRequest = Utilities.GetItem<MeetingRequest>(this.userContext, storeId, false, this.properties);
					calendarItemBase = MeetingUtilities.UpdateCalendarItem(meetingRequest);
				}
				if (meetingRequest != null && !Utilities.IsItemInDefaultFolder(meetingRequest, DefaultFolderType.DeletedItems))
				{
					MeetingPagePreFormAction.UpdateItem(meetingRequest, this.responseType);
					Utilities.SaveItem(meetingRequest);
				}
				MeetingPagePreFormAction.UpdateItem(calendarItemBase, this.responseType);
				Utilities.ValidateCalendarItemBaseStoreObject(calendarItemBase);
				if (meetingRequest != null)
				{
					result = this.CreateItemOperationsResultForDelete(itemId, folderId);
				}
				if (responseAction == MeetingPagePreFormAction.ResponseAction.SendResponse)
				{
					MeetingUtilities.NonEditResponse(this.responseType, calendarItemBase, true, meetingRequest);
				}
				else
				{
					if (responseAction != MeetingPagePreFormAction.ResponseAction.NoResponse)
					{
						throw new ArgumentException("Unhandled ResponseAction value.");
					}
					MeetingUtilities.NonEditResponse(this.responseType, calendarItemBase, false, meetingRequest);
				}
			}
			finally
			{
				if (meetingRequest != null)
				{
					meetingRequest.Dispose();
					meetingRequest = null;
				}
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
			}
			this.userContext.ForceNewSearch = true;
			return ItemOperations.GetPreFormActionResponse(this.userContext, result);
		}

		private ItemOperations.Result DeleteItem(StoreObjectId itemId, StoreObjectId folderId)
		{
			if (ObjectClass.IsMeetingRequest(this.itemType) || ObjectClass.IsMeetingCancellation(this.itemType))
			{
				MeetingUtilities.DeleteMeetingMessageCalendarItem(itemId);
			}
			return ItemOperations.DeleteItem(this.userContext, itemId, folderId);
		}

		private ItemOperations.Result RemoveFromCalendar(StoreObjectId itemId, StoreObjectId folderId)
		{
			if (!Utilities.IsDefaultFolderId(this.userContext.MailboxSession, folderId, DefaultFolderType.DeletedItems))
			{
				return this.DeleteItem(itemId, folderId);
			}
			if (ObjectClass.IsMeetingCancellation(this.itemType))
			{
				MeetingUtilities.DeleteMeetingMessageCalendarItem(itemId);
			}
			else
			{
				if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(this.itemType))
				{
					throw new OwaInvalidOperationException("RemoveFromCalendar can not handle this type of item");
				}
				MeetingUtilities.DeleteCalendarItem(itemId, DeleteItemFlags.MoveToDeletedItems);
			}
			return null;
		}

		private ItemOperations.Result CreateItemOperationsResultForDelete(StoreObjectId itemId, StoreObjectId folderId)
		{
			if (this.userContext.UserOptions.NextSelection != NextSelectionDirection.ReturnToView)
			{
				return ItemOperations.GetNextViewItem(this.userContext, ItemOperations.Action.Delete, itemId, folderId);
			}
			return null;
		}

		private const string FormMessageId = "hidid";

		private const string FormChangeKey = "hidchk";

		private const string FormResponse = "rdoRsp";

		private const string QueryStringFolderId = "fId";

		private const string FormFolderId = "hidfldid";

		private const string MessageId = "id";

		private const string PreviousItemAction = "Prev";

		private const string NextItemAction = "Next";

		private const string DeleteAction = "Del";

		private const string RemoveFromCalendarAction = "RemoveFromCal";

		private const string JunkAction = "Junk";

		private const string NotJunkAction = "NotJunk";

		private const string CloseAction = "Close";

		private const string AcceptAction = "Accept";

		private const string TentativeAction = "Tentative";

		private const string DeclineAction = "Decline";

		private const string DeclineDeleteAction = "DeclineDelete";

		private static readonly PropertyDefinition[] requestedProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private UserContext userContext;

		private OwaContext context;

		private PropertyDefinition[] properties;

		private ResponseType responseType;

		private string itemType;

		private MeetingPagePreFormAction.ResponseAction responseAction = MeetingPagePreFormAction.ResponseAction.NoResponse;

		public enum ResponseAction
		{
			SendResponse,
			EditResponse,
			NoResponse,
			SendResponseAndDelete
		}
	}
}
