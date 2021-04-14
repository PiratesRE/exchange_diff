using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.DocumentModel;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PipelineContext : PropertyBag, IPipelineContext, IReadOnlyPropertyBag
	{
	}
}
