using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal class DeleteBirthdayEventForContact : EntityCommand<IBirthdayEvents, IBirthdayEvent>, IBirthdayEventCommand
	{
		internal DeleteBirthdayEventForContact(StoreObjectId contactStoreObjectId, IBirthdayEvents scope)
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "DeleteBirthdayEventForContact:Constructor/store object ID");
			this.ContactStoreObjectId = contactStoreObjectId;
			this.Scope = scope;
		}

		internal DeleteBirthdayEventForContact(IBirthdayContact contact, IBirthdayEvents scope)
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "DeleteBirthdayEventForContact:Constructor/contact");
			IBirthdayContactInternal birthdayContactInternal = contact as IBirthdayContactInternal;
			if (birthdayContactInternal == null)
			{
				throw new ArgumentException("Contact has to implement IBirthdayContactInternal", "contact");
			}
			this.ContactStoreObjectId = StoreId.GetStoreObjectId(birthdayContactInternal.StoreId);
			this.Scope = scope;
		}

		public StoreObjectId ContactStoreObjectId { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return DeleteBirthdayEventForContact.DeleteBirthdayEventTracer;
			}
		}

		public BirthdayEventCommandResult ExecuteAndGetResult()
		{
			BirthdayEventCommandResult birthdayEventCommandResult = new BirthdayEventCommandResult();
			IBirthdayEvent birthdayEvent = base.Execute(null);
			if (birthdayEvent != null)
			{
				birthdayEventCommandResult.DeletedEvents.Add(birthdayEvent);
			}
			return birthdayEventCommandResult;
		}

		protected override IBirthdayEvent OnExecute()
		{
			return this.DeleteExistingBirthdayEventForContact(this.ContactStoreObjectId);
		}

		private IBirthdayEvent DeleteExistingBirthdayEventForContact(StoreObjectId birthdayContactStoreObjectId)
		{
			return this.Scope.BirthdayEventDataProvider.DeleteBirthdayEventForContact(birthdayContactStoreObjectId);
		}

		private static readonly ITracer DeleteBirthdayEventTracer = ExTraceGlobals.DeleteBirthdayEventForContactTracer;
	}
}
