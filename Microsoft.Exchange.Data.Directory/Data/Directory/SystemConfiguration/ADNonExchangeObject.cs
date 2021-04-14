using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class ADNonExchangeObject : ADConfigurationObject
	{
		internal override QueryFilter VersioningFilter
		{
			get
			{
				return null;
			}
		}
	}
}
