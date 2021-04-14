using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Reporting;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class TenantThrottleInfoBatchSchema
	{
		public static readonly HygienePropertyDefinition PhysicalInstanceKeyProp = DalHelper.PhysicalInstanceKeyProp;

		public static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;

		internal static readonly HygienePropertyDefinition TenantThrottleInfoListProperty = new HygienePropertyDefinition("batchProperties", typeof(TenantThrottleInfo), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
