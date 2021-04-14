using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("EditMeetingResponse")]
	[OwaEventSegmentation(Feature.Calendar)]
	internal sealed class EditMeetingResponseEventHandler : MessageEventHandler
	{
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, false)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Cc", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Bcc", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("IdM", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("Rsp", typeof(ResponseType), false, false)]
		[OwaEventParameter("PermanentDelete", typeof(bool), false, true)]
		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("HideMailTipsByDefault", typeof(bool), false, true)]
		[OwaEvent("Send")]
		public void Send()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingResponseEventHandler.SendEditResponse");
			MeetingResponse meetingResponse = base.GetRequestItem<MeetingResponse>(new PropertyDefinition[0]);
			try
			{
				if (base.UpdateMessage(meetingResponse, StoreObjectType.MeetingMessage))
				{
					throw new OwaEventHandlerException("Unresolved recipients", LocalizedStrings.GetNonEncoded(2063734279));
				}
				if (meetingResponse.Recipients.Count == 0)
				{
					throw new OwaEventHandlerException("No recipients", LocalizedStrings.GetNonEncoded(1878192149));
				}
				if (Utilities.RecipientsOnlyHaveEmptyPDL<Recipient>(base.UserContext, meetingResponse.Recipients))
				{
					base.RenderPartialFailure(1389137820);
				}
				else
				{
					this.HandleSendOnBehalf(meetingResponse);
					base.SaveHideMailTipsByDefault();
					ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Sending meeting response");
					meetingResponse.Send();
					OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("IdM");
					if (owaStoreObjectId != null)
					{
						object parameter = base.GetParameter("PermanentDelete");
						bool flag = parameter is bool && (bool)parameter;
						if (flag)
						{
							Utilities.Delete(base.UserContext, DeleteItemFlags.SoftDelete, new OwaStoreObjectId[]
							{
								owaStoreObjectId
							});
						}
						else
						{
							Utilities.Delete(base.UserContext, DeleteItemFlags.MoveToDeletedItems, new OwaStoreObjectId[]
							{
								owaStoreObjectId
							});
						}
					}
				}
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

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("Save")]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, false)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Cc", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Bcc", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("IdM", typeof(StoreObjectId), false, true)]
		[OwaEventParameter("Rsp", typeof(ResponseType), false, false)]
		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("HideMailTipsByDefault", typeof(bool), false, true)]
		public void SaveEditResponse()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingResponseEventHandler.SaveEditResponse");
			MeetingResponse meetingResponse = base.GetRequestItem<MeetingResponse>(new PropertyDefinition[0]);
			try
			{
				base.UpdateMessage(meetingResponse, StoreObjectType.MeetingMessage);
				this.HandleSendOnBehalf(meetingResponse);
				base.SaveHideMailTipsByDefault();
				Utilities.SaveItem(meetingResponse);
				meetingResponse.Load();
				base.WriteChangeKey(meetingResponse);
			}
			finally
			{
				meetingResponse.Dispose();
				meetingResponse = null;
			}
		}

		[OwaEventParameter("DeliveryRcpt", typeof(bool), false, true)]
		[OwaEventParameter("IdM", typeof(StoreObjectId), false, true)]
		[OwaEventParameter("ReadRcpt", typeof(bool), false, true)]
		[OwaEventParameter("UpdRcpAs", typeof(bool))]
		[OwaEvent("AutoSave")]
		[OwaEventParameter("Subj", typeof(string), false, true)]
		[OwaEventParameter("Imp", typeof(Importance), false, true)]
		[OwaEventParameter("Recips", typeof(RecipientInfoAC), true, true)]
		[OwaEventParameter("Text", typeof(bool), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		[OwaEventParameter("Rsp", typeof(ResponseType), false, false)]
		[OwaEventParameter("Sensitivity", typeof(Sensitivity), false, false)]
		[OwaEventParameter("Body", typeof(string), false, true)]
		[OwaEventParameter("To", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Cc", typeof(RecipientInfo), true, true)]
		[OwaEventParameter("Bcc", typeof(RecipientInfo), true, true)]
		public void AutoSaveEditResponse()
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "EditMeetingResponseEventHandler.AutoSaveEditResponse");
			MeetingResponse meetingResponse = null;
			try
			{
				meetingResponse = base.GetRequestItem<MeetingResponse>(new PropertyDefinition[0]);
				base.UpdateMessageForAutoSave(meetingResponse, StoreObjectType.MeetingMessage);
				this.HandleSendOnBehalf(meetingResponse);
				Utilities.SaveItem(meetingResponse, true);
				base.WriteIdAndChangeKey(meetingResponse, true);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.MailTracer.TraceError<string>((long)this.GetHashCode(), "EditMeetingResponseEventHandler.AutoSaveEditResponse - Exception {0} thrown during autosave", ex.Message);
				if (Utilities.ShouldSendChangeKeyForException(ex))
				{
					ExTraceGlobals.MailDataTracer.TraceDebug<string>((long)this.GetHashCode(), "EditMessageEventHandler.TryProcessMessageRequestForAutoSave attempt to send change key on exception: {0}", ex.Message);
					base.SaveIdAndChangeKeyInCustomErrorInfo(meetingResponse);
				}
				base.RenderErrorForAutoSave(ex);
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

		private void HandleSendOnBehalf(MeetingResponse meetingResponse)
		{
			if (meetingResponse.Sender != null && string.CompareOrdinal(base.UserContext.ExchangePrincipal.LegacyDn, meetingResponse.Sender.EmailAddress) != 0)
			{
				meetingResponse.From = meetingResponse.Sender;
			}
		}

		public const string EventNamespace = "EditMeetingResponse";

		public const string MethodSend = "Send";

		public const string MeetingRequestID = "IdM";

		public const string Response = "Rsp";

		public const string PermanentDeleteInvite = "PermanentDelete";
	}
}
