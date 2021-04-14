using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal enum ReturnCode
	{
		ErrorSuccess,
		ErrorSuccessRebootRequired = 3010,
		ErrorBadConfiguration = 1610,
		ErrorInvalidParameter = 87,
		ErrorMoreData = 234,
		ErrorUnknownProduct = 1605,
		ErrorUnknownProperty = 1608
	}
}
