using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal abstract class StorageFolderDataProvider<TSession, TEntity, TStoreFolder> : StorageObjectDataProvider<TSession, TEntity, TStoreFolder> where TSession : class, IStoreSession where TEntity : IStorageEntity, new() where TStoreFolder : IFolder
	{
		protected StorageFolderDataProvider(IStorageEntitySetScope<TSession> scope, ITracer trace) : base(scope, trace)
		{
		}

		protected override SaveMode ConflictResolutionSaveMode
		{
			get
			{
				return SaveMode.FailOnAnyConflict;
			}
		}

		protected internal override void ValidateStoreObjectIdForCorrectType(StoreObjectId storeObjectId)
		{
			if (!storeObjectId.IsFolderId)
			{
				string id = storeObjectId.ToString();
				throw new ObjectNotFoundException(Strings.CanNotUseFolderIdForItem(id));
			}
		}
	}
}
