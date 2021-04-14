using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationMessagePersistableBase : MigrationPersistableBase
	{
		public override bool TryLoad(IMigrationDataProvider dataProvider, StoreObjectId id)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(id, "id");
			bool success = true;
			MigrationUtil.RunTimedOperation(delegate()
			{
				using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(dataProvider, id, this.InitializationPropertyDefinitions))
				{
					this.OrganizationId = dataProvider.OrganizationId;
					if (!this.InitializeFromMessageItem(migrationStoreObject))
					{
						success = false;
					}
					else
					{
						migrationStoreObject.Load(this.PropertyDefinitions);
						if (!this.ReadFromMessageItem(migrationStoreObject))
						{
							success = false;
						}
						else
						{
							this.LoadLinkedStoredObjects(migrationStoreObject, dataProvider);
							this.CheckVersion();
						}
					}
				}
			}, this);
			return success;
		}

		public IMigrationMessageItem FindMessageItem(IMigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(id, "id");
			return dataProvider.FindMessage(id, properties);
		}

		public IMigrationMessageItem FindMessageItem(IMigrationDataProvider dataProvider, PropertyDefinition[] properties)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			ExAssert.RetailAssert(base.StoreObjectId != null, "Need to persist the objects before trying to retrieve their storage object.");
			return this.FindMessageItem(dataProvider, base.StoreObjectId, properties);
		}

		public override IMigrationStoreObject FindStoreObject(IMigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties)
		{
			return this.FindMessageItem(dataProvider, id, properties);
		}

		protected override IMigrationStoreObject CreateStoreObject(IMigrationDataProvider dataProvider)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			return dataProvider.CreateMessage();
		}
	}
}
