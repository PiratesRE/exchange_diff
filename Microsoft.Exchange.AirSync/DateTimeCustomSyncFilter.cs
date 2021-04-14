using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class DateTimeCustomSyncFilter : ISyncFilter
	{
		internal DateTimeCustomSyncFilter(ExDateTime filterStartTime, FolderSyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			this.filterStartTime = filterStartTime;
			this.syncState = syncState;
		}

		internal DateTimeCustomSyncFilter(FolderSyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			this.syncState = syncState;
		}

		private DateTimeCustomSyncFilter()
		{
		}

		public string Id
		{
			get
			{
				if (this.prepopulate)
				{
					return "DateTimeCustomSyncFilter: Prepopulating";
				}
				return "DateTimeCustomSyncFilter: > " + this.filterStartTime.ToString(DateTimeFormatInfo.InvariantInfo);
			}
		}

		private Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState> CustomFilterState
		{
			get
			{
				return this.syncState.GetData<GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, DateTimeCustomSyncFilter.FilterState>, Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState>>(CustomStateDatumType.CustomCalendarSyncFilter, null);
			}
			set
			{
				this.syncState[CustomStateDatumType.CustomCalendarSyncFilter] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, DateTimeCustomSyncFilter.FilterState>(value);
			}
		}

		public bool IsItemInFilter(ISyncItemId itemId)
		{
			if (this.prepopulate)
			{
				return false;
			}
			if (this.CustomFilterState == null)
			{
				this.CustomFilterState = new Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState>();
			}
			return this.CustomFilterState.ContainsKey(itemId) && this.IsFilterStateInFilter(this.CustomFilterState[itemId]);
		}

		public void UpdateFilterState(SyncOperation syncOperation)
		{
			if (syncOperation == null)
			{
				throw new ArgumentNullException("syncOperation");
			}
			if (this.prepopulate)
			{
				return;
			}
			if (this.CustomFilterState == null)
			{
				this.CustomFilterState = new Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState>();
			}
			switch (syncOperation.ChangeType)
			{
			case ChangeType.Add:
			case ChangeType.Change:
			case ChangeType.ReadFlagChange:
			{
				CalendarItem calendarItem = null;
				try
				{
					calendarItem = (syncOperation.GetItem(new PropertyDefinition[0]).NativeItem as CalendarItem);
					if (calendarItem == null)
					{
						this.UpdateFilterStateWithAddOrChange(syncOperation.Id, false, false, ExDateTime.MinValue);
					}
					else if (calendarItem.Recurrence == null)
					{
						this.UpdateFilterStateWithAddOrChange(syncOperation.Id, true, false, calendarItem.EndTime);
					}
					else if (calendarItem.Recurrence.Range is NoEndRecurrenceRange)
					{
						this.UpdateFilterStateWithAddOrChange(syncOperation.Id, true, true, ExDateTime.MinValue);
					}
					else
					{
						this.UpdateFilterStateWithAddOrChange(syncOperation.Id, true, true, calendarItem.Recurrence.GetLastOccurrence().EndTime);
					}
				}
				catch (Exception ex)
				{
					if (ex is ObjectNotFoundException)
					{
						if (this.CustomFilterState.ContainsKey(syncOperation.Id))
						{
							this.CustomFilterState.Remove(syncOperation.Id);
						}
					}
					else
					{
						if (!SyncCommand.IsItemSyncTolerableException(ex))
						{
							throw;
						}
						StoreId storeId = null;
						string text = "Unknown";
						ExDateTime exDateTime = ExDateTime.MinValue;
						try
						{
							if (calendarItem != null)
							{
								storeId = calendarItem.Id;
								text = calendarItem.Subject;
								exDateTime = calendarItem.StartTime;
							}
						}
						catch
						{
						}
						AirSyncUtility.ExceptionToStringHelper exceptionToStringHelper = new AirSyncUtility.ExceptionToStringHelper(ex);
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Exception was caught in UpdateFilterState. Item id=\"{0}\", subject=\"{1}\", meetingTime={2}\r\n{3}\r\nIgnoring exception and proceeding to next item.", new object[]
						{
							(storeId != null) ? storeId : "null",
							text,
							exDateTime,
							exceptionToStringHelper
						});
					}
				}
				break;
			}
			case (ChangeType)3:
				break;
			case ChangeType.Delete:
				this.CustomFilterState.Remove(syncOperation.Id);
				return;
			default:
				return;
			}
		}

		internal void Prepopulate(Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.prepopulate = true;
			if (this.CustomFilterState == null)
			{
				this.CustomFilterState = new Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState>();
			}
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
			{
				ItemSchema.Id,
				CalendarItemInstanceSchema.EndTime,
				CalendarItemBaseSchema.CalendarItemType,
				StoreObjectSchema.ItemClass,
				ItemSchema.Subject,
				CalendarItemInstanceSchema.StartTime
			}))
			{
				bool flag = false;
				while (!flag)
				{
					object[][] rows = queryResult.GetRows(10000);
					flag = (rows.Length == 0);
					for (int i = 0; i < rows.Length; i++)
					{
						StoreObjectId storeObjectId = null;
						DateTimeCustomSyncFilter.FilterState filterState = null;
						ISyncItemId key = null;
						try
						{
							storeObjectId = ((VersionedId)rows[i][0]).ObjectId;
							string itemClass = rows[i][3] as string;
							key = MailboxSyncItemId.CreateForNewItem(storeObjectId);
							if (!this.CustomFilterState.ContainsKey(key))
							{
								filterState = new DateTimeCustomSyncFilter.FilterState();
								this.CustomFilterState[key] = filterState;
							}
							else
							{
								filterState = this.CustomFilterState[key];
							}
							if (!ObjectClass.IsCalendarItem(itemClass))
							{
								filterState.IsCalendarItem = false;
							}
							else
							{
								filterState.IsCalendarItem = true;
								if (!(rows[i][2] is CalendarItemType))
								{
									filterState.IsCalendarItem = false;
								}
								else
								{
									filterState.IsRecurring = (CalendarItemType.RecurringMaster == (CalendarItemType)rows[i][2]);
									if (filterState.IsRecurring)
									{
										using (CalendarItem calendarItem = CalendarItem.Bind(folder.Session, storeObjectId))
										{
											if (calendarItem.Recurrence != null)
											{
												if (calendarItem.Recurrence.Range is NoEndRecurrenceRange)
												{
													filterState.DoesRecurrenceEnd = false;
												}
												else
												{
													filterState.DoesRecurrenceEnd = true;
													OccurrenceInfo lastOccurrence = calendarItem.Recurrence.GetLastOccurrence();
													filterState.EndTime = lastOccurrence.EndTime;
												}
											}
											else
											{
												filterState.IsCalendarItem = false;
											}
											goto IL_1E6;
										}
									}
									if (!(rows[i][1] is ExDateTime))
									{
										filterState.IsCalendarItem = false;
									}
									else
									{
										filterState.EndTime = (ExDateTime)rows[i][1];
									}
									IL_1E6:;
								}
							}
						}
						catch (Exception ex)
						{
							if (ex is ObjectNotFoundException)
							{
								this.CustomFilterState.Remove(key);
							}
							else
							{
								if (!SyncCommand.IsItemSyncTolerableException(ex))
								{
									throw;
								}
								string text = "Unknown";
								ExDateTime exDateTime = ExDateTime.MinValue;
								try
								{
									text = (rows[i][4] as string);
									exDateTime = (ExDateTime)rows[i][5];
								}
								catch
								{
								}
								AirSyncUtility.ExceptionToStringHelper exceptionToStringHelper = new AirSyncUtility.ExceptionToStringHelper(ex);
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Exception was caught in Prepopulate. Item id=\"{0}\", subject=\"{1}\", meetingTime={2}\r\n{3}\r\nIgnoring exception and proceeding to next item.", new object[]
								{
									(storeObjectId != null) ? storeObjectId : "null",
									text,
									exDateTime,
									exceptionToStringHelper
								});
								if (filterState != null)
								{
									filterState.IsCalendarItem = false;
								}
							}
						}
					}
				}
			}
		}

		internal void UpdateFilterStateWithAddOrChange(ISyncItemId itemId, bool calendar, bool recurring, ExDateTime endTime)
		{
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			DateTimeCustomSyncFilter.FilterState filterState = null;
			if (this.CustomFilterState == null)
			{
				this.CustomFilterState = new Dictionary<ISyncItemId, DateTimeCustomSyncFilter.FilterState>();
			}
			try
			{
				if (!this.CustomFilterState.ContainsKey(itemId))
				{
					filterState = new DateTimeCustomSyncFilter.FilterState();
					this.CustomFilterState[itemId] = filterState;
				}
				else
				{
					filterState = this.CustomFilterState[itemId];
				}
				filterState.IsCalendarItem = calendar;
				if (calendar)
				{
					filterState.IsRecurring = recurring;
					if (recurring)
					{
						if (endTime.Equals(ExDateTime.MinValue))
						{
							filterState.DoesRecurrenceEnd = false;
						}
						else
						{
							filterState.DoesRecurrenceEnd = true;
							filterState.EndTime = endTime;
						}
					}
					else
					{
						filterState.EndTime = endTime;
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is ObjectNotFoundException || !SyncCommand.IsItemSyncTolerableException(ex))
				{
					throw;
				}
				if (filterState != null)
				{
					filterState.IsCalendarItem = false;
				}
			}
		}

		private bool IsFilterStateInFilter(DateTimeCustomSyncFilter.FilterState itemFilterState)
		{
			return itemFilterState != null && itemFilterState.IsCalendarItem && ((itemFilterState.IsRecurring && !itemFilterState.DoesRecurrenceEnd) || itemFilterState.EndTime > this.filterStartTime);
		}

		private bool prepopulate;

		private SyncState syncState;

		private ExDateTime filterStartTime = ExDateTime.MinValue;

		internal sealed class FilterState : ICustomSerializableBuilder, ICustomSerializable
		{
			public ushort TypeId
			{
				get
				{
					return DateTimeCustomSyncFilter.FilterState.typeId;
				}
				set
				{
					DateTimeCustomSyncFilter.FilterState.typeId = value;
				}
			}

			internal ExDateTime EndTime
			{
				get
				{
					return this.endTime;
				}
				set
				{
					this.endTime = value;
				}
			}

			internal bool IsRecurring
			{
				get
				{
					return this.recurring;
				}
				set
				{
					this.recurring = value;
				}
			}

			internal bool DoesRecurrenceEnd
			{
				get
				{
					return this.doesRecurrenceEnd;
				}
				set
				{
					this.doesRecurrenceEnd = value;
				}
			}

			internal bool IsCalendarItem
			{
				get
				{
					return this.calendarItem;
				}
				set
				{
					this.calendarItem = value;
				}
			}

			public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
			{
				writer.Write(this.calendarItem);
				componentDataPool.GetDateTimeDataInstance().Bind(this.endTime).SerializeData(writer, componentDataPool);
				writer.Write(this.recurring);
				writer.Write(this.doesRecurrenceEnd);
			}

			public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
			{
				this.calendarItem = reader.ReadBoolean();
				DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance.DeserializeData(reader, componentDataPool);
				this.endTime = dateTimeDataInstance.Data;
				this.recurring = reader.ReadBoolean();
				this.doesRecurrenceEnd = reader.ReadBoolean();
			}

			public ICustomSerializable BuildObject()
			{
				return new DateTimeCustomSyncFilter.FilterState();
			}

			private static ushort typeId;

			private bool calendarItem;

			private ExDateTime endTime = ExDateTime.MinValue;

			private bool recurring;

			private bool doesRecurrenceEnd;
		}
	}
}
