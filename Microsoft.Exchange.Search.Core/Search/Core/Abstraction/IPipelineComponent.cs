using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipelineComponent : IDocumentProcessor, INotifyFailed
	{
		string Name { get; }

		string Description { get; }
	}
}
