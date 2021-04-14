using System;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal enum LOCKTYPE
	{
		Write = 1,
		Exclusive,
		OnlyOnce = 4
	}
}
