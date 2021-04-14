using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal interface IBingError
	{
		string Code { get; }

		string Message { get; }
	}
}
