using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands;

namespace Microsoft.Exchange.Entities.BirthdayCalendar
{
	internal sealed class BirthdayAssistantBusinessLogic
	{
		public BirthdayEventCommandResult OnContactAdded(IBirthdayContact birthdayContact, IStoreSession storeSession)
		{
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug((long)this.GetHashCode(), "OnContactAdded");
			BirthdayEventCommandResult birthdayEventCommandResult = this.OnContactAdded(birthdayContact, new BirthdaysContainer(storeSession, null));
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug<BirthdayEventCommandResult>((long)this.GetHashCode(), "OnContactAdded: birthday event is <{0}>", birthdayEventCommandResult);
			return birthdayEventCommandResult;
		}

		public BirthdayEventCommandResult OnContactDeleted(StoreObjectId birthdayContactStoreObjectId, IStoreSession storeSession)
		{
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug((long)this.GetHashCode(), "OnContactDeleted");
			BirthdayEventCommandResult birthdayEventCommandResult = this.OnContactDeleted(birthdayContactStoreObjectId, new BirthdaysContainer(storeSession, null));
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug<BirthdayEventCommandResult>((long)this.GetHashCode(), "OnContactDeleted: birthday event was <{0}>", birthdayEventCommandResult);
			return birthdayEventCommandResult;
		}

		public BirthdayEventCommandResult OnContactModified(IBirthdayContact birthdayContact, IStoreSession storeSession)
		{
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug((long)this.GetHashCode(), "OnContactModified: started");
			BirthdaysContainer birthdaysContainer = new BirthdaysContainer(storeSession, null);
			BirthdayEventCommandResult result = this.OnContactModified(birthdayContact, birthdaysContainer);
			ExTraceGlobals.BirthdayAssistantBusinessLogicTracer.TraceDebug((long)this.GetHashCode(), "OnContactModified: finished");
			return result;
		}

		internal BirthdayEventCommandResult OnContactAdded(IBirthdayContact birthdayContact, IBirthdaysContainer birthdaysContainer)
		{
			return birthdaysContainer.Events.CreateBirthdayEventForContact(birthdayContact);
		}

		internal BirthdayEventCommandResult OnContactDeleted(StoreObjectId birthdayContactStoreObjectId, IBirthdaysContainer birthdaysContainer)
		{
			return birthdaysContainer.Events.DeleteBirthdayEventForContactId(birthdayContactStoreObjectId);
		}

		internal BirthdayEventCommandResult OnContactModified(IBirthdayContact birthdayContact, IBirthdaysContainer birthdaysContainer)
		{
			IBirthdayContactInternal birthdayContactInternal = birthdayContact as IBirthdayContactInternal;
			if (birthdayContactInternal == null)
			{
				throw new ArgumentException("Argument must implement IBirthdayContactInternal", "birthdayContact");
			}
			IEnumerable<IBirthdayContact> linkedContacts = birthdaysContainer.Contacts.GetLinkedContacts(birthdayContactInternal.PersonId);
			return birthdaysContainer.Events.UpdateBirthdaysForLinkedContacts(linkedContacts);
		}
	}
}
