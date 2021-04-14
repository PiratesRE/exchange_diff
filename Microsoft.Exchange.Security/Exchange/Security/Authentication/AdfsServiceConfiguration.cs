using System;
using Microsoft.IdentityModel.Configuration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class AdfsServiceConfiguration : ServiceConfiguration
	{
		internal AdfsServiceConfiguration(ServiceElement element)
		{
			base.LoadConfiguration(element);
		}
	}
}
