using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class QuarantinedMessageRecipientBatchSchema
	{
		internal static readonly HygienePropertyDefinition BatchAddressesProperty = new HygienePropertyDefinition("batchProperties", typeof(BatchPropertyTable));

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		public static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;
	}
}
