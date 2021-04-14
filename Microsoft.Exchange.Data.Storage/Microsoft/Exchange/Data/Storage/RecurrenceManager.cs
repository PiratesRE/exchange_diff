using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecurrenceManager
	{
		internal RecurrenceManager(StoreSession storeSession, InternalRecurrence recurrence)
		{
			this.recurrence = recurrence;
			this.storeSession = storeSession;
		}

		internal static bool CanPropertyBeInExceptionData(PropertyDefinition property)
		{
			for (int i = 0; i < RecurrenceManager.PropertiesInTheBlobO11.Length; i++)
			{
				if (RecurrenceManager.PropertiesInTheBlobO11[i].Equals(property))
				{
					return true;
				}
			}
			return false;
		}

		internal static void DeleteAttachment(Item message, ExTimeZone organizerTimeZone, ExDateTime startTime, ExDateTime endTime)
		{
			bool valueOrDefault = message.GetValueOrDefault<bool>(InternalSchema.MapiHasAttachment, true);
			if (!valueOrDefault)
			{
				ExTraceGlobals.RecurrenceTracer.TraceDebug((long)message.GetHashCode(), "RecurrenceManager::DeleteAttachment: There are no attachments on this message");
				return;
			}
			ItemAttachment itemAttachment = null;
			AttachmentId attachmentId = null;
			AttachmentId[] array;
			using (RecurrenceManager.InternalOpenEmbeddedMessageAndAttachment(message.AttachmentCollection, organizerTimeZone, startTime, endTime, out itemAttachment, true, out array, null))
			{
				if (itemAttachment != null)
				{
					using (itemAttachment)
					{
						if (itemAttachment.IsCalendarException)
						{
							CalendarItem calendarItem = message as CalendarItem;
							if (calendarItem != null)
							{
								calendarItem.ClearExceptionSummaryList();
							}
						}
						attachmentId = itemAttachment.Id;
					}
				}
			}
			if (attachmentId != null)
			{
				message.AttachmentCollection.Remove(attachmentId);
				foreach (AttachmentId attachmentId2 in array)
				{
					message.AttachmentCollection.Remove(attachmentId2);
				}
				return;
			}
			ExTraceGlobals.RecurrenceTracer.TraceDebug<ExDateTime, ExDateTime>((long)message.GetHashCode(), "RecurrenceManager::DeleteAttachment, Couldn't find attachment for startTime: {0} endTime: {1}", startTime, endTime);
		}

		internal static Item OpenEmbeddedMessageAndAttachment(AttachmentCollection attachments, ExTimeZone organizerTimeZone, ExDateTime userStartTime, ExDateTime userEndTime, out ItemAttachment itemAttachment, ICollection<PropertyDefinition> properties)
		{
			AttachmentId[] array;
			return RecurrenceManager.InternalOpenEmbeddedMessageAndAttachment(attachments, organizerTimeZone, userStartTime, userEndTime, out itemAttachment, false, out array, properties);
		}

		private static Item InternalOpenEmbeddedMessageAndAttachment(AttachmentCollection attachments, ExTimeZone organizerTimeZone, ExDateTime userStartTime, ExDateTime userEndTime, out ItemAttachment itemAttachment, bool detectDuplicatedAttachment, out AttachmentId[] duplicatedAttachmentIds, ICollection<PropertyDefinition> properties)
		{
			itemAttachment = null;
			duplicatedAttachmentIds = null;
			Item item = null;
			ItemAttachment itemAttachment2 = null;
			List<KeyValuePair<long, AttachmentHandle>> list = new List<KeyValuePair<long, AttachmentHandle>>();
			List<AttachmentId> list2 = null;
			ExDateTime exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(userStartTime);
			ExDateTime exDateTime2 = ExTimeZone.UtcTimeZone.ConvertDateTime(userEndTime);
			ExDateTime exDateTime3 = organizerTimeZone.ConvertDateTime(exDateTime);
			ExDateTime exDateTime4 = organizerTimeZone.ConvertDateTime(exDateTime2);
			exDateTime3 = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)exDateTime3);
			exDateTime4 = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)exDateTime4);
			if (properties == null)
			{
				properties = new PropertyDefinition[]
				{
					InternalSchema.StartTime,
					InternalSchema.EndTime
				};
			}
			else
			{
				properties = InternalSchema.Combine<PropertyDefinition>(properties, new PropertyDefinition[]
				{
					InternalSchema.StartTime,
					InternalSchema.EndTime
				});
			}
			CalendarItem calendarItem = attachments.ContainerItem as CalendarItem;
			List<RecurrenceManager.ExceptionSummary> list3;
			if (calendarItem != null)
			{
				if (calendarItem.ExceptionSummaryList == null)
				{
					ExTraceGlobals.RecurrenceTracer.TraceDebug((long)calendarItem.GetHashCode(), "For calendar item, retrieve exception summary information by walking through all hidden attachements");
					calendarItem.ExceptionSummaryList = RecurrenceManager.GetExceptionSumaryList(attachments);
				}
				list3 = calendarItem.ExceptionSummaryList;
			}
			else
			{
				ExTraceGlobals.RecurrenceTracer.TraceDebug((long)attachments.GetHashCode(), "No calendar item exists, retrieve exception summary information by walking through all hidden attachements");
				list3 = RecurrenceManager.GetExceptionSumaryList(attachments);
			}
			foreach (RecurrenceManager.ExceptionSummary exceptionSummary in list3)
			{
				ExDateTime utcStartTime = exceptionSummary.UtcStartTime;
				ExDateTime utcEndTime = exceptionSummary.UtcEndTime;
				TimeSpan timeSpan = utcStartTime - exDateTime;
				utcEndTime - exDateTime2;
				if (Math.Abs((exDateTime3 - exDateTime4 - (utcStartTime - utcEndTime)).Ticks) < 1200000000L && Math.Abs(timeSpan.Ticks) <= 504000000000L)
				{
					list.Add(new KeyValuePair<long, AttachmentHandle>(Math.Abs((utcStartTime - exDateTime3).Ticks), exceptionSummary.Handle));
				}
			}
			list.Sort(delegate(KeyValuePair<long, AttachmentHandle> left, KeyValuePair<long, AttachmentHandle> right)
			{
				if (left.Key == right.Key)
				{
					return 0;
				}
				if (left.Key <= right.Key)
				{
					return -1;
				}
				return 1;
			});
			bool flag = false;
			try
			{
				foreach (KeyValuePair<long, AttachmentHandle> keyValuePair in list)
				{
					ItemAttachment itemAttachment3 = (ItemAttachment)attachments.Open(keyValuePair.Value, new AttachmentType?(AttachmentType.EmbeddedMessage), null);
					ExTraceGlobals.RecurrenceTracer.TraceDebug<ExDateTime, int>((long)keyValuePair.Value.GetHashCode(), "Open embedded message for StartTime: {0}, Probables queue length: {1}", userStartTime, list.Count);
					if (itemAttachment3 != null)
					{
						Item item2 = null;
						bool flag2 = true;
						try
						{
							if (itemAttachment3.IsItemOpen)
							{
								ExTraceGlobals.RecurrenceTracer.TraceError((long)keyValuePair.Value.GetHashCode(), "Embedded message is in erroneous open state, which should be guaranteed by master");
							}
							else
							{
								item2 = itemAttachment3.GetItem(InternalSchema.Combine<PropertyDefinition>(MessageItemSchema.Instance.AutoloadProperties, properties));
								TimeSpan timeSpan2 = item2.GetValueOrDefault<ExDateTime>(InternalSchema.MapiStartTime, ExDateTime.MinValue) - userStartTime;
								TimeSpan timeSpan3 = item2.GetValueOrDefault<ExDateTime>(InternalSchema.MapiEndTime, ExDateTime.MinValue) - userEndTime;
								if (timeSpan2.TotalMinutes < 1.0 && timeSpan2.TotalMinutes > -1.0 && timeSpan3.TotalMinutes < 1.0 && timeSpan3.TotalMinutes > -1.0)
								{
									if (item == null)
									{
										flag2 = false;
										itemAttachment2 = itemAttachment3;
										item = item2;
										if (!detectDuplicatedAttachment)
										{
											break;
										}
										list2 = new List<AttachmentId>();
									}
									else
									{
										ExTraceGlobals.RecurrenceTracer.TraceDebug<AttachmentId, ExDateTime>((long)keyValuePair.Value.GetHashCode(), "Detected duplicated attachment {0} for StartTime: {1}", itemAttachment3.Id, userStartTime);
										list2.Add(itemAttachment3.Id);
									}
								}
							}
						}
						finally
						{
							if (flag2)
							{
								Util.DisposeIfPresent(item2);
								Util.DisposeIfPresent(itemAttachment3);
							}
						}
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(item);
					Util.DisposeIfPresent(itemAttachment2);
				}
			}
			itemAttachment = itemAttachment2;
			duplicatedAttachmentIds = ((list2 == null) ? null : list2.ToArray());
			return item;
		}

		internal object[] GetTruncatedRecurringMaster(object[] masterRow, ExDateTime windowStart, ExDateTime windowEnd, PropertyDefinition[] columns)
		{
			if (masterRow.Length != columns.Length)
			{
				throw new ArgumentException(ServerStrings.ExInvalidMasterValueAndColumnLength);
			}
			IList<OccurrenceInfo> occurrenceInfoList = this.recurrence.GetOccurrenceInfoList(windowStart, windowEnd);
			if (occurrenceInfoList.Count > 0)
			{
				OccurrenceInfo occurrenceInfo = occurrenceInfoList[0];
				OccurrenceInfo occurrenceInfo2 = occurrenceInfoList[occurrenceInfoList.Count - 1];
				int num = Array.IndexOf<PropertyDefinition>(columns, InternalSchema.MapiStartTime);
				int num2 = Array.IndexOf<PropertyDefinition>(columns, InternalSchema.MapiEndTime);
				if (num > 0)
				{
					masterRow[num] = occurrenceInfo.OccurrenceDateId + this.recurrence.StartOffset;
				}
				if (num2 > 0)
				{
					masterRow[num2] = occurrenceInfo.OccurrenceDateId + this.recurrence.EndOffset;
				}
				int num3 = Array.IndexOf<PropertyDefinition>(columns, InternalSchema.AppointmentRecurrenceBlob);
				if (num3 > 0)
				{
					int num4 = Array.IndexOf<PropertyDefinition>(columns, InternalSchema.GlobalObjectId);
					if (num4 > 0)
					{
						try
						{
							new GlobalObjectId(masterRow[num4] as byte[]);
						}
						catch (CorruptDataException)
						{
						}
					}
					InternalRecurrence truncatedRecurrence = this.GetTruncatedRecurrence(occurrenceInfo.OccurrenceDateId, occurrenceInfo2.OccurrenceDateId, windowEnd == ExDateTime.MaxValue);
					if (truncatedRecurrence != null)
					{
						masterRow[num3] = truncatedRecurrence.ToByteArray();
					}
				}
				return masterRow;
			}
			return null;
		}

		internal InternalRecurrence GetTruncatedRecurrence(ExDateTime firstOccurrenceId, ExDateTime lastOccurrenceId, bool keepNoEndDateRange)
		{
			RecurrenceRange recurrenceRange = (keepNoEndDateRange && this.recurrence.Range is NoEndRecurrenceRange) ? new NoEndRecurrenceRange(firstOccurrenceId) : new EndDateRecurrenceRange(firstOccurrenceId, lastOccurrenceId);
			if (!recurrenceRange.Equals(this.recurrence.Range))
			{
				InternalRecurrence internalRecurrence = new InternalRecurrence(this.recurrence.Pattern, recurrenceRange, this.recurrence.MasterItem as CalendarItem, this.recurrence.CreatedExTimeZone, this.recurrence.ReadExTimeZone, this.recurrence.StartOffset, this.recurrence.EndOffset);
				RecurrenceBlobMerger.Merge(this.storeSession, null, null, this.recurrence, internalRecurrence);
				return internalRecurrence;
			}
			return null;
		}

		internal List<object[]> Expand(object[] masterValues, ExDateTime start, ExDateTime end, int codePage, PropertyDefinition[] columns, RecurrenceExpansionOption options)
		{
			List<object[]> list = new List<object[]>();
			if (masterValues.Length != columns.Length)
			{
				throw new ArgumentException(ServerStrings.ExInvalidMasterValueAndColumnLength);
			}
			object[] array = null;
			if ((options & RecurrenceExpansionOption.TruncateMaster) == RecurrenceExpansionOption.TruncateMaster)
			{
				array = this.GetTruncatedRecurringMaster(masterValues, start, end, columns);
				if (array == null)
				{
					return list;
				}
			}
			else
			{
				array = masterValues;
			}
			int num = Array.IndexOf<PropertyDefinition>(columns, InternalSchema.ItemId);
			VersionedId versionedId = (VersionedId)array[num];
			Item item = null;
			AttachmentCollection attachmentCollection = null;
			bool flag;
			try
			{
				byte[] timeZoneBlob = O12TimeZoneFormatter.GetTimeZoneBlob(this.recurrence.CreatedExTimeZone);
				IList<OccurrenceInfo> occurrenceInfoList = this.recurrence.GetOccurrenceInfoList(start, end);
				flag = (occurrenceInfoList.Count != 0);
				foreach (OccurrenceInfo occurrenceInfo in occurrenceInfoList)
				{
					bool flag2 = occurrenceInfo is ExceptionInfo;
					if (flag2 || (options & RecurrenceExpansionOption.IncludeRegularOccurrences) == RecurrenceExpansionOption.IncludeRegularOccurrences)
					{
						object[] array2 = new object[array.Length];
						array.CopyTo(array2, 0);
						if (flag2)
						{
							this.PopulateExceptionOccurrence(array2, columns, (ExceptionInfo)occurrenceInfo, ref attachmentCollection, ref item, occurrenceInfo.VersionedId);
						}
						this.AdjustOccurrenceColumnsForExpansion(array2, columns, occurrenceInfo, codePage, timeZoneBlob, flag2);
						list.Add(array2);
					}
				}
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			if (flag && (options & RecurrenceExpansionOption.IncludeMaster) == RecurrenceExpansionOption.IncludeMaster)
			{
				list.Insert(0, array);
			}
			return list;
		}

		private static List<RecurrenceManager.ExceptionSummary> GetExceptionSumaryList(AttachmentCollection attachments)
		{
			List<RecurrenceManager.ExceptionSummary> list = new List<RecurrenceManager.ExceptionSummary>(attachments.Count);
			foreach (AttachmentHandle handle in attachments.GetAllHandles())
			{
				if (CoreAttachmentCollection.IsCalendarException(handle))
				{
					using (Attachment attachment = attachments.Open(handle, null))
					{
						ExDateTime? valueAsNullable = attachment.GetValueAsNullable<ExDateTime>(InternalSchema.AppointmentExceptionStartTime);
						if (valueAsNullable != null)
						{
							ExDateTime? valueAsNullable2 = attachment.GetValueAsNullable<ExDateTime>(InternalSchema.AppointmentExceptionEndTime);
							if (valueAsNullable2 != null)
							{
								ExDateTime utcStart = ExTimeZone.UtcTimeZone.ConvertDateTime(valueAsNullable.Value);
								ExDateTime utcEnd = ExTimeZone.UtcTimeZone.ConvertDateTime(valueAsNullable2.Value);
								list.Add(new RecurrenceManager.ExceptionSummary(handle, utcStart, utcEnd));
							}
						}
					}
				}
			}
			return list;
		}

		private static PropertyDefinition[] BuildPropertiesInTheBlobO11(PropertyDefinition[] propertiesInTheBlobO11)
		{
			return new NativeStorePropertyDefinition[]
			{
				InternalSchema.MapiStartTime,
				InternalSchema.MapiEndTime,
				InternalSchema.SubjectPrefixInternal,
				InternalSchema.NormalizedSubjectInternal,
				InternalSchema.MapiSubject,
				InternalSchema.Location,
				InternalSchema.AppointmentColor,
				InternalSchema.MapiIsAllDayEvent,
				InternalSchema.MapiHasAttachment,
				InternalSchema.FreeBusyStatus,
				InternalSchema.ReminderIsSetInternal,
				InternalSchema.ReminderMinutesBeforeStartInternal,
				InternalSchema.AppointmentStateInternal,
				InternalSchema.AllAttachmentsHidden,
				InternalSchema.ChangeHighlight,
				InternalSchema.TimeZoneDefinitionStart,
				InternalSchema.TimeZoneDefinitionEnd
			};
		}

		private static HashSet<PropertyDefinition> BuildMasterOnlyProperties(HashSet<PropertyDefinition> masterOnlyProperties)
		{
			return new HashSet<PropertyDefinition>(20)
			{
				InternalSchema.EntryId,
				InternalSchema.ParentEntryId,
				InternalSchema.Fid,
				InternalSchema.MID,
				InternalSchema.ChangeKey,
				InternalSchema.ItemClass,
				InternalSchema.SentRepresentingEmailAddress,
				InternalSchema.SentRepresentingType,
				InternalSchema.SentRepresentingDisplayName,
				InternalSchema.SentRepresentingEntryId,
				InternalSchema.AppointmentRecurrenceBlob,
				InternalSchema.TimeZone,
				InternalSchema.TimeZoneBlob,
				InternalSchema.TimeZoneDefinitionRecurring,
				InternalSchema.CleanGlobalObjectId,
				InternalSchema.AppointmentRecurring,
				InternalSchema.IsException,
				InternalSchema.IsRecurring,
				InternalSchema.MapiSensitivity,
				InternalSchema.ContainerClass,
				InternalSchema.MapiPRStartDate,
				InternalSchema.MapiPREndDate,
				InternalSchema.Categories,
				InternalSchema.ConversationIndexTracking,
				InternalSchema.ConversationIndex,
				InternalSchema.ConversationTopic,
				InternalSchema.EffectiveRights,
				InternalSchema.InstanceKey,
				InternalSchema.OnlineMeetingExternalLink,
				InternalSchema.IsResponseRequested,
				CalendarItemInstanceSchema.PropertyChangeMetadataRaw
			};
		}

		private void AdjustOccurrenceColumnsForExpansion(object[] occurrence, PropertyDefinition[] columns, OccurrenceInfo occurrenceInfo, int codePage, byte[] createdTimeZoneBlobCache, bool isException)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				PropertyDefinition propertyDefinition = columns[i];
				if (propertyDefinition.Equals(InternalSchema.ItemId))
				{
					occurrence[i] = occurrenceInfo.VersionedId;
				}
				else if (propertyDefinition.Equals(InternalSchema.IsRecurring))
				{
					occurrence[i] = true;
				}
				else if (propertyDefinition.Equals(InternalSchema.IsException))
				{
					occurrence[i] = isException;
				}
				else if (propertyDefinition.Equals(InternalSchema.AppointmentRecurring))
				{
					occurrence[i] = false;
				}
				else if (propertyDefinition.Equals(InternalSchema.AppointmentRecurrenceBlob))
				{
					occurrence[i] = new PropertyError(columns[i], PropertyErrorCode.NotFound);
				}
				else if (propertyDefinition.Equals(InternalSchema.MapiStartTime) || propertyDefinition.Equals(InternalSchema.MapiPRStartDate))
				{
					occurrence[i] = occurrenceInfo.StartTime;
				}
				else if (propertyDefinition.Equals(InternalSchema.MapiEndTime) || propertyDefinition.Equals(InternalSchema.MapiPREndDate))
				{
					occurrence[i] = occurrenceInfo.EndTime;
				}
				else if (propertyDefinition.Equals(InternalSchema.TimeZoneDefinitionStart) || propertyDefinition.Equals(InternalSchema.TimeZoneDefinitionEnd))
				{
					if (occurrence[i] is PropertyError)
					{
						occurrence[i] = createdTimeZoneBlobCache;
					}
				}
				else if (propertyDefinition.Equals(InternalSchema.ItemClass))
				{
					occurrence[i] = (isException ? (occurrence[i] = "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}") : (occurrence[i] = "IPM.Appointment.Occurrence"));
				}
				else if (propertyDefinition.Equals(InternalSchema.GlobalObjectId))
				{
					object obj = occurrence[Array.IndexOf<PropertyDefinition>(columns, InternalSchema.CleanGlobalObjectId)];
					byte[] array = obj as byte[];
					if (array != null)
					{
						occurrence[i] = new GlobalObjectId(array)
						{
							Date = ((OccurrenceStoreObjectId)occurrenceInfo.VersionedId.ObjectId).OccurrenceId
						}.Bytes;
					}
					else
					{
						PropertyError propertyError = obj as PropertyError;
						occurrence[i] = new PropertyError(InternalSchema.GlobalObjectId, propertyError.PropertyErrorCode, propertyError.PropertyErrorDescription);
					}
				}
				else if (propertyDefinition.Equals(InternalSchema.Codepage))
				{
					occurrence[i] = codePage;
				}
			}
		}

		private void PopulateExceptionOccurrence(object[] occurrence, PropertyDefinition[] columns, ExceptionInfo exception, ref AttachmentCollection attachments, ref Item master, VersionedId versionedId)
		{
			object[] array = null;
			for (int i = 0; i < columns.Length; i++)
			{
				if (!columns[i].Equals(InternalSchema.ItemId) && !columns[i].Equals(InternalSchema.IsRecurring) && !columns[i].Equals(InternalSchema.IsException) && !columns[i].Equals(InternalSchema.AppointmentRecurring) && !columns[i].Equals(InternalSchema.MapiStartTime) && !columns[i].Equals(InternalSchema.MapiPRStartDate) && !columns[i].Equals(InternalSchema.MapiEndTime) && !columns[i].Equals(InternalSchema.MapiPREndDate) && !columns[i].Equals(InternalSchema.CalendarItemType) && !columns[i].Equals(InternalSchema.GlobalObjectId) && !columns[i].Equals(InternalSchema.TimeZoneDefinitionStart) && !columns[i].Equals(InternalSchema.TimeZoneDefinitionEnd) && !columns[i].Equals(InternalSchema.Codepage) && !RecurrenceManager.MasterOnlyProperties.Contains(columns[i]))
				{
					if (RecurrenceManager.CanPropertyBeInExceptionData(columns[i]))
					{
						object obj = exception.PropertyBag.TryGetProperty(columns[i]);
						PropertyError propertyError = obj as PropertyError;
						if (propertyError != null)
						{
							if (propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
							{
								throw PropertyError.ToException(new PropertyError[]
								{
									propertyError
								});
							}
						}
						else
						{
							occurrence[i] = obj;
						}
					}
					else
					{
						ExTraceGlobals.RecurrenceTracer.Information<PropertyDefinition>((long)this.recurrence.GetHashCode(), "RecurrenceManager::Expand, Opening embedded message for {0} property", columns[i]);
						if (array == null)
						{
							if (attachments == null)
							{
								if (master == null)
								{
									ExTraceGlobals.RecurrenceTracer.Information((long)this.recurrence.GetHashCode(), "RecurrenceManager::Expand. Fetching master message when constructing view");
									StoreObjectId masterId = versionedId.ObjectId;
									bool flag = false;
									try
									{
										master = ItemBuilder.ConstructItem<Item>(this.storeSession, masterId, null, CalendarItemBaseSchema.Instance.AutoloadProperties, () => new StoreObjectPropertyBag(this.storeSession, this.storeSession.GetMapiProp(masterId), CalendarItemBaseSchema.Instance.AutoloadProperties), ItemCreateInfo.GenericItemInfo.Creator, Origin.Existing, ItemLevel.TopLevel);
										flag = true;
									}
									finally
									{
										if (!flag && master != null)
										{
											master.Dispose();
											master = null;
										}
									}
								}
								attachments = master.AttachmentCollection;
								ExTraceGlobals.RecurrenceTracer.Information((long)this.recurrence.GetHashCode(), "RecurrenceManager::Expand, Fetching attachments");
							}
							ItemAttachment itemAttachment = null;
							using (Item item = RecurrenceManager.OpenEmbeddedMessageAndAttachment(attachments, TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(attachments.ContainerItem.PropertyBag), exception.StartTime, exception.EndTime, out itemAttachment, columns))
							{
								ExTraceGlobals.RecurrenceTracer.Information<ExDateTime>((long)this.recurrence.GetHashCode(), "RecurrenceManager::Expand, Fetching Embedded Message for exception occurence dateId: {0}", exception.OccurrenceDateId);
								if (item != null)
								{
									array = item.GetProperties(columns);
									itemAttachment.Dispose();
								}
								else
								{
									array = new object[columns.Length];
								}
							}
						}
						if (array[i] != null && !(array[i] is PropertyError))
						{
							occurrence[i] = array[i];
						}
					}
				}
			}
		}

		internal static readonly PropertyDefinition[] PropertiesInTheBlobO11 = RecurrenceManager.BuildPropertiesInTheBlobO11(RecurrenceManager.PropertiesInTheBlobO11);

		internal static readonly HashSet<PropertyDefinition> MasterOnlyProperties = RecurrenceManager.BuildMasterOnlyProperties(RecurrenceManager.MasterOnlyProperties);

		private InternalRecurrence recurrence;

		private StoreSession storeSession;

		internal class ExceptionSummary
		{
			internal ExceptionSummary(AttachmentHandle handle, ExDateTime utcStart, ExDateTime utcEnd)
			{
				this.Handle = handle;
				this.UtcStartTime = utcStart;
				this.UtcEndTime = utcEnd;
			}

			public readonly AttachmentHandle Handle;

			public readonly ExDateTime UtcStartTime;

			public readonly ExDateTime UtcEndTime;
		}
	}
}
