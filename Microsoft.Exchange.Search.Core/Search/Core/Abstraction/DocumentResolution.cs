using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum DocumentResolution
	{
		IgnoreAndContinue,
		CompleteError,
		CompleteSuccess,
		PoisonComponentAndContinue
	}
}
