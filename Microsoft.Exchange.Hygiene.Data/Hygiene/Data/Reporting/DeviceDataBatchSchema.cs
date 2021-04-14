using System;
using System.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class DeviceDataBatchSchema
	{
		internal static readonly HygienePropertyDefinition DeviceDataTableProperty = new HygienePropertyDefinition("deviceProperties", typeof(DataTable));
	}
}
