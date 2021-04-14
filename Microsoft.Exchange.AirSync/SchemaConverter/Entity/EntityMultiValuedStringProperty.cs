using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityMultiValuedStringProperty : EntityProperty, IMultivaluedProperty<string>, IProperty, IEnumerable<string>, IEnumerable
	{
		public EntityMultiValuedStringProperty(EntityPropertyDefinition propertyDefinition) : base(propertyDefinition, PropertyType.ReadWrite, false)
		{
		}

		public int Count
		{
			get
			{
				if (this.values == null)
				{
					return 0;
				}
				return this.values.Count;
			}
		}

		public override void Bind(IItem item)
		{
			base.Bind(item);
			if (base.EntityPropertyDefinition.Getter == null)
			{
				throw new ConversionException("Unable to retrieve data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			this.values = (List<string>)base.EntityPropertyDefinition.Getter(base.Item);
		}

		public override void Unbind()
		{
			try
			{
				this.values = null;
			}
			finally
			{
				base.Unbind();
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			if (this.values == null)
			{
				return null;
			}
			return this.values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			IMultivaluedProperty<string> multivaluedProperty = srcProperty as IMultivaluedProperty<string>;
			if (multivaluedProperty != null)
			{
				List<string> list = new List<string>(multivaluedProperty.Count);
				list.AddRange(multivaluedProperty);
				base.EntityPropertyDefinition.Setter(base.Item, list);
			}
		}

		private IList<string> values;
	}
}
