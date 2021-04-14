using System;

namespace Microsoft.Exchange.UnifiedContent
{
	internal interface IRawContent
	{
		string FileName { get; }

		long Rawsize { get; }

		string RawFileName { get; }

		long StreamOffset { get; }
	}
}
