using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AnchorPersistableBase : IAnchorPersistable, IAnchorSerializable
	{
		protected AnchorPersistableBase(AnchorContext context)
		{
			this.AnchorContext = context;
			this.ExtendedProperties = new PersistableDictionary();
		}

		public StoreObjectId StoreObjectId { get; protected set; }

		public ExDateTime CreationTime { get; protected set; }

		public PersistableDictionary ExtendedProperties { get; protected set; }

		public virtual PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return AnchorPersistableBase.MigrationBaseDefinitions;
			}
		}

		protected AnchorContext AnchorContext { get; set; }

		public bool TryLoad(IAnchorDataProvider dataProvider, StoreObjectId id)
		{
			return this.TryLoad(dataProvider, id, null);
		}

		public virtual bool TryLoad(IAnchorDataProvider dataProvider, StoreObjectId id, Action<IAnchorStoreObject> streamAction)
		{
			AnchorUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			AnchorUtil.ThrowOnNullArgument(id, "id");
			bool success = true;
			AnchorUtil.RunTimedOperation(dataProvider.AnchorContext, delegate()
			{
				using (IAnchorStoreObject anchorStoreObject = this.FindStoreObject(dataProvider, id, this.PropertyDefinitions))
				{
					if (anchorStoreObject.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationPersistableDictionary, null) == null)
					{
						success = false;
					}
					else
					{
						anchorStoreObject.Load(this.PropertyDefinitions);
						if (!this.ReadFromMessageItem(anchorStoreObject))
						{
							success = false;
						}
						else
						{
							if (streamAction != null)
							{
								streamAction(anchorStoreObject);
							}
							this.LoadLinkedStoredObjects(anchorStoreObject, dataProvider);
						}
					}
				}
			}, this);
			return success;
		}

		public virtual bool ReadFromMessageItem(IAnchorStoreObject message)
		{
			AnchorUtil.ThrowOnNullArgument(message, "message");
			this.ExtendedProperties = AnchorHelper.GetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationPersistableDictionary, true);
			this.ReadStoreObjectIdProperties(message);
			return true;
		}

		public virtual void CreateInStore(IAnchorDataProvider dataProvider, Action<IAnchorStoreObject> streamAction)
		{
			AnchorUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			AnchorUtil.AssertOrThrow(this.StoreObjectId == null, "Object should not have been created already.", new object[0]);
			using (IAnchorStoreObject anchorStoreObject = this.CreateStoreObject(dataProvider))
			{
				if (streamAction != null)
				{
					streamAction(anchorStoreObject);
				}
				this.WriteToMessageItem(anchorStoreObject, false);
				anchorStoreObject.Save(SaveMode.FailOnAnyConflict);
				anchorStoreObject.Load(this.PropertyDefinitions);
				this.ReadStoreObjectIdProperties(anchorStoreObject);
			}
		}

		public virtual void WriteToMessageItem(IAnchorStoreObject message, bool loaded)
		{
			AnchorUtil.ThrowOnNullArgument(message, "message");
			this.WriteExtendedPropertiesToMessageItem(message);
		}

		public abstract IAnchorStoreObject FindStoreObject(IAnchorDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties);

		public IAnchorStoreObject FindStoreObject(IAnchorDataProvider dataProvider, PropertyDefinition[] properties)
		{
			ExAssert.RetailAssert(this.StoreObjectId != null, "Need to persist the objects before trying to retrieve their storage object.");
			if (properties == null)
			{
				properties = this.PropertyDefinitions;
			}
			return this.FindStoreObject(dataProvider, this.StoreObjectId, properties);
		}

		public IAnchorStoreObject FindStoreObject(IAnchorDataProvider dataProvider)
		{
			return this.FindStoreObject(dataProvider, null);
		}

		protected virtual void LoadLinkedStoredObjects(IAnchorStoreObject item, IAnchorDataProvider dataProvider)
		{
		}

		protected abstract IAnchorStoreObject CreateStoreObject(IAnchorDataProvider dataProvider);

		protected virtual void SaveExtendedProperties(IAnchorDataProvider provider)
		{
			AnchorUtil.AssertOrThrow(this.StoreObjectId != null, "Object should have been created before trying to save properties.", new object[0]);
			using (IAnchorStoreObject anchorStoreObject = this.FindStoreObject(provider, AnchorPersistableBase.MigrationBaseDefinitions))
			{
				anchorStoreObject.OpenAsReadWrite();
				this.WriteExtendedPropertiesToMessageItem(anchorStoreObject);
				anchorStoreObject.Save(SaveMode.FailOnAnyConflict);
			}
		}

		protected virtual void WriteExtendedPropertiesToMessageItem(IAnchorStoreObject message)
		{
			AnchorHelper.SetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationPersistableDictionary, this.ExtendedProperties);
		}

		protected void ReadStoreObjectIdProperties(IAnchorStoreObject storeObject)
		{
			this.StoreObjectId = storeObject.Id;
			this.CreationTime = storeObject.CreationTime;
		}

		private static object GetDiagnosticObject(object obj)
		{
			byte[] array = obj as byte[];
			if (array != null)
			{
				try
				{
					obj = new Guid(array);
				}
				catch (ArgumentException)
				{
				}
			}
			return obj;
		}

		protected static readonly PropertyDefinition[] MigrationBaseDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationPersistableDictionary
		};
	}
}
