using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipeline : IDocumentProcessor, IStartStop, IDisposable, IDiagnosable
	{
		int MaxConcurrency { get; }
	}
}
