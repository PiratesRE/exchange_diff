using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class ReadFlagDataObject : PropertyBase, IPropertyContainer, IProperty
	{
		public ReadFlagDataObject(IList<IProperty> propertyFromSchemaLinkId, SyncOperation syncOp)
		{
			if (syncOp == null)
			{
				throw new ArgumentNullException("syncOp");
			}
			if (propertyFromSchemaLinkId == null)
			{
				throw new ArgumentNullException("propertyFromSchemaLinkId");
			}
			this.propertyFromSchemaLinkId = propertyFromSchemaLinkId;
			ReadFlagProperty readFlagProperty = new ReadFlagProperty(syncOp.IsRead);
			this.propertyList = new ReadFlagProperty[]
			{
				readFlagProperty
			};
		}

		public IList<IProperty> Children
		{
			get
			{
				return this.propertyList;
			}
		}

		public void SetChangedProperties(PropertyDefinition[] changedProperties)
		{
			if (changedProperties == null)
			{
				return;
			}
			if (changedProperties != SyncCollection.ReadFlagChangedOnly)
			{
				throw new ArgumentException("The only acceptable value for changedProperties is SyncCollection.ReadFlagChangedOnly", "changedProperties");
			}
			foreach (IProperty property in this.propertyFromSchemaLinkId)
			{
				XsoProperty xsoProperty = (XsoProperty)property;
				if (xsoProperty.StorePropertyDefinition == changedProperties[0])
				{
					this.propertyList[0].State = PropertyState.Modified;
					this.propertyList[0].SchemaLinkId = xsoProperty.SchemaLinkId;
					break;
				}
			}
		}

		public override void CopyFrom(IProperty srcRootProperty)
		{
			throw new InvalidOperationException("ReadFlagDataObject is read-only, thus CopyFrom is not implemented.");
		}

		public void SetCopyDestination(IPropertyContainer dstPropertyContainer)
		{
		}

		private IProperty[] propertyList;

		private IList<IProperty> propertyFromSchemaLinkId;
	}
}
