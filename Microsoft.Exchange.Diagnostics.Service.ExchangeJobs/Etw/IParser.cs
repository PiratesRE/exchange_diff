using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	internal interface IParser
	{
		IEnumerable<Guid> Guids { get; }

		unsafe void Parse(EtwTraceNativeComponents.EVENT_RECORD* rawData);
	}
}
