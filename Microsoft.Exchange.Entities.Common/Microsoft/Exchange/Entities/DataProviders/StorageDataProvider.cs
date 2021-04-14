using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.DataProviders
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class StorageDataProvider<TSession, TEntity, TId> : DataProvider<TEntity, TId> where TSession : class, IStoreSession where TEntity : IEntity
	{
		protected StorageDataProvider(IStorageEntitySetScope<TSession> scope, ITracer trace) : base(trace)
		{
			this.Scope = scope;
		}

		public IStorageEntitySetScope<TSession> Scope { get; private set; }

		protected IdConverter IdConverter
		{
			get
			{
				return this.Scope.IdConverter;
			}
		}

		protected TSession Session
		{
			get
			{
				return this.Scope.StoreSession;
			}
		}

		protected IXSOFactory XsoFactory
		{
			get
			{
				return this.Scope.XsoFactory;
			}
		}

		protected virtual SaveMode ConflictResolutionSaveMode
		{
			get
			{
				return SaveMode.ResolveConflicts;
			}
		}

		protected virtual IItem BindToItem(StoreId id)
		{
			return this.XsoFactory.BindToItem(this.Session, id, new PropertyDefinition[0]);
		}

		protected SaveMode GetSaveMode(string changeKeyInPayload, CommandContext commandContext)
		{
			if (commandContext != null && !string.IsNullOrEmpty(commandContext.IfMatchETag))
			{
				return SaveMode.FailOnAnyConflict;
			}
			if (!string.IsNullOrEmpty(changeKeyInPayload))
			{
				return this.ConflictResolutionSaveMode;
			}
			return SaveMode.NoConflictResolutionForceSave;
		}
	}
}
