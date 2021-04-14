using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal delegate T Resolver<T>(T originalValue, T currentValue, T updatedValue);
}
