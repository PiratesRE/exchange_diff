using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class SendMailCommand : SendMailBase
	{
		public SendMailCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSendMails;
		}

		protected override string RootNodeName
		{
			get
			{
				return "SendMail";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			MessageItem messageItem = null;
			bool flag = false;
			DeleteItemFlags deletedItemFlag = DeleteItemFlags.HardDelete;
			try
			{
				base.ValidateBody();
				StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				messageItem = MessageItem.Create(base.MailboxSession, defaultFolderId);
				base.ParseMimeToMessage(messageItem);
				Guid guid;
				if (base.IsIrmOperation(out guid))
				{
					RmsTemplate rmsTemplate = RmsTemplateReaderCache.LookupRmsTemplate(base.User.OrganizationId, guid);
					if (rmsTemplate == null)
					{
						AirSyncDiagnostics.TraceError<Guid>(ExTraceGlobals.RequestsTracer, this, "Template {0} not found in cache", guid);
						throw new AirSyncPermanentException(StatusCode.IRM_InvalidTemplateID, false)
						{
							ErrorStringForProtocolLogger = "smcEInvalidTemplateID"
						};
					}
					messageItem = RightsManagedMessageItem.Create(messageItem, AirSyncUtility.GetOutboundConversionOptions());
					RightsManagedMessageItem rightsManagedMessageItem = messageItem as RightsManagedMessageItem;
					rightsManagedMessageItem.SetRestriction(rmsTemplate);
					rightsManagedMessageItem.Sender = new Participant(base.MailboxSession.MailboxOwner);
				}
				Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.ClassName, messageItem.ClassName);
				if (!GlobalSettings.OnlyOrganizersCanSendMeetingChanges || !this.IsMeetingRelatedMessage(messageItem))
				{
					base.SendMessage(messageItem);
					flag = true;
				}
				else
				{
					bool flag2 = this.CanSendMessage(messageItem);
					if (GlobalSettings.GetGoidFromCalendarItemForMeetingResponse && base.Version < 160 && this.IsResponseMessageToSingleMeeting(messageItem))
					{
						GlobalObjectId goidFromItem = this.GetGoidFromItem(messageItem);
						try
						{
							ResponseType valueOrDefault = messageItem.GetValueOrDefault<ResponseType>(MeetingResponseSchema.ResponseType);
							DefaultFolderType defaultFolderType = (valueOrDefault == ResponseType.Decline) ? DefaultFolderType.DeletedItems : DefaultFolderType.Calendar;
							List<PropertyBag> list = this.QueryRelatedCalendarItems(defaultFolderType, goidFromItem);
							if (list.Count == 0 && valueOrDefault == ResponseType.Decline)
							{
								list = this.QueryRelatedCalendarItems(DefaultFolderType.Calendar, goidFromItem);
								if (list.Count == 0)
								{
									list = this.QueryRelatedCalendarItems(DefaultFolderType.DeletedItems, goidFromItem);
								}
							}
							this.FindAndSetMessageGoid(list, messageItem, goidFromItem);
						}
						catch (Exception ex)
						{
							AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, this, "GetRelatedCalendarItemGoid threw exception {0}", ex);
							AirSyncDiagnostics.SendInMemoryTraceWatson(ex);
						}
					}
					if (flag2)
					{
						base.SendMessage(messageItem);
						flag = true;
					}
					else
					{
						AirSyncDiagnostics.TraceDebug<Participant>(ExTraceGlobals.RequestsTracer, this, "Attempt to send meeting cancellation by attendee. Sender {0}", messageItem.Sender);
						messageItem.ClassName = "IPM.Note";
						messageItem.Save(SaveMode.NoConflictResolution);
						deletedItemFlag = DeleteItemFlags.MoveToDeletedItems;
						Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.SkipSend, true.ToString());
					}
				}
			}
			finally
			{
				if (messageItem != null)
				{
					if (!flag)
					{
						base.DeleteMessage(messageItem, deletedItemFlag);
					}
					messageItem.Dispose();
				}
			}
			return Command.ExecutionState.Complete;
		}

		private List<PropertyBag> QueryRelatedCalendarItems(DefaultFolderType defaultFolderType, GlobalObjectId goid)
		{
			List<PropertyBag> calendarItems = new List<PropertyBag>();
			using (Folder folder = Folder.Bind(base.MailboxSession, defaultFolderType))
			{
				CalendarCorrelationMatch.QueryRelatedItems(folder, goid, null, SendMailCommand.CalendarItemPropertySet, false, delegate(PropertyBag match)
				{
					calendarItems.Add(match);
					return true;
				}, false, false, SendMailCommand.CalendarItemClassFilter, null, null);
			}
			AirSyncDiagnostics.TraceDebug<int, DefaultFolderType>(ExTraceGlobals.RequestsTracer, this, "Found {0} related calendar items in folder {0}", calendarItems.Count, defaultFolderType);
			return calendarItems;
		}

		internal bool FindAndSetMessageGoid(List<PropertyBag> calendarItems, MessageItem message, GlobalObjectId goid)
		{
			if (calendarItems.Count == 0)
			{
				AirSyncDiagnostics.TraceDebug<GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "Found no related calendar items using message GOID {0}", goid);
				return false;
			}
			bool flag = false;
			ExDateTime valueOrDefault = message.GetValueOrDefault<ExDateTime>(MeetingMessageInstanceSchema.MapiStartTime);
			foreach (PropertyBag propertyBag in calendarItems)
			{
				if (propertyBag.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.StartTime).Date == valueOrDefault.Date)
				{
					byte[] array = propertyBag.TryGetProperty(CalendarItemBaseSchema.GlobalObjectId) as byte[];
					byte[] array2 = propertyBag.TryGetProperty(CalendarItemBaseSchema.CleanGlobalObjectId) as byte[];
					if (array != null && array2 != null)
					{
						if (!array.SequenceEqual(array2))
						{
							GlobalObjectId arg = new GlobalObjectId(array);
							AirSyncDiagnostics.TraceDebug<GlobalObjectId, GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "Found an orphan calendar item, replacing message GOID {0} with calendar GOID {1}", goid, arg);
							message[MeetingMessageInstanceSchema.GlobalObjectId] = array;
							return true;
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				AirSyncDiagnostics.TraceDebug<GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "This is a response to a non-recurring meeting. Keep the original message GOID {0}", goid);
			}
			else
			{
				AirSyncDiagnostics.TraceDebug<ExDateTime, GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "Found no calendar items matching start date {0} and message GOID {1}", valueOrDefault.Date, goid);
			}
			return false;
		}

		private bool CanSendMessage(MessageItem message)
		{
			bool flag = ObjectClass.IsMeetingRequest(message.ClassName);
			bool flag2 = ObjectClass.IsMeetingCancellation(message.ClassName);
			if (!flag2 && !flag)
			{
				AirSyncDiagnostics.TraceInfo<VersionedId, string>(ExTraceGlobals.RequestsTracer, this, "Current Message is not a meeting request or cancellation. messageId: {0}, ClassName: {1}", message.Id, message.ClassName);
				return true;
			}
			AirSyncDiagnostics.TraceDebug<bool, bool>(ExTraceGlobals.RequestsTracer, this, "Checking if allowed to send message. isMeetingRequest: {0}, isMeetingCancellation: {1}", flag, flag2);
			GlobalObjectId globalObjectId = null;
			try
			{
				globalObjectId = new GlobalObjectId(message);
			}
			catch (CorruptDataException)
			{
				Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("CalItemNoGOID+MeetingCancellation:{0}+Discarded", flag2));
				return false;
			}
			MailboxSession mailboxSession = (MailboxSession)message.Session;
			Command.CurrentCommand.LoadMeetingOrganizerSyncState();
			MeetingOrganizerEntry entry = Command.CurrentCommand.MeetingOrganizerSyncState.MeetingOrganizerInfo.GetEntry(globalObjectId);
			bool flag3 = entry.IsOrganizer != null;
			Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("OrganizerCheck:{0}", flag3 ? entry.IsOrganizer.Value.ToString() : "<UNKNOWN>"));
			bool flag4 = flag2 ? (flag3 && entry.IsOrganizer.Value) : (!flag3 || entry.IsOrganizer.Value);
			if (!flag4)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(flag2 ? (globalObjectId.IsCleanGlobalObjectId ? "MeetingCancellationDiscarded+" : "MeetingInstanceCancellationDiscarded+") : "MeetingUpdateDiscarded+");
				string subject = entry.Subject;
				stringBuilder.AppendFormat("Organizer:{0}+Subject:'{1}'+", entry.Organizer ?? "<NULL>", subject ?? "<NULL>");
				Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, stringBuilder.ToString());
			}
			return flag4;
		}

		internal GlobalObjectId GetGoidFromItem(Item item)
		{
			GlobalObjectId result;
			try
			{
				result = new GlobalObjectId(item);
			}
			catch (CorruptDataException)
			{
				result = null;
			}
			return result;
		}

		internal bool IsMeetingRelatedMessage(IMessageItem message)
		{
			string className = message.ClassName;
			return ObjectClass.IsMeetingRequest(className) || ObjectClass.IsMeetingCancellation(className) || ObjectClass.IsMeetingResponse(className);
		}

		internal bool IsResponseMessageToSingleMeeting(IMessageItem message)
		{
			string className = message.ClassName;
			if (ObjectClass.IsMeetingResponse(className))
			{
				bool flag = InternalRecurrence.HasRecurrenceBlob(message.CoreItem.PropertyBag);
				return !flag;
			}
			return false;
		}

		private static readonly PropertyDefinition[] CalendarItemPropertySet = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.GlobalObjectId,
			CalendarItemBaseSchema.CleanGlobalObjectId,
			CalendarItemInstanceSchema.StartTime
		};

		private static readonly string[] CalendarItemClassFilter = new string[]
		{
			"IPM.Appointment"
		};
	}
}
