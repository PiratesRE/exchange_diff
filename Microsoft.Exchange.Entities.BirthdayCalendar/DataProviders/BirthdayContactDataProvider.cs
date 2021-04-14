using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.DataProviders
{
	internal class BirthdayContactDataProvider : StorageItemDataProvider<IStoreSession, BirthdayContact, IContact>
	{
		public BirthdayContactDataProvider(IStorageEntitySetScope<IStoreSession> scope, StoreId containerFolderId) : base(scope, containerFolderId, ExTraceGlobals.BirthdayEventDataProviderTracer)
		{
		}

		protected override IStorageTranslator<IContact, BirthdayContact> Translator
		{
			get
			{
				return BirthdayContactTranslator.Instance;
			}
		}

		public virtual IEnumerable<IBirthdayContact> GetLinkedContacts(PersonId personId)
		{
			base.Trace.TraceDebug<PersonId>((long)this.GetHashCode(), "GetLinkedContacts: the person ID is {0}", personId);
			foreach (IStorePropertyBag contactPropertyBag in AllPersonContactsEnumerator.Create(base.Session as MailboxSession, personId, BirthdayContactDataProvider.BirthdayContactPropertyBagProperties))
			{
				using (IContact contact = base.XsoFactory.BindToContact(base.Session, contactPropertyBag.GetValueOrDefault<StoreId>(CoreItemSchema.Id, null), BirthdayContactDataProvider.BirthdayContactProperties))
				{
					yield return this.ConvertToEntity(contact);
				}
			}
			base.Trace.TraceDebug<PersonId>((long)this.GetHashCode(), "GetLinkedContacts: no more contacts for person ID {0}", personId);
			yield break;
		}

		protected internal override IContact BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToContact(base.Session, id, BirthdayContactDataProvider.BirthdayContactProperties);
		}

		protected override IContact CreateNewStoreObject()
		{
			throw new NotImplementedException();
		}

		internal static readonly PropertyDefinition[] BirthdayContactProperties = new PropertyDefinition[]
		{
			CoreItemSchema.Id,
			StoreObjectSchema.DisplayName,
			ContactSchema.BirthdayLocal,
			ContactSchema.NotInBirthdayCalendar,
			ContactSchema.PersonId,
			ContactSchema.PartnerNetworkId,
			ContactSchema.IsWritable,
			ItemSchema.ParentDisplayName
		};

		internal static readonly PropertyDefinition[] BirthdayContactPropertyBagProperties = new PropertyDefinition[]
		{
			CoreItemSchema.Id,
			ContactSchema.PersonId
		};
	}
}
