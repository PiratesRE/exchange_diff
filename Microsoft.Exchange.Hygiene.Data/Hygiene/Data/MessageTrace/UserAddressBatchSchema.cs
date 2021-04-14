using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UserAddressBatchSchema
	{
		internal static readonly HygienePropertyDefinition BatchAddressesProperty = new HygienePropertyDefinition("tvp_UserAddressProperties", typeof(BatchPropertyTable));

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		public static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;
	}
}
