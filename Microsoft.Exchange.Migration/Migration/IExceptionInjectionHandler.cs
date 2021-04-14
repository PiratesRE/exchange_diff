using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Migration
{
	public interface IExceptionInjectionHandler
	{
		ExceptionInjectionCallback Callback { get; }
	}
}
