using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IStartStopPipelineComponent : IPipelineComponent, IDocumentProcessor, INotifyFailed, IStartStop, IDisposable
	{
	}
}
