using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncDataObject : PropertyBase, IPropertyContainer, IProperty, IDataObjectGeneratorContainer
	{
		public AirSyncDataObject(IList<IProperty> propertyFromSchemaLinkId, IAirSyncMissingPropertyStrategy missingPropStrategy, IAirSyncDataObjectGenerator schemaState)
		{
			if (propertyFromSchemaLinkId == null)
			{
				throw new ArgumentNullException("propertyFromSchemaLinkId");
			}
			if (missingPropStrategy == null)
			{
				throw new ArgumentNullException("missingPropStrategy");
			}
			this.missingPropStrategy = missingPropStrategy;
			this.propertyFromSchemaLinkId = propertyFromSchemaLinkId;
			this.schemaState = schemaState;
			this.propertyFromAirSyncTag = new Dictionary<string, AirSyncProperty>(propertyFromSchemaLinkId.Count);
			for (int i = 0; i < propertyFromSchemaLinkId.Count; i++)
			{
				AirSyncProperty airSyncProperty = (AirSyncProperty)propertyFromSchemaLinkId[i];
				if (airSyncProperty == null)
				{
					throw new ArgumentNullException("airSyncProperty");
				}
				if (airSyncProperty.AirSyncTagNames == null)
				{
					throw new ArgumentNullException("airSyncProperty.AirSyncTagNames");
				}
				for (int j = 0; j < airSyncProperty.AirSyncTagNames.Length; j++)
				{
					if (airSyncProperty.AirSyncTagNames[j] != null)
					{
						this.propertyFromAirSyncTag[airSyncProperty.AirSyncTagNames[j]] = airSyncProperty;
					}
				}
			}
			this.missingPropStrategy.Validate(this);
		}

		private AirSyncDataObject()
		{
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
				this.schemaState = (value as IAirSyncDataObjectGenerator);
			}
		}

		public AirSyncProperty this[string airSyncTagName]
		{
			get
			{
				return this.propertyFromAirSyncTag[airSyncTagName];
			}
		}

		public void Bind(XmlNode xmlItemRoot)
		{
			if (xmlItemRoot == null)
			{
				throw new ArgumentNullException("xmlItemRoot");
			}
			foreach (object obj in xmlItemRoot.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					throw new ConversionException("Unexpected node type, should have been caught in a higher validation layer");
				}
				AirSyncProperty airSyncProperty = null;
				if (!this.propertyFromAirSyncTag.TryGetValue(xmlNode.Name, out airSyncProperty))
				{
					throw new ConversionException("Cannot bind property to XML node " + xmlNode.Name + ", property does not exist");
				}
				airSyncProperty.Bind(xmlNode);
			}
			foreach (AirSyncProperty airSyncProperty2 in this.propertyFromAirSyncTag.Values)
			{
				if (airSyncProperty2.State == PropertyState.Uninitialized)
				{
					airSyncProperty2.BindToParent(xmlItemRoot);
				}
			}
			this.missingPropStrategy.PostProcessPropertyBag(this);
		}

		public override void CopyFrom(IProperty srcRootProperty)
		{
			IPropertyContainer propertyContainer = srcRootProperty as IPropertyContainer;
			if (propertyContainer == null)
			{
				throw new UnexpectedTypeException("IPropertyContainer", srcRootProperty);
			}
			propertyContainer.SetCopyDestination(this);
			foreach (IProperty property in propertyContainer.Children)
			{
				if (property.State != PropertyState.NotSupported)
				{
					AirSyncProperty propBySchemaLinkId = this.GetPropBySchemaLinkId(property.SchemaLinkId);
					if (propBySchemaLinkId is IDataObjectGeneratorContainer)
					{
						((IDataObjectGeneratorContainer)propBySchemaLinkId).SchemaState = this.schemaState;
						((IDataObjectGeneratorContainer)property).SchemaState = ((IDataObjectGeneratorContainer)srcRootProperty).SchemaState;
					}
					this.missingPropStrategy.ExecuteCopyProperty(property, propBySchemaLinkId);
				}
			}
			foreach (IProperty property2 in propertyContainer.Children)
			{
				if (property2.State != PropertyState.NotSupported)
				{
					AirSyncProperty propBySchemaLinkId2 = this.GetPropBySchemaLinkId(property2.SchemaLinkId);
					if (propBySchemaLinkId2 != null && propBySchemaLinkId2.PostProcessingDelegate != null)
					{
						propBySchemaLinkId2.PostProcessingDelegate(property2, this.Children);
						propBySchemaLinkId2.PostProcessingDelegate = null;
					}
				}
			}
		}

		public void SetCopyDestination(IPropertyContainer dstPropertyContainer)
		{
		}

		public AirSyncProperty TryGetAirSyncPropertyByName(string propertyName)
		{
			AirSyncProperty result = null;
			if (this.propertyFromAirSyncTag.TryGetValue(propertyName, out result))
			{
				return result;
			}
			return null;
		}

		public override void Unbind()
		{
			base.Unbind();
			foreach (AirSyncProperty airSyncProperty in this.propertyFromAirSyncTag.Values)
			{
				airSyncProperty.Unbind();
			}
		}

		private AirSyncProperty GetPropBySchemaLinkId(int schemaLinkId)
		{
			return (AirSyncProperty)this.propertyFromSchemaLinkId[schemaLinkId];
		}

		private IAirSyncMissingPropertyStrategy missingPropStrategy;

		private Dictionary<string, AirSyncProperty> propertyFromAirSyncTag;

		private IList<IProperty> propertyFromSchemaLinkId;

		private IAirSyncDataObjectGenerator schemaState;
	}
}
