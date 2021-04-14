using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal abstract class StorageEntitySet<TEntitySet, TEntity, TCommandFactory, TSession> : EntitySet<TEntitySet, TEntity, TCommandFactory>, IStorageEntitySetScope<TSession> where TEntitySet : class, IEntitySet<TEntity> where TEntity : class, IEntity where TCommandFactory : IEntityCommandFactory<TEntitySet, TEntity> where TSession : class, IStoreSession
	{
		internal StorageEntitySet(IStorageEntitySetScope<TSession> parentScope, string relativeName, TCommandFactory commandFactory) : base(commandFactory)
		{
			this.description = string.Format("{0}.{1}", parentScope, relativeName);
			this.Session = parentScope.StoreSession;
			this.StoreSession = this.Session;
			this.XsoFactory = parentScope.XsoFactory;
			this.RecipientSession = parentScope.RecipientSession;
			this.IdConverter = parentScope.IdConverter;
		}

		public IRecipientSession RecipientSession { get; private set; }

		public TSession Session { get; private set; }

		public TSession StoreSession { get; private set; }

		public IXSOFactory XsoFactory { get; private set; }

		public IdConverter IdConverter { get; private set; }

		public sealed override string ToString()
		{
			return this.description;
		}

		private readonly string description;
	}
}
