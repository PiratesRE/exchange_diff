using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoIntegerProperty : XsoProperty, IIntegerProperty, IProperty
	{
		public XsoIntegerProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoIntegerProperty(StorePropertyDefinition propertyDef, bool nodelete) : base(propertyDef)
		{
			this.nodelete = nodelete;
		}

		public XsoIntegerProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoIntegerProperty(StorePropertyDefinition propertyDef, PropertyDefinition[] prefetchProps) : base(propertyDef, prefetchProps)
		{
		}

		public XsoIntegerProperty(StorePropertyDefinition propertyDef, PropertyType type, PropertyDefinition[] prefetchProps) : base(propertyDef, type, prefetchProps)
		{
		}

		public virtual int IntegerData
		{
			get
			{
				return (int)base.XsoItem.TryGetProperty(base.PropertyDef);
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = ((IIntegerProperty)srcProperty).IntegerData;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (this.nodelete)
			{
				return;
			}
			if (base.XsoItem.TryGetProperty(base.PropertyDef) is PropertyError)
			{
				return;
			}
			base.XsoItem.DeleteProperties(new PropertyDefinition[]
			{
				base.PropertyDef
			});
		}

		private bool nodelete;
	}
}
