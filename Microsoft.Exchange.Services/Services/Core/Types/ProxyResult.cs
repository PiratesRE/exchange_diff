using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal enum ProxyResult
	{
		Success,
		Failure,
		SoapFault,
		ExpiredToken
	}
}
