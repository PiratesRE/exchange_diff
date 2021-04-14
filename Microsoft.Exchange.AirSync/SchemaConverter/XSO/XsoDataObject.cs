using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoDataObject : PropertyBase, IServerDataObject, IPropertyContainer, IProperty, IDataObjectGeneratorContainer
	{
		public XsoDataObject(IList<IProperty> propertyFromSchemaLinkId, IXsoDataObjectGenerator schemaState, QueryFilter supportedClassFilter)
		{
			if (propertyFromSchemaLinkId == null)
			{
				throw new ArgumentNullException("propertyFromSchemaLinkId");
			}
			int num = 0;
			foreach (IProperty property in propertyFromSchemaLinkId)
			{
				XsoProperty xsoProperty = (XsoProperty)property;
				PropertyDefinition[] array = xsoProperty.GetPrefetchProperties();
				if (array != null)
				{
					num += xsoProperty.GetPrefetchProperties().Length;
				}
			}
			this.prefetchProperties = new PropertyDefinition[num];
			int num2 = 0;
			foreach (IProperty property2 in propertyFromSchemaLinkId)
			{
				XsoProperty xsoProperty2 = (XsoProperty)property2;
				PropertyDefinition[] array2 = xsoProperty2.GetPrefetchProperties();
				if (array2 != null)
				{
					array2.CopyTo(this.prefetchProperties, num2);
					num2 += array2.Length;
				}
			}
			this.propertyFromSchemaLinkId = propertyFromSchemaLinkId;
			this.schemaState = schemaState;
			this.supportedClassFilter = supportedClassFilter;
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
				this.schemaState = (value as IXsoDataObjectGenerator);
			}
		}

		public QueryFilter SupportedClassFilter
		{
			get
			{
				return this.supportedClassFilter;
			}
		}

		public void Bind(object item)
		{
			this.areChildPropertyStatesSet = false;
			foreach (IProperty property in this.propertyFromSchemaLinkId)
			{
				XsoProperty xsoProperty = (XsoProperty)property;
				xsoProperty.Bind((StoreObject)item);
			}
		}

		public override void Unbind()
		{
			try
			{
				this.areChildPropertyStatesSet = false;
				foreach (IProperty property in this.propertyFromSchemaLinkId)
				{
					XsoProperty xsoProperty = (XsoProperty)property;
					xsoProperty.Unbind();
				}
			}
			finally
			{
				base.Unbind();
			}
		}

		public bool CanConvertItemClassUsingCurrentSchema(string itemClass)
		{
			SinglePropertyBag propertyBag = new SinglePropertyBag(StoreObjectSchema.ItemClass, itemClass);
			return EvaluatableFilter.Evaluate(this.SupportedClassFilter, propertyBag);
		}

		public PropertyDefinition[] GetPrefetchProperties()
		{
			return this.prefetchProperties;
		}

		public void SetCopyDestination(IPropertyContainer dstPropertyContainer)
		{
			if (!this.areChildPropertyStatesSet)
			{
				foreach (IProperty property in this.propertyFromSchemaLinkId)
				{
					XsoProperty xsoProperty = (XsoProperty)property;
					xsoProperty.ComputePropertyState();
				}
			}
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
				XsoProperty xsoProperty = this.GetPropBySchemaLinkId(property.SchemaLinkId) as XsoProperty;
				if (xsoProperty.Type != PropertyType.ReadOnly && xsoProperty.State != PropertyState.NotSupported)
				{
					if (xsoProperty is IDataObjectGeneratorContainer)
					{
						((IDataObjectGeneratorContainer)xsoProperty).SchemaState = this.schemaState;
						((IDataObjectGeneratorContainer)property).SchemaState = ((IDataObjectGeneratorContainer)propertyContainer).SchemaState;
					}
					xsoProperty.CopyFrom(property);
				}
			}
			foreach (IProperty property2 in propertyContainer.Children)
			{
				XsoProperty xsoProperty2 = this.GetPropBySchemaLinkId(property2.SchemaLinkId) as XsoProperty;
				if (xsoProperty2 != null && xsoProperty2.Type != PropertyType.ReadOnly && xsoProperty2.State != PropertyState.NotSupported && xsoProperty2.PostProcessingDelegate != null)
				{
					xsoProperty2.PostProcessingDelegate(property2, this.Children);
					xsoProperty2.PostProcessingDelegate = null;
				}
			}
		}

		public void SetChangedProperties(PropertyDefinition[] changedProperties)
		{
			if (changedProperties == null)
			{
				return;
			}
			this.areChildPropertyStatesSet = true;
			for (int i = 0; i < changedProperties.Length; i++)
			{
				foreach (IProperty property in this.propertyFromSchemaLinkId)
				{
					XsoProperty xsoProperty = (XsoProperty)property;
					if (xsoProperty.StorePropertyDefinition == changedProperties[i])
					{
						xsoProperty.State = PropertyState.Modified;
					}
					else
					{
						xsoProperty.State = PropertyState.Unmodified;
					}
				}
			}
		}

		public IProperty GetPropBySchemaLinkId(int id)
		{
			return this.propertyFromSchemaLinkId[id];
		}

		private readonly QueryFilter supportedClassFilter;

		private IList<IProperty> propertyFromSchemaLinkId;

		private IXsoDataObjectGenerator schemaState;

		private PropertyDefinition[] prefetchProperties;

		private bool areChildPropertyStatesSet;
	}
}
