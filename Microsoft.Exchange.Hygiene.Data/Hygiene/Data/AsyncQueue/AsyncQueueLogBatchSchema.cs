using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLogBatchSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;

		internal static readonly HygienePropertyDefinition LogProperty = new HygienePropertyDefinition("logProperties", typeof(object), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
