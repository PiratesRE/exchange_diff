using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CalendarQuery
	{
		internal static bool ArePropertiesValid(CalendarItemBase item, out string errorString)
		{
			return CalendarQuery.ArePropertiesValid(item.GetProperties(CalendarQuery.CalendarQueryProps), out errorString);
		}

		internal static bool ArePropertiesValid(object[] properties, out string errorString)
		{
			bool flag = true;
			errorString = null;
			if (properties[29] is string)
			{
				string text = (string)properties[29];
				if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(text))
				{
					flag = false;
					errorString = string.Format("The item is of class '{0}'.", text);
				}
				else
				{
					object[] array;
					if (properties[5] is PropertyError)
					{
						array = CalendarQuery.SingleMeetingPropertyDefaults;
					}
					else if ((CalendarItemType)properties[5] == CalendarItemType.RecurringMaster)
					{
						array = CalendarQuery.RecurringMeetingPropertyDefaults;
					}
					else
					{
						array = CalendarQuery.SingleMeetingPropertyDefaults;
					}
					for (int i = 0; i < properties.Length; i++)
					{
						if (properties[i] is PropertyError)
						{
							CalendarQueryPropOrder calendarQueryPropOrder = (CalendarQueryPropOrder)i;
							if (calendarQueryPropOrder == CalendarQueryPropOrder.Id || calendarQueryPropOrder == CalendarQueryPropOrder.GlobalId || calendarQueryPropOrder == CalendarQueryPropOrder.CleanGlobalId || calendarQueryPropOrder == CalendarQueryPropOrder.StartTime || calendarQueryPropOrder == CalendarQueryPropOrder.EndTime)
							{
								if (properties[9] is string)
								{
									errorString = string.Format("Cannot access property {0}({1}) on Meeting=\"{2}\", skipping calendar item", i, CalendarQuery.CalendarQueryProps[i].Name, properties[9]);
								}
								else
								{
									errorString = string.Format("Cannot access property {0}({1}) on Meeting=\"subject missing\", skipping calendar item", i, CalendarQuery.CalendarQueryProps[i].Name);
								}
								flag = false;
								break;
							}
							properties[i] = array[i];
						}
					}
					if (flag)
					{
						if (properties[1] != null && ((byte[])properties[1]).Length < 40)
						{
							if (properties[9] is string)
							{
								errorString = string.Format("Incorrectly formatted GOID on Meeting=\"{0}\", skipping calendar item", (string)properties[9]);
							}
							else
							{
								errorString = "Incorrectly formatted GOID on Meeting=\"subject missing\", skipping calendar item";
							}
							flag = false;
						}
						else if (properties[2] != null && ((byte[])properties[2]).Length < 40)
						{
							if (properties[9] is string)
							{
								errorString = string.Format("Incorrectly formatted CleanGOID on Meeting=\"{0}\", skipping calendar item", (string)properties[9]);
							}
							else
							{
								errorString = "Incorrectly formatted CleanGOID on Meeting=\"subject missing\", skipping calendar item";
							}
							flag = false;
						}
					}
				}
			}
			else
			{
				flag = false;
				errorString = "The item class returns error.";
			}
			return flag;
		}

		internal static object[][] GetRecurringMasters(CalendarFolder calendarFolder, params PropertyDefinition[] properties)
		{
			object[][] array;
			using (QueryResult queryResult = calendarFolder.ItemQuery(ItemQueryType.None, CalendarQuery.RecurringItemFilter, null, properties))
			{
				queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
				int num = 10000;
				int num2 = num;
				bool flag = true;
				List<object[][]> list = new List<object[][]>();
				while (num2 > 0 && flag)
				{
					object[][] rows = queryResult.GetRows(num2);
					num2 -= rows.Length;
					if (list.Count == 0 || rows.Length > 0)
					{
						list.Add(rows);
					}
					if (rows.Length == 0)
					{
						flag = false;
					}
				}
				if (list.Count == 1)
				{
					array = list[0];
				}
				else
				{
					array = new object[num - num2][];
					int num3 = 0;
					foreach (object[][] array2 in list)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							array[num3++] = array2[i];
						}
					}
				}
			}
			return array;
		}

		internal static List<MeetingData> GetCorrelatedMeetings(MailboxSession session, UserObject mailboxUser, StoreId meetingId)
		{
			List<MeetingData> list = new List<MeetingData>();
			using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(session, meetingId, CalendarQuery.CalendarQueryProps))
			{
				MeetingData item = MeetingData.CreateInstance(mailboxUser, calendarItemBase);
				list.Add(item);
				if (calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster)
				{
					try
					{
						IList<OccurrenceInfo> modifiedOccurrences = (calendarItemBase as CalendarItem).Recurrence.GetModifiedOccurrences();
						foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
						{
							MeetingData item2 = MeetingData.CreateInstance(mailboxUser, occurrenceInfo.VersionedId, calendarItemBase.GlobalObjectId, CalendarItemType.Exception);
							list.Add(item2);
						}
					}
					catch (RecurrenceFormatException)
					{
					}
				}
			}
			return list;
		}

		internal static List<MeetingData> GetMeetingsInRange(MailboxSession session, UserObject mailboxUser, CalendarFolder calendarFolder, ExDateTime rangeStart, ExDateTime rangeEnd, string locationToFilterWith, string subjectToFilterWith, List<MeetingValidationResult> results)
		{
			object[][] array = null;
			List<MeetingData> list = new List<MeetingData>();
			HashSet<StoreId> hashSet = new HashSet<StoreId>();
			if (results == null)
			{
				return list;
			}
			array = CalendarQuery.GetRecurringMasters(calendarFolder, CalendarQuery.CalendarQueryProps);
			foreach (object[] array3 in array)
			{
				if (!ObjectClass.IsCalendarItemSeries((string)array3[29]))
				{
					string text;
					if (!CalendarQuery.ArePropertiesValid(array3, out text))
					{
						StoreId storeId = (StoreId)array3[0];
						MeetingData meetingData = MeetingData.CreateInstance(mailboxUser, storeId);
						results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData, false, text));
					}
					else
					{
						string location = (string)array3[8];
						string text2 = (string)array3[9];
						if (CalendarQuery.ShouldContinueLocationSubjectFilters(locationToFilterWith, subjectToFilterWith, location, text2))
						{
							StoreId storeId = (StoreId)array3[0];
							bool flag = (bool)array3[3];
							if (flag)
							{
								CalendarItem calendarItem = null;
								string empty = string.Empty;
								try
								{
									calendarItem = CalendarItem.Bind(session, storeId, CalendarQuery.CalendarQueryProps);
									if (calendarItem.Recurrence != null)
									{
										IList<OccurrenceInfo> modifiedOccurrences = calendarItem.Recurrence.GetModifiedOccurrences();
										foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
										{
											StoreId versionedId = occurrenceInfo.VersionedId;
											hashSet.Add(versionedId);
										}
										OccurrenceInfo firstOccurrence;
										OccurrenceInfo occurrenceInfo2;
										try
										{
											firstOccurrence = calendarItem.Recurrence.GetFirstOccurrence();
											if (calendarItem.Recurrence.Range is NoEndRecurrenceRange)
											{
												occurrenceInfo2 = null;
											}
											else
											{
												occurrenceInfo2 = calendarItem.Recurrence.GetLastOccurrence();
											}
										}
										catch (RecurrenceHasNoOccurrenceException)
										{
											goto IL_350;
										}
										catch (InvalidOperationException)
										{
											MeetingData meetingData2 = MeetingData.CreateInstance(mailboxUser, storeId);
											results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData2, false, string.Format("GetMeetingsInRange: InvalidOperationException: no end occurence, Subject=\"{0}\"", text2)));
											goto IL_350;
										}
										if ((occurrenceInfo2 == null || !(occurrenceInfo2.StartTime < rangeStart)) && !(firstOccurrence.EndTime >= rangeEnd))
										{
											QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(session, CalendarQuery.CalendarQueryProps);
											queryResultPropertyBag.SetQueryResultRow(array3);
											MeetingData item = MeetingData.CreateInstance(mailboxUser, calendarItem);
											list.Add(item);
											foreach (OccurrenceInfo occurrenceInfo3 in modifiedOccurrences)
											{
												if (!(occurrenceInfo3.StartTime > rangeEnd) && !(occurrenceInfo3.StartTime < rangeStart))
												{
													MeetingData item2 = MeetingData.CreateInstance(mailboxUser, occurrenceInfo3.VersionedId, calendarItem.GlobalObjectId, CalendarItemType.Exception);
													list.Add(item2);
												}
											}
										}
									}
								}
								catch (RecurrenceFormatException exception)
								{
									MeetingData item3 = MeetingData.CreateInstance(mailboxUser, storeId, null, exception);
									list.Add(item3);
								}
								catch (ObjectNotFoundException exception2)
								{
									MeetingData meetingData3 = MeetingData.CreateInstance(mailboxUser, storeId, null, exception2);
									list.Add(meetingData3);
									results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData3, false, string.Format("GetMeetingsInRange: ObjectNotFoundException - could not find this meeting, Subject=\"{0}\"", text2)));
								}
								catch (CorruptDataException arg)
								{
									text = string.Format("GetMeetingsInRange: Subject=\"{0}\", CorruptDataException: {1}", text2, arg);
									MeetingValidationResult mvresultWithMoreCalItemData = CalendarQuery.GetMVResultWithMoreCalItemData(session, mailboxUser, storeId, text, text2, rangeStart, rangeEnd);
									results.Add(mvresultWithMoreCalItemData);
								}
								catch (ArgumentException exception3)
								{
									MeetingData meetingData4 = MeetingData.CreateInstance(mailboxUser, storeId, null, exception3);
									list.Add(meetingData4);
									results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData4, false, string.Format("GetMeetingsInRange: ArgumentException - could not bind this meeting, Subject=\"{0}\"", text2)));
								}
								catch (AccessDeniedException arg2)
								{
									MeetingData meetingData5 = MeetingData.CreateInstance(mailboxUser, storeId);
									results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData5, false, string.Format("GetMeetingsInRange:  AccessDeniedException: {0}", arg2)));
								}
								finally
								{
									if (calendarItem != null)
									{
										calendarItem.Dispose();
									}
								}
							}
						}
					}
				}
				IL_350:;
			}
			try
			{
				array = calendarFolder.GetCalendarView(rangeStart, rangeEnd, CalendarQuery.CalendarQueryProps);
			}
			catch (Exception arg3)
			{
				MeetingData meetingData6 = MeetingData.CreateInstance(mailboxUser, null);
				results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData6, false, string.Format("GetMeetingsInRange:  Exception: {0}", arg3)));
				return list;
			}
			foreach (object[] array5 in array)
			{
				if (!ObjectClass.IsCalendarItemSeries((string)array5[29]))
				{
					string errorDescription;
					if (!CalendarQuery.ArePropertiesValid(array5, out errorDescription))
					{
						StoreId storeId2 = (StoreId)array5[0];
						MeetingData meetingData7 = MeetingData.CreateInstance(mailboxUser, storeId2);
						results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData7, false, errorDescription));
					}
					else
					{
						string location2 = (string)array5[8];
						string subject = (string)array5[9];
						if (CalendarQuery.ShouldContinueLocationSubjectFilters(locationToFilterWith, subjectToFilterWith, location2, subject))
						{
							StoreId storeId2 = (StoreId)array5[0];
							bool flag2 = (bool)array5[3];
							if (flag2)
							{
								byte[] array6 = (byte[])array5[1];
								GlobalObjectId globalObjectId;
								try
								{
									globalObjectId = new GlobalObjectId(array6);
								}
								catch (CorruptDataException arg4)
								{
									MeetingData meetingData8 = MeetingData.CreateInstance(mailboxUser, storeId2);
									results.Add(new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData8, false, string.Format("GetMeetingsInRange: CorruptDataException: The Global Object ID is invalid ({0}). Error: {1}", GlobalObjectId.ByteArrayToHexString(array6), arg4)));
									goto IL_54A;
								}
								int value = (int)array5[30];
								CalendarItemType calendarItemType = (CalendarItemType)array5[5];
								if (calendarItemType == CalendarItemType.Single)
								{
									int appointmentSequenceNumber = (int)array5[15];
									ExDateTime lastModifiedTime = (ExDateTime)array5[14];
									ExDateTime ownerCriticalChangeTime = (ExDateTime)array5[17];
									int documentId = (int)array5[33];
									MeetingData item4 = MeetingData.CreateInstance(mailboxUser, storeId2, globalObjectId, appointmentSequenceNumber, lastModifiedTime, ownerCriticalChangeTime, CalendarItemType.Single, subject, new int?(value), documentId);
									list.Add(item4);
								}
								else if (calendarItemType == CalendarItemType.Exception)
								{
									if (!hashSet.Contains(storeId2))
									{
										MeetingData item5 = MeetingData.CreateInstance(mailboxUser, storeId2, globalObjectId, CalendarItemType.Exception);
										list.Add(item5);
									}
								}
								else if (calendarItemType == CalendarItemType.Occurrence)
								{
									MeetingData item6 = MeetingData.CreateInstance(mailboxUser, storeId2, globalObjectId, CalendarItemType.Occurrence);
									list.Add(item6);
								}
							}
						}
					}
				}
				IL_54A:;
			}
			return list;
		}

		internal static bool ShouldContinueLocationSubjectFilters(string locationToFilterWith, string subjectToFilterWith, string location, string subject)
		{
			if (!string.IsNullOrEmpty(locationToFilterWith))
			{
				if (locationToFilterWith.EndsWith("*"))
				{
					string value = locationToFilterWith.Substring(0, locationToFilterWith.Length - 1);
					if (!string.IsNullOrEmpty(value) && !location.StartsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
				}
				else if (string.Compare(location, locationToFilterWith, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(subjectToFilterWith))
			{
				if (subjectToFilterWith.EndsWith("*"))
				{
					string value2 = subjectToFilterWith.Substring(0, subjectToFilterWith.Length - 1);
					if (!string.IsNullOrEmpty(value2) && !subject.StartsWith(value2, StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
				}
				else if (string.Compare(subject, subjectToFilterWith, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
			}
			return true;
		}

		internal static MeetingValidationResult GetMVResultWithMoreCalItemData(MailboxSession session, UserObject mailboxUser, StoreId id, string errorString, string subject, ExDateTime rangeStart, ExDateTime rangeEnd)
		{
			MeetingValidationResult result = null;
			try
			{
				using (MessageItem messageItem = Item.BindAsMessage(session, id, CalendarQuery.CalendarQueryProps))
				{
					MeetingData meetingData = MeetingData.CreateInstance(mailboxUser, messageItem);
					if (!string.IsNullOrEmpty(meetingData.InternetMessageId))
					{
						errorString += string.Format(", InternetMessageId = {0}", meetingData.InternetMessageId);
					}
					result = new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData, false, errorString);
				}
			}
			catch (Exception arg)
			{
				errorString = string.Format("GetMVResultWithMoreCalItemData: Could not re-open as a message, Subject=\"{0}\", Original Error Message:\"{1}\", Exception = {2}", subject, errorString, arg);
				MeetingData meetingData2 = MeetingData.CreateInstance(mailboxUser, id);
				result = new MeetingValidationResult(rangeStart, rangeEnd, mailboxUser, meetingData2, false, errorString);
			}
			return result;
		}

		internal static CalendarItemBase FindMatchingItem(MailboxSession session, CalendarFolder calendarFolder, CalendarItemType itemType, byte[] globalId, ref string errorString)
		{
			VersionedId calendarItemId = calendarFolder.GetCalendarItemId(globalId);
			if (calendarItemId == null)
			{
				errorString = string.Format("FindMatchingItem: Could not find calendar item, itemId is null. ", new object[0]);
				return null;
			}
			CalendarItemBase result;
			try
			{
				result = CalendarItemBase.Bind(session, calendarItemId, CalendarQuery.CalendarQueryProps);
			}
			catch (ObjectNotFoundException ex)
			{
				errorString = string.Format("[{0}(UTC)] FindMatchingItem: Could not find calendar item, exception = {1}. ", ExDateTime.UtcNow, ex.GetType());
				result = null;
			}
			catch (ArgumentException ex2)
			{
				errorString = string.Format("[{0}(UTC)] FindMatchingItem: Could not bind to item as CalendarItemBase, exception = {1}. ", ExDateTime.UtcNow, ex2.GetType());
				result = null;
			}
			return result;
		}

		internal static readonly Guid PSETID_Appointment = new Guid("{00062002-0000-0000-C000-000000000046}");

		internal static readonly GuidIdPropertyDefinition AppointmentRecurrenceBlob = GuidIdPropertyDefinition.CreateCustom("AppointmentRecurrenceBlob", typeof(byte[]), CalendarQuery.PSETID_Appointment, 33302, PropertyFlags.None);

		internal static PropertyDefinition[] CalendarQueryProps = new PropertyDefinition[]
		{
			ItemSchema.Id,
			CalendarItemBaseSchema.GlobalObjectId,
			CalendarItemBaseSchema.CleanGlobalObjectId,
			CalendarItemBaseSchema.IsMeeting,
			CalendarItemBaseSchema.AppointmentState,
			CalendarItemBaseSchema.CalendarItemType,
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			CalendarItemBaseSchema.Location,
			ItemSchema.Subject,
			CalendarQuery.AppointmentRecurrenceBlob,
			CalendarItemBaseSchema.ResponseType,
			ItemSchema.IsResponseRequested,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.LastModifiedTime,
			CalendarItemBaseSchema.AppointmentSequenceNumber,
			CalendarItemBaseSchema.OwnerAppointmentID,
			CalendarItemBaseSchema.OwnerCriticalChangeTime,
			CalendarItemBaseSchema.AttendeeCriticalChangeTime,
			CalendarItemBaseSchema.AppointmentReplyTime,
			CalendarItemBaseSchema.TimeZoneBlob,
			CalendarItemBaseSchema.AppointmentExtractTime,
			CalendarItemBaseSchema.AppointmentExtractVersion,
			CalendarItemBaseSchema.AppointmentLastSequenceNumber,
			MessageItemSchema.MessageInConflict,
			ItemSchema.TimeZoneDefinitionStart,
			CalendarItemBaseSchema.TimeZoneDefinitionEnd,
			CalendarItemBaseSchema.TimeZone,
			CalendarItemBaseSchema.TimeZoneDefinitionRecurring,
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.ItemVersion,
			ItemSchema.SubjectPrefix,
			ItemSchema.NormalizedSubject,
			ItemSchema.DocumentId
		};

		internal static object[] SingleMeetingPropertyDefaults = new object[]
		{
			null,
			null,
			null,
			true,
			null,
			null,
			null,
			null,
			"MVERROR: location missing",
			"MVERROR: subject missing",
			new byte[0],
			0,
			true,
			null,
			null,
			0,
			null,
			ExDateTime.MinValue,
			ExDateTime.MinValue,
			ExDateTime.MinValue,
			new byte[0],
			ExDateTime.MinValue,
			0L,
			0,
			false,
			new byte[0],
			new byte[0],
			string.Empty,
			new byte[0],
			null,
			0,
			string.Empty,
			string.Empty,
			0
		};

		internal static object[] RecurringMeetingPropertyDefaults = new object[]
		{
			null,
			null,
			null,
			true,
			null,
			null,
			null,
			null,
			"MVERROR: location missing",
			"MVERROR: subject missing",
			null,
			0,
			true,
			null,
			null,
			0,
			null,
			ExDateTime.MinValue,
			ExDateTime.MinValue,
			ExDateTime.MinValue,
			new byte[0],
			ExDateTime.MinValue,
			0L,
			0,
			false,
			new byte[0],
			new byte[0],
			string.Empty,
			new byte[0],
			null,
			0,
			string.Empty,
			string.Empty,
			0
		};

		internal static QueryFilter RecurringItemFilter = new ExistsFilter(CalendarQuery.AppointmentRecurrenceBlob);
	}
}
