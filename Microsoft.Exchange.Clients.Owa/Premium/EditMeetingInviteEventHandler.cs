using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.Calendar)]
	[OwaEventNamespace("EditMeetingInvite")]
	internal sealed class EditMeetingInviteEventHandler : ItemEventHandler
	{
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		[OwaEventParameter("idci", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("sn", typeof(int), false, true)]
		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEventParameter("Fbs", typeof(BusyType), false, true)]
		[OwaEvent("SaveMeetingInvite")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Subj", typeof(string))]
		public void SaveMeetingInvite()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.SaveMeetingInvite");
			MeetingRequest meetingRequest = null;
			try
			{
				meetingRequest = this.GetMeetingRequest(new PropertyDefinition[0]);
				this.UpdateItem(meetingRequest);
				Utilities.SaveItem(meetingRequest);
				meetingRequest.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(meetingRequest.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
			finally
			{
				if (meetingRequest != null)
				{
					meetingRequest.Dispose();
					meetingRequest = null;
				}
			}
		}

		[OwaEventParameter("Fbs", typeof(BusyType), false, true)]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEvent("SaveCalendarItem")]
		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("Subj", typeof(string))]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		public void SaveCalendarItem()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.SaveCalendarItem");
			CalendarItemBase calendarItemBase = null;
			try
			{
				calendarItemBase = base.GetRequestItem<CalendarItemBase>(new PropertyDefinition[0]);
				this.UpdateItem(calendarItemBase);
				Utilities.SaveItem(calendarItemBase);
				calendarItemBase.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(calendarItemBase.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("Subj", typeof(string))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEvent("SaveMeetingCancel")]
		public void SaveMeetingCancel()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.SaveMeetingCancel");
			MeetingCancellation meetingCancellation = null;
			try
			{
				meetingCancellation = base.GetRequestItem<MeetingCancellation>(new PropertyDefinition[0]);
				meetingCancellation[ItemSchema.Subject] = (string)base.GetParameter("Subj");
				Utilities.SaveItem(meetingCancellation);
				meetingCancellation.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(meetingCancellation.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
			finally
			{
				if (meetingCancellation != null)
				{
					meetingCancellation.Dispose();
					meetingCancellation = null;
				}
			}
		}

		[OwaEvent("SaveMeetingResponse")]
		[OwaEventParameter("Subj", typeof(string))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		public void SaveMeetingResponse()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.SaveMeetingResponse");
			MeetingResponse meetingResponse = null;
			try
			{
				meetingResponse = base.GetRequestItem<MeetingResponse>(new PropertyDefinition[0]);
				meetingResponse[ItemSchema.Subject] = (string)base.GetParameter("Subj");
				Utilities.SaveItem(meetingResponse);
				meetingResponse.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(meetingResponse.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
			finally
			{
				if (meetingResponse != null)
				{
					meetingResponse.Dispose();
					meetingResponse = null;
				}
			}
		}

		[OwaEventParameter("idci", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		[OwaEventParameter("sn", typeof(int), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEvent("EditResponseInvite")]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		public void EditResponseInvite()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.EditResponseInvite");
			ResponseType responseType = (ResponseType)base.GetParameter("Rsp");
			this.properties = new PropertyDefinition[]
			{
				MeetingMessageSchema.CalendarProcessed,
				StoreObjectSchema.ParentItemId
			};
			CalendarItemBase calendarItemBase = null;
			MeetingRequest meetingRequest = null;
			try
			{
				meetingRequest = this.GetMeetingRequest(this.properties);
				calendarItemBase = MeetingUtilities.UpdateCalendarItem(meetingRequest);
				if (calendarItemBase == null)
				{
					throw new OwaInvalidRequestException(string.Format("calendarItemBase associated with meeting request with Id {0} is null.", base.GetParameter("Id")));
				}
				this.EditResponseInternal(responseType, calendarItemBase);
				this.UpdateItem(meetingRequest);
				Utilities.SaveItem(meetingRequest);
				meetingRequest.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(meetingRequest.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
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
		}

		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("Subj", typeof(string))]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		[OwaEvent("EditResponseCalendarItem")]
		public void EditResponseCalendarItem()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.EditResponseCalendarItem");
			ResponseType responseType = (ResponseType)base.GetParameter("Rsp");
			CalendarItemBase calendarItemBase = null;
			try
			{
				calendarItemBase = base.GetRequestItem<CalendarItemBase>(new PropertyDefinition[0]);
				if (calendarItemBase != null)
				{
					this.EditResponseInternal(responseType, calendarItemBase);
					calendarItemBase.Load();
					this.Writer.Write("<div id=nid>");
					if (calendarItemBase.Id != null && calendarItemBase.Id.ObjectId != null)
					{
						this.Writer.Write(OwaStoreObjectId.CreateFromStoreObject(calendarItemBase).ToBase64String());
					}
					this.Writer.Write("</div>");
				}
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
					calendarItemBase = null;
				}
			}
		}

		[OwaEvent("EditDeclineResponseCalendarItem")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		public void EditDeclineResponseCalendarItem()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.EditDeclineResponseCalendarItem");
			using (CalendarItemBase requestItem = base.GetRequestItem<CalendarItemBase>(new PropertyDefinition[0]))
			{
				this.EditResponseInternal(ResponseType.Decline, requestItem, false);
			}
		}

		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		[OwaEventParameter("sn", typeof(int), false, true)]
		[OwaEventParameter("idci", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("SendResponse")]
		public void SendResponse()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.SendResponse");
			this.NonEditResponseInternal(true);
		}

		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("sn", typeof(int), false, true)]
		[OwaEventParameter("Prvt", typeof(bool), false, true)]
		[OwaEventParameter("RemS", typeof(bool), false, true)]
		[OwaEventParameter("RemT", typeof(int), false, true)]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		[OwaEventParameter("idci", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("NoResponse")]
		public void NoResponse()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.NoResponse");
			this.NonEditResponseInternal(false);
		}

		private void NonEditResponseInternal(bool sendResponse)
		{
			ResponseType responseType = (ResponseType)base.GetParameter("Rsp");
			StoreObjectType storeObjectType = (StoreObjectType)base.GetParameter("ItemType");
			MeetingRequest meetingRequest = null;
			CalendarItemBase calendarItemBase = null;
			try
			{
				StoreObjectType storeObjectType2 = storeObjectType;
				if (storeObjectType2 != StoreObjectType.MeetingRequest)
				{
					if (storeObjectType2 == StoreObjectType.CalendarItem)
					{
						calendarItemBase = base.GetRequestItem<CalendarItemBase>(new PropertyDefinition[0]);
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
					meetingRequest = this.GetMeetingRequest(this.properties);
					calendarItemBase = MeetingUtilities.UpdateCalendarItem(meetingRequest);
					if (calendarItemBase == null)
					{
						throw new OwaInvalidRequestException(string.Format("calendarItem associated with meetingRequest with Id {0} is null.", base.GetParameter("Id")));
					}
				}
				this.UpdateItem(calendarItemBase);
				Utilities.SaveItem(calendarItemBase);
				calendarItemBase.Load();
				MeetingUtilities.NonEditResponse(responseType, calendarItemBase, sendResponse, null);
				calendarItemBase.Load();
				if (meetingRequest != null)
				{
					this.UpdateItem(meetingRequest);
					Utilities.SaveItem(meetingRequest);
					MeetingUtilities.DeleteMeetingRequestAfterResponse(meetingRequest);
				}
				if (storeObjectType == StoreObjectType.CalendarItem)
				{
					this.Writer.Write("<div id=nid>");
					this.Writer.Write(OwaStoreObjectId.CreateFromStoreObject(calendarItemBase).ToBase64String());
					this.Writer.Write("</div>");
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
		}

		private void EditResponseInternal(ResponseType responseType, CalendarItemBase calendarItemBase)
		{
			this.EditResponseInternal(responseType, calendarItemBase, true);
		}

		private void EditResponseInternal(ResponseType responseType, CalendarItemBase calendarItemBase, bool doCalendarItemUpdate)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.EditResponseInternal");
			MeetingUtilities.ThrowIfMeetingResponseInvalid(calendarItemBase);
			if (doCalendarItemUpdate)
			{
				this.UpdateItem(calendarItemBase);
				Utilities.SaveItem(calendarItemBase);
				calendarItemBase.Load();
			}
			using (MeetingResponse meetingResponse = MeetingUtilities.EditResponse(responseType, calendarItemBase))
			{
				meetingResponse.Load();
				this.Writer.Write("<div id=divOp _sOp=mr>");
				this.Writer.Write(OwaStoreObjectId.CreateFromStoreObject(meetingResponse).ToBase64String());
				this.Writer.Write("</div>");
			}
		}

		private void UpdateItem(Item item)
		{
			if (base.IsParameterSet("Subj"))
			{
				item[ItemSchema.Subject] = (string)base.GetParameter("Subj");
			}
			if (base.IsParameterSet("RemS"))
			{
				bool flag = (bool)base.GetParameter("RemS");
				item[ItemSchema.ReminderIsSet] = flag;
				if (flag && base.IsParameterSet("RemT"))
				{
					int num = (int)base.GetParameter("RemT");
					if (num < 0)
					{
						throw new OwaInvalidRequestException("Reminder minutes before start cannot be a negative value");
					}
					item[ItemSchema.ReminderMinutesBeforeStart] = num;
				}
			}
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			MeetingRequest meetingRequest = item as MeetingRequest;
			if (base.IsParameterSet("Prvt") && (meetingRequest != null || (calendarItemBase != null && (calendarItemBase.CalendarItemType == CalendarItemType.Single || calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster))))
			{
				bool flag2 = (bool)base.GetParameter("Prvt");
				if (flag2)
				{
					item[ItemSchema.Sensitivity] = Sensitivity.Private;
				}
				else
				{
					object obj = item.TryGetProperty(ItemSchema.Sensitivity);
					if (obj is Sensitivity && (Sensitivity)obj == Sensitivity.Private)
					{
						item[ItemSchema.Sensitivity] = Sensitivity.Normal;
					}
				}
			}
			if (base.IsParameterSet("Fbs"))
			{
				object parameter = base.GetParameter("Fbs");
				if (parameter != null)
				{
					item[CalendarItemBaseSchema.FreeBusyStatus] = (BusyType)parameter;
				}
			}
			if (base.IsParameterSet("Rsp"))
			{
				item[CalendarItemBaseSchema.ResponseType] = (ResponseType)base.GetParameter("Rsp");
			}
			if (meetingRequest != null)
			{
				((MessageItem)item).IsRead = true;
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("RemCal")]
		public void RemoveFromCalendar()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.RemoveFromCalendar");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			MeetingUtilities.DeleteMeetingMessageCalendarItem(owaStoreObjectId.StoreObjectId);
			base.Delete();
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEvent("DeleteAttendeeMeeting")]
		public void DeleteAttendeeMeeting()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.DeleteAttendeeMeeting");
			using (CalendarItemBase requestItem = base.GetRequestItem<CalendarItemBase>(new PropertyDefinition[0]))
			{
				if (!requestItem.IsOrganizer() && !MeetingUtilities.IsCalendarItemEndTimeInPast(requestItem))
				{
					base.ResponseContentType = OwaEventContentType.Html;
					this.Writer.Write("<div id=divOp _sOp=sr></div>");
				}
				else
				{
					base.Delete();
				}
			}
		}

		[OwaEvent("Delete")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		public override void Delete()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.Delete");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			StoreObjectType storeObjectType = (StoreObjectType)base.GetParameter("ItemType");
			if (storeObjectType == StoreObjectType.MeetingRequest || storeObjectType == StoreObjectType.MeetingCancellation)
			{
				MeetingUtilities.DeleteMeetingMessageCalendarItem(owaStoreObjectId.StoreObjectId);
			}
			else if (storeObjectType == StoreObjectType.CalendarItem || storeObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				MeetingUtilities.DeleteCalendarItem(owaStoreObjectId.StoreObjectId, DeleteItemFlags.MoveToDeletedItems);
				return;
			}
			base.Delete();
		}

		[OwaEvent("PermanentDelete")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("ItemType", typeof(StoreObjectType))]
		public override void PermanentDelete()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingInviteEventHandler.PermanentDelete");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			StoreObjectType storeObjectType = (StoreObjectType)base.GetParameter("ItemType");
			if (!owaStoreObjectId.IsPublic && (storeObjectType == StoreObjectType.MeetingRequest || storeObjectType == StoreObjectType.MeetingCancellation))
			{
				MeetingUtilities.DeleteMeetingMessageCalendarItem(owaStoreObjectId.StoreObjectId);
			}
			else if (storeObjectType == StoreObjectType.CalendarItem || storeObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				MeetingUtilities.DeleteCalendarItem(owaStoreObjectId.StoreObjectId, DeleteItemFlags.SoftDelete);
				return;
			}
			base.PermanentDelete();
		}

		private MeetingRequest GetMeetingRequest(params PropertyDefinition[] prefetchProperties)
		{
			MeetingRequest result = null;
			try
			{
				result = base.GetRequestItem<MeetingRequest>(prefetchProperties);
			}
			catch (ObjectNotFoundException innerException)
			{
				if (!base.IsParameterSet("idci"))
				{
					throw;
				}
				OwaStoreObjectId itemId = (OwaStoreObjectId)base.GetParameter("idci");
				int num;
				int num2;
				using (CalendarItemBase readOnlyRequestItem = base.GetReadOnlyRequestItem<CalendarItemBase>(itemId, new PropertyDefinition[0]))
				{
					num = (int)base.GetParameter("sn");
					num2 = (int)readOnlyRequestItem[CalendarItemBaseSchema.AppointmentSequenceNumber];
				}
				if (num < num2)
				{
					throw new OwaEventHandlerException("Meeting request must be out of date", LocalizedStrings.GetNonEncoded(2031473992), innerException);
				}
				throw;
			}
			return result;
		}

		public const string EventNamespace = "EditMeetingInvite";

		public const string MethodSaveMeetingInvite = "SaveMeetingInvite";

		public const string MethodSaveCalendarItem = "SaveCalendarItem";

		public const string MethodSaveMeetingCancel = "SaveMeetingCancel";

		public const string MethodSaveMeetingResponse = "SaveMeetingResponse";

		public const string MethodEditResponseInvite = "EditResponseInvite";

		public const string MethodEditResponseCalendarItem = "EditResponseCalendarItem";

		public const string MethodEditDeclineResponseCalendarItem = "EditDeclineResponseCalendarItem";

		public const string MethodDeleteAttendeeMeeting = "DeleteAttendeeMeeting";

		public const string MethodSendResponse = "SendResponse";

		public const string MethodNoResponse = "NoResponse";

		public const string MethodRemoveFromCalendar = "RemCal";

		public const string Response = "Rsp";

		public const string Subject = "Subj";

		public const string Importance = "Imp";

		public const string Private = "Prvt";

		public const string ReminderSet = "RemS";

		public const string ReminderTime = "RemT";

		public const string FreeBusyStatus = "Fbs";

		public const string Type = "ItemType";

		public const string SequenceNumber = "sn";

		public const string CalendarItemId = "idci";

		private PropertyDefinition[] properties;

		private enum ResponseAction
		{
			EditResponse,
			SendResponse,
			NoResponse
		}
	}
}
