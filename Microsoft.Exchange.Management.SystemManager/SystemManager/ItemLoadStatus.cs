using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public enum ItemLoadStatus
	{
		[LocDescription(Strings.IDs.Loading)]
		Loading,
		[LocDescription(Strings.IDs.ResolverObjectNotFound)]
		Failed
	}
}
