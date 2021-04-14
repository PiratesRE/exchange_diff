using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoBooleanProperty : XsoProperty, IBooleanProperty, IProperty
	{
		public XsoBooleanProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoBooleanProperty(StorePropertyDefinition propertyDef, bool nodelete) : base(propertyDef)
		{
			this.nodelete = nodelete;
		}

		public XsoBooleanProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoBooleanProperty(StorePropertyDefinition propertyDef, PropertyDefinition[] prefetchPropDef) : base(propertyDef, prefetchPropDef)
		{
		}

		public bool BooleanData
		{
			get
			{
				return (bool)base.XsoItem.TryGetProperty(base.PropertyDef);
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = ((IBooleanProperty)srcProperty).BooleanData;
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
