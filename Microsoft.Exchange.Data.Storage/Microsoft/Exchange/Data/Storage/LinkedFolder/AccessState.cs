using System;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	internal enum AccessState : uint
	{
		Allowed = 1U,
		Denied,
		Pending
	}
}
