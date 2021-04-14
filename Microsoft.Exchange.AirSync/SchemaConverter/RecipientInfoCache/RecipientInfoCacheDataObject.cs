using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache
{
	internal class RecipientInfoCacheDataObject : RecipientInfoCacheProperty, IPropertyContainer, IProperty, IDataObjectGeneratorContainer
	{
		public RecipientInfoCacheDataObject(IList<IProperty> propertyFromSchemaLinkId)
		{
			base.State = PropertyState.Modified;
			this.propertyFromSchemaLinkId = propertyFromSchemaLinkId;
		}

		public IList<IProperty> Children
		{
			get
			{
				return this.propertyFromSchemaLinkId;
			}
		}

		public IDataObjectGenerator SchemaState
		{
			get
			{
				return this.schemaState;
			}
			set
			{
				this.schemaState = value;
			}
		}

		public override void Bind(RecipientInfoCacheEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("Entry is null!");
			}
			foreach (IProperty property in this.propertyFromSchemaLinkId)
			{
				RecipientInfoCacheProperty recipientInfoCacheProperty = (RecipientInfoCacheProperty)property;
				recipientInfoCacheProperty.Bind(entry);
			}
		}

		public void SetCopyDestination(IPropertyContainer dstPropertyContainer)
		{
		}

		public override void CopyFrom(IProperty srcRootProperty)
		{
			IPropertyContainer propertyContainer = srcRootProperty as IPropertyContainer;
			if (propertyContainer == null)
			{
				throw new ArgumentNullException("srcPropertyContainer");
			}
			propertyContainer.SetCopyDestination(this);
			foreach (IProperty property in propertyContainer.Children)
			{
				RecipientInfoCacheProperty recipientInfoCacheProperty = (RecipientInfoCacheProperty)this.propertyFromSchemaLinkId[property.SchemaLinkId];
				recipientInfoCacheProperty.CopyFrom(property);
			}
		}

		private IList<IProperty> propertyFromSchemaLinkId;

		private IDataObjectGenerator schemaState;
	}
}
