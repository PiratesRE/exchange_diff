using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class DeviceDataBatch : ConfigurablePropertyBag
	{
		public DeviceDataBatch(IEnumerable<DeviceData> batch)
		{
			this.DeviceProperties = DalHelper.CreateDataTable(DeviceDataBatch.DevicePropertiesTvp, DeviceData.propertydefinitions, batch);
		}

		internal DeviceDataBatch(DataTable items)
		{
			this.DeviceProperties = items;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable DeviceProperties
		{
			get
			{
				return (DataTable)this[DeviceDataBatchSchema.DeviceDataTableProperty];
			}
			private set
			{
				this[DeviceDataBatchSchema.DeviceDataTableProperty] = value;
			}
		}

		internal static readonly string DevicePropertiesTvp = "deviceProperties";

		private static readonly HygienePropertyDefinition[] DataTableProperties = new HygienePropertyDefinition[]
		{
			DeviceDataBatchSchema.DeviceDataTableProperty
		};
	}
}
