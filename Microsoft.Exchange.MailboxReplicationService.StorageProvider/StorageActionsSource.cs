using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class StorageActionsSource : IActionsSource
	{
		internal StorageActionsSource(MailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			this.mailboxSession = mailboxSession;
		}

		IEnumerable<ReplayAction> IActionsSource.ReadActions(IActionWatermark watermark)
		{
			MrsTracer.Provider.Function("StorageActionsSource.ReadActions", new object[0]);
			IActivityLog activityLog = ActivityLogFactory.Current.Bind(this.mailboxSession);
			List<ReplayAction> actionsIndescendingOrder = new List<ReplayAction>(100);
			foreach (Activity activity in activityLog.Query())
			{
				if (activity.ClientId.LoggedViaServerSideInstrumentation)
				{
					IActionWatermark actionWatermark = new StorageActionWatermark(activity);
					if (watermark != null && watermark.CompareTo(actionWatermark) >= 0)
					{
						break;
					}
					ReplayAction replayAction = this.TryConvertToAction(activity, actionWatermark);
					if (replayAction != null)
					{
						actionsIndescendingOrder.Add(replayAction);
					}
				}
			}
			for (int index = actionsIndescendingOrder.Count - 1; index >= 0; index--)
			{
				yield return actionsIndescendingOrder[index];
			}
			yield break;
		}

		private static byte[] GetItemAndFolderId(StoreObjectId itemId, out byte[] folderId)
		{
			byte[] providerLevelItemId = itemId.ProviderLevelItemId;
			folderId = IdConverter.GetParentEntryIdFromMessageEntryId(providerLevelItemId);
			return providerLevelItemId;
		}

		private ReplayAction TryConvertToAction(Activity activity, IActionWatermark watermark)
		{
			ActivityId id = activity.Id;
			switch (id)
			{
			case ActivityId.Move:
				if (IdConverter.IsMessageId(activity.ItemId))
				{
					byte[] array;
					byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
					byte[] prevFolderId;
					StorageActionsSource.GetItemAndFolderId(activity.PreviousItemId, out prevFolderId);
					DefaultFolderType defaultFolderType = this.mailboxSession.IsDefaultFolderType(StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Folder));
					if (defaultFolderType == DefaultFolderType.RecoverableItemsDeletions || defaultFolderType == DefaultFolderType.RecoverableItemsPurges)
					{
						return new DeleteAction(itemAndFolderId, array, prevFolderId, watermark.SerializeToString());
					}
					return new MoveAction(itemAndFolderId, array, prevFolderId, watermark.SerializeToString());
				}
				break;
			case ActivityId.Flag:
			{
				byte[] array;
				byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
				return new FlagAction(itemAndFolderId, array, watermark.SerializeToString());
			}
			case ActivityId.FlagComplete:
			{
				byte[] array;
				byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
				return new FlagCompleteAction(itemAndFolderId, array, watermark.SerializeToString());
			}
			case ActivityId.FlagCleared:
			{
				byte[] array;
				byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
				return new FlagClearAction(itemAndFolderId, array, watermark.SerializeToString());
			}
			case ActivityId.Categorize:
			case ActivityId.InspectorDisplayStart:
			case ActivityId.InspectorDisplayEnd:
				break;
			case ActivityId.MarkAsRead:
			{
				byte[] array;
				byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
				return new MarkAsReadAction(itemAndFolderId, array, watermark.SerializeToString());
			}
			case ActivityId.MarkAsUnread:
			{
				byte[] array;
				byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
				return new MarkAsUnReadAction(itemAndFolderId, array, watermark.SerializeToString());
			}
			default:
				switch (id)
				{
				case ActivityId.RemoteSend:
				{
					string[] recipients;
					byte[] mimeData = this.GetMimeData(activity.ItemId, out recipients);
					byte[] array;
					byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
					return new SendAction(itemAndFolderId, array, watermark.SerializeToString())
					{
						Data = mimeData,
						Recipients = recipients
					};
				}
				case ActivityId.CreateCalendarEvent:
				{
					byte[] array;
					byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
					IList<Event> exceptionalOccurrences;
					IList<string> deletedOccurrences;
					Event calendarEventData = this.GetCalendarEventData(activity, out exceptionalOccurrences, out deletedOccurrences);
					if (calendarEventData == null)
					{
						return null;
					}
					return new CreateCalendarEventAction(itemAndFolderId, array, watermark.SerializeToString(), calendarEventData, exceptionalOccurrences, deletedOccurrences);
				}
				case ActivityId.UpdateCalendarEvent:
				{
					byte[] array;
					byte[] itemAndFolderId = StorageActionsSource.GetItemAndFolderId(activity.ItemId, out array);
					IList<Event> exceptionalOccurrences2;
					IList<string> deletedOccurrences2;
					Event calendarEventData2 = this.GetCalendarEventData(activity, out exceptionalOccurrences2, out deletedOccurrences2);
					if (calendarEventData2 == null)
					{
						return null;
					}
					return new UpdateCalendarEventAction(itemAndFolderId, array, watermark.SerializeToString(), calendarEventData2, exceptionalOccurrences2, deletedOccurrences2);
				}
				}
				break;
			}
			return null;
		}

		private Event GetCalendarEventData(Activity activity, out IList<Event> exceptionalOccurrences, out IList<string> deletedOccurrences)
		{
			exceptionalOccurrences = null;
			deletedOccurrences = null;
			Event result;
			try
			{
				StoreObjectId storeObjectId = activity.ItemId;
				OccurrenceStoreObjectId occurrenceStoreObjectId = storeObjectId as OccurrenceStoreObjectId;
				if (occurrenceStoreObjectId != null)
				{
					storeObjectId = occurrenceStoreObjectId.GetMasterStoreObjectId();
				}
				string text = IdConverter.Instance.ToStringId(storeObjectId, this.mailboxSession);
				EventReference eventReference = new EventReference(XSOFactory.Default, this.mailboxSession, text);
				IEvents events = eventReference.Events;
				Event @event = events.Read(text, null);
				if (@event.PatternedRecurrence != null)
				{
					ExpandedEvent expandedEvent = events.Expand(@event.Id, new ExpandEventParameters
					{
						ReturnExceptions = true,
						ReturnCancellations = true
					}, null);
					deletedOccurrences = expandedEvent.CancelledOccurrences;
					exceptionalOccurrences = this.FilterOutNonExceptionalProperties(expandedEvent.Occurrences);
				}
				result = @event;
			}
			catch (ObjectNotFoundException)
			{
				MrsTracer.Provider.Warning("StorageActionsSource.GetCalendarEventData unable to locate the event to replay.", new object[0]);
				result = null;
			}
			return result;
		}

		private IList<Event> FilterOutNonExceptionalProperties(IList<Event> exceptions)
		{
			if (exceptions == null)
			{
				return null;
			}
			List<Event> list = new List<Event>(exceptions.Count);
			foreach (Event exception in exceptions)
			{
				list.Add(this.GetExceptionDelta(exception));
			}
			return list;
		}

		private Event GetExceptionDelta(Event exception)
		{
			Event @event = new Event();
			EventSchema schema = exception.Schema;
			@event.Id = exception.Id;
			@event.Start = exception.Start;
			@event.End = exception.End;
			@event.LastModifiedTime = exception.LastModifiedTime;
			foreach (string a in exception.ExceptionalProperties)
			{
				if (a == schema.BodyProperty.Name)
				{
					@event.Body = exception.Body;
				}
				else if (a == schema.IsAllDayProperty.Name)
				{
					@event.IsAllDay = exception.IsAllDay;
				}
				else if (a == schema.ShowAsProperty.Name)
				{
					@event.ShowAs = exception.ShowAs;
				}
				else if (a == schema.LocationProperty.Name)
				{
					@event.Location = exception.Location;
				}
				else if (a == schema.PopupReminderSettingsProperty.Name)
				{
					@event.PopupReminderSettings = exception.PopupReminderSettings;
				}
				else if (a == schema.SensitivityProperty.Name)
				{
					@event.Sensitivity = exception.Sensitivity;
				}
				else if (a == schema.SubjectProperty.Name)
				{
					@event.Subject = exception.Subject;
				}
				else if (a == schema.AttendeesProperty.Name)
				{
					@event.Attendees = exception.Attendees;
				}
				else if (a == schema.CalendarProperty.Name)
				{
					@event.Categories = exception.Categories;
				}
			}
			return @event;
		}

		private byte[] GetMimeData(StoreObjectId itemId, out string[] recipients)
		{
			recipients = null;
			byte[] result;
			using (MessageItem messageItem = MessageItem.Bind(this.mailboxSession, itemId, StoreObjectSchema.ContentConversionProperties))
			{
				List<string> list = new List<string>(messageItem.Recipients.Count);
				foreach (RecipientBase recipientBase in messageItem.Recipients)
				{
					Participant participant = recipientBase.Participant;
					if (participant != null)
					{
						string valueOrDefault = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
						list.Add(valueOrDefault);
					}
				}
				OutboundConversionOptions options = new OutboundConversionOptions(this.mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), this.mailboxSession.ServerFullyQualifiedDomainName)
				{
					IsSenderTrusted = true,
					RecipientCache = null,
					ClearCategories = true,
					Limits = 
					{
						MimeLimits = MimeLimits.Unlimited
					},
					AllowPartialStnefConversion = true
				};
				using (PooledMemoryStream pooledMemoryStream = new PooledMemoryStream(6))
				{
					ItemConversion.ConvertItemToMime(messageItem, pooledMemoryStream, options);
					recipients = list.ToArray();
					result = pooledMemoryStream.ToArray();
				}
			}
			return result;
		}

		private readonly MailboxSession mailboxSession;
	}
}
