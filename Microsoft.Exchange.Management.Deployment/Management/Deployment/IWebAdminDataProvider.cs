using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IWebAdminDataProvider
	{
		bool Enable32BitAppOnWin64 { get; }
	}
}
