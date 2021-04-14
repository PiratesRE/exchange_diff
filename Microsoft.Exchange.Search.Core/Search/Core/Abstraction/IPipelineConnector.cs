using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipelineConnector : IStartStop, IDisposable, IDiagnosable
	{
	}
}
