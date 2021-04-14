using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class OccurrenceInfoProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private OccurrenceInfoProperty(CommandContext commandContext, OccurrenceInfoProperty.PropertyCommandKind propertyType) : base(commandContext)
		{
			this.propertyCommandKind = propertyType;
		}

		public static OccurrenceInfoProperty CreateCommandForFirstOccurrence(CommandContext commandContext)
		{
			return new OccurrenceInfoProperty(commandContext, OccurrenceInfoProperty.PropertyCommandKind.FirstOccurrence);
		}

		public static OccurrenceInfoProperty CreateCommandForLastOccurrence(CommandContext commandContext)
		{
			return new OccurrenceInfoProperty(commandContext, OccurrenceInfoProperty.PropertyCommandKind.LastOccurrence);
		}

		public static OccurrenceInfoProperty CreateCommandForModifiedOccurrences(CommandContext commandContext)
		{
			return new OccurrenceInfoProperty(commandContext, OccurrenceInfoProperty.PropertyCommandKind.ModifiedOccurrences);
		}

		public static OccurrenceInfoProperty CreateCommandForDeletedOccurrences(CommandContext commandContext)
		{
			return new OccurrenceInfoProperty(commandContext, OccurrenceInfoProperty.PropertyCommandKind.DeletedOccurrences);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("OccurrenceInfoProperty.ToXml should not be called");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			StoreObject storeObject = commandSettings.StoreObject;
			MeetingRequest meetingRequest = storeObject as MeetingRequest;
			if (meetingRequest != null)
			{
				CalendarItem calendarItem = meetingRequest.GetCachedEmbeddedItem() as CalendarItem;
				this.Render(serviceObject, idAndSession, calendarItem);
				return;
			}
			this.Render(serviceObject, idAndSession, storeObject as CalendarItem);
		}

		private void Render(ServiceObject serviceObject, IdAndSession storeIdAndSession, CalendarItem calendarItem)
		{
			if (calendarItem == null)
			{
				return;
			}
			if (calendarItem.Recurrence == null)
			{
				return;
			}
			switch (this.propertyCommandKind)
			{
			case OccurrenceInfoProperty.PropertyCommandKind.FirstOccurrence:
				serviceObject.PropertyBag[CalendarItemSchema.FirstOccurrence] = this.RenderOccurrenceInfo(storeIdAndSession, (calendarItem.Recurrence != null) ? calendarItem.Recurrence.GetFirstOccurrence() : null);
				return;
			case OccurrenceInfoProperty.PropertyCommandKind.LastOccurrence:
				if (calendarItem.Recurrence.Range is EndDateRecurrenceRange || calendarItem.Recurrence.Range is NumberedRecurrenceRange)
				{
					serviceObject.PropertyBag[CalendarItemSchema.LastOccurrence] = this.RenderOccurrenceInfo(storeIdAndSession, calendarItem.Recurrence.GetLastOccurrence());
					return;
				}
				break;
			case OccurrenceInfoProperty.PropertyCommandKind.ModifiedOccurrences:
				if (calendarItem.Recurrence != null)
				{
					IList<OccurrenceInfo> modifiedOccurrences = calendarItem.Recurrence.GetModifiedOccurrences();
					if (modifiedOccurrences.Count > 0)
					{
						OccurrenceInfoType[] array = new OccurrenceInfoType[modifiedOccurrences.Count];
						for (int i = 0; i < modifiedOccurrences.Count; i++)
						{
							array[i] = this.RenderOccurrenceInfo(storeIdAndSession, modifiedOccurrences[i]);
						}
						serviceObject.PropertyBag[CalendarItemSchema.ModifiedOccurrences] = array;
					}
				}
				break;
			case OccurrenceInfoProperty.PropertyCommandKind.DeletedOccurrences:
				if (calendarItem.Recurrence != null)
				{
					ExDateTime[] deletedOccurrences = calendarItem.Recurrence.GetDeletedOccurrences();
					if (deletedOccurrences.Length > 0)
					{
						DeletedOccurrenceInfoType[] array2 = new DeletedOccurrenceInfoType[deletedOccurrences.Length];
						for (int j = 0; j < deletedOccurrences.Length; j++)
						{
							array2[j] = new DeletedOccurrenceInfoType
							{
								Start = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(deletedOccurrences[j])
							};
						}
						serviceObject.PropertyBag[CalendarItemSchema.DeletedOccurrences] = array2;
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		private OccurrenceInfoType RenderOccurrenceInfo(IdAndSession storeIdAndSession, OccurrenceInfo occurrenceInfo)
		{
			ItemId itemId;
			if (storeIdAndSession.Session is PublicFolderSession)
			{
				ConcatenatedIdAndChangeKey concatenatedIdForPublicFolderItem = IdConverter.GetConcatenatedIdForPublicFolderItem(occurrenceInfo.VersionedId, StoreId.GetStoreObjectId(storeIdAndSession.ParentFolderId), null);
				itemId = new ItemId(concatenatedIdForPublicFolderItem.Id, concatenatedIdForPublicFolderItem.ChangeKey);
			}
			else
			{
				itemId = IdConverter.ConvertStoreItemIdToItemId(occurrenceInfo.VersionedId, storeIdAndSession.Session);
			}
			return new OccurrenceInfoType
			{
				ItemId = itemId,
				Start = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(occurrenceInfo.StartTime),
				End = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(occurrenceInfo.EndTime),
				OriginalStart = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(occurrenceInfo.OriginalStartTime)
			};
		}

		private const string XmlElementNameDeletedOccurrence = "DeletedOccurrence";

		private const string XmlElementNameOccurrence = "Occurrence";

		private const string XmlElementNameStart = "Start";

		private const string XmlElementNameEnd = "End";

		private const string XmlElementNameOriginalStart = "OriginalStart";

		private OccurrenceInfoProperty.PropertyCommandKind propertyCommandKind;

		private enum PropertyCommandKind
		{
			FirstOccurrence,
			LastOccurrence,
			ModifiedOccurrences,
			DeletedOccurrences
		}
	}
}
