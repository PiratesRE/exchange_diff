using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPipelineComponentConfig
	{
		string this[string keyName]
		{
			get;
		}
	}
}
