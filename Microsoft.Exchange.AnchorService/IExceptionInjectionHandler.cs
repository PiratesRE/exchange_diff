using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.AnchorService
{
	public interface IExceptionInjectionHandler
	{
		ExceptionInjectionCallback Callback { get; }
	}
}
