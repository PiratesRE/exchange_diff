using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Flags]
	public enum FailureMode
	{
		Transient = 1,
		Permanent = 2,
		All = 3
	}
}
