using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal abstract class StorageItemDataProvider<TSession, TEntity, TStoreItem> : StorageObjectDataProvider<TSession, TEntity, TStoreItem> where TSession : class, IStoreSession where TEntity : IStorageEntity where TStoreItem : IItem
	{
		protected StorageItemDataProvider(IStorageEntitySetScope<TSession> scope, StoreId containerFolderId, ITracer trace) : base(scope, trace)
		{
			this.ContainerFolderId = containerFolderId;
		}

		public StoreId ContainerFolderId { get; private set; }

		public override TStoreItem BindToWrite(StoreId id, string changeKey)
		{
			TStoreItem result = base.BindToWrite(id, changeKey);
			if (string.IsNullOrEmpty(changeKey))
			{
				result.OpenAsReadWrite();
			}
			return result;
		}

		public virtual IEnumerable<TEntity> Find(QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] propertiesToLoad)
		{
			using (IFolder folder = this.BindToContainingFolder())
			{
				using (IQueryResult result = folder.IItemQuery(ItemQueryType.None, queryFilter, sortColumns, propertiesToLoad))
				{
					Dictionary<PropertyDefinition, int> propertyIndices = this.GetPropertyIndices(propertiesToLoad);
					object[][] rows;
					do
					{
						bool mightBeMoreRows;
						rows = result.GetRows(10, out mightBeMoreRows);
						IEnumerable<TEntity> currentBatch = this.ReadQueryResults(rows, propertyIndices);
						foreach (TEntity theEvent in currentBatch)
						{
							yield return theEvent;
						}
					}
					while (rows.Length > 0);
				}
			}
			yield break;
		}

		protected internal override void ValidateStoreObjectIdForCorrectType(StoreObjectId storeObjectId)
		{
			if (storeObjectId.IsFolderId)
			{
				string id = storeObjectId.ToString();
				throw new ObjectNotFoundException(Strings.CanNotUseFolderIdForItem(id));
			}
		}

		protected virtual IFolder BindToContainingFolder()
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(this.ContainerFolderId);
			if (storeObjectId == null)
			{
				throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter("ContainerFolderId"));
			}
			return base.XsoFactory.BindToFolder(base.Session, storeObjectId);
		}

		protected override void SaveAndCheckForConflicts(TStoreItem storeObject, SaveMode saveMode)
		{
			ConflictResolutionResult result = storeObject.Save(saveMode);
			result.ThrowOnIrresolvableConflict();
		}
	}
}
