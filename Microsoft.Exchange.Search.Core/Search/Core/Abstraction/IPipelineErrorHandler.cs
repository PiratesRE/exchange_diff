using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipelineErrorHandler
	{
		DocumentResolution HandleException(IPipelineComponent component, ComponentException exception);
	}
}
