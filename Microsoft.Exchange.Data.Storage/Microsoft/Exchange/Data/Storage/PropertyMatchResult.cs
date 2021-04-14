using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum PropertyMatchResult
	{
		Default,
		NotFound = 0,
		Found,
		TypeMismatch
	}
}
