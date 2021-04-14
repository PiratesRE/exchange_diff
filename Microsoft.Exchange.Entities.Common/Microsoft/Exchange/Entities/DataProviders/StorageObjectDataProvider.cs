using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.DataProviders
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class StorageObjectDataProvider<TSession, TEntity, TStoreObject> : StorageDataProvider<TSession, TEntity, StoreId> where TSession : class, IStoreSession where TEntity : IStorageEntity where TStoreObject : IStoreObject, IDisposable
	{
		protected StorageObjectDataProvider(IStorageEntitySetScope<TSession> scope, ITracer trace) : base(scope, trace)
		{
		}

		public event EventHandler<TStoreObject> StoreObjectSaved;

		public event Action<TEntity, TStoreObject> BeforeStoreObjectSaved;

		protected abstract IStorageTranslator<TStoreObject, TEntity> Translator { get; }

		public virtual TStoreObject Bind(StoreId id)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			if (storeObjectId != null)
			{
				this.ValidateStoreObjectIdForCorrectType(storeObjectId);
			}
			TStoreObject result;
			try
			{
				result = this.BindToStoreObject(id);
			}
			catch (WrongObjectTypeException innerException)
			{
				throw new ObjectNotFoundException(Strings.ItemWithGivenIdNotFound(id.ToString()), innerException);
			}
			return result;
		}

		public virtual TStoreObject BindToWrite(StoreId id, string changeKey)
		{
			return this.Bind(id);
		}

		public override TEntity Create(TEntity entity)
		{
			this.Validate(entity, true);
			TEntity result;
			using (TStoreObject tstoreObject = this.CreateNewStoreObject())
			{
				result = this.Update(entity, tstoreObject, SaveMode.NoConflictResolution);
			}
			return result;
		}

		public override void Delete(StoreId id, DeleteItemFlags flags)
		{
			AggregateOperationResult aggregateOperationResult;
			try
			{
				TSession session = base.Session;
				aggregateOperationResult = session.Delete(flags, new StoreId[]
				{
					id
				});
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new AccessDeniedException(Strings.ErrorAccessDenied, innerException);
			}
			if (aggregateOperationResult.OperationResult == OperationResult.Failed)
			{
				GroupOperationResult groupOperationResult = aggregateOperationResult.GroupOperationResults.First((GroupOperationResult singleResult) => singleResult.OperationResult == OperationResult.Failed);
				throw groupOperationResult.Exception;
			}
		}

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> MapProperties(IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> properties)
		{
			return this.Translator.Map(properties);
		}

		public override TEntity Read(StoreId id)
		{
			TEntity result;
			using (TStoreObject tstoreObject = this.Bind(id))
			{
				result = this.ConvertToEntity(tstoreObject);
			}
			return result;
		}

		public override TEntity Update(TEntity entity, CommandContext commandContext)
		{
			TEntity result;
			using (TStoreObject tstoreObject = this.ValidateAndBindToWrite(entity))
			{
				result = this.Update(entity, tstoreObject, commandContext);
			}
			return result;
		}

		public virtual TEntity Update(TEntity sourceEntity, TStoreObject targetStoreObject, CommandContext commandContext)
		{
			SaveMode saveMode = base.GetSaveMode(sourceEntity.ChangeKey, commandContext);
			return this.Update(sourceEntity, targetStoreObject, saveMode);
		}

		public virtual TEntity Update(TEntity sourceEntity, TStoreObject targetStoreObject, SaveMode saveMode)
		{
			this.UpdateOnly(sourceEntity, targetStoreObject, saveMode);
			return this.ConvertToEntity(targetStoreObject);
		}

		public virtual void UpdateOnly(TEntity sourceEntity, StoreId targetStoreObjectId)
		{
			using (TStoreObject tstoreObject = this.BindToWrite(targetStoreObjectId, null))
			{
				this.UpdateOnly(sourceEntity, tstoreObject, SaveMode.NoConflictResolution);
			}
		}

		public virtual void UpdateOnly(TEntity sourceEntity, TStoreObject targetStoreObject, SaveMode saveMode)
		{
			this.ValidateAndApplyChanges(sourceEntity, targetStoreObject);
			this.OnBeforeStoreObjectSaved(sourceEntity, targetStoreObject);
			this.SaveAndCheckForConflicts(targetStoreObject, saveMode);
			this.LoadStoreObject(targetStoreObject);
			this.OnStoreObjectSaved(targetStoreObject);
		}

		public virtual TStoreObject ValidateAndBindToWrite(TEntity entity)
		{
			this.Validate(entity, false);
			StoreId storeId = base.IdConverter.GetStoreId(entity);
			return this.BindToWrite(storeId, entity.ChangeKey);
		}

		public virtual TEntity ConvertToEntity(TStoreObject storeObject)
		{
			return this.Translator.ConvertToEntity(storeObject);
		}

		protected internal abstract TStoreObject BindToStoreObject(StoreId id);

		protected internal abstract void ValidateStoreObjectIdForCorrectType(StoreObjectId storeObjectId);

		protected virtual void LoadStoreObject(TStoreObject storeObject)
		{
			storeObject.Load();
		}

		protected virtual void ValidateChanges(TEntity sourceEntity, TStoreObject targetStoreObject)
		{
		}

		protected virtual void ValidateAndApplyChanges(TEntity sourceEntity, TStoreObject targetStoreObject)
		{
			this.ValidateChanges(sourceEntity, targetStoreObject);
			this.Translator.SetPropertiesFromEntityOnStorageObject(sourceEntity, targetStoreObject);
		}

		protected abstract TStoreObject CreateNewStoreObject();

		protected virtual Dictionary<Microsoft.Exchange.Data.PropertyDefinition, int> GetPropertyIndices(Microsoft.Exchange.Data.PropertyDefinition[] loadedProperties)
		{
			int index = 0;
			return loadedProperties.ToDictionary((Microsoft.Exchange.Data.PropertyDefinition property) => property, (Microsoft.Exchange.Data.PropertyDefinition property) => index++);
		}

		protected virtual IEnumerable<TEntity> ReadQueryResults(object[][] rows, Dictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices)
		{
			return from row in rows
			select this.Translator.ConvertToEntity(propertyIndices, row, this.Session);
		}

		protected abstract void SaveAndCheckForConflicts(TStoreObject storeObject, SaveMode saveMode);

		protected void OnStoreObjectSaved(TStoreObject storeObject)
		{
			EventHandler<TStoreObject> storeObjectSaved = this.StoreObjectSaved;
			if (storeObjectSaved != null)
			{
				storeObjectSaved(this, storeObject);
			}
		}

		protected void OnBeforeStoreObjectSaved(TEntity sourceEntity, TStoreObject storeObject)
		{
			Action<TEntity, TStoreObject> beforeStoreObjectSaved = this.BeforeStoreObjectSaved;
			if (beforeStoreObjectSaved != null)
			{
				beforeStoreObjectSaved(sourceEntity, storeObject);
			}
		}
	}
}
