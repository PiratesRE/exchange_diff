using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IShadowRedundancyComponent
	{
		IShadowRedundancyManagerFacade ShadowRedundancyManager { get; }
	}
}
