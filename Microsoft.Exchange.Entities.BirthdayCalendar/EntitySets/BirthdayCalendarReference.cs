using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal class BirthdayCalendarReference : StorageEntityReference<IBirthdayCalendars, IBirthdayCalendar, IMailboxSession>, IBirthdayCalendarReference, IEntityReference<IBirthdayCalendar>
	{
		public BirthdayCalendarReference(IBirthdayCalendars entitySet) : base(entitySet)
		{
		}

		public BirthdayCalendarReference(IBirthdayCalendars entitySet, string entityKey) : base(entitySet, entityKey)
		{
		}

		public BirthdayCalendarReference(IBirthdayCalendars entitySet, StoreId entityStoreId) : base(entitySet, entityStoreId)
		{
		}

		public IBirthdayEvents Events
		{
			get
			{
				return new BirthdayEvents(base.EntitySet);
			}
		}

		protected override StoreId ResolveReference()
		{
			ExTraceGlobals.BirthdayCalendarReferenceTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BirthdayCalendarReference::ResolveReference. MailboxGuid:{0}", base.StoreSession.MailboxGuid);
			return base.EntitySet.BirthdayCalendarFolderId;
		}
	}
}
