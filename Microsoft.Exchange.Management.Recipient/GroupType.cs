using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Flags]
	public enum GroupType
	{
		Distribution = 0,
		Security = -2147483648
	}
}
