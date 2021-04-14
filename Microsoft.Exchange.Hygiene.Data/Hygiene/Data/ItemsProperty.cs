using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal abstract class ItemsProperty : ConfigurablePropertyBag
	{
		public ItemsProperty(DataTable items)
		{
			this.Items = items;
		}

		public ItemsProperty()
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable Items
		{
			get
			{
				return (DataTable)this[ItemsProperty.ItemsTableProp];
			}
			set
			{
				this[ItemsProperty.ItemsTableProp] = value;
			}
		}

		public static readonly HygienePropertyDefinition ItemsTableProp = new HygienePropertyDefinition("tvp_Items", typeof(DataTable));
	}
}
