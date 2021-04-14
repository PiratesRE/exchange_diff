using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class CalendarGroupDataProvider : StorageItemDataProvider<IMailboxSession, CalendarGroup, ICalendarGroup>
	{
		public CalendarGroupDataProvider(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope, null, ExTraceGlobals.CalendarDataProviderTracer)
		{
		}

		protected override IStorageTranslator<ICalendarGroup, CalendarGroup> Translator
		{
			get
			{
				return CalendarGroupTranslator.Instance;
			}
		}

		public override void Delete(StoreId id, DeleteItemFlags flags)
		{
			using (ICalendarGroup calendarGroup = this.Bind(id))
			{
				CalendarGroupType groupType = calendarGroup.GroupType;
				if (groupType == CalendarGroupType.MyCalendars || groupType == CalendarGroupType.OtherCalendars)
				{
					throw new CannotDeleteSpecialCalendarGroupException(calendarGroup.Id, calendarGroup.GroupClassId, calendarGroup.GroupName);
				}
				ReadOnlyCollection<CalendarGroupEntryInfo> childCalendars = calendarGroup.GetChildCalendars();
				List<StoreId> list = new List<StoreId>();
				foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in childCalendars)
				{
					if (calendarGroupEntryInfo is LocalCalendarGroupEntryInfo)
					{
						throw new CalendarGroupIsNotEmptyException(calendarGroup.Id, calendarGroup.GroupClassId, calendarGroup.GroupName, childCalendars.Count);
					}
					list.Add(calendarGroupEntryInfo.Id);
				}
				foreach (StoreId id2 in list)
				{
					base.Delete(id2, flags);
				}
			}
			base.Delete(id, flags);
		}

		public override void Validate(CalendarGroup entity, bool isNew)
		{
			if (entity.IsPropertySet(entity.Schema.NameProperty))
			{
				entity.Name = ((entity.Name != null) ? entity.Name.Trim() : null);
				if (string.IsNullOrWhiteSpace(entity.Name))
				{
					throw new InvalidCalendarGroupNameException();
				}
			}
			else if (isNew)
			{
				throw new InvalidCalendarGroupNameException();
			}
			base.Validate(entity, isNew);
		}

		protected internal override ICalendarGroup BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToCalendarGroup(base.Session, id, null);
		}

		protected override ICalendarGroup CreateNewStoreObject()
		{
			return base.XsoFactory.CreateCalendarGroup(base.Session);
		}
	}
}
