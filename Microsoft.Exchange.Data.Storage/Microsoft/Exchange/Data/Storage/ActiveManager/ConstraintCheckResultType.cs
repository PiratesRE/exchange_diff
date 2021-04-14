using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum ConstraintCheckResultType
	{
		Retry,
		Satisfied,
		NotSatisfied = 3
	}
}
