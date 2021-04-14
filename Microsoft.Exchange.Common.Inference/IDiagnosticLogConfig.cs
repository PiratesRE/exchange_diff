using System;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public interface IDiagnosticLogConfig : ILogConfig
	{
		LoggingLevel LoggingLevel { get; }
	}
}
