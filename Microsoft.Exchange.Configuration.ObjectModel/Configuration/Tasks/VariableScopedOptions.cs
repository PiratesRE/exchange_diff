using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Flags]
	public enum VariableScopedOptions
	{
		AllScope = 8,
		Constant = 2,
		None = 0,
		Private = 4,
		ReadOnly = 1,
		Unspecified = 16
	}
}
