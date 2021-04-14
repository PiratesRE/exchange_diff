using System;

namespace Microsoft.Exchange.Data
{
	public interface IInvalidProxy
	{
		ArgumentOutOfRangeException ParseException { get; }
	}
}
