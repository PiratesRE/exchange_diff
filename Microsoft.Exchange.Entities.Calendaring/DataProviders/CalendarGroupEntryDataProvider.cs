using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class CalendarGroupEntryDataProvider : StorageItemDataProvider<IMailboxSession, Calendar, ICalendarGroupEntry>
	{
		public CalendarGroupEntryDataProvider(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope, null, ExTraceGlobals.CalendarDataProviderTracer)
		{
		}

		protected override IStorageTranslator<ICalendarGroupEntry, Calendar> Translator
		{
			get
			{
				return CalendarGroupEntryTranslator.Instance;
			}
		}

		public virtual Calendar Create(Calendar entity, ICalendarGroup calendarGroup)
		{
			this.Validate(entity, true);
			Calendar result;
			using (ICalendarGroupEntry calendarGroupEntry = base.XsoFactory.CreateCalendarGroupEntry(base.Session, StoreId.GetStoreObjectId(entity.CalendarFolderStoreId), calendarGroup))
			{
				result = this.Update(entity, calendarGroupEntry, SaveMode.NoConflictResolution);
			}
			return result;
		}

		public override void Validate(Calendar entity, bool isNew)
		{
			if (entity.IsPropertySet(entity.Schema.NameProperty))
			{
				entity.Name = ((entity.Name != null) ? entity.Name.Trim() : null);
				if (string.IsNullOrWhiteSpace(entity.Name))
				{
					throw new CalendarNameCannotBeEmptyException();
				}
			}
			else if (isNew)
			{
				throw new CalendarNameCannotBeEmptyException();
			}
			base.Validate(entity, isNew);
		}

		protected internal override ICalendarGroupEntry BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToCalendarGroupEntry(base.Session, id);
		}

		protected internal override void ValidateStoreObjectIdForCorrectType(StoreObjectId storeObjectId)
		{
		}

		protected override ICalendarGroupEntry CreateNewStoreObject()
		{
			throw new NotImplementedException();
		}

		protected override void SaveAndCheckForConflicts(ICalendarGroupEntry storeObject, SaveMode saveMode)
		{
			try
			{
				base.SaveAndCheckForConflicts(storeObject, saveMode);
			}
			catch (IrresolvableConflictException innerException)
			{
				throw new CalendarGroupEntryUpdateFailedException(innerException);
			}
		}
	}
}
