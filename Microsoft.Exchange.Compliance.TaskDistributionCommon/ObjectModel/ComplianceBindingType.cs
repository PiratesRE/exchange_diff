using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal enum ComplianceBindingType : byte
	{
		UnknownType,
		ExchangeBinding,
		SharePointBinding,
		PublicFolderBinding
	}
}
