using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal class StorageEntitySetScope<TStoreSession> : IStorageEntitySetScope<TStoreSession> where TStoreSession : class, IStoreSession
	{
		public StorageEntitySetScope(TStoreSession storeSession, IRecipientSession recipientSession, IXSOFactory xsoFactory, IdConverter idConverter = null)
		{
			this.StoreSession = storeSession;
			this.RecipientSession = recipientSession;
			this.XsoFactory = xsoFactory;
			this.IdConverter = (idConverter ?? IdConverter.Instance);
		}

		public StorageEntitySetScope(IStorageEntitySetScope<TStoreSession> scope) : this(scope.StoreSession, scope.RecipientSession, scope.XsoFactory, scope.IdConverter)
		{
		}

		public IRecipientSession RecipientSession { get; private set; }

		public TStoreSession StoreSession { get; private set; }

		public IXSOFactory XsoFactory { get; private set; }

		public IdConverter IdConverter { get; private set; }

		public override string ToString()
		{
			TStoreSession storeSession = this.StoreSession;
			return storeSession.MailboxGuid.ToString();
		}
	}
}
