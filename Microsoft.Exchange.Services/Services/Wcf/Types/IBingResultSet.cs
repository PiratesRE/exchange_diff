using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal interface IBingResultSet
	{
		IBingResult[] Results { get; }

		IBingError[] Errors { get; }
	}
}
