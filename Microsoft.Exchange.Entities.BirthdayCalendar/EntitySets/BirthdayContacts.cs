using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar.DataProviders;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal class BirthdayContacts : StorageEntitySet<IBirthdayContacts, IBirthdayContact, IStoreSession>, IBirthdayContacts, IEntitySet<IBirthdayContact>, IStorageEntitySetScope<IStoreSession>
	{
		public BirthdayContacts(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope, "BirthdayContacts", new SimpleCrudNotSupportedCommandFactory<IBirthdayContacts, IBirthdayContact>())
		{
		}

		public BirthdayContactDataProvider BirthdayContactDataProvider
		{
			get
			{
				BirthdayContactDataProvider result;
				if ((result = this.contactDataProvider) == null)
				{
					result = (this.contactDataProvider = new BirthdayContactDataProvider(this, null));
				}
				return result;
			}
			set
			{
				this.contactDataProvider = value;
			}
		}

		public IEnumerable<IBirthdayContact> GetLinkedContacts(PersonId personId)
		{
			return this.BirthdayContactDataProvider.GetLinkedContacts(personId);
		}

		private BirthdayContactDataProvider contactDataProvider;
	}
}
