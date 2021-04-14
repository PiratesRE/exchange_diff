using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractCalendarItem : AbstractCalendarItemInstance, ICalendarItem, ICalendarItemInstance, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual string InternetMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int InstanceCreationIndex
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool HasExceptionalInboxReminders
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual Recurrence Recurrence
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public CalendarItemOccurrence OpenOccurrence(StoreObjectId id, params PropertyDefinition[] prefetchPropertyDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
