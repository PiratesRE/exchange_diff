using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal class UpdateBirthdayEventForContact : EntityCommand<IBirthdayEvents, IBirthdayEvent>, IBirthdayEventCommand
	{
		internal UpdateBirthdayEventForContact(IBirthdayEvent birthdayEvent, IBirthdayContact birthdayContact, IBirthdayEvents scope)
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "UpdateBirthdayEventForContact:Constructor");
			this.BirthdayContact = birthdayContact;
			this.BirthdayEvent = birthdayEvent;
			this.Scope = scope;
		}

		public IBirthdayEvent BirthdayEvent { get; private set; }

		public IBirthdayContact BirthdayContact { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return UpdateBirthdayEventForContact.UpdateBirthdayEventsForContactTracer;
			}
		}

		private BirthdayEventCommandResult Result { get; set; }

		public BirthdayEventCommandResult ExecuteAndGetResult()
		{
			base.Execute(null);
			return this.Result;
		}

		protected override IBirthdayEvent OnExecute()
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "Updating birthday for contact");
			this.Result = new BirthdayEventCommandResult();
			IBirthdayEventInternal birthdayEventInternal = this.BirthdayEvent as IBirthdayEventInternal;
			if (birthdayEventInternal == null)
			{
				throw new ArgumentException("Was not expecting a null internal birthday event");
			}
			IBirthdayContactInternal birthdayContactInternal = this.BirthdayContact as IBirthdayContactInternal;
			if (birthdayContactInternal == null)
			{
				throw new ArgumentException("Was not expecting a null internal birthday contact");
			}
			if (UpdateBirthdayEventForContact.ShouldEventBeUpdated(birthdayEventInternal, birthdayContactInternal))
			{
				this.Scope.BirthdayEventDataProvider.Delete(birthdayEventInternal.StoreId, DeleteItemFlags.HardDelete);
				this.Result.DeletedEvents.Add(this.BirthdayEvent);
				this.Result.MergeWith(this.Scope.CreateBirthdayEventForContact(this.BirthdayContact));
			}
			PersonId personId = birthdayEventInternal.PersonId;
			if (personId != null && !personId.Equals(birthdayContactInternal.PersonId))
			{
				IEnumerable<IBirthdayContact> linkedContacts = this.Scope.ParentScope.ParentScope.Contacts.GetLinkedContacts(personId);
				this.Result.MergeWith(this.Scope.UpdateBirthdaysForLinkedContacts(linkedContacts));
			}
			return this.Result.CreatedEvents.FirstOrDefault<IBirthdayEvent>();
		}

		private static bool ShouldEventBeUpdated(IBirthdayEventInternal birthdayEvent, IBirthdayContactInternal birthdayContact)
		{
			if (birthdayEvent == null)
			{
				throw new ArgumentNullException("birthdayEvent");
			}
			if (birthdayContact == null)
			{
				throw new ArgumentNullException("birthdayContact");
			}
			if (!birthdayEvent.ContactId.Equals(StoreId.GetStoreObjectId(birthdayContact.StoreId)))
			{
				throw new ArgumentException("Birthday event and birthday contact should have the same contact IDs", "birthdayEvent");
			}
			bool flag = birthdayEvent.Subject != birthdayContact.DisplayName;
			bool flag2 = birthdayEvent.Birthday != birthdayContact.Birthday;
			bool flag3 = !birthdayEvent.PersonId.Equals(birthdayContact.PersonId);
			bool flag4 = birthdayEvent.Attribution != birthdayContact.Attribution;
			UpdateBirthdayEventForContact.UpdateBirthdayEventsForContactTracer.TraceDebug(0L, "Differences: subject - {0}, birthday - {1}, person ID - {2}, attribution - {3}", new object[]
			{
				flag,
				flag2,
				flag3,
				flag4
			});
			return flag || flag2 || flag3 || flag4;
		}

		private static readonly ITracer UpdateBirthdayEventsForContactTracer = ExTraceGlobals.UpdateBirthdayEventForContactTracer;
	}
}
