using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum EcpFeatureSet
	{
		AdminEnabledMask = 1,
		OwaOptionsEnabledMask = 2
	}
}
