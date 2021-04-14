using System;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal interface IServiceLocator
	{
		TServiceType GetService<TServiceType>();
	}
}
