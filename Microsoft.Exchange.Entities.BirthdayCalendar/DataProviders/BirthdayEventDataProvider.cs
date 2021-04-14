using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.DataProviders
{
	internal class BirthdayEventDataProvider : StorageItemDataProvider<IStoreSession, BirthdayEvent, ICalendarItemBase>
	{
		public BirthdayEventDataProvider(IStorageEntitySetScope<IStoreSession> scope, StoreId containerFolderId) : base(scope, containerFolderId, ExTraceGlobals.BirthdayEventDataProviderTracer)
		{
		}

		protected override IStorageTranslator<ICalendarItemBase, BirthdayEvent> Translator
		{
			get
			{
				return BirthdayEventTranslator.Instance;
			}
		}

		public virtual BirthdayEvent CreateBirthday(BirthdayEvent birthdayEvent)
		{
			base.Trace.TraceDebug<string>((long)this.GetHashCode(), "BirthdayEventDataProvider::CreateBirthday: creating birthday for contact {0}", birthdayEvent.Subject);
			return this.Create(birthdayEvent);
		}

		public virtual IBirthdayEvent DeleteBirthdayEventForContact(StoreObjectId birthdayContactStoreObjectId)
		{
			IEnumerable<BirthdayEvent> results = this.FindBirthdayEventsForContactId(birthdayContactStoreObjectId);
			return this.DeleteBirthdayEvents(results);
		}

		public virtual BirthdayEvent DeleteBirthdayEvents(IEnumerable<BirthdayEvent> results)
		{
			if (results == null)
			{
				return null;
			}
			BirthdayEvent birthdayEvent = null;
			foreach (BirthdayEvent birthdayEvent2 in results)
			{
				birthdayEvent = birthdayEvent2;
				this.Delete(birthdayEvent.StoreId, DeleteItemFlags.HardDelete);
			}
			return birthdayEvent;
		}

		public virtual IEnumerable<BirthdayEvent> GetBirthdayCalendarView(ExDateTime startTime, ExDateTime endTime)
		{
			IEnumerable<BirthdayEvent> result;
			using (CalendarFolder calendarFolder = (CalendarFolder)base.XsoFactory.BindToCalendarFolder(base.Session, base.ContainerFolderId))
			{
				object[][] rows = calendarFolder.InternalGetCalendarView(startTime, endTime, false, false, true, RecurrenceExpansionOption.IncludeRegularOccurrences, BirthdayEventDataProvider.BirthdayEventPropertyDefinitions);
				Dictionary<PropertyDefinition, int> propertyIndices = this.GetPropertyIndices(BirthdayEventDataProvider.BirthdayEventPropertyDefinitions);
				IEnumerable<BirthdayEvent> enumerable = from birthdayEvent in this.ReadQueryResults(rows, propertyIndices)
				where ((IBirthdayEventInternal)birthdayEvent).ContactId != null
				select birthdayEvent;
				result = enumerable;
			}
			return result;
		}

		public virtual IEnumerable<BirthdayEvent> FindBirthdayEventsForContact(IBirthdayContact contact)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(((IBirthdayContactInternal)contact).StoreId);
			return this.FindBirthdayEventsForContactId(storeObjectId);
		}

		protected internal override ICalendarItemBase BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToCalendarItemBase(base.Session, id, BirthdayEventDataProvider.BirthdayEventPropertyDefinitions);
		}

		protected override ICalendarItemBase CreateNewStoreObject()
		{
			return base.XsoFactory.CreateCalendarItem(base.Session, base.ContainerFolderId);
		}

		protected override void LoadStoreObject(ICalendarItemBase storeObject)
		{
			storeObject.Load(BirthdayEventDataProvider.BirthdayEventPropertyDefinitions);
		}

		protected override IFolder BindToContainingFolder()
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(base.ContainerFolderId);
			return base.XsoFactory.BindToFolder(base.Session, storeObjectId);
		}

		private IEnumerable<BirthdayEvent> FindBirthdayEventsForContactId(ObjectId contactStoreObjectId)
		{
			SortBy[] sortByColumns = new SortBy[]
			{
				new SortBy(CalendarItemBaseSchema.BirthdayContactId, SortOrder.Ascending)
			};
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, CalendarItemBaseSchema.BirthdayContactId, contactStoreObjectId);
			using (IFolder folder = this.BindToContainingFolder())
			{
				using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, null, sortByColumns, BirthdayEventDataProvider.BirthdayEventPropertyDefinitions))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, queryFilter))
					{
						bool itemsRemaining = true;
						while (itemsRemaining)
						{
							base.Trace.TraceDebug<Guid>((long)this.GetHashCode(), "FindBirthdayEventForContact: Retrieving more items. Mailbox={0}", base.Session.MailboxGuid);
							IStorePropertyBag[] items = queryResult.GetPropertyBags(100);
							if (items != null && items.Length > 0)
							{
								foreach (IStorePropertyBag item in items)
								{
									if (EvaluatableFilter.Evaluate(queryFilter, item))
									{
										base.Trace.TraceDebug<Guid>((long)this.GetHashCode(), "FindBirthdayEventForContact: Returning found property bag. Mailbox={0}", base.Session.MailboxGuid);
										using (ICalendarItemBase calendarItem = this.Bind(item.GetValueOrDefault<StoreId>(CoreItemSchema.Id, null)))
										{
											yield return this.ConvertToEntity(calendarItem);
										}
									}
									else
									{
										base.Trace.TraceDebug((long)this.GetHashCode(), "FindBirthdayEventForContact: no more items");
										itemsRemaining = false;
									}
								}
							}
							else
							{
								base.Trace.TraceDebug<Guid>((long)this.GetHashCode(), "FindBirthdayEventForContact: No items found. Stop iterating. Mailbox={0}", base.Session.MailboxGuid);
								itemsRemaining = false;
							}
						}
					}
					base.Trace.TraceDebug<Guid>((long)this.GetHashCode(), "FindBirthdayEventForContact: No more property bags found. Mailbox={0}", base.Session.MailboxGuid);
				}
			}
			yield break;
		}

		private const int QueryResultRowCount = 100;

		internal static readonly PropertyDefinition[] BirthdayEventPropertyDefinitions = new PropertyDefinition[]
		{
			CoreItemSchema.Id,
			ItemSchema.Subject,
			CalendarItemBaseSchema.Birthday,
			CalendarItemBaseSchema.BirthdayContactId,
			CalendarItemBaseSchema.BirthdayContactAttributionDisplayName,
			CalendarItemBaseSchema.BirthdayContactPersonId,
			CalendarItemBaseSchema.IsBirthdayContactWritable
		};
	}
}
